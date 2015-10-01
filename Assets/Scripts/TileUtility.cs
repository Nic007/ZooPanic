using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public static class TileUtility
    {
        public static LevelData CurrentLevelData { get; set; }

        public static Vector3 GetTilePosition(TileIndex tileIndex)
        {
            return new Vector3(
                x: tileIndex.x * CurrentLevelData.TileSize - (CurrentLevelData.NbTilesX * CurrentLevelData.TileSize) / 2 + CurrentLevelData.TileSize / 2, 
                y: -1 * tileIndex.y * CurrentLevelData.TileSize + (CurrentLevelData.NbTilesY * CurrentLevelData.TileSize) / 2 - CurrentLevelData.TileSize / 2);
        }

        public static TileIndex GetTileIndex(Vector3 pos)
        {
            var tileIndex = new TileIndex();

            var deltaX = pos.x + CurrentLevelData.NbTilesX * CurrentLevelData.TileSize / 2;
            tileIndex.x = (int)(deltaX / (CurrentLevelData.TileSize));

            var deltaY = CurrentLevelData.NbTilesY * CurrentLevelData.TileSize / 2 - pos.y;
            tileIndex.y = (int)(deltaY / (CurrentLevelData.TileSize));

            return tileIndex;
        }

                
    }
}
