using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapController : MonoBehaviour
{
    public Tilemap tilemap;
    public TileBase floorTile;
    public TileBase wallTile;

    [SerializeField] public int width = 20;
    [SerializeField] public int height = 20;
    [SerializeField] public int randomFillPercent = 50; // Percentage
    [SerializeField] public int iterations = 5;
    [SerializeField] bool connectRooms = true;
    [SerializeField] bool colorRooms = false;

    public bool[,] grid;

    public GameObject playerPrefab;

    void Start()
    {
        Generate(); // Generate the initial map

        if (connectRooms)
        {
            ConnectIsolatedRooms(); // Connect any isolated rooms
        }

        UpdateTilemap(); // Update the Tilemap to visualize the changes

        Vector3Int playerSpawnPosition = FindMainRoomAndSpawnPlayer();

        if (playerSpawnPosition != Vector3Int.zero)
        {
            Instantiate(playerPrefab, tilemap.CellToWorld(playerSpawnPosition), Quaternion.identity);
        }
        else
        {
            Debug.LogError("No suitable main room found for player spawn!");
        }
    }

    public void Generate()
    {
        grid = new bool[width, height];
        RandomFillMap();

        for (int i = 0; i < iterations; i++)
        {
            SmoothMap();
        }
    }

    public void FloodFill(int x, int y, bool[,] visited, HashSet<Vector2Int> room)
    {
        if (x < 0 || x >= width || y < 0 || y >= height || grid[x, y] || visited[x, y])
        {
            return; // Out of bounds, wall, or already visited
        }

        visited[x, y] = true;
        room.Add(new Vector2Int(x, y));

        FloodFill(x + 1, y, visited, room);
        FloodFill(x - 1, y, visited, room);
        FloodFill(x, y + 1, visited, room);
        FloodFill(x, y - 1, visited, room);
    }

    void RandomFillMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                {
                    grid[x, y] = true; // Borders are walls
                }
                else
                {
                    grid[x, y] = Random.Range(0, 100) < randomFillPercent;
                }
            }
        }
    }

    void SmoothMap()
    {
        bool[,] newGrid = new bool[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int neighbourWallTiles = CountAliveNeighbors(x, y);

                if (neighbourWallTiles > 4)
                    newGrid[x, y] = true;
                else if (neighbourWallTiles < 4)
                    newGrid[x, y] = false;
                else
                    newGrid[x, y] = grid[x, y]; // Keep the same state if exactly 4 neighbors
            }
        }

        grid = newGrid;
    }

    int CountAliveNeighbors(int x, int y)
    {
        int count = 0;

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0) continue; // Skip the cell itself

                int neighborX = x + i;
                int neighborY = y + j;

                if (neighborX >= 0 && neighborX < width && neighborY >= 0 && neighborY < height && grid[neighborX, neighborY])
                {
                    count++;
                }
            }
        }

        return count;
    }

    public void UpdateTilemap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                tilemap.SetTile(new Vector3Int(x, y, 0), grid[x, y] ? wallTile : floorTile);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (colorRooms)
        {
            List<HashSet<Vector2Int>> rooms = FindRooms();
            Color[] colors = new Color[20];

            colors[0] = Color.red;
            colors[1] = Color.green;
            colors[2] = Color.blue;
            colors[3] = Color.yellow;
            colors[4] = Color.cyan;
            colors[5] = Color.magenta;
            colors[6] = Color.white;
            colors[7] = Color.black;
            colors[8] = Color.gray;
            colors[9] = new Color(1, 0.5f, 0); // Orange
            colors[10] = new Color(0.5f, 0, 0.5f); // Purple
            colors[11] = new Color(0, 0.5f, 0); // Dark Green
            colors[12] = new Color(0.5f, 0.5f, 0); // Olive
            colors[13] = new Color(0, 0.5f, 0.5f); // Teal
            colors[14] = new Color(0.5f, 0, 1); // Indigo
            colors[15] = new Color(1, 1, 0.5f); // Light Yellow
            colors[16] = new Color(1, 0.5f, 1); // Pink
            colors[17] = new Color(0.5f, 1, 1); // Light Cyan
            colors[18] = new Color(1, 0.5f, 0.5f); // Light Pink
            colors[19] = new Color(0.75f, 0.75f, 0.75f); // Light Gray

            for (int i = 0; i < rooms.Count; i++)
            {
                Color roomColor = colors[i % colors.Length];

                foreach (Vector2Int tile in rooms[i])
                {
                    Vector3 tileWorldPos = tilemap.CellToWorld(new Vector3Int(tile.x, tile.y, 0));
                    Gizmos.color = roomColor;
                    Gizmos.DrawWireCube(tileWorldPos + new Vector3(0.5f, 0.5f, 0), Vector3.one);
                }
            }
        }
    }

    private Vector3Int FindMainRoomAndSpawnPlayer()
    {
        List<HashSet<Vector2Int>> rooms = FindRooms();

        if (rooms.Count == 0)
        {
            return Vector3Int.zero; // No rooms found
        }

        HashSet<Vector2Int> mainRoom = rooms[0]; // Assume the first room is the largest

        for (int i = 1; i < rooms.Count; i++)
        {
            if (rooms[i].Count > mainRoom.Count)
            {
                mainRoom = rooms[i];
            }
        }

        // Randomly choose a tile within the main room
        Vector2Int[] roomTiles = new Vector2Int[mainRoom.Count];
        mainRoom.CopyTo(roomTiles);
        Vector2Int randomTile = roomTiles[Random.Range(0, roomTiles.Length)];

        return new Vector3Int(randomTile.x, randomTile.y, 0);
    }

    public List<HashSet<Vector2Int>> FindRooms()
    {
        List<HashSet<Vector2Int>> rooms = new List<HashSet<Vector2Int>>();

        bool[,] visited = new bool[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (!grid[x, y] && !visited[x, y]) // If it's a path tile and not visited
                {
                    HashSet<Vector2Int> newRoom = new HashSet<Vector2Int>();
                    FloodFill(x, y, visited, newRoom);
                    rooms.Add(newRoom);
                }
            }
        }

        return rooms;
    }

    public void ConnectIsolatedRooms()
    {
        List<HashSet<Vector2Int>> rooms = FindRooms();

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
                    if (x >= 0 && x < width && y >= 0 && y < height && !grid[x, y])
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
            grid[x, y] = false; // Carve path
        }
    }

    void CreateVerticalCorridor(int y1, int y2, int x)
    {
        int minY = Mathf.Min(y1, y2);
        int maxY = Mathf.Max(y1, y2);

        for (int y = minY; y <= maxY; y++)
        {
            grid[x, y] = false; // Carve path
        }
    }
}
