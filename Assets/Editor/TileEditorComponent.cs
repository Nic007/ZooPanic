using Assets.Scripts;
using Assets.Scripts.Components;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor
{
    [ExecuteInEditMode]
    [CustomEditor(typeof(LevelDataComponent))]
    public class TileEditorComponent : UnityEditor.Editor
    {
        [SerializeField]
        private LevelDataComponent _levelData;

        void OnEnable()
        {
            _levelData = (LevelDataComponent) target ?? new LevelDataComponent();
        }

        public override void OnInspectorGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("NbTilesX");
            _levelData.NbTilesX = EditorGUILayout.IntField(_levelData.NbTilesX, GUILayout.Width(50));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("NbTilesY");
            _levelData.NbTilesY = EditorGUILayout.IntField(_levelData.NbTilesY, GUILayout.Width(50));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("TileSize");
            _levelData.TileSize = EditorGUILayout.FloatField(_levelData.TileSize, GUILayout.Width(50));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("CurrentTile");
            _levelData.CurrentTile = (LevelDataComponent.TypesOfTiles)EditorGUILayout.EnumPopup(_levelData.CurrentTile);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Rotation");
            _levelData.CurrentRotation = (LevelDataComponent.TileRotation)EditorGUILayout.EnumPopup(_levelData.CurrentRotation);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("DummyBackgound");
            _levelData.DummyBackground = (GameObject)EditorGUILayout.ObjectField(_levelData.DummyBackground, typeof(GameObject), true);
            GUILayout.EndHorizontal();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(_levelData);
            }
        }

        void OnSceneGUI()
        {
            if (_levelData.TileMap == null || _levelData.TileMap.Length != _levelData.NbTilesY || _levelData.TileMap.Length == 0 || _levelData.TileMap[0].Length != _levelData.NbTilesX || _levelData.TileMap[1].Length == 0)
            {
                _levelData.TileMap = new GameObject[_levelData.NbTilesY][];
                for (var i = 0; i < _levelData.NbTilesY; ++i)
                {
                    _levelData.TileMap[i] = new GameObject[_levelData.NbTilesX];
                }
            }

            if (_levelData.DummyBackground != null)
            {
                _levelData.DummyBackground.transform.position = new Vector3(0, 0, 0);
                _levelData.DummyBackground.transform.localScale = new Vector3(_levelData.NbTilesX * _levelData.TileSize, _levelData.NbTilesY * _levelData.TileSize);
            }

            var controlId = GUIUtility.GetControlID(FocusType.Passive);
            switch (Event.current.GetTypeForControl(controlId))
            {
                case EventType.MouseDown:
                {
                    GUIUtility.hotControl = controlId;

                    var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                    RaycastHit hitInfo;
                    if(Physics.Raycast(ray, out hitInfo))
                    {
                        OnAddTile(hitInfo.point);
                    } 

                    Event.current.Use();
                    break;
                }
                case EventType.MouseUp:
                {
                    GUIUtility.hotControl = 0;
                    Event.current.Use();
                    break;
                } 
            }
        }

        void OnDrawGizmos()
        {
            for (var i = 0; i <= _levelData.NbTilesX; ++i)
            {
                var xPos = -1 * _levelData.NbTilesX * _levelData.TileSize / 2 + i * _levelData.TileSize;
                var fromVector = new Vector3(xPos, _levelData.NbTilesY*_levelData.TileSize/2);
                var toVector = new Vector3(xPos, -1 * _levelData.NbTilesY * _levelData.TileSize / 2);

                Gizmos.DrawLine(fromVector, toVector);
            }

            for (int i = 0; i <= _levelData.NbTilesY; ++i)
            {
                var yPos = -1 * _levelData.NbTilesY * _levelData.TileSize / 2 + i * _levelData.TileSize;
                var fromVector = new Vector3(_levelData.NbTilesX*_levelData.TileSize/2, yPos);
                var toVector = new Vector3(-1 * _levelData.NbTilesX * _levelData.TileSize / 2, yPos);

                Gizmos.DrawLine(fromVector, toVector);
            }
        }

        private void OnAddTile(Vector3 pos)
        {
            ReloadTileMap();

            var gameObject = Object.Instantiate(Resources.Load("Tiles/" + _levelData.CurrentTile)) as GameObject;
            if (gameObject == null)
            {
                Debug.LogError("Cannot create the tile.");
                return;
            }

            var tileIndex = TileUtility.GetTileIndex(_levelData, pos);
            gameObject.transform.position = TileUtility.GetTilePosition(_levelData, tileIndex);
            gameObject.transform.localScale = new Vector3(_levelData.TileSize, _levelData.TileSize, 0.1F);

			if (_levelData.CurrentRotation == LevelDataComponent.TileRotation.North) 
			{
				gameObject.transform.Rotate(Vector3.right);
			} 
			if (_levelData.CurrentRotation == LevelDataComponent.TileRotation.East) 
			{
				gameObject.transform.Rotate(Vector3.back * 90);
			}
			if (_levelData.CurrentRotation == LevelDataComponent.TileRotation.South) 
			{
				gameObject.transform.Rotate(Vector3.back * 180);
			}
			if (_levelData.CurrentRotation == LevelDataComponent.TileRotation.West) 
			{
				gameObject.transform.Rotate(Vector3.back * 270);
			}

            var tileComponent = gameObject.GetComponent<TileComponent>();
            tileComponent.GameManager = GameObject.Find("GameManager");

            tileComponent.CurrentRotation = _levelData.CurrentRotation;
            var newNeighborsState = new TileComponent.PathState[4];
            for (var i = 0; i < (int) LevelDataComponent.TileRotation.Size; ++i)
            {
                newNeighborsState[i] =
                    tileComponent.NeighborsState[(i - (int)_levelData.CurrentRotation + (int)LevelDataComponent.TileRotation.Size) % (int)LevelDataComponent.TileRotation.Size];
            }
            tileComponent.NeighborsState = newNeighborsState;

            var currentTile = _levelData.TileMap[tileIndex.y][tileIndex.x];
            if (currentTile != null)
            {
                DestroyImmediate(currentTile);
                currentTile = null;
            }

            _levelData.TileMap[tileIndex.y][tileIndex.x] = gameObject;
            tileComponent.CurrentLocation = tileIndex;
            AssignNeighbors(tileIndex);

            gameObject.name = gameObject.name.Substring(0, gameObject.name.IndexOf('('));
            gameObject.name += "_" + tileIndex.y + "_" + tileIndex.x;
            gameObject.transform.parent = GameObject.Find("LevelTiles").transform;
        }

		//Assigne les voisins de la tuile créée et assigne la tuile en tant que voisin a ses voisins
        private void AssignNeighbors(TileIndex tileIndex)
        {
            var currentTile = _levelData.TileMap[tileIndex.y][tileIndex.x];
            var neighbors = new GameObject[]
            {
                tileIndex.y > 0 ? _levelData.TileMap[tileIndex.y - 1][tileIndex.x] : null,
                tileIndex.x < _levelData.NbTilesX - 1 ? _levelData.TileMap[tileIndex.y][tileIndex.x + 1] : null,
                tileIndex.y < _levelData.NbTilesY - 1 ? _levelData.TileMap[tileIndex.y + 1][tileIndex.x] : null,
                tileIndex.x > 0 ? _levelData.TileMap[tileIndex.y][tileIndex.x - 1] : null
            };

            var basicTileComponent = currentTile.GetComponent<TileComponent>();
            for (var i = 0; i < sizeof (LevelDataComponent.TileRotation); ++i)
            {
                basicTileComponent.NeighborsObjects[i] = neighbors[i];
                if (neighbors[i] != null)
                {
                    neighbors[i].GetComponent<TileComponent>().NeighborsObjects[
                        (i + 2) % (int)LevelDataComponent.TileRotation.Size] = currentTile;
                }
            }
        }

        private void ReloadTileMap()
        {
            var tiles = GameObject.Find("LevelTiles");
            for (var i = 0; i < tiles.transform.childCount; ++i)
            {
                var tile = tiles.transform.GetChild(i).gameObject;
                var tileComp = tile.GetComponent<TileComponent>();
                _levelData.TileMap[tileComp.CurrentLocation.y][tileComp.CurrentLocation.x] = tile;
            }
        }

        void Awake()
        {
        }

        void Start()
        {
        }
    }
}
