using UnityEditor;
using UnityEngine;

namespace Assets.Editor
{
    [ExecuteInEditMode]
    [CustomEditor(typeof(TileEditorComponent))]
    public class TileEditorComponent : UnityEditor.Editor 
    {
        public enum TypesOfTiles { Grass = 0, Road, CrossRoad, TRoad, CornerRoad }

        public int              NbTilesX;
        public int              NbTilesY;
        public float            TileSize;

        public TypesOfTiles     CurrentTile;
        public bool             InsertionMode;

        public GameObject       DummyBackground;

        void OnSceneGUI()
        {
            if (Event.current.button == 0 && Event.current.type == EventType.MouseDown)
            {
                var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                RaycastHit hitInfo;
                if(Physics.Raycast(ray, out hitInfo))
                {
                    var gameObject = PrefabUtility.InstantiatePrefab(Resources.Load("Tiles/BasicTile")) as GameObject;
                    gameObject.transform.position = hitInfo.point;
                } 

                Event.current.Use();
            }
        }

        void OnDrawGizmos()
        {
            if (DummyBackground != null)
            {
                DummyBackground.transform.position = new Vector3(0, 0, 0);
                DummyBackground.transform.localScale = new Vector3(NbTilesX * TileSize, NbTilesY * TileSize);
            }

            for (int i = 0; i <= NbTilesX; ++i)
            {
                var xPos = -1 * NbTilesX * TileSize / 2 + i * TileSize;
                var fromVector = new Vector3(xPos, NbTilesY*TileSize/2);
                var toVector = new Vector3(xPos, -1 * NbTilesY * TileSize / 2);

                Gizmos.DrawLine(fromVector, toVector);
            }

            for (int i = 0; i <= NbTilesY; ++i)
            {
                var yPos = -1 * NbTilesY * TileSize / 2 + i * TileSize;
                var fromVector = new Vector3(NbTilesX*TileSize/2, yPos);
                var toVector = new Vector3(-1 * NbTilesX * TileSize / 2, yPos);

                Gizmos.DrawLine(fromVector, toVector);
            }
        }
    }
}
