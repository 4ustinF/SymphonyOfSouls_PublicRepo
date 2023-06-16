using UnityEngine;

public class FogController : MonoBehaviour
{
    [SerializeField] private bool _enableFog = false;
    [SerializeField] private FogMode _fogMode = FogMode.ExponentialSquared;
    [SerializeField] private float _fogDensity = 0.015f;
    [SerializeField] private float _linearFogStartDistance = 0.0f;
    [SerializeField] private float _linearFogEndDistance = 300.0f;
    [SerializeField] private Color _fogColor = Color.gray;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") == false)
        {
            return;
        }

        // Enable/Disable fog
        RenderSettings.fog = _enableFog;
        if(_enableFog == false)
        {
            return;
        }

        // Update fog settings
        RenderSettings.fogMode = _fogMode;
        RenderSettings.fogColor = _fogColor;

        switch (_fogMode)
        {
            case FogMode.Linear:
                RenderSettings.fogStartDistance = _linearFogStartDistance;
                RenderSettings.fogEndDistance = _linearFogEndDistance;
                break;
            case FogMode.Exponential:
            case FogMode.ExponentialSquared:
                RenderSettings.fogDensity = _fogDensity;
                break;
        }
    }

}
