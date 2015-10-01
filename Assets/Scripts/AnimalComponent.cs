using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class AnimalComponent : MonoBehaviour
    {
        public GameObject CurrentTile;
        public GameObject GoalTile;

        public float Acceleration;
        public float MaxSpeed;

        private Rigidbody2D _rigidbody2d;
        private TileIndex[] _pathToFollow;
        private int _pathIndex = 0;
        private bool _inputState = false;
        private LevelData.TileRotation _currentDirection;

        // Use this for initialization
        void Start ()
        {
            transform.position =
                TileUtility.GetTilePosition(CurrentTile.GetComponent<BasicTileComponent>().CurrentLocation);
            _rigidbody2d = GetComponent<Rigidbody2D>();
        }
	
        // Update is called once per frame
        void Update () {
            if (_pathToFollow == null || _pathToFollow.Length == 0)
            {
                GoTo(GoalTile);
            }

            WalkTo();
        }

        void WalkTo()
        {
            if (!_inputState && _pathToFollow != null && _pathIndex < _pathToFollow.Length && Equals(_pathToFollow[_pathIndex], CurrentTile.GetComponent<BasicTileComponent>().CurrentLocation))
            {
                ++_pathIndex;

                if (_pathIndex >= _pathToFollow.Length)
                {
                    return;
                }

                _currentDirection = FindDirection(CurrentTile.GetComponent<BasicTileComponent>().CurrentLocation, _pathToFollow[_pathIndex]);
            }

            var newTile = TileUtility.GetTileIndex(gameObject.transform.position);
            if (_pathToFollow != null && _pathIndex < _pathToFollow.Length && Equals(newTile, _pathToFollow[_pathIndex]))
            {
                CurrentTile = TileUtility.CurrentLevelData.TileMap[newTile.y][newTile.x];
                _inputState = true;
            }

            if (_inputState)
            {
                if (Math.Abs(transform.position.x - CurrentTile.transform.position.x) < 0.05 &&
                    Math.Abs(transform.position.y - CurrentTile.transform.position.y) < 0.05)
                {
                    transform.position = CurrentTile.transform.position;
                    _inputState = false;

                    if (_pathToFollow != null && (_pathIndex >= _pathToFollow.Length - 1 || _currentDirection != FindDirection(_pathToFollow[_pathIndex], _pathToFollow[_pathIndex + 1])))
                    {
                        _rigidbody2d.velocity = new Vector2(0, 0);
                    }
                }
            }
            else
            {
                if (_pathToFollow != null && _pathIndex < _pathToFollow.Length && Math.Abs(_rigidbody2d.velocity.x) + Math.Abs(_rigidbody2d.velocity.y) < MaxSpeed)
                {
                    switch (_currentDirection)
                    {
                        case LevelData.TileRotation.North:
                            _rigidbody2d.AddForce(new Vector2(0, Acceleration));
                            break;
                        case LevelData.TileRotation.East:
                            _rigidbody2d.AddForce(new Vector2(Acceleration, 0));
                            break;
                        case LevelData.TileRotation.South:
                            _rigidbody2d.AddForce(new Vector2(0, -1 * Acceleration));
                            break;
                        case LevelData.TileRotation.West:
                            _rigidbody2d.AddForce(new Vector2(-1 * Acceleration, 0));
                            break;
                    }
                }
            }

        }

        void GoTo(GameObject destinationTile)
        {
            var pathfinder = new PathFinderStar();
            var result = pathfinder.FindShortestPath(CurrentTile, destinationTile);
            if (result == null)
            {
                Debug.Log("Path not found");
            }
            else
            {
                _pathToFollow = result;
                _pathIndex = 0;
            }
        }

        private LevelData.TileRotation FindDirection(TileIndex currentTile, TileIndex nextTile)
        {
            var deltaX = nextTile.x - currentTile.x;
            var deltaY = currentTile.y - nextTile.y;

            var z = (deltaY < 0 || deltaX < 0) ? 1 : 0;
            return (LevelData.TileRotation) (2 * z + Math.Abs(deltaX));
        }
    }
}
