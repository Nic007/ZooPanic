using Assets.Scripts;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor
{
    [ExecuteInEditMode]
    [CustomEditor(typeof(LevelData))]
    public class TileEditorComponent : UnityEditor.Editor
    {
        [SerializeField]
        private LevelData _levelData;

        public struct TileIndex
        {
            public int x { get; set; } 
            public int y { get; set; }
        }

        void OnEnable()
        {
            _levelData = (LevelData) target;
            if (_levelData == null)
            {
                _levelData = new LevelData();
            }
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
            _levelData.CurrentTile = (LevelData.TypesOfTiles)EditorGUILayout.EnumPopup(_levelData.CurrentTile);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Rotation");
            _levelData.CurrentRotation = (LevelData.TileRotation)EditorGUILayout.EnumPopup(_levelData.CurrentRotation);
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
            var gameObject = Object.Instantiate(Resources.Load("Tiles/" + _levelData.CurrentTile)) as GameObject;
            if (gameObject == null)
            {
                Debug.LogError("Cannot create the tile.");
                return;
            }

            var tileIndex = GetTileIndex(pos); 
            gameObject.transform.position = new Vector3(tileIndex.x * _levelData.TileSize - (_levelData.NbTilesX * _levelData.TileSize) / 2 + _levelData.TileSize / 2, -1 * tileIndex.y * _levelData.TileSize + (_levelData.NbTilesY * _levelData.TileSize) / 2 - _levelData.TileSize / 2);
            gameObject.transform.localScale = new Vector3(_levelData.TileSize, _levelData.TileSize, 0.1F);

			if (_levelData.CurrentRotation == LevelData.TileRotation.North) 
			{
				gameObject.transform.Rotate(Vector3.right);
			} 
			if (_levelData.CurrentRotation == LevelData.TileRotation.East) 
			{
				gameObject.transform.Rotate(Vector3.back * 90);
			}
			if (_levelData.CurrentRotation == LevelData.TileRotation.South) 
			{
				gameObject.transform.Rotate(Vector3.back * 180);
			}
			if (_levelData.CurrentRotation == LevelData.TileRotation.West) 
			{
				gameObject.transform.Rotate(Vector3.back * 270);
			}

            var currentTile = _levelData.TileMap[tileIndex.y][tileIndex.x];
            if (currentTile != null)
            {
                DestroyImmediate(currentTile);
                currentTile = null;
            }

            _levelData.TileMap[tileIndex.y][tileIndex.x] = gameObject;
            AssignNeighbors(tileIndex);
        }

		//Assigne les voisins de la tuile créée et assigne la tuile en tant que voisin a ses voisins
        private void AssignNeighbors(TileIndex tileIndex)
        {
            var currentTile = _levelData.TileMap[tileIndex.y][tileIndex.x];
            var northTile = tileIndex.y > 0 ? _levelData.TileMap[tileIndex.y - 1][tileIndex.x] : null;
            var eastTile = tileIndex.x < _levelData.NbTilesX - 1 ? _levelData.TileMap[tileIndex.y][tileIndex.x + 1] : null;
            var southTile = tileIndex.y < _levelData.NbTilesY - 1 ? _levelData.TileMap[tileIndex.y + 1][tileIndex.x] : null;
            var westTile = tileIndex.x > 0 ? _levelData.TileMap[tileIndex.y][tileIndex.x - 1] : null;

			//Je sais qu'on se répete mais va savoir pourquoi ca marche pas pour west et south si je répete pas... apres c'est juste l'éditeur je vis avec
            currentTile.GetComponent<BasicTileComponent>().NorthTile = northTile;
            if (northTile != null)
            {
				currentTile.GetComponent<BasicTileComponent>().NorthTile=_levelData.TileMap[tileIndex.y - 1][tileIndex.x];
                northTile.GetComponent<BasicTileComponent>().SouthTile = currentTile;
            }

            currentTile.GetComponent<BasicTileComponent>().EastTile = eastTile;
            if (eastTile != null)
            {
				currentTile.GetComponent<BasicTileComponent>().EastTile =_levelData.TileMap[tileIndex.y][tileIndex.x + 1];
                eastTile.GetComponent<BasicTileComponent>().WestTile = currentTile;
            }

            currentTile.GetComponent<BasicTileComponent>().SouthTile = southTile;
            if (southTile != null)
            {
				currentTile.GetComponent<BasicTileComponent>().SouthTile = _levelData.TileMap[tileIndex.y + 1][tileIndex.x];
                southTile.GetComponent<BasicTileComponent>().NorthTile = currentTile;
            }

            currentTile.GetComponent<BasicTileComponent>().WestTile = northTile;
            if (westTile != null)
            {
				currentTile.GetComponent<BasicTileComponent>().WestTile=_levelData.TileMap[tileIndex.y][tileIndex.x - 1];
                westTile.GetComponent<BasicTileComponent>().EastTile = currentTile;
            }
        }

        private TileIndex GetTileIndex(Vector3 pos)
        {
            var tileIndex = new TileIndex();

            var deltaX = pos.x + _levelData.NbTilesX * _levelData.TileSize / 2;
            tileIndex.x = (int)(deltaX / (_levelData.TileSize));

            var deltaY = _levelData.NbTilesY * _levelData.TileSize / 2 - pos.y;
            tileIndex.y = (int)(deltaY / (_levelData.TileSize));

            return tileIndex;
        }
    }
}
