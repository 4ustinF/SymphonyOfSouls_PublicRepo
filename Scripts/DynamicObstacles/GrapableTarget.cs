using System.Collections.Generic;
using UnityEngine;

public class GrapableTarget : MonoBehaviour, IInteractable
{
    private enum EInternalState
    {
        Invalid = -1,
        Idle = 0,
        GrappleActivated = 1,
    }

    private EInternalState state = EInternalState.Invalid;
    private Vector3 _spawnPosition = Vector3.zero;
    private Vector3 _targetPosition = Vector3.zero;
    [SerializeField] private float _wanderingRadius = 5.0f;
    [SerializeField] private float _wanderingSpeed = 5.0f;
    [SerializeField] private List<RewardDef> _rewards = new List<RewardDef>();
    [SerializeField] private int _totalInteractionsRequired = 0;
    [SerializeField] private Material _targetOutlineMaterial = null;
    [SerializeField] private Color _targetColor = Color.green;
    private int _interactionsCounter = 0;

    [SerializeField] private Material _activeMaterial = null;
    [SerializeField] private Material _grapableActiveMaterial = null;
    [SerializeField] private AimAssist _aimAssist = null;
    [SerializeField] private GameObject _radialVFX = null;
    [SerializeField] private GameObject _onGrappableTargetActivatedVFX = null;

    [SerializeField] private MeshRenderer _meshRenderer = null;
    [SerializeField] private GameObject _grappleTarget = null;
    [SerializeField] private GrappablePointBreakAnimation _breakAnimationController = null;

    private IWwiseManager _wwiseManager = null;

    public int InteractionsRequired { get => _totalInteractionsRequired; set { } }

    public void Interact(out List<RewardDef> rewards)
    {
        ++_interactionsCounter;
        if (_interactionsCounter == InteractionsRequired)
        {
            rewards = _rewards;
            _meshRenderer.material = _grapableActiveMaterial;
            state = EInternalState.GrappleActivated;
            gameObject.layer = LayerMask.NameToLayer("Grappable");
            _grappleTarget.gameObject.SetActive(true);
            _onGrappableTargetActivatedVFX.gameObject.SetActive(true);
            _radialVFX.SetActive(false);
            _aimAssist.SetHighlightMatirial(_targetOutlineMaterial);
            if (_breakAnimationController)
            {
                _wwiseManager.PlayGrapplePointBreakSoundEffect();
                _breakAnimationController.Play();
            }
        }
        else
        {
            rewards = null;
        }
    }

    private void Awake()
    {
        GameLoader.CallOnComplete(() =>
        {
            Initialize();
        });
    }

    private void Initialize()
    {
        state = EInternalState.Idle;
        _spawnPosition = transform.position;
        _targetPosition = GetRandomTargetPosition();
        if (_meshRenderer == null)
        {
            _meshRenderer = GetComponent<MeshRenderer>();
        }
        _meshRenderer.material = _activeMaterial;
        _wwiseManager = ServiceLocator.Get<IWwiseManager>();
    }

    private void Update()
    {
        if (state == EInternalState.Idle)
        {
            if (Vector3.Distance(transform.position, _targetPosition) > 0.5f)
            {
                MoveToPosition(_targetPosition);
            }
            else
            {
                _targetPosition = GetRandomTargetPosition();
            }
        }
    }

    private Vector3 GetRandomTargetPosition()
    {
        return Random.insideUnitSphere * _wanderingRadius + _spawnPosition;
    }

    private void MoveToPosition(Vector3 desiredPosition)
    {
        Vector3 direction = (_targetPosition - transform.position).normalized;
        transform.position += direction * _wanderingSpeed * Time.deltaTime;
    }
}
