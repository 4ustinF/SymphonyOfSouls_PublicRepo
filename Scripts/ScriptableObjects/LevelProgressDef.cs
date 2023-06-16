using UnityEngine;

[CreateAssetMenu(fileName = "newLevelSettings", menuName = "ScriptableObjects/Level/LevelProgress")]
public class LevelProgressDef : ScriptableObject
{
    public IntValue PointsCollected = null;
    public IntValue Multiplier = null;
    public FloatValue TotalTime = null;
    public FloatValue TimeBonusCounter = null;
    public float currentDecayValue = 0.0f;
}
