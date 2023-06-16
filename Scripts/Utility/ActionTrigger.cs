using UnityEngine;
using UnityEngine.Events;

public class ActionTrigger : MonoBehaviour
{
    [SerializeField] private bool _isOneShot = true;
    private bool _isPlayed = false;
    public UnityEvent unityAction = null;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (_isOneShot && !_isPlayed)
            {
                _isPlayed = true;
                unityAction?.Invoke();
            }
            else
            {
               // unityAction.Invoke();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (string.Compare(other.tag, "Player", System.StringComparison.OrdinalIgnoreCase) == 0)
        {

        }
    }
}
