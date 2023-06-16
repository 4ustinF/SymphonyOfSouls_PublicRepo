using UnityEngine;

public class GameOverTrigger : MonoBehaviour
{
    [SerializeField] private GameEvent _onTriggerEvent = null;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            _onTriggerEvent?.Invoke();
        }
    }
}
