using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Data Asset", menuName = "ScriptableObjects/SaveAndLoad/DataAsset", order = 50)]
[System.Serializable]
public class DataAsset : ScriptableObject
{
    public Action onModified = null;
    public MenuSettings MenuSettings = null;
    public GameData GameData = null;
    public KeyBindings KeyBindings = null;

    public void Initialize(DataAssetWrapper dataAssetWrapper)
    {
        MenuSettings.Initialize(dataAssetWrapper.MenuSettings);
        GameData.Initialize(dataAssetWrapper.GameData);
        //    KeyBindings.UpdateBindings(dataAssetWrapper.KeyBindings);
        KeyBindings = dataAssetWrapper.KeyBindings;
        KeyBindings.CheckIfDefaultsRequired();
    }

    public void AddWalktroughDetails(LevelSettingsDef levelSettings, WalkthroughDetails currentWalktrougDetails)
    {
        switch (levelSettings.levelName)
        {
            case Enums.LevelName.None:
                Debug.LogError("Wrong Level Name Assigned!");
                break;

            case Enums.LevelName.Level01:
                InsertWalktroughDetails(currentWalktrougDetails, ref GameData._level1TopDetails);
                if(GameData._maxLevelUnlocked == 1)
                {
                    GameData._maxLevelUnlocked = 2;
                    onModified?.Invoke();
                }
                break;
            case Enums.LevelName.Level02:
                InsertWalktroughDetails(currentWalktrougDetails, ref GameData._level2TopDetails);
                if (GameData._maxLevelUnlocked == 2)
                {
                    GameData._maxLevelUnlocked = 3;
                    onModified?.Invoke();
                }
                break;
            case Enums.LevelName.Level03:
                InsertWalktroughDetails(currentWalktrougDetails, ref GameData._level3TopDetails);
                if (GameData._maxLevelUnlocked == 3)
                {
                    GameData._maxLevelUnlocked = 3;
                    onModified?.Invoke();
                }
                break;
        }
    }

    private void InsertWalktroughDetails(WalkthroughDetails newDetails, ref List<WalkthroughDetails> topScores)
    {
        topScores.Add(newDetails);

        var sorted = topScores.OrderBy(x => x.TimeSpent).ToList<WalkthroughDetails>();
        topScores = sorted;

        topScores.RemoveAt(topScores.Count - 1);

        onModified?.Invoke();
    }
}

[System.Serializable]
public class DataAssetWrapper
{
    public MenuSettingsWrapper MenuSettings = null;
    public GameDataWrapper GameData = null;
    public KeyBindings KeyBindings = null;

    public DataAssetWrapper()
    {

    }

    public DataAssetWrapper(DataAsset data)
    {
        KeyBindings = new KeyBindings(data.KeyBindings);
        //  KeyBindings = new KeyBindings();
        //KeyBindings.UpdateBindings(data.KeyBindings);
        MenuSettings = new MenuSettingsWrapper(data.MenuSettings);
        GameData = new GameDataWrapper(data.GameData);
    }
}
