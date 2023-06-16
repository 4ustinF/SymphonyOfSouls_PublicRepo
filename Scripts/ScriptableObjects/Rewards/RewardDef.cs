using UnityEngine;

[CreateAssetMenu(fileName = "RewardDef", menuName = "ScriptableObjects/RewardDef", order = 50)]
public class RewardDef : ScriptableObject
{
    public enum ERewardType
    {
        Invalid = -1,
        Points = 0,
    };

    [SerializeField] private ERewardType _rewardType = ERewardType.Invalid;
    [SerializeField] private int _rewardAmount = 0;

    public ERewardType RewardType { get => _rewardType; }
    public int RewardAmount { get => _rewardAmount; }

}
