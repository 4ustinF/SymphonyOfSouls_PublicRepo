using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RewardReciver", menuName = "ScriptableObjects/RewardReciver", order = 50)]
public class RewardReciverDef : ScriptableObject, IRewardReciever
{
    [SerializeField] private IntValue pointsReference = null;

    public void Apply(List<RewardDef> rewards)
    {
        foreach (var reward in rewards)
        {
            ApplyReward(reward);
        }
    }

    private void ApplyReward(RewardDef reward)
    {
        switch (reward.RewardType)
        {
            case RewardDef.ERewardType.Invalid:
                Debug.LogError("Invalid type of reward: " + reward.name);
                break;
            case RewardDef.ERewardType.Points:
                AddPoints(reward.RewardAmount);
                break;
        }
    }

    private void AddPoints(int points)
    {
        pointsReference.Add(points);
    }
}
