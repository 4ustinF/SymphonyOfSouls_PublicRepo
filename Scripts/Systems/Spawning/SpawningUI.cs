using TMPro;
using UnityEngine;

namespace SpawningSystem
{
    public class SpawningUI : MonoBehaviour
    {
        [SerializeField] private InputHandler _inputHandler = null;
        [SerializeField] private SpawningManager _spawningManager = null;
        [SerializeField] private TMP_Text _controlInforText = null;
        [SerializeField] private TMP_Dropdown _spawningPoints_Dropdown = null;

        private void OnEnable()
        {
            _inputHandler.onUnlockCursor += UnlockCursorUI;
            _inputHandler.onNextPoint += NextPoint;
            _inputHandler.onPreviousPoint += PreviousPoint;
            _spawningPoints_Dropdown.onValueChanged.AddListener(DropDownValueChanged);
            UpdateDropdown();
        }

        private void OnDisable()
        {
            _inputHandler.onUnlockCursor -= UnlockCursorUI;
            _inputHandler.onNextPoint -= NextPoint;
            _inputHandler.onPreviousPoint -= PreviousPoint;
            _spawningPoints_Dropdown.onValueChanged.RemoveListener(DropDownValueChanged);
        }

        private void Awake()
        {
            _controlInforText.text = $"'{_inputHandler.Respawn}' - Respawn\n'{_inputHandler.Next}' - Select Next\n'{_inputHandler.Previous}' - Select Prev\n'{_inputHandler.Confirm}' - Spawn at Selected\n'{_inputHandler.UnlockCursor}' - UnlockCursor\n'{_inputHandler.CreateCustomPointKey}' - CreateCustomPoint\n'{_inputHandler.TeleportCustomPointKey}' - TeleportCustomPoint";
            //Cursor.lockState = CursorLockMode.None;
        }

        private void Update()
        {
            //if (Input.GetKeyDown(KeyCode.Mouse0))
            //{
            //    Cursor.lockState = CursorLockMode.Locked;
            //}
        }

        private void NextPoint()
        {
            _spawningPoints_Dropdown.value = (_spawningPoints_Dropdown.value + 1) % (_spawningPoints_Dropdown.options.Count);
            _spawningPoints_Dropdown.onValueChanged.Invoke(_spawningPoints_Dropdown.value);
        }

        private void PreviousPoint()
        {
            if (_spawningPoints_Dropdown.value == 0)
            {
                _spawningPoints_Dropdown.value = _spawningPoints_Dropdown.options.Count - 1;
            }
            else
            {
                _spawningPoints_Dropdown.value = _spawningPoints_Dropdown.value - 1;
            }

            _spawningPoints_Dropdown.onValueChanged.Invoke(_spawningPoints_Dropdown.value);
        }

        private void UpdateDropdown()
        {
            _spawningPoints_Dropdown.ClearOptions();
            var options = _spawningManager.GetSpawningPoints();
            foreach (var x in options)
            {
                _spawningPoints_Dropdown.options.Add(new TMP_Dropdown.OptionData(x.pointName));
            }
            _spawningPoints_Dropdown.value = 0;
            _spawningPoints_Dropdown.onValueChanged.Invoke(0);
        }

        private void UnlockCursorUI()
        {
            //Cursor.lockState = CursorLockMode.None;
        }

        private void DropDownValueChanged(int value)
        {
            //Cursor.lockState = CursorLockMode.Locked;
            _spawningManager.SelectSpawningPoint(value);
        }
    }
}