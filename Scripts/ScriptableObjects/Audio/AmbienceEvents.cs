using UnityEngine;

[CreateAssetMenu(fileName = "AmbienceEvents", menuName = "ScriptableObjects/Wwise/AmbienceEvents", order = 0)]
public class AmbienceEvents : ScriptableObject
{
    [SerializeField] private AK.Wwise.Event[] _stopEvents = new AK.Wwise.Event[0];

    public void StopAmbienceEvents(GameObject invokingObject)
    {
        foreach (var stopAction in _stopEvents)
        {
            stopAction.Post(invokingObject);
        }
    }
}