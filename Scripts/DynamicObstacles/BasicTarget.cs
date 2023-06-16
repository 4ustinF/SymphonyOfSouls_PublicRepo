using System.Collections.Generic;
using UnityEngine;

public class BasicTarget : MonoBehaviour, IInteractable
{
    private Vector3 _spawnPosition = Vector3.zero;
    private Vector3 _targetPosition = Vector3.zero;
    [SerializeField] private float _wanderingRadius = 5.0f;
    [SerializeField] private float _wanderingSpeed = 5.0f;
    [SerializeField] private List<RewardDef> _rewards = new List<RewardDef>();
    [SerializeField] private int _totalInteractionsRequired = 0;
    [SerializeField] private ParticleSystem _explosionVFX  = null;
    [SerializeField] private GameObject _meshHolder = null;

    private int _interactionsCounter = 0;
    private BoxCollider _boxCollider = null;
    private IWwiseManager _wwiseManager = null;

    public int InteractionsRequired { get => _totalInteractionsRequired; set { } }

    public void Interact(out List<RewardDef> rewards)
    {
        ++_interactionsCounter;

        if (_interactionsCounter == InteractionsRequired)
        {
            rewards = _rewards;
            _wwiseManager.PlayVaseBreakingSoundEffect();
            HandleVisualEffects();
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

    public void Initialize()
    {
        _spawnPosition = transform.position;
        _targetPosition = GetRandomTargetPosition();
        TryGetComponent<BoxCollider>(out _boxCollider);
        _wwiseManager = ServiceLocator.Get<IWwiseManager>();
    }    

    private void Update()
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

    private Vector3 GetRandomTargetPosition()
    {
        return new Vector3(Random.Range(-_wanderingRadius, _wanderingRadius), 0, Random.Range(-_wanderingRadius, _wanderingRadius)) + _spawnPosition;
    }

    private void MoveToPosition(Vector3 desiredPosition)
    {
        Vector3 direction = (_targetPosition - transform.position).normalized;
        transform.position += direction * _wanderingSpeed * Time.deltaTime;
    }

    private void HandleVisualEffects()
    {

        if (TryGetComponent<AimAssist>(out AimAssist aimAssist))
        {
            aimAssist.Sleep();
        }

        Destroy(_meshHolder, 0.0f);
        _explosionVFX.Play();
        Destroy(this.gameObject, 1.0f);
        _boxCollider.enabled = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.gray;
        Gizmos.DrawCube(_targetPosition, Vector3.one);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(_spawnPosition, _wanderingRadius);
    }
}