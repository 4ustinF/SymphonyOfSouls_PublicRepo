using UnityEngine;

[System.Serializable]
public class Tier
{
    [SerializeField] private float _tierMultiplier = 0.0f;
    [SerializeField] private int _overTimeMultiplier = 1;
    [SerializeField] private Enums.MultiplierTier _tier = Enums.MultiplierTier.Tier1;

    public float TierMultiplier => _tierMultiplier;
    public Enums.MultiplierTier TierType => _tier;
    public int OverTimeMultiplier => _overTimeMultiplier;
}
