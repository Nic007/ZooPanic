using UnityEngine;

namespace Assets.Scripts.Components
{
	public class GuardComponent : AgentComponent {
	
		public GameObject[] patrolPath;
		public fsmState actualState;
		public enum fsmState { patrol=0, goTo, returnTo}

		public float visionRange;

		private int patrolIndex = 0;

		// Use this for initialization
	    new void Start () {
			base.Start ();
			actualState = fsmState.patrol;
			GoTo(patrolPath[patrolIndex]);

		}
		
		// Update is called once per frame
	    new void Update () 
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