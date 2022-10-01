using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using UnityEngine;
using UnityEngine.UI;

public enum DataGetState
{
    Login = 0,
    MarketPurchase = 1,
    RefillPurchase = 2,
    AfterFight = 3,
    AfterMerge = 4
}

public class NearHelper : MonoBehaviour
{
    string WalletUri = "https://wallet.testnet.near.org";
    string ContractId = "pixeltoken.testnet";
    string WalletSuffix = ".testnet";
    string BackendUri = "https://dev-testnet.pixeldapps.co/api";
    public bool Testnet = true;
    public LoginController loginController;
    public MainMenuController mainMenuController;
    public MarketController marketController;
    public EggController eggController;
    public SungenController sungenController;
    public MergeController mergeController;
    public LeaderboardController leaderboardController;
    private bool firstLogin = true;
    [HideInInspector]
    public DataGetState dataGetState;
    private int eggToHatch;
    private void Start()
    {
        if (BaseUtils.offlineMode)
        {
            return;
        }
        if (!Testnet)
        {
            BackendUri = "https://ecosystem.pixeldapps.co/api";
            WalletUri = "https://wallet.near.org";
            ContractId = "pixeltoken.near";
            WalletSuffix = ".near";
        }
        //Debug.Log(Database.databaseStruct.privateKey + "," + Database.databaseStruct.playerAccount + "," + Database.databaseStruct.publicKey);
        if (Database.databaseStruct.playerAccount != null && Database.databaseStruct.privateKey != null && Database.databaseStruct.publicKey != null)
        {
            //Debug.Log("checking for valid login");
            CheckForValidLogin();
        }
        else
        {
            //Debug.Log("No login found, please login");
            loginController.Setup();
        }
    }
    public void CheckForValidLogin()
    {
        StartCoroutine(CheckLoginValid(Database.databaseStruct.playerAccount, Database.databaseStruct.publicKey));
    }
    public IEnumerator CallViewMethod<T>(string methodName, string args, Action<T> callbackFunction)
    {
        var requestData = RequestParamsBuilder.CreateFunctionCallRequest(this.ContractId, methodName, args);
        var content = JsonUtility.ToJson(requestData);
        //Debug.Log(content);
        //content = content.Replace(@"\", "");
        return WebHelper.SendPost<T>(BackendUri + "/proxy/call-view-function", content, callbackFunction);
    }

    public IEnumerator CallChangeMethod<T>(string methodName, string args, Action<T> callbackFunction, string accountId, string privatekey, bool attachYoctoNear)
    {
        var requestData = RequestParamsBuilder.CreateFunctionCallRequest(this.ContractId, methodName, args, accountId, privatekey, attachYoctoNear);
        var content = JsonUtility.ToJson(requestData);

        return WebHelper.SendPost<T>(BackendUri + "/proxy/call-change-function", content, callbackFunction);
    }

    public void Login(string account_id)
    {
        //Debug.Log("Login start");
        Database.databaseStruct.playerAccount = account_id + WalletSuffix;
        StartCoroutine(WebHelper.SendGet<ProxyAccessKeyResponse>(BackendUri + "/proxy/get-ed25519pair", LoginCallback));
    }

    private void LoginCallback(ProxyAccessKeyResponse res)
    {
        //Debug.Log("Login Callback");
        Database.databaseStruct.privateKey = res.privateKey;
        Database.databaseStruct.publicKey = res.publicKey;
        Database.SaveDatabase();
        Application.OpenURL(WalletUri + "/login/?referrer=PixelPets%20Client&public_key=" + res.publicKey + "&contract_id=" + ContractId);
    }

    //this happens on invalid login
    public void Logout(bool fromPlayer = false)
    {
        Database.databaseStruct.playerAccount = null;
        Database.databaseStruct.publicKey = null;
        Database.databaseStruct.privateKey = null;
        if (!fromPlayer)
        {
            if (!firstLogin)
            {
                mainMenuController.ShowWarning("Invalid Login", "Login has been invalidated", "please try again.");
                firstLogin = false;
            }
            loginController.ResetLogin();
        }
    }


    private IEnumerator CheckLoginValid(string accountId, string publickey)
    {
        mainMenuController.ShowLoading();
        var requestData = RequestParamsBuilder.CreateAccessKeyCheckRequest(accountId, publickey);
        var content = JsonUtility.ToJson(requestData);

        return WebHelper.SendPost<BackendResponse<ProxyCheckAccessKeyResponse>>(BackendUri + "/pixelpets/is-valid-login", content, CheckLoginValidCallback);
    }

