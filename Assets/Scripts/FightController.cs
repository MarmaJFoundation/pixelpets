using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[Serializable]
public struct PlayerInfo
{
    public string playerAccount;
    public int playerRank;
    public CreatureType[] creatureTypes;
    public RarityType[] rarityTypes;
    public int[] creatureLevels;
    public int[] creatureTrainLevels;
    public int[] creaturePowerLevels;
    public int[] creatureExp;

    public PlayerInfo(string playerAccount, int playerRank, CreatureType[] creatureTypes, RarityType[] rarityTypes, int[] creatureLevels, int[] creatureTrainLevels, int[] creaturePowerLevels, int[] creatureExp)
    {
        this.playerAccount = playerAccount;
        this.playerRank = playerRank;
        this.creatureTypes = creatureTypes;
        this.rarityTypes = rarityTypes;
        this.creatureLevels = creatureLevels;
        this.creatureTrainLevels = creatureTrainLevels;
        this.creaturePowerLevels = creaturePowerLevels;
        this.creatureExp = creatureExp;
    }
}
[Serializable]
public struct CardStats
{
    public int maxHealth;
    public int health;
    public int damage;
    public int speed;
    public int defense;
    public int magic;

    public CardStats(int maxHealth, int health, int damage, int speed, int defense, int magic)
    {
        this.maxHealth = maxHealth;
        this.health = health;
        this.damage = damage;
        this.speed = speed;
        this.defense = defense;
        this.magic = magic;
    }
}
[Serializable]
public struct BattleInfo
{
    public string winnerAccountName;
    public int rankChange;
    public int expGain;
    public PlayerInfo playerInfo1;
    public PlayerInfo playerInfo2;
    public CardStats[] player1CardStats;
    public CardStats[] player2CardStats;
    public List<int> attackerIndexes;
    public List<int> defenderIndexes;
    public List<int> damageDealt;
    public List<bool> allyTurn;
}
public class FightController : MonoBehaviour
{
    public Image[] playerBorders;
    public Image[] playerTrophies;
    public Image lockedButton;
    public Sprite[] lockedSprites;
    public CustomText[] playerRanks;
    public CustomText[] playerNames;
    public RectTransform[] playerParents;
    public MainMenuController mainMenuController;
    public GameObject textPrefab;
    public GameObject cardPrefab;
    public CustomText speedText;
    public Color damageColor;
    public Color magicColor;
    public Color defenseColor;
    public Color speedColor;
    public Color onHitColor;
    private readonly int[] p1VisualIndexes = new int[3] { 0, 1, 2 };
    private readonly int[] p2VisualIndexes = new int[3] { 0, 1, 2 };
    private readonly List<CardController> player1Cards = new List<CardController>();
    private readonly List<CardController> player2Cards = new List<CardController>();
    private bool executingAttack;
    private int lastScreenWidth;
    private int lastScreenHeight;
    private int battleIndex;
    public Queue<TextController> textPool = new Queue<TextController>();
    public List<TextController> activeTexts = new List<TextController>();
    private BattleInfo battleInfo;

