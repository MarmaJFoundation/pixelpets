using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
public enum StateType
{
    InPool = 4,
    Selected = 3,
    Training = 0,
    Injured = 1,
    Selling = 2
}
public struct CreatureStruct
{
    public int creatureID;
    public int experience;
    public int level;
    public int trainLevel;
    public int powerLevel;
    public float sellPrice;
    public DateTime stateTimer;
    public StateType currentState;
    public RarityType rarity;
    public CreatureType creatureType;

    public CreatureStruct(int creatureID, int experience, int level, int trainLevel, int powerLevel, float sellPrice, DateTime stateTimer, StateType currentState, RarityType rarity, CreatureType creatureType)
    {
        this.creatureID = creatureID;
        this.experience = experience;
        this.level = level;
        this.trainLevel = trainLevel;
        this.powerLevel = powerLevel;
        this.sellPrice = sellPrice;
        this.stateTimer = stateTimer;
        this.currentState = currentState;
        this.rarity = rarity;
        this.creatureType = creatureType;
    }
}
public struct SelectStruct
{
    public string playerAccount;
    public int[] selectCreatures;

    public SelectStruct(string playerAccount, int[] selectCreatures)
    {
        this.playerAccount = playerAccount;
        this.selectCreatures = selectCreatures;
    }
}

public class DatabaseStruct
{
    public string playerAccount;
    public string publicKey;
    public string privateKey;
    public bool validCredentials;
    public bool stoppedBackground;
    public bool hideSelling;
    public bool hideTraining;
    public bool hideInjured;
    public int lockSpeed;
    public int pixelTokens;
    public int currentRank;
    public int fightBalance;
    public bool hasSungen;
    public DateTime sungenTimer;
    public float allowance;
    public RarityType hatchingEgg;
    public DateTime hatchingTimer;
    public DateTime lastFight;
    public int elapsedDays;
    public int[] eggAmount = new int[4];
    public List<CreatureStruct> ownedCreatures = new List<CreatureStruct>();

    //do not sync this \/
    public List<SelectStruct> rosterSlots = new List<SelectStruct>();
    //public List<int> creatureIndexes = new List<int>();
    //public List<int> indexIDs = new List<int>();
}
public class Database : MonoBehaviour
{
    public static DatabaseStruct databaseStruct;
    private static string databaseName;
    private static readonly string currentVersion = "1.7";
    public CustomText versionText;

