public interface IWwiseManager
{
    // Initialization Methods
    void SetUpLevelAudio();
    void UnloadSoundBanks();
    void SetUpMusicManager();
    void LoadGeneralSoundbank();
    void UnloadGeneralSoundbank();
    void LoadSpecificSoundbank();
    void UnloadSpecificSoundbank();
    void SetupSceneType();
    void StartAudioDelayOptimization();
    void StopAudioDelayOptimization();

    // Music Controls
    void PlayMusicEvent();
    void StopMusicEvent();
    void PauseMusicEvent();
    void ResumeMusicEvent();
    void ModifyMasterVolume();
    void ModifyMusicVolume();
    void ModifySFXVolume();
    void ModifyMusicIntensity(Tier tier);

    // Wwise Switch Methods
    void SetRunningSwitch();
    void SetWallRunningSwitch();

    // Trigger Player Sound Effects
    void PlayFootSteps();
    void PlayWallRunFootSteps();
    void PlayAttackSoundEfect();
    void PlayClimbingSoundEffect();
    void PlayDashSoundEffect();
    void PlayGrappleLaunchSoundEffect();
    void PlayGrapplePullSoundEffect();
    void PlayGrappleRopeSoundEffect();
    void StopGrappleSoundEffect();
    void PlayJumpingSoundEffect();
    void PlayLedgeGrabSoundEffect();
    void PlaySlideSoundEffect();

    // Global Controls
    void PauseSoundEffects();
    void ResumeSoundEffects();
    void StopSoundEffects();

    // Trigger World Sound Effects
    void PlaySlidingObjectSoundEffect();
    void PlayDoorSoundEffect(Enums.DoorTypeSFX doorType);
    void PlayMetalGrateSoundEffect();
    void PlayVaseBreakingSoundEffect();
    void PlayFlagDropSoundEffect();
    void PlayGrapplePointBreakSoundEffect();
    void PlayStatueHitSoundEffect();
    void PlayWallAppearingSoundEffect();
    void PlayStatueAppearingSoundEffect();
    void PlayGrappAppearingSoundEffect();

    //Trigger UI Sound Effects
    void PlayUIScrollUpwardSoundEffect();
    void PlayUIScrollDownwardSoundEffect();
    void PlayCutScene(Enums.CutScene cutScene);
}