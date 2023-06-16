using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    public IntValue NumbersOfLevelsCompleted = null;
    public List<WalkthroughDetails>_level1TopDetails = new List<WalkthroughDetails>(5);
    public List<WalkthroughDetails> _level2TopDetails = new List<WalkthroughDetails>(5);
    public List<WalkthroughDetails> _level3TopDetails = new List<WalkthroughDetails>(5);
    public int _maxLevelUnlocked = 1;

    public void Initialize(GameDataWrapper gameData)
    {
        NumbersOfLevelsCompleted.SetValue(gameData.NumbersOfLevelsCompleted);
        _level1TopDetails = gameData.level1TopDetails;
        _level2TopDetails = gameData.level2TopDetails;
        _level3TopDetails = gameData.level3TopDetails;
        _maxLevelUnlocked = gameData.maxLevelUnlocked;
    }
}

[System.Serializable]
public class GameDataWrapper
{
    public int NumbersOfLevelsCompleted = -1;
    public List<WalkthroughDetails> level1TopDetails = new List<WalkthroughDetails>(5);
    public List<WalkthroughDetails> level2TopDetails = new List<WalkthroughDetails>(5);
    public List<WalkthroughDetails> level3TopDetails = new List<WalkthroughDetails>(5);
    public int maxLevelUnlocked = 1;

    public GameDataWrapper()
    {
    }

    public GameDataWrapper(GameData data)
    {
        NumbersOfLevelsCompleted = data.NumbersOfLevelsCompleted.GetValue;
        level1TopDetails = data._level1TopDetails;
        level2TopDetails = data._level2TopDetails;
        level3TopDetails = data._level3TopDetails;
        maxLevelUnlocked = data._maxLevelUnlocked;
    }
}
