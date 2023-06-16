using System.Collections;
using TMPro;
using UnityEngine;
using static Enums;

public class CutSceneManager : MonoBehaviour
{
    [SerializeField] private Animator _animator = null;
    [SerializeField] private TMP_Text _skipText = null;
    [SerializeField] private float _skipTextFadeInTime = 1.0f;
    //[SerializeField] private float _fadeOutTime = 1.0f;

    private IWwiseManager _wiseMan = null; 
    private CutScene _cutScene = CutScene.None;
    private bool _canSkip = false;
    private bool _animationIsPlaying = false;
    private bool _isFinished = false;

    private void Awake()
    {
        _skipText.alpha = 0;
        _canSkip = false;
        _animationIsPlaying = false;
        _isFinished = false;
        LevelLoader.CallOnComplete(Initialize);
        _wiseMan = ServiceLocator.Get<IWwiseManager>();
    }

    private void Initialize()
    {
        _cutScene = GameLoader.CutScene;
        switch (_cutScene)
        {
            case CutScene.LevelOne:
                InitializeClip(GameConsts.CutScene01Name);
                break;
            case CutScene.LevelTwo:
                InitializeClip(GameConsts.CutScene02Name);
                break;
            case CutScene.LevelThree:
                InitializeClip(GameConsts.CutScene03Name);
                break;
        }
    }

    private void InitializeClip(string cutSceneName)
    {
        _animator.Play(cutSceneName);
        _animationIsPlaying = true;
        StartCoroutine(SkipVideoTextFade());
        StartCoroutine(InputForSkipRoutine());
    }

    public void PlayAnimationAudio()
    {
        _wiseMan.PlayCutScene(_cutScene);
    }

    private IEnumerator SkipVideoTextFade()
    {
        float startTime = Time.time;
        float waitTime = startTime + Mathf.Abs(_skipTextFadeInTime);

        while (Time.time < waitTime)
        {
            _skipText.alpha = Mathf.Lerp(0.0f, 1.0f, Time.time / waitTime);
            yield return null;
        }

        _skipText.alpha = 1.0f;
        _canSkip = true;
    }

    private IEnumerator InputForSkipRoutine()
    {
        while (_animationIsPlaying == true)
        {
            if (_canSkip && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))) // TODO: Upgrade to new input system
            {
                Debug.Log("Skipped");
                break;
            }

            yield return null;
        }

        yield return null;
        FinishedCutScene();
    }

    private void FinishedCutScene()
    {
        if(_isFinished == true)
        {
            return;
        }

        Debug.Log("Finished Cut Scene");
        _isFinished = true;

        _animationIsPlaying = false;
        switch (_cutScene)
        {
            case CutScene.LevelOne:
                LoadLevel(GameConsts.level01SceneName);
                break;
            case CutScene.LevelTwo:
                LoadLevel(GameConsts.level02SceneName);
                break;
            case CutScene.LevelThree:
                //LoadLevel(GameConsts.level03SceneName);
                LoadLevel(GameConsts.LevelSelectSceneName);
                break;
        }
    }

    private void LoadLevel(string sceneName)
    {
        _wiseMan.StopSoundEffects();
        ServiceLocator.Get<SceneLoaderManager>().LoadScene(sceneName);
    }
}
