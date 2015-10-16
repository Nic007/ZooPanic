using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngineInternal;

namespace Assets.Scripts
{
    public class InputComponent : MonoBehaviour
    {
        private GameObject _selectionnedAnimal;
        private ButtonComponent _selectionnedButton;

        private GameObject _currentTile = null;
        private LinkedList<GameObject> _tempPath;
        private ArrayList _selectionOverlay;
        private bool _tilePathMode;

        // Use this for initialization
        void Start ()
        {
            _selectionnedAnimal = null;
            _tilePathMode = false;
            _currentTile = null;
            _selectionOverlay = null;
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
                        EndSelection();
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
                EndSelection();
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
                    var dirX = tileComponent.CurrentLocation.x -
                               _currentTile.GetComponent<BasicTileComponent>().CurrentLocation.x;
                    var dirY = tileComponent.CurrentLocation.y -
                               _currentTile.GetComponent<BasicTileComponent>().CurrentLocation.y;

                    // Not direct neighbors
                    if (!((Math.Abs(dirX) == 1 && Math.Abs(dirY) == 0) ^
                          (Math.Abs(dirY) == 1 && Math.Abs(dirX) == 0)))
                    {
                        continue;
                    }

                    // Not available
                    var orientation = dirX != 0 ? -dirX + 2 : dirY + 1;
                    if (_currentTile.GetComponent<BasicTileComponent>().NeighborsState[orientation] !=
                        BasicTileComponent.PathState.Available ||
                        tileComponent.NeighborsState[(orientation + 2)%(int) LevelData.TileRotation.Size] !=
                        BasicTileComponent.PathState.Available)
                    {
                        continue;
                    }

                    _tempPath.AddLast(collidedObject);
                    _currentTile = collidedObject;

                    var newOverlay = GameObject.Instantiate(Resources.Load("Tiles/SelectionOverlayTile")) as GameObject;
                    newOverlay.transform.position = collidedObject.transform.position;
                    _selectionOverlay.Add(newOverlay);
                }
            }

        }

        private void EndSelection()
        {
            if (_selectionnedButton != null)
            {
                _selectionnedButton.Release();
                _selectionnedButton = null;
            }
        }

        private void EndPathBuilding()
        {
            if (_tilePathMode)
            {
                var animalComponent = _selectionnedAnimal.GetComponent<AnimalComponent>();
                animalComponent.PathToDo = _tempPath.Select(x => x.GetComponent<BasicTileComponent>().CurrentLocation).ToArray();

                foreach (var tile in _selectionOverlay)
                {
                    DestroyImmediate(tile as GameObject);
                }
                _selectionOverlay = null;

                _tempPath = null;
            }

            _tilePathMode = false;
        }

        private void Selection(Vector2 screenPos)
        {
            var mustRemoveAnimalSelection = true;

            var ray = Camera.main.ScreenPointToRay(screenPos);
            var hit2D = Physics2D.Raycast(ray.origin, Vector2.zero);
            if(hit2D.collider != null)
            {
                var collidedObject = hit2D.collider.gameObject;

                var buttonComponent = collidedObject.GetComponents<ButtonComponent>().FirstOrDefault();
                if (buttonComponent != null)
                {
                    _selectionnedButton = buttonComponent;
                    _selectionnedButton.Press();
                    mustRemoveAnimalSelection = false;
                }

                var animalComponent = collidedObject.GetComponents<AnimalComponent>().FirstOrDefault();
                if (animalComponent != null)
                {
                    var hits = Physics.RaycastAll(ray);
                    foreach (var hit in hits)
                    {
                        var collidedTile = hit.collider.gameObject;
                        var tileComponent = collidedTile.GetComponents<BasicTileComponent>().FirstOrDefault();

                        if (tileComponent != null && _selectionnedAnimal != null && _selectionnedAnimal.GetComponent<AnimalComponent>().CurrentTile == collidedTile)
                        {
                            _tilePathMode = true;
                            _currentTile = collidedTile;
                            _tempPath = new LinkedList<GameObject>();
                            _tempPath.AddLast(_currentTile);

                            _selectionOverlay = new ArrayList();
                            var newOverlay = Instantiate(Resources.Load("Tiles/SelectionOverlayTile")) as GameObject;
                            newOverlay.transform.position = collidedTile.transform.position;
                            _selectionOverlay.Add(newOverlay);
                        }
                    }

                    _selectionnedAnimal = collidedObject;
                    _selectionnedAnimal.GetComponent<ButtonComponent>().Activated = true;
                    mustRemoveAnimalSelection = false;
                }
            }

            if (mustRemoveAnimalSelection && _selectionnedAnimal != null)
            {
                _selectionnedAnimal.GetComponent<ButtonComponent>().Activated = false;
                _selectionnedAnimal = null;
            }
        }
    }
}
