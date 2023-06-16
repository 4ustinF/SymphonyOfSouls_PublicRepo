using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace SpawningSystem
{
    [CustomEditor(typeof(SpawningManager))]
    class SpawningManagerHelperEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            SpawningManager myScript = (SpawningManager)target;
            if (GUILayout.Button("Reset All Check Points"))
            {
                myScript.ResetAllCheckPoints();
            }
            else if (GUILayout.Button("Create New Check Point"))
            {
                myScript.CreateNewCheckPoint();
            }
            else if (GUILayout.Button("Create New Spawning Point"))
            {
                myScript.CreateNewSpawningPoint();
            }
            else if (GUILayout.Button("Fix Hierarchy"))
            {
                myScript.FixHierarchy();
            }
        }
    }
}
#endif