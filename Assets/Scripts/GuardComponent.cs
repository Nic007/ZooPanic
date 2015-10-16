using UnityEngine;
using System.Collections;

namespace Assets.Scripts
{
	public class GuardComponent : AgentComponent {
	
		public GameObject[] patrolPath;
		public fsmState actualState;
		public enum fsmState { patrol=0, goTo, returnTo}

		public float visionRange;

		private int patrolIndex = 0;

		// Use this for initialization
		void Start () {
			base.Start ();
			actualState = fsmState.patrol;
			GoTo(patrolPath[patrolIndex]);

		}
		
		// Update is called once per frame
		void Update () 
		{
			base.Update ();
			if (actualState == fsmState.patrol)
			{
				Patrol();
			}
		}

		void Patrol()
		{
			if (CurrentTile == patrolPath [patrolIndex]) {
				patrolIndex++;
				patrolIndex %= patrolPath.Length;
				GoTo(patrolPath[patrolIndex]);
			}

		}
	}
}