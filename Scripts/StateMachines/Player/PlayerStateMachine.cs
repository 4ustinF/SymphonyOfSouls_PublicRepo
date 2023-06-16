using System;
using System.Collections;
using UnityEngine;

public class PlayerStateMachine : StateMachine
{
    public Action onDeath = null;

    [SerializeField] private string _currentState = string.Empty;
    private Vector3 _spawnPosition = Vector3.zero;

    //[Header("References")]
    [field: SerializeField] public InputReader InputReader { get; private set; }
    [field: SerializeField] public RewardReciverDef RewardReciver { get; private set; }
    [field: SerializeField] public Rigidbody RigidBody { get; private set; }
    [field: SerializeField] public Transform Orientation { get; private set; }
    [field: SerializeField] public Transform CameraHolder { get; private set; }
    [field: SerializeField] public PlayerCamera PlayerCamera { get; private set; }
    [field: SerializeField] public Transform LedgeProtectionSphereCenter { get; private set; }
    [field: SerializeField] public Transform OverHeadSphereCenter { get; private set; }
    [field: SerializeField] public GameObject BouncyCoat { get; private set; }
    [field: SerializeField] public PlayerVisualEffects PlayerVisualEffects { get; private set; }
    public IWwiseManager WwiseManager { get; private set; }

    [field: SerializeField] public LayerMask GameLayers { get; private set; }
    [field: SerializeField] public LayerMask InteractableLayers { get; private set; }
    private MultiplierManager _multiplierManager = null;

    [Header("Ground Check")]
    [SerializeField] private float _playerHeight = 2.0f;
    [SerializeField] private LayerMask _groundLayer = 0;

    [Header("Wall Detection")]
    [SerializeField] private float _wallCheckDistance = 0.0f;
    [SerializeField] private LayerMask _wallLayer = 0;
    public RaycastHit LeftWallHit = new RaycastHit();
    public RaycastHit RightWallHit = new RaycastHit();
    public bool IsWallLeft { get; private set; }
    public bool IsWallRight { get; private set; }

    [Header("Ledge Detection")]
    [SerializeField] private float _ledgeGrabForwardPos = 0.0f;    // The position in front of the player where we check for ledges
    [SerializeField] private float _ledgeGrabUpwardsPos = 0.0f;    // The position in above of the player where we check for ledges
    [SerializeField] private LayerMask _ledgeGrabLayers = 0;
    public Vector3 Ledge { get; private set; }

    //[Header("Hook Shot")]
    [field: SerializeField] public ParticleSystem GrapleVFX { get; private set; }
    [field: SerializeField] public LayerMask HookableLayers { get; private set; }
    [field: SerializeField] public LineRenderer HookShotLineRenderer { get; private set; }
    [field: SerializeField] public AnimationCurve HookAffectCurve { get; private set; }
    [field: SerializeField] public Transform[] LyreTipsTransforms { get; private set; }
    [field: SerializeField] public LineRenderer[] LyreLineRenderers { get; private set; }
    [field: SerializeField] public Transform HookShotTransform { get; private set; }


    [Header("Slope Handling")]
    [SerializeField] private float _maxSlopeAngle = 0.0f;
    [SerializeField] private float _minSlopeAngle = 0.0f;
    public Vector3 SlopeNormal { get; private set; }
    private bool _exitingSlope = false;

    [Header("Movement")]
    [SerializeField] private float _speedIncreaseMultiplyer = 0.0f;
    [SerializeField] private float _slopeIncreaseMultiplyer = 0.0f;
    [field: SerializeField] public float GroundDrag { get; private set; }
    [field: SerializeField] public float MoveSpeed { get; set; }
    [field: SerializeField] public float DesiredMoveSpeed { get; set; }
    [field: SerializeField] public float LastDesiredMoveSpeed { get; private set; }
    [field: SerializeField] public float MaxYSpeed { get; private set; }

    // HookShot Interact
    public IEnumerator InteractionCoroutine = null;

