using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PostScreenController : MonoBehaviour
{
    public MainMenuController mainMenuController;
    //public GraphicRaycaster[] graphicRaycasters;
    public NearHelper nearHelper;
    public Canvas postCanvas;
    public GameObject victoryObj;
    public GameObject defeatObj;
    public GameObject skipObj;
    public Image backgroundImage;
    public Image rankUpImage;
    public Image rankUpBorder;
    public CustomText rankUpText;
    public CustomText rankFullText;
    public RectTransform rankBar;
    public Material winMaterial;
    public Material defeatMaterial;

    private bool exiting;
    private float clickTimer;
    private readonly List<CardController> yourCreatures = new List<CardController>();
    public void Setup(BattleInfo battleInfo)
    {
        StartCoroutine(WaitAndSetEndMatchInfo(battleInfo));
    }
    private void Update()
    {
        clickTimer -= Time.deltaTime;
        if (Input.GetMouseButtonUp(0))
        {
            clickTimer = 0;
        }
    }
    public void OnSkipClick()
    {
        if (exiting)
        {
            return;
        }
        exiting = true;
        StopAllCoroutines();
        mainMenuController.StopAllCoroutines();
        if (yourCreatures.Count > 0)
        {
            for (int i = 0; i < 3; i++)
            {
                if (yourCreatures[i] != null)
                {
                    Destroy(yourCreatures[i].gameObject);
                }
            }
        }
        StartCoroutine(WaitAndExit());
    }
    private IEnumerator WaitAndExit()
    {
        yield return new WaitForSeconds(.1f);
        exiting = false;
        ExitPostScreen();
    }
    private IEnumerator WaitAndSetEndMatchInfo(BattleInfo battleInfo)
    {
        while (BaseUtils.onFight)
        {
            yield return null;
        }
        /*for (int i = 0; i < graphicRaycasters.Length; i++)
        {
            graphicRaycasters[i].enabled = false;
        }*/
        Time.timeScale = 1;
        skipObj.SetActive(true);
        mainMenuController.CloseCardWindow();
        mainMenuController.fightCanvas.gameObject.SetActive(false);
        mainMenuController.fightCanvas.enabled = false;
        //mainMenuController.matchSearchWindow.SetActive(false);
        //mainMenuController.matchParentButton.SetActive(true);
        postCanvas.gameObject.SetActive(true);
        postCanvas.enabled = true;
        if (battleInfo.winnerAccountName == Database.databaseStruct.playerAccount)
        {
            backgroundImage.material = winMaterial;
            victoryObj.SetActive(true);
            mainMenuController.BumpObj(victoryObj.transform, 2);
            mainMenuController.ShowEffect(EffectType.Victory, victoryObj.transform);
        }
        else
        {
            backgroundImage.material = defeatMaterial;
            defeatObj.SetActive(true);
            mainMenuController.BumpObj(defeatObj.transform, 2);
            mainMenuController.ShowEffect(EffectType.Defeat, defeatObj.transform);
        }
        clickTimer = 3;
        while (clickTimer > 0)
        {
            yield return null;
        }
        victoryObj.SetActive(false);
        defeatObj.SetActive(false);
        yourCreatures.Clear();
        for (int i = 0; i < 3; i++)
        {
            int databaseIndex = mainMenuController.selectedCreatures[i];
            CardController cardController = mainMenuController.InstantiateCard(postCanvas.transform, true);
            cardController.Setup(mainMenuController, databaseIndex, true);
            yourCreatures.Add(cardController);
            cardController.gameObject.SetActive(false);
        }
        yield return new WaitForSeconds(.1f);
        for (int i = 0; i < 3; i++)
        {
            int databaseIndex = mainMenuController.selectedCreatures[i];
            yourCreatures[i].gameObject.SetActive(true);
            yourCreatures[i].transform.localPosition = Vector3.right * (i - 1) * 200;
            yourCreatures[i].UpdateExpBar(Database.databaseStruct.ownedCreatures[databaseIndex].experience, -1, Database.databaseStruct.ownedCreatures[databaseIndex].level);
            mainMenuController.BumpObj(yourCreatures[i].transform);
            yield return new WaitForSeconds(.1f);
        }
        clickTimer = .5f;
        while (clickTimer > 0)
        {
            yield return null;
        }
        for (int i = 0; i < 3; i++)
        {
            int creatureIndex = mainMenuController.selectedCreatures[i];
            if (Database.databaseStruct.ownedCreatures[creatureIndex].level < 100)
            {
                yourCreatures[i].UpdateExpBar(Database.databaseStruct.ownedCreatures[creatureIndex].experience, battleInfo.expGain);
                Database.SetCreatureExperience(creatureIndex, Database.databaseStruct.ownedCreatures[creatureIndex].experience + battleInfo.expGain);
                yield return new WaitForSeconds(.21f);
                bool willDelay = false;
                if (Database.databaseStruct.ownedCreatures[creatureIndex].experience >= BaseUtils.GetExpForNextLevel(Database.databaseStruct.ownedCreatures[creatureIndex].level))
                {
                    Database.SetCreatureLevel(creatureIndex, Database.databaseStruct.ownedCreatures[creatureIndex].level + 1);
                    yourCreatures[i].levelUpText.SetActive(true);
                    mainMenuController.BumpObj(yourCreatures[i].levelUpText.transform, 1.5f);
                    mainMenuController.ShowEffect(EffectType.LevelUp, yourCreatures[i].cardRect);
                    yourCreatures[i].Setup(mainMenuController, creatureIndex, true);
                    yourCreatures[i].UpdateExpBar(Database.databaseStruct.ownedCreatures[creatureIndex].experience, -1, Database.databaseStruct.ownedCreatures[creatureIndex].level);
                    willDelay = true;
                }
                int levelToEvolve = BaseUtils.creatureDict[Database.databaseStruct.ownedCreatures[creatureIndex].creatureType].levelToEvolve;
                if (levelToEvolve != -1 && Database.databaseStruct.ownedCreatures[creatureIndex].level >= levelToEvolve)
                {
                    yourCreatures[i].evolveUpText.SetActive(true);
                    mainMenuController.BumpObj(yourCreatures[i].evolveUpText.transform, 1.5f);
                    willDelay = true;
                }
                yield return new WaitForSeconds(willDelay ? .5f : .25f);
            }
        }
        clickTimer = 1;
        while (clickTimer > 0)
        {
            yield return null;
        }
        for (int i = 0; i < 3; i++)
        {
            Coroutine bumpCoroutine = mainMenuController.StartCoroutine(mainMenuController.BumpObjCoroutine(yourCreatures[i].transform, 1));
            yield return new WaitForSeconds(.21f);
            mainMenuController.StopCoroutine(bumpCoroutine);
            Destroy(yourCreatures[i].gameObject);
            yourCreatures[i] = null;
        }
        Database.databaseStruct.fightBalance--;
        Database.databaseStruct.currentRank += battleInfo.rankChange;
        if (Database.databaseStruct.currentRank < 0)
        {
            Database.databaseStruct.currentRank = 0;
        }
        float timer = 0;
        int currentElo = Database.databaseStruct.currentRank - battleInfo.rankChange;
        int previousRank = currentElo.ToRankIndex();
        int gotoElo = previousRank.ToNextElo();
        rankUpBorder.gameObject.SetActive(true);
        mainMenuController.BumpObj(rankUpBorder.transform, 1.5f);
        mainMenuController.BumpObj(rankUpImage.transform, 1);
        rankUpText.SetString($"Rank: {currentElo}");
        rankUpBorder.sprite = BaseUtils.backSprites[previousRank];
        rankUpBorder.material = previousRank >= 5 ? BaseUtils.glowUIMat : BaseUtils.normalUIMat;
        rankUpImage.sprite = BaseUtils.trophySprites[previousRank];
        rankUpImage.SetNativeSize();
        rankUpText.SetString($"Rank: {currentElo}");
        rankFullText.SetString($"{currentElo}/{gotoElo}");
        rankBar.localScale = new Vector3(Mathf.Clamp01((float)currentElo / gotoElo), 1, 1);
        clickTimer = .5f;
        while (clickTimer > 0)
        {
            yield return null;
        }
        while (timer <= 1)
        {
            currentElo = Mathf.RoundToInt(Mathf.Lerp(currentElo, Database.databaseStruct.currentRank, timer.Evaluate(CurveType.EaseIn)));
            float goScale = Mathf.Clamp01((float)currentElo / gotoElo);
            rankUpText.SetString($"Rank: {currentElo}");
            rankFullText.SetString($"{currentElo}/{gotoElo}");
            rankBar.localScale = new Vector3(goScale, 1, 1);
            mainMenuController.ShowEffect(EffectType.RankGain, rankBar.parent, Vector3.right * (rankBar.sizeDelta.x * goScale + 3));
            timer += Time.deltaTime * 2;
            yield return null;
        }
        rankUpText.SetString($"Rank: {Database.databaseStruct.currentRank}");
        rankFullText.SetString($"{Database.databaseStruct.currentRank}/{gotoElo}");
        rankBar.localScale = new Vector3(Mathf.Clamp01((float)Database.databaseStruct.currentRank / gotoElo), 1, 1);
        int currentRank = Database.databaseStruct.currentRank.ToRankIndex();
        if (currentRank != previousRank)
        {
            rankUpBorder.sprite = BaseUtils.backSprites[currentRank];
            rankUpBorder.material = currentRank >= 5 ? BaseUtils.glowUIMat : BaseUtils.normalUIMat;
            rankUpImage.sprite = BaseUtils.trophySprites[currentRank];
            rankUpImage.SetNativeSize();
            mainMenuController.BumpObj(rankUpBorder.transform, 1.5f);
            mainMenuController.BumpObj(rankUpImage.transform, 1);
            mainMenuController.ShowEffect(EffectType.RankUp, rankUpBorder.transform);
            yield return new WaitForSeconds(.5f);
        }
        clickTimer = 1.5f;
        while (clickTimer > 0)
        {
            yield return null;
        }
        ExitPostScreen();
    }

    private void ExitPostScreen()
    {
        rankUpBorder.gameObject.SetActive(false);
        victoryObj.SetActive(false);
        defeatObj.SetActive(false);
        skipObj.SetActive(false);
        postCanvas.gameObject.SetActive(false);
        postCanvas.enabled = false;
        /*for (int i = 0; i < graphicRaycasters.Length; i++)
        {
            graphicRaycasters[i].enabled = true;
        }*/
        mainMenuController.mainCanvas.gameObject.SetActive(true);
        mainMenuController.mainCanvas.enabled = true;
        mainMenuController.matchSearchWindow.SetActive(false);
        mainMenuController.matchParentButton.SetActive(true);
        BaseUtils.ClearEffects();
        mainMenuController.UpdateAllTexts();
        nearHelper.dataGetState = DataGetState.AfterFight;
        StartCoroutine(nearHelper.GetPlayerData());
    }
}
