using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;

public class LevelGenerator : MonoBehaviour {

    protected string seed; // CAVE
    protected bool SeedRandom;

    // PROGRESS BAR STUFF
    protected float currProgress;
    protected float totalProgress;
    protected float onePc;

    public void setSeedRandom(bool _s)
    {
        SeedRandom = _s;
    }

    public void setSeed(string _s)
    {
        seed = _s;
    }

}

public class Hauberk : LevelGenerator
{

    private List<Room> _rooms; //list of placed rooms
    private List<Coord> allTileCoords;
    private bool cancel;

    private bool rooms;
    private int _currentRegion = -1;

    public Transform tilePrefab;
    public Vector2 mapSize;

    public void GenerateHauberkMap(Vector2 mSize, string mapName, Transform quadPrefab)
    {
        currProgress = 0;
        mapSize = mSize;
        totalProgress = mapSize.x * mapSize.y * 2;
        onePc = totalProgress / 100;

        System.Random rnd = new System.Random(seed.GetHashCode());
        tilePrefab = quadPrefab;

        // SORTS OUT WHERE TO PLACE THE WALLS
        allTileCoords = new List<Coord>();
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                if (cancel)
                {
                    EditorUtility.ClearProgressBar();
                    allTileCoords = new List<Coord>();
                    break;
                }
                allTileCoords.Add(new Coord(x, y));
                currProgress += onePc;
                cancel = EditorUtility.DisplayCancelableProgressBar("Creating Map", "Defining where quads are placed " + (currProgress / totalProgress) * 100, currProgress / totalProgress);
            }
        }

        if (rooms)
        {
            allTileCoords = addRooms(allTileCoords);
        }

        // DRAWS THE MAP TO THE SCENE
        Transform map = new GameObject(mapName).transform;

        Transform mapHolder = new GameObject("Generated Map").transform;
        mapHolder.parent = map;
        for (int x = 0; x < mapSize.x; x++)
        {

            for (int y = 0; y < mapSize.y; y++)
            {
                if (cancel)
                {
                    EditorUtility.ClearProgressBar();
                    allTileCoords = new List<Coord>();
                    break;
                }

                if (!coordExists(allTileCoords, x, y))
                {
                    continue;
                }
                Vector3 tilePosition = coordToPosition(x, y);
                Transform newTile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right * 90)) as Transform;
                
                newTile.name = x + " " + y;
                newTile.parent = mapHolder;
                currProgress += onePc;
                cancel = EditorUtility.DisplayCancelableProgressBar("Adding to scene", "Adding your quad prefabs to the current screen " + (currProgress / totalProgress), currProgress / totalProgress);
            }
        }

        HauberkEditor h = GameObject.Find(map.name).AddComponent<HauberkEditor>();
        h.setDefaults(mapSize.x, mapSize.y, tilePrefab, SeedRandom);
        EditorUtility.ClearProgressBar();

    }

    public void addRooms(bool b)
    {
        rooms = b;
    }

    public List<Coord> addRooms(List<Coord> alTileCoords)
    {
        System.Random rnd = new System.Random();
        _rooms = new List<Room>();
        int numRoomTries = 3;
        for (var i = 0; i < numRoomTries; i++)
        {
            if (cancel)
            {
                EditorUtility.ClearProgressBar();
                alTileCoords = new List<Coord>();
                break;
            }
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
            currProgress += onePc;
            cancel = EditorUtility.DisplayCancelableProgressBar("Adding rooms", "Defining rooms in your dungeon", currProgress / totalProgress);

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
    private int width, height;
    private int[,] map;

    public void GenerateCave(string mapName, int randomFillPercent, int width, int height)
    {
        // 0 = empty, 1 occupied by wall
        map = new int[width, height];

        System.Random pseudoRandom = new System.Random(seed.GetHashCode());
        print(seed.GetHashCode());
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                map[x, y] = (pseudoRandom.Next(0, 100) < randomFillPercent) ? 1 : 0;
            }
        }

        // DRAWING MAP
        Transform level = new GameObject(mapName).transform;

        Transform mapHolder = new GameObject("Generated Level").transform;
        mapHolder.parent = level;
        
        CaveEditor r = GameObject.Find(level.name).AddComponent<CaveEditor>();
        r.setDefaults(width,height,map, SeedRandom);
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


