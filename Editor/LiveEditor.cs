using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LiveEditor : Editor {
    
}

[CustomEditor(typeof(HauberkEditor))]
public class HauberkE : LiveEditor {
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        HauberkEditor map = target as HauberkEditor;

    }
}
