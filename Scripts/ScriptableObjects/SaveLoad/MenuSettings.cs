using UnityEngine;

[System.Serializable]
public class MenuSettings
{
    //Gameplay settings
    public FloatValue VerticalSensitivity = null;
    public FloatValue HorizontalSensitivity = null;

    //Audio Settings 
    public FloatValue MasterVolume = null;
    public FloatValue MusicVolume = null;
    public FloatValue SfxVolume = null;

    public void Initialize(MenuSettingsWrapper menuSettingsWrapper)
    {
        VerticalSensitivity.SetValue(menuSettingsWrapper.VerticalSensitivity);
        HorizontalSensitivity.SetValue(menuSettingsWrapper.HorizontalSensitivity);

        MasterVolume.SetValue(menuSettingsWrapper.MasterVolume);
        MusicVolume.SetValue(menuSettingsWrapper.MusicVolume);
        SfxVolume.SetValue(menuSettingsWrapper.SfxVolume);

        
    }

    //Graphics settings
    // TODO: 
    // Resolution
    // Quality
}

[System.Serializable]
public class MenuSettingsWrapper
{
    //Gameplay settings
    public float VerticalSensitivity = 0.0f;
    public float HorizontalSensitivity = 0.0f;

    //Audio Settings 
    public float MasterVolume = 0.0f;
    public float MusicVolume = 0.0f;
    public float SfxVolume = 0.0f;

    public MenuSettingsWrapper()
    {

    }

    public MenuSettingsWrapper(MenuSettings data)
    {
        VerticalSensitivity = data.VerticalSensitivity.GetValue;
        HorizontalSensitivity = data.HorizontalSensitivity.GetValue;

        MasterVolume = data.MasterVolume.GetValue;
        MusicVolume = data.MusicVolume.GetValue;
        SfxVolume = data.SfxVolume.GetValue;
    }

}