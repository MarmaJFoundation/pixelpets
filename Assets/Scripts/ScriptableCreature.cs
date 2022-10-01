using UnityEngine;

public enum CreatureType
{
    None = 0,
    Trekkin = 1,
    Padoru = 2,
    Gaia = 3,
    Mub = 4,
    Buapi = 5,
    Vex = 6,
    Goette = 7,
    Straia = 8,
    Chiro = 9,
    Zelix = 10,
    Ignim = 11,
    Volbrite = 12,
    Helion = 13,
    Podmol = 14,
    Gravlus = 15,
    Machiron = 16,
    Vyladium = 17,
    Zoandi = 18,
    Minita = 19,
    Sungen = 20,
    Tephra = 21,
    Archor = 22,
    Eschiin = 23,
    Thecoga = 24,
    Adamep = 25,
    Veryl = 26,
    Dratar = 27,
    Blotin = 28,
    Tidae = 29,
    Carpide = 30,
    Derkom = 31,
    Gitt = 32,
    Vanyr = 33,
    Passimus = 34,
    Gabip = 35,
    Mustrakk = 36,
    Olangui = 37,
    Edrak = 38,
    Amusk = 39,
    KinVex = 40,
    Blossar = 41,
    Tachi = 42,
    Repitine = 43,
    Nox = 44,
    Hypollus = 45,
    Amaphila = 46,
    Padonar = 47,
    Hospan = 48,
    Mirchin = 49,
    Laspit = 50,
    Dreed = 51,
    Moley = 52,
    Ryclus = 53,
    Tizi = 54,
    Laczat = 55,
    Spore = 56,
    Phypo = 57,
    Snock = 58,
    Clin = 59,
    Nymphae = 60,
    Choidal = 61,
    Chydaz = 62,
    Krathal = 63,
    Nymph = 64,
    Croncat = 65,
    Gobbler = 66,
    Strax = 67,
    Marark = 68,
    Escheon = 69,
    Hircus = 70,
    Hapran = 71,
    Gagrus = 72,
    Bogy = 73,
    Fletch = 74,
    Shaitan = 75,
    Achtea = 76,
    Pachaon = 77,
    Aconthea = 78,
    Simli = 79,
    Chordata = 80

}
public enum ElementType
{
    Pet = 0,
    Onyx = 1,
    Fire = 2,
    Water = 3,
    Electric = 4,
    Ghost = 5,
    Plant = 6,
    Psychic = 7,
    Sand = 8
}
public enum DropRarity
{
    Common = 0,
    //15%
    Rare = 1,
    //1%
    VeryRare = 2
}
[CreateAssetMenu(fileName = "New Creature", menuName = "Scriptable/Creature")]
public class ScriptableCreature : ScriptableObject
{
    public CreatureType creatureType;
    public CreatureType evolutionType;
    public ElementType damageType;
    public ElementType bodyType;
    public DropRarity dropRarity;
    public Sprite creatureSprite;
    public int levelToEvolve;
    public int evolution;
    public int damage;
    public int speed;
    public int defense;
    public int magic;
    [Space]
    public Vector2 frameOffset;
}
