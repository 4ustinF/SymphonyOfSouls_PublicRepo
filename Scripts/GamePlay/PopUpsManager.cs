using UnityEngine;
using UnityEngine.InputSystem;

public class PopUpsManager : MonoBehaviour
{
    [SerializeField] private TutorialAreaTrigger _movement = null;
    [SerializeField] private TutorialAreaTrigger _jump = null;
    [SerializeField] private TutorialAreaTrigger _slide = null;
    [SerializeField] private TutorialAreaTrigger _wallRun = null;
    [SerializeField] private TutorialAreaTrigger _dash = null;
    [SerializeField] private TutorialAreaTrigger _shoot = null;
    [SerializeField] private TutorialAreaTrigger _grapple = null;

    private void Awake()
    {
        GameLoader.CallOnComplete(Initialize);
    }

    private void Initialize()
    {
        ServiceLocator.Get<EventBusCallbacks>().OnPlayerInputsChangedHandled += InputsChangedHandle;
    }

    private void OnDestroy()
    {
        ServiceLocator.Get<EventBusCallbacks>().OnPlayerInputsChangedHandled -= InputsChangedHandle;
    }

    private void InputsChangedHandle(OnPlayerInputsChangedMessage obj)
    {
        UpdatePopUpText(obj.NewControls);
    }

    // Finish controls key passing
    private void UpdatePopUpText(Controls controls)
    {
        UpdateMovementText();
        UpdateWallRunText();
        UpdateJumpText(controls.Player.Jump);
        UpdateSlideText(controls.Player.Slide1);
        UpdateDashText(controls.Player.Dash);
        UpdateInteractText(controls.Player.Interact);
        UpdateGrappleText(controls.Player.HookShot);
    }

    private void UpdateMovementText()
    {
        _movement.SetTipText($"'WASD' To Move");
    }

    private void UpdateWallRunText()
    {
        _wallRun.SetTipText($"Jump Onto Grid Walls To Wallrun");
    }

    private void UpdateJumpText(InputAction action)
    {
        _jump.SetTipText($"'{GetKeyName(action)}' To Jump");
    }

    private void UpdateSlideText(InputAction action)
    {
        _slide.SetTipText($"Hold '{GetKeyName(action)}' To Slide");
    }

    private void UpdateDashText(InputAction action)
    {
        _dash.SetTipText($"'{GetKeyName(action)}' To Dash");
    }

    private void UpdateInteractText(InputAction action)
    {
        _shoot.SetTipText($"'{GetKeyName(action)}' To Activate Golden Objects");
    }

    private void UpdateGrappleText(InputAction action)
    {
        _grapple.SetTipText($"'{GetKeyName(action)}' To Grapple To Green Objects");
    }

    private string GetKeyName(InputAction action)
    {
        string result = string.Empty;
        result = InputControlPath.ToHumanReadableString(action.bindings[0].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
        return result.ToUpper();
    }
}
