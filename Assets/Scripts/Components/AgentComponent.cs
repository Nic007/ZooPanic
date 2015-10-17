using System;
using System.Collections.Generic;
using Assets.Scripts.Actions;
using UnityEngine;

namespace Assets.Scripts.Components
{
	public abstract class AgentComponent : MonoBehaviour {

		public float Acceleration;
		public float MaxSpeed;
	    public float RotationSpeed;
		public GameObject CurrentTile;
		public GameObject GameManager;

	    public LinkedList<IAction> ActionsList;
	    public LevelDataComponent.TileRotation CurrentDirection;
		
		// Use this for initialization
		protected void Start ()
		{
            ActionsList = new LinkedList<IAction>();

			transform.position =
			    TileUtility.GetTilePosition(GameManager.GetComponent<LevelDataComponent>(), CurrentTile.GetComponent<TileComponent>().CurrentLocation);
		}
	
		// Update is called once per frame
		protected void Update () {
            if(ActionsList.Count > 0)
            {
                if (ActionsList.First.Value.Completed)
                {
                    ActionsList.RemoveFirst();
                }
                else
                {
                    ActionsList.First.Value.Update();
                }
            }
		}
		
		protected void GoTo(GameObject destinationTile)
		{
			var pathfinder = new PathFinderStar();
			var result = pathfinder.FindShortestPath(CurrentTile, destinationTile);
			if (result == null)
			{
				Debug.Log("Path not found");
			}
			else
			{
			    ActionsList.AddLast(new WalkAction(this, result));
			}
		}		
	}
}