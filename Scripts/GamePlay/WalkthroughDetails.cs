using Newtonsoft.Json;
using UnityEngine;

[System.Serializable]
public class WalkthroughDetails
{
    [SerializeField] private float _timeSpent = 0.0f;
    [SerializeField] private int _pointsCollected = -1;

    [JsonProperty] public float TimeSpent { get { return _timeSpent; } private set { _timeSpent = value; } }
    [JsonProperty] public int PointsCollected { get { return _pointsCollected; } private set { _pointsCollected = value; } }


    public WalkthroughDetails(float timeSpent, int pointsCollected)
    {
        _timeSpent = timeSpent;
        _pointsCollected = pointsCollected;
    }
}
