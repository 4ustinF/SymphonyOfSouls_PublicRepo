using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelLoader : AsyncLoader
{
    private static LevelLoader _instance = null;
    private readonly static List<Action> _queuedCallbacks = new List<Action>();

    //[SerializeField] private GameObject _spwaningManager = null;
    [SerializeField] private Enums.SceneType _sceneType = Enums.SceneType.None;
    [SerializeField] private GameEvent onLevelLoadedEvent = null;
    [SerializeField] private PlayerStateMachine _player = null;
    
    public Enums.SceneType SceneType { get => _sceneType; }

    private Enums.SceneType _previousSceneType = Enums.SceneType.None;

    [Serializable]
    public class DummyDictionary
    {
        public System.Type Key;
        public GameObject Value;
    }
   /// [SerializeField] private List<DummyDictionary> _dummyDict = new List<DummyDictionary>();

    protected override void Awake()
    {
        GameLoader.CallOnComplete(LevelSetup);
    }

    // When switching levels we reset the values so they can be overwritten by the new scene and just basic household static cleaning 
    private void OnDestroy()
    {
        ResetVariables();
    }

    public Enums.SceneType GetSceneType()
    {
        return _sceneType;
    }

    private void LevelSetup()
    {
        Debug.Log($"<color=Lime> {this.GetType()} starting setup. </color>");
        Initialize();
        ProcessQueuedCallbacks();
        CallOnComplete(OnComplete);
        ServiceLocator.Get<IWwiseManager>().SetUpLevelAudio();
        ServiceLocator.Get<SceneLoaderManager>().FadeIn();
    }

    private void Initialize()
    {
        _instance = this;
        var wwiseMan = ServiceLocator.Get<IWwiseManager>();

        if(_previousSceneType != this.SceneType)
        {
            wwiseMan.UnloadSpecificSoundbank();
            _previousSceneType = this.SceneType;
        }

        ServiceLocator.Register<LevelLoader>(this, true);
        wwiseMan.SetUpLevelAudio();

        var levelController = Resources.Load("LevelController");
        var levelControllerGO = Instantiate(levelController) as GameObject;
        levelControllerGO.transform.SetParent(_instance.transform);
        levelControllerGO.name = "LevelController";
        var levelControllerComponent = levelControllerGO.GetComponent<LevelController>();
        levelControllerComponent.Initialize(_sceneType);
        ServiceLocator.Register<LevelController>(levelControllerComponent, true);

        var multiplierManager = Resources.Load("MultiplierManager");
        var multiplierManagerGO = Instantiate(multiplierManager) as GameObject;
        multiplierManagerGO.transform.SetParent(_instance.transform);
        multiplierManagerGO.name = "MultiplierManager";
        var multiplierManagerComponent = multiplierManagerGO.GetComponent<MultiplierManager>();
        multiplierManagerComponent.Initialize(_player);
        ServiceLocator.Register<MultiplierManager>(multiplierManagerComponent, true);

        var ambienceManager = Resources.Load("AmbienceManager");
        var ambienceManagerGO = Instantiate(ambienceManager) as GameObject;
        ambienceManagerGO.transform.SetParent(_instance.transform);
        ambienceManagerGO.name = "AmbienceManager";
        var ambienceManagerComponent = ambienceManagerGO.GetComponent<AmbiencesManager>();
        ambienceManagerComponent.Initialize();
        ServiceLocator.Register<AmbiencesManager>(ambienceManagerComponent, true);
    }

    private void ProcessQueuedCallbacks()
    {
        foreach (var callback in _queuedCallbacks)
        {
            callback?.Invoke();
        }
    }

    protected override void ResetVariables()
    {
        base.ResetVariables();
        _queuedCallbacks.Clear();
    }

    public static void CallOnComplete(Action callback)
    {
        if (_instance == null)
        {
            _queuedCallbacks.Add(callback);
            return;
        }

        _instance.CallOnComplete_Internal(callback);
    }

    private void OnComplete()
    {
        Debug.Log($"<color=Lime> {this.GetType()} finished setup. </color>");
        onLevelLoadedEvent?.Invoke();
    }
}