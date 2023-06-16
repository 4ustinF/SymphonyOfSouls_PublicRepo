using System;
using System.Collections.Generic;
using UnityEngine;

public class StatueTarget : MonoBehaviour, IInteractable, IConditional
{
    [SerializeField] private List<RewardDef> _rewards = new List<RewardDef>();
    [SerializeField] private int _totalInteractionsRequired = 0;
    private int _interactionsCounter = 0;

    [SerializeField] private Material _inActiveMaterial = null;
    [SerializeField] private MeshRenderer _meshRenderer = null;

    private IWwiseManager _wwiseManager = null;

    private float _minPower = 0.75f;
    private float _maxPower = 10.0f;

    public int InteractionsRequired { get => _totalInteractionsRequired; set { } }
    public Action OnComplete { get; set; }

    private void Awake()
    {
        GameLoader.CallOnComplete(Initialize);
    }

    private void OnMouseEnter()
    {
        var material = _meshRenderer.materials[1];
        material?.SetFloat("_FresnelPower", _maxPower);
    }

    private void OnMouseExit()
    {
        var material = _meshRenderer.materials[1];
        material?.SetFloat("_FresnelPower", _minPower);
    }

    public void Initialize()
    {
        _wwiseManager = ServiceLocator.Get<IWwiseManager>();
    }

    public void Interact(out List<RewardDef> rewards)
    {
        ++_interactionsCounter;
        if (_interactionsCounter == InteractionsRequired)
        {
            rewards = _rewards;
            if (TryGetComponent<AimAssist>(out AimAssist aimAssist))
            {
                aimAssist.Sleep();
            }
            _meshRenderer.material = _inActiveMaterial;
            OnComplete?.Invoke();
            _wwiseManager.PlayStatueHitSoundEffect();
        }
        else
        {
            rewards = null;
        }
    }

    public bool IsTrue()
    {
        return _interactionsCounter >= InteractionsRequired;
    }
}
