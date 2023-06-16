using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModularSoundEffectPlayer : MonoBehaviour
{
    [SerializeField] Enums.ObjectTypeSFX _objectType = Enums.ObjectTypeSFX.None;
    private IWwiseManager _wwiseManager = null;

    private void Awake()
    {
        LevelLoader.CallOnComplete(Initialize);
    }

    private void Initialize()
    {
        _wwiseManager = ServiceLocator.Get<IWwiseManager>();
    }

    public void Play()
    {
        switch (_objectType)
        {
            case Enums.ObjectTypeSFX.None:
                Debug.LogError("ModularSoundEffectPlayer: Not assigned an Enum type");
                break;
            case Enums.ObjectTypeSFX.DefaultDoor:
                _wwiseManager.PlayDoorSoundEffect(Enums.DoorTypeSFX.Default);
                break;
            case Enums.ObjectTypeSFX.LightDoor:
                _wwiseManager.PlayDoorSoundEffect(Enums.DoorTypeSFX.Light);
                break;
            case Enums.ObjectTypeSFX.HeavyDoor:
                _wwiseManager.PlayDoorSoundEffect(Enums.DoorTypeSFX.Heavy);
                break;
            case Enums.ObjectTypeSFX.BossDoor:
                _wwiseManager.PlayDoorSoundEffect(Enums.DoorTypeSFX.Boss);
                break;
            case Enums.ObjectTypeSFX.ExplosiveDoor:
                _wwiseManager.PlayDoorSoundEffect(Enums.DoorTypeSFX.Explosive);
                break;
            case Enums.ObjectTypeSFX.Vine:
                _wwiseManager.PlayDoorSoundEffect(Enums.DoorTypeSFX.Vine);
                break;
            case Enums.ObjectTypeSFX.MetalGrate:
                _wwiseManager.PlayMetalGrateSoundEffect();
                break;
            case Enums.ObjectTypeSFX.Flag:
                _wwiseManager.PlayFlagDropSoundEffect();
                break;
            case Enums.ObjectTypeSFX.WallAppear:
                _wwiseManager.PlayWallAppearingSoundEffect();
                break;
            case Enums.ObjectTypeSFX.StatueAppear:
                _wwiseManager.PlayStatueAppearingSoundEffect();
                break;
            case Enums.ObjectTypeSFX.GrappAppear:
                _wwiseManager.PlayGrappAppearingSoundEffect();
                break;
        }
    }
}