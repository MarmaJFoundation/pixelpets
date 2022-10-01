using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnhancedUI;
using EnhancedUI.EnhancedScroller;
using UnityEngine.UI;

public class PetTabController : MonoBehaviour, IEnhancedScrollerDelegate
{
    private SmallList<PetData> petData = new SmallList<PetData>();
    public EnhancedScroller scroller;
    public ScrollRect scrollRect;
    public EnhancedScrollerCellView cellViewPrefab;
    public MainMenuController mainMenuController;
    public RosterController rosterController;
    public CustomText maxCreatureText;
    public CustomText titleText;
    public CustomButton trainingButton;
    public CustomButton injuredButton;
    public CustomButton sellingButton;
    public Image trainingImage;
    public Image injuredImage;
    public Image sellingImage;
    //public static int movingCreature = -1;
    //private Vector3 lastDragPos;
    private readonly List<PetData> baseData = new List<PetData>();
    [HideInInspector]
    public List<int> creaturesToAnimate = new List<int>();
    public void UpdateSortingButtons()
    {
        injuredButton.deactivated = Database.databaseStruct.hideInjured;
        sellingButton.deactivated = Database.databaseStruct.hideSelling;
        trainingButton.deactivated = Database.databaseStruct.hideTraining;
        injuredImage.material = Database.databaseStruct.hideInjured ? BaseUtils.grayscaleUIMat[0] : BaseUtils.normalUIMat;
        sellingImage.material = Database.databaseStruct.hideSelling ? BaseUtils.grayscaleUIMat[0] : BaseUtils.normalUIMat;
        trainingImage.material = Database.databaseStruct.hideTraining ? BaseUtils.grayscaleUIMat[0] : BaseUtils.normalUIMat;
    }

