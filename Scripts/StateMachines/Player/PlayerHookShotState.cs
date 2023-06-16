using UnityEngine;

public class PlayerHookShotState : PlayerBaseState
{
    enum HookShotState
    {
        None,
        StartThrow,
        Throwing,
        StartMove,
        Moving
    }

    private HookShotState _hookShotState = HookShotState.None;
    private Vector3 _charVelMomentum = Vector3.zero;
    private float _hookShotSpeedMin = 10.0f;
    private float _hookShotSpeedMax = 50.0f;
    private float _hookShotSpeedMultiplier = 2.0f;
    private float _reachedHookShotPositionDistance = 1.0f;
    private float _momentumExtraSpeed = 5.0f;
    private float _jumpSpeed = 2.0f;
    private float _jumpYCap = 10.0f;
    private float _hookShotFOV = 100.0f;
    private float _hookshotSpeed = 0.0f;
    private RaycastHit _raycastHit = new RaycastHit();
    private Vector3 _targetPosition = Vector3.zero;
    private Transform _cachedTargetTransform = null;

    private readonly int _quality = 100;
    private readonly float _damper = 7.0f;
    private readonly float _strength = 800.0f;
    private readonly float _startVelocity = 10.0f;
    private float _velocity = 0.0f;
    private readonly float _waveCount = 3.0f;
    private readonly float _waveHeight = 6.0f;
    private readonly float _hookSpeed = 12.0f;

    public PlayerHookShotState(PlayerStateMachine stateMachine, RaycastHit raycastHit) : base(stateMachine)
    {
        _raycastHit = raycastHit;
    }

    private Vector3 _hookShotDirection = new Vector3();

    Vector3 point = Vector3.zero;
    Vector3 currentEndPosition = Vector3.zero;
    float value = 0.0f;

    public override void Enter()
    {
        stateMachine.MultiplierManager.ActionPerformed(Enums.PlayerActionType.HookShot);

        stateMachine.InputReader.JumpEvent += HookShotJump;
        stateMachine.InputReader.DashEvent += Dash;
        stateMachine.InputReader.DashEvent += TransitionToAirState;
        stateMachine.InputReader.SlideEvent += TransitionToAirState;
        stateMachine.InputReader.InteractEvent += Interact;

        stateMachine.PlayerVisualEffects.PlayHookshotAnimation();
        stateMachine.RigidBody.useGravity = false;
        _hookShotState = HookShotState.StartThrow;
        if (_raycastHit.collider.gameObject.TryGetComponent<AimAssist>(out AimAssist aimAssist))
        {
            _targetPosition = aimAssist.TargetPoint.position;
            _cachedTargetTransform = aimAssist.TargetPoint.transform;
        }
        else
        {
            _targetPosition = _raycastHit.point;
        }
        // _hookShotDirection = (_targetPosition - stateMachine.CameraHolder.position).normalized;

        stateMachine.HookShotLineRenderer.positionCount = _quality + 1;
        point = Vector3.zero;
        currentEndPosition = stateMachine.Orientation.transform.position;
        value = 0.0f;
        _velocity = _startVelocity;
    }

    public override void Exit()
    {
        stateMachine.InputReader.JumpEvent -= HookShotJump;
        stateMachine.InputReader.DashEvent -= Dash;
        stateMachine.InputReader.DashEvent -= TransitionToAirState;
        stateMachine.InputReader.SlideEvent -= TransitionToAirState;
        stateMachine.InputReader.InteractEvent -= Interact;

        AddMomentum();
        stateMachine.RigidBody.useGravity = true;
        stateMachine.PlayerCamera.CameraEffectsReset(1.0f);
        stateMachine.GrapleVFX.Stop();

        ServiceLocator.Get<IWwiseManager>().StopGrappleSoundEffect();

        stateMachine.HookShotLineRenderer.positionCount = 0;
    }

    private void AddMomentum()
    {
        //Vector3 hookShotDir = (_targetPosition - stateMachine.CameraHolder.position).normalized;
        Vector3 finalDir = (_hookShotDirection + stateMachine.CameraHolder.forward).normalized;
        _charVelMomentum = finalDir * _hookshotSpeed * _momentumExtraSpeed;
        _charVelMomentum += Vector3.up * _jumpSpeed;

        if (_charVelMomentum.y > _jumpYCap)
        {
            _charVelMomentum = new Vector3(_charVelMomentum.x, _jumpYCap, _charVelMomentum.z);
        }
        else if (_charVelMomentum.y < -_jumpYCap)
        {
            _charVelMomentum = new Vector3(_charVelMomentum.x, -_jumpYCap, _charVelMomentum.z);
        }

        stateMachine.RigidBody.AddForce(_charVelMomentum, ForceMode.Impulse);
    }

