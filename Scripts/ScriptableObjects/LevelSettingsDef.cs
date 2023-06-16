using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newLevelSettings", menuName = "ScriptableObjects/Level/LevelSettings")]
public class LevelSettingsDef : ScriptableObject
{
    public Enums.LevelName levelName = Enums.LevelName.None;
    public Enums.SceneType sceneType = Enums.SceneType.None;
    public float TimeToCompleteLevel = 0.0f;
}
