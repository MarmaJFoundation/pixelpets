using UnityEngine;

public enum CurveType
{
    None = 0,
    AttackCurve = 1,
    ButtonPressCurve = 2,
    EaseOut = 3,
    EaseIn = 4,
    EaseInOut = 6,
    DeathCurve = 7,
    DamageCurve = 8,
    PeakCurve = 9,
    ShakeCurve = 10
}
[CreateAssetMenu(fileName = "New Curve", menuName = "Scriptable/Curve")]
public class ScriptableCurve : ScriptableObject
{
    public CurveType curveType;
    public AnimationCurve animationCurve;
}