    public void UpdateList(bool keepTabPos = false)
    {
        titleText.SetString($"All Pets: {Database.databaseStruct.ownedCreatures.Count}/28");
        scroller.Delegate = this;
        petData.Clear();
        baseData.Clear();
        /*List<int> takenEmptySpots = new List<int>();
        //add elements and sort?
        for (int i = 0; i < 28; i++)
        {
            bool emptyItem = Database.databaseStruct.ownedCreatures.Count <= i;
            bool creatureSelected = !emptyItem && Database.databaseStruct.ownedCreatures[i].currentState == StateType.Selected;
            int displayIndex = -1;
            for (int k = 0; k < 28; k++)
            {
                if (emptyItem)
                {
                    if (Database.databaseStruct.indexIDs[k] == -1 && !takenEmptySpots.Contains(k))
                    {
                        displayIndex = Database.databaseStruct.creatureIndexes[k];
                        takenEmptySpots.Add(k);
                        break;
                    }
                }
                else
                {
                    if (Database.databaseStruct.indexIDs[k] == Database.databaseStruct.ownedCreatures[i].creatureID)
                    {
                        displayIndex = Database.databaseStruct.creatureIndexes[k];
                        break;
                    }
                }
            }
            if (!emptyItem)
            {
                //Debug.Log(i + "," + Database.databaseStruct.ownedCreatures[i].creatureType);
                int creatureID = Database.databaseStruct.ownedCreatures[i].creatureID;
                baseData.Add(new PetData()
                {
                    databaseIndex = i,
                    displayIndex = displayIndex,
                    creatureID = creatureID,
                    creatureSelected = creatureSelected,
                });
            }
        }
        baseData.Sort((x, y) => x.displayIndex.CompareTo(y.displayIndex));
        for (int i = 0; i < baseData.Count; i++)
        {
            petData.Add(baseData[i]);
        }*/
        for (int i = 0; i < Database.databaseStruct.ownedCreatures.Count; i++)
        {
            StateType creatureState = Database.databaseStruct.ownedCreatures[i].currentState;
            if ((creatureState == StateType.Injured && Database.databaseStruct.hideInjured) ||
                (creatureState == StateType.Training && Database.databaseStruct.hideTraining) ||
                (creatureState == StateType.Selling && Database.databaseStruct.hideSelling))
            {
                continue;
            }
            bool emptyItem = Database.databaseStruct.ownedCreatures.Count <= i;
            bool creatureSelected = !emptyItem && creatureState == StateType.Selected;
            int displayIndex = -1;
            int creatureID = Database.databaseStruct.ownedCreatures[i].creatureID;
            int creatureLevel = Database.databaseStruct.ownedCreatures[i].level;
            baseData.Add(new PetData()
            {
                databaseIndex = i,
                displayIndex = displayIndex,
                creatureID = creatureID,
                creatureLevel = creatureLevel,
                creatureSelected = creatureSelected,
            });
        }
        baseData.Sort((x, y) => y.creatureLevel.CompareTo(x.creatureLevel));
        for (int i = 0; i < baseData.Count; i++)
        {
            petData.Add(baseData[i]);
        }
        scroller.ReloadData(keepTabPos ? scroller.NormalizedScrollPosition : 0);
        UpdateSortingButtons();
    }
    public void OnCreatureClick(int databaseIndex)
    {
        mainMenuController.ShowCard(databaseIndex);
    }
    public void OnSelectClick(PetTabCell petTabCell)
    {
        if (!Database.HasSelectSlot())
        {
            mainMenuController.ShowWarning("Roster full!", "Remove a creature from your roster", "in order to select this one.");
            return;
        }
        if (Database.databaseStruct.ownedCreatures[petTabCell.databaseIndex].currentState == StateType.Training ||
            Database.databaseStruct.ownedCreatures[petTabCell.databaseIndex].currentState == StateType.Injured ||
            Database.databaseStruct.ownedCreatures[petTabCell.databaseIndex].currentState == StateType.Selling)
        {
            mainMenuController.ShowWarning("Not possible!", "Cannot select this creature in this state.", "it may be training or injured.");
            return;
        }
        Database.SetCreatureState(petTabCell.databaseIndex, StateType.Selected);
        rosterController.UpdateSelectCreatures(true);
        UpdateList(true);
    }
    public void OnMoveClick(int dataIndex)
    {
        //movingCreature = dataIndex;
        //UpdateList(true);
    }
    public void SwitchCreatures(PetTabCell petCellA, PetTabCell petCellB)
    {
        //movingCreature = -1;
        /*if (petCellA != petCellB && petCellB != null)
        {
            if (petCellB.rosterSlot != -1)
            {
                if (!petCellB.hasCreature)
                {
                    OnSelectClick(petCellA);
                }
                return;
            }
            //loop here
            int itemIndexA = -1;
            int itemIndexB = -1;
            for (int i = 0; i < 28; i++)
            {
                //find the index of item A
                if (Database.databaseStruct.indexIDs[i] == petCellA.petData.creatureID)
                {
                    itemIndexA = i;
                }
                //find the index of item B
                if (Database.databaseStruct.indexIDs[i] == petCellB.petData.creatureID)
                {
                    itemIndexB = i;
                }
            }
            //Debug.Log(itemIndexA + "," + itemIndexB);
            if (itemIndexA == -1 || itemIndexB == -1)
            {
                Debug.Log("indexes not found");
                return;
            }
            int indexA = Database.databaseStruct.creatureIndexes[itemIndexA];
            Database.databaseStruct.creatureIndexes[itemIndexA] = Database.databaseStruct.creatureIndexes[itemIndexB];
            Database.databaseStruct.creatureIndexes[itemIndexB] = indexA;
        }
        UpdateList(true);*/
    }
    public void OnInjuredClick()
    {
        Database.databaseStruct.hideInjured = !Database.databaseStruct.hideInjured;
        injuredButton.deactivated = Database.databaseStruct.hideInjured;
        injuredImage.material = Database.databaseStruct.hideInjured ? BaseUtils.grayscaleUIMat[0] : BaseUtils.normalUIMat;
        Database.SaveDatabase();
        UpdateList(true);
    }
    public void OnSellingClick()
    {
        Database.databaseStruct.hideSelling = !Database.databaseStruct.hideSelling;
        sellingButton.deactivated = Database.databaseStruct.hideSelling;
        sellingImage.material = Database.databaseStruct.hideSelling ? BaseUtils.grayscaleUIMat[0] : BaseUtils.normalUIMat;
        Database.SaveDatabase();
        UpdateList(true);
    }
    public void OnTrainingClick()
    {
        Database.databaseStruct.hideTraining = !Database.databaseStruct.hideTraining;
        trainingButton.deactivated = Database.databaseStruct.hideTraining;
        trainingImage.material = Database.databaseStruct.hideTraining ? BaseUtils.grayscaleUIMat[0] : BaseUtils.normalUIMat;
        Database.SaveDatabase();
        UpdateList(true);
    }
    public void OnExitClick(int rosterSlot)
    {
        mainMenuController.OnRemoveClick(rosterSlot);
    }
    /*public void OnMoveClick(int direction, int databaseIndex)
    {
        int valueIndex = Database.databaseStruct.creaturesIndexes[databaseIndex];
        int listIndex = Mathf.Clamp(databaseIndex + direction, 0, Database.databaseStruct.creaturesIndexes.Count - 1);
        Database.databaseStruct.creaturesIndexes.RemoveAt(databaseIndex);
        Database.databaseStruct.creaturesIndexes.Insert(listIndex, valueIndex);
        UpdateList(true);
        rosterController.UpdateSelectCreatures();
    }*/
    #region EnhancedScroller Callbacks
    public int GetNumberOfCells(EnhancedScroller scroller)
    {
        return petData.Count;
    }
    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
    {
        return 56f;
    }
    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        PetTabCell cellView = scroller.GetCellView(cellViewPrefab) as PetTabCell;
        cellView.name = "petCell";
        cellView.SetData(this, dataIndex, petData[dataIndex]);
        return cellView;
    }
    #endregion
}
public class PetData
{
    public int databaseIndex;
    public int displayIndex;
    public int creatureID;
    public int creatureLevel;
    public bool creatureSelected;
}