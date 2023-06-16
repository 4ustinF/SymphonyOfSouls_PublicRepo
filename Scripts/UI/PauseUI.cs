using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Enums;

public class PauseUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private StatefulObject _statefulObject = null;

    [Header("Buttons")]
    [SerializeField] private Button _resumeBTN = null;
    [SerializeField] private Button _restartBTN = null;
    [SerializeField] private Button _SettingsBTN = null;
    [SerializeField] private Button _menuBTN = null;

    private void OnEnable()
    {
        _resumeBTN.onClick.AddListener(Hide);
        _restartBTN.onClick.AddListener(RestartScene);
        _resumeBTN.onClick.AddListener(UnPause);
        _SettingsBTN.onClick.AddListener(ShowSettingsCanvas);
        _menuBTN.onClick.AddListener(GoToMainMenu);
    }

    private void OnDisable()
    {
        _resumeBTN.onClick.RemoveListener(Hide);
        _restartBTN.onClick.RemoveListener(RestartScene);
        _resumeBTN.onClick.RemoveListener(UnPause);
        _SettingsBTN.onClick.RemoveListener(ShowSettingsCanvas);
        _menuBTN.onClick.RemoveListener(GoToMainMenu);
    }

    public void Hide()
    {
        _statefulObject.PublicSetState(UICanvases.GameCanvas.ToString());
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void RestartScene()
    {
        ServiceLocator.Get<IWwiseManager>().StopMusicEvent();
        ServiceLocator.Get<IWwiseManager>().StopSoundEffects();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        //ServiceLocator.Get<SceneLoaderManager>().LoadScene(SceneManager.GetActiveScene().name);
    }

    private void ShowSettingsCanvas()
    {
        _statefulObject.PublicSetState(UICanvases.SettingsCanvas.ToString());
    }

    private void GoToMainMenu()
    {
        _statefulObject.PublicSetState(UICanvases.MainMenuCanvas.ToString());
        SceneManager.LoadScene((int)SceneIndecies.MainMenu);
        //ServiceLocator.Get<SceneLoaderManager>().LoadScene("MainMenu");
    }

    private void UnPause()
    {
        LevelController.SetPauseStatus(false);
    }
}
