using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public static class TileUtility
    {
        public static Vector3 GetTilePosition(LevelData currentLevelData, TileIndex tileIndex)
        {
            return new Vector3(
                x: tileIndex.x * currentLevelData.TileSize - (currentLevelData.NbTilesX * currentLevelData.TileSize) / 2 + currentLevelData.TileSize / 2, 
                y: -1 * tileIndex.y * currentLevelData.TileSize + (currentLevelData.NbTilesY * currentLevelData.TileSize) / 2 - currentLevelData.TileSize / 2);
        }

        public static TileIndex GetTileIndex(LevelData currentLevelData, Vector3 pos)
        {
            var tileIndex = new TileIndex();

            var deltaX = pos.x + currentLevelData.NbTilesX * currentLevelData.TileSize / 2;
            tileIndex.x = (int)(deltaX / (currentLevelData.TileSize));

            var deltaY = currentLevelData.NbTilesY * currentLevelData.TileSize / 2 - pos.y;
            tileIndex.y = (int)(deltaY / (currentLevelData.TileSize));

            return tileIndex;
        }

                
    }
}
