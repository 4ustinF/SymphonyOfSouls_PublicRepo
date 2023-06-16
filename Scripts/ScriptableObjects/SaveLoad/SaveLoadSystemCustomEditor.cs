using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

[CustomEditor(typeof(SaveLoadSystem))]
public class SaveLoadSystemCustomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var script = (SaveLoadSystem)target;

        if (GUILayout.Button("Save", GUILayout.Height(30)))
        {
            script.Save();
        }
        else if (GUILayout.Button("Load", GUILayout.Height(30)))
        {
            script.Load();
        }
    }
}
#endif
