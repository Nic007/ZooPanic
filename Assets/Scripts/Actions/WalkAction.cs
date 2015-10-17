using System;
using Assets.Scripts.Components;
using UnityEngine;

namespace Assets.Scripts.Actions
{
    class WalkAction : IAction
    {
        private readonly Rigidbody2D _rigidbody2D;
		private readonly LevelDataComponent _currentLevelData;
        
		private readonly TileIndex[] _pathToFollow;

		private int _pathIndex = 0;
		private bool _inputState = false;
		
        public WalkAction(AgentComponent agent, TileIndex[] pathToFollow)
        {
            Agent = agent;
            _pathToFollow = pathToFollow;
            _currentLevelData = Agent.GameManager.GetComponent<LevelDataComponent>();
            _rigidbody2D = Agent.GetComponent<Rigidbody2D>();
        }


        public AgentComponent Agent { get; set; }

        public bool Completed { get; set; }

        public void Update()
        {
            // If we are on the tile referenced by the index, increment the index
            // Must have finished to enter the tile
			if (!_inputState && _pathIndex < _pathToFollow.Length && Equals(_pathToFollow[_pathIndex], Agent.CurrentTile.GetComponent<TileComponent>().CurrentLocation))
			{
				++_pathIndex;
                
                // Check if the tile is really available!
			    if (_pathIndex < _pathToFollow.Length)
			    {
			        var currentTileComp = Agent.CurrentTile.GetComponent<TileComponent>();
                    var wantedDirection = FindDirection(Agent.CurrentTile.GetComponent<TileComponent>().CurrentLocation, _pathToFollow[_pathIndex]);
			        var nextTileComp = currentTileComp.NeighborsObjects[(int) wantedDirection].GetComponent<TileComponent>();

			        if (!Equals(nextTileComp.CurrentLocation, _pathToFollow[_pathIndex])
                            || currentTileComp.NeighborsState[(int) wantedDirection] != TileComponent.PathState.Available
                            || nextTileComp.NeighborsState[((int) wantedDirection + 2) % (int) LevelDataComponent.TileRotation.Size] != TileComponent.PathState.Available)
			        {
                        Completed = true;
			            return;
			        }
			    }
				
                // We have finished to move
				if (_pathIndex >= _pathToFollow.Length)
				{
				    _pathIndex = 0;
				    Completed = true;
					return;
				}
			}
			
            // We have crossed a new tile
			var newTile = TileUtility.GetTileIndex(_currentLevelData, Agent.transform.position);
			if (_pathIndex < _pathToFollow.Length && Equals(newTile, _pathToFollow[_pathIndex]))
			{
				Agent.CurrentTile = _currentLevelData.TileMap[newTile.y][newTile.x];
				_inputState = true;
			}
			
			if (_inputState)
			{
                // We are at the center, we must end the input state
                if (Math.Abs(Agent.transform.position.x - Agent.CurrentTile.transform.position.x) < 0.05 &&
				    Math.Abs(Agent.transform.position.y - Agent.CurrentTile.transform.position.y) < 0.05)
				{
					Agent.transform.position = Agent.CurrentTile.transform.position;
					_inputState = false;
					
                    // We are at the end! Brake!
					if (_pathIndex >= _pathToFollow.Length - 1 || Agent.CurrentDirection != FindDirection(_pathToFollow[_pathIndex], _pathToFollow[_pathIndex + 1]))
					{
						_rigidbody2D.velocity = new Vector2(0, 0);
					}
				}
			}
			else
			{
                // Check if we are in the good direction, if not rotate!
			    if (_pathIndex < _pathToFollow.Length)
			    {
                    var wantedDirection = FindDirection(Agent.CurrentTile.GetComponent<TileComponent>().CurrentLocation, _pathToFollow[_pathIndex]);
                    if (Agent.CurrentDirection != wantedDirection)
                    {
                        Agent.ActionsList.AddFirst(new RotateAction(Agent, wantedDirection));
                        return;
                    }
			    }

                // Now we can really continue
				if (_pathIndex < _pathToFollow.Length && Math.Abs(_rigidbody2D.velocity.x) + Math.Abs(_rigidbody2D.velocity.y) < Agent.MaxSpeed)
				{
					switch (Agent.CurrentDirection)
					{
					case LevelDataComponent.TileRotation.North:
						_rigidbody2D.AddForce(new Vector2(0, Agent.Acceleration));
						break;
					case LevelDataComponent.TileRotation.East:
						_rigidbody2D.AddForce(new Vector2(Agent.Acceleration, 0));
						break;
					case LevelDataComponent.TileRotation.South:
						_rigidbody2D.AddForce(new Vector2(0, -1 * Agent.Acceleration));
						break;
					case LevelDataComponent.TileRotation.West:
						_rigidbody2D.AddForce(new Vector2(-1 * Agent.Acceleration, 0));
						break;
					}
				}
			}
			
		}
		private LevelDataComponent.TileRotation FindDirection(TileIndex currentTile, TileIndex nextTile)
		{
			var deltaX = nextTile.x - currentTile.x;
			var deltaY = currentTile.y - nextTile.y;
			
			var z = (deltaY < 0 || deltaX < 0) ? 1 : 0;
			return (LevelDataComponent.TileRotation) (2 * z + Math.Abs(deltaX));
		}
    }
}