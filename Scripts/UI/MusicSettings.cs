using UnityEngine;
using UnityEngine.UI;

public class MusicSettings : MonoBehaviour
{
    [SerializeField] private Button _resetButton = null;
    [SerializeField] private FloatValue _masterValue = null;
    [SerializeField] private FloatValue _musicValue = null;
    [SerializeField] private FloatValue _SFXValue = null;
    [SerializeField] private float _masterDefaultValue = 100.0f;
    [SerializeField] private float _musicDefaultValue = 100.0f;
    [SerializeField] private float _SFXDefaultValue = 100.0f;

    private void OnEnable()
    {
        _resetButton.onClick.AddListener(ResetValues);
    }

    private void OnDisable()
    {
        _resetButton.onClick.RemoveListener(ResetValues);
    }

    private void ResetValues()
    {
        _masterValue.SetValue(_masterDefaultValue);
        _musicValue.SetValue(_musicDefaultValue);
        _SFXValue.SetValue(_SFXDefaultValue);
    }
}
