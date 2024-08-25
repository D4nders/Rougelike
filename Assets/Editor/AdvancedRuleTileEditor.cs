using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace UnityEditor
{
    [CustomEditor(typeof(WallTile))]
    [CanEditMultipleObjects]
    public class AdvancedRuleTileEditor : RuleTileEditor
    {
        public Texture2D test;
        public Texture2D[] neighborTextures = new Texture2D[16];

        public override void RuleOnGUI(Rect rect, Vector3Int position, int neighbor)
        {
            // Check if a texture is assigned for this neighbor
            if (neighbor >= 3 && neighbor < neighborTextures.Length && neighborTextures[neighbor] != null)
            {
                GUI.DrawTexture(rect, neighborTextures[neighbor]);
                return;
            }
            base.RuleOnGUI(rect, position, neighbor);
        }
    }
}
