public static class Enums
{
    public enum MultiplierTier
    {
        Tier1,
        Tier2,
        Tier3,
        Tier4,
        Tier5
    }

    public enum SceneIndecies
    {
        MainMenu=1,
        Level01,
        Level02,
        Level03
    }

    public enum SceneType
    {
        None = -1,
        MainMenu,
        LevelSelect,
        CutScene,
        Level01,
        Level02
    }

    public enum MusicSyncType
    {
        Invalid,
        BeatSync,
        BarSync
    }

    public enum ObjectTypeSFX
    {
        None = -1,
        DefaultDoor,
        LightDoor,
        HeavyDoor,
        BossDoor,
        MetalGrate,
        Vine,
        Flag,
        ExplosiveDoor,
        WallAppear,
        StatueAppear,
        GrappAppear
    }

    public enum DoorTypeSFX
    {
        Default,
        Light,
        Heavy,
        Boss,
        Explosive,
        Vine
    }

    public enum LevelName
    {
        None = -1,
        Level01,
        Level02,
        Level03
    }

    public enum PlayerActionType
    {
        Dash,
        Slide,
        Interact,
        HookShot,
    }

    public enum UICanvases
    {
        EmptyCanvas,
        MainMenuCanvas,
        GameCanvas,
        GameOverWinUI,
        SettingsCanvas,
        PauseCanvas,
        CreditsCanvas,
        LeaderBoardCanvas
    }

    public enum TabButtons
    {
        None,
        GameplaySettingsBTN,
        MusicSettingsBT,
        ControlsSettingsBTN,
        DelaySettingsBTN
    };

    public enum CutScene
    {
        None = -1,
        LevelOne,
        LevelTwo,
        LevelThree,
    }
}