using System;
using UnityEngine;

//using static TileEditorComponent;

namespace Assets.Scripts
{
    public class BasicTileComponent : MonoBehaviour
    {
        public enum PathState
        {
            NotExisting,
            Available,
            Blocked
        };

        public GameObject[] NeighborsObjects = {null, null, null, null};

        public PathState[] NeighborsState =
        {
            PathState.NotExisting,
            PathState.NotExisting,
            PathState.NotExisting,
            PathState.NotExisting
        };

        [SerializeField]
        public TileIndex CurrentLocation;
        public LevelData.TileRotation CurrentRotation;

        // Use this for initialization
        void Start ()
        {
            if (TileUtility.CurrentLevelData.TileMap == null || TileUtility.CurrentLevelData.TileMap.Length != TileUtility.CurrentLevelData.NbTilesY || TileUtility.CurrentLevelData.TileMap.Length == 0 || TileUtility.CurrentLevelData.TileMap[0].Length != TileUtility.CurrentLevelData.NbTilesX || TileUtility.CurrentLevelData.TileMap[1].Length == 0)
            {
                TileUtility.CurrentLevelData.TileMap = new GameObject[TileUtility.CurrentLevelData.NbTilesY][];
                for (var i = 0; i < TileUtility.CurrentLevelData.NbTilesY; ++i)
                {
                    TileUtility.CurrentLevelData.TileMap[i] = new GameObject[TileUtility.CurrentLevelData.NbTilesX];
                }
            }

            TileUtility.CurrentLevelData.TileMap[CurrentLocation.y][CurrentLocation.x] = gameObject;

        }

        void OnEditor()
        {
        
        }
	
        // Update is called once per frame
        void Update ()
        {
	
        }
    }
}
