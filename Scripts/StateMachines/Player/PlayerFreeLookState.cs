public class PlayerFreeLookState : PlayerBaseState
{
    public PlayerFreeLookState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        stateMachine.InputReader.JumpEvent += Jump;

    }

    public override void Exit()
    {
        stateMachine.InputReader.JumpEvent -= Jump;
    }

    public override void FixedTick(float fixedDeltaTime)
    {
        throw new System.NotImplementedException();
    }

    public override void Tick(float deltaTime)
    {
    
    }

    private void Jump()
    {
    }
}
