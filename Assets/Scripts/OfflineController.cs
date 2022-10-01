using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfflineController : MonoBehaviour
{
    public MainMenuController mainMenuController;
    public EggController eggController;
    public SungenController sungenController;
    public PostScreenController postScreenController;
    //this decides how fast server updates the timer
    private readonly int timerUpdateInterval = 1;
    private float timerDelay;
    //private bool lostFocus;
    //private float focusTimer;
    private void Start()
    {
        timerDelay = timerUpdateInterval;
    }
    private void Update()
    {
        /*if (BaseUtils.offlineMode)
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                FindObjectOfType<CardController>().SetCreatureInfo(CreatureType.Ignim, RarityType.Rare, false);
                FindObjectOfType<MainMenuController>().StartCoroutine(FindObjectOfType<MainMenuController>().AnimateEvolution(CreatureType.Ignim));
            }
        }*/
        if (!BaseUtils.syncedDatabase || BaseUtils.onFight)
        {
            return;
        }
        /*if (lostFocus)
        {
            focusTimer += Time.deltaTime;
        }*/
        if (timerDelay <= timerUpdateInterval)
        {
            timerDelay += Time.deltaTime;
            return;
        }
        timerDelay = 0;
        UpdateTimers(/*timerUpdateInterval*/);
    }
    public void UpdateTimers(/*float interval*/)
    {
        if (Database.databaseStruct.hatchingEgg != RarityType.None)
        {
            //Database.databaseStruct.hatchingTimer -= interval;
            float hatchingTime = Database.databaseStruct.hatchingTimer.ToFlatTime();
            if (hatchingTime <= 0)
            {
                if (BaseUtils.offlineMode)
                {
                    //CreatureStruct creatureStruct = BaseUtils.GenerateCreatureStruct((int)Database.databaseStruct.hatchingEgg);
                    eggController.SyncEggHatch((int)Database.databaseStruct.hatchingEgg);
                }
                else
                {
                    eggController.SyncEggTimers((int)Database.databaseStruct.hatchingEgg, 0);
                }
            }
            else
            {
                eggController.SyncEggTimers((int)Database.databaseStruct.hatchingEgg, hatchingTime);
            }
        }
        for (int i = 0; i < Database.databaseStruct.ownedCreatures.Count; i++)
        {
            float stateTimer = Database.databaseStruct.ownedCreatures[i].stateTimer.ToFlatTime();
            if (Database.databaseStruct.ownedCreatures[i].currentState == StateType.Training)
            {
                if (stateTimer <= 0)
                {
                    Database.SetCreatureState(i, StateType.InPool);
                    mainMenuController.SyncCreatureTrain(i);
                }
                else
                {
                    Database.SetCreatureState(i, Database.databaseStruct.ownedCreatures[i].currentState);
                    mainMenuController.UpdateTrainTimers(i);
                }
            }
            else if (Database.databaseStruct.ownedCreatures[i].currentState == StateType.Injured)
            {
                if (stateTimer <= 0)
                {
                    Database.SetCreatureState(i, StateType.InPool);
                    mainMenuController.SyncCreatureInjured(i);
                }
                else
                {
                    Database.SetCreatureState(i, Database.databaseStruct.ownedCreatures[i].currentState);
                    mainMenuController.UpdateInjuredTimers(i);
                }
            }
        }
        if (Database.databaseStruct.hasSungen)
        {
            sungenController.UpdateSungenTimer(Database.databaseStruct.sungenTimer.ToFlatTime());
        }
    }
}
