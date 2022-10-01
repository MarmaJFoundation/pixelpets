using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EffectType
{
    None = 0,
    Charge = 1,
    Hit = 2,
    LevelUp = 3,
    RankUp = 4,
    Evolve = 5,
    Plant = 6,
    Psychic = 7,
    Sand = 8,
    Pet = 9,
    Onyx = 10,
    Fire = 11,
    Water = 12,
    Electric = 13, 
    Ghost = 14,
    GlowNormal = 15,
    GlowRare = 16,
    GlowEpic = 17,
    GlowLegendary = 18,
    Desintegrate = 19,
    SmallGlowNormal = 20,
    SmallGlowRare = 21,
    SmallGlowEpic = 22,
    SmallGlowLegendary = 23,
    Victory = 24,
    Defeat = 25,
    XpGain = 26,
    RankGain = 27,
    Hatch = 28
}
[CreateAssetMenu(fileName = "New Effect", menuName = "Scriptable/Effect")]
public class ScriptableEffects : ScriptableObject
{
    public GameObject effectPrefab;
    public EffectType effectType;
}
