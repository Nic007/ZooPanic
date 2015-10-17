using UnityEngine;

namespace Assets.Scripts.Components
{
    public class TileComponent : MonoBehaviour
    {
        public enum PathState
        {
            NotExisting,
            Available,
            Blocked
        };

        public GameObject[] NeighborsObjects = {null, null, null, null};
        public GameObject GameManager;

        public PathState[] NeighborsState =
        {
            PathState.NotExisting,
            PathState.NotExisting,
            PathState.NotExisting,
            PathState.NotExisting
        };

        [SerializeField]
        public TileIndex CurrentLocation;
        public LevelDataComponent.TileRotation CurrentRotation;

        private LevelDataComponent _currentLevelData;

        // Use this for initialization
        void Start ()
        {
            _currentLevelData = GameManager.GetComponent<LevelDataComponent>();

            if (_currentLevelData.TileMap == null || _currentLevelData.TileMap.Length != _currentLevelData.NbTilesY || _currentLevelData.TileMap.Length == 0 || _currentLevelData.TileMap[0].Length != _currentLevelData.NbTilesX || _currentLevelData.TileMap[1].Length == 0)
            {
                _currentLevelData.TileMap = new GameObject[_currentLevelData.NbTilesY][];
                for (var i = 0; i < _currentLevelData.NbTilesY; ++i)
                {
                    _currentLevelData.TileMap[i] = new GameObject[_currentLevelData.NbTilesX];
                }
            }

            _currentLevelData.TileMap[CurrentLocation.y][CurrentLocation.x] = gameObject;

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
