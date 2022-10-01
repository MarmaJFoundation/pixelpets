using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RosterCellController : MonoBehaviour
{
    public int databaseIndex;
    public Image borderImage;
    public Image creatureImage;
    public Image bodyImage;
    public Image attackImage;
    public CustomTooltip bodyTooltip;
    public CustomTooltip attackTooltip;
    public CustomText levelText;
    public CustomText trainLevelText;
    public CustomText powerLevelText;
    public CustomText damageText;
    public CustomText defenseText;
    public CustomText magicText;
    public CustomText speedText;
    public CustomText nameText;

    public int overallStats;

    public void Setup(int databaseIndex)
    {
        this.databaseIndex = databaseIndex;
        ScriptableCreature scriptableCreature = BaseUtils.creatureDict[Database.databaseStruct.ownedCreatures[databaseIndex].creatureType];
        int level = Database.databaseStruct.ownedCreatures[databaseIndex].level;
        int trainLevel = Database.databaseStruct.ownedCreatures[databaseIndex].trainLevel;
        int powerLevel = Database.databaseStruct.ownedCreatures[databaseIndex].powerLevel;
        int rarity = (int)Database.databaseStruct.ownedCreatures[databaseIndex].rarity + 1;
        int evolution = scriptableCreature.evolution;
        int damage = BaseUtils.GetModifiedStat(scriptableCreature.damage, rarity, trainLevel, level, evolution, powerLevel);
        int speed = BaseUtils.GetModifiedStat(scriptableCreature.speed, rarity, trainLevel, level, evolution, powerLevel);
        int magic = BaseUtils.GetModifiedStat(scriptableCreature.magic, rarity, trainLevel, level, evolution, powerLevel);
        int defense = BaseUtils.GetModifiedStat(scriptableCreature.defense, rarity, trainLevel, level, evolution, powerLevel);
        overallStats = damage + speed + magic + defense;
        creatureImage.sprite = scriptableCreature.creatureSprite;
        creatureImage.color = Color.white;
        borderImage.sprite = BaseUtils.borderSprites[rarity - 1];
        nameText.SetString(scriptableCreature.creatureType.ToString());
        damageText.SetString(damage.ToString());
        speedText.SetString(speed.ToString());
        magicText.SetString(magic.ToString());
        defenseText.SetString(defense.ToString());
        levelText.SetString($"lvl:{level}");
        trainLevelText.SetString($"tlvl:{trainLevel}");
        powerLevelText.SetString($"plvl:{powerLevel}");
        bodyTooltip.elementType = scriptableCreature.bodyType;
        bodyTooltip.elementIndex = 1;
        bodyImage.enabled = true;
        bodyImage.sprite = BaseUtils.typeSpriteDict[scriptableCreature.bodyType];
        bodyImage.SetNativeSize();
        attackTooltip.elementType = scriptableCreature.damageType;
        attackTooltip.elementIndex = 2;
        attackImage.enabled = true;
        attackImage.sprite = BaseUtils.typeSpriteDict[scriptableCreature.damageType];
        attackImage.SetNativeSize();
    }
    public void Setup(int databaseIndex1, int databaseIndex2)
    {
        ScriptableCreature scriptableCreature = BaseUtils.creatureDict[Database.databaseStruct.ownedCreatures[databaseIndex1].creatureType];
        int level = Database.databaseStruct.ownedCreatures[databaseIndex1].level;
        int trainLevel = Database.databaseStruct.ownedCreatures[databaseIndex1].trainLevel;
        int powerLevel = Database.databaseStruct.ownedCreatures[databaseIndex1].powerLevel;
        int rarity = (int)Database.databaseStruct.ownedCreatures[databaseIndex1].rarity + 1;
        if (powerLevel == 100)
        {
            powerLevel = 80 + (Mathf.FloorToInt(Database.databaseStruct.ownedCreatures[databaseIndex2].powerLevel / 30f) + 1) * 2;
            rarity++;
            if (rarity > 3)
            {
                rarity = 3;
            }
        }
        else
        {
            powerLevel += Mathf.FloorToInt(Database.databaseStruct.ownedCreatures[databaseIndex2].powerLevel / 30f) + 1;
            if (powerLevel > 100)
            {
                powerLevel = 100;
            }
        }
        int evolution = scriptableCreature.evolution;
        int damage = BaseUtils.GetModifiedStat(scriptableCreature.damage, rarity, trainLevel, level, evolution, powerLevel);
        int speed = BaseUtils.GetModifiedStat(scriptableCreature.speed, rarity, trainLevel, level, evolution, powerLevel);
        int magic = BaseUtils.GetModifiedStat(scriptableCreature.magic, rarity, trainLevel, level, evolution, powerLevel);
        int defense = BaseUtils.GetModifiedStat(scriptableCreature.defense, rarity, trainLevel, level, evolution, powerLevel);
        creatureImage.sprite = scriptableCreature.creatureSprite;
        creatureImage.color = Color.white;
        borderImage.sprite = BaseUtils.borderSprites[rarity - 1];
        nameText.SetString(scriptableCreature.creatureType.ToString());
        damageText.SetString(damage.ToString());
        speedText.SetString(speed.ToString());
        magicText.SetString(magic.ToString());
        defenseText.SetString(defense.ToString());
        levelText.SetString($"lvl:{level}");
        trainLevelText.SetString($"tlvl:{trainLevel}");
        powerLevelText.SetString($"plvl:{powerLevel}");
        bodyTooltip.elementType = scriptableCreature.bodyType;
        bodyTooltip.elementIndex = 1;
        bodyImage.enabled = true;
        bodyImage.sprite = BaseUtils.typeSpriteDict[scriptableCreature.bodyType];
        bodyImage.SetNativeSize();
        attackTooltip.elementType = scriptableCreature.damageType;
        attackTooltip.elementIndex = 2;
        attackImage.enabled = true;
        attackImage.sprite = BaseUtils.typeSpriteDict[scriptableCreature.damageType];
        attackImage.SetNativeSize();
    }
    public void Clear()
    {
        databaseIndex = -1;
        creatureImage.sprite = null;
        creatureImage.color = BaseUtils.emptyColor;
        borderImage.sprite = BaseUtils.borderSprites[4];
        nameText.SetString("?");
        damageText.SetString("0");
        speedText.SetString("0");
        magicText.SetString("0");
        defenseText.SetString("0");
        levelText.SetString("lvl:0");
        trainLevelText.SetString("tlvl:0");
        powerLevelText.SetString("plvl:0");
        bodyImage.enabled = false;
        attackImage.enabled = false;
    }
}
