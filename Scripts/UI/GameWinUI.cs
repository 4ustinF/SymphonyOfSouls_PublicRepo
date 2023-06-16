using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameWinUI : MonoBehaviour
{
    [SerializeField] private Button _nextLevelButton = null;
    [SerializeField] private Button _replayButton = null;

    [SerializeField] private TMP_Text _resultTimeTxt = null;
    [SerializeField] private TMP_Text _resultPointsTxt = null;
    //[SerializeField] private TMP_Text _topResultsTxt = null;

    //public Action onMainMenu = null;
    public Action onNextLevel = null;
    public Action onGameUI = null;

    private void OnEnable()
    {
        _nextLevelButton.onClick.AddListener(LoadNextLevelScene);
        _replayButton.onClick.AddListener(ReloadLevel);
    }

    private void OnDisable()
    {
        _nextLevelButton.onClick.RemoveListener(LoadNextLevelScene);
        _replayButton.onClick.RemoveListener(ReloadLevel);
    }

    private void LoadNextLevelScene()
    {
        Debug.Log("Loading LevelSelect Screen");
        StopAllAudioEvents();
        SceneManager.LoadScene(GameConsts.LevelSelectSceneName);

        //ServiceLocator.Get<SceneLoaderManager>().LoadScene("MainMenu");
        //onMainMenu?.Invoke();
        onNextLevel.Invoke();
    }

    private void ReloadLevel()
    {
        Debug.Log("Reloading Level");
        StopAllAudioEvents();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        //ServiceLocator.Get<SceneLoaderManager>().LoadScene(SceneManager.GetActiveScene().name);
        onGameUI?.Invoke();
    }

    public void UpdateUI(WalkthroughDetails recentDetails, List<WalkthroughDetails> bestScores)
    {
        DrawRecentResult(recentDetails);
        //DrawTopScores(bestScores);
    }

    //private void DrawTopScores(List<WalkthroughDetails> bestScores)
    //{
    //    string scoresText = string.Empty;
    //    for (int i = 0; i < bestScores.Count; i++)
    //    {
    //        scoresText += $"{i + 1} Points: {bestScores[i].PointsCollected}  Time Spent: {System.Math.Round(bestScores[i].TimeSpent, 2)}.\n"; 
    //    }
    //    _topResultsTxt.text = scoresText;
    //}

    private void DrawRecentResult(WalkthroughDetails recent)
    {
        _resultTimeTxt.text = $"<color=#FF0000> Time\n<color=#000000> {System.Math.Round(recent.TimeSpent, 2)}";
        _resultPointsTxt.text = $"<color=#FF0000> Score\n<color=#000000> {recent.PointsCollected}";
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
