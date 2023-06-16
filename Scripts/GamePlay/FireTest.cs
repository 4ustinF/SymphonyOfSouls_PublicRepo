using DG.Tweening;
using UnityEngine;

public class FireTest : MonoBehaviour
{
    [SerializeField] private bool _reactOnFirst = true;
    [SerializeField] private bool _reactOnSecond = true;
    [SerializeField] private bool _reactOnThird = true;
    [SerializeField] private Transform _fire1 = null;
    [SerializeField] private Transform _fire2 = null;
    private int _beatCount = 0;

    [SerializeField] private Vector3 _startingSize = Vector3.zero;
    [SerializeField] private Vector3 _beatScale = Vector3.zero;
    [SerializeField] private Ease _beatScaleEase = Ease.Linear;
    [SerializeField] private Vector3 _barScale = Vector3.zero;
    [SerializeField] private Ease _barScaleEase = Ease.Linear;

    private Sequence _barSequence = null;
    private Sequence _beatSequence = null;

    private void OnEnable()
    {
        LevelLoader.CallOnComplete(Initialize);
    }

    private void OnDestroy()
    {
        var bus = ServiceLocator.Get<EventBusCallbacks>();
        bus.OnMusicBarMessageHandled -= BarRespond;
        bus.OnMusicBeatMessageHandled -= BeatRespond;
        _beatSequence.Kill();
        _barSequence.Kill();
    }

    private void Initialize()
    {
        var bus = ServiceLocator.Get<EventBusCallbacks>();
        bus.OnMusicBarMessageHandled += BarRespond;
        bus.OnMusicBeatMessageHandled += BeatRespond;
    }

    public void BeatRespond(OnMusicBeatMessage message)
    {
        _beatCount++;
        if ((_beatCount == 1 && !_reactOnFirst) || (_beatCount == 2 && !_reactOnSecond) || (_beatCount == 3 && !_reactOnThird))
        {
            return;
        }

        _beatSequence = DOTween.Sequence();
        _beatSequence.Append(_fire1.transform.DOScale(_beatScale, 0.1f).SetEase(_beatScaleEase)).SetDelay(0.1f);
        _beatSequence.Append(_fire1.transform.DOScale(_startingSize, 0.1f).SetEase(_beatScaleEase)).SetDelay(0.1f);
        _beatSequence.Play();

        if (_beatCount == 4)
        {
            _beatCount = 0;
        }
    }

    public void BarRespond(OnMusicBarMessage message)
    {
        _barSequence = DOTween.Sequence();
        _fire2.gameObject.SetActive(true);
        _barSequence.Append(_fire2.transform.DOScale(_barScale, 0.2f).SetEase(_barScaleEase)).SetDelay(0.1f);
        _barSequence.Append(_fire2.transform.DOScale(_startingSize, 0.1f).SetEase(_barScaleEase)).SetDelay(0.1f);
        _barSequence.OnComplete(() => { _fire2.gameObject.SetActive(false); });
        _barSequence.Play();
        // Debug.Log("Do Bar");
    }
}
