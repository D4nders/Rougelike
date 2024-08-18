using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    public Tilemap tilemap;
    public TileBase floorTile;
    public TileBase wallTile;

    [SerializeField] public int width = 20;
    [SerializeField] public int height = 20;
    [SerializeField] public int randomFillPercent = 50; // Percentage
    [SerializeField] public int iterations = 5;

    public bool[,] grid;

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
}