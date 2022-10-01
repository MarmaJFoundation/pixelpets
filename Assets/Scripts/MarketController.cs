using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EnhancedUI;
using EnhancedUI.EnhancedScroller;

public enum SortType
{
    Level = 0,
    Stats = 1,
    Rarity = 2,
    Price = 3,
    TrainLevel = 4
}
public class MarketController : MonoBehaviour, IEnhancedScrollerDelegate
{
    private SmallList<MarketData> elementData = new SmallList<MarketData>();
    private readonly SmallList<MarketData> baseData = new SmallList<MarketData>();
    public EnhancedScroller scroller;
    public EnhancedScrollerCellView cellViewPrefab;
    public int numberOfCellsPerRow = 3;
    public Sprite emptyBorderSprite;
    public Sprite emptyTypeSprite;
    public CustomInput customInput;
    public CustomText marketTitle;
    private MarketData creatureToBuy;
    public DropdownController dropdownController;
    public GameObject authWindow;
    public GameObject dropdownWindow;
    public GameObject dockButton;
    public GameObject dropButton;
    public GameObject sortingButtons;
    public GameObject marketRow;
    public MainMenuController mainMenuController;
    public NearHelper nearHelper;
    public Image[] sortButtons;
    public Image[] highLowButtons;
    private SortType sortType;
    private bool lowToHigh;
    private string sortString;
    private List<CreatureType> availablePets = new List<CreatureType>();
    public void Setup(bool resetSorting = true)
    {
        /*if (resetSorting)
        {
            lowToHigh = true;
            sortType = SortType.Level;
            for (int i = 0; i < sortButtons.Length; i++)
            {
                sortButtons[i].material = BaseUtils.normalUIMat;
            }
            sortButtons[0].material = BaseUtils.outlineUIMat;
            highLowButtons[0].material = BaseUtils.normalUIMat;
            highLowButtons[1].material = BaseUtils.outlineUIMat;
        }*/
        OnDropClick();
        marketTitle.gameObject.SetActive(true);
        marketTitle.SetString("Click the creature you want to search for");
        dropdownController.Setup();
        customInput.Setup();
        scroller.Delegate = this;
        sortString = "";
        elementData.Clear();
        if (BaseUtils.offlineMode)
        {
            for (int i = 0; i < 100; i++)
            {
                baseData.Add(new MarketData()
                {
                    creatureType = BaseUtils.GetRandomCreature(),
                    rarityType = (RarityType)BaseUtils.RandomInt(0, 3),
                    level = BaseUtils.RandomInt(0, 20),
                    trainLevel = BaseUtils.RandomInt(0, 20),
                    price = BaseUtils.RandomInt(100, 10000),
                    creatureID = BaseUtils.RandomInt(-100000, 100000)
                });
            }
            SortData();
        }
    }
    public void OnReceiveAvailablePets(List<int> availablePets)
    {
        this.availablePets.Clear();
        for (int i = 0; i < availablePets.Count; i++)
        {
            this.availablePets.Add((CreatureType)availablePets[i]);
        }
        dropdownController.SetAvailablePets(this.availablePets);
    }
    public void OnCellClick(string nameString)
    {
        if (nameString == "")
        {
            return;
        }
        sortString = nameString;
        if (BaseUtils.offlineMode)
        {
            SortData();
        }
        else
        {
            StartCoroutine(nearHelper.RequestMarketData(sortString));
        }
    }
    public void OnSpecialClick()
    {
        StartCoroutine(nearHelper.RequestStrongestMarket());
    }
    public void OnReceiveStrongestData(List<MarketPetData> callbackPets)
    {
        baseData.Clear();
        for (int i = 0; i < callbackPets.Count; i++)
        {
            int.TryParse(callbackPets[i].token_id, out int petID);
            long.TryParse(callbackPets[i].price, out long rawPrice);
            //Debug.Log(rawPrice + "," + (rawPrice / 1000000));
            baseData.Add(new MarketData()
            {
                creatureType = callbackPets[i].pet_data.pet_type,
                rarityType = callbackPets[i].pet_data.rarity,
                level = callbackPets[i].pet_data.level,
                trainLevel = callbackPets[i].pet_data.train_level,
                powerLevel = callbackPets[i].pet_data.power_level,
                owner = callbackPets[i].pet_data.owner,
                price = (int)(rawPrice / 1000000),
                creatureID = petID,
            });
        }
        SortData();
        marketTitle.gameObject.SetActive(false);
        marketTitle.SetString($"Searching for: {sortString}");
        OnDockClick();
    }
    public void OnDropClick()
    {
        dropdownWindow.SetActive(true);
        //dockButton.SetActive(true);
        dropButton.SetActive(false);
        sortingButtons.SetActive(false);
        marketRow.SetActive(false);
        dropdownController.Setup();
        OnSortClick((int)sortType);
        OnHighLowClick(lowToHigh);
        marketTitle.gameObject.SetActive(true);
        marketTitle.SetString("Click the creature you want to search for");
        StartCoroutine(nearHelper.RequestGetAvailableMarketPets());
        StartCoroutine(CheckForFailMarketRequest());
    }
    private IEnumerator CheckForFailMarketRequest()
    {
        BaseUtils.searchingForMarket = 1;
        while (BaseUtils.searchingForMarket == 1)
        {
            yield return null;
        }
        if (BaseUtils.searchingForMarket == 2)
        {
            BaseUtils.searchingForMarket = 0;
            mainMenuController.HideLoading();
            mainMenuController.ShowWarning("Error on loading info", "There was an error with your connection", "please relogin or refresh your page.");
        }
    }
    public void OnDockClick()
    {
        dropdownWindow.SetActive(false);
        //dockButton.SetActive(false);
        dropButton.SetActive(true);
        sortingButtons.SetActive(true);
        marketRow.SetActive(true);
    }
    public void OnReceiveMarketData(List<MarketPetData> callbackPets)
    {
        baseData.Clear();
        for (int i = 0; i < callbackPets.Count; i++)
        {
            int.TryParse(callbackPets[i].token_id, out int petID);
            long.TryParse(callbackPets[i].price, out long rawPrice);
            //Debug.Log(rawPrice + "," + (rawPrice / 1000000));
            baseData.Add(new MarketData()
            {
                creatureType = callbackPets[i].pet_data.pet_type,
                rarityType = callbackPets[i].pet_data.rarity,
                level = callbackPets[i].pet_data.level,
                trainLevel = callbackPets[i].pet_data.train_level,
                powerLevel = callbackPets[i].pet_data.power_level,
                owner = callbackPets[i].pet_data.owner,
                price = (int)(rawPrice / 1000000),
                creatureID = petID,
            });
        }
        SortData();
        marketTitle.gameObject.SetActive(false);
        marketTitle.SetString($"Searching for: {sortString}");
        OnDockClick();
    }
    private void SortData()
    {
        List<MarketData> listToSort = new List<MarketData>();
        for (int i = 0; i < baseData.Count; i++)
        {
            //string nameString = baseData[i].creatureType.ToString().ToLower();
            //if (sortString == "" || nameString.Contains(sortString))
            //{
            listToSort.Add(baseData[i]);
            //}
        }
        switch (sortType)
        {
            case SortType.Level:
                if (lowToHigh)
                {
                    listToSort.Sort((x, y) => x.level.CompareTo(y.level));
                }
                else
                {
                    listToSort.Sort((x, y) => y.level.CompareTo(x.level));
                }
                break;
            case SortType.Stats:
                if (lowToHigh)
                {
                    listToSort.Sort((x, y) => x.powerLevel.CompareTo(y.powerLevel));
                }
                else
                {
                    listToSort.Sort((x, y) => y.powerLevel.CompareTo(x.powerLevel));
                }
                break;
            case SortType.Rarity:
                if (lowToHigh)
                {
                    listToSort.Sort((x, y) => (x.Rarity * 10000 + x.Stats).CompareTo(y.Rarity * 10000 + y.Stats));
                }
                else
                {
                    listToSort.Sort((x, y) => (y.Rarity * 10000 + y.Stats).CompareTo(x.Rarity * 10000 + x.Stats));
                }
                break;
            case SortType.Price:
                if (lowToHigh)
                {
                    listToSort.Sort((x, y) => x.price.CompareTo(y.price));
                }
                else
                {
                    listToSort.Sort((x, y) => y.price.CompareTo(x.price));
                }
                break;
            case SortType.TrainLevel:
                if (lowToHigh)
                {
                    listToSort.Sort((x, y) => x.trainLevel.CompareTo(y.trainLevel));
                }
                else
                {
                    listToSort.Sort((x, y) => y.trainLevel.CompareTo(x.trainLevel));
                }
                break;
        }
        elementData.Clear();
        for (int i = 0; i < listToSort.Count; i++)
        {
            elementData.Add(listToSort[i]);
        }
        scroller.ReloadData();
    }
    public void OnSortClick(int sortType)
    {
        this.sortType = (SortType)sortType;
        for (int i = 0; i < sortButtons.Length; i++)
        {
            sortButtons[i].material = BaseUtils.normalUIMat;
        }
        sortButtons[sortType].material = BaseUtils.outlineUIMat;
        SortData();
    }
    public void OnHighLowClick(bool lowToHigh)
    {
        for (int i = 0; i < 2; i++)
        {
            highLowButtons[i].material = BaseUtils.normalUIMat;
        }
        highLowButtons[lowToHigh ? 1 : 0].material = BaseUtils.outlineUIMat;
        this.lowToHigh = lowToHigh;
        SortData();
    }
    public void OnBuyClick(MarketData marketData)
    {
        creatureToBuy = marketData;
        if (Database.databaseStruct.ownedCreatures.Count >= 28)
        {
            mainMenuController.ShowWarning("Pet limit reached!", "You have reached the max amount of pets", "possible to have, which is 28.");
            return;
        }
        if (Database.databaseStruct.pixelTokens <= creatureToBuy.price)
        {
            mainMenuController.ShowWarning("Not enough Tokens!", $"This creature costs {creatureToBuy.price} @", "Would you like to purchase more tokens?", mainMenuController.OnAnswerTokenBuy);
            return;
        }
        mainMenuController.ShowWarning($"{marketData.creatureType}", $"Would you like to buy {marketData.creatureType}", $"for {marketData.price} @?", OnBuyClickResponse);
    }
    public void ShowBuyAuthorize()
    {
        authWindow.SetActive(true);
    }
    public void OnAuthorizedClick()
    {
        authWindow.SetActive(false);
        nearHelper.dataGetState = DataGetState.MarketPurchase;
        StartCoroutine(nearHelper.GetPlayerData());
    }
    public void OnReceiveNewPlayerData()
    {
        mainMenuController.HideLoading();
        int creatureIndex = -1;
        for (int i = 0; i < Database.databaseStruct.ownedCreatures.Count; i++)
        {
            if (Database.databaseStruct.ownedCreatures[i].creatureID == creatureToBuy.creatureID)
            {
                creatureIndex = i;
                break;
            }
        }
        if (creatureIndex != -1)
        {
            //CreatureStruct creatureStruct = new CreatureStruct(creatureToBuy.creatureID, creatureToBuy.experience, creatureToBuy.level, creatureToBuy.trainLevel, creatureToBuy.powerLevel, 0, 0, StateType.InPool, creatureToBuy.rarityType, creatureToBuy.creatureType);
            //Database.AddNewCreature(creatureStruct);
            //Database.databaseStruct.pixelTokens -= creatureToBuy.price;
            mainMenuController.ShowNewCreature(creatureIndex, "you bought a new creature!");
            mainMenuController.UpdatePlayerInfo();
            Setup(false);
        }
        else
        {
            mainMenuController.ShowWarning($"Error on purhcase", $"The purchase for the creature {creatureToBuy.creatureType} failed", $"please try again!");
        }
        Database.SaveDatabase();
    }
    private void OnBuyClickResponse(bool accepted)
    {
        if (accepted)
        {
            StartCoroutine(nearHelper.RequestBuyPet(creatureToBuy.creatureID));
        }
    }
    #region EnhancedScroller Handlers
    public int GetNumberOfCells(EnhancedScroller scroller)
    {
        return Mathf.CeilToInt((float)elementData.Count / (float)numberOfCellsPerRow);
    }
    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
    {
        return 107f;
    }
    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        MarketCellRow cellView = scroller.GetCellView(cellViewPrefab) as MarketCellRow;
        var di = dataIndex * numberOfCellsPerRow;

        cellView.name = "Cell " + (di).ToString() + " to " + ((di) + numberOfCellsPerRow - 1).ToString();

        // pass in a reference to our data set with the offset for this cell
        cellView.SetData(ref elementData, this, di);

        // return the cell to the scroller
        return cellView;
    }

    #endregion
}
public class MarketData
{
    public CreatureType creatureType;
    public RarityType rarityType;
    public string owner;
    public int level;
    public int trainLevel;
    public int powerLevel;
    public int price;
    public int experience;
    public int creatureID;

    public int Rarity
    {
        get
        {
            return (int)rarityType;
        }
    }
    public int Stats
    {
        get
        {
            return level + trainLevel;
        }
    }
}