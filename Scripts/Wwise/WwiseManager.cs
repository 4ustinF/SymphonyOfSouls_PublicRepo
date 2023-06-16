using UnityEngine;

public class WwiseManager : MonoBehaviour, IWwiseManager
{
    [SerializeField] WwiseMusicManager _musicManager = null;
    [SerializeField] FloatValue _masterVolume = null;
    [SerializeField] FloatValue _musicVolume = null;
    [SerializeField] FloatValue _sfxVolume = null;

    private Enums.SceneType _sceneType = Enums.SceneType.None;

    public Enums.SceneType SceneType { private set { } get => _sceneType; }

    private bool _menuBankLoaded = false;

    public WwiseManager Initialize()
    {
        _masterVolume.Changed += ModifyMasterVolume;
        _musicVolume.Changed += ModifyMusicVolume;
        _sfxVolume.Changed += ModifySFXVolume;

        ModifyMasterVolume();
        ModifyMusicVolume();
        ModifySFXVolume();

        LevelController.onPause += PauseMusicEvent;
        LevelController.onPause += PauseSoundEffects;

        _musicManager.Initialize();

        return this;
    }

    public void SetUpLevelAudio()
    {
        LoadGeneralSoundbank();
        LoadSpecificSoundbank();
        SetUpMusicManager();
    }

    public void UnloadSoundBanks()
    {
        UnloadGeneralSoundbank();
        UnloadSpecificSoundbank();
    }

    public void SetUpMusicManager()
    {
        AkSoundEngine.SetState("PlayerLife", "Alive");
        AkSoundEngine.SetState("StreakLevel", "Base");
        PlayMusicEvent();
    }

    private void OnDestroy()
    {
        UnloadSoundBanks();
        _masterVolume.Changed -= ModifyMasterVolume;
        _musicVolume.Changed -= ModifyMusicVolume;
        _sfxVolume.Changed -= ModifySFXVolume;
        LevelController.onPause -= PauseMusicEvent;
        LevelController.onPause -= PauseSoundEffects;
    }

    public void PlayMusicEvent()
    {
        _musicManager.PlayMusicEvent();
    }

    public void StopMusicEvent()
    {
        _musicManager.StopMusicEvent();
    }

    public void PauseMusicEvent()
    {
        if (LevelController.IsPaused)
        {
            _musicManager.PauseMusicEvent();
        }
        else
        {
            ResumeMusicEvent();
        }
    }

    public void ResumeMusicEvent()
    {
        _musicManager.ResumeMusicEvent();
    }

    public void StartAudioDelayOptimization()
    {
        _musicManager.StartAudioDelayOptimization();
    }

    public void StopAudioDelayOptimization()
    {
        _musicManager.StopAudioDelayOptimization();
    }

    public void LoadGeneralSoundbank()
    {
        AkBankManager.LoadBank("General", false, false);
        Debug.Log("Loaded general SoundBank");
    }

    public void LoadSpecificSoundbank()
    {
        _sceneType = ServiceLocator.Get<LevelLoader>().GetSceneType();

        switch (_sceneType)
        {
            case Enums.SceneType.MainMenu:
                if (_menuBankLoaded)
                {
                    break;
                }
                AkBankManager.LoadBank("MainMenu", false, false);
                AkSoundEngine.SetSwitch("LevelSwitch", "NoLevel", gameObject);
                _menuBankLoaded = true;
                break;
            case Enums.SceneType.LevelSelect:
                if (!_menuBankLoaded)
                {
                    AkBankManager.LoadBank("MainMenu", false, false);
                    AkSoundEngine.SetSwitch("LevelSwitch", "NoLevel", gameObject);
                }
                break;
            case Enums.SceneType.CutScene:
                AkBankManager.LoadBank("CutScene", false, false);
                AkSoundEngine.SetSwitch("LevelSwitch", "NoLevel", gameObject);
                break;
            case Enums.SceneType.Level01:
                AkBankManager.LoadBank("Level01", false, false);
                AkSoundEngine.SetSwitch("LevelSwitch", "LevelOne", gameObject);
                break;
            case Enums.SceneType.Level02:
                AkBankManager.LoadBank("Level02", false, false);
                AkSoundEngine.SetSwitch("LevelSwitch", "LevelTwo", gameObject);
                break;
        }

        Debug.Log("Loaded specific SoundBank");

        SetupSceneType();
    }

