using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

[CreateAssetMenu]
public class WallTile : RuleTile<WallTile.Neighbor> {
    public Tilemap floorTilemap;
    public Tilemap wallTilemap;

    public class Neighbor : RuleTile.TilingRule.Neighbor
    {
        public const int Wall = 3; // Represents a wall tile
        public const int Floor = 4; // Represents a floor tile
    }

    public override bool RuleMatch(int neighbor, TileBase tile)
    {
        switch (neighbor)
        {
            case Neighbor.This: return Check_This(tile);
            case Neighbor.NotThis: return Check_NotThis(tile);
            case Neighbor.Wall: return Check_Wall(tile);
            case Neighbor.Floor: return Check_Floor(tile);
        }
        return base.RuleMatch(neighbor, tile);
    }

    private bool Check_This(TileBase tile)
    {
        return tile == this;
    }

    private bool Check_NotThis(TileBase tile)
    {
        return !Check_This(tile);
    }

    private bool Check_Wall(TileBase tile)
    {
        return HasAdjacentTile(tile, wallTilemap);
    }

    private bool Check_Floor(TileBase tile)
    {
        return HasAdjacentTile(tile, floorTilemap);
    }

    private bool HasAdjacentTile(TileBase tile, Tilemap tilemap)
    {
        Vector3Int cellPosition = tilemap.WorldToCell(tile.gameObject.transform.position);

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue; // Skip the current cell

                Vector3Int neighborCell = new Vector3Int(cellPosition.x + x, cellPosition.y + y, cellPosition.z);
                if (tilemap.HasTile(neighborCell))
                {
                    return true;
                }
            }
        }

        return false;
    }
}