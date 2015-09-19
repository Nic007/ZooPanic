using System;

namespace Assets.Scripts
{
    [Serializable()]
    public class LevelData {

        public enum TypesOfTiles { Grass = 0, Road, CrossRoad, TRoad, CornerRoad }
        public enum TileRotation { North = 0, East, South, West }

        [Serializable()]
        public class SerializableTile
        {
            public TypesOfTiles Type;
            public TileRotation Rotation;
        }

        public int NbTilesX = 16;
        public int NbTilesY = 9;
        public SerializableTile[][] TilesGrid;
    }
}
