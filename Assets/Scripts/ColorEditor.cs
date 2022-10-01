using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class ColorEditor : MonoBehaviour
{
    public Texture2D colorTexture;
    private List<Color> pickedColors;
    public int yourElo;
    public int percentage;
    public int[] level;
    public int[] trainLevel;
    public int[] rarity;
    public int[] powerLevel;
    public void PrintColors()
    {
        Debug.Log(BaseUtils.GetModifiedElo(yourElo,
            new CreatureStruct(-1, 0, level[0], trainLevel[0], powerLevel[0], 0, System.DateTime.Now, StateType.InPool, (RarityType)rarity[0], CreatureType.Adamep),
            new CreatureStruct(-1, 0, level[1], trainLevel[1], powerLevel[1], 0, System.DateTime.Now, StateType.InPool, (RarityType)rarity[1], CreatureType.Adamep),
            new CreatureStruct(-1, 0, level[2], trainLevel[2], powerLevel[2], 0, System.DateTime.Now, StateType.InPool, (RarityType)rarity[2], CreatureType.Adamep)));
        /*int baseElo = yourElo * 20 - 3000;
        int petLevels = (level[0] + level[1] + level[2] - 50) * 120 - 1500;
        int petTrainLevels = (trainLevel[0] + trainLevel[1] + trainLevel[2] - 3) * 120 - 250;
        int petPowerLevels = (powerLevel[0] + powerLevel[1] + powerLevel[2] - 200) * 150 - 1500;
        int petRarities = (rarity[0] + rarity[1] + rarity[2]) * 800 - 250;
        int modifiedElo = Mathf.Clamp(800 + (baseElo + petLevels + petTrainLevels + petPowerLevels + petRarities) / 50, 800, 3000);
        Debug.Log(modifiedElo);
        return;8/
        /*pickedColors = new List<Color>();
        string finalString = "";
        for (int x = 0; x < colorTexture.width; x++)
        {
            for (int y = 0; y < colorTexture.height; y++)
            {
                Color color = colorTexture.GetPixel(x, y);
                if (!pickedColors.Contains(color) && color.a == 1)
                {
                    pickedColors.Add(color);
                    finalString += "Turn(float3(" + Mathf.Round(color.r * 255) + ", " + Mathf.Round(color.g * 255) + ", " + Mathf.Round(color.b * 255) + ") / 255);\n";
                }
            }
        }
        Debug.Log(finalString);*/
    }
    public void PrintExp()
    {
        int totalExp = 0;
        for (int i = 100; i > 0; i--)
        {
            totalExp += BaseUtils.GetExpForNextLevel(i);
        }
        Debug.Log(totalExp);
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(ColorEditor))]
public class ColorEditorUI : Editor
{
    public override void OnInspectorGUI()
    {
        ColorEditor generator = (ColorEditor)target;
        DrawDefaultInspector();
        if (GUILayout.Button("Print Colors"))
        {
            generator.PrintColors();
        }
        if (GUILayout.Button("Print Exp Predicition"))
        {
            generator.PrintExp();
        }
    }
}
#endif