    public override void Tick(float deltaTime)
    {
        // Handles States
        switch (_hookShotState)
        {
            case HookShotState.StartThrow: StartHookShotThrow(); break;
            case HookShotState.Throwing: UpdateHookShotThrow(deltaTime); break;
            case HookShotState.StartMove: StartHookShotMove(); break;
            case HookShotState.Moving: UpdateHookShotMove(deltaTime); break;
        }

        _hookShotDirection = (_targetPosition - stateMachine.CameraHolder.position).normalized;
    }

    public override void FixedTick(float fixedDeltaTime)
    {
    }

    private void StartHookShotThrow()
    {
        _hookShotState = HookShotState.Throwing;
        ServiceLocator.Get<IWwiseManager>().PlayGrappleLaunchSoundEffect();
    }

    private void UpdateHookShotThrow(float deltaTime)
    {
        // Update velocity and value
        var direction = -value >= 0 ? 1f : -1f;
        var force = Mathf.Abs(-value) * _strength;
        _velocity += (force * direction - _velocity * _damper) * Time.deltaTime;
        value += _velocity * Time.deltaTime;
        currentEndPosition = Vector3.Lerp(currentEndPosition, _targetPosition, Time.deltaTime * _hookSpeed);

        for (var i = 0; i <= _quality; i++)
        {
            var delta = i / (float)_quality;
            float curveEvaluate = stateMachine.HookAffectCurve.Evaluate(delta);

            Vector3 up = Quaternion.LookRotation((_targetPosition - stateMachine.HookShotTransform.position).normalized) * Vector3.up;
            Vector3 offset = up * _waveHeight * Mathf.Sin(delta * _waveCount * Mathf.PI) * value * curveEvaluate;
            stateMachine.HookShotLineRenderer.SetPosition(i, Vector3.Lerp(stateMachine.HookShotTransform.position, currentEndPosition, delta) + offset);
        }

        if (Vector3.Distance(stateMachine.HookShotLineRenderer.GetPosition(_quality), _targetPosition) < _reachedHookShotPositionDistance)
        {
            _hookShotState = HookShotState.StartMove;
            //ServiceLocator.Get<IWwiseManager>().PlayGrappleRopeSoundEffect();
        }
    }

    private void StartHookShotMove()
    {
        stateMachine.PlayerCamera.DoFov(_hookShotFOV);
        stateMachine.GrapleVFX.Play();

        _hookShotState = HookShotState.Moving;
        ServiceLocator.Get<IWwiseManager>().PlayGrapplePullSoundEffect();
    }

    private void UpdateHookShotMove(float deltaTime)
    {
        Vector3 finalTargetPoint = _cachedTargetTransform != null ? _cachedTargetTransform.position : _targetPosition;

        _hookshotSpeed = Mathf.Clamp(Vector3.Distance(stateMachine.transform.position, finalTargetPoint), _hookShotSpeedMin, _hookShotSpeedMax);
        stateMachine.transform.position = Vector3.MoveTowards(stateMachine.transform.position, finalTargetPoint, _hookshotSpeed * _hookShotSpeedMultiplier * deltaTime);

        var direction = -value >= 0 ? 1f : -1f;
        var force = Mathf.Abs(-value) * _strength;
        _velocity += (force * direction - _velocity * _damper) * Time.deltaTime;
        value += _velocity * Time.deltaTime;

        for (var i = 0; i <= _quality; i++)
        {
            float delta = i / (float)_quality;

            Vector3 up = Quaternion.LookRotation((_targetPosition - stateMachine.HookShotTransform.position).normalized) * Vector3.up;
            Vector3 offset = up * value;
            stateMachine.HookShotLineRenderer.SetPosition(i, Vector3.Lerp(stateMachine.HookShotTransform.position, finalTargetPoint, delta) + offset);
        }

        if (Vector3.Distance(stateMachine.transform.position, finalTargetPoint) < _reachedHookShotPositionDistance)
        {
            // Reached hookshot position
            TransitionToAirState();
        }
    }

    private void HookShotJump()
    {
        if (_hookShotState != HookShotState.Moving) { return; }

        TransitionToAirState();
    }

    private void TransitionToAirState()
    {
        stateMachine.SwitchState(new PlayerAirState(stateMachine));
    }
}
