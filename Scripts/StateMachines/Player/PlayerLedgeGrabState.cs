using System.Collections;
using UnityEngine;

public class PlayerLedgeGrabState : PlayerBaseState
{
    private float _desiredYoffset = 0.5f;         // How much you should should be above the ledge
    private float _desiredZoffset = 0.3f;         // How much you should should be forward on the ledge
    private Vector3 _origPos = Vector3.zero;      // The original Position before grabbing a ledge
    private Vector3 _ledgePos = Vector3.zero;     // The ledge position to move to
    private Vector3 _origVel = Vector3.zero;      // The original velocity before ledge grab
    private float _velModifier = 0.8f;            // Effects velocity reset after ledge grab 1.0 = no loss (full), 0.5 = half; 
    private float _ledgeGrabSpeed = 3.75f;        // The time it takes to pull onto a ledge

    public PlayerLedgeGrabState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        // Set our ledge position
        _ledgePos = stateMachine.Ledge;
        _origPos = stateMachine.transform.position;

        // Remove speed and velocity
        _origVel = stateMachine.RigidBody.velocity;
        stateMachine.RigidBody.velocity = Vector3.zero;
        stateMachine.WwiseManager.PlayLedgeGrabSoundEffect();

        MovePlayer();
    }

    public override void Exit()
    {
    }

    public override void Tick(float deltaTime)
    {
    }

    public override void FixedTick(float fixedDeltaTime)
    {
    }

    protected override void MovePlayer()
    {
        stateMachine.StartCoroutine(PullUp());
    }

    private IEnumerator PullUp()
    {
        _ledgePos += stateMachine.Orientation.forward * _desiredZoffset;
        _ledgePos += stateMachine.Orientation.up * _desiredYoffset;

        for (float time = 0.0f; time < 1.0f; time += Time.deltaTime * _ledgeGrabSpeed)
        {
            stateMachine.transform.position = Vector3.Lerp(_origPos, _ledgePos, time);
            stateMachine.transform.position += stateMachine.Orientation.up * _desiredYoffset;

            yield return null;
        }

        // Momentum after ledge grab
        Vector3 velocityAfterGrab = new Vector3(_origVel.x, _origVel.y * 0.5f, _origVel.z * 0.5f);
        stateMachine.RigidBody.velocity = velocityAfterGrab * _velModifier;

        stateMachine.SwitchState(new PlayerAirState(stateMachine));
    }
}

