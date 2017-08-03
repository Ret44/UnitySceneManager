using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ScreenManager))]
public class ScreenManagerEditor : Editor {

    ScreenManager script;
    int screenId = 0;

    public void OnEnable()
    {
        script = target as ScreenManager;
    }

    private string[] GetScreenNames()
    {
        List<string> screenNames = new List<string>();
        for(int i=0; i<script.screens.Length;i++)
        {
            screenNames.Add(script.screens[i].key);
        }
        return screenNames.ToArray();
    }

    public override void OnInspectorGUI()
    {
        if (script.screens.Length == 0)
        {
            EditorGUILayout.HelpBox("You have to add at least one ScreenData asset before you could load any scene.", MessageType.Info);
        }
        else {
            if (EditorApplication.isPlaying)
            {
                EditorGUILayout.BeginHorizontal();
                screenId = EditorGUILayout.Popup(screenId, GetScreenNames());
                if (GUILayout.Button("Load scene"))
                {
                    ScreenManager.LoadScreen(script.screens[screenId]);
                }
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.HelpBox("Scene loading is not available in Editor mode!", MessageType.Info);
            }

        }
        DrawDefaultInspector();
    }
}
