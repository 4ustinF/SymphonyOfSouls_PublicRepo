using UnityEngine;

public class BillBoard : MonoBehaviour
{
    private Camera _cam = null;

    private void Awake()
    {
        if (_cam == null)
        {
            _cam = Camera.current;
        }
    }

    void Update()
    {
        transform.LookAt(_cam.transform);
    }
}
