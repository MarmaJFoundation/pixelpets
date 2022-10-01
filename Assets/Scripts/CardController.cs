using System.Collections;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class CardController : MonoBehaviour
{
    public RectTransform cardRect;
    public Image cardImage;
    public Image creatureImage;
    public Image bodyTypeImage;
    public Image damageTypeImage;
    public Image hpBarImage;
    public RectTransform levelBar;
    public CustomText creatureName;
    public CustomText levelText;
    public CustomText damageNumber;
    public CustomText speedNumber;
    public CustomText magicNumber;
    public CustomText defenseNumber;
    public CustomText pLvlText;
    public CustomText tLvlText;

    public CustomTooltip bodyTooltip;
    public CustomTooltip attackTooltip;
    public CustomTooltip expTooltip;

    public RectTransform hpBar;
    public GameObject visualHpBar;
    public CustomText hpText;

    public GameObject levelUpText;
    public GameObject evolveUpText;

    public GameObject expBigObj;
    public CustomText expBigText;
    public RectTransform expBigBar;

    [HideInInspector]
    public int databaseIndex;
    [HideInInspector]
    public int level;
    [HideInInspector]
    public int experience;
    [HideInInspector]
    public int trainLevel;
    [HideInInspector]
    public int powerLevel;
    [HideInInspector]
    public int rarity;
    [HideInInspector]
    public int evolution;

    private MainMenuController mainMenuController;
    private FightController fightController;

    private bool fightingCard;
    public void Setup(FightController fightController, CreatureType creatureType, RarityType rarityType, int level, int trainLevel, int powerLevel, int experience)
    {
        fightingCard = true;
        gameObject.SetActive(true);
        //visualHpBar.SetActive(false);
        this.fightController = fightController;
        this.level = level;
        this.trainLevel = trainLevel;
        this.powerLevel = powerLevel;
        this.experience = experience;
        rarity = (int)rarityType + 1;
        evolution = BaseUtils.creatureDict[creatureType].evolution;
        SetCreatureInfo(creatureType, rarityType, true);
    }
    public void Setup(MainMenuController mainMenuController, int databaseIndex, bool fromPostScreen)
    {
        fightingCard = fromPostScreen;
        this.mainMenuController = mainMenuController;
        this.databaseIndex = databaseIndex;
        gameObject.SetActive(true);
        visualHpBar.SetActive(!fromPostScreen);
        expBigObj.SetActive(false);
        CreatureType creatureType = Database.databaseStruct.ownedCreatures[databaseIndex].creatureType;
        RarityType rarityType = Database.databaseStruct.ownedCreatures[databaseIndex].rarity;
        level = Database.databaseStruct.ownedCreatures[databaseIndex].level;
        experience = Database.databaseStruct.ownedCreatures[databaseIndex].experience;
        trainLevel = Database.databaseStruct.ownedCreatures[databaseIndex].trainLevel;
        powerLevel = Database.databaseStruct.ownedCreatures[databaseIndex].powerLevel;
        rarity = (int)Database.databaseStruct.ownedCreatures[databaseIndex].rarity + 1;
        evolution = BaseUtils.creatureDict[creatureType].evolution;
        SetCreatureInfo(creatureType, rarityType, false);
    }
    private void SetCreatureInfo(CreatureType creatureType, RarityType rarityType, bool fightCard)
    {
        ScriptableCreature scriptableCreature = BaseUtils.creatureDict[creatureType];
        cardImage.sprite = fightCard ? BaseUtils.battleCardSprites[(int)rarityType] : BaseUtils.cardSprites[(int)rarityType];
        creatureImage.sprite = scriptableCreature.creatureSprite;
        bodyTooltip.elementType = scriptableCreature.bodyType;
        bodyTooltip.elementIndex = 1;
        attackTooltip.elementType = scriptableCreature.damageType;
        attackTooltip.elementIndex = 2;
        bodyTypeImage.sprite = BaseUtils.typeSpriteDict[scriptableCreature.bodyType];
        bodyTypeImage.SetNativeSize();
        damageTypeImage.sprite = BaseUtils.typeSpriteDict[scriptableCreature.damageType];
        damageTypeImage.SetNativeSize();
        creatureName.SetString(scriptableCreature.creatureType.ToString());
        int trueTrainLevel = Database.databaseStruct.ownedCreatures[databaseIndex].currentState == StateType.Training ? trainLevel - 1 : trainLevel;
        if (!fightCard)
        {
            int baseExp = BaseUtils.GetExpForNextLevel(level - 1);
            levelBar.localScale = new Vector3(Mathf.Clamp((float)(experience - baseExp) / (BaseUtils.GetExpForNextLevel(level) - baseExp), .05f, 1), 1, 1);
            levelText.SetString($"lvl:{level}");
            pLvlText.SetString($"p.lvl:{powerLevel}");
            tLvlText.SetString($"t.lvl:{trueTrainLevel}");
        }
        int damage = BaseUtils.GetModifiedStat(scriptableCreature.damage, rarity, trueTrainLevel, level, evolution, powerLevel);
        int speed = BaseUtils.GetModifiedStat(scriptableCreature.speed, rarity, trueTrainLevel, level, evolution, powerLevel);
        int magic = BaseUtils.GetModifiedStat(scriptableCreature.magic, rarity, trueTrainLevel, level, evolution, powerLevel);
        int defense = BaseUtils.GetModifiedStat(scriptableCreature.defense, rarity, trueTrainLevel, level, evolution, powerLevel);
        damageNumber.SetString(damage.ToString());
        speedNumber.SetString(speed.ToString());
        magicNumber.SetString(magic.ToString());
        defenseNumber.SetString(defense.ToString());
        if (!fightCard)
        {
            hpBarImage.sprite = BaseUtils.borderSprites[(int)rarityType];
            int baseExp = BaseUtils.GetExpForNextLevel(level - 1);
            expTooltip.tooltipText[0] = $"exp:{Mathf.RoundToInt(experience - baseExp)} / {BaseUtils.GetExpForNextLevel(level) - baseExp}";
            hpText.SetString($"Max Health: {BaseUtils.GetModifiedStat(300, rarity, trainLevel, level, evolution, powerLevel)}");
        }
    }
    public void UpdateExpBar(int fromExp, int addExp, int level = -1)
    {
        expBigObj.SetActive(true);
        if (addExp == -1)
        {
            this.level = level;
            int baseExp = BaseUtils.GetExpForNextLevel(level - 1);
            float goScale = Mathf.Clamp01((float)(fromExp - baseExp) / (BaseUtils.GetExpForNextLevel(level) - baseExp));
            levelText.SetString("lvl:" + level);
            levelBar.localScale = new Vector3(goScale, 1, 1);
            expBigBar.localScale = new Vector3(goScale, 1, 1);
            expBigText.SetString($"exp:{Mathf.RoundToInt(fromExp - baseExp)} / {BaseUtils.GetExpForNextLevel(level) - baseExp}");
            mainMenuController.ShowEffect(EffectType.XpGain, expBigBar.parent, Vector3.right * (expBigBar.sizeDelta.x * goScale + 5));
            return;
        }
        StartCoroutine(UpdateExpBarCoroutine(fromExp, addExp));
    }
    private IEnumerator UpdateExpBarCoroutine(int fromExp, int addExp)
    {
        float timer = 0;
        int gotoExp = fromExp + addExp;
        int baseExp = BaseUtils.GetExpForNextLevel(level - 1);
        while (timer <= 1)
        {
            float currentExp = Mathf.Lerp(fromExp - baseExp, gotoExp - baseExp, timer.Evaluate(CurveType.EaseOut));
            float goScale = Mathf.Clamp(currentExp / (BaseUtils.GetExpForNextLevel(level) - baseExp), .05f, 1);
            levelBar.localScale = new Vector3(goScale, 1, 1);
            expBigBar.localScale = new Vector3(goScale, 1, 1);
            expBigText.SetString($"exp:{Mathf.RoundToInt(currentExp)} / {BaseUtils.GetExpForNextLevel(level) - baseExp}");
            mainMenuController.ShowEffect(EffectType.XpGain, expBigBar.parent, Vector3.right * (expBigBar.sizeDelta.x * goScale + 5));
            timer += Time.deltaTime * 5;
            yield return null;
        }
        levelBar.localScale = new Vector3(Mathf.Clamp((float)(gotoExp - baseExp) / (BaseUtils.GetExpForNextLevel(level) - baseExp), .05f, 1), 1, 1);
        expBigText.SetString($"exp:{Mathf.RoundToInt(gotoExp - baseExp)} / {BaseUtils.GetExpForNextLevel(level) - baseExp}");
    }
    public void SetColor(Color goColor)
    {
        cardImage.color = goColor;
        creatureImage.color = goColor;
        bodyTypeImage.color = goColor;
        damageTypeImage.color = goColor;
        creatureName.SetString(goColor);
        //levelText.SetString(goColor);
        damageNumber.SetString(goColor);
        speedNumber.SetString(goColor);
        magicNumber.SetString(goColor);
        defenseNumber.SetString(goColor);
    }
    public void BumpAttackStats()
    {
        StartCoroutine(BumpText(damageNumber, fightController.damageColor));
        StartCoroutine(BumpText(magicNumber, fightController.magicColor));
        StartCoroutine(BumpImage(damageTypeImage));
    }
    public void BumpDefenseStats()
    {
        StartCoroutine(BumpText(speedNumber, fightController.speedColor));
        StartCoroutine(BumpText(defenseNumber, fightController.defenseColor));
        StartCoroutine(BumpImage(bodyTypeImage));
    }
    public void UpdateHP(float fromHealth, float gotoHealth, float maxHealth)
    {
        hpText.SetString($"HP:{Mathf.Round(fromHealth * 10) / 10}/{Mathf.Round(maxHealth * 10) / 10}");
        hpBar.transform.localScale = new Vector3(1, Mathf.Clamp01(fromHealth / maxHealth), 1);
        if (gotoHealth != -1)
        {
            StartCoroutine(AnimateHPBar(fromHealth, gotoHealth, maxHealth));
        }
    }
    private IEnumerator AnimateHPBar(float fromHealth, float gotoHealth, float maxHealth)
    {
        float timer = 0;
        while (timer <= 1)
        {
            float lerpHP = Mathf.Lerp(fromHealth, gotoHealth, timer.Evaluate(CurveType.EaseOut));
            hpText.SetString($"HP:{Mathf.Clamp(Mathf.Round(lerpHP * 10) / 10, 0, Mathf.Infinity)}/{Mathf.Round(maxHealth * 10) / 10}");
            hpBar.transform.localScale = new Vector3(1, Mathf.Clamp01(lerpHP / maxHealth), 1);
            timer += Time.deltaTime * 3;
            yield return null;
        }
        hpText.SetString($"HP:{Mathf.Clamp(Mathf.Round(gotoHealth * 10) / 10, 0, Mathf.Infinity)}/{Mathf.Round(maxHealth * 10) / 10}");
        hpBar.transform.localScale = new Vector3(1, Mathf.Clamp01(gotoHealth / maxHealth), 1);
    }
    private IEnumerator BumpText(CustomText customText, Color goColor)
    {
        float timer = 0;
        while (timer <= 1)
        {
            customText.SetString(Color.Lerp(Color.white, goColor, timer.Evaluate(CurveType.PeakCurve)));
            customText.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 1.5f, timer.Evaluate(CurveType.PeakCurve));
            timer += Time.deltaTime * 2.5f;
            yield return null;
        }
        customText.SetString(Color.white);
        customText.transform.localScale = Vector3.one;
    }
    private IEnumerator BumpImage(Image goImage)
    {
        float timer = 0;
        while (timer <= 1)
        {
            goImage.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 1.5f, timer.Evaluate(CurveType.PeakCurve));
            timer += Time.deltaTime * 2.5f;
            yield return null;
        }
        goImage.transform.localScale = Vector3.one;
    }
    public void OnSelfClick()
    {
        if (fightingCard)
        {
            return;
        }
        Destroy();
        mainMenuController.cardScreenObj.SetActive(false);
    }
    public void Destroy()
    {
        gameObject.SetActive(false);
        hpBar.gameObject.SetActive(false);
        mainMenuController.showingCard = false;
    }
#if UNITY_EDITOR
    public void PrintPos()
    {
        //1.0, 0.5 left
        //0.0, 0.5 right
        Debug.Log(GetComponent<RectTransform>().anchorMin);
        Debug.Log(GetComponent<RectTransform>().anchorMax);
    }
#endif
}
#if UNITY_EDITOR
[CustomEditor(typeof(CardController))]
public class OfflineCardUI : Editor
{
    public override void OnInspectorGUI()
    {
        CardController generator = (CardController)target;
        DrawDefaultInspector();
        if (GUILayout.Button("Print Pos"))
        {
            generator.PrintPos();
        }
    }
}
#endif
