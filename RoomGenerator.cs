using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    #region Fields
    private GameObject roomPrefab;
    public Vector3 roomSize;

    private GameObject corridorPrefab;
    public float corridorWidth;

    public Vector2 origin;

    public int size;
    public float roomDistance
    {
        get
        {
            return Mathf.Max(roomSize.x + (roomSize.x / 2), roomSize.y + (roomSize.y / 2));
        }
    }

    private Room[,] rooms;
    #endregion

    public void GenerateLevel()
    {
        ClearRooms();



        rooms = new Room[size * 2, size * 2];
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
        Sprite square = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 4, 4), new Vector2(0.5f, 0.5f), 4);
        #region Create Room Prefab
        roomPrefab = new GameObject("Room");
        roomPrefab.tag = "Room";
        roomPrefab.SetActive(false);
        roomPrefab.hideFlags = HideFlags.HideInHierarchy;

        SpriteRenderer roomRend = roomPrefab.AddComponent<SpriteRenderer>();
        roomRend.sprite = square;

        roomPrefab.transform.localScale = roomSize;
        #endregion
        #region Create Corridor Prefab
        corridorPrefab = new GameObject("Corridor");
        corridorPrefab.tag = "Room";
        corridorPrefab.SetActive(false);
        corridorPrefab.hideFlags = HideFlags.HideInHierarchy;

        SpriteRenderer corridorRend = corridorPrefab.AddComponent<SpriteRenderer>();
        corridorRend.sprite = square;
        #endregion

        //Room
        foreach (Room r in rooms)
        {
            if(r != null)
            {
                GameObject roomGo = Instantiate(roomPrefab, r.pos, Quaternion.identity);
                roomGo.SetActive(true);
            }
        }

        //Corridor
        for(int r = 0; r < size * 2 - 1; r++)
        {
            for (int c = 0; c < size * 2 - 1; c++)
            {
                Room vert1 = rooms[r, c];
                Room vert2 = rooms[r, c + 1];

                Room hori1 = rooms[r, c];
                Room hori2 = rooms[r + 1, c];

                if (vert1 != null && vert2 != null)
                {
                    Vector2 midpoint = (vert1.pos + vert2.pos) / 2;
                    GameObject corridor = Instantiate(corridorPrefab, midpoint, Quaternion.identity);
                    corridor.SetActive(true);

                    float yScale = Mathf.Abs(vert1.pos.y - vert2.pos.y) - roomSize.y;
                    corridor.transform.localScale = new Vector3(Mathf.Min(corridorWidth, roomSize.x / 2), yScale, 1);
                }
                if (hori1 != null && hori2 != null)
                {
                    Vector2 midpoint = (hori1.pos + hori2.pos) / 2;
                    GameObject corridor = Instantiate(corridorPrefab, midpoint, Quaternion.identity);
                    corridor.SetActive(true);

                    float xScale = Mathf.Abs(hori1.pos.x - hori2.pos.x) - roomSize.x;
                    corridor.transform.localScale = new Vector3(xScale, Mathf.Min(corridorWidth, roomSize.y / 2), 1);
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

