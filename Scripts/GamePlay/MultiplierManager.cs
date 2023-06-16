using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplierManager : MonoBehaviour
{
    [SerializeField] private float _idleMovementMultiplier = 1.5f; // 3.0f
    [SerializeField] private float _runningMovementMultiplier = 1.25f; // 2.0f
    [SerializeField] private float _slidingMovementMultiplier = 1.0f;
    [SerializeField] private List<Tier> _tiers = new List<Tier>();
    [SerializeField] private LevelProgressDef _levelProgress = null;
    [SerializeField] private MusicInformation _musicInformation = null;

    private float _decayValue = 0.0f;
    private bool _isSafetyNetActive = false;
    private bool _isDecaying = true;
    private readonly float _maxProgress = 1.0f;

    private readonly float _decayPerSecond = 0.05f;
    private readonly float _decayFrequency = 0.1f;

    private readonly float _dashBonus = 0.3f;
    private readonly float _slideBonus = 0.3f;
    private readonly float _interactBonus = 0.3f;
    private readonly float _hookShotBonus = 0.3f;
    private readonly float _wallRunBonus = 0.3f;

    private int _numOfBeats = 0;
    private Tier _currentTier = null;
    private PlayerStateMachine _playerRef = null;
    private Coroutine _bonusCorutine = null;
    private IWwiseManager _wwiseManager = null;

    public void Initialize(PlayerStateMachine player)
    {
        _playerRef = player;
        _currentTier = GetTier(Enums.MultiplierTier.Tier1);
        _decayValue = 0.0f;
        _levelProgress.currentDecayValue = 0.0f;
        _numOfBeats = 0;
         _isDecaying = true;
        UpdateActualMultiplier();
        _bonusCorutine = StartCoroutine(DecayRoutine());
        ServiceLocator.Get<EventBusCallbacks>().OnKillBoxTriggerMessageHandled += PlayerKilledHandle;
        _wwiseManager = ServiceLocator.Get<IWwiseManager>();
        _wwiseManager.ModifyMusicIntensity(_currentTier);
    }

    public void SetDecayingStatus(bool newStatus)
    {
        if (_isSafetyNetActive)
        {
            return;
        }

        _isDecaying = newStatus;
    }

    private void PlayerKilledHandle(OnKillBoxTrigger obj)
    {
        _levelProgress.currentDecayValue = 0.0f;
        //TierDowngrade();
    }

    private IEnumerator DecayRoutine()
    {
        while (true)
        {
            if (_isDecaying)
            {
                ApplyDecay();
            }
            yield return new WaitForSeconds(_decayFrequency);
        }
    }

    private void ApplyDecay()
    {
        if (LevelController.IsPaused)
        {
            return;
        }

        float movementMultiplier = GetMovementMultiplier();

        _decayValue = (_decayPerSecond * _decayFrequency) * _currentTier.TierMultiplier * movementMultiplier;
        _levelProgress.currentDecayValue -= _decayValue;

        // Check safety net
        if (_levelProgress.currentDecayValue <= 0.0f)
        {
            StartCoroutine(SafetyNetRoutine());
        }
    }

    private IEnumerator SafetyNetRoutine()
    {
        _isSafetyNetActive = true;
        _isDecaying = false;

        _numOfBeats = 0;
        while (_numOfBeats < 4 && _levelProgress.currentDecayValue <= 0.0f)
        {
            yield return null;
        }

        if (_levelProgress.currentDecayValue <= 0.0f)
        {
            _levelProgress.currentDecayValue = 0.0f;
            // TierDowngrade();
        }

        _isDecaying = true;
        _isSafetyNetActive = false;
    }

    public void IncreaseBeatCounter()
    {
        if (_isDecaying)
        {
            return;
        }

        Mathf.Clamp(++_numOfBeats, 0, 4);
    }

    public void StartWallRun()
    {
       _bonusCorutine = StartCoroutine(BonusRoutine());
    }

    public void StoptWallRun()
    {
        StopCoroutine(_bonusCorutine);
    }

    public void ActionPerformed(Enums.PlayerActionType actionType)
    {
        if (_musicInformation.ProcessMusicSynchronizationInput() == false)
        {
            return;
        }

        float amt = 0.02f;

        switch (actionType)
        {
            case Enums.PlayerActionType.Dash:
                AddDecayScore(_dashBonus - ((float)(_currentTier.TierType) * amt)); // = 0.3f - (t * 0.05f)
                break;
            case Enums.PlayerActionType.Slide:
                AddDecayScore(_slideBonus - ((float)(_currentTier.TierType) * amt));
                break;
            case Enums.PlayerActionType.Interact:
                _playerRef.PlayerVisualEffects.PlayMusicNotesVFX();
                AddDecayScore(_interactBonus - ((float)(_currentTier.TierType) * amt));
                // Test where to position this particular effect
                _playerRef.WwiseManager.PlayAttackSoundEfect();
                break;
            case Enums.PlayerActionType.HookShot:
                AddDecayScore(_hookShotBonus - ((float)(_currentTier.TierType) * amt));
                break;
            default:
                Debug.LogError("Wrong Action Type registered!");
                break;
        }
    }

    private void AddDecayScore(float amount)
    {
        _levelProgress.currentDecayValue += amount;

        // Tier upgrade
        if (_levelProgress.currentDecayValue >= _maxProgress)
        {
            TierUpgrade();
        }
    }

    private void TierUpgrade()
    {
        if (_currentTier.TierType == Enums.MultiplierTier.Tier5)
        {
            _levelProgress.currentDecayValue = _maxProgress;
            return;
        }

        LevelUpTier();
        _levelProgress.currentDecayValue = Mathf.Abs(_maxProgress - _levelProgress.currentDecayValue);
        UpdateActualMultiplier();
    }

    private void TierDowngrade()
    {
        if (_currentTier.TierType == Enums.MultiplierTier.Tier1)
        {
            _levelProgress.currentDecayValue = 0.0f;
            return;
        }

        LevelDownTier();
        _levelProgress.currentDecayValue = Mathf.Abs(_maxProgress + _levelProgress.currentDecayValue);
        UpdateActualMultiplier();
        _levelProgress.currentDecayValue = 1.0f;
    }

    private Tier GetTier(Enums.MultiplierTier tier)
    {
        return _tiers.Find(x => x.TierType == tier);
    }

    private void LevelUpTier()
    {
        int lvl = (int)_currentTier.TierType;
        _currentTier = GetTier((Enums.MultiplierTier)(lvl + 1));
        _wwiseManager.ModifyMusicIntensity(_currentTier);
    }

    private void LevelDownTier()
    {
        int lvl = (int)_currentTier.TierType;
        _currentTier = GetTier((Enums.MultiplierTier)(lvl - 1));
        _wwiseManager.ModifyMusicIntensity(_currentTier);
    }

    private void UpdateActualMultiplier()
    {
        _levelProgress.Multiplier.SetValue(_currentTier.OverTimeMultiplier);
    }

    private float GetMovementMultiplier()
    {
        switch (_playerRef?.CurrentMovementBehaviour)
        {
            case MovementBehaviour.Idle:
                return _idleMovementMultiplier;
            case MovementBehaviour.Running:
                return _runningMovementMultiplier;
            case MovementBehaviour.Sliding:
                return _slidingMovementMultiplier;
        }

        return _idleMovementMultiplier;
    }

    private IEnumerator BonusRoutine()
    {
        while (true)
        {
            AddDecayScore(_wallRunBonus * _decayFrequency);
            yield return new WaitForSeconds(_decayFrequency);
        }
    }
}