    private void CheckLoginValidCallback(BackendResponse<ProxyCheckAccessKeyResponse> res)
    {
        //Debug.Log(res.data.allowance);
        //Debug.Log("Is player registered: " + res.data.player_registered);//false
        //Debug.Log("Is key valid: " + res.data.valid);//true
        mainMenuController.HideLoading();
        if (!res.success)
        {
            loginController.Setup();
            return;
        }
        if (!res.data.valid)
        {
            Logout();
            Database.databaseStruct.validCredentials = false;
        }
        else
        {
            string cropAllowance = res.data.allowance.Substring(0, 4);
            float.TryParse(cropAllowance, out float allowanceFloat);
            Database.databaseStruct.allowance = allowanceFloat;
            if (!res.data.player_registered)
            {
                StartCoroutine(RegisterPlayerCoroutine());
                loginController.Setup();
            }
            else
            {
                Database.databaseStruct.validCredentials = true;
                //mainMenuController.ShowLoading();
                StartCoroutine(GetPlayerData());
            }
        }
    }

    public IEnumerator RegisterPlayerCoroutine()
    {
        //Debug.Log("REGISTER PLAYER");
        return CallChangeMethod<BackendResponse<object>>("register_player", null, GetPlayerDataCoroutine, Database.databaseStruct.playerAccount, Database.databaseStruct.privateKey, false);
    }

    public void GetPlayerDataCoroutine(BackendResponse<object> res)
    {
        //Debug.Log("Get Playerdata");
        if (res.success)
        {
            StartCoroutine(GetPlayerData());
        }
        else
        {
            mainMenuController.ShowWarning("Error on login", "Login was not possible, please check", "if your wallet has enough balance to cover transaction fees.");
        }
    }

    public IEnumerator GetPlayerData()
    {
        mainMenuController.ShowLoading();
        var d = new PlayerDataRequest
        {
            account_id = Database.databaseStruct.playerAccount
        };
        var dString = JsonUtility.ToJson(d);

        return WebHelper.SendPost<BackendResponse<PlayerDataResponse>>(BackendUri + "/pixelpets/get-playerdata", dString, GetPlayerDataCallback);
    }

