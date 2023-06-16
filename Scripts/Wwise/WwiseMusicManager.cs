using UnityEngine;

public class WwiseMusicManager : MonoBehaviour
{
    // References
    [SerializeField] private MusicInformation _musicInformation = null;

    // Wwise Music Player related variables
    //[SerializeField] private GameEvent _onBeatEvent = null;
    //[SerializeField] private GameEvent _onBarEvent = null;
    private IEventBusSystemHub _messenger = null;

    // Wwise Music Callbacks related variables
    private Enums.SceneType _sceneType = Enums.SceneType.None;
    private uint _callbackType = (uint)AkCallbackType.AK_MusicSyncAll | (uint)AkCallbackType.AK_EnableGetMusicPlayPosition;
    private uint _musicEventID = uint.MaxValue;
    private bool _isPlaying = false;
    private bool _isPaused = false;
    private bool _durationsNotSet = true;
    private bool _metronomeDurationsNotSet = true;
    //private uint _callbackType = (uint)AkCallbackType.AK_MusicSyncBeat | (uint) AkCallbackType.AK_MusicSyncBar;

    public Enums.SceneType SceneType { set => _sceneType = value; }

    private void OnDestroy()
    {
        StopMusicEvent();
        _musicEventID = uint.MaxValue;
    }

    public void Initialize()
    {
        _messenger = ServiceLocator.Get<IEventBusSystemHub>();
    }

    public void PlayMusicEvent()
    {
        if (_isPlaying)
        {
            return;
        }

        _musicInformation.Initialize();

        switch (_sceneType)
        {
            case Enums.SceneType.MainMenu:
            case Enums.SceneType.LevelSelect:
                _musicEventID = AkSoundEngine.PostEvent("Play_Music_MainMenu", gameObject, _callbackType, PlayMusicEventCallback, null);
                _isPlaying = true;
                break;
            case Enums.SceneType.Level01:
                _musicEventID = AkSoundEngine.PostEvent("Play_Music_Level01", gameObject, _callbackType, PlayMusicEventCallback, null);
                _isPlaying = true;
                break;
            case Enums.SceneType.Level02:
                _musicEventID = AkSoundEngine.PostEvent("Play_Music_Level02", gameObject, _callbackType, PlayMusicEventCallback, null);
                _isPlaying = true;
                break;
        }
    }

    public void StopMusicEvent()
    {
        switch (_sceneType)
        {
            case Enums.SceneType.MainMenu:
            case Enums.SceneType.LevelSelect:
                _musicEventID = AkSoundEngine.PostEvent("Stop_Music_MainMenu", gameObject);
                break;
            case Enums.SceneType.Level01:
                _musicEventID = AkSoundEngine.PostEvent("Stop_Music_Level01", gameObject);
                break;
            case Enums.SceneType.Level02:
                _musicEventID = AkSoundEngine.PostEvent("Stop_Music_Level02", gameObject);
                break;
        }

        _isPlaying = false;
        _durationsNotSet = true;
    }

    public void PauseMusicEvent()
    {
        switch (_sceneType)
        {
            case Enums.SceneType.MainMenu:
                _musicEventID = AkSoundEngine.PostEvent("Pause_Music_MainMenu", gameObject);
                break;
            case Enums.SceneType.Level01:
                _musicEventID = AkSoundEngine.PostEvent("Pause_Music_Level01", gameObject);
                break;
            case Enums.SceneType.Level02:
                _musicEventID = AkSoundEngine.PostEvent("Pause_Music_Level02", gameObject);
                break;
        }

        _isPaused = true;
    }

    public void ResumeMusicEvent()
    {
        switch (_sceneType)
        {
            case Enums.SceneType.MainMenu:
                _musicEventID = AkSoundEngine.PostEvent("Resume_Music_MainMenu", gameObject, _callbackType, PlayMusicEventCallback, null);
                break;
            case Enums.SceneType.Level01:
                _musicEventID = AkSoundEngine.PostEvent("Resume_Music_Level01", gameObject, _callbackType, PlayMusicEventCallback, null);
                break;
            case Enums.SceneType.Level02:
                _musicEventID = AkSoundEngine.PostEvent("Resume_Music_Level02", gameObject, _callbackType, PlayMusicEventCallback, null);
                break;
        }

        _isPaused = false;
        _isPlaying = true;
    }

