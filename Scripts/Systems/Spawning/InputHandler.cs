using UnityEngine;
using System;

namespace SpawningSystem
{
    public class InputHandler : MonoBehaviour
    {
        [field: SerializeField] public KeyCode Next { private set; get; }
        [field: SerializeField] public KeyCode Previous { private set; get; }
        [field: SerializeField] public KeyCode Confirm { private set; get; }
        [field: SerializeField] public KeyCode Respawn { private set; get; }
        [field: SerializeField] public KeyCode UnlockCursor { private set; get; }
        [field: SerializeField] public KeyCode CreateCustomPointKey { private set; get; }
        [field: SerializeField] public KeyCode TeleportCustomPointKey { private set; get; }

        public event Action onNextPoint;
        public event Action onPointConfirmed;
        public event Action onPreviousPoint;
        public event Action onRespawn;
        public event Action onUnlockCursor;
        public event Action onCreateCustomPoint;
        public event Action onTeleportToCustomPoint;

        void Update()
        {
            if (Input.GetKeyDown(Next))
            {
                onNextPoint?.Invoke();
            }
            else if (Input.GetKeyDown(Previous))
            {
                onPreviousPoint?.Invoke();

            }
            else if (Input.GetKeyDown(Confirm))
            {
                onPointConfirmed?.Invoke();

            }
            else if (Input.GetKeyDown(Respawn))
            {
                onRespawn?.Invoke();
            }
            else if (Input.GetKeyDown(UnlockCursor))
            {
                onUnlockCursor?.Invoke();
            }
            else if (Input.GetKeyDown(CreateCustomPointKey))
            {
                onCreateCustomPoint?.Invoke();
            }
            else if (Input.GetKeyDown(TeleportCustomPointKey))
            {
                onTeleportToCustomPoint?.Invoke();
            }
        }
    }
}