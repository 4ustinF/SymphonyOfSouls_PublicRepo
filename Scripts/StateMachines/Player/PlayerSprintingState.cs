using System.Collections;
using UnityEngine;

public class PlayerSprintingState : PlayerBaseState
{
    private Vector3 _moveDirection = Vector3.zero;
    private readonly float _jumpForce = 10.5f;
    private readonly float _sprintingSpeed = 10.0f;
    private readonly float _sprintingFOV = 85.0f;
    private readonly float _ledgeGrabDistance = 1.5f;      // The distance the ledge can be from our raycast before we grab it (this is projects from the top of the wall grab position, downwards
    private IEnumerator _sfxRunningCoroutine = null;

    public PlayerSprintingState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        stateMachine.InputReader.JumpEvent += Jump;
        stateMachine.InputReader.DashEvent += Dash;
        stateMachine.InputReader.HookShotEvent += ShootHookShot;
        stateMachine.InputReader.InteractEvent += Interact;

        stateMachine.RigidBody.drag = stateMachine.GroundDrag;
        stateMachine.DesiredMoveSpeed = _sprintingSpeed;
        stateMachine.MoveSpeed = _sprintingSpeed;
        stateMachine.WwiseManager.SetRunningSwitch();

        _sfxRunningCoroutine = SFXRunningCoroutine();
        stateMachine.StartCoroutine(_sfxRunningCoroutine);
    }

    public override void Exit()
    {
        stateMachine.InputReader.JumpEvent -= Jump;
        stateMachine.InputReader.DashEvent -= Dash;
        stateMachine.InputReader.HookShotEvent -= ShootHookShot;
        stateMachine.InputReader.InteractEvent -= Interact;

        stateMachine.RigidBody.drag = 0.0f;
        stateMachine.RigidBody.useGravity = true;

        stateMachine.StopCoroutine(_sfxRunningCoroutine);
    }

    public override void Tick(float deltaTime)
    {
        var reader = stateMachine.InputReader;
        //if (reader.IsCrouching && stateMachine.CanSlide() && (stateMachine.RigidBody.velocity.magnitude > 0.1f || Physics.CheckSphere(stateMachine.OverHeadSphereCenter.position, 0.5f, stateMachine.GameLayers)))
        if (reader.IsCrouching)
        {
            //Debug.Break();
            stateMachine.SwitchState(new PlayerSlidingState(stateMachine));
        }
        else if (stateMachine.IsGrounded == false)
        {
            stateMachine.SwitchState(new PlayerAirState(stateMachine));
        }
        else if (stateMachine.CheckForLedgeGrab(_ledgeGrabDistance))
        {
            stateMachine.SwitchState(new PlayerLedgeGrabState(stateMachine));
        }
    }

    public override void FixedTick(float fixedDeltaTime)
    {
        MovePlayer();
    }

    private void Jump()
    {
        stateMachine.WwiseManager.PlayJumpingSoundEffect();
        stateMachine.RigidBody.velocity = new Vector3(stateMachine.RigidBody.velocity.x, 0.0f, stateMachine.RigidBody.velocity.z);
        stateMachine.RigidBody.AddForce(stateMachine.transform.up * _jumpForce, ForceMode.Impulse);
    }

    protected override void MovePlayer()
    {
        var reader = stateMachine.InputReader;
        if (reader.MovementValue.magnitude == 0.0f)
        {
            stateMachine.PlayerCamera.CameraEffectsReset();
            return;
        }

        stateMachine.PlayerCamera.DoFov(_sprintingFOV);

        // Calculate movement direction
        _moveDirection = stateMachine.Orientation.forward * reader.MovementValue.y + stateMachine.Orientation.right * reader.MovementValue.x;

        if (stateMachine.OnSlope())
        {
            Vector3 slopeMoveDirection = Vector3.ProjectOnPlane(_moveDirection, stateMachine.SlopeNormal).normalized;
            stateMachine.RigidBody.AddForce(slopeMoveDirection * stateMachine.MoveSpeed * 20.0f, ForceMode.Force);

            if (stateMachine.RigidBody.velocity.y > 0.0f)
            {
                stateMachine.RigidBody.AddForce(Vector3.down * 80.0f, ForceMode.Force);
            }
        }
        else
        {
            stateMachine.RigidBody.AddForce(_moveDirection.normalized * stateMachine.MoveSpeed * 10.0f, ForceMode.Force);
        }
        stateMachine.RigidBody.useGravity = !stateMachine.OnSlope();
    }

    IEnumerator SFXRunningCoroutine()
    {
        while (true)
        {
            if (stateMachine.RigidBody.velocity.magnitude > 1.0f)
            {
                stateMachine.WwiseManager.PlayFootSteps();
            }
            yield return new WaitForSeconds(0.2f);
        }
    }
}