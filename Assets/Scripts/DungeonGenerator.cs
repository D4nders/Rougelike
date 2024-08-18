using UnityEngine;
using UnityEngine.Tilemaps;

public class DungeonGenerator : MonoBehaviour
{
    public Tilemap tilemap;
    public TileBase floorTile;
    public TileBase wallTile;

    [SerializeField] public int width = 20;
    [SerializeField] public int height = 20;
    [SerializeField] public int initialWallDensity = 50; // Percentage
    [SerializeField] public int iterations = 5;

    private bool[,] grid;

    void Start()
    {
        grid = new bool[width, height];
        InitializeGrid();

        for (int i = 0; i < iterations; i++)
        {
            SimulateGeneration();
        }

        UpdateTilemap();
    }

    void InitializeGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                grid[x, y] = Random.Range(0, 100) < initialWallDensity;
            }
        }
    }

    void SimulateGeneration()
    {
        bool[,] newGrid = new bool[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int aliveNeighbors = CountAliveNeighbors(x, y);

                if (grid[x, y]) // Alive cell
                {
                    newGrid[x, y] = aliveNeighbors >= 4; // Survival rule
                }
                else // Dead cell
                {
                    newGrid[x, y] = aliveNeighbors == 3; // Birth rule
                }
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

    void UpdateTilemap()
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