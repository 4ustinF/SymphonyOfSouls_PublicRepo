using System.Collections;
using UnityEngine;

public class PlayerWallRunState : PlayerBaseState
{
    private float _wallRunTimer = 0.0f;
    private readonly float _wallRunCoolDown = 1.5f;
    private readonly float _maxWallRunTime = 2.5f;
    private readonly float _wallRunForce = 200.0f;
    private readonly float _wallJumpUpForce = 5.5f;
    private readonly float _wallJumpSideForce = 15.0f;
    private readonly float _wallJumpForwardForce = 12.5f;
    private readonly bool _useGravity = true;
    private readonly float _gravityCounterForce = 16.5f;
    private readonly float _wallRunFOV = 90.0f;
    private readonly float _tiltAmount = 2.0f;
    private IEnumerator _sfxRunningCoroutine = null;

    public PlayerWallRunState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        stateMachine.InputReader.JumpEvent += WallJump;
        stateMachine.InputReader.DashEvent += Dash;
        stateMachine.InputReader.HookShotEvent += ShootHookShot;
        stateMachine.InputReader.InteractEvent += Interact;
        ServiceLocator.Get<IWwiseManager>().SetWallRunningSwitch();
        stateMachine.MultiplierManager.StartWallRun();
        StartWallRun();
        stateMachine.PlayerVisualEffects.PlayWallRunFeedback();

        _sfxRunningCoroutine = SFXRunningCoroutine();
        stateMachine.StartCoroutine(_sfxRunningCoroutine);
        stateMachine.PlayerCamera.EnableDisableWiggle(false);
    }

    public override void Exit()
    {
        stateMachine.InputReader.JumpEvent -= WallJump;
        stateMachine.InputReader.DashEvent -= Dash;
        stateMachine.InputReader.HookShotEvent -= ShootHookShot;
        stateMachine.InputReader.InteractEvent -= Interact;

        stateMachine.MultiplierManager.SetDecayingStatus(true);
        stateMachine.MultiplierManager.StoptWallRun();
        stateMachine.PlayerCamera.CameraEffectsReset();

        stateMachine.StopCoroutine(_sfxRunningCoroutine);
        stateMachine.PlayerCamera.EnableDisableWiggle(true);

        //stateMachine.PlayerVisualEffects.PlayWallExitFeedback();
    }

    public override void Tick(float deltaTime)
    {
        if (_wallRunTimer <= 0.0f)
        {
            stateMachine.ResetWallCoolDown(_wallRunCoolDown);
            stateMachine.SwitchState(new PlayerAirState(stateMachine));
        }
        else if (stateMachine.IsWallDetected == false)
        {
            stateMachine.SwitchState(new PlayerAirState(stateMachine));
        }

        if (_wallRunTimer > 0.0f)
        {
            _wallRunTimer -= deltaTime;
        }
    }

    public override void FixedTick(float fixedDeltaTime)
    {
        MovePlayer();
    }

    private void StartWallRun()
    {
        _wallRunTimer = _maxWallRunTime;
        stateMachine.RigidBody.velocity = new Vector3(stateMachine.RigidBody.velocity.x, 0.0f, stateMachine.RigidBody.velocity.z);

        // Apply Camera FOV Effects
        stateMachine.PlayerCamera.DoFov(_wallRunFOV);
        if (stateMachine.IsWallLeft)
        {
            stateMachine.PlayerCamera.DoTilt(-_tiltAmount);
        }
        else if (stateMachine.IsWallRight)
        {
            stateMachine.PlayerCamera.DoTilt(_tiltAmount);
        }
    }

    private void WallJump()
    {
        // Enter exiting wall state
        Vector3 wallNormal = stateMachine.IsWallRight ? stateMachine.RightWallHit.normal : stateMachine.LeftWallHit.normal;
        Vector3 forceToApply = stateMachine.Orientation.up * _wallJumpUpForce + wallNormal * _wallJumpSideForce + stateMachine.Orientation.forward * _wallJumpForwardForce;

        // Reset y velocity to have a nice jump even if your falling
        stateMachine.RigidBody.velocity = new Vector3(stateMachine.RigidBody.velocity.x, 0.0f, stateMachine.RigidBody.velocity.z);

        // Wall Jump
        stateMachine.RigidBody.AddForce(forceToApply, ForceMode.Impulse);

        stateMachine.PlayerVisualEffects.PlayWallExitFeedback();
        stateMachine.WwiseManager.PlayJumpingSoundEffect();
    }

    protected override void MovePlayer()
    {
        stateMachine.RigidBody.useGravity = _useGravity;

        Vector3 wallNormal = stateMachine.IsWallRight ? stateMachine.RightWallHit.normal : stateMachine.LeftWallHit.normal;
        Vector3 wallForward = Vector3.Cross(wallNormal, stateMachine.Orientation.up);

        if ((stateMachine.Orientation.forward - wallForward).magnitude > (stateMachine.Orientation.forward + wallForward).magnitude)
        {
            wallForward = -wallForward;
        }

        // Forward force
        stateMachine.RigidBody.AddForce(wallForward * _wallRunForce, ForceMode.Force);

        // Passive Force
        if (!(stateMachine.IsWallLeft && stateMachine.InputReader.MovementValue.x > 0.0f) && !(stateMachine.IsWallRight && stateMachine.InputReader.MovementValue.x < 0.0f))
        {
            stateMachine.RigidBody.AddForce(-wallForward * 100.0f, ForceMode.Force);
        }

        // Weaken gravity
        if (_useGravity)
        {
            stateMachine.RigidBody.AddForce(stateMachine.Orientation.up * _gravityCounterForce, ForceMode.Force);
        }
    }

    IEnumerator SFXRunningCoroutine()
    {
        while (true)
        {
            if (stateMachine.RigidBody.velocity.magnitude > 1.0f)
            {
                stateMachine.WwiseManager.PlayFootSteps();
            }
            yield return new WaitForSeconds(0.18f);
        }
    }
}
