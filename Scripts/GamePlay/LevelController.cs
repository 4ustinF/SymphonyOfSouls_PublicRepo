using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    [SerializeField] List<LevelSettingsDef> _allLevelSettings = new List<LevelSettingsDef>();
    [SerializeField] private LevelProgressDef _levelProgress = null;
    [SerializeField] private GameEvent _onGameOver = null;
    [SerializeField] private DataAsset _dataAsset = null;

    private LevelSettingsDef _levelSettings = null;
    private UIManager _uiManager = null;
    private bool _isGameOver = false;
    private Enums.SceneType _sceneType = Enums.SceneType.None;
    public static bool IsPaused { get; private set; }
    public static Action onPause = null;

    public static void SetPauseStatus(bool status)
    {
        IsPaused = status;
        onPause?.Invoke();
    }

    public void Initialize(Enums.SceneType sceneType)
    {
        _levelSettings = GetLevelSettings(sceneType);
        _uiManager = ServiceLocator.Get<UIManager>();
        _sceneType = ServiceLocator.Get<LevelLoader>().SceneType;
        IsPaused = false;

        if (sceneType != Enums.SceneType.MainMenu)
        {
            Reset();
        }
        else
        {
            _uiManager.ShowMainMenu();
        }
    }

    public void GameOver()
    {
        _isGameOver = true;
        var currentWalktrougDetails = GetWalkthroughDetails();
        _dataAsset.AddWalktroughDetails(_levelSettings, currentWalktrougDetails);
        _uiManager.ShowGameOverWin(currentWalktrougDetails, GetTopScores(_levelSettings));
    }

    private void Reset()
    {
        if (_sceneType == Enums.SceneType.MainMenu)
        {
            return;
        }

        Debug.Log("Level Reseted!");
        //_levelProgress.TotalTime.SetValue(_levelSettings.TimeToCompleteLevel);
        _levelProgress.TotalTime.SetValue(0.0f);
        _levelProgress.TimeBonusCounter.SetValue(0.0f);
        _levelProgress.PointsCollected.SetValue(0);
        _levelProgress.Multiplier.SetValue(1);
        _isGameOver = false;
    }

    private void Update()
    {
        if (IsPaused || _sceneType == Enums.SceneType.MainMenu)
        {
            return;
        }

        if (!_isGameOver)
        {
            _levelProgress.TotalTime.Add(Time.deltaTime);
        }
    }

    private WalkthroughDetails GetWalkthroughDetails()
    {
        // _levelSettings.TimeToCompleteLevel
        float timePassed = _levelProgress.TotalTime.GetValue;
        var details = new WalkthroughDetails(timePassed, _levelProgress.PointsCollected.GetValue);
        return details;
    }

    private List<WalkthroughDetails> GetTopScores(LevelSettingsDef levelSettingsDef)
    {
        switch (levelSettingsDef.levelName)
        {
            case Enums.LevelName.Level01:
                return _dataAsset.GameData._level1TopDetails;
            case Enums.LevelName.Level02:
                return _dataAsset.GameData._level2TopDetails;
            case Enums.LevelName.Level03:
                return _dataAsset.GameData._level3TopDetails;
            default:
                break;
        }

        Debug.LogError("Wrong Behaviour!");
        return new List<WalkthroughDetails>();
    }

    private LevelSettingsDef GetLevelSettings(Enums.SceneType sceneType)
    {
        return _allLevelSettings.Find(x => x.sceneType == sceneType);
    }
}