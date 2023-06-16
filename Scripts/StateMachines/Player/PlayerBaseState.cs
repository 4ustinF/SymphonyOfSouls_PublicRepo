using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerBaseState : State
{
    protected PlayerStateMachine stateMachine = null;

    // Dash
    private bool _resetVel = true;
    private readonly float _dashForce = 200.0f;
    private readonly float _dashUpwardForce = 11.25f;
    private readonly float _dashFOV = 90.0f;
    private readonly float _dashMultiplier = 5.0f;

    // Hook Interact
    private readonly int _quality = 100;
    private readonly float _damper = 7.0f;
    private readonly float _strength = 800.0f;
    private readonly float _startVelocity = 10.0f;
    private float _velocity = 0.0f;
    private readonly float _waveCount = 3.0f;
    private readonly float _waveHeight = 6.0f;
    private readonly float _hookSpeed = 12.0f;

    // Hook Shot
    private readonly float _hookShotRange = 100.0f;

    public PlayerBaseState(PlayerStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    protected void Dash()
    {
        if (stateMachine.DashCount <= 0 || stateMachine.DashCoolDownTimer > 0.0f)
        {
            return;
        }

        stateMachine.ResetDashTimer();
        stateMachine.DashCount--;

        float horizontalInput = stateMachine.InputReader.MovementValue.x;
        float verticalInput = stateMachine.InputReader.MovementValue.y;
        Transform orientation = stateMachine.Orientation;

        // Omnidirectional angle
        float verticalAngle = Vector3.Dot(stateMachine.CameraHolder.forward, Vector3.up);
        Mathf.Clamp(verticalAngle, -0.95f, 0.85f);

        Vector3 direction = orientation.forward;
        if (verticalInput != 0.0f || horizontalInput != 0.0f)
        {
            direction = (orientation.forward * verticalInput + orientation.right * horizontalInput).normalized;
        }

        Vector3 forceToApply = direction * _dashForce + stateMachine.Orientation.up * verticalAngle * _dashUpwardForce;
        if (_resetVel)
        {
            stateMachine.RigidBody.velocity = Vector3.zero;
        }
        stateMachine.StartCoroutine(DashRoutine(forceToApply));
    }

    private IEnumerator DashRoutine(Vector3 force)
    {
        ServiceLocator.Get<IWwiseManager>().PlayDashSoundEffect();
        stateMachine.MultiplierManager.ActionPerformed(Enums.PlayerActionType.Dash);
        stateMachine.PlayerCamera.DoFov(_dashFOV);
        float time = 0.0f;
        while (time < 1.0f)
        {
            float fraction = Time.deltaTime * _dashMultiplier;
            stateMachine.RigidBody.AddForce(force * fraction, ForceMode.Impulse);
            time += fraction;
            yield return null;
        }
        stateMachine.PlayerCamera.CameraEffectsReset(0.1f);
    }

    protected void ShootHookShot()
    {
        var raycastHit = new RaycastHit();

        if (Physics.Raycast(stateMachine.CameraHolder.position, stateMachine.CameraHolder.forward, out raycastHit, _hookShotRange))
        {
            if (((1 << raycastHit.collider.gameObject.layer) & stateMachine.HookableLayers) != 0)
            {
                stateMachine.SwitchState(new PlayerHookShotState(stateMachine, raycastHit));
            }
        }
    }

    protected void Interact()
    {
        stateMachine.PlayerVisualEffects.PlayInteractAnimation();
        stateMachine.MultiplierManager.ActionPerformed(Enums.PlayerActionType.Interact);

        var raycastHit = new RaycastHit();
        if (Physics.Raycast(stateMachine.CameraHolder.position, stateMachine.CameraHolder.forward, out raycastHit, _hookShotRange))
        {
            if (stateMachine.InteractionCoroutine == null)
            {
                if (((1 << raycastHit.collider.gameObject.layer) & stateMachine.InteractableLayers) != 0)
                {
                    stateMachine.WwiseManager.PlayAttackSoundEfect();
                    stateMachine.InteractionCoroutine = InteractRoutine(raycastHit);
                    stateMachine.StartCoroutine(stateMachine.InteractionCoroutine);
                }
            }
        }
    }

    protected virtual void MovePlayer()
    {
    }

    private IEnumerator InteractRoutine(RaycastHit raycastHit)
    {
        // Enter
        Vector3 point = Vector3.zero;
        Vector3 currentEndPosition = stateMachine.Orientation.transform.position;
        AimAssist aimAssist = raycastHit.collider.gameObject.GetComponent<AimAssist>();
        float value = 0;
        _velocity = _startVelocity;

        SetPoistionsCount(_quality + 1);

        while (true)
        {
            point = aimAssist != null ? aimAssist.TargetPoint.position : raycastHit.point;

            // Update velocity and value
            var direction = -value >= 0 ? 1f : -1f;
            var force = Mathf.Abs(-value) * _strength;
            _velocity += (force * direction - _velocity * _damper) * Time.deltaTime;
            value += _velocity * Time.deltaTime;
            currentEndPosition = Vector3.Lerp(currentEndPosition, point, Time.deltaTime * _hookSpeed);

            for (var i = 0; i <= _quality; i++)
            {
                var delta = i / (float)_quality;
                float curveEvaluate = stateMachine.HookAffectCurve.Evaluate(delta);

                EvaluateString(0, i, delta, curveEvaluate, value, point, currentEndPosition,0);
                EvaluateString(2, i, delta, curveEvaluate, value, point, currentEndPosition, 1);
                EvaluateString(4, i, delta, curveEvaluate, value, point, currentEndPosition, 2);
                EvaluateString(6, i, delta, curveEvaluate, value, point, currentEndPosition, 1);
                EvaluateString(7, i, delta, curveEvaluate, value, point, currentEndPosition, 0);
            }

            if (Vector3.Distance(currentEndPosition, point) < 1.0f)
            {
                Interact(raycastHit);
                break;
            }
            yield return null;
        }

        // Exit
        stateMachine.CleanLyreLineRenderers();
        stateMachine.PlayerCamera.CameraEffectsReset();
        stateMachine.InteractionCoroutine = null;
    }

    private void EvaluateString(int rendererIndex, int pointIndex, float delta, float curveEvaluate, float value, Vector3 point, Vector3 currentEndPosition, int offsetType)
    {
        Vector3 up = Quaternion.LookRotation((point - stateMachine.LyreTipsTransforms[rendererIndex].position).normalized) * Vector3.up;
        Vector3 offset = GetRandomOffset(offsetType, up, delta, value, curveEvaluate);
        var renderer = stateMachine.LyreLineRenderers[rendererIndex];
        renderer.SetPosition(pointIndex, Vector3.Lerp(stateMachine.LyreTipsTransforms[rendererIndex].position, currentEndPosition, delta) + offset);
    }

    private void Interact(RaycastHit _hitInfo)
    {
        IInteractable interactable = null;
        if (_hitInfo.collider.gameObject.TryGetComponent<IInteractable>(out interactable))
        {
            List<RewardDef> rewards = null;
            interactable.Interact(out rewards);
            if (rewards != null)
            {
                stateMachine.RewardReciver.Apply(rewards);
            }
        }
    }

    private void SetPoistionsCount(int positionCount)
    {
        foreach (var renderer in stateMachine.LyreLineRenderers)
        {
            renderer.positionCount = _quality + 1;
        }
    }

    private Vector3 GetRandomOffset(int offsetType, Vector3 up, float delta, float value, float curveEvaluate)
    {
        Vector3 offset = Vector3.zero;

        if (offsetType == 0)
        {
            offset = up * _waveHeight * Mathf.Sin(delta * _waveCount * Mathf.PI) * value * curveEvaluate;
        }
        else if (offsetType == 1)
        {
            offset = new Vector3(offset.z, offset.x, offset.y);
        }
        else if (offsetType == 2)
        {
            offset = up * _waveHeight * Mathf.Cos(delta * _waveCount * Mathf.PI) * value * curveEvaluate;
        }

        return offset;
    }
}


/*

    private IEnumerator InteractRoutine(RaycastHit raycastHit)
    {
        float hookShotSize = 0.0f;
        float hookShotThrowSpeed = 0.5f;

        // Enter
        Vector3 point = Vector3.zero;
        stateMachine.HookShootTransform.localScale = new Vector3(stateMachine.HookShootTransform.localScale.x, stateMachine.HookShootTransform.localScale.y, hookShotSize);
        AimAssist aimAssist = raycastHit.collider.gameObject.GetComponent<AimAssist>();
        stateMachine.HookShootTransform.gameObject.SetActive(true);

        float time = 0.0f;
        while (true)
        {
            if (aimAssist != null)
            {
                point = aimAssist.TargetPoint.position;
            }
            else
            {
                point = raycastHit.collider.gameObject.transform.position + (raycastHit.collider.gameObject.transform.position - raycastHit.point);
            }

            time += Time.deltaTime * hookShotThrowSpeed;
            hookShotSize += time;
            stateMachine.HookShootTransform.localScale = new Vector3(stateMachine.HookShootTransform.localScale.x, stateMachine.HookShootTransform.localScale.y, hookShotSize);
            stateMachine.HookShootTransform.LookAt(point);

            if (hookShotSize >= Vector3.Distance(stateMachine.transform.position, point))
            {
                Interact(raycastHit);
                break;
            }
            yield return null;
        }

        // Exit
        stateMachine.HookShootTransform.gameObject.SetActive(false);
        stateMachine.PlayerCamera.CameraEffectsReset();
        stateMachine.InteractionCoroutine = null;
    }
 
*/