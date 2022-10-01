using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.EventSystems;
public enum RarityType
{
    Common = 0,
    Rare = 1,
    Epic = 2,
    Legendary = 3,
    None = 4
}
public class MainMenuController : MonoBehaviour
{
    public PetTabController petTabController;
    public RosterController rosterController;
    public EggController eggController;
    public LeaderboardController leaderboardController;
    public MarketController marketController;
    public PostScreenController postScreenController;
    public SungenController sungenController;
    public LoginController loginController;
    public MergeController mergeController;
    public NearHelper nearHelper;
    //left window
    public GameObject leftButtonsObj;
    public Image[] leftButtons;
    public Canvas[] windowObjs;
    public CustomText playerName;
    public CustomText pixelTokenText;
    public CustomText allowanceText;
    //card window
    public Transform cardWindowTransform;
    public GameObject cardScreenObj;
    public GameObject trainButton;
    public GameObject trainDescription;
    public GameObject evolveButton;
    public GameObject deleteButton;
    public GameObject leftCardButtons;
    public GameObject rightCardButtons;
    public GameObject newTextObj;
    public CustomText newText;
    public CustomText trainTitleText;
    public CustomText trainTimerText;
    public CustomText fightText;
    //fight window
    public FightController fightController;
    public Canvas fightCanvas;
    public Canvas mainCanvas;
    public Canvas mainMenuCanvas;
    public Canvas topCanvas;
    public CustomText matchSearchText;
    public GameObject matchParentButton;
    public GameObject matchSearchWindow;
    private Coroutine matchSearchCoroutine;
    public GameObject loadingScreen;
    public GameObject cardPrefab;
    public GameObject warningPrefab;
    public GameObject refillButton;
    public GameObject matchButton;
    //selling buttons
    public GameObject sellWindow;
    public GameObject sellButton;
    public GameObject stopButton;
    public GameObject inputParent;
    public CustomText titleText;
    public CustomText marketTime;
    public CustomInput priceInput;
    //sungen
    public GameObject sungenTab;

    private RectTransform tooltipTarget;
    public RectTransform tooltipWindow;
    public CustomText[] tooltipTexts;

    public GameObject allowanceWindow;
    public GameObject refillWindow;
    public GameObject maintenanceWindow;

