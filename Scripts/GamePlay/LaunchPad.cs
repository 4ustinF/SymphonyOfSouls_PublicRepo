using UnityEditor;
using UnityEngine;

public class LaunchPad : MonoBehaviour
{
    [SerializeField] private Transform _pivotDirection = null;
    [SerializeField] private float _minSpeed = 3.0f;
    [SerializeField] private float _minSpeedForceBonusMultiplier = 0.1f;

    [SerializeField] private float _launchForce = 150.0f;
    [SerializeField] private float _yLaunchMultiplier = 1.0f;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent<PlayerStateMachine>(out PlayerStateMachine player))
            {
                AkSoundEngine.PostEvent("Play_Object_JumpPad", gameObject);
                AddLaunchForce(player);
            }
        }
    }

    private void AddLaunchForce(PlayerStateMachine player)
    {
        Vector3 forceDiection = new Vector3(_pivotDirection.transform.forward.x, _pivotDirection.transform.forward.y * _yLaunchMultiplier, _pivotDirection.transform.forward.z);
        Vector3 forceImpulse = forceDiection * _launchForce;

        //if speed is low 
        if (player.RigidBody.velocity.magnitude <= _minSpeed)
        {
            // Adds default velocity along launch pad direction 10% of launch power
            player.RigidBody.AddForce(forceDiection * _launchForce * _minSpeedForceBonusMultiplier, ForceMode.VelocityChange);
        }

        player.RigidBody.AddForce(forceImpulse, ForceMode.Impulse);
    }

#if UNITY_EDITOR
    void DrawHandles(GizmoType gizmoType)
    {
        Handles.color = Color.cyan;
        Handles.ArrowHandleCap(EditorGUIUtility.GetControlID(FocusType.Passive), _pivotDirection.position + _pivotDirection.up * 0.2f, _pivotDirection.rotation, 2.0f, EventType.Repaint);
    }

    private void OnDrawGizmos()
    {
        DrawHandles(GizmoType.NotInSelectionHierarchy);
    }
#endif
}

