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
