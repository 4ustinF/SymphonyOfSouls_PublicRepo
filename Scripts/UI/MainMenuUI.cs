using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Enums;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private StatefulObject _statefulObject = null;

    [SerializeField] private Button _playButton = null;
    [SerializeField] private Button _settingsButton = null;
    [SerializeField] private Button _creditsButton = null;
    [SerializeField] private Button _exitButton = null;
    [SerializeField] private List<GameObject> _buttonHoverImages = null;

    private WwiseManager _wwiseManager = null;

    private void OnEnable()
    {
        _playButton.onClick.AddListener(PlayGame);
        _settingsButton.onClick.AddListener(OpenSettingsUI);
        _creditsButton.onClick.AddListener(OpenCreditsUI);
        _exitButton.onClick.AddListener(QuitGame);
        LevelLoader.CallOnComplete(ControlMenuMusic);

        foreach (var image in _buttonHoverImages)
        {
            image.SetActive(false);
        }
    }

    private void OnDisable()
    {
        _playButton.onClick.RemoveListener(PlayGame);
        _settingsButton.onClick.RemoveListener(OpenSettingsUI);
        _creditsButton.onClick.RemoveListener(OpenCreditsUI);
        _exitButton.onClick.RemoveListener(QuitGame);
    }

    private void PlayGame()
    {
        AkSoundEngine.PostEvent("Play_UI_ButtonMenu", gameObject);
        //_statefulObject.PublicSetState(UICanvases.EmptyCanvas.ToString());
        Cursor.lockState = CursorLockMode.None;

        //SceneManager.LoadScene(GameConsts.LevelSelectSceneName);
        ServiceLocator.Get<SceneLoaderManager>().LoadScene(GameConsts.LevelSelectSceneName, UICanvases.EmptyCanvas.ToString());
    }

    private void OpenCreditsUI()
    {
        _statefulObject.PublicSetState(UICanvases.CreditsCanvas.ToString());
    }

    public void OpenSettingsUI()
    {
        OnSettingsPointerExit();
        _statefulObject.PublicSetState(UICanvases.SettingsCanvas.ToString());
    }

    private void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void OpenPauseUI()
    {
        _statefulObject.PublicSetState(UICanvases.PauseCanvas.ToString());
    }

    private void ControlMenuMusic()
    {
        _wwiseManager = ServiceLocator.Get<IWwiseManager>() as WwiseManager;

        switch (_wwiseManager.SceneType)
        {
            case Enums.SceneType.MainMenu:
                break;
            case Enums.SceneType.Level01:
                _wwiseManager.StopMusicEvent();
                break;
            case Enums.SceneType.Level02:
                _wwiseManager.StopMusicEvent();
                break;
        }
    }

    private void PlayUpwardScrollingUIButtonSoundEffect()
    {
        _wwiseManager = ServiceLocator.Get<IWwiseManager>() as WwiseManager;
        _wwiseManager.PlayUIScrollUpwardSoundEffect();
    }

    private void PlayDownwardScrollingUIButtonSoundEffect()
    {
        _wwiseManager = ServiceLocator.Get<IWwiseManager>() as WwiseManager;
        _wwiseManager.PlayUIScrollDownwardSoundEffect();
    }

    public void OnNewGamePointerEnter()
    {
        _buttonHoverImages[0].SetActive(true);
        PlayUpwardScrollingUIButtonSoundEffect();
    }

    public void OnNewGamePointerExit()
    {
        _buttonHoverImages[0].SetActive(false);
    }

    public void OnContinuePointerEnter()
    {
        _buttonHoverImages[1].SetActive(true);
        PlayUpwardScrollingUIButtonSoundEffect();
    }

    public void OnContinuePointerExit()
    {
        _buttonHoverImages[1].SetActive(false);
    }

    public void OnSettingsPointerEnter()
    {
        _buttonHoverImages[2].SetActive(true);
        PlayUpwardScrollingUIButtonSoundEffect();
    }

    public void OnSettingsPointerExit()
    {
        _buttonHoverImages[2].SetActive(false);
    }

    public void OnQuitPointerEnter()
    {
        _buttonHoverImages[3].SetActive(true);
        PlayUpwardScrollingUIButtonSoundEffect();
    }

    public void OnQuitPointerExit()
    {
        _buttonHoverImages[3].SetActive(false);
    }
}