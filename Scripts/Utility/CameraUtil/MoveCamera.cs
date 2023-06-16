using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    [SerializeField] private Transform _cameraPosition = null;

    private void Update()
    {
        transform.position = _cameraPosition.position;
    }
}