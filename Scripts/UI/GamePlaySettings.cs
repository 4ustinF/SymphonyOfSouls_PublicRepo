using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamePlaySettings : MonoBehaviour
{
    [SerializeField] private Button _resetButton = null;
    [SerializeField] private FloatValue _verticalSensValue = null;
    [SerializeField] private FloatValue _horizontalSensValue = null;
    [SerializeField] private TMP_Dropdown _resolutionDropdown = null;
    [SerializeField] private float _verticalSensDefaultValue = 50.0f;
    [SerializeField] private float _horizontalSensDefaultValue = 50.0f;
    private Resolution[] _resolutions;

    private float _currentRefreshRate = 0.0f;
    private int _currentResolutionIndex = 0;
    private List<Resolution> _filteredResolutions = new List<Resolution>();

    private void OnEnable()
    {
        _resetButton.onClick.AddListener(ResetValues);
        ResolutionSetUp();
    }

    private void OnDisable()
    {
        _resetButton.onClick.RemoveListener(ResetValues);
    }

    private void ResetValues()
    {
        _verticalSensValue.SetValue(_verticalSensDefaultValue);
        _horizontalSensValue.SetValue(_horizontalSensDefaultValue);
    }

    private void ResolutionSetUp()
    {
        _resolutions = Screen.resolutions;
        _filteredResolutions = new List<Resolution>();

        _resolutionDropdown.ClearOptions();
        _currentRefreshRate = Screen.currentResolution.refreshRate;

        for(int i = 0; i < _resolutions.Length; ++i)
        {
            if(_resolutions[i].refreshRate == _currentRefreshRate)
            {
                _filteredResolutions.Add(_resolutions[i]);
            }
        }

        List<string> options = new List<string>();
        for(int i = 0; i < _filteredResolutions.Count; ++i)
        {
            string resolutionOption = $"{_filteredResolutions[i].width} x {_filteredResolutions[i].height} {_filteredResolutions[i].refreshRate} hz";
            options.Add(resolutionOption);
            if (_filteredResolutions[i].width == Screen.width && _filteredResolutions[i].height == Screen.height)
            {
                _currentResolutionIndex = i;
            }
        }

        _resolutionDropdown.AddOptions(options);
        _resolutionDropdown.value = _currentResolutionIndex;
        _resolutionDropdown.RefreshShownValue();
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = _filteredResolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }
}
