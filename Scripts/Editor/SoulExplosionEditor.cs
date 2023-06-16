using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SoulExplosionManager))]
public class SoulExplosionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SoulExplosionManager myScript = (SoulExplosionManager)target;
        if (GUILayout.Button("Fix Soul Transforms"))
        {
            myScript.FixSoulTransforms();
        }
    }
}
