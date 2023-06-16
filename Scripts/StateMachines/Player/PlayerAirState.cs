using UnityEngine;

public class PlayerAirState : PlayerBaseState
{
    private readonly float _movementMultiplier = 2.0f;
    private readonly float _crouchingMultiplier = 4.5f;
    private readonly float _ledgeGrabDistance = 2.0f;      // The distance the ledge can be from our raycast before we grab it (this is projects from the top of the wall grab position, downwards
    private bool _useCrouch = true;

    public PlayerAirState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        stateMachine.InputReader.DashEvent += Dash;
        stateMachine.InputReader.HookShotEvent += ShootHookShot;
        stateMachine.InputReader.InteractEvent += Interact;

        stateMachine.RigidBody.useGravity = true;
        _useCrouch = !(stateMachine.InputReader.IsCrouching);
    }

    public override void Exit()
    {
        stateMachine.InputReader.DashEvent -= Dash;
        stateMachine.InputReader.HookShotEvent -= ShootHookShot;
        stateMachine.InputReader.InteractEvent -= Interact;
    }

    public override void Tick(float deltaTime)
    {
        if (stateMachine.IsGrounded && stateMachine.InputReader.IsCrouching) //&&stateMachine.CanSlide() && stateMachine.RigidBody.velocity.magnitude > 0.1f)
        {
            stateMachine.SwitchState(new PlayerSlidingState(stateMachine));
        }
        else if (stateMachine.IsGrounded)
        {
            stateMachine.SwitchState(new PlayerSprintingState(stateMachine));
        }
        else if (stateMachine.CanWallRun() && stateMachine.IsWallDetected)
        {
            stateMachine.SwitchState(new PlayerWallRunState(stateMachine));
        }
        else if (stateMachine.CheckForLedgeGrab(_ledgeGrabDistance))
        {
            stateMachine.SwitchState(new PlayerLedgeGrabState(stateMachine));
        }

        if(_useCrouch == false && stateMachine.InputReader.IsCrouching == false)
        {
            _useCrouch = true;
        }

        MovePlayer();
    }

    public override void FixedTick(float fixedDeltaTime)
    {

    }

    protected override void MovePlayer()
    {
        Vector3 inputDirection = stateMachine.Orientation.forward * stateMachine.InputReader.MovementValue.y + stateMachine.Orientation.right * stateMachine.InputReader.MovementValue.x;
        stateMachine.RigidBody.AddForce(inputDirection.normalized * stateMachine.MoveSpeed * _movementMultiplier, ForceMode.Force);

        if (stateMachine.InputReader.IsCrouching && _useCrouch)
        {
            stateMachine.RigidBody.AddForce(-stateMachine.Orientation.up * stateMachine.MoveSpeed * _crouchingMultiplier, ForceMode.Force);
        }
    }

}
