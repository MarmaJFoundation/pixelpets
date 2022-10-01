using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseUtils : MonoBehaviour
{
    public Camera _mainCam;
    public static Camera mainCam;
    public Transform _restingCanvas;
    public static Transform restingCanvas;
    public RectTransform _mainCanvas;
    public static RectTransform mainCanvas;
    public char[] letterChars;
    public Sprite[] mediumLetters;
    public Sprite[] smallLetters;
    public Sprite[] _borderSprites;
    public Sprite[] _backSprites;
    public Sprite[] _cardSprites;
    public Sprite[] _trophySprites;
    public Sprite[] _battleCardSprites;
    public static Sprite[] backSprites;
    public static Sprite[] borderSprites;
    public static Sprite[] cardSprites;
    public static Sprite[] trophySprites;
    public static Sprite[] battleCardSprites;
    public Material _normalUIMat;
    public Material _glowUIMat;
    public Material _outGlowUIMat;
    public Material _outlineUIMat;
    public Material _highlightUI;
    public Material _highLineUI;
    public Material _grayLineUIMat;
    public Material _grayHighlineUIMat;
    public Material[] _grayscaleUIMat;
    public Material[] _graylightUIMat;
    public static Material normalUIMat;
    public static Material glowUIMat;
    public static Material outGlowUIMat;
    public static Material highlightUI;
    public static Material highLineUI;
    public static Material outlineUIMat;
    public static Material grayLineUIMat;
    public static Material grayHighlineUIMat;
    public static Material[] grayscaleUIMat;
    public static Material[] graylightUIMat;
    public Color[] _rarityColors;
    public static Color[] rarityColors;
    public Color _emptyColor;
    public static Color emptyColor;
    public bool _offlineMode;
    public static bool offlineMode;
    public static bool onFight;
    public ScriptableCurve[] curves;
    public ScriptableCreature[] creatures;
    public ScriptableEffects[] effects;
    public Sprite[] typeSprites;
    public static readonly Dictionary<ElementType, Sprite> typeSpriteDict = new Dictionary<ElementType, Sprite>();
    public static readonly Dictionary<CurveType, ScriptableCurve> curveDict = new Dictionary<CurveType, ScriptableCurve>();
    public static readonly Dictionary<EffectType, ScriptableEffects> effectDict = new Dictionary<EffectType, ScriptableEffects>();
    public static readonly Dictionary<CreatureType, ScriptableCreature> creatureDict = new Dictionary<CreatureType, ScriptableCreature>();

    public static readonly Dictionary<char, Sprite> mediumLetterDict = new Dictionary<char, Sprite>();
    public static readonly Dictionary<char, Sprite> smallLetterDict = new Dictionary<char, Sprite>();
    public static readonly Dictionary<char, Queue<Image>> mediumLetterPool = new Dictionary<char, Queue<Image>>();
    public static readonly Dictionary<char, Queue<Image>> smallLetterPool = new Dictionary<char, Queue<Image>>();

    public static Dictionary<EffectType, Queue<EffectController>> effectPool = new Dictionary<EffectType, Queue<EffectController>>();
    public static List<EffectController> activeEffects = new List<EffectController>();

    public static int searchingForMarket = 0;
    //public static List<List<CreatureType>> unitsByRarity = new List<List<CreatureType>>();

    //base
    public static readonly CreatureType[] normalDrops = new CreatureType[24]
    {
        CreatureType.Blotin,CreatureType.Buapi,CreatureType.Eschiin,CreatureType.Gabip,CreatureType.Gitt,CreatureType.Gravlus,
        CreatureType.Hypollus,CreatureType.Ignim,CreatureType.Minita,
        CreatureType.Mirchin,CreatureType.Padoru,CreatureType.Podmol,
        CreatureType.Tachi,CreatureType.Tephra,CreatureType.Thecoga,
        CreatureType.Vanyr,CreatureType.Moley,CreatureType.Tizi,
        CreatureType.Spore,CreatureType.Nymphae,CreatureType.Choidal,CreatureType.Hircus,CreatureType.Bogy,CreatureType.Achtea
    };
    //15%
    public static readonly CreatureType[] rareDrops = new CreatureType[12]
    {
        CreatureType.Blotin,CreatureType.Chiro,CreatureType.Edrak,CreatureType.Hypollus,CreatureType.KinVex,
        CreatureType.Tidae,CreatureType.Trekkin,CreatureType.Zelix,CreatureType.Zoandi,CreatureType.Laspit,CreatureType.Croncat,CreatureType.Simli
    };
    //2%
    public static readonly CreatureType[] veryRareDrops = new CreatureType[3]
    {
        CreatureType.Mub,CreatureType.Straia,CreatureType.Gobbler
    };
    //except sungen
    public static readonly CreatureType[] allCreatures = new CreatureType[79]
    {
        CreatureType.Adamep,CreatureType.Amaphila,CreatureType.Amusk,CreatureType.Achtea,CreatureType.Aconthea,CreatureType.Archor,CreatureType.Blossar,
        CreatureType.Blotin,CreatureType.Buapi,CreatureType.Bogy,CreatureType.Carpide,CreatureType.Chiro,CreatureType.Clin,
        CreatureType.Choidal,CreatureType.Chordata,CreatureType.Chydaz,CreatureType.Croncat,CreatureType.Derkom,CreatureType.Dratar,CreatureType.Dreed,CreatureType.Edrak,
        CreatureType.Eschiin, CreatureType.Escheon,CreatureType.Fletch,CreatureType.Gabip,CreatureType.Gaia,CreatureType.Gagrus,CreatureType.Gitt,CreatureType.Goette,CreatureType.Gobbler,
        CreatureType.Gravlus,CreatureType.Hapran, CreatureType.Helion,CreatureType.Hospan,CreatureType.Hypollus,CreatureType.Hircus,CreatureType.Ignim,
        CreatureType.KinVex,CreatureType.Krathal,CreatureType.Laspit,CreatureType.Laczat,CreatureType.Machiron,CreatureType.Marark,CreatureType.Minita,
        CreatureType.Mirchin,CreatureType.Mub,CreatureType.Mustrakk,CreatureType.Moley,CreatureType.Nox,
        CreatureType.Nymphae,CreatureType.Nymph,CreatureType.Olangui,CreatureType.Padonar,CreatureType.Padoru,CreatureType.Phypo,
        CreatureType.Passimus,CreatureType.Pachaon,CreatureType.Podmol,CreatureType.Repitine,CreatureType.Ryclus,CreatureType.Spore,CreatureType.Snock,CreatureType.Simli,
        CreatureType.Straia,CreatureType.Strax, CreatureType.Shaitan,CreatureType.Tachi,CreatureType.Tephra,CreatureType.Thecoga,CreatureType.Tidae,CreatureType.Tizi,CreatureType.Trekkin,
        CreatureType.Vanyr,CreatureType.Veryl,CreatureType.Vex,CreatureType.Volbrite,CreatureType.Vyladium,CreatureType.Zelix,CreatureType.Zoandi
    };
    public static readonly KeyCode[] modifierKeys = new KeyCode[44]
    {
        KeyCode.LeftShift, KeyCode.RightShift, KeyCode.LeftControl, KeyCode.RightControl,
        KeyCode.AltGr, KeyCode.LeftAlt, KeyCode.RightAlt, KeyCode.Tab, KeyCode.CapsLock,
        KeyCode.Numlock, KeyCode.Print, KeyCode.End, KeyCode.PageDown, KeyCode.PageUp,
        KeyCode.Pause, KeyCode.Insert, KeyCode.ScrollLock, KeyCode.Break, KeyCode.DownArrow,
        KeyCode.LeftArrow,KeyCode.RightArrow,KeyCode.UpArrow,KeyCode.F1,KeyCode.F2,KeyCode.F3,
        KeyCode.F4,KeyCode.F5,KeyCode.F6,KeyCode.F7,KeyCode.F8,KeyCode.F9,KeyCode.F10,KeyCode.F11,
        KeyCode.F12,KeyCode.LeftWindows,KeyCode.RightWindows,KeyCode.SysReq, KeyCode.Mouse0, KeyCode.Mouse1,
        KeyCode.Mouse2,KeyCode.Mouse3,KeyCode.Mouse4,KeyCode.Mouse5,KeyCode.Mouse6
    };
    public static readonly float[,] elementMatrix = new float[9, 9]
    { 
//\/attacker defense ->
              //Pet Onyx Fire Water Electric Ghost Plant Psychic Sand
/*Pet*/       {1,    1.5f,  1,   1,   1,    .5f,     1,   1.5f,  1  },
/*Onyx*/      {.5f,   1,  .5f, 1.5f, 1.5f,  .5f,   1.5f,  .5f,  1.5f},
/*Fire*/      {2f,   .5f,  1,  .5f,   1,     1,    2f,    1,   .5f },
/*Water*/     {1,    .5f, 2f,   1,   .5f,    1,    .5f,    1,    2f },
/*Electric*/  {1.5f,1.5f,  1,  2f,  .5f,    1.5f, .5f,    1,   .5f },
/*Ghost*/     {1.5f,1.5f,  1,    1,   1,    .5f,     1,   1.5f,  1 },
/*Plant*/     {1,    .5f,.5f,  1.5f, 1.5f,   1,    .5f,   2f,  .5f },
/*Psychic*/   {.5f, 1.5f,  1,    1,   1,    2f,    1,    .5f,   1  },
/*Sand*/      {1,    .5f,  1.5f,.5f, 1.5f,    1,   1.5f,    1,    1  }
    };

    public static bool syncedDatabase;

    private void Awake()
    {
        Setup();
    }
    public virtual void Setup()
    {
        Physics.autoSimulation = false;
        offlineMode = _offlineMode;
        normalUIMat = _normalUIMat;
        glowUIMat = _glowUIMat;
        outGlowUIMat = _outGlowUIMat;
        highlightUI = _highlightUI;
        highLineUI = _highLineUI;
        outlineUIMat = _outlineUIMat;
        grayHighlineUIMat = _grayHighlineUIMat;
        grayLineUIMat = _grayLineUIMat;
        grayscaleUIMat = _grayscaleUIMat;
        graylightUIMat = _graylightUIMat;
        restingCanvas = _restingCanvas;
        backSprites = _backSprites;
        trophySprites = _trophySprites;
        borderSprites = _borderSprites;
        battleCardSprites = _battleCardSprites;
        cardSprites = _cardSprites;
        mainCanvas = _mainCanvas;
        mainCam = _mainCam;
        rarityColors = _rarityColors;
        emptyColor = _emptyColor;
        SetLetterDicts();
        SetDicts();
    }
    public void SetLetterDicts()
    {
        mediumLetterDict.Clear();
        smallLetterDict.Clear();
        mediumLetterPool.Clear();
        smallLetterPool.Clear();
        for (int i = 0; i < letterChars.Length; i++)
        {
            mediumLetterDict.Add(letterChars[i], mediumLetters[i]);
            smallLetterDict.Add(letterChars[i], smallLetters[i]);
            mediumLetterPool.Add(letterChars[i], new Queue<Image>());
            smallLetterPool.Add(letterChars[i], new Queue<Image>());
        }
    }
    public virtual void SetDicts()
    {
        for (int i = 0; i < curves.Length; i++)
        {
            curveDict.Add(curves[i].curveType, curves[i]);
        }
        /*for (int i = 0; i < 4; i++)
        {
            unitsByRarity.Add(new List<CreatureType>());
        }*/
        for (int i = 0; i < effects.Length; i++)
        {
            effectDict.Add(effects[i].effectType, effects[i]);
            effectPool.Add(effects[i].effectType, new Queue<EffectController>());
        }
        for (int i = 0; i < typeSprites.Length; i++)
        {
            typeSpriteDict.Add((ElementType)i, typeSprites[i]);
        }
        for (int i = 0; i < creatures.Length; i++)
        {
            creatureDict.Add(creatures[i].creatureType, creatures[i]);

            /*if (creatures[i].rank != -1 && !creatures[i].evolutionOnly)
            {
                unitsByRarity[creatures[i].rank].Add(creatures[i].creatureType);
            }*/
        }
    }
    //#Dan -> This also should be on server side, in this case, how to calculate unit drops from eggs
    public static RarityType GetDropRarity(int eggRarity)
    {
        switch ((RarityType)eggRarity)
        {
            default:
                return RarityType.Common;
            case RarityType.Rare:
                if (RandomInt(0, 100) <= 1)
                {
                    return RarityType.Epic;
                }
                return RarityType.Rare;
            case RarityType.Epic:
                if (RandomInt(0, 100) <= 1)
                {
                    return RarityType.Legendary;
                }
                return RarityType.Epic;
            case RarityType.Legendary:
                return RarityType.Legendary;
        }
    }
    public static bool RandomBool()
    {
        return UnityEngine.Random.Range(0, 100) > 50;
    }
    public static int RandomSign()
    {
        if (RandomBool())
        {
            return -1;
        }
        return 1;
    }
    public static int RandomInt(int rangeX, int rangeY)
    {
        return UnityEngine.Random.Range(rangeX, rangeY);
    }
    public static float RandomFloat(float fromFloat, float gotoFloat)
    {
        return RandomInt(Mathf.RoundToInt(fromFloat * 100), Mathf.RoundToInt(gotoFloat * 100)) / 100f;
    }
    public static int GetExpForNextLevel(int level)
    {
        return Mathf.RoundToInt(50 * level * (1 + level * .2f));
    }
    public static CreatureStruct GenerateCreatureStruct(int eggRarity)
    {
        RarityType rarityType = GetDropRarity(eggRarity);
        CreatureType creatureType = GetDropCreature();
        int creatureID = RandomInt(-900000, 900000);
        return new CreatureStruct(creatureID, 0, 1, 1, 95, 0, DateTime.Now, StateType.InPool, rarityType, creatureType);
    }
    public static void ClearEffects()
    {
        for (int i = 0; i < activeEffects.Count; i++)
        {
            activeEffects[i].Dispose(false);
        }
        activeEffects.Clear();
    }
    public static string GetStrongFromType(ElementType bodyType, bool fromAttack)
    {
        switch (bodyType)
        {
            case ElementType.Pet:
                return fromAttack ? "Onyx § Psychic &" : "Onyx § Psychic &";
            case ElementType.Onyx:
                return fromAttack ? "Water ¨ Electric # Plant $ Sand °" : "Fire ¬ Water ¨ Plant $ Sand °";
            case ElementType.Fire:
                return fromAttack ? "Pet ¢ Plant $" : "Onyx § Plant $";
            case ElementType.Water:
                return fromAttack ? "Fire ¬ Sand °" : "Fire ¬ Sand °";
            case ElementType.Electric:
                return fromAttack ? "Pet ¢ Onyx § Water ¨ Ghost £" : "Electric #";
            case ElementType.Ghost:
                return fromAttack ? "Pet ¢ Onyx § Psychic &" : "Pet ¢ Onyx § Ghost £";
            case ElementType.Plant:
                return fromAttack ? "Water ¨ Electric # Psychic &" : "Water ¨ Electric # Plant $";
            case ElementType.Psychic:
                return fromAttack ? "Onyx § Ghost £" : "Onyx § Psychic &";
            default:
                return fromAttack ? "Fire ¬ Electric # Plant $" : "Fire ¬ Electric # Plant $";
        }
    }
    public static string GetWeakFromType(ElementType bodyType, bool fromAttack)
    {
        switch (bodyType)
        {
            case ElementType.Pet:
                return fromAttack ? "Ghost £" : "Fire ¬ Electric # Ghost £";
            case ElementType.Onyx:
                return fromAttack ? "Pet ¢ Fire ¬ Ghost £ Psychic &" : "Pet ¢ Electric # Ghost £ Psychic &";
            case ElementType.Fire:
                return fromAttack ? "Onyx § Water ¨ Sand °" : "Water ¨ Sand °";
            case ElementType.Water:
                return fromAttack ? "Onyx § Electric # Plant $" : "Onyx § Electric # Plant $";
            case ElementType.Electric:
                return fromAttack ? "Electric # Plant $ Sand °" : "Onyx § Water ¨ Plant $ Sand °";
            case ElementType.Ghost:
                return fromAttack ? "Ghost £" : "Electric # Psychic &";
            case ElementType.Plant:
                return fromAttack ? "Onyx § Fire ¬ Plant $ Sand °" : "Onyx § Fire ¬ Sand °";
            case ElementType.Psychic:
                return fromAttack ? "Pet ¢ Psychic &" : "Pet ¢ Ghost £ Plant $";
            default:
                return fromAttack ? "Onyx § Water ¨" : "Onyx § Water ¨";
        }
    }
    //drops by rarity
    public static CreatureType GetDropCreature()
    {
        int randomDropChance = RandomInt(0, 100);
        if (randomDropChance <= 2)
        {
            return veryRareDrops[RandomInt(0, veryRareDrops.Length)];
        }
        else if (randomDropChance <= 15)
        {
            return rareDrops[RandomInt(0, rareDrops.Length)];
        }
        return normalDrops[RandomInt(0, normalDrops.Length)];
    }
    public static CreatureType GetRandomCreature()
    {
        return allCreatures[RandomInt(0, allCreatures.Length)];
    }
    public static RarityType GetRarity(CreatureType creatureType)
    {
        if (Array.IndexOf(rareDrops, creatureType) != -1)
        {
            return RarityType.Rare;
        }
        if (Array.IndexOf(veryRareDrops, creatureType) != -1)
        {
            return RarityType.Epic;
        }
        return RarityType.Common;
    }
    private static float RarityModifier(int rarity)
    {
        switch (rarity)
        {
            case 1:
                return 1f;
            case 2:
                return 1.1f;
            case 3:
                return 1.17f;
            default:
                return 1.26f;
        }
    }
    public static Material GetGrayMaterial(StateType stateType, bool highlight)
    {
        switch (stateType)
        {
            case StateType.Selected:
                return highlight ? graylightUIMat[1] : grayscaleUIMat[1];
            case StateType.Training:
                return highlight ? graylightUIMat[2] : grayscaleUIMat[2];
            case StateType.Injured:
                return highlight ? graylightUIMat[3] : grayscaleUIMat[3];
            case StateType.Selling:
                return highlight ? graylightUIMat[4] : grayscaleUIMat[4];
            default:
                return highlight ? graylightUIMat[0] : grayscaleUIMat[0];
        }
    }
    public static Color GetRarityColor(RarityType rarityType)
    {
        if (rarityType == RarityType.Legendary)
        {
            return rarityColors[3];
        }
        else if (rarityType == RarityType.Epic)
        {
            return rarityColors[2];
        }
        else if (rarityType == RarityType.Rare)
        {
            return rarityColors[1];
        }
        return rarityColors[0];
    }
    public static int GetModifiedStat(int baseStat, int rarity, int trainLevel, int level, int evolution, int powerlevel)
    {
        return Mathf.RoundToInt((baseStat + (baseStat * evolution * .1f) + (baseStat * trainLevel * .04f) + (baseStat * level * .03f) + (baseStat * (powerlevel - 65) * .02f)) * RarityModifier(rarity));
    }

    public static Vector2Int GetModifiedElo(int yourElo, CreatureStruct creatureStruct1, CreatureStruct creatureStruct2, CreatureStruct creatureStruct3)
    {
        int baseElo = yourElo * 25 - 3500;
        int petLevels = (creatureStruct1.level + creatureStruct2.level + creatureStruct3.level - 50) * 100 - 1500;
        int petTrainLevels = (creatureStruct1.trainLevel + creatureStruct2.trainLevel + creatureStruct3.trainLevel - 3) * 100 - 250;
        int petPowerLevels = (creatureStruct1.powerLevel + creatureStruct2.powerLevel + creatureStruct3.powerLevel - 200) * 100 - 1500;
        int petRarities = ((int)creatureStruct1.rarity + (int)creatureStruct2.rarity + (int)creatureStruct3.rarity) * 700 - 250;
        int modifiedElo = Mathf.Clamp(800 + (baseElo + petLevels + petTrainLevels + petPowerLevels + petRarities) / 50, 800, 20000);
        int percentage = Mathf.Clamp(Mathf.RoundToInt(yourElo * yourElo * .00005f), 150, 10000);
        int divider = Mathf.Clamp(Mathf.RoundToInt(percentage * .003f), 1, 20);
        //Debug.Log(baseElo + "," + petLevels + "," + petTrainLevels + "," + petPowerLevels + "," + petRarities);
        return new Vector2Int(modifiedElo - percentage / divider, modifiedElo + percentage);
    }
    private static float GetExpectedRating(int difference)
    {
        return 1f / (1f + Mathf.Pow(10f, difference / 400f));
    }
    private static int GetNewRating(int previousRating, float expectedRating, bool won)
    {
        float k;
        if (previousRating < 1200)
        {
            k = 32f;
        }
        else if ((previousRating >= 1200) && (previousRating < 1600))
        {
            k = 24f;
        }
        else
        {
            k = 16f;
        }
        int finalRating = previousRating + Mathf.RoundToInt(k * (1f - expectedRating));
        int finalEloGain = finalRating - previousRating;
        if (won && finalEloGain < 3)
        {
            finalRating += 3 - finalEloGain;
        }
        return finalRating;
    }
    public static Vector2Int GetAddedElo(int yourElo, Vector2Int eloPossibilites)
    {
        float winnerExpected1 = GetExpectedRating(eloPossibilites.x - yourElo);
        int winnerRating1 = GetNewRating(yourElo, winnerExpected1, true);
        float winnerExpected2 = GetExpectedRating(eloPossibilites.y - yourElo);
        int winnerRating2 = GetNewRating(yourElo, winnerExpected2, true);
        return new Vector2Int(winnerRating1 - yourElo, winnerRating2 - yourElo);
    }
}
public static class Extensions
{
    //public static bool capsLockOn;
    public static int ToNextElo(this int rank)
    {
        switch (rank)
        {
            case 0:
                return 1100;
            case 1:
                return 1400;
            case 2:
                return 1700;
            case 3:
                return 2000;
            case 4:
                return 2500;
            default:
                return 3000;
        }
    }
    public static int ToClashIndex(this int rank)
    {
        if (rank >= 400)
        {
            return 6;
        }
        else if (rank >= 300)
        {
            return 5;
        }
        else if (rank >= 200)
        {
            return 4;
        }
        else if (rank >= 150)
        {
            return 3;
        }
        else if (rank >= 100)
        {
            return 2;
        }
        else if (rank >= 50)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }
    public static int ToRankIndex(this int rank)
    {
        if (rank >= 3000)
        {
            return 6;
        }
        else if (rank >= 2500)
        {
            return 5;
        }
        else if (rank >= 2000)
        {
            return 4;
        }
        else if (rank >= 1700)
        {
            return 3;
        }
        else if (rank >= 1400)
        {
            return 2;
        }
        else if (rank >= 1100)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }
    public static readonly KeyCode[] acceptKeycodes = new KeyCode[26]
    {
            KeyCode.A, KeyCode.B, KeyCode.C, KeyCode.D, KeyCode.E, KeyCode.F, KeyCode.G, KeyCode.H, KeyCode.I, KeyCode.J,
            KeyCode.K, KeyCode.L, KeyCode.M, KeyCode.N, KeyCode.O, KeyCode.P, KeyCode.Q, KeyCode.R,
            KeyCode.S, KeyCode.T, KeyCode.U, KeyCode.V, KeyCode.X, KeyCode.W, KeyCode.Y, KeyCode.Z
    };
    public static char ToCorrectChar(this KeyCode keyCode)
    {
        if (System.Array.IndexOf(acceptKeycodes, keyCode) != -1)
        {
            return (char)keyCode;
        }
        bool upperCheck = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        switch (keyCode)
        {
            case KeyCode.Alpha0:
            case KeyCode.Keypad0:
                return upperCheck ? ')' : '0';
            case KeyCode.Alpha1:
            case KeyCode.Keypad1:
                return upperCheck ? '!' : '1';
            case KeyCode.Alpha2:
            case KeyCode.Keypad2:
                return upperCheck ? '@' : '2';
            case KeyCode.Alpha3:
            case KeyCode.Keypad3:
                return upperCheck ? '#' : '3';
            case KeyCode.Alpha4:
            case KeyCode.Keypad4:
                return upperCheck ? '$' : '4';
            case KeyCode.Alpha5:
            case KeyCode.Keypad5:
                return upperCheck ? '%' : '5';
            case KeyCode.Alpha6:
            case KeyCode.Keypad6:
                return upperCheck ? '¨' : '6';
            case KeyCode.Alpha7:
            case KeyCode.Keypad7:
                return upperCheck ? '&' : '7';
            case KeyCode.Alpha8:
            case KeyCode.Keypad8:
                return upperCheck ? '*' : '8';
            case KeyCode.Alpha9:
            case KeyCode.Keypad9:
                return upperCheck ? '(' : '9';
            case KeyCode.KeypadPeriod:
                return upperCheck ? '>' : '.';
            case KeyCode.KeypadDivide:
                return upperCheck ? '?' : '/';
            case KeyCode.KeypadMultiply:
                return upperCheck ? '8' : '*';
            case KeyCode.KeypadMinus:
                return upperCheck ? '_' : '-';
            case KeyCode.KeypadPlus:
                return upperCheck ? '=' : '+';
            case KeyCode.KeypadEquals:
                return upperCheck ? '+' : '=';
            case KeyCode.Exclaim:
                return '!';
            case KeyCode.DoubleQuote:
                return '"';
            case KeyCode.Hash:
                return '#';
            case KeyCode.Dollar:
                return '$';
            case KeyCode.Percent:
                return '%';
            case KeyCode.Ampersand:
                return '&';
            case KeyCode.Quote:
                return '\'';
            case KeyCode.LeftParen:
                return '(';
            case KeyCode.RightParen:
                return ')';
            case KeyCode.Asterisk:
                return '*';
            case KeyCode.Plus:
                return '+';
            case KeyCode.Comma:
                return upperCheck ? '<' : ',';
            case KeyCode.Minus:
                return upperCheck ? '_' : '-';
            case KeyCode.Period:
                return upperCheck ? '>' : '.';
            case KeyCode.Slash:
                return upperCheck ? '?' : '/';
            case KeyCode.Colon:
                return upperCheck ? ':' : ';';
            case KeyCode.Semicolon:
                return upperCheck ? ':' : ';';
            case KeyCode.Less:
                return upperCheck ? ',' : '<';
            case KeyCode.Equals:
                return '=';
            case KeyCode.Greater:
                return '>';
            case KeyCode.Question:
                return '?';
            case KeyCode.At:
                return '@';
            case KeyCode.LeftBracket:
                return upperCheck ? '{' : '[';
            case KeyCode.Backslash:
                return '\\';
            case KeyCode.RightBracket:
                return upperCheck ? '}' : ']';
            case KeyCode.Caret:
                return '^';
            case KeyCode.Underscore:
                return '_';
            case KeyCode.BackQuote:
                return '`';
            case KeyCode.LeftCurlyBracket:
                return '{';
            case KeyCode.Pipe:
                return '|';
            case KeyCode.RightCurlyBracket:
                return '}';
            case KeyCode.Tilde:
                return '~';
            case KeyCode.Space:
                return ' ';
            default:
                return '?';
        }
    }
    public static float ToFlatTime(this DateTime dateTime)
    {
        if (DateTime.Now > dateTime)
        {
            return 0;
        }
        double elapsedTicks = (dateTime - DateTime.Now).TotalSeconds;
        //Debug.Log(elapsedTicks);
        return (float)elapsedTicks;
    }
    public static string GetNiceTime(this float currentTime, bool noHours = false)
    {
        int seconds = Mathf.FloorToInt(currentTime % 60f);
        int minutes = Mathf.FloorToInt((currentTime / 60f) % 60f);
        int hours = Mathf.FloorToInt(currentTime / 3600f);
        return noHours ? $"{minutes:00}:{seconds:00}" : $"{hours:00}:{minutes:00}:{seconds:00}";
    }
    public static string GetNiceTime(this float currentTime)
    {
        int minutes = Mathf.FloorToInt((currentTime / 60f) % 60f);
        int hours = Mathf.FloorToInt(currentTime / 3600f % 24f);
        int days = Mathf.FloorToInt(currentTime / 3600f / 24f);
        return $"{days}d {hours}h {minutes}m";
    }
    public static float Evaluate(this float value, CurveType curveType)
    {
        return BaseUtils.curveDict[curveType].animationCurve.Evaluate(value);
    }
    public static EffectType ToEffect(this ElementType elementType)
    {
        switch (elementType)
        {
            case ElementType.Pet:
                return EffectType.Pet;
            case ElementType.Onyx:
                return EffectType.Onyx;
            case ElementType.Fire:
                return EffectType.Fire;
            case ElementType.Water:
                return EffectType.Water;
            case ElementType.Electric:
                return EffectType.Electric;
            case ElementType.Ghost:
                return EffectType.Ghost;
            case ElementType.Plant:
                return EffectType.Plant;
            case ElementType.Psychic:
                return EffectType.Psychic;
            case ElementType.Sand:
                return EffectType.Sand;
            default:
                return EffectType.Hit;
        }
    }
    public static CreatureType ToBaseCreatureType(this CreatureType creatureType)
    {
        switch (creatureType)
        {
            case CreatureType.Gaia:
                return CreatureType.Gabip;
            case CreatureType.Vex:
                return CreatureType.KinVex;
            case CreatureType.Goette:
                return CreatureType.Gitt;
            case CreatureType.Volbrite:
            case CreatureType.Helion:
                return CreatureType.Ignim;
            case CreatureType.Machiron:
                return CreatureType.Chiro;
            case CreatureType.Vyladium:
            case CreatureType.Veryl:
                return CreatureType.Vanyr;
            case CreatureType.Archor:
                return CreatureType.Mirchin;
            case CreatureType.Dratar:
                return CreatureType.Tachi;
            case CreatureType.Carpide:
            case CreatureType.Hospan:
                return CreatureType.Buapi;
            case CreatureType.Derkom:
                return CreatureType.Edrak;
            case CreatureType.Passimus:
            case CreatureType.Padonar:
                return CreatureType.Padoru;
            case CreatureType.Mustrakk:
                return CreatureType.Trekkin;
            case CreatureType.Olangui:
                return CreatureType.Podmol;
            case CreatureType.Amusk:
                return CreatureType.Minita;
            case CreatureType.Blossar:
                return CreatureType.Blotin;
            case CreatureType.Repitine:
                return CreatureType.Adamep;
            case CreatureType.Nox:
                return CreatureType.Laspit;
            case CreatureType.Dreed:
                return CreatureType.Spore;
            case CreatureType.Ryclus:
                return CreatureType.Moley;
            case CreatureType.Laczat:
                return CreatureType.Tizi;
            case CreatureType.Chydaz:
            case CreatureType.Krathal:
                return CreatureType.Choidal;
            case CreatureType.Nymph:
                return CreatureType.Nymphae;
            default:
                return creatureType;
        }
    }
}
