using UnityEngine;
using UnityEngine.UI;
using static Enums;

public class SettingsUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private StatefulObject _statefulObject = null;
    [SerializeField] private StatefulObject _settingsStatefulObject = null;
    [SerializeField] private SaveLoadSystem _saveLoadSystem = null;

    [Header("Buttons")]
    [SerializeField] private Button _backBTN = null;
    [SerializeField] private Button _gameplaySettingsBTN = null;
    [SerializeField] private Button _musicSettingsBTN = null;
    [SerializeField] private Button _controlsSettingsBTN = null;
    [SerializeField] private Button _delaySettingsBTN = null;

    [Header("Select Images")]
    [SerializeField] private GameObject _gameplaySelect = null;
    [SerializeField] private GameObject _musicSelect = null;
    [SerializeField] private GameObject _controlsSelect = null;
    [SerializeField] private GameObject _delaySelect = null;

    [Header("Settings Sliders")]
    [SerializeField] private SettingsSlider _horizontalSensativity = null;
    [SerializeField] private SettingsSlider _verticalSensativity = null;
    [SerializeField] private SettingsSlider _masterVolume = null;
    [SerializeField] private SettingsSlider _musicVolume = null;
    [SerializeField] private SettingsSlider _sfxVolume = null;
    [SerializeField] private SceneType _currentScene = SceneType.None;

    private void OnEnable()
    {
        _backBTN.interactable = true;
        _settingsStatefulObject.SetToDefaultState();

        LevelLoader levelLoader = ServiceLocator.Get<LevelLoader>();
        if (levelLoader)
        {
            _currentScene = levelLoader.SceneType;
        }

        _backBTN.onClick.AddListener(Hide);
        _gameplaySettingsBTN.onClick.AddListener(ShowGameplaySettings);
        _musicSettingsBTN.onClick.AddListener(ShowMusicSettings);
        _controlsSettingsBTN.onClick.AddListener(ShowControlSettings);
        _delaySettingsBTN.onClick.AddListener(ShowDelaySettingsCanvas);

        EnableTabButtons(TabButtons.GameplaySettingsBTN);

        InitializeSliders();
        ShowGameplaySettings();
    }

    private void OnDisable()
    {
        _backBTN.onClick.RemoveListener(Hide);
        _gameplaySettingsBTN.onClick.RemoveListener(ShowGameplaySettings);
        _musicSettingsBTN.onClick.RemoveListener(ShowMusicSettings);
        _controlsSettingsBTN.onClick.RemoveListener(ShowControlSettings);
        _delaySettingsBTN.onClick.RemoveListener(ShowDelaySettingsCanvas);

        TerminateSliders();
    }

    public void Hide()
    {
        _saveLoadSystem.Save();
        _settingsStatefulObject.SetToDefaultState();

        if (_currentScene == SceneType.MainMenu)
        {
            _statefulObject.PublicSetState(UICanvases.MainMenuCanvas.ToString());
        }
        else
        {
            _statefulObject.PublicSetState(UICanvases.PauseCanvas.ToString());
        }
    }

    public void ShowGameplaySettings()
    {
        _settingsStatefulObject.PublicSetState("GameplaySettings");
        UpdateSelectImages(_gameplaySelect);
        EnableTabButtons(TabButtons.GameplaySettingsBTN);
    }

    public void ShowMusicSettings()
    {
        _settingsStatefulObject.PublicSetState("MusicSettings");
        UpdateSelectImages(_musicSelect);
        EnableTabButtons(TabButtons.MusicSettingsBT);
    }

    public void ShowControlSettings()
    {
        _settingsStatefulObject.PublicSetState("ControlSettings");
        UpdateSelectImages(_controlsSelect);
        EnableTabButtons(TabButtons.ControlsSettingsBTN);
    }

    public void ShowDelaySettingsCanvas()
    {
        _settingsStatefulObject.PublicSetState("DelaySettings");
        UpdateSelectImages(_delaySelect);
        EnableTabButtons(TabButtons.DelaySettingsBTN);
    }

    private void InitializeSliders()
    {
        _horizontalSensativity.Initialize(_saveLoadSystem);
        _verticalSensativity.Initialize(_saveLoadSystem);
        _masterVolume.Initialize(_saveLoadSystem);
        _musicVolume.Initialize(_saveLoadSystem);
        _sfxVolume.Initialize(_saveLoadSystem);
    }

    private void TerminateSliders()
    {
        _horizontalSensativity.Terminate();
        _verticalSensativity.Terminate();
        _masterVolume.Terminate();
        _musicVolume.Terminate();
        _sfxVolume.Terminate();
    }

    public void EnableTabButtons(TabButtons disableButton)
    {
        _gameplaySettingsBTN.interactable = true;
        _musicSettingsBTN.interactable = true;
        _controlsSettingsBTN.interactable = true;
        _delaySettingsBTN.interactable = true;

        switch (disableButton)
        {
            case TabButtons.GameplaySettingsBTN:
                _gameplaySettingsBTN.interactable = false;
                break;
            case TabButtons.MusicSettingsBT:
                _musicSettingsBTN.interactable = false;
                break;
            case TabButtons.ControlsSettingsBTN:
                _controlsSettingsBTN.interactable = false;
                break;
            case TabButtons.DelaySettingsBTN:
                _delaySettingsBTN.interactable = false;
                break;
        }
    }

    public void DisableTabButtons(TabButtons enableButton)
    {
        _gameplaySettingsBTN.interactable = false;
        _musicSettingsBTN.interactable = false;
        _controlsSettingsBTN.interactable = false;
        _delaySettingsBTN.interactable = false;

        switch (enableButton)
        {
            case TabButtons.GameplaySettingsBTN:
                _gameplaySettingsBTN.interactable = true;
                break;
            case TabButtons.MusicSettingsBT:
                _musicSettingsBTN.interactable = true;
                break;
            case TabButtons.ControlsSettingsBTN:
                _controlsSettingsBTN.interactable = true;
                break;
            case TabButtons.DelaySettingsBTN:
                _delaySettingsBTN.interactable = true;
                break;
        }
    }

    private void UpdateSelectImages(GameObject gameObject)
    {
        _gameplaySelect.SetActive(false);
        _musicSelect.SetActive(false);
        _controlsSelect.SetActive(false);
        _delaySelect.SetActive(false);

        gameObject.SetActive(true);
    }
}
