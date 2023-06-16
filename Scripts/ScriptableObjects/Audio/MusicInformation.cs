using System;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "MusicInformation", menuName = "ScriptableObjects/Music/MusicInformation", order = 0)]
public class MusicInformation : ScriptableObject
{
    // Music Events Information
    [SerializeField] private DateTime _nextBeatTime = DateTime.Now;
    [SerializeField] private DateTime _currentBeatTime = DateTime.Now;
    [SerializeField] private float _barDuration = float.MinValue;
    [SerializeField] private float _barPosition = float.MinValue;
    [SerializeField] private float _gridDuration = float.MinValue;
    [SerializeField] private float _beatDuration = float.MinValue;
    [SerializeField] private float _currentPlayTime = float.MinValue;
    [SerializeField] private int _currentBeatTimeInt = int.MinValue;
    [SerializeField] private int _nextBeatTimeInt = int.MinValue;
    [SerializeField] private int _threshold = int.MinValue;
    [SerializeField] private FloatValue _delayThreshold = null;

    // Metronome Event Information
    private float _metronomeBarDuration = float.MinValue;
    private float _metronomeBarPosition = float.MinValue;
    private float _metronomeGridDuration = float.MinValue;
    private float _metronomeBeatDuration = float.MinValue;
    private float _currentMetronomePlayTime = float.MinValue;

    // Local Variables
    private WwiseManager _wwiseManager = null;
    private IEventBusSystemHub _messenger = null;
    private bool _isCheckingInput = false;
    private bool _wasOnTime = false;

    // Music Event Getters
    public DateTime CurrentBeatTime { get => _currentBeatTime; }
    public float BarDuration { get => _barDuration; set => _barDuration = value; }
    public float BeatDuration { get => _beatDuration; set => _beatDuration = value; }
    public float GridDuration { get => _gridDuration; set => _gridDuration = value; }
    public float BarPosition { get => _barPosition; }
    public float CurrentPlayTime { get => _currentPlayTime; }
    public float Threshold { get => _threshold; }
    public FloatValue DelayThreshold { get => _delayThreshold; set => _delayThreshold = value; }

    // Metronome Event Getters
    public float MetronomeBarDuration { get => _metronomeBarDuration; set => _metronomeBarDuration = value; }
    public float MetronomeBeatDuration { get => _metronomeBeatDuration; set => _metronomeBeatDuration = value; }
    public float MetronomeGridDuration { get => _metronomeGridDuration; set => _metronomeGridDuration = value; }
    public float MetronomeBarPosition { get => _metronomeBarPosition; }
    public float MetronomeCurrentPlayTime { get => _currentMetronomePlayTime; }

    public void Initialize()
    {
        _barPosition = 0.0f;
        _metronomeBarPosition = 0.0f;
        _currentPlayTime = 0.0f;
        _currentMetronomePlayTime = 0.0f;
        _isCheckingInput = false;
        _wasOnTime = false;
        _messenger = ServiceLocator.Get<IEventBusSystemHub>();
        _wwiseManager = ServiceLocator.Get<IWwiseManager>() as WwiseManager;
#if DEBUG_INPUT_TEST
        _wwiseManager.StartCoroutine(InputSpamTester());
#endif
    }

    public void UpdatePosition(float time)
    {
        _barPosition = time / _barDuration;
        _currentPlayTime = _barPosition;
    }

    public void UpdateMetronomePosition(float time)
    {
        _metronomeBarPosition = time / _metronomeBarDuration;
        _currentMetronomePlayTime = _metronomeBarPosition;
    }

    public void UpdateCurrentBeatTime()
    {
        _currentBeatTime = DateTime.Now;
        _currentBeatTimeInt = _currentBeatTime.Millisecond;
        _nextBeatTime = _currentBeatTime.AddSeconds(_beatDuration);
        _nextBeatTimeInt = _nextBeatTime.Millisecond;
    }

    private bool CheckIfInputIsCloseToBeat()
    {
        //int precision = _threshold;
        int precision = _threshold + (int)(DelayThreshold.GetValue + 0.5f);
        int inputTime = DateTime.Now.Millisecond;
        int previousBeatThreshold = (_currentBeatTimeInt + precision);
        int nextBeatThreshold = (_nextBeatTimeInt - precision);
        int deltaLast = inputTime - previousBeatThreshold;
        int deltaNext = nextBeatThreshold - inputTime;

        return (deltaLast <= 0 || deltaNext <= 0) ? true : false;
    }

    public float ReturnInputTimeDifference()
    {
        //int precision = _threshold;
        int precision = _threshold + (int)(DelayThreshold.GetValue + 0.5f);
        int inputTime = DateTime.Now.Millisecond;
        int previousBeatThreshold = (_currentBeatTimeInt + precision);
        int nextBeatThreshold = (_nextBeatTimeInt - precision);
        int deltaLast = inputTime - previousBeatThreshold;
        int deltaNext = nextBeatThreshold - inputTime;

        return Mathf.Min(Mathf.Abs(deltaLast), Mathf.Abs(deltaNext));
    }

    public bool ProcessMusicSynchronizationInput()
    {
        if (_isCheckingInput)
        {
            Debug.Log($"<color=magenta> Stop ABUSING the input!</color>");
            return false;
        }

        _wasOnTime = CheckIfInputIsCloseToBeat();
        _wwiseManager.StartCoroutine(PreventInputSpamCoroutine());

        if (_wasOnTime)
        {
            // Add reward logic here
            Debug.Log($"<color=green> You WERE on the beat! </color>");
            _messenger.Publish(new OnBeatInputMessage(this, true));
        }
        else if (!_wasOnTime)
        {
            // Add puishment logic here
            Debug.Log($"<color=red> You were NOT on the beat! </color>");
            _messenger.Publish(new OnBeatInputMessage(this, false));
        }
        return _wasOnTime;
    }

    private IEnumerator PreventInputSpamCoroutine()
    {
        _isCheckingInput = true;
        yield return new WaitForSeconds(_beatDuration);
        _isCheckingInput = false;
    }

    private IEnumerator InputSpamTester()
    {
        while (true)
        {
            ProcessMusicSynchronizationInput();
            yield return null;
        }
    }
}