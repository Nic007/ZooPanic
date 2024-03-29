﻿using System;
using UnityEngine;

namespace Assets.Scripts.Components
{
    [Serializable()]
    public class LevelDataComponent : MonoBehaviour{

        public enum TypesOfTiles { Grass = 0, IRoad, CrossRoad, TRoad, CornerRoad, Objective, Building }
        public enum TileRotation { North = 0, East, South, West, Size }

        [Serializable()]
        public class SerializableTile
        {
            public TypesOfTiles Type;
            public TileRotation Rotation;
        }

        public int NbTilesX;
        public int NbTilesY;
        public float TileSize;
        public GameObject[][]  TileMap;

        public TypesOfTiles     CurrentTile;
        public TileRotation     CurrentRotation;

        public GameObject       DummyBackground;
    }
}
