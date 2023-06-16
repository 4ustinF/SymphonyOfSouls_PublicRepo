using UnityEngine;
using UnityEngine.UI;

public class SettingsSlider : MonoBehaviour
{
    [SerializeField] FloatValue _targetValue = null;
    [SerializeField] Slider _slider = null;
    private SaveLoadSystem _saveLoadSystem = null;

    public void Initialize(SaveLoadSystem saveLoadSystem)
    {
        _saveLoadSystem = saveLoadSystem;
        _targetValue.Changed += UpdateVisual;
        _slider.onValueChanged.AddListener(UpdateTargetValue);
        UpdateVisual();
    }
  
    public void Terminate()
    {
        _targetValue.Changed -= UpdateVisual;
        _slider.onValueChanged.RemoveListener(UpdateTargetValue);
    }

    public void UpdateVisual()
    {
        _slider.value = _targetValue.GetValue;
    }

    private void UpdateTargetValue(float newValue)
    {
        _targetValue.SetValue(newValue);
        //_saveLoadSystem.isAnythingWasChanged;
    }
}