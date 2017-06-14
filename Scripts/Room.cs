using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Room
{
    private Coord c;
    private int h, w;
    private List<Coord> allTileCoords = new List<Coord>();

    public Room()
    {
        c = new Coord(1, 1);
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
            if (allTileCoords[i].x == x & allTileCoords[i].y == y)
            {
                return allTileCoords[i];
            }
        }
        return new Coord(-99, -99);
    }

    public int distanceTo(Room r)
    {
        int dx = this.c.x - r.c.x;
        int dy = this.c.y - r.c.y;
        float result = Mathf.Sqrt(dy * dy + dx * dx);
        //float result = Mathf.Abs(Mathf.Sqrt(dy * dy + dx * dx));
        return (int)result;
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
