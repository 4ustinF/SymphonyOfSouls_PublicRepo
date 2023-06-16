using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CrystalsTool))]
public class CrystalsToolCustomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var crystalsTool = target as CrystalsTool;

        if (GUILayout.Button("Find Wall", GUILayout.Height(30)))
        {
            crystalsTool.FindWall();
        }

        if (GUILayout.Button("Random Position", GUILayout.Height(30)))
        {
            crystalsTool.RandomPosition();
        }

        if (GUILayout.Button("Random Rotation", GUILayout.Height(30)))
        {
            crystalsTool.RandomRotation();
        }

        if (GUILayout.Button("Random Scale", GUILayout.Height(30)))
        {
            crystalsTool.RandomScale();
        }

        //DrawDefaultInspector();
    }
}
