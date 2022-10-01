using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EggController : MonoBehaviour
{
    public MainMenuController mainMenuController;
    public NearHelper nearHelper;
    public int[] eggPrices;
    public int[] eggTimers;
    public Image[] eggsImages;
    public GameObject[] eggHatchButtons;
    public GameObject[] eggBuyButtons;
    public CustomText[] eggsCount;
    public CustomText[] eggSupplyTexts;
    public Canvas eggCanvas;
    //public CustomText[] eggHatchTimers;
    public CustomText[] descriptionTexts;
    public Color[] backgroundColors;
    public Sprite[] eggSprites;
    public Transform eggPivot;
    public GameObject eggObj;
    public Image eggImage;
    public Image eggBackground;
    public Image eggEdge;
    //public GameObject legendaryButton;
    //public bool legendaryAvailable;
    private int eggIndexToBuy;
    private Coroutine shakeCoroutine;
    private Coroutine bumpCoroutine;

    public GameObject openButtonObj;
    public GameObject descriptionObj;
    //eggscreen
    public GameObject eggScreenObj;
    public RectTransform eggScreenPivot;
    public Image eggScreenImage;

    private Dictionary<RarityType, int> eggPriceDict = new Dictionary<RarityType, int>();
    public void Setup()
    {
        /*for (int i = 0; i < 4; i++)
        {
            eggHatchObjs[i].SetActive(false);
        }*/
        //legendaryButton.SetActive(legendaryAvailable);
        UpdateEggCount();
        UpdateEggSupply();
        UpdateHatchTimers();
    }
    private void UpdateEggSupply()
    {
        for (int i = 0; i < 4; i++)
        {
            eggBuyButtons[i].SetActive(false);
        }
        eggSupplyTexts[0].SetString("?");
        eggSupplyTexts[1].SetString("?");
        eggSupplyTexts[2].SetString("?");
        eggSupplyTexts[3].SetString("?");
        StartCoroutine(nearHelper.RequestEggSupply());
    }
    public void OnReceiveEggSupply(EggSupplyCallback eggSupplyCallback)
    {
        eggPriceDict.Clear();
        eggPriceDict.Add(RarityType.Common, (int)(long.Parse(eggSupplyCallback.common_price) / 1000000));
        eggPriceDict.Add(RarityType.Rare, (int)(long.Parse(eggSupplyCallback.rare_price) / 1000000));
        eggPriceDict.Add(RarityType.Epic, (int)(long.Parse(eggSupplyCallback.epic_price) / 1000000));
        eggPriceDict.Add(RarityType.Legendary, (int)(long.Parse(eggSupplyCallback.legendary_price) / 1000000));
        eggSupplyTexts[0].SetString(eggSupplyCallback.common.ToString());
        eggSupplyTexts[1].SetString(eggSupplyCallback.rare.ToString());
        eggSupplyTexts[2].SetString(eggSupplyCallback.epic.ToString());
        eggSupplyTexts[3].SetString(eggSupplyCallback.legendary.ToString());
        eggBuyButtons[0].SetActive(eggSupplyCallback.common > 0);
        eggBuyButtons[1].SetActive(eggSupplyCallback.rare > 0);
        eggBuyButtons[2].SetActive(eggSupplyCallback.epic > 0);
        eggBuyButtons[3].SetActive(eggSupplyCallback.legendary > 0);
    }
    public void UpdateEggCount()
    {
        for (int i = 0; i < 4; i++)
        {
            eggsCount[i].SetString(Database.databaseStruct.eggAmount[i].ToString());
        }
    }
    public void UpdateHatchTimers()
    {
        if (Database.databaseStruct.hatchingEgg != RarityType.None)
        {
            float hatchTimer = Database.databaseStruct.hatchingTimer.ToFlatTime();
            descriptionTexts[0].SetString($"Hatching egg of type {Database.databaseStruct.hatchingEgg}");
            descriptionTexts[1].SetString($"Time Left: {hatchTimer.GetNiceTime(false)}");
            eggObj.SetActive(true);
            eggImage.sprite = eggSprites[(int)Database.databaseStruct.hatchingEgg];
            eggImage.SetNativeSize();
            eggEdge.sprite = BaseUtils.borderSprites[(int)Database.databaseStruct.hatchingEgg];
            eggBackground.color = backgroundColors[(int)Database.databaseStruct.hatchingEgg];
        }
        else
        {
            descriptionTexts[0].SetString("Currently not hatching anything.");
            descriptionTexts[1].SetString("Buy an egg in order to start hatching it.");
            eggEdge.sprite = BaseUtils.borderSprites[4];
            eggBackground.color = backgroundColors[4];
            eggObj.SetActive(false);
        }
    }
    public void OnBuyClick(int eggIndex)
    {
        /*if (eggIndex == 3)
        {
            mainMenuController.ShowWarning("Not possible!", "Sorry, legendary eggs are only available", "During events or promotions.");
            return;
        }*/
        eggIndexToBuy = eggIndex;
        mainMenuController.ShowWarning($"{(RarityType)eggIndex} Egg", $"Would you like to buy a {(RarityType)eggIndex} egg", $"for {eggPriceDict[(RarityType)eggIndex]} @?", EggWindowResponse);
    }
    private void EggWindowResponse(bool accepted)
    {
        if (accepted)
        {
            if (Database.databaseStruct.pixelTokens >= eggPriceDict[(RarityType)eggIndexToBuy])
            {
                StartCoroutine(nearHelper.RequestBuyEgg((RarityType)eggIndexToBuy));
            }
            else
            {
                mainMenuController.ShowWarning("Not enough Tokens!", $"This egg costs {eggPriceDict[(RarityType)eggIndexToBuy]} @", "Would you like to purchase more tokens?", mainMenuController.OnAnswerTokenBuy);
            }
        }
    }
    public void OnEggBuyCallback(bool success)
    {
        if (success)
        {
            Database.databaseStruct.eggAmount[eggIndexToBuy]++;
            Database.databaseStruct.pixelTokens -= eggPriceDict[(RarityType)eggIndexToBuy];
            mainMenuController.BumpObj(eggsImages[eggIndexToBuy].transform);
            mainMenuController.ShowEffect((EffectType)(20 + eggIndexToBuy), eggsImages[eggIndexToBuy].transform);
            mainMenuController.ShowWarning("Sucess!", "You purchased an egg of type " + (RarityType)eggIndexToBuy, "hatch it when you can.");
            mainMenuController.pixelTokenText.SetString(Database.databaseStruct.pixelTokens.ToString());
            UpdateEggCount();
            UpdateEggSupply();
        }
        else
        {
            mainMenuController.ShowWarning($"Error on buying egg", $"The request for buying {(RarityType)eggIndexToBuy} egg failed", $"please try again!");
        }
        Database.SaveDatabase();
    }
    private void BumpEgg(int rarity)
    {
        if (bumpCoroutine != null)
        {
            StopCoroutine(bumpCoroutine);
        }
        mainMenuController.ShowEffect((EffectType)(15 + rarity), eggImage.transform);
        bumpCoroutine = StartCoroutine(BumpEggCoroutine());
    }
    private IEnumerator BumpEggCoroutine()
    {
        float timer = 0;
        while (timer <= 1)
        {
            eggPivot.transform.localScale = Vector3.Lerp(Vector3.one * 1.5f, Vector3.one * 2, timer.Evaluate(CurveType.PeakCurve));
            timer += Time.deltaTime * 5;
            yield return null;
        }
        eggPivot.transform.localScale = Vector3.one * 1.5f;
    }
    private void ShakeEgg(int eggHatchIndex)
    {
        if (!eggCanvas.enabled)
        {
            return;
        }
        if (shakeCoroutine != null)
        {
            return;
        }
        shakeCoroutine = StartCoroutine(ShakeEggCoroutine(eggHatchIndex));
    }
    private IEnumerator ShakeEggCoroutine(int eggHatchIndex)
    {
        mainMenuController.ShowEffect((EffectType)(15 + eggHatchIndex), eggImage.transform);
        float timer = 0;
        float goRotation = 10 * BaseUtils.RandomSign();
        while (timer <= 1)
        {
            eggPivot.transform.localEulerAngles = Vector3.Lerp(Vector3.zero, Vector3.forward * goRotation, timer.Evaluate(CurveType.ShakeCurve));
            timer += Time.deltaTime * 2;
            yield return null;
        }
        eggPivot.transform.localEulerAngles = Vector3.zero;
        yield return new WaitForSeconds(2);
        shakeCoroutine = null;
    }
    public void OnOpenClick()
    {
        if (!BaseUtils.offlineMode)
        {
            StartCoroutine(nearHelper.RequestOpenEgg());
        }
    }
    //#protocol
    public void OnHatchClick(int eggIndex)
    {
        if (Database.databaseStruct.ownedCreatures.Count >= 28)
        {
            mainMenuController.ShowWarning("Too many pets!", "You have reached the maximum amount", "of creatures which is 28.");
            return;
        }
        if (Database.databaseStruct.eggAmount[eggIndex] <= 0)
        {
            if ((RarityType)eggIndex == RarityType.Legendary/* && !legendaryAvailable*/)
            {
                mainMenuController.ShowWarning("No eggs!", "You have no Eggs of type " + (RarityType)eggIndex, "you can buy some in special events only.");
            }
            else
            {
                mainMenuController.ShowWarning("No eggs!", "You have no Eggs of type " + (RarityType)eggIndex, "purchase some before hatching them.");
            }
            return;
        }
        if (Database.databaseStruct.hatchingEgg != RarityType.None)
        {
            mainMenuController.ShowWarning("Already hatching!", $"You are already hatching egg of type {Database.databaseStruct.hatchingEgg}", "wait until its done to hatch another.");
            return;
        }
        /*if (Database.databaseStruct.hatchingEggs[eggIndex])
        {
            ShowWarning("Already hatching!", "You are already hatching egg of type " + (RarityType)eggIndex, "wait until its done to hatch another.");
            return;
        }*/
        //Debug.Log("Hatching egg" + (RarityType)eggIndex);
        /*if (BaseUtils.offlineMode)
        {
            Database.databaseStruct.hatchingTimer = System.DateTime.Now.add 60 * 60 * eggTimers[eggIndex];
        }*/
        Database.databaseStruct.hatchingEgg = (RarityType)eggIndex;
        //eggHatchObjs[eggIndex].SetActive(true);
        //mainMenuController.BumpObj(eggHatchObjs[eggIndex].transform);
        mainMenuController.BumpObj(eggsImages[eggIndex].transform);
        BumpEgg(eggIndex);
        UpdateHatchTimers();
        if (!BaseUtils.offlineMode)
        {
            descriptionTexts[0].SetString("Requesting Hatch Info");
            descriptionTexts[1].SetString("");
            StartCoroutine(nearHelper.RequestHatchEgg((RarityType)eggIndex));
        }
    }
    //#Dan -> Call this function from the server to sync the egg timers as exampled above
    public void SyncEggTimers(int eggIndex, float eggTimer)
    {
        if (mainMenuController.loadingScreen.activeSelf)
        {
            return;
        }
        //Database.databaseStruct.hatchingTimer = eggTimer;
        UpdateHatchTimers();
        ShakeEgg(eggIndex);
        if (eggTimer <= 0)
        {
            descriptionObj.SetActive(false);
            openButtonObj.SetActive(true);
        }
        else
        {
            descriptionObj.SetActive(true);
            openButtonObj.SetActive(false);
        }
    }
    //#Dan -> Call this function from the server to hatch an egg
    public void SyncEggHatch(int eggIndex/*, CreatureStruct creatureStruct*/)
    {
        //Database.databaseStruct.eggAmount[eggIndex]--;
        //Database.databaseStruct.hatchingTimer = 0;
        Database.databaseStruct.hatchingEgg = RarityType.None;
        descriptionObj.SetActive(true);
        openButtonObj.SetActive(false);
        //Database.AddNewCreature(creatureStruct);
        mainMenuController.BumpObj(eggsImages[eggIndex].transform);
        BumpEgg(eggIndex);
        UpdateEggCount();
        UpdateHatchTimers();
        Debug.Log("Hatched Egg " + (RarityType)eggIndex);
        StartCoroutine(AnimateEggHatch(eggIndex));
        Database.SaveDatabase();
    }
    private IEnumerator AnimateEggHatch(int eggIndex)
    {
        eggScreenObj.SetActive(true);
        eggScreenImage.sprite = eggSprites[eggIndex];
        for (int i = 0; i < 5; i++)
        {
            float intensity = 1 + (i + 1) * .3f;
            float timer = 0;
            float goRotation = 10 * BaseUtils.RandomSign() * intensity;
            while (timer <= 1)
            {
                eggScreenPivot.localEulerAngles = Vector3.Lerp(Vector3.zero, Vector3.forward * goRotation, timer.Evaluate(CurveType.ShakeCurve));
                timer += Time.deltaTime * intensity;
                yield return null;
            }
            mainMenuController.ShowEffect(i == 5 ? EffectType.Hatch : EffectType.Hit, eggScreenImage.rectTransform);
            mainMenuController.ShowEffect((EffectType)(15 + eggIndex), eggScreenImage.rectTransform);
            mainMenuController.BumpObj(eggScreenImage.rectTransform);
            yield return new WaitForSeconds(1 / intensity);
        }
        yield return new WaitForSeconds(.35f);
        eggScreenObj.SetActive(false);
        /*if (BaseUtils.offlineMode)
        {
            Database.databaseStruct.ownedCreatures.Add(BaseUtils.GenerateCreatureStruct(1));
        }*/
        mainMenuController.ShowNewCreature(Database.databaseStruct.ownedCreatures.Count - 1, "your egg hatched!");
    }
}