    [Header("Cooldowns")]
    [SerializeField] private float _dashCoolDown = 0.0f;
    [SerializeField] private int _dashAmount = 1;
    private int _dashCount = 0;
    public float DashCoolDownTimer { get; private set; }

    private float _wallCoolDown = 0.0f;
    private float _slideCoolDown = 0.0f;

    // Flags
    private bool _isGrounded = false;
    private bool _isWallDetected = false;

    // Props
    public bool IsGrounded { get => _isGrounded; }
    public int DashCount { get { return _dashCount; } set { _dashCount = value; } }
    public bool IsWallDetected { get => _isWallDetected; }
    public MultiplierManager MultiplierManager => _multiplierManager;
    public MovementBehaviour CurrentMovementBehaviour => InputReader.CurrentMovementBehaviour;

    public bool CanWallRun() { return _wallCoolDown <= 0.0f; }

    public bool CanSlide() { return _slideCoolDown <= 0.0f; }

    private void Awake()
    {
        LevelLoader.CallOnComplete(Initialize);
    }

    private void OnDestroy()
    {
        ServiceLocator.Get<EventBusCallbacks>().OnKillBoxTriggerMessageHandled -= PlayerKilledHandle;
    }

    public void Initialize()
    {
        WwiseManager = ServiceLocator.Get<IWwiseManager>();
        ResetDashTimer();
        RigidBody.freezeRotation = true;
        SwitchState(new PlayerSprintingState(this));
        _spawnPosition = this.transform.position;

        ServiceLocator.Get<EventBusCallbacks>().OnKillBoxTriggerMessageHandled += PlayerKilledHandle;
        _multiplierManager = ServiceLocator.Get<MultiplierManager>();

        _dashCount = _dashAmount;
    }

    protected override void Update()
    {
        base.Update();
        _currentState = GetCurrentStateName();

        // Update flags
        UpdateTimers();

        // Ground check
        RaycastHit hitInfo;
        _isGrounded = Physics.SphereCast(transform.position, 0.15f, Vector3.down, out hitInfo, _playerHeight * 0.5f + 0.2f, _groundLayer);

        // Wall check
        IsWallLeft = Physics.Raycast(transform.position, -Orientation.right, out LeftWallHit, _wallCheckDistance, _wallLayer);
        IsWallRight = Physics.Raycast(transform.position, Orientation.right, out RightWallHit, _wallCheckDistance, _wallLayer);
        _isWallDetected = IsWallLeft || IsWallRight;

        if (_isGrounded || _isWallDetected)
        {
            _dashCount = _dashAmount;
        }

        SpeedControl();
    }

    public bool CheckForLedgeGrab(float ledgeGrabDistance)
    {
        return CheckLedges(ledgeGrabDistance) != Vector3.zero && !Physics.CheckSphere(LedgeProtectionSphereCenter.position, 0.75f, _ledgeGrabLayers);
    }

    private Vector3 CheckLedges(float ledgeGrabDistance)
    {
        Vector3 top = transform.position + (Orientation.forward * _ledgeGrabForwardPos) + (Orientation.up * _ledgeGrabUpwardsPos);
        Vector3 bot = new Vector3(top.x, top.y - ledgeGrabDistance, top.z);

        RaycastHit hit;
        if (Physics.Raycast(top, bot - top, out hit, ledgeGrabDistance, _ledgeGrabLayers))
        {
            Ledge = hit.point;
            return hit.point;
        }

        return Vector3.zero;
    }