    public void StartAudioDelayOptimization()
    {
        if(!_isPaused)
        {
            PauseMusicEvent();
            _isPaused = true;
        }

        _musicEventID = AkSoundEngine.PostEvent("Play_Menu_Metronome", gameObject, _callbackType, PlayMetronomeEventCallback, null);
    }

    public void StopAudioDelayOptimization()
    {
        _musicEventID = AkSoundEngine.PostEvent("Stop_Menu_Metronome", gameObject);
    }

    private void PlayMusicEventCallback(object in_cookie, AkCallbackType in_type, object in_info)
    {
        if (in_info is AkMusicSyncCallbackInfo)
        {
            AkMusicSyncCallbackInfo musicInfo = (AkMusicSyncCallbackInfo)in_info;

            // This if statement is called every time MusicSyncBeat is triggered via wwise
            if (in_type == AkCallbackType.AK_MusicSyncBeat)
            {
                _musicInformation.UpdateCurrentBeatTime();
                //_onBeatEvent?.Invoke();
                _messenger.Publish(new OnMusicBeatMessage(this));
            }

            // This if statement is called every time MusicSyncBar is triggered via Wwise
            if (in_type == AkCallbackType.AK_MusicSyncBar)
            {
                //_musicInformation.UpdatePosition((float)musicInfo.segmentInfo_iCurrentPosition / 1000f);
                //_onBarEvent?.Invoke();
                _messenger.Publish(new OnMusicBarMessage(this));

                if (_durationsNotSet)
                {
                    _musicInformation.BarDuration = musicInfo.segmentInfo_fBarDuration;
                    Debug.Log($"Bar duration: {_musicInformation.BarDuration}");
                    _musicInformation.GridDuration = musicInfo.segmentInfo_fGridDuration;
                    Debug.Log($"Grid duration: {_musicInformation.GridDuration}");
                    _musicInformation.BeatDuration = musicInfo.segmentInfo_fBeatDuration;
                    Debug.Log($"Beat duration: {_musicInformation.BeatDuration}");
                    _durationsNotSet = false;
                }
            }
        }
    }

    private void PlayMetronomeEventCallback(object in_cookie, AkCallbackType in_type, object in_info)
    {
        if (in_info is AkMusicSyncCallbackInfo)
        {
            AkMusicSyncCallbackInfo musicInfo = (AkMusicSyncCallbackInfo)in_info;

            // This if statement is called every time MusicSyncBeat is triggered via wwise
            if (in_type == AkCallbackType.AK_MusicSyncBeat)
            {
                //_onBeatEvent?.Invoke();
                _messenger.Publish(new OnMusicBeatMessage(this));

            }

            // This if statement is called every time MusicSyncBar is triggered via Wwise
            if (in_type == AkCallbackType.AK_MusicSyncBar)
            {
                //_onBarEvent?.Invoke();
                _messenger.Publish(new OnMusicBarMessage(this));

                if (_metronomeDurationsNotSet)
                {
                    _musicInformation.MetronomeBarDuration = musicInfo.segmentInfo_fBarDuration;
                    _musicInformation.MetronomeGridDuration = musicInfo.segmentInfo_fGridDuration;
                    _musicInformation.MetronomeBeatDuration = musicInfo.segmentInfo_fBeatDuration;
                    _metronomeDurationsNotSet = false;

                    Debug.Log($"Metronome Bar duration: {_musicInformation.BarDuration}");
                    Debug.Log($"Metronome Grid duration: {_musicInformation.GridDuration}");
                    Debug.Log($"Metronome Beat duration: {_musicInformation.BeatDuration}");
                }
            }
        }
    }
}