    public Material backgroundMaterial;
    public Sprite[] buttonSprites;
    public Image backgroundButton;
    //public EventSystem eventSystem;
    [HideInInspector]
    public CardController cardController;
    [HideInInspector]
    public WarningController warningController;
    [HideInInspector]
    public bool showingCard;
    [HideInInspector]
    public bool showingWarn;
    public readonly int[] selectedCreatures = new int[3];
    private bool evolvingCreature = false;
    private float glowTimer = 1;
    private int sellingPetID;
    private long priceOffered;
    private int trainPrice;
    private int previousFightPoints;
    public void Setup()
    {
        for (int i = 0; i < 3; i++)
        {
            selectedCreatures[i] = -1;
        }
        leftButtonsObj.SetActive(true);
        mainMenuCanvas.enabled = true;
        mainMenuCanvas.gameObject.SetActive(true);
        UpdateSelectedCreatures();
        SetWindow(0);
        if (Database.databaseStruct.hasSungen)
        {
            sungenTab.SetActive(true);
        }
    }
    private void LateUpdate()
    {
        if (cardController != null && cardController.gameObject.activeSelf)
        {
            if (glowTimer > 0)
            {
                glowTimer -= Time.deltaTime;
            }
            else
            {
                ShowEffect((EffectType)(15 + cardController.rarity - 1), cardController.transform);
                glowTimer = 4;
            }
        }
        if (tooltipWindow.gameObject.activeSelf)
        {
            tooltipWindow.position = tooltipTarget.position + Vector3.down * (tooltipTarget.sizeDelta.y * .5f * BaseUtils.mainCanvas.parent.localScale.x);
            if (!tooltipTarget.gameObject.activeSelf)
            {
                HideTooltip();
            }
        }
    }
    public void DiscordClick()
    {
        Application.OpenURL("https://discord.gg/xFAAa8Db6f");
    }
    public void SwitchBackground()
    {
        Database.databaseStruct.stoppedBackground = !Database.databaseStruct.stoppedBackground;
        UpdateBackground();
        Database.SaveDatabase();
    }
    public void UpdateBackground()
    {
        backgroundButton.sprite = buttonSprites[Database.databaseStruct.stoppedBackground ? 1 : 0];
        backgroundMaterial.SetFloat("AnimatedOffsetUV_Speed_1", Database.databaseStruct.stoppedBackground ? 0 : .1f);
    }
    public void SetWindow(int windowIndex)
    {
        if (BaseUtils.onFight)
        {
            return;
        }
        HideTooltip();
        BaseUtils.ClearEffects();
        for (int i = 0; i < leftButtons.Length; i++)
        {
            if (i == windowIndex)
            {
                continue;
            }
            leftButtons[i].material = BaseUtils.normalUIMat;
            windowObjs[i].gameObject.SetActive(false);
            windowObjs[i].enabled = false;
        }
        leftButtons[windowIndex].material = BaseUtils.outlineUIMat;
        windowObjs[windowIndex].gameObject.SetActive(true);
        windowObjs[windowIndex].enabled = true;
        switch (windowIndex)
        {
            case 0:
                UpdatePlayerInfo();
                break;
            case 1:
                marketController.Setup();
                break;
            case 2:
                leaderboardController.Setup();
                break;
            case 3:
                eggController.Setup();
                break;
            case 4:
                mergeController.Setup();
                break;
        }
        if (windowIndex != 0)
        {
            Database.SaveDatabase();
        }
    }
    public void SetMaintenanceMode()
    {
        StartCoroutine(WaitAndSetMaintenance());
    }
    private IEnumerator WaitAndSetMaintenance()
    {
        while (BaseUtils.onFight)
        {
            yield return null;
        }
        for (int i = 0; i < windowObjs.Length; i++)
        {
            windowObjs[i].gameObject.SetActive(false);
            windowObjs[i].enabled = false;
        }
        topCanvas.gameObject.SetActive(false);
        topCanvas.enabled = false;
        maintenanceWindow.SetActive(true);
    }
    public void CloseCardWindow(bool forceClose = false)
    {
        HideTooltip();
        Database.SaveDatabase();
        if (evolvingCreature && !forceClose)
        {
            return;
        }
        if (cardController != null)
        {
            cardController.Destroy();
            cardController = null;
        }
        cardScreenObj.SetActive(false);
    }
    public void OnLogoutClick()
    {
        SetWindow(0);
        nearHelper.Logout(true);
        Database.SaveDatabase();
        loginController.ResetLogin();
        leftButtonsObj.SetActive(false);
        mainMenuCanvas.enabled = false;
        mainMenuCanvas.gameObject.SetActive(false);
        if (Database.databaseStruct.hasSungen)
        {
            sungenTab.SetActive(false);
        }
    }
    public void OnWikipediaClick()
    {
        Application.OpenURL("https://github.com/pixeldapps/pixelpets-wiki/wiki");
    }
    public void OnSellCreatureClick()
    {
        if (priceInput.typeString == "")
        {
            ShowWarning("Enter a value!", "Please input a value", "for this creature to be sold.");
            return;
        }
        priceOffered = int.Parse(priceInput.typeString);
        int databaseIndex = cardController.databaseIndex;
        if (Database.databaseStruct.ownedCreatures[databaseIndex].currentState == StateType.Injured ||
            Database.databaseStruct.ownedCreatures[databaseIndex].currentState == StateType.Training ||
            Database.databaseStruct.ownedCreatures[databaseIndex].currentState == StateType.Selling)
        {
            ShowWarning("Not possible!", "Cannot sell this creature in this state.", "it may be training or injured.");
            return;
        }
        sellingPetID = databaseIndex;
        StartCoroutine(nearHelper.RequestOfferPet(Database.databaseStruct.ownedCreatures[databaseIndex].creatureID, priceOffered));
    }
    public void OnAcceptCreatureSell(BackendResponse<ScaledPetdata> res)
    {
        if (res.success)
        {
            petTabController.creaturesToAnimate.Add(sellingPetID);
            Database.SetCreatureSellPrice(sellingPetID, StateType.Selling, (int)priceOffered);
            petTabController.UpdateList(true);
            rosterController.UpdateSelectCreatures();
            CloseCardWindow();
        }
        else
        {
            switch (res.error)
            {
                case "LackBalanceForState":
                    ShowWarning("Lack of balance", "Not enough funds to cover simple transactions", "Please fill your wallet with some near.");
                    break;
                case "NotEnoughAllowance":
                    ShowWarning("Not enough allowance", "Your allowance has ran out", "please relogin with your account.");
                    break;
                default:
                    ShowWarning("Request Failed!", "Cannot sell this creature right now.", "please try again!");
                    break;
            }
        }
    }
    public void OnStopSellClick()
    {
        StartCoroutine(nearHelper.RequestCancelOfferPet(Database.databaseStruct.ownedCreatures[cardController.databaseIndex].creatureID));
    }
    public void OnCancelOfferPet()
    {
        int databaseIndex = cardController.databaseIndex;
        Database.SetCreatureSellPrice(databaseIndex, StateType.InPool, 0);
        petTabController.UpdateList(true);
        CloseCardWindow();
    }
    //#protocol
    public void OnMatchClick()
    {
        if (BaseUtils.onFight)
        {
            return;
        }
        for (int i = 0; i < 3; i++)
        {
            if (selectedCreatures[i] == -1)
            {
                ShowWarning("Roster incomplete!", "You need atleast 3 creatures to fight.", "Select more or hatch more eggs.");
                return;
            }
        }
        matchSearchWindow.SetActive(true);
        matchParentButton.SetActive(false);
        matchSearchCoroutine = StartCoroutine(MatchSearchCoroutine());
        if (!BaseUtils.offlineMode)
        {
            StartCoroutine(nearHelper.RequestFight(new List<string>() {
            Database.databaseStruct.ownedCreatures[selectedCreatures[0]].creatureID.ToString(),
            Database.databaseStruct.ownedCreatures[selectedCreatures[1]].creatureID.ToString(),
            Database.databaseStruct.ownedCreatures[selectedCreatures[2]].creatureID.ToString() }));
        }
    }
    //#protocol
    public void OnMatchCancel()
    {
        StopCoroutine(matchSearchCoroutine);
        matchSearchWindow.SetActive(false);
        matchParentButton.SetActive(true);
    }
    private IEnumerator MatchSearchCoroutine()
    {
        float searchTimer = 0;
        while (true)
        {
            searchTimer++;
            matchSearchText.SetString($"Searching for match: {searchTimer.GetNiceTime(true)}");
            yield return new WaitForSeconds(1);
            if (BaseUtils.offlineMode)
            {
                PlayerInfo playerInfo = GetYourPlayerInfo();
                PlayerInfo botInfo = GetBotInfo(playerInfo);
                OnFindMatch(fightController.GenerateBattleInformation(playerInfo, botInfo));
            }
        }
    }
    public void OnFindMatch(BattleInfo battleInfo)
    {
        Database.SetSelected(selectedCreatures);
        Database.SaveDatabase();
        mainCanvas.gameObject.SetActive(false);
        mainCanvas.enabled = false;
        StopCoroutine(matchSearchCoroutine);
        fightCanvas.gameObject.SetActive(true);
        fightCanvas.enabled = true;
        cardController = null;
        fightController.Setup(battleInfo);
    }
    private PlayerInfo GetYourPlayerInfo()
    {
        PlayerInfo yourPlayerInfo = new PlayerInfo()
        {
            playerAccount = Database.databaseStruct.playerAccount,
            playerRank = Database.databaseStruct.currentRank,
            creatureTypes = new CreatureType[3]
            {
                Database.databaseStruct.ownedCreatures[selectedCreatures[0]].creatureType,
                Database.databaseStruct.ownedCreatures[selectedCreatures[1]].creatureType,
                Database.databaseStruct.ownedCreatures[selectedCreatures[2]].creatureType,
            },
            rarityTypes = new RarityType[3]
            {
                Database.databaseStruct.ownedCreatures[selectedCreatures[0]].rarity,
                Database.databaseStruct.ownedCreatures[selectedCreatures[1]].rarity,
                Database.databaseStruct.ownedCreatures[selectedCreatures[2]].rarity,
            },
            creatureExp = new int[3]
            {
                Database.databaseStruct.ownedCreatures[selectedCreatures[0]].experience,
                Database.databaseStruct.ownedCreatures[selectedCreatures[1]].experience,
                Database.databaseStruct.ownedCreatures[selectedCreatures[2]].experience,
            },
            creatureLevels = new int[3]
            {
                Database.databaseStruct.ownedCreatures[selectedCreatures[0]].level,
                Database.databaseStruct.ownedCreatures[selectedCreatures[1]].level,
                Database.databaseStruct.ownedCreatures[selectedCreatures[2]].level,
            },
            creatureTrainLevels = new int[3]
            {
                Database.databaseStruct.ownedCreatures[selectedCreatures[0]].trainLevel,
                Database.databaseStruct.ownedCreatures[selectedCreatures[1]].trainLevel,
                Database.databaseStruct.ownedCreatures[selectedCreatures[2]].trainLevel,
            }
        };
        return yourPlayerInfo;
    }
    //#Dan -> This is an example of how to generate a bot player info, it uses another playerinfo as the base for its variables
    private PlayerInfo GetBotInfo(PlayerInfo yourPlayerInfo)
    {
        int levelMedium = Mathf.RoundToInt((
            yourPlayerInfo.creatureLevels[0] +
            yourPlayerInfo.creatureLevels[1] +
            yourPlayerInfo.creatureLevels[2]) / 3f);
        int trainLevelMedium = Mathf.RoundToInt((
            yourPlayerInfo.creatureTrainLevels[0] +
            yourPlayerInfo.creatureTrainLevels[1] +
            yourPlayerInfo.creatureTrainLevels[2]) / 3f);
        int expMedium = Mathf.RoundToInt((
            yourPlayerInfo.creatureExp[0] +
            yourPlayerInfo.creatureExp[1] +
            yourPlayerInfo.creatureExp[2]) / 3f);

        PlayerInfo botPlayerInfo = new PlayerInfo()
        {
            playerAccount = Database.databaseStruct.playerAccount + " BOT",
            playerRank = Mathf.RoundToInt(BaseUtils.RandomFloat(yourPlayerInfo.playerRank * .75f, yourPlayerInfo.playerRank * 1.25f)),
            creatureTypes = new CreatureType[3]
            {
                BaseUtils.GetRandomCreature(),
                BaseUtils.GetRandomCreature(),
                BaseUtils.GetRandomCreature(),
            },
            rarityTypes = new RarityType[3]
            {
                (RarityType)BaseUtils.RandomInt(0, 3),
                (RarityType)BaseUtils.RandomInt(0, 3),
                (RarityType)BaseUtils.RandomInt(0, 3),
            },
            creatureExp = new int[3]
            {
                Mathf.RoundToInt(Mathf.Clamp(BaseUtils.RandomFloat(expMedium * .5f, expMedium), 0, Mathf.Infinity)),
                Mathf.RoundToInt(Mathf.Clamp(BaseUtils.RandomFloat(expMedium * .5f, expMedium), 0, Mathf.Infinity)),
                Mathf.RoundToInt(Mathf.Clamp(BaseUtils.RandomFloat(expMedium * .5f, expMedium), 0, Mathf.Infinity)),
            },
            creatureLevels = new int[3]
            {
                Mathf.RoundToInt(BaseUtils.RandomFloat(levelMedium * .75f, levelMedium * 1.25f)),
                Mathf.RoundToInt(BaseUtils.RandomFloat(levelMedium * .75f, levelMedium * 1.25f)),
                Mathf.RoundToInt(BaseUtils.RandomFloat(levelMedium * .75f, levelMedium * 1.25f)),
            },
            creatureTrainLevels = new int[3]
            {
                Mathf.RoundToInt(BaseUtils.RandomFloat(trainLevelMedium * .75f, trainLevelMedium * 1.25f)),
                Mathf.RoundToInt(BaseUtils.RandomFloat(trainLevelMedium * .75f, trainLevelMedium * 1.25f)),
                Mathf.RoundToInt(BaseUtils.RandomFloat(trainLevelMedium * .75f, trainLevelMedium * 1.25f)),
            }
        };
        return botPlayerInfo;
    }
    public void OnFightFinished(BattleInfo battleInfo)
    {
        /*for (int i = 0; i < 3; i++)
        {
            int creatureIndex = selectedCreatures[i];
            if (battleInfo.playerInfo1.playerAccount == Database.databaseStruct.playerAccount && battleInfo.player1CardStats[i].health <= 0)
            {
                int injuredTime = 60 * 3 * Database.databaseStruct.ownedCreatures[creatureIndex].level;
                Database.SetCreatureState(creatureIndex, StateType.Injured, injuredTime);
                petTabController.creaturesToAnimate.Add(creatureIndex);
            }
            if (battleInfo.playerInfo2.playerAccount == Database.databaseStruct.playerAccount && battleInfo.player2CardStats[i].health <= 0)
            {
                int injuredTime = 60 * 3 * Database.databaseStruct.ownedCreatures[creatureIndex].level;
                Database.SetCreatureState(creatureIndex, StateType.Injured, injuredTime);
                petTabController.creaturesToAnimate.Add(creatureIndex);
            }
        }*/
        HideTooltip();
        postScreenController.Setup(battleInfo);
    }
    //#protocol
    public void OnTrainClick()
    {
        int databaseIndex = cardController.databaseIndex;
        if (Database.databaseStruct.ownedCreatures[databaseIndex].currentState == StateType.Selling ||
            Database.databaseStruct.ownedCreatures[databaseIndex].currentState == StateType.Injured ||
            Database.databaseStruct.ownedCreatures[databaseIndex].currentState == StateType.Training)
        {
            ShowWarning("Not posssible!", "This creature cannot be trained right now", "it may be injured or selling.");
            return;
        }
        if (!Database.HasTrainSlot())
        {
            ShowWarning("Already training!", "You cannot train more than", "one creature at a time.");
            return;
        }
        trainPrice = (Database.databaseStruct.ownedCreatures[databaseIndex].trainLevel + 1) * 4;
        ShowWarning("Training creature", $"Would you like to train {cardController.creatureName.lastString}", $"for a total amount of {trainPrice} @ ?", OnAnswerTraining);
    }
    public void OnAnswerTokenBuy(bool accepted)
    {
        if (accepted)
        {
            Application.OpenURL("https://app.ref.finance/#wrap.near%7Cpixeltoken.near");
        }
    }
    private void OnAnswerTraining(bool accepted)
    {
        if (accepted)
        {
            if (Database.databaseStruct.pixelTokens <= trainPrice)
            {
                ShowWarning("Not enough Tokens!", $"It costs {trainPrice} @ to train your creature", "Would you like to purchase more tokens?", OnAnswerTokenBuy);
                return;
            }
            StartCoroutine(nearHelper.RequestTrainPet(Database.databaseStruct.ownedCreatures[cardController.databaseIndex].creatureID));
        }
    }
    public void OnTrainingCallback(long trainTimer)
    {
        petTabController.creaturesToAnimate.Add(cardController.databaseIndex);
        int trainlevel = Database.databaseStruct.ownedCreatures[cardController.databaseIndex].trainLevel;
        Database.SetCreatureTrainLevel(cardController.databaseIndex, trainlevel + 1);
        Database.databaseStruct.pixelTokens -= trainPrice;
        Database.SetCreatureState(cardController.databaseIndex, StateType.Training, TimestampHelper.GetDateTimeFromTimestamp(trainTimer));
        CloseCardWindow();
        UpdatePlayerInfo();
    }
    public void OnEvolveClick()
    {
        ShowWarning("Evolving creature", $"Would you like to evolve {cardController.creatureName.lastString}", $"for a total amount of {cardController.evolution * 50} @ ?", OnAnswerEvolve);
    }
    private void OnAnswerEvolve(bool accepted)
    {
        if (accepted)
        {
            if (Database.databaseStruct.pixelTokens <= cardController.evolution * 50)
            {
                ShowWarning("Not enough Tokens!", $"It costs {cardController.evolution * 50} @ to evolve your creature", "Would you like to purchase more tokens?", OnAnswerTokenBuy);
                return;
            }
            StartCoroutine(nearHelper.RequestEvolvePet(Database.databaseStruct.ownedCreatures[cardController.databaseIndex].creatureID));
        }
    }
    public void OnEvolveCallback(bool succcess)
    {
        if (succcess)
        {
            StartCoroutine(AnimateEvolution());
        }
        else
        {
            ShowWarning("Error on evolving", "Evolving your creature failed", "please try again!");
        }
    }
    public IEnumerator AnimateEvolution()
    {
        evolvingCreature = true;
        int databaseIndex = cardController.databaseIndex;
        CreatureType preCreature = Database.databaseStruct.ownedCreatures[databaseIndex].creatureType;
        leftCardButtons.SetActive(false);
        rightCardButtons.SetActive(false);
        deleteButton.SetActive(false);
        newTextObj.SetActive(false);
        float timer = 0;
        Vector3 fromPos = cardController.transform.localPosition;
        Vector3 gotoPos = fromPos + Vector3.right * 20 * BaseUtils.RandomSign();
        bool midEffect = false;
        ShowEffect(BaseUtils.creatureDict[preCreature].bodyType.ToEffect(), cardController.cardRect);
        ShowEffect(EffectType.Desintegrate, cardController.cardRect);
        while (timer <= 1)
        {
            if (timer >= .5f && !midEffect)
            {
                midEffect = true;
                ShowEffect(BaseUtils.creatureDict[preCreature].bodyType.ToEffect(), cardController.cardRect);
            }
            cardController.transform.localPosition = Vector3.LerpUnclamped(fromPos, gotoPos, timer.Evaluate(CurveType.DeathCurve));
            timer += Time.deltaTime * .75f;
            yield return null;
        }
        cardController.transform.localPosition = fromPos;
        Database.SetCreatureType(databaseIndex, BaseUtils.creatureDict[preCreature].evolutionType);
        CloseCardWindow(true);
        rosterController.UpdateSelectCreatures();
        petTabController.UpdateList();
        ShowCard(databaseIndex);
        deleteButton.SetActive(false);
        ShowEffect(EffectType.Evolve, cardController.cardRect);
        ShowEffect((EffectType)(15 + cardController.rarity - 1), cardController.cardRect);
        yield return new WaitForSeconds(.2f);
        ShowEffect(BaseUtils.creatureDict[preCreature].bodyType.ToEffect(), cardController.cardRect);
        newTextObj.SetActive(true);
        newText.SetString("your creature has evolved!");
        yield return new WaitForSeconds(1);
        evolvingCreature = false;
        yield return new WaitForSeconds(4);
        newTextObj.SetActive(false);
        deleteButton.SetActive(true);
    }
    private bool IsStarterPet(int databaseIndex)
    {
        CreatureType creatureType = Database.databaseStruct.ownedCreatures[databaseIndex].creatureType;
        return !(creatureType != CreatureType.Clin && creatureType != CreatureType.Phypo && creatureType != CreatureType.Snock);
    }
    public void OnDeleteClick()
    {
        //deleteCheckObj.SetActive(true);
        bool isStarter = IsStarterPet(cardController.databaseIndex);
        //ShowWarning("Wait!", "Are you sure you want to refund", isStarter ? "this pet for free?" : $"this pet for {GetPriceFromRarity(cardController.rarity - 1)} @ ?", OnDeleteConfirmClick);
        ShowWarning("Wait!", "Are you sure you want to release your", "pet? You cannot undo this action!", OnDeleteConfirmClick);
    }
    public void OnDeleteConfirmClick(bool accepted)
    {
        if (accepted)
        {
            StartCoroutine(nearHelper.RequestReleasePet(Database.databaseStruct.ownedCreatures[cardController.databaseIndex].creatureID));
        }
    }
    private int GetPriceFromRarity(int rarity)
    {
        if (rarity == 1)
        {
            return 80;
        }
        else if (rarity == 2)
        {
            return 500;
        }
        else if (rarity == 3)
        {
            return 2500;
        }
        return 12;
    }
    public void OnReleasePetCallback(bool success)
    {
        if (success)
        {
            bool isStarter = IsStarterPet(cardController.databaseIndex);
            int databaseIndex = cardController.databaseIndex;
            //int refundPrice = isStarter ? 0 : GetPriceFromRarity(cardController.rarity - 1);
            //Database.databaseStruct.pixelTokens += refundPrice;
            Database.databaseStruct.ownedCreatures.RemoveAt(databaseIndex);
            petTabController.UpdateList();
            rosterController.UpdateSelectCreatures();
            UpdatePlayerInfo();
            CloseCardWindow();
            //ShowWarning("Pet has been refunded!", "Your pet was released", isStarter ? "for free." : $"and you refunded {refundPrice} @");
            ShowWarning("Pet released!", "Your pet has been released forever.", "Godspeed.");
        }
        else
        {
            ShowWarning("Error on releasing", "There was an issue releasing your pet", "please try again!");
        }
    }
    public void UpdateAllTexts()
    {
        foreach (CustomText customText in FindObjectsOfType<CustomText>())
        {
            customText.UpdateText();
        }
    }
    public void UpdatePlayerInfo()
    {
        matchButton.SetActive(Database.databaseStruct.fightBalance > 0);
        refillButton.SetActive(Database.databaseStruct.fightBalance <= 0);
        string playerString = Database.databaseStruct.playerAccount;
        if (playerString.Length > 18)
        {
            playerString = playerString.Substring(0, 18);
        }
        playerName.SetString($"{playerString}");
        pixelTokenText.SetString(Database.databaseStruct.pixelTokens.ToString());
        allowanceText.SetString($"Allowance: {Database.databaseStruct.allowance}");
        fightText.SetString($"Fight balance: {Database.databaseStruct.fightBalance}");
        rosterController.UpdateSelectCreatures();
        rosterController.UpdateInfo();
        petTabController.UpdateList();
        sungenController.Setup();
        Database.SaveDatabase();
    }
    public void UpdateTrainTimers(int creatureIndex)
    {
        if (showingCard && cardController.databaseIndex == creatureIndex)
        {
            trainTitleText.SetString("Currently Training!");
            trainTimerText.SetString(Database.databaseStruct.ownedCreatures[cardController.databaseIndex].stateTimer.ToFlatTime().GetNiceTime(false));
        }
    }
    public void UpdateInjuredTimers(int creatureIndex)
    {
        if (showingCard && cardController.databaseIndex == creatureIndex)
        {
            trainTitleText.SetString("Currently Injured!");
            trainTimerText.SetString(Database.databaseStruct.ownedCreatures[cardController.databaseIndex].stateTimer.ToFlatTime().GetNiceTime(false));
        }
    }
    public void BumpObj(Transform transform, float normalScale = 1)
    {
        StartCoroutine(BumpObjCoroutine(transform, normalScale));
    }
    public IEnumerator BumpObjCoroutine(Transform bumpTransform, float normalScale)
    {
        float timer = 0;
        bool hasRect = bumpTransform.TryGetComponent(out RectTransform transformRect);
        float transformSize = hasRect ? (1 + transformRect.sizeDelta.magnitude * .001f) : 1;
        while (timer <= 1)
        {
            bumpTransform.localScale = Vector3.LerpUnclamped(Vector3.one * normalScale, Vector3.one * (1.25f / transformSize) * normalScale, timer.Evaluate(CurveType.PeakCurve));
            timer += Time.deltaTime * 5;
            yield return null;
        }
        bumpTransform.localScale = Vector3.one * normalScale;
    }
    public void OnRemoveClick(int index)
    {
        if (selectedCreatures[index] == -1)
        {
            return;
        }
        Database.SetCreatureState(selectedCreatures[index], StateType.InPool);
        selectedCreatures[index] = -1;
        rosterController.UpdateSelectCreatures();
        petTabController.UpdateList(true);
    }
    public void OnRosterCreatureClick(int index)
    {
        if (selectedCreatures[index] == -1)
        {
            ShowWarning("Empty slot!", "Select a creature from the right tab", "before checking your roster.");
            return;
        }
        ShowCard(selectedCreatures[index]);
    }
    public void ShowCard(int databaseIndex)
    {
        if (showingCard || BaseUtils.onFight)
        {
            return;
        }
        glowTimer = 1;
        leftCardButtons.SetActive(true);
        rightCardButtons.SetActive(true);
        newTextObj.SetActive(false);
        cardController = InstantiateCard(cardWindowTransform, false);
        cardController.Setup(this, databaseIndex, false);
        cardController.transform.localPosition = new Vector3(0, 10, 0);
        cardController.transform.SetSiblingIndex(4);
        cardScreenObj.SetActive(true);
        StateType creatureState = Database.databaseStruct.ownedCreatures[databaseIndex].currentState;
        bool creatureFree = creatureState == StateType.InPool || creatureState == StateType.Selected;
        trainButton.SetActive(creatureFree && Database.HasTrainSlot() && cardController.trainLevel < 10);
        int levelToEvolve = BaseUtils.creatureDict[Database.databaseStruct.ownedCreatures[databaseIndex].creatureType].levelToEvolve;
        bool canEvolve = levelToEvolve != -1 && Database.databaseStruct.ownedCreatures[databaseIndex].level >= levelToEvolve;
        evolveButton.SetActive(canEvolve && creatureFree);
        deleteButton.SetActive(creatureFree);
        sellWindow.SetActive(creatureFree || creatureState == StateType.Selling);
        BumpObj(cardController.transform);
        showingCard = true;
        trainDescription.SetActive(creatureState == StateType.Training || creatureState == StateType.Injured);
        if (creatureState == StateType.Training)
        {
            UpdateTrainTimers(databaseIndex);
        }
        else if (creatureState == StateType.Injured)
        {
            UpdateInjuredTimers(databaseIndex);
        }
        else if (creatureState == StateType.Selling)
        {
            float sellingPrice = Database.databaseStruct.ownedCreatures[databaseIndex].sellPrice;
            titleText.SetString($"selling for {sellingPrice} @");
            inputParent.SetActive(false);
            marketTime.gameObject.SetActive(false);
            //marketTime.SetString("5:00");
            sellButton.SetActive(false);
            stopButton.SetActive(true);

        }
        else if (creatureState == StateType.InPool || creatureState == StateType.Selected)
        {
            CreatureType creatureType = Database.databaseStruct.ownedCreatures[databaseIndex].creatureType;
            if (creatureType != CreatureType.Phypo && creatureType != CreatureType.Snock && creatureType != CreatureType.Clin)
            {
                titleText.SetString("sell on the market:");
                inputParent.SetActive(true);
                priceInput.Setup();
                //priceInput.OnPointerClick(null);
                //eventSystem.SetSelectedGameObject(priceInput.gameObject);
                marketTime.gameObject.SetActive(false);
                sellButton.SetActive(true);
                stopButton.SetActive(false);
            }
            else
            {
                rightCardButtons.SetActive(false);
            }
        }
    }
    public void ShowNewCreature(int databaseIndex, string goText)
    {
        StartCoroutine(WaitForShowingCard(databaseIndex, goText));
    }
    private IEnumerator WaitForShowingCard(int databaseIndex, string goText)
    {
        bool willWait = false;
        while (showingCard)
        {
            willWait = true;
            yield return null;
        }
        if (willWait)
        {
            yield return new WaitForSeconds(.5f);
        }
        glowTimer = .1f;
        leftCardButtons.SetActive(false);
        rightCardButtons.SetActive(false);
        deleteButton.SetActive(false);
        cardController = InstantiateCard(cardWindowTransform, false);
        cardController.Setup(this, databaseIndex, true);
        cardController.transform.localPosition = new Vector3(0, 10, 0);
        cardController.transform.SetSiblingIndex(4);
        ShowEffect(BaseUtils.creatureDict[Database.databaseStruct.ownedCreatures[databaseIndex].creatureType].bodyType.ToEffect(), cardController.cardRect);
        cardScreenObj.SetActive(true);
        newTextObj.SetActive(true);
        newText.SetString(goText);
        petTabController.UpdateList();
        UpdatePlayerInfo();
    }
    public void ShowAllowanceWindow()
    {
        previousFightPoints = Database.databaseStruct.fightBalance;
        allowanceWindow.SetActive(true);
    }
    public void HideAllowanceWindow()
    {
        allowanceWindow.SetActive(false);
    }
    public void OnRefillBalanceClick()
    {
        StartCoroutine(nearHelper.RequestFightRefill());
    }
    public void ShowRefillAuthorize()
    {
        refillWindow.SetActive(true);
    }
    public void OnAuthorizedClick()
    {
        refillWindow.SetActive(false);
        nearHelper.dataGetState = DataGetState.RefillPurchase;
        StartCoroutine(nearHelper.GetPlayerData());
    }
    public void OnReceiveNewPlayerData()
    {
        HideLoading();
        if (previousFightPoints < Database.databaseStruct.fightBalance)
        {
            ShowWarning("Fight points added!", "Your transaction was successful", "and you also received an egg! Enjoy!");
            UpdatePlayerInfo();
        }
        else
        {
            ShowWarning("Error on purchase", "The purchase for fight points has failed", "please try again!");
        }
    }
    public void OnReceiveFightPlayerData()
    {
        HideLoading();
        UpdateSelectedCreatures();
        UpdatePlayerInfo();
    }
    public void UpdateSelectedCreatures()
    {
        int[] finalIndexes = new int[3] { -1, -1, -1 };
        for (int i = 0; i < 3; i++)
        {
            int index = Database.GetSelectUnit(i);
            if (index != -1)
            {
                if (index >= Database.databaseStruct.ownedCreatures.Count)
                {
                    Database.SetSelected(new int[3] { -1, -1, -1 });
                    return;
                }
                if (Database.databaseStruct.ownedCreatures[index].currentState == StateType.InPool)
                {
                    finalIndexes[i] = index;
                    Database.SetCreatureState(index, StateType.Selected, Database.databaseStruct.ownedCreatures[index].stateTimer);
                }
            }
        }
        Database.SetSelected(finalIndexes);
    }
    public CardController InstantiateCard(Transform parentTransform, bool fromFight)
    {
        if (cardController != null && !fromFight)
        {
            return cardController;
        }
        GameObject cardObj = Instantiate(cardPrefab, parentTransform);
        return cardObj.GetComponent<CardController>();
    }
    public void ShowTooltip(RectTransform target, string[] bodyText)
    {
        tooltipTarget = target;
        tooltipWindow.gameObject.SetActive(true);
        for (int i = 0; i < tooltipTexts.Length; i++)
        {
            tooltipTexts[i].gameObject.SetActive(i < bodyText.Length);
            if (i < bodyText.Length)
            {
                tooltipTexts[i].SetString(bodyText[i]);
            }
        }
    }
    public void HideTooltip()
    {
        tooltipWindow.gameObject.SetActive(false);
        //tooltipWindow.SetParent(BaseUtils.restingCanvas);
    }
    public void ShowWarning(string titleText, string bodyText1, string bodyText2, System.Action<bool> callback = null)
    {
        if (showingWarn)
        {
            return;
        }
        warningController = InstantiateWarning(BaseUtils.mainCanvas);
        warningController.Setup(this, titleText, bodyText1, bodyText2, callback);
        BumpObj(warningController.transform.GetChild(0));
    }
    private WarningController InstantiateWarning(Transform parentTransform)
    {
        if (warningController != null)
        {
            return warningController;
        }
        GameObject warningObj = Instantiate(warningPrefab, parentTransform);
        return warningObj.GetComponent<WarningController>();
    }
    public void ShowEffect(EffectType effectType, Transform goParent)
    {
        if (!goParent.gameObject.activeSelf)
        {
            return;
        }
        EffectController effectController = InstantiateEffect(effectType, goParent);
        effectController.Setup(effectType, Vector3.zero);
    }
    public void ShowEffect(EffectType effectType, Transform goParent, Vector3 offset)
    {
        if (!goParent.gameObject.activeSelf)
        {
            return;
        }
        EffectController effectController = InstantiateEffect(effectType, goParent);
        effectController.Setup(effectType, offset);
    }
    private EffectController InstantiateEffect(EffectType effectType, Transform goParent)
    {
        EffectController effectController;
        if (BaseUtils.effectPool[effectType].Count > 0)
        {
            effectController = BaseUtils.effectPool[effectType].Dequeue();
            effectController.transform.SetParent(goParent);
            effectController.transform.localPosition = Vector3.back * 20;
        }
        else
        {
            GameObject effectObj = Instantiate(BaseUtils.effectDict[effectType].effectPrefab, goParent);
            effectController = effectObj.GetComponent<EffectController>();
        }
        return effectController;
    }
    public void ShowLoading()
    {
        loadingScreen.SetActive(true);
    }
    public void HideLoading()
    {
        loadingScreen.SetActive(false);
    }
    #region ReceiveOnlineSyncs
    public void SyncCreatureTrain(int creatureIndex)
    {
        //Database.SetCreatureTrainLevel(creatureIndex, trainLevel);
        petTabController.creaturesToAnimate.Add(creatureIndex);
        petTabController.UpdateList(true);
        UpdateSelectedCreatures();
        //ShowWarning("Creature done training!", $"Your {Database.databaseStruct.ownedCreatures[creatureIndex].creatureType} has finished training!", "and its now stronger!");
    }
    public void SyncCreatureInjured(int creatureIndex)
    {
        petTabController.creaturesToAnimate.Add(creatureIndex);
        petTabController.UpdateList(true);
    }
    #endregion
}
