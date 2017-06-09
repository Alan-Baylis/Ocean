using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public enum OPTIONS
{
    Hauberk,
    TEST
}

public class Settings : EditorWindow
{
    private Vector2 scrollPos;
    private Vector2 mapSize;

    private Transform tilePrefab;
    private string objectName = "map001";
    bool rooms = true;

    private string debugPhrase;

    public OPTIONS op;

    [MenuItem("Window/Level Generator")]
    public static void ShowWindow(){
        EditorWindow.GetWindow(typeof(Settings));
    }

    void OnGUI()
    {
        GUILayout.Label("Output Settings for Map Generation", EditorStyles.boldLabel);
        objectName = EditorGUILayout.TextField("Object Name: ", objectName);
        op = (OPTIONS)EditorGUILayout.EnumPopup("Map style:", op);
        DoGUI(op);
        if (GUILayout.Button("Generate"))
        {
            generate(op);
        }
    }

    private void DoGUI(OPTIONS op)
    {
        if (op == OPTIONS.Hauberk)
        {
            GUILayout.Label("Hauberk Map Settings", EditorStyles.boldLabel);
            mapSize = EditorGUILayout.Vector2Field("Size:", mapSize);
            tilePrefab = EditorGUILayout.ObjectField("Quad Wall Prefab", tilePrefab, typeof(Transform), true) as Transform;
            rooms = EditorGUILayout.Toggle("Add rooms", rooms);
        }
        if (op == OPTIONS.TEST) {
            GUILayout.Label("Test stuff", EditorStyles.boldLabel);
            debugPhrase = EditorGUILayout.TextField("Debug Phrase: ", debugPhrase);
        }
    }

    void generate(OPTIONS op)
    {
        if (GameObject.Find(objectName) != null)
        {
            EditorUtility.DisplayDialog("Error", "Object "+objectName+" already exists", "Ok","");
        }
        else
        {
            switch (op)
            {
                case OPTIONS.Hauberk:
                    LevelGenerator mg = new LevelGenerator();
                    mg.addRooms(rooms);
                    mg.GenerateHauberkMap(mapSize, objectName, tilePrefab);
                    break;
                case OPTIONS.TEST:
                    Debug.Log("Testing...");
                    Debug.Log(System.IO.Directory.GetCurrentDirectory());
                    break;
                default:
                    Debug.LogError("Unrecognized Option");
                    break;
            }
        }
    }

    void OnInspectorUpdate()
    {
        Repaint();
    }

}