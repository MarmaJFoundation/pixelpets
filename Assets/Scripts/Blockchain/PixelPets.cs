using System;
using UnityEngine;

public class PixelPets
{
    public static void FillDatabaseFromPlayerResponse(PlayerDataResponse res)
    {
        Database.databaseStruct.eggAmount = new int[] { res.balance.egg_common, res.balance.egg_rare, res.balance.egg_epic, res.balance.egg_legendary };
        Database.databaseStruct.hatchingEgg = res.playerdata.hatching.rarity; //Rarity.None
        //Debug.Log(TimestampHelper.GetDateTimeFromTimestamp(res.playerdata.hatching.timestamp));
        Database.databaseStruct.hatchingTimer = TimestampHelper.GetDateTimeFromTimestamp(res.playerdata.hatching.timestamp);
        Database.databaseStruct.fightBalance = res.playerdata.fight_balance;
        Database.databaseStruct.sungenTimer = TimestampHelper.GetDateTimeFromTimestamp(res.sungen_timer);
        Database.databaseStruct.lastFight = TimestampHelper.GetDateTimeFromTimestamp(res.playerdata.last_fight);
        Database.databaseStruct.elapsedDays = res.playerdata.elapsed_days;
        Database.databaseStruct.hasSungen = res.owns_sungen;
        Database.databaseStruct.currentRank = res.playerdata.rating;
        Database.databaseStruct.pixelTokens = (int)((long.Parse(res.balance.pixeltoken) / 1000000));
        Database.databaseStruct.ownedCreatures.Clear();
        foreach (ScaledPetdata pet in res.pets)
        {
            FillPetData(pet);
        }
        Database.RegisterNewCreatures();
        Database.RegisterSelected();
        Database.SaveDatabase();
    }

    public static void FillPetData(ScaledPetdata pet)
    {
        StateType creatureState;
        if (pet.state != StateType.Selling && TimestampHelper.GetDateTimeFromTimestamp(pet.state_timer).ToFlatTime() <= 0)
        {
            creatureState = StateType.InPool;
        }
        else
        {
            creatureState = pet.state;
        }
        int.TryParse(pet.token_id, out int creatureID);
        float finalPrice = 0;
        if (pet.price != "0")
        {
            string shortPrice = pet.price.Remove(pet.price.Length - 4);
            int.TryParse(shortPrice, out int shortInt);
            finalPrice = shortInt / 100f;
        }
        CreatureStruct creatureStruct = new CreatureStruct(creatureID, pet.xp, pet.level, pet.train_level, pet.power_level, finalPrice, TimestampHelper.GetDateTimeFromTimestamp(pet.state_timer), creatureState, pet.rarity, pet.pet_type);
        Database.databaseStruct.ownedCreatures.Add(creatureStruct);
    }
}
