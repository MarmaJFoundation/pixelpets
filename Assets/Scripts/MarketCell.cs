using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EnhancedUI.EnhancedScroller;

public class MarketCell : EnhancedScrollerCellView
{
    public Image creatureImage;
    public Image borderImage;
    public Image bodyTypeImage;
    public Image attackTypeImage;
    public CustomText nameText;
    public CustomText priceText;
    public CustomText trainLevelText;
    public CustomText powerLevelText;
    public CustomText levelText;
    public CustomText attackText;
    public CustomText defenseText;
    public CustomText speedText;
    public CustomText magicText;
    public CustomTooltip bodyTooltip;
    public CustomTooltip attackTooltip;
    public GameObject pivotObj;
    public GameObject buyButton;
    public GameObject yoursButton;
    public int DataIndex { get; private set; }

    private MarketController marketController;
    private MarketData marketData;

    public void SetData(int dataIndex, MarketController marketController, MarketData marketData)
    {
        DataIndex = dataIndex;
        this.marketController = marketController;
        this.marketData = marketData;
        if (marketData == null)
        {
            gameObject.SetActive(false);
            return;
        }
        gameObject.SetActive(true);
        buyButton.SetActive(marketData.owner != Database.databaseStruct.playerAccount);
        yoursButton.SetActive(!buyButton.activeSelf);
        ScriptableCreature scriptableCreature = BaseUtils.creatureDict[marketData.creatureType];
        creatureImage.sprite = scriptableCreature.creatureSprite;
        bodyTooltip.elementIndex = 1;
        bodyTooltip.elementType = scriptableCreature.bodyType;
        bodyTypeImage.sprite = BaseUtils.typeSpriteDict[scriptableCreature.bodyType];
        bodyTypeImage.SetNativeSize();
        attackTooltip.elementIndex = 2;
        attackTooltip.elementType = scriptableCreature.damageType;
        attackTypeImage.sprite = BaseUtils.typeSpriteDict[scriptableCreature.damageType];
        attackTypeImage.SetNativeSize();
        borderImage.sprite = BaseUtils.borderSprites[(int)marketData.rarityType];
        nameText.SetString(marketData.creatureType.ToString());
        priceText.SetString($"{marketData.price} @");
        int finalLevel = marketData.level - 4;
        if (finalLevel < 1)
        {
            finalLevel = 1;
        }
        levelText.SetString($"lvl:{finalLevel}");
        trainLevelText.SetString($"tlvl:{marketData.trainLevel}");
        powerLevelText.SetString($"plvl:{marketData.powerLevel}");
        int damage = BaseUtils.GetModifiedStat(scriptableCreature.damage, marketData.Rarity + 1, marketData.trainLevel, finalLevel, scriptableCreature.evolution, marketData.powerLevel);
        int speed = BaseUtils.GetModifiedStat(scriptableCreature.speed, marketData.Rarity + 1, marketData.trainLevel, finalLevel, scriptableCreature.evolution, marketData.powerLevel);
        int magic = BaseUtils.GetModifiedStat(scriptableCreature.magic, marketData.Rarity + 1, marketData.trainLevel, finalLevel, scriptableCreature.evolution, marketData.powerLevel);
        int defense = BaseUtils.GetModifiedStat(scriptableCreature.defense, marketData.Rarity + 1, marketData.trainLevel, finalLevel, scriptableCreature.evolution, marketData.powerLevel);
        attackText.SetString(damage.ToString());
        defenseText.SetString(defense.ToString());
        speedText.SetString(speed.ToString());
        magicText.SetString(magic.ToString());
    }
    public void OnBuyClick()
    {
        if (marketData == null)
        {
            return;
        }
        marketController.OnBuyClick(marketData);
    }
}