    private void SpeedControl()
    {
        // Limiting speed on slope
        if (OnSlope() && !_exitingSlope)
        {
            if (RigidBody.velocity.magnitude > MoveSpeed)
            {
                RigidBody.velocity = RigidBody.velocity.normalized * MoveSpeed;
            }
        }
        else // Limiting speed on ground or in air
        {
            Vector3 flatVel = new Vector3(RigidBody.velocity.x, 0.0f, RigidBody.velocity.z);

            // Limit velocity if needed
            if (flatVel.magnitude > MoveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * MoveSpeed;
                RigidBody.velocity = new Vector3(limitedVel.x, RigidBody.velocity.y, limitedVel.z);
            }
        }

        // Limit Y velocity
        if (MaxYSpeed != 0.0f && RigidBody.velocity.y > MaxYSpeed)
        {
            RigidBody.velocity = new Vector3(RigidBody.velocity.x, MaxYSpeed, RigidBody.velocity.z);
        }

        // Check if desiredMoveSpeed has changed drastically
        if (Mathf.Abs(DesiredMoveSpeed - LastDesiredMoveSpeed) >= 4.0f && MoveSpeed != 0.0f)
        {
            //StopAllCoroutines();
            StartCoroutine(SmoothlyLerpMoveSpeed());
        }
        else
        {
            MoveSpeed = DesiredMoveSpeed;
        }

        LastDesiredMoveSpeed = DesiredMoveSpeed;
    }

    public bool OnSlope()
    {
        RaycastHit _slopeHit;
        if (Physics.Raycast(transform.position, Vector3.down, out _slopeHit, _playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, _slopeHit.normal);
            SlopeNormal = _slopeHit.normal;
            return angle <= _maxSlopeAngle && angle >= _minSlopeAngle;
        }

        return false;
    }

    private IEnumerator SmoothlyLerpMoveSpeed()
    {
        // Smootly lerp movement speed
        float time = 0.0f;
        float difference = Mathf.Abs(DesiredMoveSpeed - MoveSpeed);
        float startValue = MoveSpeed;

        while (time < difference)
        {
            MoveSpeed = Mathf.Lerp(startValue, DesiredMoveSpeed, time / difference);

            if (OnSlope())
            {
                float slopeAngle = Vector3.Angle(Vector3.up, SlopeNormal);
                float slopeAngleIncrease = 1.0f + (slopeAngle / 90.0f);

                time += Time.deltaTime * _speedIncreaseMultiplyer * _slopeIncreaseMultiplyer * slopeAngleIncrease;
            }
            else
            {
                time += Time.deltaTime;
            }
            yield return null;
        }

        MoveSpeed = DesiredMoveSpeed;
    }

    private void UpdateTimers()
    {
        if (DashCoolDownTimer >= 0.0f)
        {
            DashCoolDownTimer -= Time.deltaTime;
        }

        if (_wallCoolDown >= 0.0f)
        {
            _wallCoolDown -= Time.deltaTime;
        }

        if (_slideCoolDown >= 0.0f)
        {
            _slideCoolDown -= Time.deltaTime;
        }
    }

    public void ResetDashTimer()
    {
        DashCoolDownTimer = _dashCoolDown;
    }

    public void ResetWallCoolDown(float coolDown) { _wallCoolDown = coolDown; }

    public void ResetSlideCoolDown(float coolDown) { _slideCoolDown = coolDown; }

    private void PlayerKilledHandle(OnKillBoxTrigger obj)
    {
        PlayerVisualEffects.PlayOnDieFeedback();
        Die();
    }

    public void Die()
    {
        onDeath?.Invoke();

        // Stop Interacting
        if (InteractionCoroutine != null)
        {
            StopCoroutine(InteractionCoroutine);
            InteractionCoroutine = null;
            CleanLyreLineRenderers();
        }

        SwitchState(new PlayerAirState(this));
    }

    public void CleanLyreLineRenderers()
    {
        foreach (var renderer in LyreLineRenderers)
        {
            renderer.positionCount = 0;
        }
    }

    private void OnDrawGizmos()
    {
        // Ledge Check
        Vector3 top = transform.position + (Orientation.forward * _ledgeGrabForwardPos) + (Orientation.up * _ledgeGrabUpwardsPos);
        Vector3 bot = new Vector3(top.x, top.y - 2.0f, top.z);
        Gizmos.color = Color.gray;
        Gizmos.DrawRay(top, bot - top);
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(top, bot - top + Vector3.up * 0.5f);
        Gizmos.DrawSphere(LedgeProtectionSphereCenter.position, 0.75f);

        // Slide Check
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(OverHeadSphereCenter.position, 0.5f);
    }
}