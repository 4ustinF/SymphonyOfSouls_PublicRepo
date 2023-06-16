[System.Serializable]
public class KeyBindingsDefaults
{
    public KeyBinding Jump = null;
    public KeyBinding Dash = null;
    public KeyBinding Slide1 = null;
    public KeyBinding Interact = null;
    public KeyBinding HookShot = null;

    public KeyBindingsDefaults()
    {
        Jump = new KeyBinding("<Keyboard>/space");
        Dash = new KeyBinding("<Keyboard>/e");
        Slide1 = new KeyBinding("<Keyboard>/leftShift");
        Interact = new KeyBinding("<Mouse>/leftButton");
        HookShot = new KeyBinding("<Mouse>/rightButton");
    }
}