    public void GetPlayerDataCallback(BackendResponse<PlayerDataResponse> res)
    {
        mainMenuController.HideLoading();
        //Debug.Log(JsonUtility.ToJson(res));
        if (res.data.maintenance_mode)
        {
            mainMenuController.SetMaintenanceMode();
            return;
        }
        PixelPets.FillDatabaseFromPlayerResponse(res.data);
        switch (dataGetState)
        {
            case DataGetState.Login:
                loginController.OnReceiveLoginAuthorise(true);
                break;
            case DataGetState.MarketPurchase:
                marketController.OnReceiveNewPlayerData();
                break;
            case DataGetState.RefillPurchase:
                mainMenuController.OnReceiveNewPlayerData();
                break;
            case DataGetState.AfterFight:
                mainMenuController.OnReceiveFightPlayerData();
                break;
            case DataGetState.AfterMerge:
                mergeController.OnReceiveMergeData();
                break;
        }
        dataGetState = DataGetState.Login;
    }
    public IEnumerator RequestHatchEgg(RarityType rarity)
    {
        mainMenuController.ShowLoading();
        var d = new HatchEggRequest
        {
            rarity = rarity
        };
        var dString = JsonUtility.ToJson(d);
        eggToHatch = (int)rarity;
        return CallChangeMethod<BackendResponse<long>>("hatch_egg", dString, HatchEggCallback, Database.databaseStruct.playerAccount, Database.databaseStruct.privateKey, false);
    }
    public void HatchEggCallback(BackendResponse<long> res)
    {
        RemoveAllowanceAndCheck();
        mainMenuController.HideLoading();
        if (res.success)
        {
            Database.databaseStruct.eggAmount[eggToHatch]--;
            Database.databaseStruct.hatchingTimer = TimestampHelper.GetDateTimeFromTimestamp(res.data);
            eggController.UpdateEggCount();
            Database.SaveDatabase();
            //Debug.Log(TimestampHelper.GetDateTimeFromTimestamp(res.data) + "," + res.data);
        }
        else
        {
            mainMenuController.ShowWarning("Error on loading info", "There was an error with your connection", "please relogin or refresh your page.");
        }
    }
    public IEnumerator RequestOpenEgg()
    {
        mainMenuController.ShowLoading();
        var d = new WalletAuth
        {
            account_id = Database.databaseStruct.playerAccount,
            privatekey = Database.databaseStruct.privateKey
        };
        var dString = JsonUtility.ToJson(d);
        return WebHelper.SendPost<BackendResponse<ScaledPetdata>>(BackendUri + "/pixelpets/open-egg", dString, OpenEggCallback);
    }
    public void OpenEggCallback(BackendResponse<ScaledPetdata> res)
    {
        RemoveAllowanceAndCheck();
        mainMenuController.HideLoading();
        if (res.success)
        {
            PixelPets.FillPetData(res.data);
            Database.RegisterNewCreatures();
            eggController.SyncEggHatch((int)Database.databaseStruct.hatchingEgg);
        }
        else
        {
            SendErrorMessage(res.error);
        }
    }
    private void SendErrorMessage(string error)
    {
        switch (error)
        {
            case "LackBalanceForState":
                mainMenuController.ShowWarning("Lack of balance", "Not enough funds to cover simple transactions", "Please fill your wallet with some near.");
                break;
            case "NotEnoughAllowance":
                mainMenuController.ShowWarning("Not enough allowance", "Your allowance has ran out", "please relogin with your account.");
                break;
            default:
                mainMenuController.ShowWarning("Error on loading info", "There was an error with your connection", "please relogin or refresh your page.");
                break;
        }
    }
    public IEnumerator RequestOfferPet(int creatureID, long price)
    {
        mainMenuController.ShowLoading();
        var d = new OfferAuth
        {
            account_id = Database.databaseStruct.playerAccount,
            privatekey = Database.databaseStruct.privateKey,
            token_id = creatureID.ToString(),
            price = (price * 1000000).ToString()
        };
        var dString = JsonUtility.ToJson(d);
        return WebHelper.SendPost<BackendResponse<ScaledPetdata>>(BackendUri + "/pixelpets/marketplace/offer-pet", dString, OfferPetCallback);
    }
    //todo remove callback param 
    public void OfferPetCallback(BackendResponse<ScaledPetdata> res)
    {
        RemoveAllowanceAndCheck();
        mainMenuController.HideLoading();
        mainMenuController.OnAcceptCreatureSell(res);
    }
    public IEnumerator RequestMarketData(string petName)
    {
        mainMenuController.ShowLoading();
        var d = new MarketAuth
        {
            account_id = Database.databaseStruct.playerAccount,
            privatekey = Database.databaseStruct.privateKey,
            pet_name = petName
        };
        var dString = JsonUtility.ToJson(d);
        return WebHelper.SendPost<BackendResponse<List<MarketPetData>>>(BackendUri + "/pixelpets/marketplace/search", dString, MarketDataCallback);
    }
    public void MarketDataCallback(BackendResponse<List<MarketPetData>> res)
    {
        mainMenuController.HideLoading();
        marketController.OnReceiveMarketData(res.data);
    }
    public IEnumerator RequestStrongestMarket()
    {
        mainMenuController.ShowLoading();
        var d = new MarketAuth
        {
            account_id = Database.databaseStruct.playerAccount,
            privatekey = Database.databaseStruct.privateKey,
        };
        var dString = JsonUtility.ToJson(d);
        return WebHelper.SendPost<BackendResponse<List<MarketPetData>>>(BackendUri + "/pixelpets/marketplace/search2", dString, StrongestMarketCallback);
    }
    public void StrongestMarketCallback(BackendResponse<List<MarketPetData>> res)
    {
        mainMenuController.HideLoading();
        marketController.OnReceiveStrongestData(res.data);
    }
    public IEnumerator RequestCancelOfferPet(int creatureID)
    {
        mainMenuController.ShowLoading();
        var d = new OfferAuth
        {
            account_id = Database.databaseStruct.playerAccount,
            privatekey = Database.databaseStruct.privateKey,
            token_id = creatureID.ToString()
        };
        var dString = JsonUtility.ToJson(d);
        return WebHelper.SendPost<BackendResponse<ScaledPetdata>>(BackendUri + "/pixelpets/marketplace/cancel-offer-pet", dString, CancelOfferPetCallback);
    }
    //todo remove callback param 
    public void CancelOfferPetCallback(BackendResponse<ScaledPetdata> res)
    {
        RemoveAllowanceAndCheck();
        mainMenuController.HideLoading();
        if (res.success)
        {
            mainMenuController.OnCancelOfferPet();
        }
        else
        {
            SendErrorMessage(res.error);
        }
    }
    public IEnumerator RequestBuyPet(int creatureID)
    {
        mainMenuController.ShowLoading();
        var d = new OfferAuth
        {
            account_id = Database.databaseStruct.playerAccount,
            privatekey = Database.databaseStruct.privateKey,
            token_id = creatureID.ToString()
        };
        var dString = JsonUtility.ToJson(d);
        return WebHelper.SendPost<BackendResponse<string>>(BackendUri + "/pixelpets/marketplace/buy-pet", dString, BuyPetCallback);
    }
    public void BuyPetCallback(BackendResponse<string> res)
    {
        RemoveAllowanceAndCheck();
        mainMenuController.HideLoading();
        if (res.success)
        {
            Application.OpenURL(res.data);
            marketController.ShowBuyAuthorize();
        }
        else
        {
            SendErrorMessage(res.error);
        }
    }
    public IEnumerator RequestTrainPet(int creatureID)
    {
        mainMenuController.ShowLoading();
        var d = new TrainPetRequest
        {
            token_id = creatureID.ToString()
        };
        var dString = JsonUtility.ToJson(d);

        return CallChangeMethod<BackendResponse<long>>("train_pet", dString, TrainPetCallback, Database.databaseStruct.playerAccount, Database.databaseStruct.privateKey, false);
    }
    public void TrainPetCallback(BackendResponse<long> res)
    {
        mainMenuController.HideLoading();
        RemoveAllowanceAndCheck();
        if (res.success)
        {
            mainMenuController.OnTrainingCallback(res.data);
        }
        else
        {
            mainMenuController.ShowWarning("Error on loading info", "There was an error with your connection", "please relogin or refresh your page.");
        }
    }
    public void RemoveAllowanceAndCheck()
    {
        Database.databaseStruct.allowance -= 25;
        mainMenuController.allowanceText.SetString($"allowance: {Database.databaseStruct.allowance}");
        if (Database.databaseStruct.allowance <= 0)
        {
            mainMenuController.ShowAllowanceWindow();
        }
    }
    public IEnumerator RequestFight(List<string> selectedPetTokens)
    {
        mainMenuController.ShowLoading();
        var fr = new FightRequest
        {
            account_id = Database.databaseStruct.playerAccount,
            privatekey = Database.databaseStruct.privateKey,
            playerdata = new PlayerLoadoutDataRequest
            {
                pet_loadout = selectedPetTokens.ToArray(),
            }
        };
        var dString = JsonUtility.ToJson(fr);
        return WebHelper.SendPost<BackendResponse<BattleInfo>>(BackendUri + "/pixelpets/simulate-fight", dString, RequestFightCallback);
    }
    public void RequestFightCallback(BackendResponse<BattleInfo> res)
    {
        mainMenuController.HideLoading();
        if (res.success)
        {
            mainMenuController.OnFindMatch(res.data);
        }
        else
        {
            SendErrorMessage(res.error);
            mainMenuController.OnMatchCancel();
        }
    }
    public IEnumerator RequestBuyEgg(RarityType rarityType)
    {
        mainMenuController.ShowLoading();
        var d = new BuyEggRequest
        {
            egg_type = rarityType.ToString().ToLower()
        };
        var dString = JsonUtility.ToJson(d);

        return CallChangeMethod<BackendResponse<short>>("buy_egg", dString, BuyEggCallback, Database.databaseStruct.playerAccount, Database.databaseStruct.privateKey, false);
    }
    public void BuyEggCallback(BackendResponse<short> res)
    {
        mainMenuController.HideLoading();
        RemoveAllowanceAndCheck();
        eggController.OnEggBuyCallback(res.success);
    }
    public IEnumerator RequestFightRefill()
    {
        mainMenuController.ShowLoading();
        var d = new WalletAuth
        {
            account_id = Database.databaseStruct.playerAccount,
            privatekey = Database.databaseStruct.privateKey
        };
        var dString = JsonUtility.ToJson(d);
        return WebHelper.SendPost<BackendResponse<string>>(BackendUri + "/pixelpets/refill-fightpoints", dString, FightRefillCallback);
    }
    //todo remove callback param 
    public void FightRefillCallback(BackendResponse<string> res)
    {
        RemoveAllowanceAndCheck();
        mainMenuController.HideLoading();
        Application.OpenURL(res.data);
        mainMenuController.ShowRefillAuthorize();
    }
    public IEnumerator RequestEggSupply()
    {
        mainMenuController.ShowLoading();
        var d = new WalletAuth { };
        var dString = JsonUtility.ToJson(d);

        return CallViewMethod<BackendResponse<EggSupplyCallback>>("get_egg_supply", dString, EggSupplyCallback);
    }
    public void EggSupplyCallback(BackendResponse<EggSupplyCallback> res)
    {
        mainMenuController.HideLoading();
        if (res.success)
        {
            eggController.OnReceiveEggSupply(res.data);
        }
        else
        {
            mainMenuController.ShowWarning("Error on loading info", "There was an error with your connection", "please relogin or refresh your page.");
        }
    }
    public IEnumerator RequestSungenEgg()
    {
        mainMenuController.ShowLoading();
        var d = new WalletAuth { };
        var dString = JsonUtility.ToJson(d);

        return CallChangeMethod<BackendResponse<long>>("claim_sungen_reward", dString, SungenEggCallback, Database.databaseStruct.playerAccount, Database.databaseStruct.privateKey, false);
    }
    public void SungenEggCallback(BackendResponse<long> res)
    {
        RemoveAllowanceAndCheck();
        mainMenuController.HideLoading();
        sungenController.SungenEggCallback(res.success, res.data);
    }
    public IEnumerator RequestEvolvePet(int creatureID)
    {
        mainMenuController.ShowLoading();
        var d = new TrainPetRequest
        {
            token_id = creatureID.ToString()
        };
        var dString = JsonUtility.ToJson(d);

        return CallChangeMethod<BackendResponse<short>>("evolve_pet", dString, EvolvePetCallback, Database.databaseStruct.playerAccount, Database.databaseStruct.privateKey, false);
    }
    public void EvolvePetCallback(BackendResponse<short> res)
    {
        RemoveAllowanceAndCheck();
        mainMenuController.HideLoading();
        mainMenuController.OnEvolveCallback(res.success);
    }
    public IEnumerator RequestLeaderboardData()
    {
        mainMenuController.ShowLoading();
        var d = new WalletAuth
        {
            account_id = Database.databaseStruct.playerAccount,
            privatekey = Database.databaseStruct.privateKey
        };
        var dString = JsonUtility.ToJson(d);
        return WebHelper.SendPost<BackendResponse<LeaderboardWrapper>>(BackendUri + "/pixelpets/get-leaderboard", dString, LeaderboardDataCallback);
    }
    public void LeaderboardDataCallback(BackendResponse<LeaderboardWrapper> res)
    {
        mainMenuController.HideLoading();
        if (res.success)
        {
            leaderboardController.OnReceiveRankData(res.data);
        }
        else
        {
            mainMenuController.ShowWarning("Error on loading info", "There was an error with your connection", "please relogin or refresh your page.");
        }
    }
    public IEnumerator RequestReleasePet(int creatureID)
    {
        mainMenuController.ShowLoading();
        var d = new TrainPetRequest
        {
            token_id = creatureID.ToString()
        };
        var dString = JsonUtility.ToJson(d);

        return CallChangeMethod<BackendResponse<object>>("release_pet", dString, ReleasePetCallback, Database.databaseStruct.playerAccount, Database.databaseStruct.privateKey, false);
    }
    public void ReleasePetCallback(BackendResponse<object> res)
    {
        RemoveAllowanceAndCheck();
        mainMenuController.HideLoading();
        mainMenuController.OnReleasePetCallback(res.success);
    }
    public IEnumerator RequestGetAvailableMarketPets()
    {
        mainMenuController.ShowLoading();
        var d = new WalletAuth
        {
            account_id = Database.databaseStruct.playerAccount
        };
        var dString = JsonUtility.ToJson(d);
        return WebHelper.SendPost<BackendResponse<List<int>>>(BackendUri + "/pixelpets/marketplace/get-available-pets", dString, AvailableMarketPetsCallback);
    }
    public void AvailableMarketPetsCallback(BackendResponse<List<int>> res)
    {
        BaseUtils.searchingForMarket = 0;
        mainMenuController.HideLoading();
        if (res.success)
        {
            marketController.OnReceiveAvailablePets(res.data);
        }
        else
        {
            mainMenuController.ShowWarning("Error on loading info", "There was an error with your connection", "please relogin or refresh your page.");
        }
    }
    public IEnumerator RequestMergePet(int creatureID1, int creatureID2)
    {
        mainMenuController.ShowLoading();
        var d = new MergePetRequest
        {
            improved_token_id = creatureID1.ToString(),
            removed_token_id = creatureID2.ToString()
        };
        var dString = JsonUtility.ToJson(d);

        return CallChangeMethod<BackendResponse<short>>("merge_pets", dString, MergePetCallback, Database.databaseStruct.playerAccount, Database.databaseStruct.privateKey, false);
    }
    public void MergePetCallback(BackendResponse<short> res)
    {
        RemoveAllowanceAndCheck();
        mainMenuController.HideLoading();
        mergeController.OnMergeCallback(res.success);
    }
}
