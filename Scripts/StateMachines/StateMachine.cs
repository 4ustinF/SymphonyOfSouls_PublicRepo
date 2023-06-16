using UnityEngine;

public abstract class StateMachine : MonoBehaviour
{
    private State currentState;

    protected virtual void Update()
    {
        currentState?.Tick(Time.deltaTime);
    }

    protected virtual void FixedUpdate()
    {
        currentState?.FixedTick(Time.fixedDeltaTime);
    }

    public void SwitchState(State newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState?.Enter();
    }

    public string GetCurrentStateName()
    {
        return currentState?.GetType().Name;
    }
}
