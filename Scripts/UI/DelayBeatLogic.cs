using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DelayBeatLogic : MonoBehaviour
{
    [SerializeField] private MusicInformation _musicInformation = null;
    [SerializeField] private TMP_Text _delayText = null;
    [SerializeField] private Button _incrementDelayButton = null;
    [SerializeField] private Button _decrementDelayButton = null;
    [SerializeField] private Button _startButton = null;
    [SerializeField] private Image[] _beatImages = new Image[4];
    [SerializeField] private MMF_Player _onBeatFeedback = null;
    private WwiseManager _wwiseManager = null;

    private float _delayTime = 0.0f;
    readonly private int _incrementAmount = 1;
    readonly private int _decrementAmount = -1;
    readonly private float _maxDelayAmount = 200.0f;
    private bool _inCountDown = false;
    private bool _inAttempt = false;
    private int _metronomeIndex = 0;
    readonly private int _metronomeCount = 15;
    private int _imageIndex = 0;
    private List<float> _totalAverages = new List<float>();
    private Color _stringColor = Color.grey;
    private float tempDelayTime = 0.0f;

    private void OnEnable()
    {
        LevelLoader.CallOnComplete(Initialize);
    }

    private void OnDisable()
    {
        Disable();

        var bus = ServiceLocator.Get<EventBusCallbacks>();
        bus.OnMusicBeatMessageHandled -= IlluminateBeatSyncImage;
        bus.OnMusicBeatMessageHandled -= IncrementMetronomeCount;
    }

    private void Update()
    {
        if (_inAttempt && Input.anyKeyDown)
        {
            SyncInputDelay();
        }
    }

    private void Initialize()
    {
        _incrementDelayButton.onClick.AddListener(delegate { ModifyDelay(_incrementAmount); });
        _decrementDelayButton.onClick.AddListener(delegate { ModifyDelay(_decrementAmount); });
        _startButton.onClick.AddListener(StartAttempt);
        _wwiseManager = ServiceLocator.Get<IWwiseManager>() as WwiseManager;

        _totalAverages.Clear();

        ResetVaribles();
        _delayTime = _musicInformation.DelayThreshold.GetValue;
        tempDelayTime = _delayTime;
        UpdateDelayTimeText();

        var bus = ServiceLocator.Get<EventBusCallbacks>();
        bus.OnMusicBeatMessageHandled += IlluminateBeatSyncImage;
        bus.OnMusicBeatMessageHandled += IncrementMetronomeCount;
    }

    private void StartAttempt()
    {
        // TODO: We can instead make this kill the StartCountdown / metronome and restart on click
        if (_inCountDown || _inAttempt)
        {
            return; 
        }

        Debug.Log("StartAttempt");
        _wwiseManager.StartAudioDelayOptimization();

        // Countdown
        StartCoroutine(StartCountdown());

        _delayText.text = "(0)";
    }

    private IEnumerator StartCountdown()
    {
        _inCountDown = true;
        ResetBeatImageColors();
        int countDown = 0;

        while(countDown < 4)
        {
            switch(countDown)
            {
                case 0: _beatImages[0].color = Color.red;
                    break;
                case 1: _beatImages[1].color = Color.magenta;
                    break;
                case 2: _beatImages[2].color = Color.yellow;
                    break;
                case 3: _beatImages[3].color = Color.green;
                    break;
            }
            countDown++;

            //yield return new WaitForSeconds(_musicInformation.MetronomeBeatDuration);
            yield return new WaitForSeconds(0.6f);
        }

        ResetBeatImageColors();

        _beatImages[0].color = Color.green;
        _beatImages[1].color = _stringColor;
        _beatImages[2].color = _stringColor;
        _beatImages[3].color = _stringColor;

        _inCountDown = false;
        _inAttempt = true;
    }

    // Increment Metronome index on beat event
    public void IncrementMetronomeCount(OnMusicBeatMessage message)
    {
        if(!_inAttempt)
        {
            return;
        }

        _metronomeIndex++;

        if (_metronomeIndex > _metronomeCount)
        {
            _wwiseManager.StopAudioDelayOptimization();
            ResetVaribles();

            if(_totalAverages.Count > 0)
            {
                float totalAverage = 0.0f;
                foreach (var average in _totalAverages)
                {
                    totalAverage += average;
                }
                totalAverage /= _totalAverages.Count;
                _delayTime = Mathf.Floor(totalAverage + 0.5f);
                _totalAverages.Clear();
            }

            UpdateDelayTimeText();

            // Save delay time
            _musicInformation.DelayThreshold.SetValue(_delayTime);
            _wwiseManager.ResumeMusicEvent();
        }
    }

    public void IlluminateBeatSyncImage(OnMusicBeatMessage message)
    {
        if(!_inAttempt)
        {
            return;
        }

        _beatImages[0].color = _stringColor;
        _beatImages[1].color = _stringColor;
        _beatImages[2].color = _stringColor;
        _beatImages[3].color = _stringColor;

        ++_imageIndex;

        if (_imageIndex > 3)
        {
            _imageIndex = 0;
        }

        _beatImages[_imageIndex].color = Color.green;
    }

    private void ClampDelayTime()
    {
        _delayTime = Mathf.Clamp(_delayTime, 0.0f, _maxDelayAmount);
    }

    private void UpdateDelayTimeText()
    {
        ClampDelayTime();
        _delayText.text = $"{_delayTime} ms";
    }

    private void SyncInputDelay()
    {
        _onBeatFeedback.PlayFeedbacks();
        float timeDiff = _musicInformation.ReturnInputTimeDifference();
        tempDelayTime = Mathf.Floor(((tempDelayTime + timeDiff) * 0.5f) + 0.5f);
        tempDelayTime = Mathf.Clamp(tempDelayTime, 0.0f, _maxDelayAmount);

        _totalAverages.Add(tempDelayTime);
    }

    private void ModifyDelay(int time)
    {
        if(_inAttempt || _inCountDown)
        {
            return;
        }

        _delayTime += time;
        UpdateDelayTimeText();
    }

    private void ResetVaribles()
    {
        _inCountDown = false;
        _inAttempt = false;
        _metronomeIndex = 0;
        _imageIndex = 0;

        ResetBeatImageColors();
    }

    private void ResetBeatImageColors()
    {
        _beatImages[0].color = _stringColor;
        _beatImages[1].color = _stringColor;
        _beatImages[2].color = _stringColor;
        _beatImages[3].color = _stringColor;
    }

    private void BackButton()
    {
        Disable();
    }

    private void Disable()
    {
        _wwiseManager.StopAudioDelayOptimization();
        _incrementDelayButton.onClick.RemoveAllListeners();
        _decrementDelayButton.onClick.RemoveAllListeners();
        _startButton.onClick.RemoveListener(StartAttempt);

        // Save delay time
        _musicInformation.DelayThreshold.SetValue(_delayTime);
        _wwiseManager.ResumeMusicEvent();

        ResetVaribles();
    }
}