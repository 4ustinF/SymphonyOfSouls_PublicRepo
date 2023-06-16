using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputActionRebindingExtensions;

public class ControlsMapping : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DataAsset _dataAsset = null;
    [SerializeField] private InputActionAsset controls = null;
    [SerializeField] private Button _backButton = null;
    [SerializeField] private Button _resetButton = null;
    [SerializeField] private SettingsUI _settingsUI = null;

    [SerializeField] private ControlButtonTemplate _jumpTemplate = null;
    [SerializeField] private ControlButtonTemplate _dashTemplate = null;
    [SerializeField] private ControlButtonTemplate _slide1Template = null;
    [SerializeField] private ControlButtonTemplate _interactTemplate = null;
    [SerializeField] private ControlButtonTemplate _hookShotTemplate = null;

    private InputAction _jumpInputAction = null;
    private InputAction _dashInputAction = null;
    private InputAction _slide1InputAction = null;
    private InputAction _interactInputAction = null;
    private InputAction _hookShotInputAction = null;

    private Dictionary<InputAction, KeyBinding> _keyDictionary = new Dictionary<InputAction, KeyBinding>();
    private Dictionary<InputAction, ControlButtonTemplate> _uiDictionary = new Dictionary<InputAction, ControlButtonTemplate>();
    private KeyBindings _keyBindingsCopy = null;
    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;

    private void OnEnable()
    {
        var actionMap = controls.FindActionMap("Player");
        _jumpInputAction = actionMap.FindAction("Jump");
        _dashInputAction = actionMap.FindAction("Dash");
        _slide1InputAction = actionMap.FindAction("Slide1");
        _interactInputAction = actionMap.FindAction("Interact");
        _hookShotInputAction = actionMap.FindAction("HookShot");

        _keyBindingsCopy = new KeyBindings(_dataAsset.KeyBindings);

        _keyDictionary.Add(_jumpInputAction, _keyBindingsCopy.Jump);
        _keyDictionary.Add(_dashInputAction, _keyBindingsCopy.Dash);
        _keyDictionary.Add(_slide1InputAction, _keyBindingsCopy.Slide1);
        _keyDictionary.Add(_interactInputAction, _keyBindingsCopy.Interact);
        _keyDictionary.Add(_hookShotInputAction, _keyBindingsCopy.HookShot);

        _uiDictionary.Add(_jumpInputAction, _jumpTemplate);
        _uiDictionary.Add(_dashInputAction, _dashTemplate);
        _uiDictionary.Add(_slide1InputAction, _slide1Template);
        _uiDictionary.Add(_interactInputAction, _interactTemplate);
        _uiDictionary.Add(_hookShotInputAction, _hookShotTemplate);

        UpdateButtonText();

        // Add Listeners
        _jumpTemplate.Button.onClick.AddListener(delegate { StartRebinding(_jumpInputAction, _keyBindingsCopy.Jump); });
        _dashTemplate.Button.onClick.AddListener(delegate { StartRebinding(_dashInputAction, _keyBindingsCopy.Dash); });
        _slide1Template.Button.onClick.AddListener(delegate { StartRebinding(_slide1InputAction, _keyBindingsCopy.Slide1); });
        _interactTemplate.Button.onClick.AddListener(delegate { StartRebinding(_interactInputAction, _keyBindingsCopy.Interact); });
        _hookShotTemplate.Button.onClick.AddListener(delegate { StartRebinding(_hookShotInputAction, _keyBindingsCopy.HookShot); });
        _resetButton.onClick.AddListener(ResetCopyModel);
    }

    private void ResetCopyModel()
    {
        _keyBindingsCopy.ResetToDefault();
        UpdateButtonText();
        _backButton.interactable = true;
    }

    private void OnDisable()
    {
        _jumpTemplate.Button.onClick.RemoveAllListeners();
        _dashTemplate.Button.onClick.RemoveAllListeners();
        _slide1Template.Button.onClick.RemoveAllListeners();
        _interactTemplate.Button.onClick.RemoveAllListeners();
        _hookShotTemplate.Button.onClick.RemoveAllListeners();

        _resetButton.onClick.RemoveListener(ResetCopyModel);

        if (!IsAnyEmptyBindings())
        {
            UpdateDataAsset();
        }

        foreach (var inputAction in _uiDictionary)
        {
            inputAction.Value.UpdateWarningStatus(false);
        }

        _keyDictionary.Clear();
        _uiDictionary.Clear();
    }

    InputAction _currentlySelectedInput = null;
    private void StartRebinding(InputAction inputAction, KeyBinding binding)
    {
        SetButtons(false);
        _backButton.interactable = false;

        inputAction.Disable();
        _settingsUI.DisableTabButtons(Enums.TabButtons.None);

        _currentlySelectedInput = inputAction;
        _uiDictionary[inputAction].UpdateWarningStatus(true, true);

        rebindingOperation = inputAction.PerformInteractiveRebinding()
            //.WithControlsExcluding("Mouse")
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(operation => RebindComplete(inputAction, binding))
            .Start();
    }

    private void RebindComplete(InputAction inputAction, KeyBinding key)
    {
        // Dispose the memory created for rebinding meaning DO NOT DELETE
        rebindingOperation.Dispose();

        key.SetValue(inputAction.bindings[0].effectivePath);
        inputAction.Enable();

        SetButtons(true);
        _backButton.interactable = true;
        _settingsUI.EnableTabButtons(Enums.TabButtons.ControlsSettingsBTN);

        InputAction dropedInput = new InputAction();
        if (CheckForInputsOverlap(inputAction, out dropedInput))
        {
            StartCoroutine(WaitTillDropedInputAssigned(dropedInput));
        }

        UpdateButtonText();

        _uiDictionary[_currentlySelectedInput].UpdateWarningStatus(false);
    }

    private void UpdateDataAsset()
    {
        _dataAsset.KeyBindings = new KeyBindings(_keyBindingsCopy);
        _dataAsset.onModified?.Invoke(); // Save dataAsset
    }

    private void UpdateButtonText()
    {
        _jumpTemplate.TextElement.text = GetActionName(_keyBindingsCopy.Jump.Value);
        _dashTemplate.TextElement.text = GetActionName(_keyBindingsCopy.Dash.Value);
        _slide1Template.TextElement.text = GetActionName(_keyBindingsCopy.Slide1.Value);
        _interactTemplate.TextElement.text = GetActionName(_keyBindingsCopy.Interact.Value);
        _hookShotTemplate.TextElement.text = GetActionName(_keyBindingsCopy.HookShot.Value);
    }

    private string GetActionName(string keyPath)
    {
        return InputControlPath.ToHumanReadableString(keyPath, InputControlPath.HumanReadableStringOptions.OmitDevice);
    }

    private void SetButtons(bool interactable)
    {
        _resetButton.interactable = interactable;
        _jumpTemplate.Button.interactable = interactable;
        _dashTemplate.Button.interactable = interactable;
        _slide1Template.Button.interactable = interactable;
        _interactTemplate.Button.interactable = interactable;
        _hookShotTemplate.Button.interactable = interactable;
    }

    private bool CheckForInputsOverlap(InputAction newlyBindedAction, out InputAction droppedAction)
    {
        string newBinding = _keyDictionary[newlyBindedAction].Value;

        foreach (var action in _keyDictionary)
        {
            if (action.Key == newlyBindedAction)
            {
                continue;
            }

            string actionBind = action.Value.Value;
            if (string.Compare(actionBind, newBinding, System.StringComparison.OrdinalIgnoreCase) == 0)
            {
                if (_keyDictionary.ContainsKey(action.Key))
                {
                    action.Key.ChangeBinding(0).WithPath(string.Empty);
                    _keyDictionary[action.Key].SetValue(string.Empty);
                    droppedAction = action.Key;
                    return true;
                }
            }
        }

        droppedAction = null;
        return false;
    }

    private IEnumerator WaitTillDropedInputAssigned(InputAction droppedInput)
    {
        _backButton.interactable = false;
        _uiDictionary[droppedInput].UpdateWarningStatus(true);

        while (string.IsNullOrEmpty(_keyDictionary[droppedInput].Value))
        {
            yield return null;
        }

        _uiDictionary[droppedInput].UpdateWarningStatus(false);
        if (!IsAnyEmptyBindings())
        {
            _backButton.interactable = true;
        }
    }

    public bool IsAnyEmptyBindings()
    {
        foreach (var inputAction in _keyDictionary)
        {
            if (string.IsNullOrEmpty(inputAction.Value.Value))
            {
                return true;
            }
        }

        return false;
    }
}