    private void Awake()
    {
        versionText.SetString($"pixel pets beta v.{Application.version}");
        databaseName = "PixelPetsDatabase";
        if (PlayerPrefs.HasKey("ClientVersion"))
        {
            string clientVersion = PlayerPrefs.GetString("ClientVersion");
            if (clientVersion != currentVersion && PlayerPrefs.HasKey(databaseName))
            {
                PlayerPrefs.DeleteKey(databaseName);
            }
        }
        else if (PlayerPrefs.HasKey(databaseName))
        {
            PlayerPrefs.DeleteKey(databaseName);
        }

        PlayerPrefs.SetString("ClientVersion", currentVersion);

        if (PlayerPrefs.HasKey(databaseName))
        {
            LoadDatabase();
        }
        else
        {
            databaseStruct = new DatabaseStruct
            {
                hatchingEgg = RarityType.None,
                lockSpeed = -1
            };
            /*for (int i = 0; i < 28; i++)
            {
                databaseStruct.creatureIndexes.Add(i);
                databaseStruct.indexIDs.Add(-1);
            }*/
            if (BaseUtils.offlineMode)
            {
                databaseStruct.playerAccount = "Offline Player";
                databaseStruct.currentRank = 800;
                databaseStruct.pixelTokens = 9999999;
                databaseStruct.hasSungen = true;
                databaseStruct.sungenTimer = DateTime.Now.AddDays(30);
            }
            SaveDatabase();
        }
    }
    private void LoadDatabase()
    {
        databaseStruct = XMLGenerator.DeserializeObject(PlayerPrefs.GetString(databaseName), typeof(DatabaseStruct)) as DatabaseStruct;
        /*if (databaseStruct.creatureIndexes == null || databaseStruct.creatureIndexes.Count < databaseStruct.ownedCreatures.Count)
        {
            databaseStruct.creatureIndexes = new List<int>();
            for (int i = 0; i < databaseStruct.ownedCreatures.Count; i++)
            {
                databaseStruct.creatureIndexes.Add(i);
            }
        }*/
    }
    public static void SaveDatabase()
    {
        PlayerPrefs.SetString(databaseName, XMLGenerator.SerializeObject(databaseStruct, typeof(DatabaseStruct)));
    }
    public static void AddNewCreature(CreatureStruct creatureStruct)
    {
        databaseStruct.ownedCreatures.Add(creatureStruct);
        RegisterNewCreatures();
    }
    public static void RegisterNewCreatures()
    {
        /*for (int i = 0; i < databaseStruct.ownedCreatures.Count; i++)
        {
            if (!databaseStruct.indexIDs.Contains(databaseStruct.ownedCreatures[i].creatureID))
            {
                List<VisualCreatureStruct> emptyVisualIndexes = new List<VisualCreatureStruct>();
                for (int k = 0; k < 28; k++)
                {
                    if (databaseStruct.indexIDs[k] == -1)
                    {
                        emptyVisualIndexes.Add(new VisualCreatureStruct(databaseStruct.creatureIndexes[k], k));
                    }
                }
                emptyVisualIndexes.Sort((x, y) => x.visualIndex.CompareTo(y.visualIndex));

                //get its index
                //int thisCreatureIndex = databaseStruct.ownedCreatures.Count - 1;
                //set its indexID
                databaseStruct.indexIDs[i] = databaseStruct.ownedCreatures[i].creatureID;
                //gets the visual index of this item
                int fromVisualIndex = databaseStruct.creatureIndexes[i];
                //gets the first empty visualIndex
                int gotoVisualIndex = emptyVisualIndexes[0].visualIndex;
                //sets the empty visual index to this one
                databaseStruct.creatureIndexes[emptyVisualIndexes[0].indexIndex] = fromVisualIndex;
                //sets this visual index to the empty one
                databaseStruct.creatureIndexes[i] = gotoVisualIndex;
            }
        }*/
    }
    public static void RegisterSelected()
    {
        bool hasPlayer = false;
        for (int i = 0; i < databaseStruct.rosterSlots.Count; i++)
        {
            if (databaseStruct.rosterSlots[i].playerAccount == databaseStruct.playerAccount)
            {
                hasPlayer = true;
                break;
            }
        }
        if (!hasPlayer)
        {
            databaseStruct.rosterSlots.Add(new SelectStruct(databaseStruct.playerAccount, new int[3] { -1, -1, -1 }));
        }
    }
    public static void SetSelected(int[] selectIndexes)
    {
        bool hasPlayer = false;
        for (int i = 0; i < databaseStruct.rosterSlots.Count; i++)
        {
            if (databaseStruct.rosterSlots[i].playerAccount == databaseStruct.playerAccount)
            {
                databaseStruct.rosterSlots[i] = new SelectStruct(databaseStruct.playerAccount, selectIndexes);
                hasPlayer = true;
                break;
            }
        }
        if (!hasPlayer)
        {
            databaseStruct.rosterSlots.Add(new SelectStruct(databaseStruct.playerAccount, selectIndexes));
        }
    }
    public static int GetSelectUnit(int index)
    {
        for (int i = 0; i < databaseStruct.rosterSlots.Count; i++)
        {
            if (databaseStruct.rosterSlots[i].playerAccount == databaseStruct.playerAccount)
            {
                return databaseStruct.rosterSlots[i].selectCreatures[index];
            }
        }
        return -1;
    }
    public static void SetCreatureSellPrice(int databaseIndex, StateType stateType, int priceOffered)
    {
        databaseStruct.ownedCreatures[databaseIndex] = new CreatureStruct(
            databaseStruct.ownedCreatures[databaseIndex].creatureID,
            databaseStruct.ownedCreatures[databaseIndex].experience,
            databaseStruct.ownedCreatures[databaseIndex].level,
            databaseStruct.ownedCreatures[databaseIndex].trainLevel,
            databaseStruct.ownedCreatures[databaseIndex].powerLevel,
            priceOffered,
            databaseStruct.ownedCreatures[databaseIndex].stateTimer,
            stateType,
            databaseStruct.ownedCreatures[databaseIndex].rarity,
            databaseStruct.ownedCreatures[databaseIndex].creatureType);
    }
    public static void SetCreatureState(int databaseIndex, StateType stateType)
    {
        databaseStruct.ownedCreatures[databaseIndex] = new CreatureStruct(
            databaseStruct.ownedCreatures[databaseIndex].creatureID,
            databaseStruct.ownedCreatures[databaseIndex].experience,
            databaseStruct.ownedCreatures[databaseIndex].level,
            databaseStruct.ownedCreatures[databaseIndex].trainLevel,
            databaseStruct.ownedCreatures[databaseIndex].powerLevel,
            databaseStruct.ownedCreatures[databaseIndex].sellPrice,
            databaseStruct.ownedCreatures[databaseIndex].stateTimer,
            stateType,
            databaseStruct.ownedCreatures[databaseIndex].rarity,
            databaseStruct.ownedCreatures[databaseIndex].creatureType);
    }
    public static void SetCreatureState(int databaseIndex, StateType stateType, DateTime stateTimer)
    {
        databaseStruct.ownedCreatures[databaseIndex] = new CreatureStruct(
            databaseStruct.ownedCreatures[databaseIndex].creatureID,
            databaseStruct.ownedCreatures[databaseIndex].experience,
            databaseStruct.ownedCreatures[databaseIndex].level,
            databaseStruct.ownedCreatures[databaseIndex].trainLevel,
            databaseStruct.ownedCreatures[databaseIndex].powerLevel,
            databaseStruct.ownedCreatures[databaseIndex].sellPrice,
            stateTimer,
            stateType,
            databaseStruct.ownedCreatures[databaseIndex].rarity,
            databaseStruct.ownedCreatures[databaseIndex].creatureType);
    }
    public static void SetCreatureType(int databaseIndex, CreatureType creatureType)
    {
        databaseStruct.ownedCreatures[databaseIndex] = new CreatureStruct(
            databaseStruct.ownedCreatures[databaseIndex].creatureID,
            databaseStruct.ownedCreatures[databaseIndex].experience,
            databaseStruct.ownedCreatures[databaseIndex].level,
            databaseStruct.ownedCreatures[databaseIndex].trainLevel,
            databaseStruct.ownedCreatures[databaseIndex].powerLevel,
            databaseStruct.ownedCreatures[databaseIndex].sellPrice,
            databaseStruct.ownedCreatures[databaseIndex].stateTimer,
            databaseStruct.ownedCreatures[databaseIndex].currentState,
            databaseStruct.ownedCreatures[databaseIndex].rarity,
            creatureType);
    }
    public static void SetCreatureTrainLevel(int databaseIndex, int trainLevel)
    {
        databaseStruct.ownedCreatures[databaseIndex] = new CreatureStruct(
            databaseStruct.ownedCreatures[databaseIndex].creatureID,
            databaseStruct.ownedCreatures[databaseIndex].experience,
            databaseStruct.ownedCreatures[databaseIndex].level,
            trainLevel,
            databaseStruct.ownedCreatures[databaseIndex].powerLevel,
            databaseStruct.ownedCreatures[databaseIndex].sellPrice,
            databaseStruct.ownedCreatures[databaseIndex].stateTimer,
            databaseStruct.ownedCreatures[databaseIndex].currentState,
            databaseStruct.ownedCreatures[databaseIndex].rarity,
            databaseStruct.ownedCreatures[databaseIndex].creatureType);
    }
    public static void SetCreatureLevel(int databaseIndex, int level)
    {
        databaseStruct.ownedCreatures[databaseIndex] = new CreatureStruct(
            databaseStruct.ownedCreatures[databaseIndex].creatureID,
            databaseStruct.ownedCreatures[databaseIndex].experience,
            level,
            databaseStruct.ownedCreatures[databaseIndex].trainLevel,
            databaseStruct.ownedCreatures[databaseIndex].powerLevel,
            databaseStruct.ownedCreatures[databaseIndex].sellPrice,
            databaseStruct.ownedCreatures[databaseIndex].stateTimer,
            databaseStruct.ownedCreatures[databaseIndex].currentState,
            databaseStruct.ownedCreatures[databaseIndex].rarity,
            databaseStruct.ownedCreatures[databaseIndex].creatureType);
    }
    public static void SetCreatureExperience(int databaseIndex, int experience)
    {
        databaseStruct.ownedCreatures[databaseIndex] = new CreatureStruct(
            databaseStruct.ownedCreatures[databaseIndex].creatureID,
            experience,
            databaseStruct.ownedCreatures[databaseIndex].level,
            databaseStruct.ownedCreatures[databaseIndex].trainLevel,
            databaseStruct.ownedCreatures[databaseIndex].powerLevel,
            databaseStruct.ownedCreatures[databaseIndex].sellPrice,
            databaseStruct.ownedCreatures[databaseIndex].stateTimer,
            databaseStruct.ownedCreatures[databaseIndex].currentState,
            databaseStruct.ownedCreatures[databaseIndex].rarity,
            databaseStruct.ownedCreatures[databaseIndex].creatureType);
    }
    public static bool HasTrainSlot()
    {
        //int trainingAmount = 0;
        for (int i = 0; i < databaseStruct.ownedCreatures.Count; i++)
        {
            if (databaseStruct.ownedCreatures[i].currentState == StateType.Training)
            {
                return false;
            }
        }
        return true;
    }
    public static bool HasSelectSlot()
    {
        int selectAmount = 0;
        for (int i = 0; i < databaseStruct.ownedCreatures.Count; i++)
        {
            if (databaseStruct.ownedCreatures[i].currentState == StateType.Selected)
            {
                selectAmount++;
            }
        }
        return selectAmount < 3;
    }
    private void OnApplicationQuit()
    {
        SaveDatabase();
    }
}
public struct VisualCreatureStruct
{
    public int visualIndex;
    public int indexIndex;

    public VisualCreatureStruct(int visualIndex, int indexIndex)
    {
        this.visualIndex = visualIndex;
        this.indexIndex = indexIndex;
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(Database))]
public class OfflineDatabaseUI : Editor
{
    public override void OnInspectorGUI()
    {
        //Database generator = (Database)target;
        DrawDefaultInspector();
        if (GUILayout.Button("Clear Database"))
        {
            PlayerPrefs.DeleteAll();
        }
    }
}
#endif