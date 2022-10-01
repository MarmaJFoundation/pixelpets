using System;
using System.Collections.Generic;
using System.Net.Http;

public class KeyPair
{
    public string publicKey;
    public string privateKey;
}

[System.Serializable]
public class RequestParamsBuilder
{
    static public RequestParams CreateFunctionCallRequest(string contract_id, string methodName, string args, string accountId = null, string privatekey = null, bool attachYoctoNear = false)
    {
        return new RequestParams
        {
            contract_id = contract_id,
            account_id = accountId,
            method_name = methodName,
            args = args,
            privatekey = privatekey,
            attachYoctoNear = attachYoctoNear

        };

    }

    static public RequestParams CreateFunctionViewRequest<T>(string contract_id, string methodName, string args)
    {
        return new RequestParams
        {
            contract_id = contract_id,
            method_name = methodName,
            args = args,
        };

    }

    static public RequestParams CreateAccessKeyCheckRequest(string accountId, string publickey)
    {
        return new RequestParams
        {
            account_id = accountId,
            publickey = publickey,
        };

    }
}

[Serializable]
public class WalletAuth
{
    public string account_id;
    public string privatekey;
}
[Serializable]
public class OfferAuth : WalletAuth
{
    public string token_id;
    public string price;
}
[Serializable]
public class MarketAuth : WalletAuth
{
    public string pet_name;
}
[Serializable]
public class RequestParams
{
    public string account_id;
    public string contract_id;
    public string method_name;
    public string args;
    public string privatekey;
    public string publickey;
    public bool attachYoctoNear;

}

[Serializable]
public class ProxyAccessKeyResponse
{
    public string privateKey;
    public string publicKey;
}

[Serializable]
public class ProxyCheckAccessKeyResponse
{
    public bool valid;
    public string allowance;
    public bool fullAccess;
    public bool player_registered;
}

[Serializable]
public class PlayerDataRequest
{
    public string account_id;
}

public class HatchEggRequest
{
    public RarityType rarity;
}
public class MergePetRequest
{
    public string improved_token_id;
    public string removed_token_id;
}
public class TrainPetRequest
{
    public string token_id;
}
public class BuyEggRequest
{
    public string egg_type;
}
[Serializable]
public class PlayerLoadoutDataRequest
{
    public string[] pet_loadout;
}

[Serializable]
public class FightRequest : WalletAuth
{
    public PlayerLoadoutDataRequest playerdata;
}

public class TimestampHelper
{
    public static DateTime GetDateTimeFromTimestamp(long timestamp)
    {
        System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dtDateTime = dtDateTime.AddMilliseconds(timestamp).ToLocalTime();
        return dtDateTime;
    }
}

[Serializable]
public class BackendResponse<T>
{
    public bool success;
    public T data;
    public string error;
}

[Serializable]
public class PlayerDataResponse
{
    public PlayerBalance balance;
    public PlayerData playerdata;
    public long sungen_timer;
    public bool owns_sungen;
    public bool maintenance_mode;
    public List<ScaledPetdata> pets;
}

[Serializable]
public class PlayerBalance
{
    public string pixeltoken;
    public int egg_common;
    public int egg_rare;
    public int egg_epic;
    public int egg_legendary;
}

[Serializable]
public class PlayerData
{
    public Hatching hatching;
    public int training;
    public int matches_won;
    public int matches_lost;
    public int fight_balance;
    public int rating;
    public long last_fight;
    public int elapsed_days;
}

[Serializable]
public class Hatching
{
    public RarityType rarity;
    public long timestamp;
}

[Serializable]
public class Pet
{
    public string token_id;
    public CreatureType pet_type;
    public StateType state;
    public RarityType rarity;
    public int train_level;
    public int xp;
    public long state_timer;
}
[Serializable]
public class MarketPetData
{
    public string token_id;
    public string price;
    public ScaledPetdata pet_data;
}
[Serializable]
public class LeaderboardWrapper
{
    public List<RankingData> leaderboard;
    public List<RankingData> tournament;
    public string active_tournament;
    public long tournament_ends;
}
[Serializable]
public class RankingData
{
    public string account_id;
    public int matches_lost;
    public int matches_won;
    public List<PlayerLoadout> player_loadout;
    public int player_rating;
    public int position;
}
[Serializable]
public class PlayerLoadout
{
    public string token_id;
    public int pet_type;
    public int pet_rarity;
}

[Serializable]
public class ScaledPetdata
{
    public string token_id;
    public CreatureType pet_type;
    public int train_level;
    public StateType state;
    public long state_timer;
    public RarityType rarity;
    public int xp;
    public int level;
    public string owner;
    public string price;
    public int power_level;
    public PetCombatInfo combat_info;
}
[Serializable]
public class EggSupplyCallback
{
    public int common;
    public int rare;
    public int epic;
    public int legendary;
    public string common_price;
    public string rare_price;
    public string epic_price;
    public string legendary_price;
}
[Serializable]
public class PetCombatInfo
{
    public int damage;
    public int magic;
    public int defense;
    public int speed;
    public int maxHealth;
    public string pet_name;
}