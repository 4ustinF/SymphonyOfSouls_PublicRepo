using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicFire : MonoBehaviour
{
    [SerializeField] private MMF_Player _onBeatFeedback = null;
    [SerializeField] private MMF_Player _onBarFeedback = null;

    private int _beatCount = 0;

    private void Awake()
    {
        LevelLoader.CallOnComplete(Initialize);
    }

    private void OnDisable()
    {
        var bus = ServiceLocator.Get<EventBusCallbacks>();
        bus.OnMusicBeatMessageHandled -= OnBeat;
    }

    private void Initialize()
    {
        var bus = ServiceLocator.Get<EventBusCallbacks>();
        bus.OnMusicBeatMessageHandled += OnBeat;
    }

    public void OnBeat(OnMusicBeatMessage message)
    {
        ++_beatCount;
        if (_beatCount >= 4)
        {
            _beatCount = 0;
            BarRespond();
            return;
        }

        BeatRespond();
    }

    public void BeatRespond()
    {
        _onBeatFeedback.PlayFeedbacks();
    }

    public void BarRespond()
    {
        _onBarFeedback.PlayFeedbacks();
    }
}
