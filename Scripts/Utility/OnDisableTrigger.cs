using UnityEngine;
using UnityEngine.Events;

public class OnDisableTrigger : MonoBehaviour
{
    [SerializeField] private UnityEvent _OnDisableEvent = null;

    private void OnDisable()
    {
        _OnDisableEvent?.Invoke();       
    }
}
