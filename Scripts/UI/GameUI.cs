using MoreMountains.Feedbacks;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [SerializeField] private MusicInformation _musicInformation = null;
    [SerializeField] private TMP_Text _gameTimerText = null;
    [SerializeField] private TMP_Text _pointsText = null;
    [SerializeField] private TMP_Text _multiplierText = null;
    [SerializeField] private LevelProgressDef levelProgressDef = null;
    [SerializeField] private Image _decayProgressSliderImage = null;
    [SerializeField] private MMF_Player _onLevelUpFeedback = null;

    [SerializeField] private Image _onBeatImage = null;
    [SerializeField] private Image _leftArrowImage = null;
    [SerializeField] private Image _rightArrowImage = null;
    [SerializeField] private Transform _leftArrowTransform = null;
    [SerializeField] private Transform _rightArrowTransform = null;
    [SerializeField] private Transform _leftArrowDestTransform = null;
    [SerializeField] private Transform _rightArrowDestTransform = null;
    [SerializeField] private AnimationCurve _movingArrowPos = new AnimationCurve();
    [SerializeField] private AnimationCurve _movingArrowScale = new AnimationCurve();
    [SerializeField] private AnimationCurve _movingArrowTransparency = new AnimationCurve();
    [SerializeField] private AnimationCurve _onBeatTransparency = new AnimationCurve();

    private IEnumerator _arrowAnimationCoroutine = null;
    private Vector3 _leftArrowStartPos = Vector3.one;
    private Vector3 _leftArrowStartScale = Vector3.one;
    private Vector3 _leftArrowDestPos = Vector3.one;
    private Vector3 _rightArrowStartPos = Vector3.one;
    private Vector3 _rightArrowStartScale = Vector3.one;
    private Vector3 _rightArrowDestPos = Vector3.one;

    private EventBusCallbacks _eventBusCallbacks = null;
    private FloatValue timerReference = null;
    private IntValue pointsReference = null;
    private IntValue multiplierReference = null;
    private float _maxAnimTime = 0.55f;
    private float _onBeatAnimTime = 0.3f;

    private void OnEnable()
    {
        LevelLoader.CallOnComplete(Initialize);
    }

    private void OnDisable()
    {
        timerReference.Changed -= UpdateTimerText;
        pointsReference.Changed -= UpdatePointsText;
        multiplierReference.Changed -= UpdateMultiplierText;

        _eventBusCallbacks.OnMusicBeatMessageHandled -= PlayAnimation;
        _eventBusCallbacks.OnBeatInputMessageHandled -= PlayOnbeatAnimation;

        _leftArrowTransform.transform.position = _leftArrowStartPos;
        _leftArrowTransform.transform.localScale = _leftArrowStartScale;
        _rightArrowTransform.transform.position = _rightArrowStartPos;
        _rightArrowTransform.transform.localScale = _rightArrowStartScale;
    }

    private void Initialize()
    {
        timerReference = levelProgressDef.TotalTime;
        pointsReference = levelProgressDef.PointsCollected;
        multiplierReference = levelProgressDef.Multiplier;

        timerReference.Changed += UpdateTimerText;
        pointsReference.Changed += UpdatePointsText;
        multiplierReference.Changed += UpdateMultiplierText;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _eventBusCallbacks = ServiceLocator.Get<EventBusCallbacks>();
        _eventBusCallbacks.OnMusicBeatMessageHandled += PlayAnimation;
        _eventBusCallbacks.OnBeatInputMessageHandled += PlayOnbeatAnimation;

        _leftArrowStartPos = _leftArrowTransform.position;
        _leftArrowStartScale = _leftArrowTransform.localScale;
        _leftArrowDestPos = _leftArrowDestTransform.position;
        _rightArrowStartPos = _rightArrowTransform.position;
        _rightArrowStartScale = _rightArrowTransform.localScale;
        _rightArrowDestPos = _rightArrowDestTransform.position;

        _onBeatImage.color = new Color(_onBeatImage.color.r, _onBeatImage.color.g, _onBeatImage.color.b, 0.0f);

        var sceneType = ServiceLocator.Get<LevelLoader>().SceneType;
        switch(sceneType)
        {
            case Enums.SceneType.Level01:
                _maxAnimTime = 0.45f;
                break;
            case Enums.SceneType.Level02:
                _maxAnimTime = 0.55f;
                break;
        }

        UpdateMultiplierText();
        //_maxAnimTime = Mathf.Abs(_musicInformation.BeatDuration - 0.05f);
    }

    public void UpdatePointsText()
    {
        _pointsText.text = $"Points: {pointsReference.GetValue}";
    }

    public void UpdateTimerText()
    {
        int totalSeconds = Mathf.FloorToInt(timerReference.GetValue + 0.5f);
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        _gameTimerText.text = string.Format("{0}:{1:00}", minutes, seconds);
        _decayProgressSliderImage.fillAmount = levelProgressDef.currentDecayValue;
    }

    public void UpdateMultiplierText()
    {
        _onLevelUpFeedback?.PlayFeedbacks();
        _multiplierText.text = $"X{multiplierReference.GetValue}";
        _decayProgressSliderImage.fillAmount = multiplierReference.GetValue;
    }

    public float GetTimerValue()
    {
        return timerReference.GetValue;
    }

    private void PlayAnimation(OnMusicBeatMessage message)
    {
        if (_arrowAnimationCoroutine != null)
        {
            StopCoroutine(_arrowAnimationCoroutine);
        }

        _arrowAnimationCoroutine = ArrowAnimation();
        StartCoroutine(_arrowAnimationCoroutine);
    }

    private IEnumerator ArrowAnimation()
    {
        float time = 0.0f;

        while (time <= _maxAnimTime)
        {
            time += Time.deltaTime;
            float percentage = time / _maxAnimTime;
            float moveAmount = _movingArrowPos.Evaluate(percentage);
            float scaleAmount = _movingArrowScale.Evaluate(percentage);
            float transparencyAmount = _movingArrowTransparency.Evaluate(percentage);

            // Position
            _leftArrowTransform.transform.position = Vector3.Lerp(_leftArrowStartPos, _leftArrowDestPos, moveAmount);
            _rightArrowTransform.transform.position = Vector3.Lerp(_rightArrowStartPos, _rightArrowDestPos, moveAmount);

            // Scale
            _leftArrowTransform.transform.localScale = new Vector3(scaleAmount, scaleAmount, 1.0f);
            _rightArrowTransform.transform.localScale = new Vector3(-scaleAmount, scaleAmount, 1.0f);

            // Transparency
            _leftArrowImage.color = new Color(_leftArrowImage.color.r, _leftArrowImage.color.g, _leftArrowImage.color.b, transparencyAmount);
            _rightArrowImage.color = new Color(_rightArrowImage.color.r, _rightArrowImage.color.g, _rightArrowImage.color.b, transparencyAmount);

            yield return null;
        }

        _leftArrowTransform.transform.position = _leftArrowStartPos;
        _leftArrowTransform.transform.localScale = _leftArrowStartScale;
        _rightArrowTransform.transform.position = _rightArrowStartPos;
        _rightArrowTransform.transform.localScale = _rightArrowStartScale;
    }

    private void PlayOnbeatAnimation(OnBeatInputMessage message)
    {
        if(message.IsOnTime == false)
        {
            return;
        }

        StartCoroutine(OnBeatAnimation());
    }

    private IEnumerator OnBeatAnimation()
    {
        float time = 0.0f;
        while (time <= _onBeatAnimTime)
        {
            time += Time.deltaTime;
            float amt = _onBeatTransparency.Evaluate(time / _onBeatAnimTime);
            _onBeatImage.color = new Color(_onBeatImage.color.r, _onBeatImage.color.g, _onBeatImage.color.b, amt);
            yield return null;
        }

        _onBeatImage.color = new Color(_onBeatImage.color.r, _onBeatImage.color.g, _onBeatImage.color.b, 0.0f);
    }
}
