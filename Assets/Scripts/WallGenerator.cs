using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WallGenerator
{
    public static void CreateWallsInDirection(HashSet<Vector2Int> floorPositions, TilemapVisualizer tilemapVisualizer)
    {
        var wallPositions = FindWallsInDirections(floorPositions, Direction2D.eightDirectionsList);
        foreach (var position in wallPositions)
        {
            tilemapVisualizer.PaintSingleWall(position);
        }
    }

    private static HashSet<Vector2Int> FindWallsInDirections(HashSet<Vector2Int> floorPositions, List<Vector2Int> directionList)
    {
        HashSet<Vector2Int> wallPositions = new HashSet<Vector2Int>();

        foreach (var position in floorPositions)
        {
            foreach (var direction in directionList)
            {
                var neighbourPosition = position + direction;
                if (floorPositions.Contains(neighbourPosition) == false)

                {
                    // Prioritize cardinal directions
                    if (Direction2D.IsCardinalDirection(direction) || !wallPositions.Contains(neighbourPosition))
                    {
                        wallPositions.Add(neighbourPosition);
                    }
                }
            }
        }
        return wallPositions;
    }
}
