using UnityEngine;

public class KillTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("NPC"))
        {
            OnSendMessageKillTrigger();
        }
    }

    public void OnSendMessageKillTrigger()
    {
        var messenger = ServiceLocator.Get<IEventBusSystemHub>();
        messenger.Publish(new OnKillBoxTrigger(this));
    }
}
