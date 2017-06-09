using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class LevelGenerator : MonoBehaviour {

    private List<Room> _rooms; //list of placed rooms
    private List<Coord> allTileCoords;
    private bool cancel;

    private bool rooms;

    // PROGRESS BAR STUFF
    private float currProgress;
    private float totalProgress;
    private float onePc;

    private int _currentRegion = -1;

    public int seed;
    public Transform tilePrefab;
    public Vector2 mapSize;

    /*[Range(0,1)]
    public float outlinePercentage;*/
	
    public void GenerateHauberkMap(Vector2 mSize, string mapName, Transform quadPrefab)
    {
        currProgress = 0;
        mapSize = mSize;
        totalProgress = mapSize.x * mapSize.y * 2;
        onePc = totalProgress / 100;

        System.Random rnd = new System.Random();
        tilePrefab = quadPrefab;

        int seed = rnd.Next(0,1000000);

        // SORTS OUT WHERE TO PLACE THE WALLS
        allTileCoords = new List<Coord> ();
        for (int x = 0; x < mapSize.x; x++) {
            for (int y = 0; y < mapSize.y; y++) {
                if (cancel)
                {
                    EditorUtility.ClearProgressBar();
                    allTileCoords = new List<Coord>();
                    break;
                }
                allTileCoords.Add (new Coord(x,y));
                currProgress += onePc;
                cancel = EditorUtility.DisplayCancelableProgressBar("Creating Map", "Defining where quads are placed " + (currProgress / totalProgress )*100, currProgress /totalProgress);
                Debug.Log((currProgress / totalProgress)*100);
            }
        }

        if (rooms)
        {
            allTileCoords = addRooms(allTileCoords);
        }

        // DRAWS THE MAP TO THE SCENE
        Transform map = new GameObject(mapName).transform;

        Transform mapHolder = new GameObject ("Generated Map").transform;
        mapHolder.parent = map;
		for (int x = 0; x < mapSize.x; x++) {
            
            for (int y = 0; y < mapSize.y; y++) {
                if (cancel)
                {
                    EditorUtility.ClearProgressBar();
                    allTileCoords = new List<Coord>();
                    break;
                }

                if (!coordExists(allTileCoords,x,y)) {
                    Debug.Log("-- Skipped " + x + " " + y);
                    continue;
                }
                Vector3 tilePosition = coordToPosition(x, y);
                Debug.Log("Created " + x + " " + y);
                Transform newTile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right * 90)) as Transform;
                //newTile.localScale = Vector3.one * (1 - outlinePercentage);
                newTile.name = x + " " + y;
                newTile.parent = mapHolder;
                currProgress += onePc;
                cancel = EditorUtility.DisplayCancelableProgressBar("Adding to scene", "Adding your quad prefabs to the current screen " + (currProgress / totalProgress), currProgress / totalProgress);
                Debug.Log((currProgress / totalProgress) * 100);
            }
		}

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
            /*if (cancel)
            {
                EditorUtility.ClearProgressBar();
                alTileCoords = new List<Coord>();
                break;
            }*/
            Coord rndc = new Coord(rnd.Next(1, (int)mapSize.x), rnd.Next(1, (int)mapSize.y));
            Room room = new Room(rndc, rnd.Next(2, (int)mapSize.y), rnd.Next(2, (int)mapSize.x));
            var overlaps = false;
            if (_rooms.Count > 0) {
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

    Vector3 coordToPosition(int x, int y){
        return new Vector3 (-mapSize.x / 2 + 0.5f + x, 0, -mapSize.y / 2 + 0.5f + y);
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

public class Room
{
    private Coord c;
    private int h, w;
    private List<Coord> allTileCoords = new List<Coord>();

    public Room()
    {
        c = new Coord(1,1);
        h = 3;
        w = 3;
        setTileCoords();
    }

    public Room(Coord _c, int _h, int _w)
    {
        c = _c;
        h = _h;
        w = _w;
        setTileCoords();
    }

    private void setTileCoords()
    {
        for (int x = c.x; x < w; x++)
        {
            for (int y = c.y; y < h; y++)
            {
                allTileCoords.Add(new Coord(x, y));
            }
        }
    }

    public List<Coord> getRoomTiles()
    {
        return allTileCoords;
    }

    public Coord getCoordAt(int x, int y)
    {
        for (int i = 0; i < allTileCoords.Count; i++)
        {
            if(allTileCoords[i].x == x & allTileCoords[i].y == y) {
                return allTileCoords[i];
            }  
        }
        return new Coord(-99,-99);
    }

    public int distanceTo(Room r)
    {
        int dx = this.c.x - r.c.x;
        int dy = this.c.y - r.c.y;
        float result = Mathf.Sqrt(dy * dy + dx * dx);
        //float result = Mathf.Abs(Mathf.Sqrt(dy * dy + dx * dx));
        return (int) result;
    }

    public bool coordExists(int _x, int _y)
    {
        for (int x = c.x; x < w; x++)
        {
            for (int y = c.y; y < h; y++)
            {
                if (x == _x && y == _y)
                {
                    return true;
                }
            }
        }
        return false;
    }
}
