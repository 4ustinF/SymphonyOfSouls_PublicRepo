using MoreMountains.Feedbacks;
using System.Collections;
using UnityEngine;

public class PlayerVisualEffects : MonoBehaviour
{
    [SerializeField] private ParticleSystem _musicalNotesParticles = null;
    [SerializeField] private MMF_Player _wallRunFeedbackStart = null;
    [SerializeField] private MMF_Player _wallRunFeedbackExit = null;
    [SerializeField] private MMF_Player _onDieFeedback = null;
    [SerializeField] private MMF_Player _onInteractFeedback = null;
    [SerializeField] private MMF_Player _onDoor = null;

    [SerializeField] private Animator _handAnimator = null;
    private const string _attackAnimationTrigger = "Attack";
    private const string _grappleAnimationTrigger = "Grapple";

    [SerializeField] private Renderer _stringsRenderer = null;
    private Material _emmisiveStringsMat = null;
    private float _maxIntensity = 1.0f;
    private float _transitionDuration = 3f;
    private float _minIntensity = -3.0f;
    private float _elapsedTime;
    private Coroutine _emissionCoroutine;
    private bool _isIncreasingIntensity;
    private Color _startEmmisionColor = Color.white;
    private const string EMISSIVE_COLOR_NAME = "_EmissionColor";
    private const string EMISSIVE_KEYWORD = "_EMISSION";

    private void Awake()
    {
        _emmisiveStringsMat = _stringsRenderer.material;
        _startEmmisionColor = _emmisiveStringsMat.GetColor(EMISSIVE_COLOR_NAME);
        _emissionCoroutine = StartCoroutine(BlinkEmissionIntensity());
    }

    public void PlayMusicNotesVFX()
    {
        _onInteractFeedback.PlayFeedbacks();
        _musicalNotesParticles.Play();
    }

    public void PlayWallRunFeedback()
    {
        _wallRunFeedbackStart.PlayFeedbacks();
    }

    public void PlayWallExitFeedback()
    {
        _wallRunFeedbackExit.PlayFeedbacks();
    }

    public void PlayOnDieFeedback()
    {
        _onDieFeedback.PlayFeedbacks();
    }

    public void PlayInteractAnimation()
    {
        _handAnimator.Play(_attackAnimationTrigger);
    }

    public void PlayHookshotAnimation()
    {
        _handAnimator.Play(_grappleAnimationTrigger);
    }

    private void TurnOnStringsEmission()
    {
        _emmisiveStringsMat.EnableKeyword("_EMISSION");
    }

    private IEnumerator BlinkEmissionIntensity()
    {
        while (true)
        {
            float startIntensity = _isIncreasingIntensity ? _minIntensity : _maxIntensity;
            float targetIntensity = _isIncreasingIntensity ? _maxIntensity : _minIntensity;
            float duration = _transitionDuration;

            _elapsedTime = 0f;

            while (_elapsedTime < duration)
            {
                float currentIntensity = Mathf.Lerp(startIntensity, targetIntensity, _elapsedTime / duration);
               
                Color color = _startEmmisionColor;
                color = new Color(
                            color.r * Mathf.Pow(2, currentIntensity),
                            color.g * Mathf.Pow(2, currentIntensity),
                            color.b * Mathf.Pow(2, currentIntensity),
                            color.a);
                _emmisiveStringsMat.SetColor(EMISSIVE_COLOR_NAME, color);
                _elapsedTime += Time.deltaTime;
                yield return null;
            }

            _isIncreasingIntensity = !_isIncreasingIntensity;
        }
    }

    public void StopEmission()
    {
        if (_emissionCoroutine != null)
        {
            StopCoroutine(_emissionCoroutine);
            _emissionCoroutine = null;
        }
    }

    private void OnDestroy()
    {
        StopEmission();
        Destroy(_emmisiveStringsMat);
    }
}