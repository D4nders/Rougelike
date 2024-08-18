using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class MapTunneler : MonoBehaviour
{
    private MapGenerator mapGenerator;
    private MapController mapController;

    void Start()
    {
        mapGenerator = GetComponent<MapGenerator>();
        mapController = GetComponent<MapController>();
    }

    public void ConnectIsolatedRooms()
    {
        List<HashSet<Vector2Int>> rooms = mapController.FindRooms();

        HashSet<Vector2Int> mainRoom = rooms[0]; // Assuming the first room is the largest
        for (int i = 1; i < rooms.Count; i++)
        {
            if (rooms[i].Count > mainRoom.Count)
            {
                mainRoom = rooms[i];
            }
        }

        List<HashSet<Vector2Int>> isolatedRooms = new List<HashSet<Vector2Int>>();

        foreach (var room in rooms)
        {
            if (room != mainRoom && !HasNeighboringRoom(room, rooms))
            {
                isolatedRooms.Add(room);
            }
        }

        foreach (var isolatedRoom in isolatedRooms)
        {
            HashSet<Vector2Int> randomRoom = rooms[Random.Range(0, rooms.Count)];
            while (randomRoom == isolatedRoom)
            {
                randomRoom = rooms[Random.Range(0, rooms.Count)];
            }

            ConnectRooms(isolatedRoom, randomRoom);
        }
    }

    bool HasNeighboringRoom(HashSet<Vector2Int> room, List<HashSet<Vector2Int>> allRooms)
    {
        foreach (var tile in room)
        {
            for (int x = tile.x - 1; x <= tile.x + 1; x++)
            {
                for (int y = tile.y - 1; y <= tile.y + 1; y++)
                {
                    if (x >= 0 && x < mapGenerator.width && y >= 0 && y < mapGenerator.height && !mapGenerator.grid[x, y])
                    {
                        foreach (var otherRoom in allRooms)
                        {
                            if (otherRoom != room && otherRoom.Contains(new Vector2Int(x, y)))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
        }
        return false;
    }

    void ConnectRooms(HashSet<Vector2Int> room1, HashSet<Vector2Int> room2)
    {
        Vector2Int point1 = room1.ElementAt(Random.Range(0, room1.Count));
        Vector2Int point2 = room2.ElementAt(Random.Range(0, room2.Count));

        // You'll need to implement your corridor carving logic here
        // For simplicity, let's create a straight horizontal/vertical corridor
        CreateHorizontalCorridor(point1.x, point2.x, point1.y);
        CreateVerticalCorridor(point1.y, point2.y, point2.x);

        // Or, for a more sophisticated approach, use A* or another pathfinding algorithm
    }

    void CreateHorizontalCorridor(int x1, int x2, int y)
    {
        int minX = Mathf.Min(x1, x2);
        int maxX = Mathf.Max(x1, x2);

        for (int x = minX; x <= maxX; x++)
        {
            mapGenerator.grid[x, y] = false; // Carve path
        }
    }

    void CreateVerticalCorridor(int y1, int y2, int x)
    {
        int minY = Mathf.Min(y1, y2);
        int maxY = Mathf.Max(y1, y2);

        for (int y = minY; y <= maxY; y++)
        {
            mapGenerator.grid[x, y] = false; // Carve path
        }
    }
}