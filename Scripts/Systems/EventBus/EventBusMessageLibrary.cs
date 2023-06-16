public class SimpleTextMessage : IEventBusMessage
{
    public string Message { get; set; }
    public object Sender { get; set; }
    public SimpleTextMessage(object sender, string message)
    {
        Sender = sender;
        Message = message;
    }
}

public class OnKillBoxTrigger : IEventBusMessage
{
    public object Sender { get; set; }
    public OnKillBoxTrigger(object sender)
    {
        Sender = sender;
    }
}

public class OnBeatInputMessage : IEventBusMessage
{
    public object Sender { get; set; }
    public bool IsOnTime => _isOnTime;
    private bool _isOnTime = false;

    public OnBeatInputMessage(object sender, bool isOnTimeParam)
    {
        _isOnTime = isOnTimeParam;
        Sender = sender;
    }
}

public class OnMusicBeatMessage : IEventBusMessage
{
    public object Sender { get; set; }
    public OnMusicBeatMessage(object sender)
    {
        Sender = sender;
    }
}

public class OnMusicBarMessage : IEventBusMessage
{
    public object Sender { get; set; }
    public OnMusicBarMessage(object sender)
    {
        Sender = sender;
    }
}

public class OnPlayerInputsChangedMessage : IEventBusMessage
{
    public object Sender { get; set; }
    public Controls NewControls { get; set; }
    public OnPlayerInputsChangedMessage(object sender, Controls controls)
    {
        Sender = sender;
        NewControls = controls;
    }
}