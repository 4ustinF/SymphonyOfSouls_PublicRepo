using DG.Tweening;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] private Ease _ease = Ease.Linear;
    [SerializeField] private float _rotationTime = 0.0f;
    private Sequence mySequence = null;

    public void StartRotation()
    {
        if (mySequence == null)
        {
            mySequence = DOTween.Sequence();

            mySequence.Append(transform.DOLocalRotate(new Vector3(0.0f, 360.0f, 0.0f), _rotationTime, RotateMode.FastBeyond360).SetRelative(true).SetEase(_ease));
            mySequence.SetLoops(-1);
        }
    }

    public void StopRotation()
    {
        mySequence.Kill();
        mySequence = null;
    }
}
