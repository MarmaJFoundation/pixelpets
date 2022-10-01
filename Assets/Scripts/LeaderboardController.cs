using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EnhancedUI;
using EnhancedUI.EnhancedScroller;

public class LeaderboardController : MonoBehaviour, IEnhancedScrollerDelegate
{
    private SmallList<LeaderboardData> elementData = new SmallList<LeaderboardData>();
    private List<LeaderboardData> leaderboardData = new List<LeaderboardData>();
    private List<LeaderboardData> tournamentData = new List<LeaderboardData>();
    public NearHelper nearHelper;
    public EnhancedScroller scroller;
    public EnhancedScrollerCellView cellViewPrefab;
    public GameObject countdownObj;
    public CustomText countdownText;
    public Image globalButtonImage;
    public Image clashButtonImage;
    private DateTime tournamentEndTime;
    private string tournamentName;
    public void Setup()
    {
        scroller.Delegate = this;
        if (BaseUtils.offlineMode)
        {
            OnGlobalClick();
        }
        else
        {
            StartCoroutine(nearHelper.RequestLeaderboardData());
        }
    }
    public void OnGlobalClick()
    {
        globalButtonImage.material = BaseUtils.outlineUIMat;
        clashButtonImage.material = BaseUtils.normalUIMat;
        countdownObj.SetActive(false);
        elementData.Clear();
        if (BaseUtils.offlineMode)
        {
            for (int i = 0; i < 100; i++)
            {
                elementData.Add(new LeaderboardData()
                {
                    playerRank = BaseUtils.RandomInt(800, 1800),
                    playerName = "offline player " + i,
                    isClash = false,
                    rosterCreatures = new CreatureType[] {
                        BaseUtils.GetRandomCreature(),
                        BaseUtils.GetRandomCreature(),
                        BaseUtils.GetRandomCreature()},
                    rosterRarities = new RarityType[] {
                        (RarityType)(BaseUtils.RandomInt(0, 3)),
                        (RarityType)(BaseUtils.RandomInt(0, 3)),
                        (RarityType)(BaseUtils.RandomInt(0, 3))}
                });
            }
            SortData(false);
        }
        else
        {
            for (int i = 0; i < leaderboardData.Count; i++)
            {
                elementData.Add(leaderboardData[i]);
            }
            SortData(false);
        }
    }
    public void OnClashClick()
    {
        globalButtonImage.material = BaseUtils.normalUIMat;
        clashButtonImage.material = BaseUtils.outlineUIMat;
        countdownObj.SetActive(true);
        countdownText.SetString("Calculating clash tournament time...");
        elementData.Clear();
        if (BaseUtils.offlineMode)
        {
            for (int i = 0; i < 100; i++)
            {
                elementData.Add(new LeaderboardData()
                {
                    wins = BaseUtils.RandomInt(10, 100),
                    losses = BaseUtils.RandomInt(10, 100),
                    playerName = "offline player " + i,
                    isClash = true,
                    rosterCreatures = new CreatureType[] {
                        BaseUtils.GetRandomCreature(),
                        BaseUtils.GetRandomCreature(),
                        BaseUtils.GetRandomCreature()},
                    rosterRarities = new RarityType[] {
                        (RarityType)(BaseUtils.RandomInt(0, 3)),
                        (RarityType)(BaseUtils.RandomInt(0, 3)),
                        (RarityType)(BaseUtils.RandomInt(0, 3))}
                });
            }
            SortData(true);
        }
        else
        {
            for (int i = 0; i < tournamentData.Count; i++)
            {
                elementData.Add(tournamentData[i]);
            }
            SortData(true);
        }
    }
    public void OnReceiveRankData(LeaderboardWrapper wrapperData)
    {
        leaderboardData.Clear();
        bool hasSelfRank = wrapperData.leaderboard.Count > 0 && wrapperData.leaderboard[0].account_id == Database.databaseStruct.playerAccount;
        for (int i = 0; i < wrapperData.leaderboard.Count; i++)
        {
            int listIndex;
            if (!hasSelfRank)
            {
                listIndex = i + 1;
            }
            else
            {
                listIndex = i == 0 ? wrapperData.leaderboard[i].position : i;
            }
            leaderboardData.Add(new LeaderboardData()
            {
                listIndex = listIndex,
                selfRank = i == 0 && hasSelfRank,
                playerRank = wrapperData.leaderboard[i].player_rating,
                playerName = wrapperData.leaderboard[i].account_id,
                isClash = false,
                rosterCreatures = new CreatureType[] {
                        (CreatureType)wrapperData.leaderboard[i].player_loadout[0].pet_type,
                        (CreatureType)wrapperData.leaderboard[i].player_loadout[1].pet_type,
                        (CreatureType)wrapperData.leaderboard[i].player_loadout[2].pet_type},
                rosterRarities = new RarityType[] {
                        (RarityType)wrapperData.leaderboard[i].player_loadout[0].pet_rarity,
                        (RarityType)wrapperData.leaderboard[i].player_loadout[1].pet_rarity,
                        (RarityType)wrapperData.leaderboard[i].player_loadout[2].pet_rarity}
            });
        }
        tournamentData.Clear();
        hasSelfRank = wrapperData.tournament.Count > 0 && wrapperData.tournament[0].account_id == Database.databaseStruct.playerAccount;
        for (int i = 0; i < wrapperData.tournament.Count; i++)
        {
            int listIndex;
            if (!hasSelfRank)
            {
                listIndex = i + 1;
            }
            else
            {
                listIndex = i == 0 ? wrapperData.tournament[i].position : i;
            }
            tournamentData.Add(new LeaderboardData()
            {
                listIndex = listIndex,
                selfRank = i == 0 && hasSelfRank,
                wins = wrapperData.tournament[i].matches_won,
                losses = wrapperData.tournament[i].matches_lost,
                playerName = wrapperData.tournament[i].account_id,
                isClash = true,
                rosterCreatures = new CreatureType[] {
                        (CreatureType)wrapperData.tournament[i].player_loadout[0].pet_type,
                        (CreatureType)wrapperData.tournament[i].player_loadout[1].pet_type,
                        (CreatureType)wrapperData.tournament[i].player_loadout[2].pet_type},
                rosterRarities = new RarityType[] {
                        (RarityType)wrapperData.tournament[i].player_loadout[0].pet_rarity,
                        (RarityType)wrapperData.tournament[i].player_loadout[1].pet_rarity,
                        (RarityType)wrapperData.tournament[i].player_loadout[2].pet_rarity}
            });
        }
        string tName = wrapperData.active_tournament.ToLower();
        string weekNumber = tName.Substring(tName.IndexOf('w'), tName.IndexOf('y'));
        string yearNumber = tName.Substring(tName.IndexOf('y'));
        tournamentName = $"Weekly Clash {weekNumber} {yearNumber}";
        tournamentEndTime = TimestampHelper.GetDateTimeFromTimestamp(wrapperData.tournament_ends);
        OnGlobalClick();
    }
    public void Update()
    {
        countdownText.SetString($"Time left for {tournamentName}: {tournamentEndTime.ToFlatTime().GetNiceTime()}");
    }
    private void SortData(bool isClash)
    {
        /*List<LeaderboardData> listToSort = new List<LeaderboardData>();
        for (int i = 0; i < elementData.Count; i++)
        {
            listToSort.Add(elementData[i]);
        }
        elementData.Clear();
        if (isClash)
        {
            listToSort.Sort((x, y) => y.ClashRank.CompareTo(x.ClashRank));
        }
        else
        {
            listToSort.Sort((x, y) => y.playerRank.CompareTo(x.playerRank));
        }
        for (int i = 0; i < listToSort.Count; i++)
        {
            elementData.Add(listToSort[i]);
        }*/
        scroller.ReloadData();
    }
    #region EnhancedScroller Callbacks
    public int GetNumberOfCells(EnhancedScroller scroller)
    {
        return elementData.Count;
    }
    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
    {
        return 40f;
    }
    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        LeaderboardCell cellView = scroller.GetCellView(cellViewPrefab) as LeaderboardCell;
        cellView.name = "rankCell";
        cellView.SetData(dataIndex, elementData[dataIndex]);
        return cellView;
    }
    #endregion
}
public class LeaderboardData
{
    public CreatureType[] rosterCreatures;
    public RarityType[] rosterRarities;
    public string playerName;
    public int listIndex;
    public int playerRank;
    public int wins;
    public int losses;
    public bool isClash;
    public bool selfRank;

    public int ClashRank
    {
        get
        {
            return wins - losses;
        }
    }
}