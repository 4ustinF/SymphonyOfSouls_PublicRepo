using UnityEngine;

public class PlayerSlidingState : PlayerBaseState
{
    private readonly float _slidingCoolDown = 1.0f;
    private readonly float _jumpForce = 10.5f;
    private readonly float _slideYScale = 0.5f;
    private readonly float _maxSlideTime = 1.75f;
    private float _slideTimer = 0.0f;
    private float _startYScale = 0.0f;
    private readonly float _slideForce = 10.0f;
    private readonly float _slopeSlideSpeed = 18.0f;
    private readonly float _slideSpeed = 15.0f;
    private readonly float _slidingFov = 85.0f;
    private readonly float _jumpMultiplyer = 0.5f;
    private bool _isOverHead = false;
    private float _previousPositionY = 0.0f;
    public PlayerSlidingState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        stateMachine.MultiplierManager.ActionPerformed(Enums.PlayerActionType.Slide);
        stateMachine.BouncyCoat.SetActive(false);

        stateMachine.InputReader.JumpEvent += Jump;
        stateMachine.InputReader.HookShotEvent += ShootHookShot;
        stateMachine.InputReader.InteractEvent += Interact;

        _previousPositionY = stateMachine.transform.position.y;
        _startYScale = stateMachine.transform.localScale.y;
        stateMachine.transform.localScale = new Vector3(stateMachine.transform.localScale.x, _slideYScale, stateMachine.transform.localScale.z);
        stateMachine.RigidBody.AddForce(Vector3.down * 5.0f, ForceMode.Impulse);
        _slideTimer = _maxSlideTime;
        stateMachine.DesiredMoveSpeed = _slideSpeed;
        stateMachine.PlayerCamera.DoFov(_slidingFov);
        stateMachine.WwiseManager.PlaySlideSoundEffect();
    }

    public override void Exit()
    {
        stateMachine.BouncyCoat.SetActive(true);

        stateMachine.InputReader.HookShotEvent -= ShootHookShot;
        stateMachine.InputReader.InteractEvent -= Interact;
        stateMachine.InputReader.JumpEvent -= Jump;

        stateMachine.transform.localScale = new Vector3(stateMachine.transform.localScale.x, _startYScale, stateMachine.transform.localScale.z);
        stateMachine.PlayerCamera.CameraEffectsReset();

        stateMachine.ResetSlideCoolDown(_slidingCoolDown);
    }

    public override void Tick(float deltaTime)
    {
        var reader = stateMachine.InputReader;
        //_isOverHead = Physics.CheckSphere(stateMachine.OverHeadSphereCenter.position, 0.5f, stateMachine.GameLayers);
        _isOverHead = Physics.CheckSphere(stateMachine.OverHeadSphereCenter.position , 1.0f, stateMachine.GameLayers);
        //Debug.Log(_isOverHead);

        if ((reader.IsCrouching == false && _isOverHead == false && stateMachine.IsGrounded))
        {
            stateMachine.SwitchState(new PlayerSprintingState(stateMachine));
        }
        else if (stateMachine.IsGrounded == false)
        {
            stateMachine.SwitchState(new PlayerAirState(stateMachine));
        }

        if (reader.IsCrouching)
        {
            if (_slideTimer < 0.0f)
            {
                float moveSpeed = _slideSpeed * 0.4f;
                stateMachine.MoveSpeed = moveSpeed;
                stateMachine.DesiredMoveSpeed = moveSpeed;
            }
        }

        //if(_isOverHead)
        //{
        //    Debug.Log(Physics.OverlapSphere(stateMachine.OverHeadSphereCenter.position, 0.5f, stateMachine.GameLayers)[0].gameObject);
        //}
    }

    public override void FixedTick(float fixedDeltaTime)
    {
        MovePlayer();
    }

    private void Jump()
    {
        stateMachine.WwiseManager.PlayJumpingSoundEffect();
        stateMachine.RigidBody.velocity = new Vector3(stateMachine.RigidBody.velocity.x, 0.0f, stateMachine.RigidBody.velocity.z);
        Vector3 jumpPower = (stateMachine.transform.up + stateMachine.Orientation.forward) * (_jumpForce + stateMachine.RigidBody.velocity.magnitude);
        jumpPower = new Vector3(jumpPower.x, jumpPower.y * _jumpMultiplyer, jumpPower.z); ;
        stateMachine.RigidBody.AddForce(jumpPower, ForceMode.Impulse);
    }

    protected override void MovePlayer()
    {
        Vector3 inputDirection = stateMachine.Orientation.forward * stateMachine.InputReader.MovementValue.y + stateMachine.Orientation.right * stateMachine.InputReader.MovementValue.x;

        //if (inputDirection.magnitude < 0.01f && _isOverHead == false)
        //{
        //    stateMachine.SwitchState(new PlayerAirState(stateMachine));
        //}

        // sliding normal
        if (stateMachine.OnSlope() == false)
        {
            stateMachine.DesiredMoveSpeed = _slideSpeed;
            stateMachine.RigidBody.AddForce(inputDirection.normalized * stateMachine.MoveSpeed * 10.0f, ForceMode.Force);
            stateMachine.RigidBody.AddForce(inputDirection.normalized * _slideForce, ForceMode.Force);
            _slideTimer -= Time.deltaTime;
        }
        else  // sliding down a slope
        {
            stateMachine.DesiredMoveSpeed = _slopeSlideSpeed;
            if (_previousPositionY < stateMachine.transform.position.y)
            {
                //Go Up
                Vector3 slopeMoveDirection = Vector3.ProjectOnPlane(inputDirection, stateMachine.SlopeNormal).normalized;
                stateMachine.RigidBody.AddForce(slopeMoveDirection * _slideForce * 0.8f, ForceMode.Force);
            }
            else
            {
                //Go down
                Vector3 slopeMoveDirection = Vector3.ProjectOnPlane(inputDirection, -stateMachine.SlopeNormal).normalized;
                stateMachine.RigidBody.AddForce(slopeMoveDirection * _slideForce, ForceMode.Force);
            }
        }

        _previousPositionY = stateMachine.transform.position.y;
    }
}
