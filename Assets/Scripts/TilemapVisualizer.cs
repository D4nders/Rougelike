using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapVisualizer : MonoBehaviour
{
    [SerializeField]
    private Tilemap floorTilemap, wallTilemap;
    [SerializeField]
    private TileBase tile;

    public void PaintFloorTiles(IEnumerable<Vector2Int> floorPositions)
    {
        PaintTiles(floorPositions, floorTilemap, tile);
    }

    private void PaintTiles(IEnumerable<Vector2Int> positions, Tilemap tilemap, TileBase tile)
    {
        foreach (var position in positions)
        {
            PaintSingleTile(tilemap, tile, position);
        }
    }

    internal void PaintSingleWall(Vector2Int position)
    {
        PaintSingleTile(wallTilemap, tile, position);
        PaintSingleTile(floorTilemap, tile, position);
    }

    private void PaintSingleTile(Tilemap tilemap, TileBase tile, Vector2Int position)
    {
        var tilePosition = tilemap.WorldToCell((Vector3Int)position);
        tilemap.SetTile(tilePosition, tile);
    }

    public void PaintAndSmoothFloor(IEnumerable<Vector2Int> floorPositions)
    {
        PaintTiles(floorPositions, floorTilemap, tile);
        SmoothTiles(floorTilemap);
    }

    private void SmoothTiles(Tilemap tilemap)
    {
        // Get the bounds of the tilemap to iterate over
        BoundsInt bounds = tilemap.cellBounds;

        // Cellular Automata smoothing parameters
        int smoothingIterations = 4;
        int fillThreshold = 4;
        int removeThreshold = 2;

        for (int iteration = 0; iteration < smoothingIterations; iteration++)
        {
            for (int x = bounds.xMin; x < bounds.xMax; x++)
            {
                for (int y = bounds.yMin; y < bounds.yMax; y++)
                {
                    Vector3Int tilePosition = new Vector3Int(x, y, 0);


                    int filledNeighbors = CountFilledNeighbors(tilemap, x, y);

                    if (filledNeighbors < removeThreshold)
                    {
                        tilemap.SetTile(tilePosition, null); // Remove tile
                    }
                    else if (filledNeighbors > fillThreshold)
                    {
                        tilemap.SetTile(tilePosition, tile); // Fill tile
                    }
                }
            }
        }
    }

    private int CountFilledNeighbors(Tilemap tilemap, int x, int y)
    {
        int count = 0;
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0) continue; // Skip the center tile

                Vector3Int neighborPosition = new Vector3Int(x + i, y + j, 0);
                if (tilemap.HasTile(neighborPosition) && tilemap.GetTile(neighborPosition) == tile)
                {
                    count++;
                }
            }
        }
        return count;
    }

    public HashSet<Vector2Int> GetFloorTiles()
    {
        HashSet<Vector2Int> floorTiles = new HashSet<Vector2Int>();

        BoundsInt bounds = floorTilemap.cellBounds;
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int tilePosition = new Vector3Int(x, y, 0);
                if (floorTilemap.HasTile(tilePosition) && floorTilemap.GetTile(tilePosition) == tile)
                {
                    floorTiles.Add(new Vector2Int(x, y));
                }
            }
        }

        return floorTiles;
    }

    public void Clear()
    {
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
    }
}
