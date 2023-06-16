using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace SpawningSystem
{
    public class SpawningManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private InputHandler _inputHandler = null;
        [SerializeField] private List<SpawningPoint> _spawningPoints = new List<SpawningPoint>();
        [SerializeField] private List<CheckPoint> _checkPoints = new List<CheckPoint>();
        [SerializeField] private PlayerStateMachine _player = null;

        [Header("Settings")]
        [SerializeField] private bool isVisualizeCheckPointsTriggers = false;
        [SerializeField] private bool isVisualizeSpawningPoints = false;
        [SerializeField] private float _minYBound = 0.0f;

        [Header("Sequences")]
        public UnityEvent onOutOfBonds;

        private CheckPoint _lastCheckPoint = null;
        private GameObject _customPoint = null;
        private SpawningPoint _selecedSpawnningPoint = null;
        private int _ignoreRaycastLayerID = 2; //Ignore Raycast Layer

        private void OnEnable()
        {
          //  _ignoreRaycastLayerID = LayerMask.NameToLayer("Ignore Raycast");
            _inputHandler.onRespawn += TeloportToSelctedSpawningPoint;
            _inputHandler.onCreateCustomPoint += CreateCustomPoint;
            _inputHandler.onTeleportToCustomPoint += TeleportCustomPoint;
        }

        private void OnDisable()
        {
            _inputHandler.onRespawn -= TeloportToSelctedSpawningPoint;
            _inputHandler.onCreateCustomPoint -= CreateCustomPoint;
            _inputHandler.onTeleportToCustomPoint -= TeleportCustomPoint;
        }

        private void OnDestroy()
        {
            ServiceLocator.Get<EventBusCallbacks>().OnKillBoxTriggerMessageHandled -= PlayerKilledHandle;
        }

        private void Awake()
        {
            LevelLoader.CallOnComplete(Initialize);
        }

        private void Initialize()
        {
            ServiceLocator.Get<EventBusCallbacks>().OnKillBoxTriggerMessageHandled += PlayerKilledHandle;
            RespawnOnInitialPoint();
        }

        void FixedUpdate()
        {
            if (_player.transform.position.y < _minYBound)
            {
                // Player is out of bounds 
                if (onOutOfBonds == null)
                {
                    Debug.LogWarning("SpawningManager-> Your OnOutOfBondsSequence is empty!");
                }
                else
                {
                    onOutOfBonds?.Invoke();
                    var messenger = ServiceLocator.Get<IEventBusSystemHub>();
                    messenger.Publish(new OnKillBoxTrigger(this));
                }
            }
        }

        private void PlayerKilledHandle(OnKillBoxTrigger x)
        {
            FreezePlayer(0.1f);
            RespawnOnLastDiscoveredCheckPoint();
        }

        private void TeloportToSelctedSpawningPoint()
        {
            TeleportPlayer(_selecedSpawnningPoint.transform.position,_selecedSpawnningPoint.transform.forward);
        }

        public void ResetAllCheckPoints()
        {
            _checkPoints.ForEach(x => x.ResetPoint());
        }

#if UNITY_EDITOR
        public void CreateNewCheckPoint()
        {
            GameObject temp = Instantiate(Resources.Load("SpawningSystem/CheckPoint") as GameObject);
            temp.name = "NEW Check Point";
            temp.transform.SetParent(this.gameObject.transform.Find("CheckPoints"));
            temp.layer = _ignoreRaycastLayerID;
            _checkPoints.Add(temp.GetComponent<CheckPoint>());
            Selection.objects = new Object[] { temp };
            EditorUtility.SetDirty(this);
        }

        public void CreateNewSpawningPoint()
        {
            GameObject temp = Instantiate(Resources.Load("SpawningSystem/SpawningPoint") as GameObject);
            temp.name = "NEW Spawning Point";
            temp.transform.SetParent(this.gameObject.transform.Find("SpawningPoints"));
            temp.layer = _ignoreRaycastLayerID;
            _spawningPoints.Add(temp.GetComponent<SpawningPoint>());
            Selection.objects = new Object[] { temp };
            EditorUtility.SetDirty(this);
        }
        public void FixHierarchy()
        {
            _checkPoints.Clear();
            _spawningPoints.Clear();

            var checkTransform = this.gameObject.transform.Find("CheckPoints");
            for (int i = 0; i < checkTransform.childCount; ++i)
            {
                var checkPoint = checkTransform.GetChild(i).GetComponent<CheckPoint>();
                _checkPoints.Add(checkPoint);
                checkPoint.gameObject.layer = _ignoreRaycastLayerID;
            }

            var spawningTransform = this.gameObject.transform.Find("SpawningPoints");
            for (int i = 0; i < spawningTransform.childCount; ++i)
            {
                var spawningPoint = spawningTransform.GetChild(i).GetComponent<SpawningPoint>();
                _spawningPoints.Add(spawningPoint);
                spawningPoint.gameObject.layer = _ignoreRaycastLayerID;
            }
            EditorUtility.SetDirty(this);
        }
#endif

        public void SelectSpawningPoint(int value)
        {
            _selecedSpawnningPoint = _spawningPoints[value];
        }

        private void TeleportPlayer(Vector3 toPosition, Vector3 toRotation)
        {
            _player.transform.position = toPosition;
            _player.PlayerCamera.AdjustRotation(Quaternion.LookRotation(toRotation));
        }

        private void RespawnOnCheckPoint()
        {
            TeleportPlayer(_lastCheckPoint.transform.position, _lastCheckPoint.transform.forward);
        }

        private void CreateCustomPoint()
        {
            if (_customPoint != null)
            {
                Destroy(_customPoint.gameObject);
                _customPoint = null;
                return;
            }

            GameObject temp = Instantiate(Resources.Load("SpawningSystem/CustomTestPoint") as GameObject);
            temp.name = "Custom Teleport Point";
            temp.transform.SetParent(this.transform);
            temp.transform.position = _player.transform.position;
            _customPoint = temp;
        }

        private void TeleportCustomPoint()
        {
            if (_customPoint == null)
            {
                Debug.LogWarning("You have to create a Custom Point before trying to teleport to It!");
                return;
            }
            TeleportPlayer(_customPoint.transform.position, _customPoint.transform.forward);
        }

        public void RespawnOnInitialPoint()
        {
            var initialPoint = _checkPoints.Find(x => x.IsInitalPoint);
            TeleportPlayer(initialPoint.transform.position, initialPoint.transform.forward);
        }

        public void RespawnOnLastDiscoveredCheckPoint()
        {
            int highestIndex = -1;

            for (int i = 0; i < _checkPoints.Count; ++i)
            {
                if (_checkPoints[i].IsDiscovered == false)
                {
                    continue;
                }

                if (_checkPoints[i].Index > highestIndex)
                {
                    highestIndex = i;
                }
            }

            if (highestIndex == -1)
            {
                Debug.LogError("Set Up your indecies for Check Points.");
                return;
            }
            var checkPointTransform = _checkPoints[highestIndex].gameObject.transform;
            TeleportPlayer(checkPointTransform.position, checkPointTransform.forward);
        }

        public IEnumerable<SpawningPoint> GetSpawningPoints()
        {
            return _spawningPoints;
        }

        public void FreezePlayer(float time)
        {
            Debug.Log($"YOU FROZEN! FOR {time}s");
            var rb = _player.GetComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezePosition;
            StartCoroutine(UnFreeze(rb, time));
        }

        IEnumerator UnFreeze(Rigidbody rb, float time)
        {
            yield return new WaitForSeconds(time);
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            Debug.Log("You are unfrozen!");
        }

#if UNITY_EDITOR
        private void _OnValidate()
        {
            if (this == null) return;
            UpdateCheckPointTriggersVisualisation();
            UpdateSpawningPointsVisualisation();
        }

        private void OnValidate()
        {
            UnityEditor.EditorApplication.delayCall += _OnValidate;
        }
#endif
        private void UpdateCheckPointTriggersVisualisation()
        {
            foreach (var point in _checkPoints)
            {
                point.GetComponent<MeshRenderer>().enabled = isVisualizeCheckPointsTriggers;
            }
        }
        private void UpdateSpawningPointsVisualisation()
        {
            foreach (var point in _spawningPoints)
            {
                point.GetComponent<MeshRenderer>().enabled = isVisualizeSpawningPoints;
            }
        }
    }
}
