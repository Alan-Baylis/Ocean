using UnityEngine;
using System.Collections;
using UnityEditor;

public class LevelEditor : MonoBehaviour
{
    protected bool useRandomSeed;
    //EditorGUIUtility.ObjectContent(null, typeof(LevelEditor)).image)
}

public class HauberkEditor : LevelEditor
{
    public Vector2 mSize;
    public Transform quadPrefab;

    [Range(0,1)]
    public float outlinePercentage = 0;

    public void setDefaults(float w, float h, Transform ob, bool u)
    {
        mSize.x = w;
        mSize.y = h;
        quadPrefab = ob;
        useRandomSeed = u;
    }

    //newTile.localScale = Vector3.one * (1 - outlinePercentage);

}

public class CaveEditor : LevelEditor {

    public int width, height;
    public string seed;
    private int[,] map;

    [Range(0, 100)]
    public int randomFillPercent;

    public void setDefaults(int w, int h, int[,] m, bool u)
    {
        width = w;
        height = h;
        map = m;
        useRandomSeed = u;
    }

    void OnDrawGizmos()
    {
        if (map != null)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Gizmos.color = (map[x,y] == 1) ? Color.black : Color.white;
                    //Vector2 pos = new Vector3(-width / 2 + x + 0.5f, 0, -height / 2 + y + 0.5f);
                    Vector2 pos = new Vector3(-width / 2 + x + 0.5f, 0, -height / 2 + y + 0.5f);
                    Gizmos.DrawCube(pos, Vector3.one);
                }
            }
        }
    }
}