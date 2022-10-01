using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EnhancedUI.EnhancedScroller;

public class LeaderboardCell : EnhancedScrollerCellView
{
    public Image[] unitImages;
    public Image[] unitBorders;
    public CustomTooltip[] tooltips;
    public CustomText rankText;
    public Image borderImage;
    public Image trophyImage;
    public int DataIndex { get; private set; }

    public void SetData(int dataIndex, LeaderboardData data)
    {
        DataIndex = dataIndex;
        for (int i = 0; i < 3; i++)
        {
            unitImages[i].sprite = BaseUtils.creatureDict[data.rosterCreatures[i]].creatureSprite;
            unitImages[i].rectTransform.localPosition = BaseUtils.creatureDict[data.rosterCreatures[i]].frameOffset;
            unitBorders[i].sprite = BaseUtils.borderSprites[(int)data.rosterRarities[i]];
            tooltips[i].tooltipText[0] = data.rosterCreatures[i].ToString();
        }
        int rankIndex;
        Material normalMat;
        Material outlineMat;
        if (data.isClash)
        {
            rankText.SetString($"{data.listIndex}. w:{data.wins} l:{data.losses} {data.playerName}");
            borderImage.color = data.selfRank ? Color.white : new Color(1, 1, 1, .75f);
            rankIndex = data.ClashRank.ToClashIndex();
            normalMat = rankIndex >= 5 ? BaseUtils.glowUIMat : BaseUtils.normalUIMat;
            outlineMat = rankIndex >= 5 ? BaseUtils.outGlowUIMat : BaseUtils.outlineUIMat;
            borderImage.material = data.selfRank ? outlineMat : normalMat;
            borderImage.sprite = BaseUtils.backSprites[rankIndex];
            trophyImage.sprite = BaseUtils.trophySprites[rankIndex];
            trophyImage.SetNativeSize();
        }
        else
        {
            rankText.SetString($"{data.listIndex}. {data.playerRank} {data.playerName}");
            rankIndex = data.playerRank.ToRankIndex();
            normalMat = rankIndex >= 5 ? BaseUtils.glowUIMat : BaseUtils.normalUIMat;
            outlineMat = rankIndex >= 5 ? BaseUtils.outGlowUIMat : BaseUtils.outlineUIMat;
            borderImage.sprite = BaseUtils.backSprites[rankIndex];
            borderImage.color = data.selfRank ? Color.white : new Color(1, 1, 1, .75f);
            borderImage.material = data.selfRank ? outlineMat : normalMat;
            trophyImage.sprite = BaseUtils.trophySprites[rankIndex];
            trophyImage.SetNativeSize();
        }
    }
}
