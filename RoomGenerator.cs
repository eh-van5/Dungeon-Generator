using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    #region Fields
    public GameObject roomPrefab;
    public GameObject corridorPrefab;

    public Vector2 origin;

    public int size;
    public float roomDistance
    {
        get
        {
            return roomPrefab.transform.localScale.x + (roomPrefab.transform.localScale.x / 2);
        }
    }

    private Room[,] rooms;
    #endregion
    public void GenerateLevel()
    {
        ClearRooms();

        rooms = new Room[size * 3, size * 3];
        Vector2 arrPos = new Vector2(size, size);

        rooms[(int)arrPos.x, (int)arrPos.y] = new Room(null, origin, arrPos, 0);

        //Parameter for CreateRoom() method
        Room root = rooms[(int)arrPos.x, (int)arrPos.y];
        while (filledRooms() < size)
        {
            root = CreateRoom(root);
            arrPos = root.arrayPos;
        }

        GenerateRooms();
    }

    Room CreateRoom(Room root)
    {
        int x = Random.Range(-1, 2);
        int y = Random.Range(-1, 2);

        if(rooms[(int)root.arrayPos.x + x, (int)root.arrayPos.y + y] == null && (x == 0 || y == 0))
        {
            Vector2 newWorldPos = new Vector2(root.pos.x + (x * roomDistance), root.pos.y + (y * roomDistance));
            Vector2 newArrayPos = new Vector2(root.arrayPos.x + x, root.arrayPos.y + y);

            rooms[(int)newArrayPos.x, (int)newArrayPos.y] = new Room(root, newWorldPos, newArrayPos, 1);
            return rooms[(int)newArrayPos.x, (int)newArrayPos.y];
        }
        if (!HasMoves(root))
        {
            return CreateRoom(root.root);
        }
        return CreateRoom(root);
    }

    void GenerateRooms()
    {
        //Rooms
        foreach(Room r in rooms)
        {
            if(r != null)
                Instantiate(roomPrefab, r.pos, Quaternion.identity);
        }

        //Corridors
        for(int r = 0; r < size * 3 - 1; r++)
        {
            for (int c = 0; c < size * 3 - 1; c++)
            {
                Room vert1 = rooms[r, c];
                Room vert2 = rooms[r, c + 1];

                Room r3 = rooms[r, c];
                Room r4 = rooms[r + 1, c];

                float newScale = roomPrefab.transform.localScale.x / 2;
                if (vert1 != null && vert2 != null)
                {
                    Vector2 midpoint = (vert1.pos + vert2.pos) / 2;
                    GameObject corridor = Instantiate(corridorPrefab, midpoint, Quaternion.identity);
                    corridor.transform.localScale = new Vector3((newScale / 2) + (newScale / 4), newScale, 1);
                }
                if (r3 != null && r4 != null)
                {
                    Vector2 midpoint = (r3.pos + r4.pos) / 2;
                    GameObject corridor = Instantiate(corridorPrefab, midpoint, Quaternion.identity);
                    corridor.transform.localScale = new Vector3(newScale, (newScale / 2) + (newScale / 4), 1);
                }
            }
        }
    }

    #region Helper Methods
    int filledRooms()
    {
        int count = 0;
        foreach(Room r in rooms)
        {
            if(r != null)
            {
                count++;
            }
        }
        return count;
    }

    public void ClearRooms()
    {
        GameObject[] roomObjects = GameObject.FindGameObjectsWithTag("Room");
        foreach (GameObject go in roomObjects)
        {
            DestroyImmediate(go);
        }
    }

    bool HasMoves(Room rm)
    {
        int x = (int)rm.arrayPos.x;
        int y = (int)rm.arrayPos.y;
        if(rooms[x + 1, y] != null && rooms[x, y + 1] != null && rooms[x - 1, y] != null && rooms[x, y - 1] != null)
        {
            return false;
        }
        return true;
    }
    #endregion
}
[System.Serializable]
public class Room
{
    public Room root;

    public Vector2 pos;
    public Vector2 arrayPos;

    public int type;

    public Room(Room rootRoom, Vector2 worldPosition, Vector2 arrayPosition, int roomType)
    {
        root = rootRoom;
        pos = worldPosition;
        arrayPos = arrayPosition;
        type = roomType;
    }
}
