using UnityEngine;
using System.Collections;
using UnityEditor;

public class LevelEditor : Editor
{
    protected bool useRandomSeed;
    //EditorGUIUtility.ObjectContent(null, typeof(LevelEditor)).image)
}

[CustomEditor(typeof(Hauberk))]
public class HauberkEditor : LevelEditor
{
	public override void OnInspectorGUI(){
		base.OnInspectorGUI ();
		Hauberk h = target as Hauberk;
		h.GenerateLevel();
	}

}

[CustomEditor(typeof(Cave))]
public class CaveEditor : LevelEditor 
{

	public override void OnInspectorGUI(){
		base.OnInspectorGUI ();
		Cave c = target as Cave;
		//c.GenerateLevel();
	}
   
}