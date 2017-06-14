using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public enum OPTIONS
{
    Hauberk,
    Cave,
    TEST
}

public class Main : EditorWindow
{
    private Vector2 scrollPos;
    private Vector2 mapSize;
    public OPTIONS op;
    private string seed;

    private Transform tilePrefab;
    private string objectName = "map001";
    private bool rooms = true;

    private int randomFillPercent = 0;
    private int width;
    private int height;
    private bool useRandomSeed;

    private string debugPhrase;

    [MenuItem("Window/Ocean Level Generator")]
    public static void ShowWindow(){
        EditorWindow.GetWindow(typeof(Main));
    }

    void OnGUI()
    {
        GUILayout.Label("Output Settings for Map Generation", EditorStyles.boldLabel);
        objectName = EditorGUILayout.TextField("Object Name: ", objectName);
        op = (OPTIONS)EditorGUILayout.EnumPopup("Map style:", op);
        DoGUI(op);
        useRandomSeed = EditorGUILayout.Toggle("Use random seed", useRandomSeed);


        EditorGUI.BeginDisabledGroup(useRandomSeed);
        seed = EditorGUILayout.TextField("Seed: ", seed);
        EditorGUI.EndDisabledGroup();
    
        if (GUILayout.Button("Generate"))
        {
            generate(op);
        }
    }

    private void DoGUI(OPTIONS op)
    {
        if (op == OPTIONS.Hauberk)
        {
            GUILayout.Label("Hauberk Map Generation Settings", EditorStyles.boldLabel);
            mapSize = EditorGUILayout.Vector2Field("Size:", mapSize);
            tilePrefab = EditorGUILayout.ObjectField("Quad Wall Prefab", tilePrefab, typeof(Transform), true) as Transform;
            rooms = EditorGUILayout.Toggle("Add rooms", rooms);
        }
        if (op == OPTIONS.Cave)
        {
            GUILayout.Label("Cave Generation Settings", EditorStyles.boldLabel);
            width = EditorGUILayout.IntField("Width:", width);
            height = EditorGUILayout.IntField("Height:", height);
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
            if (useRandomSeed)
            {
                seed = Time.time.ToString();
            }
            switch (op)
            {
                case OPTIONS.Hauberk:
                    Hauberk mg = new Hauberk();
                    mg.addRooms(rooms);
                    mg.setSeed(seed);
                    mg.GenerateHauberkMap(mapSize, objectName, tilePrefab);
                    break;
                case OPTIONS.Cave:
                    Cave c = new Cave();
                    c.setSeed(seed);
                    c.GenerateCave(objectName, randomFillPercent, width, height);
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