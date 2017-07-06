using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;

public class LevelGenerator : MonoBehaviour
{

    protected bool isSeedRandom = false;
    protected Transform root;
    protected string seed;
    protected System.Random rnd;

    // PROGRESS BAR STUFF
    protected float currProgress;
    protected float totalProgress;
    protected float onePc;

	public bool isWorking = false;
	public ProgressBar window;

	public void setProgressBarWindow(ProgressBar _p){
		window = _p;
	}

    public void setIsRandomSeed(bool _s)
    {
        isSeedRandom = _s;
    }

    public void setSeed(System.Random _rng)
    {
        rnd = _rng;
    }

}

public class Hauberk : LevelGenerator
{
    private List<Room> _rooms; // list of placed rooms
    private List<Coord> allTileCoords;
    private List<Coord> savedTileCords; // for editor
    private bool cancel;

    private bool rooms;
    public Transform tilePrefab;
    public Vector2 mapSize;

    [Range(0, 1)]
    public float outlinePercentage = 0;
    public bool useRandomSeed;

    public void GenerateLevel(Vector2 mSize, Transform map, Transform quadPrefab, float _op)
    {
        // SETTING UP PROGRESSBAR STUFF
        currProgress = 0;
        totalProgress = mapSize.x * mapSize.y * 2;
        onePc = totalProgress / 100;

		window.maximum = (int)(mapSize.x * mapSize.y * 2);
		window.setup ();
		window.dialog = "Creating Map";
		window.isBuilding = true;
		window.ShowPopup();

		isWorking = true;
        mapSize = mSize;
        tilePrefab = quadPrefab;
        outlinePercentage = _op;
        root = map;

        // SORTS OUT WHERE TO PLACE THE WALLS
        allTileCoords = new List<Coord>();
		window.logThis("Defining where quads are placed");
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                /*if (cancel)
                {
                    EditorUtility.ClearProgressBar();
                    allTileCoords = new List<Coord>();
                    break;
                }*/

				if (!isWorking)
				{
					allTileCoords = new List<Coord>();
					break;
				}

                allTileCoords.Add(new Coord(x, y));
				window.performStep ();
                //currProgress += onePc;
                //cancel = EditorUtility.DisplayCancelableProgressBar("Creating Map", "Defining where quads are placed " + (currProgress / totalProgress) * 100, currProgress / totalProgress);
            }
        }

        if (rooms)
        {
            allTileCoords = addRooms(allTileCoords);
            savedTileCords = allTileCoords;
        }

        // DRAWS THE MAP TO THE SCENE

        Transform mapHolder = new GameObject("Generated Map").transform;
        mapHolder.parent = map;
		window.logThis("Drawing map to screen");
        for (int x = 0; x < mapSize.x; x++)
        {

            for (int y = 0; y < mapSize.y; y++)
            {
				/*
                if (cancel)
                {
                    EditorUtility.ClearProgressBar();
                    allTileCoords = new List<Coord>();
                    break;
                }*/

				if (!isWorking)
				{
					allTileCoords = new List<Coord>();
					break;
				}

                if (!coordExists(allTileCoords, x, y))
                {
                    continue;
                }
                Vector3 tilePosition = coordToPosition(x, y);
                Transform newTile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right * 90)) as Transform;
                newTile.localScale = Vector3.one * (1 - outlinePercentage);
                newTile.name = x + "_" + y;
                newTile.parent = mapHolder;
                //currProgress += onePc;
				window.performStep ();
                //cancel = EditorUtility.DisplayCancelableProgressBar("Adding to scene", "Adding your quad prefabs to the current screen " + (currProgress / totalProgress), currProgress / totalProgress);
            }
        }

		window.logThis ("Done");
		window.reset ();
       // EditorUtility.ClearProgressBar();

    }


    public void GenerateLevel()
    { // FOR EDITOR USAGE

        foreach (Transform child in root)
        {
            GameObject.DestroyImmediate(child.gameObject);
        }

        // SORTS OUT WHERE TO PLACE THE WALLS
        allTileCoords = new List<Coord>();
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                allTileCoords.Add(new Coord(x, y));
            }
        }

        if (rooms)
        {
            allTileCoords = savedTileCords;
        }

        // DRAWS THE MAP TO THE SCENE

        Transform mapHolder = new GameObject("Generated Map").transform;
        mapHolder.parent = root;

        for (int x = 0; x < mapSize.x; x++)
        {

            for (int y = 0; y < mapSize.y; y++)
            {
                if (!coordExists(allTileCoords, x, y))
                {
                    continue;
                }
                Vector3 tilePosition = coordToPosition(x, y);
                Transform newTile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right * 90)) as Transform;
                newTile.localScale = Vector3.one * (1 - outlinePercentage);
                newTile.name = x + "_" + y;
                newTile.parent = mapHolder;
            }
        }
    }

    public void addRooms(bool b)
    {
        rooms = b;
    }

    public List<Coord> addRooms(List<Coord> alTileCoords)
    {
        _rooms = new List<Room>();
        int numRoomTries = 3;
        for (var i = 0; i < numRoomTries; i++)
        {
            /*if (cancel)
            {
                EditorUtility.ClearProgressBar();
                alTileCoords = new List<Coord>();
                break;
            }*/
            Coord rndc = new Coord(rnd.Next(1, (int)mapSize.x), rnd.Next(1, (int)mapSize.y));
            Room room = new Room(rndc, rnd.Next(2, (int)mapSize.y), rnd.Next(2, (int)mapSize.x));
            var overlaps = false;
            if (_rooms.Count > 0)
            {
                foreach (Room other in _rooms)
                {
                    if (room.distanceTo(other) <= 0)
                    {
                        overlaps = true;
                    }
                }
            }

            if (overlaps)
            {
                // SKIPS ITERATION IF THERE'S AN OVERLAP
                continue;
            }

            // REMOVES COORDS SO SOME WALLS ARENT CREATED IN THE SCENE
            for (int x = 0; x < mapSize.x; x++)
            {
                for (int y = 0; y < mapSize.y; y++)
                {
                    if (room.coordExists(x, y))
                    {
                        Coord toRemove = room.getCoordAt(x, y);
                        alTileCoords.Remove(toRemove);
                    }
                }

            }

            _rooms.Add(room);
            //currProgress += onePc;
            //cancel = EditorUtility.DisplayCancelableProgressBar("Adding rooms", "Defining rooms in your dungeon", currProgress / totalProgress);

        }

        return alTileCoords;
    }

    public bool coordExists(List<Coord> allTileCoords, int _x, int _y)
    {
        for (int i = 0; i < allTileCoords.Count; i++)
        {
            if (allTileCoords[i].x == _x & allTileCoords[i].y == _y)
            {
                return true;
            }

        }
        return false;
    }

    Vector3 coordToPosition(int x, int y)
    {
        return new Vector3(-mapSize.x / 2 + 0.5f + x, 0, -mapSize.y / 2 + 0.5f + y);
    }
}

