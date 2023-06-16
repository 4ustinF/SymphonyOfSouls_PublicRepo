using UnityEngine;

namespace SpawningSystem
{
    public class CheckPoint : MonoBehaviour
    {
        [field: SerializeField] public bool IsDiscovered { private set;  get; }
        [field: SerializeField] public bool IsInitalPoint { private set; get; }
        [field: SerializeField] public int Index { private set; get; }

        private void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("Player"))
            {
                IsDiscovered = true;
            }
        }

        public void ResetPoint()
        {
            IsDiscovered = false;
        }
    }
}
