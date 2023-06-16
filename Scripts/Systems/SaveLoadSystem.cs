using Newtonsoft.Json;
using System.IO;
using UnityEngine;

[CreateAssetMenu(fileName = "New Save Load System", menuName = "ScriptableObjects/SaveAndLoad/SaveLoadSystem", order = 50)]

public class SaveLoadSystem : ScriptableObject
{
    [SerializeField] private DataAsset _dataAsset = null;
    private static readonly char _separator = Path.DirectorySeparatorChar;
    private const string _fileName = "DataAssetFile.json";

    public int GetMaxLevelUnlocked()
    {
        return _dataAsset.GameData._maxLevelUnlocked;
    }

    public void Initialize()
    {
        _dataAsset.onModified += Save;
    }

    private void OnDisable()
    {
        _dataAsset.onModified -= Save;
    }

    public void Load()
    {
        string path = $"{Application.dataPath}{_separator}{_fileName}";

        if(!File.Exists(path))
        {
            //Fresh Launch create a default file
            Save();
        }    

        using (StreamReader file = File.OpenText(path))
        {
            string json = file.ReadToEnd();
            DataAssetWrapper dataAssetWrapper = new DataAssetWrapper();
            dataAssetWrapper = JsonConvert.DeserializeObject<DataAssetWrapper>(json); 
            _dataAsset.Initialize(dataAssetWrapper);
        }

        Debug.Log("Loaded: " + path);
    }

    public void Save()
    {
        string path = $"{Application.dataPath}{_separator}{_fileName}";
        string json = string.Empty;

        DataAssetWrapper dataAssetWrapper = new DataAssetWrapper(_dataAsset);
        json = JsonConvert.SerializeObject(dataAssetWrapper, Formatting.Indented);
        File.WriteAllText(path, json);

        //Debug.Log("Saved: " + path);
    }
}

