using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static Enums;

public class UIManager : MonoBehaviour, Controls.IUIActions
{
    [SerializeField] private StatefulObject _statefulObject = null;
    [SerializeField] private GameUI _gameUI = null;
    [SerializeField] private GameWinUI _gameOverWinUI = null;
    [SerializeField] private MainMenuUI _mainMenuUI = null;
    [SerializeField] private SettingsUI _settingsUI = null;
    [SerializeField] private PauseUI _pauseUI = null;

    public StatefulObject StatefulObject { get => _statefulObject; private set {} }


    public UIManager Initialize()
    {
        Debug.Log($"<color=Cyan> Initializing {this.GetType()} ... </color>");

        _gameOverWinUI.onGameUI += ShowGameUI;
        _gameOverWinUI.onNextLevel += ShowLevelSelect;
        //_gameOverWinUI.onMainMenu += ShowMainMenu;

        _statefulObject.SetToDefaultState();
        var controls = new Controls();
        controls.UI.SetCallbacks(this);
        controls.UI.Enable();

        return this;
    }

    public void OnDisable()
    {
        _gameOverWinUI.onGameUI -= ShowGameUI;
        _gameOverWinUI.onNextLevel -= ShowLevelSelect;
        //_gameOverWinUI.onMainMenu -= ShowMainMenu;
    }

    public void ShowMainMenu()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _statefulObject.PublicSetState(UICanvases.MainMenuCanvas.ToString());
    }

    public void ShowLevelSelect()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _statefulObject.PublicSetState(UICanvases.EmptyCanvas.ToString());
    }

    public void ShowScoreMenu()
    {
        _statefulObject.PublicSetState(UICanvases.LeaderBoardCanvas.ToString());
    }

    public void ShowGameUI()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _statefulObject.PublicSetState(UICanvases.GameCanvas.ToString());
    }

    public void ShowGameOverWin(WalkthroughDetails recentDetails, List<WalkthroughDetails> bestScores)
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _gameOverWinUI.UpdateUI(recentDetails, bestScores);
        StopAllAudioEvents();
        _statefulObject.PublicSetState(UICanvases.GameOverWinUI.ToString());
    }

    public void OnEsc(InputAction.CallbackContext context)
    {
        if (context.performed == false)
        {
            return;
        }

        if (_statefulObject.CurrentState.StateName == UICanvases.PauseCanvas.ToString())
        {
            _pauseUI.Hide();
            LevelController.SetPauseStatus(false);
            return;
        }

        if (_statefulObject.CurrentState.StateName == UICanvases.SettingsCanvas.ToString())
        {
            _settingsUI.Hide();
            return;
        }

        if (_statefulObject.CurrentState.StateName == UICanvases.GameCanvas.ToString())
        {
            _mainMenuUI.OpenPauseUI();
            LevelController.SetPauseStatus(true);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void StopAllAudioEvents()
    {
        var wwiseMan = ServiceLocator.Get<IWwiseManager>();
        wwiseMan?.StopMusicEvent();
        wwiseMan?.StopSoundEffects();
        ServiceLocator.Get<AmbiencesManager>()?.StopAllAmbiences();
        wwiseMan?.UnloadSoundBanks();
    }
}