using EnhancedUI;
using EnhancedUI.EnhancedScroller;
using System.Collections.Generic;
using UnityEngine;

public class DropdownController : MonoBehaviour, IEnhancedScrollerDelegate
{
    private SmallList<DropdownData> elementData = new SmallList<DropdownData>();
    public EnhancedScroller scroller;
    public EnhancedScrollerCellView cellViewPrefab;
    public MarketController marketController;
    public CustomInput customInput;
    public int numberOfCellsPerRow = 3;
    public void Setup()
    {
        gameObject.SetActive(true);
        scroller.Delegate = this;
        elementData.Clear();
        /*for (int i = 0; i < BaseUtils.allCreatures.Length; i++)
        {
            elementData.Add(new DropdownData()
            {
                creatureType = BaseUtils.allCreatures[i],
                rarity = BaseUtils.GetRarity(BaseUtils.allCreatures[i]),
                available = true
            });
        }*/
        scroller.ReloadData();
    }
    public void SetAvailablePets(List<CreatureType> availablePets)
    {
        elementData.Clear();
        for (int i = 0; i < 3; i++)
        {
            elementData.Add(new DropdownData { empty = i != 1, isSpecial = i == 1 });
        }
        for (int i = 0; i < BaseUtils.allCreatures.Length; i++)
        {
            if (availablePets.Contains(BaseUtils.allCreatures[i]))
            {
                elementData.Add(new DropdownData()
                {
                    creatureType = BaseUtils.allCreatures[i],
                    rarity = BaseUtils.GetRarity(BaseUtils.allCreatures[i]),
                    //available = availablePets.Contains(BaseUtils.allCreatures[i])
                });
            }
        }
        scroller.ReloadData();
    }
    public void Dispose()
    {
        gameObject.SetActive(false);
    }
    /*private void LateUpdate()
    {
        if (eventSystem.currentSelectedGameObject != gameObject &&
            eventSystem.currentSelectedGameObject != customInput.gameObject &&
            eventSystem.currentSelectedGameObject != barObj)
        {
            gameObject.SetActive(false);
        }
    }*/
    public void OnDropdownClick(string creatureName)
    {
        customInput.SetInputText(creatureName);
        gameObject.SetActive(false);
        //onCellClick.Invoke();
    }
    #region EnhancedScroller Callbacks
    public int GetNumberOfCells(EnhancedScroller scroller)
    {
        return Mathf.CeilToInt((float)elementData.Count / (float)numberOfCellsPerRow);
    }
    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
    {
        return 35f;
    }
    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        DropdownCellRow cellView = scroller.GetCellView(cellViewPrefab) as DropdownCellRow;
        var di = dataIndex * numberOfCellsPerRow;

        cellView.name = "Cell " + (di).ToString() + " to " + ((di) + numberOfCellsPerRow - 1).ToString();

        // pass in a reference to our data set with the offset for this cell
        cellView.SetData(ref elementData, marketController, di);

        // return the cell to the scroller
        return cellView;
    }
    #endregion
}
public class DropdownData
{
    public CreatureType creatureType;
    public RarityType rarity;
    public bool available;
    public bool empty;
    public bool isSpecial;
}