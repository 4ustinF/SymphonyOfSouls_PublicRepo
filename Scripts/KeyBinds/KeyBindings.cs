using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class KeyBindings
{
    public KeyBinding Jump = null;
    public KeyBinding Interact = null;
    public KeyBinding Dash = null;
    public KeyBinding Slide1 = null;
    public KeyBinding HookShot = null;

    private KeyBindingsDefaults _keyBindingsDefaults = new KeyBindingsDefaults();

    public KeyBindings()
    {
        Jump = new KeyBinding();
        Interact = new KeyBinding();
        Dash = new KeyBinding();
        Slide1 = new KeyBinding();
        HookShot = new KeyBinding();
        _keyBindingsDefaults = new KeyBindingsDefaults();
    }

    public KeyBindings(KeyBindings rhs)
    {
        Jump = new KeyBinding(rhs.Jump);
        Interact = new KeyBinding(rhs.Interact);
        Dash = new KeyBinding(rhs.Dash);
        Slide1 = new KeyBinding(rhs.Slide1);
        HookShot = new KeyBinding(rhs.HookShot);

       // CheckIfDefaultsRequired();
    }

    public void UpdateBindings(KeyBindings rhs)
    {
        Jump = rhs.Jump;
        Interact = rhs.Interact;
        Dash = rhs.Dash;
        Slide1 = rhs.Slide1;
        HookShot = rhs.HookShot;

        CheckIfDefaultsRequired();
    }

    public void ResetToDefault()
    {
        Jump.SetValue(_keyBindingsDefaults.Jump.Value);
        Interact.SetValue(_keyBindingsDefaults.Interact.Value);
        Dash.SetValue(_keyBindingsDefaults.Dash.Value);
        Slide1.SetValue(_keyBindingsDefaults.Slide1.Value);
        HookShot.SetValue(_keyBindingsDefaults.HookShot.Value);
    }

    public void CheckIfDefaultsRequired()
    {
        if (string.IsNullOrEmpty(Jump.Value))
        {
            Jump = new KeyBinding(_keyBindingsDefaults.Jump.Value);
        }

        if (string.IsNullOrEmpty(Interact.Value))
        {
            Interact = new KeyBinding(_keyBindingsDefaults.Interact.Value);
        }

        if (string.IsNullOrEmpty(Dash.Value))
        {
            Dash = new KeyBinding(_keyBindingsDefaults.Dash.Value);
        }

        if (string.IsNullOrEmpty(Slide1.Value))
        {
            Slide1 = new KeyBinding(_keyBindingsDefaults.Slide1.Value);
        }

        if (string.IsNullOrEmpty(HookShot.Value))
        {
            HookShot = new KeyBinding(_keyBindingsDefaults.HookShot.Value);
        }
    }

    public void UpdateControls(Controls controls)
    {
        UpdateKeyBind(controls.Player.Jump, Jump.Value);
        UpdateKeyBind(controls.Player.Dash, Dash.Value);
        UpdateKeyBind(controls.Player.Slide1, Slide1.Value);
        UpdateKeyBind(controls.Player.Interact, Interact.Value);
        UpdateKeyBind(controls.Player.HookShot, HookShot.Value);
    }

    private void UpdateKeyBind(InputAction inputAction, string value)
    {
        inputAction.ChangeBinding(0).WithPath(value);
    }
}

[System.Serializable]
public class KeyBinding
{
    [JsonProperty]
    [SerializeField]
    private string _value = string.Empty;
    public string Value => _value;

    public KeyBinding()
    {

    }

    public KeyBinding(KeyBinding rhs)
    {
        _value = rhs.Value;
    }

    public KeyBinding(string buttonName)
    {
        _value = buttonName;
    }

    public void SetValue(string buttonName)
    {
        _value = buttonName;
    }
}