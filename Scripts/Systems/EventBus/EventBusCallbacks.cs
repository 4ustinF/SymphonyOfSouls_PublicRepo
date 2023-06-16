using System;
using UnityEngine;

public class EventBusCallbacks
{
    private IEventBusSystemHub _messengerHub;
    // You can also setup events to invoke when a message is handled.
    public event Action<SimpleTextMessage> SimpleTextMessageHandled = null;
    public event Action<OnKillBoxTrigger> OnKillBoxTriggerMessageHandled = null;
    public event Action<OnBeatInputMessage> OnBeatInputMessageHandled = null;
    public event Action<OnMusicBeatMessage> OnMusicBeatMessageHandled = null;
    public event Action<OnMusicBarMessage> OnMusicBarMessageHandled = null;
    public event Action<OnPlayerInputsChangedMessage> OnPlayerInputsChangedHandled = null;

    public void Initialize()
    {
        Debug.Log("EventButCallbacks SystemStart");
        _messengerHub = ServiceLocator.Get<IEventBusSystemHub>();
        if (_messengerHub == null)
        {
            Debug.LogWarning("MessengerHub Not Found As Registered System");
            return;
        }

        RegisterSubscribers();
    }

    private void RegisterSubscribers()
    {
        _messengerHub.Subscribe<SimpleTextMessage>(HandleSimpleTextMessage);
        _messengerHub.Subscribe<OnKillBoxTrigger>(HandleOnKillBoxTrigger);
        _messengerHub.Subscribe<OnBeatInputMessage>(HandleOnBeatInputMessage);
        _messengerHub.Subscribe<OnMusicBeatMessage>(HandleOnMusicBeatMessage);
        _messengerHub.Subscribe<OnMusicBarMessage>(HandleOnMusicBarMessage);
        _messengerHub.Subscribe<OnPlayerInputsChangedMessage>(HandleOnPlayerInputsChangedMessage);

    }

    private void HandleSimpleTextMessage(SimpleTextMessage obj)
    {
        Debug.Log($"Message Received: {obj.Message}");
        SimpleTextMessageHandled?.Invoke(obj);
    }

    private void HandleOnKillBoxTrigger(OnKillBoxTrigger obj)
    {
        OnKillBoxTriggerMessageHandled?.Invoke(obj);
    }

    private void HandleOnBeatInputMessage(OnBeatInputMessage obj)
    {
        OnBeatInputMessageHandled?.Invoke(obj);
    }

    private void HandleOnMusicBeatMessage(OnMusicBeatMessage obj)
    {
        OnMusicBeatMessageHandled?.Invoke(obj);
    }

    private void HandleOnMusicBarMessage(OnMusicBarMessage obj)
    {
        OnMusicBarMessageHandled?.Invoke(obj);
    }
    private void HandleOnPlayerInputsChangedMessage(OnPlayerInputsChangedMessage obj)
    {
        OnPlayerInputsChangedHandled?.Invoke(obj);
    }
}