    public void UnloadGeneralSoundbank()
    {
        AkBankManager.UnloadBank("General");
        Debug.Log("Unloaded general SoundBank");
    }

    public void UnloadSpecificSoundbank()
    {
        switch (_sceneType)
        {
            case Enums.SceneType.MainMenu:
                AkBankManager.UnloadBank("MainMenu");
                _menuBankLoaded = false;
                break;
            case Enums.SceneType.LevelSelect:
                AkBankManager.UnloadBank("MainMenu");
                _menuBankLoaded = false;
                break;
            case Enums.SceneType.CutScene:
                AkBankManager.UnloadBank("CutScene");
                _menuBankLoaded = false;
                break;
            case Enums.SceneType.Level01:
                AkBankManager.UnloadBank("Level01");
                break;
            case Enums.SceneType.Level02:
                AkBankManager.UnloadBank("Level02");
                break;
        }

        Debug.Log("Unloaded specific SoundBank");
    }

    public void SetupSceneType()
    {
        _musicManager.SceneType = _sceneType;
        AkSoundEngine.SetState("GameState", "Gameplay");
    }

    public void ModifyMasterVolume()
    {
        AkSoundEngine.SetRTPCValue("MasterVolume", _masterVolume.GetValue);
    }

    public void ModifyMusicVolume()
    {
        AkSoundEngine.SetRTPCValue("MxVolume", _musicVolume.GetValue);
    }

    public void ModifySFXVolume()
    {
        AkSoundEngine.SetRTPCValue("SFXVolume", _sfxVolume.GetValue);
    }

    public void SetRunningSwitch()
    {
        AkSoundEngine.SetSwitch("Run_Surface", "Ground", gameObject);
    }

    public void SetWallRunningSwitch()
    {
        AkSoundEngine.SetSwitch("Run_Surface", "Wall", gameObject);
    }

    public void PlayFootSteps()
    {
        AkSoundEngine.PostEvent("Play_Player_Movement", gameObject);
    }

    public void PlayWallRunFootSteps()
    {
        AkSoundEngine.PostEvent("Play_Player_Grapple_Launch", gameObject);
    }

    public void PlayAttackSoundEfect()
    {
        AkSoundEngine.PostEvent("Play_Player_Attack", gameObject);
    }

    public void PlayClimbingSoundEffect()
    {
        AkSoundEngine.PostEvent("Play_Player_Climbing", gameObject);
    }

    public void PlayDashSoundEffect()
    {
        AkSoundEngine.PostEvent("Play_Player_Dash", gameObject);
    }

    public void PlayGrappleLaunchSoundEffect()
    {
        AkSoundEngine.PostEvent("Play_Player_Grapple_Launch", gameObject);
    }

    public void PlayGrapplePullSoundEffect()
    {
        AkSoundEngine.PostEvent("Play_Player_Grapple_Pull", gameObject);
    }

    public void PlayGrappleRopeSoundEffect()
    {
        AkSoundEngine.PostEvent("Play_Player_Grapple_Rope", gameObject);
    }

    public void StopGrappleSoundEffect()
    {
        AkSoundEngine.PostEvent("Stop_Player_Grapple_Pull", gameObject);
    }

    public void PlayJumpingSoundEffect()
    {
        AkSoundEngine.PostEvent("Play_Player_Jump", gameObject);
    }

    public void PlayLedgeGrabSoundEffect()
    {
        AkSoundEngine.PostEvent("Play_Player_LedgeGrab", gameObject);
    }

    public void PlaySlideSoundEffect()
    {
        AkSoundEngine.PostEvent("Play_Player_Slide", gameObject);
    }

    public void ModifyMusicIntensity(Tier tier)
    {
        switch (tier.TierType)
        {
            case Enums.MultiplierTier.Tier1:
                AkSoundEngine.SetState("StreakLevel", "Base");
                break;
            case Enums.MultiplierTier.Tier2:
                AkSoundEngine.SetState("StreakLevel", "Low");
                break;
            case Enums.MultiplierTier.Tier3:
                AkSoundEngine.SetState("StreakLevel", "Mid");
                break;
            case Enums.MultiplierTier.Tier4:
                AkSoundEngine.SetState("StreakLevel", "High");
                break;
            case Enums.MultiplierTier.Tier5:
                AkSoundEngine.SetState("StreakLevel", "Full");
                break;
        }
    }

