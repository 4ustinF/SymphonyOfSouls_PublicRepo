using UnityEngine;
using DG.Tweening;
using System.Collections;
using MoreMountains.Feedbacks;

public class PlayerCamera : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _orientation = null;
    [SerializeField] private Transform _camHolder = null;
    [SerializeField] private Camera _cam = null;
    [SerializeField] private InputReader _inputReader = null;
    [SerializeField] private MMWiggle _wiggle = null;

    [Header("Varibles")]
    [SerializeField] private FloatValue _sensX = null;
    [SerializeField] private FloatValue _sensY = null;
    private float _startFov = 0.0f;
    bool _isChangingFOV = false;

    private float _xRotation = 0.0f;
    private float _yRotation = 0.0f;

    private void Start()
    {
        _startFov = _cam.fieldOfView;
    }

    private void Update()
    {
        var lookDeltaRaw = _inputReader.LookValue;
        
        // Account for scaling applied directly in Windows code by old input system.
        lookDeltaRaw *= 0.5f;

        // Account for sensitivity setting on old Mouse X and Y axes.
        lookDeltaRaw *= 0.1f;

        _yRotation += lookDeltaRaw.x * _sensX.GetValue;
        _xRotation -= lookDeltaRaw.y * _sensY.GetValue;

        _xRotation = Mathf.Clamp(_xRotation, -90.0f, 90.0f);

        // Rotate Camera and orientation
        _camHolder.rotation = Quaternion.Euler(_xRotation, _yRotation, 0.0f);
        _orientation.rotation = Quaternion.Euler(0.0f, _yRotation, 0.0f);
    }

    public void DoFov(float endValue, float delay = 0.0f)
    {
        //_cam.DOFieldOfView(endValue, 0.25f);

        if(_isChangingFOV == true)
        {
            return;
        }

        if (delay < float.Epsilon)
        {
            _cam.DOFieldOfView(endValue, 0.25f);
        }
        else
        {
            StartCoroutine(FOVEnumerator(endValue, delay));
        }
    }

    private IEnumerator FOVEnumerator(float endValue, float waitTime)
    {
        _isChangingFOV = true;
        waitTime = Mathf.Abs(waitTime);

        yield return new WaitForSeconds(waitTime);
        _cam.DOFieldOfView(endValue, 0.25f);

        yield return new WaitForSeconds(0.25f);
        UnlockFOV();
    }

    void UnlockFOV()
    {
        _isChangingFOV = false;
    }

    public void DoTilt(float zTilt)
    {
        transform.DOLocalRotate(new Vector3(0.0f, 0.0f, zTilt), 0.25f);
    }

    public void CameraEffectsReset(float delay = 0.0f)
    {
        DoFov(_startFov, delay);
        DoTilt(0.0f);
    }

    public void AdjustRotation(Quaternion newRoatation)
    {
        _xRotation = newRoatation.eulerAngles.x;
        _yRotation = newRoatation.eulerAngles.y;
        _camHolder.rotation = Quaternion.Euler(_xRotation, _yRotation, 0.0f);
    }

    public void EnableDisableWiggle(bool enable)
    {
        _wiggle.enabled = enable;
    }
}
