using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RosterController : MonoBehaviour
{
    public MainMenuController mainMenuController;
    public PetTabCell[] tabCells;
    public GameObject[] removeButtons;
    public CustomTooltip loadoutTooltip;
    public RosterCellController[] rosterCellControllers;
    public CustomText rankText;
    public Image rankBorder;
    public Image rankTrophy;
    public CustomTooltip decayTooltip;
    public void UpdateInfo()
    {
        if ((DateTime.Now - Database.databaseStruct.lastFight).TotalHours >= 12)
        {
            decayTooltip.gameObject.SetActive(true);
            if ((DateTime.Now - Database.databaseStruct.lastFight).TotalHours >= 24)
            {
                int elapsedDays = Mathf.Clamp(Database.databaseStruct.elapsedDays, 1, 31);
                int penalty;
                if (elapsedDays >= 30)
                {
                    penalty = Database.databaseStruct.currentRank - 800;
                }
                else
                {
                    penalty = (int)Math.Pow(2, elapsedDays);
                }
                int penaltyFuture;
                if (elapsedDays + 1 > 30)
                {
                    penaltyFuture = Database.databaseStruct.currentRank - 800;
                }
                else
                {
                    penaltyFuture = (int)Math.Pow(2, elapsedDays + 1);
                }
                decayTooltip.tooltipText = new string[3] { "Warning!", $"You havent played a match in more than one day", $"You rank has decayed by {penalty} and will decay by {penaltyFuture} in another day" };
            }
            else
            {
                decayTooltip.tooltipText = new string[3] { "Warning!", $"You havent played a match in more than 12 hours", $"Your rank will decay in {Mathf.Round((24 - (float)(DateTime.Now - Database.databaseStruct.lastFight).TotalHours) * 10) / 10} hours" };
            }
        }
        else
        {
            decayTooltip.gameObject.SetActive(false);
        }
        rankText.SetString($"Rank: {Database.databaseStruct.currentRank}");
        int rankIndex = Database.databaseStruct.currentRank.ToRankIndex();
        rankBorder.sprite = BaseUtils.backSprites[rankIndex];
        rankBorder.material = rankIndex >= 5 ? BaseUtils.glowUIMat : BaseUtils.normalUIMat;
        rankTrophy.sprite = BaseUtils.trophySprites[rankIndex];
        rankTrophy.SetNativeSize();
        ShowTooltip();
    }

    private void ShowTooltip()
    {
        bool hasAllSelected = mainMenuController.selectedCreatures[0] != -1 && mainMenuController.selectedCreatures[1] != -1 && mainMenuController.selectedCreatures[2] != -1;
        loadoutTooltip.gameObject.SetActive(hasAllSelected);
        if (hasAllSelected)
        {
            Vector2Int eloRange = BaseUtils.GetModifiedElo(
                Database.databaseStruct.currentRank,
                Database.databaseStruct.ownedCreatures[mainMenuController.selectedCreatures[0]],
                Database.databaseStruct.ownedCreatures[mainMenuController.selectedCreatures[1]],
                Database.databaseStruct.ownedCreatures[mainMenuController.selectedCreatures[2]]);
            Vector2Int wonRange = BaseUtils.GetAddedElo(Database.databaseStruct.currentRank, eloRange);
            loadoutTooltip.tooltipText = new string[3] { "You will fight oponnents", $"Within {eloRange.x}-{eloRange.y} rank", wonRange.x == wonRange.y ? $"And gain {wonRange.x} rank if won" : $"And gain {wonRange.x}-{wonRange.y} Elo if won" };
        }
    }

    public void UpdateSelectCreatures(bool bumpObj = false)
    {
        List<int> previouslySelected = new List<int>();
        for (int i = 0; i < 3; i++)
        {
            previouslySelected.Add(mainMenuController.selectedCreatures[i]);
            mainMenuController.selectedCreatures[i] = -1;
        }
        for (int k = 0; k < 3; k++)
        {
            bool foundSelected = false;
            for (int i = 0; i < Database.databaseStruct.ownedCreatures.Count; i++)
            {
                if (Database.databaseStruct.ownedCreatures[i].currentState == StateType.Selected && System.Array.IndexOf(mainMenuController.selectedCreatures, i) == -1)
                {
                    rosterCellControllers[k].Setup(i);
                    removeButtons[k].SetActive(true);
                    if (bumpObj && !previouslySelected.Contains(i))
                    {
                        mainMenuController.BumpObj(rosterCellControllers[k].transform);
                    }
                    tabCells[k].hasCreature = true;
                    //tabRemoveButtons[k].SetActive(true);
                    mainMenuController.selectedCreatures[k] = i;
                    foundSelected = true;
                    break;
                }
            }
            if (!foundSelected)
            {
                removeButtons[k].SetActive(false);
                rosterCellControllers[k].Clear();
                tabCells[k].hasCreature = false;
                //tabRemoveButtons[k].SetActive(false);
            }
        }
        Database.SetSelected(mainMenuController.selectedCreatures);
        Database.SaveDatabase();
        ShowTooltip();
    }
}