    public void PauseSoundEffects()
    {
        if (LevelController.IsPaused)
        {
            AkSoundEngine.PostEvent("Pause_All_SFX", gameObject);
        }
        else
        {
            ResumeSoundEffects();
        }
    }

    public void ResumeSoundEffects()
    {
        AkSoundEngine.PostEvent("Resume_All_SFX", gameObject);
    }

    public void StopSoundEffects()
    {
        AkSoundEngine.PostEvent("Stop_All_SFX", gameObject);
    }

    public void PlaySlidingObjectSoundEffect()
    {
        AkSoundEngine.PostEvent("Play_Object_Sliding", gameObject);
    }

    public void PlayDoorSoundEffect(Enums.DoorTypeSFX doorType)
    {
        switch (doorType)
        {
            case Enums.DoorTypeSFX.Default:
                AkSoundEngine.PostEvent("Play_Object_Open_Door", gameObject);
                break;
            case Enums.DoorTypeSFX.Light:
                AkSoundEngine.PostEvent("Play_Object_RegularDoor", gameObject);
                break;
            case Enums.DoorTypeSFX.Heavy:
                AkSoundEngine.PostEvent("Play_Object_HeavyDoor", gameObject);
                break;
            case Enums.DoorTypeSFX.Boss:
                AkSoundEngine.PostEvent("Play_Object_BossDoor", gameObject);
                break;
            case Enums.DoorTypeSFX.Explosive:
                AkSoundEngine.PostEvent("Play_Object_DoorExplosion", gameObject);
                break;
            case Enums.DoorTypeSFX.Vine:
                AkSoundEngine.PostEvent("Play_Object_VineExplosion", gameObject);
                break;
        }
    }

    public void PlayMetalGrateSoundEffect()
    {
        AkSoundEngine.PostEvent("Play_Object_MetalGrate", gameObject);
    }

    public void PlayVaseBreakingSoundEffect()
    {
        AkSoundEngine.PostEvent("Play_Object_VaseBreaking", gameObject);
    }

    public void PlayFlagDropSoundEffect()
    {
        AkSoundEngine.PostEvent("Play_Object_FlagDrop", gameObject);
    }

    public void PlayGrapplePointBreakSoundEffect()
    {
        AkSoundEngine.PostEvent("Play_Object_GrapplePointBreak", gameObject);
    }

    public void PlayStatueHitSoundEffect()
    {
        AkSoundEngine.PostEvent("Play_Object_StatueHit", gameObject);
    }

    public void PlayWallAppearingSoundEffect()
    {
        AkSoundEngine.PostEvent("Play_Object_WallAppear", gameObject);
    }

    public void PlayStatueAppearingSoundEffect()
    {
        AkSoundEngine.PostEvent("Play_Object_StatueAppear", gameObject);
    }

    public void PlayGrappAppearingSoundEffect()
    {
        AkSoundEngine.PostEvent("Play_Object_GrappablePointAppear", gameObject);
    }

    public void PlayUIScrollUpwardSoundEffect()
    {
        AkSoundEngine.PostEvent("Play_UI_ScrollMenuUp", gameObject);
    }

    public void PlayUIScrollDownwardSoundEffect()
    {
        AkSoundEngine.PostEvent("Play_UI_ScrollMenuDown", gameObject);
    }

    public void PlayCutScene(Enums.CutScene cutScene)
    {
        switch (cutScene)
        {
            case Enums.CutScene.LevelOne:
                AkSoundEngine.PostEvent("Play_CutScene_01", gameObject);
                break;
            case Enums.CutScene.LevelTwo:
                AkSoundEngine.PostEvent("Play_CutScene_02", gameObject);
                break;
            case Enums.CutScene.LevelThree:
                AkSoundEngine.PostEvent("Play_CutScene_03", gameObject);
                break;
        }
    }
}