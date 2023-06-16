using UnityEngine;

public class AmbiencesController : MonoBehaviour
{
    [SerializeField] private AmbienceEvents _levelAmbienceEvents = null;

    public void StopAmbiences()
    {
        _levelAmbienceEvents.StopAmbienceEvents(gameObject);
    }
}
