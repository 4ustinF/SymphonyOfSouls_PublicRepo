using UnityEngine;

public class CameraUtil : MonoBehaviour
{
    private Camera _cameraComp = null;

    public Camera Camera { get => _cameraComp; }

    public CameraUtil Initialize()
    {
        Debug.Log($"<color=Orange> Initializing {this.GetType()} ... </color>");

        _cameraComp = GetComponent<Camera>();

        return this;
    }
}