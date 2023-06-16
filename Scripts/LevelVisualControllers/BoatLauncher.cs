using MoreMountains.Feedbacks;
using UnityEngine;

public class BoatLauncher : MonoBehaviour
{
    [SerializeField] private MMF_Player _boatFeedback = null;

    private void Awake()
    {
        LevelLoader.CallOnComplete(Initialize);
    }

    private void Initialize()
    {
        ServiceLocator.Get<EventBusCallbacks>().OnKillBoxTriggerMessageHandled += PlayerKilledHandle;
    }

    private void OnDestroy()
    {
        ServiceLocator.Get<EventBusCallbacks>().OnKillBoxTriggerMessageHandled -= PlayerKilledHandle;
    }

    private void PlayerKilledHandle(OnKillBoxTrigger obj)
    {
        _boatFeedback.StopFeedbacks();
        _boatFeedback.RestoreInitialValues();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !_boatFeedback.IsPlaying)
        {
            _boatFeedback.PlayFeedbacks();
        }
    }
}
