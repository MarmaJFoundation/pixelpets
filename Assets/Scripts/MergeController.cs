using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MergeController : MonoBehaviour
{
    public NearHelper nearHelper;
    public MainMenuController mainMenuController;
    public GameObject mergeButton;
    public GameObject warningObj;
    public RosterCellController[] rosterCellControllers;
    public PetTabCell[] rosterTabCellControllers;
    public PetTabCell[] tabCellControllers;
    public Image resultPetBorder;
    public Image creaturePreview;
    public Image creaturePreviewEdge;
    public GameObject[] buttonObjs;
    public RectTransform creaturePreviewRect;
    private int currentPage;
    private readonly List<int> availableCreatures = new List<int>();
    public bool showingPreview;
    private bool showCombine;
    private float glowTimer;
    private int mergingID;
    public void Setup()
    {
        currentPage = 0;
        availableCreatures.Clear();
        for (int i = 0; i < Database.databaseStruct.ownedCreatures.Count; i++)
        {
            if (Database.databaseStruct.ownedCreatures[i].currentState == StateType.InPool || Database.databaseStruct.ownedCreatures[i].currentState == StateType.Selected)
            {
                availableCreatures.Add(i);
            }
        }
        for (int i = 0; i < 3; i++)
        {
            tabCellControllers[i].gameObject.SetActive(i < availableCreatures.Count);
        }
        ClearAll();
    }
    public void SetTabCellsPage(int direction)
    {
        bool cappedLeft = currentPage + direction < 0;
        bool cappedRight = currentPage + direction > availableCreatures.Count - 1;
        buttonObjs[0].SetActive(currentPage + direction > 0);
        buttonObjs[1].SetActive(currentPage + direction < availableCreatures.Count - 3);
        if (cappedLeft || cappedRight)
        {
            return;
        }
        currentPage += direction;
        for (int i = 0; i < 3; i++)
        {
            int goIndex = currentPage + i;
            if (goIndex < 0)
            {
                goIndex = 0;
            }
            if (goIndex >= availableCreatures.Count - 1)
            {
                goIndex = availableCreatures.Count - 1;
            }
            tabCellControllers[i].Setup(this, new PetData()
            {
                databaseIndex = availableCreatures[goIndex],
                creatureSelected = rosterCellControllers[0].databaseIndex == availableCreatures[goIndex] || rosterCellControllers[1].databaseIndex == availableCreatures[goIndex]
            });
        }
    }
    public void OnTabClick(PetTabCell petTabCell)
    {
        if (showCombine)
        {
            return;
        }
        if (rosterCellControllers[0].databaseIndex == -1 && rosterCellControllers[1].databaseIndex != petTabCell.petData.databaseIndex)
        {
            OnMergeDrop(petTabCell, rosterTabCellControllers[0]);
            return;
        }
        if (rosterCellControllers[1].databaseIndex == -1 && rosterCellControllers[0].databaseIndex != petTabCell.petData.databaseIndex)
        {
            OnMergeDrop(petTabCell, rosterTabCellControllers[1]);
            return;
        }
    }
    public void OnPrimaryExitClick()
    {
        rosterCellControllers[0].Clear();
        UpdateResult();
    }
    public void OnSecondaryExitClick()
    {
        rosterCellControllers[1].Clear();
        UpdateResult();
    }
    public void OnMergeClick()
    {
        mainMenuController.ShowWarning($"Wait!", "Are you sure you want to merge your", "pets? You cannot undo this action!", OnMergeResponse);
    }
    private void OnMergeResponse(bool accepted)
    {
        if (accepted)
        {
            mergingID = Database.databaseStruct.ownedCreatures[rosterCellControllers[0].databaseIndex].creatureID;
            StartCoroutine(nearHelper.RequestMergePet(
                Database.databaseStruct.ownedCreatures[rosterCellControllers[0].databaseIndex].creatureID,
                Database.databaseStruct.ownedCreatures[rosterCellControllers[1].databaseIndex].creatureID));
        }
    }
    public void OnMergeCallback(bool success)
    {
        if (success)
        {
            nearHelper.dataGetState = DataGetState.AfterMerge;
            StartCoroutine(nearHelper.GetPlayerData());
        }
        else
        {
            mainMenuController.ShowWarning("Error on merging", "Merging your creature failed", "please try again!");
            ClearAll();
        }
    }
    public void OnReceiveMergeData()
    {
        mainMenuController.UpdatePlayerInfo();
        for (int i = 0; i < Database.databaseStruct.ownedCreatures.Count; i++)
        {
            if (Database.databaseStruct.ownedCreatures[i].creatureID == mergingID)
            {
                mainMenuController.ShowNewCreature(i, "you merged your creatures!");
                break;
            }
        }
        Setup();
    }
    private void ClearAll()
    {
        for (int i = 0; i < 3; i++)
        {
            rosterCellControllers[i].Clear();
        }
        resultPetBorder.material = BaseUtils.normalUIMat;
        showCombine = false;
        mergeButton.SetActive(false);
        warningObj.SetActive(false);
        mergingID = -1;
        SetTabCellsPage(0);
    }
    private void LateUpdate()
    {
        if (creaturePreviewRect.gameObject.activeSelf)
        {
            creaturePreviewRect.position = BaseUtils.mainCam.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * 50);
        }
        if (showCombine)
        {
            if (glowTimer < 4)
            {
                glowTimer += Time.deltaTime;
            }
            else
            {
                mainMenuController.ShowEffect((EffectType)(15 + (int)Database.databaseStruct.ownedCreatures[rosterCellControllers[0].databaseIndex].rarity), rosterCellControllers[2].transform, Vector3.down * 50);
                glowTimer = 0;
            }
        }
    }
    public void BeginDrag(PetData petData)
    {
        if (showingPreview)
        {
            return;
        }
        showingPreview = true;
        creaturePreviewRect.gameObject.SetActive(true);
        CreatureType creatureType = Database.databaseStruct.ownedCreatures[petData.databaseIndex].creatureType;
        RarityType rarityType = Database.databaseStruct.ownedCreatures[petData.databaseIndex].rarity;
        ScriptableCreature scriptableCreature = BaseUtils.creatureDict[creatureType];
        creaturePreview.sprite = scriptableCreature.creatureSprite;
        creaturePreview.transform.localPosition = scriptableCreature.frameOffset * .5f;
        creaturePreview.sprite = BaseUtils.creatureDict[Database.databaseStruct.ownedCreatures[petData.databaseIndex].creatureType].creatureSprite;
        creaturePreviewEdge.sprite = BaseUtils.borderSprites[(int)rarityType];
    }
    public void EndDrag()
    {
        showingPreview = false;
        creaturePreviewRect.gameObject.SetActive(false);
    }
    public void OnMergeDrop(PetTabCell petCellA, PetTabCell petCellB)
    {
        int databaseIndex = petCellA.petData.databaseIndex;
        rosterCellControllers[petCellB.rosterSlot].Setup(databaseIndex);
        mainMenuController.ShowEffect((EffectType)(15 + (int)Database.databaseStruct.ownedCreatures[databaseIndex].rarity), rosterCellControllers[petCellB.rosterSlot].transform, Vector3.up * 50);
        UpdateResult();
    }
    private void UpdateResult()
    {
        if (rosterCellControllers[0].databaseIndex != -1 && rosterCellControllers[1].databaseIndex != -1)
        {
            if (Database.databaseStruct.ownedCreatures[rosterCellControllers[0].databaseIndex].rarity != Database.databaseStruct.ownedCreatures[rosterCellControllers[1].databaseIndex].rarity)
            {
                mainMenuController.ShowWarning("Not possible!", "Sorry, but you need to add two creatures", "That are of the same rarity.");
                ClearAll();
                return;
            }
            if (Database.databaseStruct.ownedCreatures[rosterCellControllers[0].databaseIndex].creatureType.ToBaseCreatureType() !=
                Database.databaseStruct.ownedCreatures[rosterCellControllers[1].databaseIndex].creatureType.ToBaseCreatureType())
            {
                mainMenuController.ShowWarning("Not possible!", "Sorry, but you need to add two creatures", "That are of the same type.");
                ClearAll();
                return;
            }
            rosterCellControllers[2].Setup(rosterCellControllers[0].databaseIndex, rosterCellControllers[1].databaseIndex);
            resultPetBorder.material = BaseUtils.glowUIMat;
            showCombine = true;
            mergeButton.SetActive(true);
            warningObj.SetActive(rosterCellControllers[0].overallStats < rosterCellControllers[1].overallStats);
        }
        else
        {
            showCombine = false;
            mergeButton.SetActive(false);
            warningObj.SetActive(false);
            rosterCellControllers[2].Clear();
            resultPetBorder.material = BaseUtils.normalUIMat;
        }
        SetTabCellsPage(0);
    }
}
