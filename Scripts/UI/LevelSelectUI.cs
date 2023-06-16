using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Enums;

public class LevelSelectUI : MonoBehaviour
{
    [SerializeField] private Button _backBTN = null;
    [SerializeField] private List<GameObject> _levelImages = null;
    [SerializeField] private List<Image> _levelBorderImages = null;
    [SerializeField] private Button _scoreButton = null;
    [SerializeField] private Button _levelOneButton = null;
    [SerializeField] private Button _levelTwoButton = null;
    [SerializeField] private Button _levelThreeButton = null;

    [SerializeField] private List<ParticleSystem> _fogParticles = null;
    [SerializeField] private SaveLoadSystem _saveLoadSystem = null;
    [SerializeField] private GameObject _levelTwoParticlesGameObject = null;
    [SerializeField] private GameObject _levelThreeParticlesGameObject = null;

    [SerializeField] private Color _backgroundColor = Color.white; // FFFF80
    [SerializeField] private Color _hoverColor = Color.red; // 8020FF

    private void Awake()
    {
        LevelLoader.CallOnComplete(Initialize);
    }

    private void OnDisable()
    {
        _scoreButton.onClick.RemoveListener(LoadScoreMenu);
        _levelOneButton.onClick.RemoveListener(LoadLevelOne);
        _levelTwoButton.onClick.RemoveListener(LoadLevelTwo);
        _levelThreeButton.onClick.RemoveListener(LoadLevelThree);
        _backBTN.onClick.RemoveListener(LoadMainMenu);
    }

    private void Initialize()
    {
        _levelBorderImages[0].gameObject.SetActive(true); // TODO: Level select background
        _scoreButton.gameObject.SetActive(true);

        _backBTN.onClick.AddListener(LoadMainMenu);
        _levelBorderImages[1].gameObject.SetActive(true);

        if(_saveLoadSystem.GetMaxLevelUnlocked() > 1)
        {
            _levelTwoParticlesGameObject.SetActive(false);
            _levelTwoButton.gameObject.SetActive(true);
            _levelBorderImages[2].gameObject.SetActive(true);
        }

        if (_saveLoadSystem.GetMaxLevelUnlocked() > 2)
        {
            _levelThreeParticlesGameObject.SetActive(false);
            _levelThreeButton.gameObject.SetActive(true);
            _levelBorderImages[3].gameObject.SetActive(true);
        }

        foreach(var image in _levelBorderImages)
        {
            image.color = _backgroundColor;
        }
        
        // Add Listeners
        _scoreButton.onClick.AddListener(LoadScoreMenu);
        _levelOneButton.onClick.AddListener(LoadLevelOne);
        _levelTwoButton.onClick.AddListener(LoadLevelTwo);
        _levelThreeButton.onClick.AddListener(LoadLevelThree);

        foreach (var p in _fogParticles)
        {
            p.Simulate(800.0f);
            p.Play();
        }
    }

    public void ShowBorder(int index)
    {
        _levelBorderImages[index].color = _hoverColor;
    }

    public void HideBorder(int index)
    {
        _levelBorderImages[index].color = _backgroundColor;
    }

    private void LoadScoreMenu()
    {
        // TODO: Load highscore menu
        ServiceLocator.Get<UIManager>().ShowScoreMenu();
    }

    private void LoadLevelOne()
    {
        LoadCutSceneLevel(CutScene.LevelOne);
    }

    private void LoadLevelTwo()
    {
        LoadCutSceneLevel(CutScene.LevelTwo);
    }

    private void LoadLevelThree()
    {
        LoadCutSceneLevel(CutScene.LevelThree);
    }

    private void LoadCutSceneLevel(CutScene cutScene)
    {
        GameLoader.CutScene = cutScene;
        AkSoundEngine.PostEvent("Play_UI_StartGame", gameObject);
        ServiceLocator.Get<IWwiseManager>().StopMusicEvent();
        //SceneManager.LoadScene(GameConsts.CutSceneSceneName);
        ServiceLocator.Get<SceneLoaderManager>().LoadScene(GameConsts.CutSceneSceneName);
    }

    private void LoadMainMenu()
    {
        ServiceLocator.Get<SceneLoaderManager>().LoadScene(SceneIndecies.MainMenu.ToString(), UICanvases.MainMenuCanvas.ToString());
    }
}