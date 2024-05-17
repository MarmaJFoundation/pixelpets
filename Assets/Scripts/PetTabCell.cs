using EnhancedUI.EnhancedScroller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public delegate void SelectedDelegate(EnhancedScrollerCellView cellView);

public class PetTabCell : EnhancedScrollerCellView, IPointerEnterHandler, IPointerExitHandler
{
    public PetData petData;
    public Image borderImage;
    public Image edgeImage;
    public Image petImage;
    public GameObject shuffleButton;
    public GameObject enqueueButton;
    public CustomText trainingText;
    public CustomText damageText;
    public CustomText defenseText;
    public CustomText speedText;
    public CustomText magicText;
    public CustomText lvlText;
    public CustomText pLvlText;
    public CustomText tLvlText;
    public RectTransform innerCard;
    [HideInInspector]
    public int databaseIndex;
    public int rosterSlot;
    public bool hasCreature;
    public bool mergeCell;
    public StateType cellState;
    public int DataIndex { get; private set; }
    private PetTabController petTabController;
    private MergeController mergeController;
    public void SetData(PetTabController petTabController, int dataIndex, PetData petData)
    {
        this.petTabController = petTabController;
        this.petData = petData;
        DataIndex = dataIndex;
        databaseIndex = petData.databaseIndex;
        if (enqueueButton != null)
        {
            //shuffleButton.SetActive(false);
            enqueueButton.SetActive(false);
        }
        //borderImage.material = PetTabController.movingCreature == dataIndex ? BaseUtils.outlineUIMat : BaseUtils.normalUIMat;
        //innerCard.anchoredPosition = PetTabController.movingCreature == dataIndex ? Vector2.left * 15 : Vector2.zero;
        SetCreatureImages(petData.databaseIndex, petData.creatureSelected);
        if (petTabController.creaturesToAnimate.Contains(databaseIndex))
        {
            petTabController.creaturesToAnimate.Remove(databaseIndex);
            petTabController.mainMenuController.BumpObj(transform);
            petTabController.mainMenuController.ShowEffect(EffectType.Hit, transform);
            petTabController.mainMenuController.ShowEffect(EffectType.SmallGlowNormal, transform);
        }
    }
    public void Setup(MergeController mergeController, PetData petData)
    {
        this.petData = petData;
        this.mergeController = mergeController;
        SetCreatureImages(petData.databaseIndex, petData.creatureSelected);
    }
    public void SetCreatureImages(int databaseIndex, bool creatureSelected)
    {
        CreatureType creatureType = Database.databaseStruct.ownedCreatures[databaseIndex].creatureType;
        RarityType rarityType = Database.databaseStruct.ownedCreatures[databaseIndex].rarity;
        ScriptableCreature scriptableCreature = BaseUtils.creatureDict[creatureType];
        petImage.sprite = scriptableCreature.creatureSprite;
        petImage.transform.localPosition = scriptableCreature.frameOffset * .5f;
        StateType stateType = Database.databaseStruct.ownedCreatures[databaseIndex].currentState;
        if (creatureSelected)
        {
            petImage.material = BaseUtils.GetGrayMaterial(StateType.Selected, false);
            //selectObj.SetActive(false);
            if (trainingText != null)
            {
                trainingText.gameObject.SetActive(true);
                trainingText.SetString("Selected");
            }
            cellState = StateType.Selected;
        }
        else if (stateType == StateType.Training ||
            stateType == StateType.Injured ||
            stateType == StateType.Selling)
        {
            petImage.material = BaseUtils.GetGrayMaterial(stateType, false);
            //selectObj.SetActive(false);
            if (trainingText != null)
            {
                trainingText.gameObject.SetActive(true);
                trainingText.SetString(Database.databaseStruct.ownedCreatures[databaseIndex].currentState.ToString());
            }
            cellState = stateType;
        }
        else
        {
            petImage.material = BaseUtils.normalUIMat;
            //selectObj.SetActive(true);
            if (trainingText != null)
            {
                trainingText.gameObject.SetActive(false);
            }
            cellState = StateType.InPool;
        }
        int level = Database.databaseStruct.ownedCreatures[databaseIndex].level;
        int trainLevel = Database.databaseStruct.ownedCreatures[databaseIndex].trainLevel;
        int powerLevel = Database.databaseStruct.ownedCreatures[databaseIndex].powerLevel;
        int rarity = (int)Database.databaseStruct.ownedCreatures[databaseIndex].rarity + 1;
        int evolution = scriptableCreature.evolution;
        int trueTrainLevel = Database.databaseStruct.ownedCreatures[databaseIndex].currentState == StateType.Training ? trainLevel - 1 : trainLevel;
        int damage = BaseUtils.GetModifiedStat(scriptableCreature.damage, rarity, trueTrainLevel, level, evolution, powerLevel);
        int speed = BaseUtils.GetModifiedStat(scriptableCreature.speed, rarity, trueTrainLevel, level, evolution, powerLevel);
        int magic = BaseUtils.GetModifiedStat(scriptableCreature.magic, rarity, trueTrainLevel, level, evolution, powerLevel);
        int defense = BaseUtils.GetModifiedStat(scriptableCreature.defense, rarity, trueTrainLevel, level, evolution, powerLevel);
        lvlText.SetString($"lvl:{level}");
        tLvlText.SetString($"tlvl:{trueTrainLevel}");
        pLvlText.SetString($"plvl:{powerLevel}");
        damageText.SetString(damage.ToString());
        speedText.SetString(speed.ToString());
        magicText.SetString(magic.ToString());
        defenseText.SetString(defense.ToString());
        edgeImage.sprite = BaseUtils.borderSprites[(int)rarityType];
    }

    public void OnRightClick()
    {
        /*if (PetTabController.movingCreature != -1)
        {
            petTabController.SwitchCreatures(this, petTabController.scroller.GetCellViewAtDataIndex(PetTabController.movingCreature) as PetTabCell);
            return;
        }*/
        if (mergeCell)
        {
            mergeController.OnTabClick(this);
            return;
        }
        petTabController.OnSelectClick(this);
    }
    public void OnCreatureClick()
    {
        *if (PetTabController.movingCreature != -1)
        {
            petTabController.SwitchCreatures(this, petTabController.scroller.GetCellViewAtDataIndex(PetTabController.movingCreature) as PetTabCell);
            return;
        }*/
        if (mergeCell)
        {
            mergeController.OnTabClick(this);
            return;
        }
        petTabController.OnCreatureClick(petData.databaseIndex);
    }
    public void OnMoveClick()
    {
        petTabController.OnMoveClick(dataIndex);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (enqueueButton != null)
        {
            //shuffleButton.SetActive(true);
            enqueueButton.SetActive(true);
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (enqueueButton != null)
        {
            //shuffleButton.SetActive(false);
            enqueueButton.SetActive(false);
        }
    }
}
