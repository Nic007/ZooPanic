using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngineInternal;

namespace Assets.Scripts
{
    public class InputComponent : MonoBehaviour
    {
        private GameObject _selectionnedAnimal;

        // Visual effect for the simulation
        private GameObject _selectionVisual;

        private GameObject _currentTile = null;
        private LinkedList<GameObject> _tempPath;
        private bool _tilePathMode;

        // Use this for initialization
        void Start ()
        {
            _selectionnedAnimal = null;
            _tilePathMode = false;
            _currentTile = null;

            _selectionVisual = Instantiate(Resources.Load("SelectionVisual")) as GameObject;
            if (_selectionVisual != null)
            {
                _selectionVisual.GetComponent<MeshRenderer>().enabled = false;
            }
            else
            {
                Debug.LogError("Selection Visual not created!");
            }
        }
	
        // Update is called once per frame
        void Update ()
        {
            // Mobile input management
            foreach (var touch in Input.touches)
            {
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        Selection(touch.position);
                        break;
                    case TouchPhase.Moved:
                        BuildPath(touch.position);
                        break;
                    case TouchPhase.Ended:
                    case TouchPhase.Canceled:
                        EndPathBuilding();
                        break;
                }
            }

            // Desktop input management
            if(Input.GetMouseButtonDown(0))
            {
                Selection(Input.mousePosition);
            }

            if (Input.GetMouseButton(0))
            {
                BuildPath(Input.mousePosition);                
            }

            if (Input.GetMouseButtonUp(0))
            {
                EndPathBuilding();
            }
        }

        private void BuildPath(Vector2 screenPos)
        {
            if (!_tilePathMode)
            {
                return;
            }

            var ray = Camera.main.ScreenPointToRay(screenPos);
            var hits = Physics.RaycastAll(ray);
            foreach (var hit in hits)
            {
                var collidedObject = hit.collider.gameObject;
                var tileComponent = collidedObject.GetComponents<BasicTileComponent>().FirstOrDefault();
                if (collidedObject != _currentTile && tileComponent != null)
                {
                    _tempPath.AddLast(collidedObject);
                    _currentTile = collidedObject;
                }
            }

        }

        private void EndPathBuilding()
        {
            if (_tilePathMode)
            {
                var animalComponent = _selectionnedAnimal.GetComponent<AnimalComponent>();
                animalComponent.PathToDo = _tempPath.Select(x => x.GetComponent<BasicTileComponent>().CurrentLocation).ToArray();
;
                _tempPath = null;
            }

            _tilePathMode = false;
        }

        private void Selection(Vector2 screenPos)
        {
            var ray = Camera.main.ScreenPointToRay(screenPos);
            var hit2D = Physics2D.Raycast(ray.origin, Vector2.zero);
            if(hit2D.collider != null)
            {
                var collidedObject = hit2D.collider.gameObject;

                var animalComponent = collidedObject.GetComponents<AnimalComponent>().FirstOrDefault();
                if (animalComponent != null)
                {
                    _selectionnedAnimal = collidedObject;
                    _selectionVisual.GetComponent<MeshRenderer>().enabled = true;
                    _selectionVisual.transform.parent = _selectionnedAnimal.transform;
                    _selectionVisual.transform.localPosition = Vector3.zero;

                    var hits = Physics.RaycastAll(ray);
                    foreach (var hit in hits)
                    {
                        collidedObject = hit.collider.gameObject;
                        var tileComponent = collidedObject.GetComponents<BasicTileComponent>().FirstOrDefault();

                        if (tileComponent != null && _selectionnedAnimal != null)
                        {
                            _tilePathMode = true;
                            _currentTile = collidedObject;
                            _tempPath = new LinkedList<GameObject>();
                            _tempPath.AddLast(_currentTile);
                        }
                    }

                    return;
                }
            }

            // No animal selected
            _selectionVisual.GetComponent<MeshRenderer>().enabled = false;
            _selectionnedAnimal = null;
        }
    }
}