    public void Setup(BattleInfo battleInfo)
    {
        this.battleInfo = battleInfo;
        battleIndex = 0;
        BaseUtils.onFight = true;
        lockedButton.sprite = Database.databaseStruct.lockSpeed == -1 ? lockedSprites[0] : lockedSprites[1];
        int rankIndex1 = battleInfo.playerInfo1.playerRank.ToRankIndex();
        int rankIndex2 = battleInfo.playerInfo2.playerRank.ToRankIndex();
        playerBorders[0].sprite = BaseUtils.backSprites[rankIndex1];
        playerBorders[0].material = rankIndex1 >= 5 ? BaseUtils.glowUIMat : BaseUtils.normalUIMat;
        playerBorders[1].sprite = BaseUtils.backSprites[rankIndex2];
        playerBorders[1].material = rankIndex2 >= 5 ? BaseUtils.glowUIMat : BaseUtils.normalUIMat;
        playerTrophies[0].sprite = BaseUtils.trophySprites[rankIndex1];
        playerTrophies[0].SetNativeSize();
        playerTrophies[1].sprite = BaseUtils.trophySprites[rankIndex2];
        playerTrophies[1].SetNativeSize();
        playerRanks[0].SetString(battleInfo.playerInfo1.playerRank.ToString());
        playerRanks[1].SetString(battleInfo.playerInfo2.playerRank.ToString());
        playerNames[0].SetString(battleInfo.playerInfo1.playerAccount);
        playerNames[1].SetString(battleInfo.playerInfo2.playerAccount);
        Time.timeScale = Database.databaseStruct.lockSpeed == -1 ? 1 : Database.databaseStruct.lockSpeed;
        speedText.SetString($"speed:{Time.timeScale}x");
        for (int i = 0; i < 3; i++)
        {
            p1VisualIndexes[i] = i;
            p2VisualIndexes[i] = i;
        }
        BaseUtils.ClearEffects();
        StartCoroutine(FightCoroutine(battleInfo.playerInfo1, battleInfo.playerInfo2));
    }
    private void Update()
    {
        if (lastScreenWidth != Screen.width || lastScreenHeight != Screen.height)
        {
            lastScreenWidth = Screen.width;
            lastScreenHeight = Screen.height;
            for (int i = 0; i < 2; i++)
            {
                playerParents[i].sizeDelta = new Vector2(Screen.width / 2, Screen.height);
            }
            playerParents[0].transform.localPosition = Vector3.zero;
            //playerParents[0].transform.localPosition = Vector3.left * Screen.width / 8;
            playerParents[1].transform.localPosition = Vector3.zero;
            //playerParents[1].transform.localPosition = Vector3.right * Screen.width / 8;
        }
        if (Input.GetKeyDown(KeyCode.Plus) || Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            Time.timeScale++;
            if (Time.timeScale > 10)
            {
                Time.timeScale = 10;
            }
        }
        if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            Time.timeScale--;
            if (Time.timeScale < 1)
            {
                Time.timeScale = 1;
            }
        }
    }
    public void SetTime(bool advancing)
    {
        if (advancing)
        {
            Time.timeScale++;
            if (Time.timeScale > 10)
            {
                Time.timeScale = 10;
            }
        }
        else
        {
            Time.timeScale = 1;
        }
        Database.databaseStruct.lockSpeed = (int)Time.timeScale;
        speedText.SetString($"speed:{Time.timeScale}x");
    }
    public void OnLockClick()
    {
        Database.databaseStruct.lockSpeed = Database.databaseStruct.lockSpeed == -1 ? (int)Time.timeScale : -1;
        lockedButton.sprite = Database.databaseStruct.lockSpeed == -1 ? lockedSprites[0] : lockedSprites[1];
    }
    private CardController InstantiateCard(Transform parentTransform)
    {
        GameObject cardObj = Instantiate(cardPrefab, parentTransform);
        return cardObj.GetComponent<CardController>();
    }
    private IEnumerator FightCoroutine(PlayerInfo playerInfo1, PlayerInfo playerInfo2)
    {
        playerParents[0].SetAsLastSibling();
        for (int i = 0; i < 3; i++)
        {
            CardController cardController = InstantiateCard(playerParents[0]);
            cardController.Setup(this, playerInfo1.creatureTypes[i], playerInfo1.rarityTypes[i], playerInfo1.creatureLevels[i], playerInfo1.creatureTrainLevels[i], playerInfo1.creaturePowerLevels[i], playerInfo1.creatureExp[i]);
            cardController.UpdateHP(battleInfo.player1CardStats[i].health, -1, battleInfo.player1CardStats[i].maxHealth);
            player1Cards.Add(cardController);
            cardController.cardRect.anchorMin = new Vector2(1, .5f);
            cardController.cardRect.anchorMax = new Vector2(1, .5f);
            cardController.cardRect.localPosition = Vector3.zero;
            Vector3 gotoPos = GetSteadyPosition(i, -1);
            mainMenuController.BumpObj(player1Cards[i].transform);
            mainMenuController.ShowEffect((EffectType)(15 + cardController.rarity - 1), cardController.cardRect);
            yield return new WaitForSeconds(.5f);
            StartCoroutine(LerpCard(player1Cards[i], gotoPos, CurveType.EaseInOut, 3));
            yield return new WaitForSeconds(.5f);
        }
        playerParents[1].SetAsLastSibling();
        for (int i = 0; i < 3; i++)
        {
            CardController cardController = InstantiateCard(playerParents[1]);
            cardController.Setup(this, playerInfo2.creatureTypes[i], playerInfo2.rarityTypes[i], playerInfo2.creatureLevels[i], playerInfo2.creatureTrainLevels[i], playerInfo2.creaturePowerLevels[i], playerInfo2.creatureExp[i]);
            cardController.UpdateHP(battleInfo.player2CardStats[i].health, -1, battleInfo.player2CardStats[i].maxHealth);
            player2Cards.Add(cardController);
            cardController.cardRect.anchorMin = new Vector2(0, .5f);
            cardController.cardRect.anchorMax = new Vector2(0, .5f);
            cardController.cardRect.localPosition = Vector3.zero;
            Vector3 gotoPos = GetSteadyPosition(i, 1);
            mainMenuController.BumpObj(player2Cards[i].transform);
            mainMenuController.ShowEffect((EffectType)(15 + cardController.rarity - 1), cardController.cardRect);
            yield return new WaitForSeconds(.5f);
            StartCoroutine(LerpCard(player2Cards[i], gotoPos, CurveType.EaseInOut, 3));
            yield return new WaitForSeconds(.5f);
        }
        while (battleIndex < battleInfo.attackerIndexes.Count)
        {
            int defenderIndex = battleInfo.defenderIndexes[battleIndex];
            int attackerIndex = battleInfo.attackerIndexes[battleIndex];
            int damageDealt = battleInfo.damageDealt[battleIndex];
            bool allyTurn = battleInfo.allyTurn[battleIndex];
            StartCoroutine(ExecuteAttack(defenderIndex, attackerIndex, allyTurn, damageDealt));
            while (executingAttack)
            {
                yield return null;
            }
            yield return new WaitForSeconds(.5f);
        }
        //battle finished here
        for (int i = 0; i < 3; i++)
        {
            Destroy(player1Cards[i].gameObject);
            Destroy(player2Cards[i].gameObject);
        }
        player1Cards.Clear();
        player2Cards.Clear();
        BaseUtils.onFight = false;
        for (int i = 0; i < activeTexts.Count; i++)
        {
            activeTexts[i].Dispose();
        }
        activeTexts.Clear();
        BaseUtils.ClearEffects();
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        mainMenuController.OnFightFinished(battleInfo);
    }
    private Vector3 GetSteadyPosition(int index, int side)
    {
        if (index == 0)
        {
            return new Vector3(200 * side, 72, 0);
        }
        if (index == 1)
        {
            return new Vector3(70 * side, 0, 0);
        }
        return new Vector3(200 * side, -72, 0);
    }
    private IEnumerator ExecuteAttack(int p1CardIndex, int p2CardIndex, bool player1Attacking, int damageDealt)
    {
        //Debug.Log(playerInfo1.creatureTypes[p1CardIndex] + "," + playerInfo2.creatureTypes[p2CardIndex]);
        battleIndex++;
        executingAttack = true;
        bool cardDied;
        yield return new WaitForSeconds(.1f);
        if (player1Attacking)
        {
            playerParents[0].SetAsLastSibling();
            MoveToFirstSlot(p1CardIndex, p2CardIndex);
            yield return new WaitForSeconds(.35f);
            player1Cards[p1CardIndex].BumpAttackStats();
            yield return new WaitForSeconds(.35f);
            player2Cards[p2CardIndex].BumpDefenseStats();
            yield return new WaitForSeconds(.25f);
            CreatureType damagedType = battleInfo.playerInfo2.creatureTypes[p2CardIndex];
            CreatureType attackerType = battleInfo.playerInfo1.creatureTypes[p1CardIndex];
            mainMenuController.ShowEffect(BaseUtils.creatureDict[attackerType].damageType.ToEffect(), player1Cards[p1CardIndex].cardRect);
            mainMenuController.ShowEffect(EffectType.Charge, player1Cards[p1CardIndex].cardRect);
            yield return new WaitForSeconds(.35f);
            StartCoroutine(LerpCard(player1Cards[p1CardIndex], new Vector3(120, 0, 0), CurveType.AttackCurve, 3, true));
            yield return new WaitForSeconds(.2f);
            cardDied = false;
            if (damageDealt == -1)
            {
                //miss
                InstantiateText(player2Cards[p2CardIndex].transform.localPosition, "MISS", 10, speedColor, playerParents[1]);
                StartCoroutine(LerpCard(player2Cards[p2CardIndex],
                new Vector3(60, -60, 0), CurveType.PeakCurve, 3, true, 1));
            }
            else
            {
                int magicDamage = CalculateMagicOnResistance(damagedType, attackerType, battleInfo.player1CardStats[p1CardIndex].magic);
                int physicalDamage = CalculateDamageOnDefense(battleInfo.player2CardStats[p2CardIndex].defense,
                    battleInfo.player1CardStats[p1CardIndex].damage);
                int fromHealth = battleInfo.player2CardStats[p2CardIndex].health;
                battleInfo.player2CardStats[p2CardIndex].health -= damageDealt;
                mainMenuController.ShowEffect(BaseUtils.creatureDict[damagedType].bodyType.ToEffect(), player2Cards[p2CardIndex].cardRect);
                mainMenuController.ShowEffect(EffectType.Hit, player2Cards[p2CardIndex].cardRect);
                StartCoroutine(LerpCard(player2Cards[p2CardIndex],
                new Vector3(Mathf.Clamp(physicalDamage + magicDamage, 5, 25) + 70, 0, 0), CurveType.DamageCurve, 3, true, -1));
                StartCoroutine(PaintCard(player2Cards[p2CardIndex], onHitColor, CurveType.DamageCurve, 3));
                if (physicalDamage > 0)
                {
                    InstantiateText(player2Cards[p2CardIndex].transform.localPosition + Vector3.right * 10,
                    (Mathf.Round(physicalDamage * 10) / 10).ToString(),
                    Mathf.RoundToInt(physicalDamage),
                    damageColor,
                    playerParents[1]);
                    yield return new WaitForSeconds(.1f);
                }
                if (magicDamage > 0)
                {
                    InstantiateText(player2Cards[p2CardIndex].transform.localPosition + Vector3.left * 10,
                    (Mathf.Round(magicDamage * 10) / 10).ToString(),
                    Mathf.RoundToInt(magicDamage),
                    magicColor,
                    playerParents[1]);
                    yield return new WaitForSeconds(.1f);
                    float magicModifier = GetMagicModifier(battleInfo.playerInfo2.creatureTypes[p2CardIndex],
                        battleInfo.playerInfo1.creatureTypes[p1CardIndex]);
                    if (magicModifier > 1)
                    {
                        InstantiateText(player1Cards[p1CardIndex].transform.localPosition + Vector3.right * 50 + Vector3.down * 50,
                            "effective!", 1, magicColor, playerParents[0]);
                    }
                    if (magicModifier < 1)
                    {
                        InstantiateText(player1Cards[p1CardIndex].transform.localPosition + Vector3.right * 50 + Vector3.down * 50,
                            "weak!", 1, magicColor, playerParents[0]);
                    }
                }
                player2Cards[p2CardIndex].UpdateHP(fromHealth, battleInfo.player2CardStats[p2CardIndex].health, battleInfo.player2CardStats[p2CardIndex].maxHealth);

                if (battleInfo.player2CardStats[p2CardIndex].health <= 0)
                {
                    cardDied = true;
                    StartCoroutine(KillCard(player2Cards[p2CardIndex]));
                    yield return new WaitForSeconds(1);
                }
            }
            if (!cardDied)
            {
                yield return new WaitForSeconds(1);
            }
            //MoveCardsBack(p1CardIndex, p2CardIndex);
        }
        else
        {
            playerParents[1].SetAsLastSibling();
            MoveToFirstSlot(p1CardIndex, p2CardIndex);
            yield return new WaitForSeconds(.35f);
            player2Cards[p2CardIndex].BumpAttackStats();
            yield return new WaitForSeconds(.35f);
            player1Cards[p1CardIndex].BumpDefenseStats();
            yield return new WaitForSeconds(.25f);
            CreatureType damagedType = battleInfo.playerInfo1.creatureTypes[p1CardIndex];
            CreatureType attackerType = battleInfo.playerInfo2.creatureTypes[p2CardIndex];
            mainMenuController.ShowEffect(BaseUtils.creatureDict[attackerType].damageType.ToEffect(), player2Cards[p2CardIndex].cardRect);
            mainMenuController.ShowEffect(EffectType.Charge, player2Cards[p2CardIndex].cardRect);
            yield return new WaitForSeconds(.35f);
            StartCoroutine(LerpCard(player2Cards[p2CardIndex], new Vector3(-120, 0, 0), CurveType.AttackCurve, 3, true, 1));
            yield return new WaitForSeconds(.2f);
            cardDied = false;
            if (damageDealt == -1)
            {
                //miss
                InstantiateText(player1Cards[p1CardIndex].transform.localPosition, "MISS", 10, speedColor, playerParents[0]);
                StartCoroutine(LerpCard(player1Cards[p1CardIndex],
                new Vector3(-60, -60, 0), CurveType.PeakCurve, 3, true, 1));
            }
            else
            {
                float physicalDamage = CalculateDamageOnDefense(battleInfo.player1CardStats[p1CardIndex].defense,
                    battleInfo.player2CardStats[p2CardIndex].damage);
                float magicDamage = CalculateMagicOnResistance(damagedType, attackerType, battleInfo.player2CardStats[p2CardIndex].magic);
                float fromHealth = battleInfo.player1CardStats[p1CardIndex].health;
                battleInfo.player1CardStats[p1CardIndex].health -= damageDealt;
                mainMenuController.ShowEffect(BaseUtils.creatureDict[damagedType].bodyType.ToEffect(), player1Cards[p1CardIndex].cardRect);
                mainMenuController.ShowEffect(EffectType.Hit, player1Cards[p1CardIndex].cardRect);
                StartCoroutine(LerpCard(player1Cards[p1CardIndex],
                new Vector3(Mathf.Clamp(physicalDamage + magicDamage, 5, 25) - 70f, 0, 0), CurveType.DamageCurve, 3, true, -1));
                StartCoroutine(PaintCard(player1Cards[p1CardIndex], onHitColor, CurveType.DamageCurve, 3));
                if (physicalDamage > 0)
                {
                    InstantiateText(player1Cards[p1CardIndex].transform.localPosition + Vector3.right * 10,
                    (Mathf.Round(physicalDamage * 10) / 10).ToString(),
                    Mathf.RoundToInt(physicalDamage),
                    damageColor,
                    playerParents[0]);
                    yield return new WaitForSeconds(.1f);
                }
                if (magicDamage > 0)
                {
                    InstantiateText(player1Cards[p1CardIndex].transform.localPosition + Vector3.left * 10,
                        (Mathf.Round(magicDamage * 10) / 10).ToString(),
                        Mathf.RoundToInt(magicDamage),
                        magicColor,
                        playerParents[0]);
                    yield return new WaitForSeconds(.1f);
                    float magicModifier = GetMagicModifier(battleInfo.playerInfo1.creatureTypes[p1CardIndex],
                        battleInfo.playerInfo2.creatureTypes[p2CardIndex]);
                    if (magicModifier > 1)
                    {
                        InstantiateText(player2Cards[p2CardIndex].transform.localPosition + Vector3.left * 50 + Vector3.down * 50,
                            "effective!", 1, magicColor, playerParents[1]);
                    }
                    if (magicModifier < 1)
                    {
                        InstantiateText(player2Cards[p2CardIndex].transform.localPosition + Vector3.left * 50 + Vector3.down * 50,
                            "weak!", 1, magicColor, playerParents[1]);
                    }
                }
                player1Cards[p1CardIndex].UpdateHP(fromHealth, battleInfo.player1CardStats[p1CardIndex].health, battleInfo.player1CardStats[p1CardIndex].maxHealth);
                if (battleInfo.player1CardStats[p1CardIndex].health <= 0)
                {
                    cardDied = true;
                    StartCoroutine(KillCard(player1Cards[p1CardIndex]));
                    yield return new WaitForSeconds(1);
                }
            }
            if (!cardDied)
            {
                yield return new WaitForSeconds(1);
            }
            //MoveCardsBack(p1CardIndex, p2CardIndex);
        }
        executingAttack = false;
    }

    private void MoveToFirstSlot(int p1CardIndex, int p2CardIndex)
    {
        if (p1VisualIndexes[p1CardIndex] != 1)
        {
            StartCoroutine(LerpCard(player1Cards[p1CardIndex], GetSteadyPosition(1, -1), CurveType.EaseInOut, 3));
            for (int i = 0; i < 3; i++)
            {
                if (p1VisualIndexes[i] == 1)
                {
                    StartCoroutine(LerpCard(player1Cards[i], GetSteadyPosition(p1VisualIndexes[p1CardIndex], -1), CurveType.EaseInOut, 2.5f));
                    p1VisualIndexes[i] = p1VisualIndexes[p1CardIndex];
                    break;
                }
            }
            p1VisualIndexes[p1CardIndex] = 1;
        }
        if (p2VisualIndexes[p2CardIndex] != 1)
        {
            StartCoroutine(LerpCard(player2Cards[p2CardIndex], GetSteadyPosition(1, 1), CurveType.EaseInOut, 3));
            for (int i = 0; i < 3; i++)
            {
                if (p2VisualIndexes[i] == 1)
                {
                    StartCoroutine(LerpCard(player2Cards[i], GetSteadyPosition(p2VisualIndexes[p2CardIndex], 1), CurveType.EaseInOut, 2.5f));
                    p2VisualIndexes[i] = p2VisualIndexes[p2CardIndex];
                    break;
                }
            }
            p2VisualIndexes[p2CardIndex] = 1;
        }
    }
    private IEnumerator KillCard(CardController cardController)
    {
        mainMenuController.ShowEffect(EffectType.Desintegrate, cardController.cardRect);
        yield return new WaitForSeconds(.5f);
        StartCoroutine(PaintCard(cardController, damageColor, CurveType.DeathCurve, 1));
        float timer = 0;
        Vector3 fromPos = cardController.transform.localPosition;
        Vector3 gotoPos = fromPos + Vector3.right * 20 * BaseUtils.RandomSign();
        while (timer <= 1)
        {
            cardController.transform.localPosition = Vector3.LerpUnclamped(fromPos, gotoPos, timer.Evaluate(CurveType.DeathCurve));
            timer += Time.deltaTime * 1.25f;
            yield return null;
        }
        cardController.transform.localPosition = fromPos;
        cardController.gameObject.SetActive(false);
    }
    private void InstantiateText(Vector3 fromPos, string gotoString, int damage, Color color, Transform parent)
    {
        TextController textController;
        if (textPool.Count > 0)
        {
            textController = textPool.Dequeue();
            textController.transform.SetParent(parent);
        }
        else
        {
            GameObject textObj = Instantiate(textPrefab, parent);
            textController = textObj.GetComponent<TextController>();
        }
        textController.transform.SetAsLastSibling();
        textController.Setup(this, fromPos, gotoString, color, damage);
    }
    private int CalculateDamageOnDefense(int damagedDefense, int attackDamage)
    {
        int damageSoak = damagedDefense / 500 * damagedDefense;
        int finalDamage = attackDamage - damageSoak;
        return finalDamage > 1 ? finalDamage : 1;
    }
    private int CalculateMagicOnResistance(CreatureType damagedType, CreatureType attackerType, int magicDamage)
    {
        float magicModifier = GetMagicModifier(damagedType, attackerType);
        return Mathf.RoundToInt(magicDamage * magicModifier);
    }
    private float GetMagicModifier(CreatureType damagedType, CreatureType attackerType)
    {
        ElementType bodyType = BaseUtils.creatureDict[damagedType].bodyType;
        ElementType attackType = BaseUtils.creatureDict[attackerType].damageType;
        return BaseUtils.elementMatrix[(int)attackType, (int)bodyType];
    }
    private IEnumerator PaintCard(CardController cardController, Color goColor, CurveType curveType, float speed)
    {
        float timer = 0;
        // fromColor = cardController.GetComponent<Image>().color;
        while (timer <= 1)
        {
            cardController.SetColor(Color.Lerp(Color.white, goColor, timer.Evaluate(curveType)));
            timer += Time.deltaTime * speed;
            yield return null;
        }
        cardController.SetColor(Color.white);
    }
    private IEnumerator LerpCard(CardController cardController, Vector3 gotoPos, CurveType curveType, float speed = 1, bool useRot = false, int rotDirection = -1)
    {
        float timer = 0;
        Vector3 fromPos = cardController.cardRect.localPosition;
        float rotation = Vector3.Distance(fromPos, gotoPos) * .1f * rotDirection;
        while (timer <= 1)
        {
            cardController.cardRect.localPosition = Vector3.LerpUnclamped(fromPos, gotoPos, timer.Evaluate(curveType));
            cardController.cardRect.localEulerAngles = Vector3.LerpUnclamped(Vector3.zero, Vector3.forward * rotation, timer.Evaluate(useRot ? curveType : CurveType.PeakCurve));
            timer += Time.deltaTime * speed;
            yield return null;
        }
        timer = 1;
        cardController.cardRect.localPosition = Vector3.LerpUnclamped(fromPos, gotoPos, timer.Evaluate(curveType));
        cardController.cardRect.localEulerAngles = Vector3.LerpUnclamped(Vector3.zero, Vector3.forward * rotation, timer.Evaluate(useRot ? curveType : CurveType.PeakCurve));
    }
    #region BattleSimulation
    public BattleInfo GenerateBattleInformation(PlayerInfo playerInfo1, PlayerInfo playerInfo2)
    {
        BattleInfo battleInfo = new BattleInfo
        {
            defenderIndexes = new List<int>(),
            attackerIndexes = new List<int>(),
            damageDealt = new List<int>(),
            allyTurn = new List<bool>(),
            playerInfo1 = playerInfo1,
            playerInfo2 = playerInfo2,
            player1CardStats = new CardStats[3],
            player2CardStats = new CardStats[3]
        };
        //set card stats
        for (int i = 0; i < 3; i++)
        {
            ScriptableCreature scriptableCreature = BaseUtils.creatureDict[playerInfo1.creatureTypes[i]];
            int rarity = (int)playerInfo1.rarityTypes[i] + 1;
            int trainLevel = playerInfo1.creatureTrainLevels[i];
            int powerLevel = playerInfo1.creaturePowerLevels[i];
            int level = playerInfo1.creatureLevels[i];
            int evolution = scriptableCreature.evolution;
            //float multiplier = 100 + Mathf.Clamp(Mathf.RoundToInt((playerInfo1.playerRank - 2980) * .05f), 0, Mathf.Infinity);
            battleInfo.player1CardStats[i] = new CardStats(
                BaseUtils.GetModifiedStat(300, rarity, trainLevel, level, evolution, powerLevel),
                BaseUtils.GetModifiedStat(300, rarity, trainLevel, level, evolution, powerLevel),
                BaseUtils.GetModifiedStat(scriptableCreature.damage, rarity, trainLevel, level, evolution, powerLevel),
                BaseUtils.GetModifiedStat(scriptableCreature.speed, rarity, trainLevel, level, evolution, powerLevel),
                BaseUtils.GetModifiedStat(scriptableCreature.defense, rarity, trainLevel, level, evolution, powerLevel),
                BaseUtils.GetModifiedStat(scriptableCreature.magic, rarity, trainLevel, level, evolution, powerLevel));
        }
        for (int i = 0; i < 3; i++)
        {
            ScriptableCreature scriptableCreature = BaseUtils.creatureDict[playerInfo2.creatureTypes[i]];
            int rarity = (int)playerInfo2.rarityTypes[i] + 1;
            int trainLevel = playerInfo2.creatureTrainLevels[i];
            int powerLevel = playerInfo2.creaturePowerLevels[i];
            int level = playerInfo2.creatureLevels[i];
            int evolution = scriptableCreature.evolution;
            //float multiplier = 100 + Mathf.Clamp(Mathf.RoundToInt((playerInfo1.playerRank - 2980) * .05f), 0, Mathf.Infinity);
            battleInfo.player2CardStats[i] = new CardStats(
                BaseUtils.GetModifiedStat(300, rarity, trainLevel, level, evolution, powerLevel),
                BaseUtils.GetModifiedStat(300, rarity, trainLevel, level, evolution, powerLevel),
                BaseUtils.GetModifiedStat(scriptableCreature.damage, rarity, trainLevel, level, evolution, powerLevel) ,
                BaseUtils.GetModifiedStat(scriptableCreature.speed, rarity, trainLevel, level, evolution, powerLevel),
                BaseUtils.GetModifiedStat(scriptableCreature.defense, rarity, trainLevel, level, evolution, powerLevel),
                BaseUtils.GetModifiedStat(scriptableCreature.magic, rarity, trainLevel, level, evolution, powerLevel));
        }
        //execute fight until someone is dead
        while (BothPlayersAlive(battleInfo))
        {
            for (int i = 0; i < 3; i++)
            {
                if (battleInfo.player1CardStats[i].health > 0 && Player2Alive(battleInfo))
                {
                    int enemyIndex = BaseUtils.RandomInt(0, 3);
                    while (battleInfo.player2CardStats[enemyIndex].health <= 0)
                    {
                        enemyIndex++;
                        if (enemyIndex > 2)
                        {
                            enemyIndex = 0;
                        }
                    }
                    battleInfo.defenderIndexes.Add(i);
                    battleInfo.attackerIndexes.Add(enemyIndex);
                    battleInfo.allyTurn.Add(true);
                    RawExecuteAttack(ref battleInfo, i, enemyIndex, true);
                }
                if (battleInfo.player2CardStats[i].health > 0 && Player1Alive(battleInfo))
                {
                    int enemyIndex = BaseUtils.RandomInt(0, 3);
                    while (battleInfo.player1CardStats[enemyIndex].health <= 0)
                    {
                        enemyIndex++;
                        if (enemyIndex > 2)
                        {
                            enemyIndex = 0;
                        }
                    }
                    battleInfo.defenderIndexes.Add(enemyIndex);
                    battleInfo.attackerIndexes.Add(i);
                    battleInfo.allyTurn.Add(false);
                    RawExecuteAttack(ref battleInfo, enemyIndex, i, false);
                }
            }
        }
        //gets the winner
        battleInfo.winnerAccountName = GetWinnerAccountName(battleInfo);
        /*for (int i = 0; i < 3; i++)
        {
            battleInfo.player1CardStats[i].health = battleInfo.player1CardStats[i].maxHealth;
            battleInfo.player2CardStats[i].health = battleInfo.player2CardStats[i].maxHealth;
        }*/
        //returns info of everything that happened
        return battleInfo;
    }
    private void RawExecuteAttack(ref BattleInfo battleInfo, int p1CardIndex, int p2CardIndex, bool player1Attacking)
    {
        if (player1Attacking)
        {
            if (battleInfo.player2CardStats[p2CardIndex].speed > BaseUtils.RandomInt(0, 3000))
            {
                battleInfo.damageDealt.Add(-1);
            }
            else
            {
                int magicDamage = CalculateMagicOnResistance(
                    battleInfo.playerInfo2.creatureTypes[p2CardIndex],
                    battleInfo.playerInfo1.creatureTypes[p1CardIndex],
                    battleInfo.player1CardStats[p1CardIndex].magic);
                int physicalDamage = CalculateDamageOnDefense(
                    battleInfo.player2CardStats[p2CardIndex].defense,
                    battleInfo.player1CardStats[p1CardIndex].damage);
                battleInfo.player2CardStats[p2CardIndex].health -= magicDamage + physicalDamage;
                battleInfo.damageDealt.Add(magicDamage + physicalDamage);
            }
        }
        else
        {
            if (battleInfo.player1CardStats[p1CardIndex].speed > BaseUtils.RandomInt(0, 3000))
            {
                battleInfo.damageDealt.Add(-1);
            }
            else
            {
                int magicDamage = CalculateMagicOnResistance(
                    battleInfo.playerInfo1.creatureTypes[p1CardIndex],
                    battleInfo.playerInfo2.creatureTypes[p2CardIndex],
                    battleInfo.player2CardStats[p2CardIndex].magic);
                int physicalDamage = CalculateDamageOnDefense(
                    battleInfo.player1CardStats[p1CardIndex].defense,
                    battleInfo.player2CardStats[p2CardIndex].damage);
                battleInfo.player1CardStats[p1CardIndex].health -= magicDamage + physicalDamage;
                battleInfo.damageDealt.Add(magicDamage + physicalDamage);
            }
        }
    }
    private bool BothPlayersAlive(BattleInfo battleInfo)
    {
        bool p1Alive = Player1Alive(battleInfo);
        bool p2Alive = Player2Alive(battleInfo);
        return p1Alive && p2Alive;
    }
    private bool Player2Alive(BattleInfo battleInfo)
    {
        for (int i = 0; i < 3; i++)
        {
            if (battleInfo.player2CardStats[i].health > 0)
            {
                return true;
            }
        }
        return false;
    }
    private bool Player1Alive(BattleInfo battleInfo)
    {
        for (int i = 0; i < 3; i++)
        {
            if (battleInfo.player1CardStats[i].health > 0)
            {
                return true;
            }
        }
        return false;
    }
    private string GetWinnerAccountName(BattleInfo battleInfo)
    {
        for (int i = 0; i < 3; i++)
        {
            if (battleInfo.player1CardStats[i].health > 0)
            {
                return battleInfo.playerInfo1.playerAccount;
            }
            if (battleInfo.player2CardStats[i].health > 0)
            {
                return battleInfo.playerInfo2.playerAccount;
            }
        }
        return "";
    }
    #endregion
}
