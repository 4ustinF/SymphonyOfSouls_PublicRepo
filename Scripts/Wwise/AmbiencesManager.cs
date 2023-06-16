using UnityEngine;

public class AmbiencesManager : MonoBehaviour
{
    private GameObject _ambienceObject = null;

    public void Initialize()
    {
        LevelController.onPause += PauseAmbience;

        var _sceneType = ServiceLocator.Get<LevelLoader>().GetSceneType();

        switch (_sceneType)
        {
            case Enums.SceneType.Level01:
                var ambienceLevel01 = Resources.Load("Level01Ambience");
                _ambienceObject = Instantiate(ambienceLevel01) as GameObject;
                _ambienceObject.transform.SetParent(this.transform);
                _ambienceObject.name = "Level01Ambience";
                Debug.Log("Loaded specific level ambience prefab");
                break;
            case Enums.SceneType.Level02:
                var ambienceLevel02 = Resources.Load("Level02Ambience");
                _ambienceObject = Instantiate(ambienceLevel02) as GameObject;
                _ambienceObject.transform.SetParent(this.transform);
                _ambienceObject.name = "Level02Ambience";
                Debug.Log("Loaded specific level ambience prefab");
                break;
        }
    }

    private void PauseAmbience()
    {
        if (LevelController.IsPaused)
        {
            AkSoundEngine.SetState("GameState", "Pause");
            return;
        }

        AkSoundEngine.SetState("GameState", "Gameplay");
    }

    public void StopAllAmbiences()
    {
        _ambienceObject.GetComponent<AmbiencesController>()?.StopAmbiences();
    }

    private void OnDestroy()
    {
        LevelController.onPause -= PauseAmbience;
    }
}