public class Cave : LevelGenerator
{
    public int width, height;
    int[,] map;

    [Range(0, 100)]
    public int randomFillPercent;

    public void GenerateLevel(Transform root, int _rfpc, int _w, int _h)
    {
        // SETTING UP PROGRESSBAR STUFF
        currProgress = 0;
        totalProgress = height * width;
        onePc = totalProgress / 100;

        randomFillPercent = _rfpc;
        width = _w;
        height = _h;

        map = new int[width, height]; // 0 = empty, 1 occupied by wall

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                map[x, y] = (rnd.Next(0, 100) < randomFillPercent) ? 1 : 0;
            }
        }

        // DRAWING MAP
        Transform mapHolder = new GameObject("Generated Level").transform;
        mapHolder.parent = root;

        // imcomplete

    }

    public void GenerateLevel()
    { // FOR EDITOR USAGE

        map = new int[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                map[x, y] = (rnd.Next(0, 100) < randomFillPercent) ? 1 : 0;
            }
        }
    }

    void OnDrawGizmos()
    {
        if (map != null)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Gizmos.color = (map[x, y] == 1) ? Color.black : Color.white;
                    Vector3 pos = new Vector3(-width / 2 + x + .5f, 0, -height / 2 + y + .5f);
                    Gizmos.DrawCube(pos, Vector3.one);
                }
            }
        }
    }

}

public struct Coord
{
    public int x;
    public int y;

    public Coord(int _x, int _y)
    {
        x = _x;
        y = _y;
    }
}
