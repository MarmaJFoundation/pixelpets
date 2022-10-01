using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EnhancedUI.EnhancedScroller;

public class DropdownCell : EnhancedScrollerCellView
{
    public CustomText creatureText;
    public CustomText dropText;
    public CustomTooltip bodyTooltip;
    public CustomTooltip attackTooltip;
    public GameObject creatureObj;
    public GameObject rankImage;
    public Image backImage;
    public Image bodyType;
    public Image attackType;
    public Image creatureImage;
    public Image frameImage;
    public GameObject pivotObj;
    public int DataIndex { get; private set; }
    private DropdownData data;
    private MarketController marketController;
    public void SetData(MarketController marketController, int dataIndex, DropdownData data)
    {
        this.marketController = marketController;
        this.data = data;
        DataIndex = dataIndex;
        pivotObj.SetActive(true);
        backImage.color = Color.white;
        if (data == null || data.empty)
        {
            pivotObj.SetActive(false);
            backImage.color = new Color(1, 1, 1, 0);
            return;
        }
        if (data.isSpecial)
        {
            creatureText.SetString("strongest pets");
            dropText.SetString("the top 100 pets in market", BaseUtils.GetRarityColor(RarityType.Legendary));
            bodyType.enabled = false;
            attackType.enabled = false;
            rankImage.SetActive(true);
            creatureObj.SetActive(false);
            return;
        }
        bodyType.enabled = true;
        attackType.enabled = true;
        rankImage.SetActive(false);
        creatureObj.SetActive(true);
        creatureText.SetString(data.creatureType.ToString());
        bodyTooltip.elementIndex = 1;
        bodyTooltip.elementType = BaseUtils.creatureDict[data.creatureType].bodyType;
        bodyType.sprite = BaseUtils.typeSpriteDict[BaseUtils.creatureDict[data.creatureType].bodyType];
        bodyType.SetNativeSize();
        attackTooltip.elementIndex = 2;
        attackTooltip.elementType = BaseUtils.creatureDict[data.creatureType].damageType;
        attackType.sprite = BaseUtils.typeSpriteDict[BaseUtils.creatureDict[data.creatureType].damageType];
        attackType.SetNativeSize();
        creatureImage.sprite = BaseUtils.creatureDict[data.creatureType].creatureSprite;
        dropText.SetString($"{data.rarity} drop", BaseUtils.GetRarityColor(data.rarity));
        //frameImage.sprite = BaseUtils.borderSprites[(int)data.rarity];
        creatureImage.rectTransform.anchoredPosition = BaseUtils.creatureDict[data.creatureType].frameOffset;
        //creatureImage.material = data.available ? BaseUtils.normalUIMat : BaseUtils.grayscaleUIMat;
        //backImage.color = data.available ? Color.white : new Color(1, 1, 1, .75f);
    }
    public void OnClick()
    {
        if (data == null)
        {
            return;
        }
        if (data.isSpecial)
        {
            marketController.OnSpecialClick();
        }
        else
        {
            marketController.OnCellClick(data.creatureType.ToString());
        }
    }
}
