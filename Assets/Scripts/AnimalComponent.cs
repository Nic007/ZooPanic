using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class AnimalComponent : AgentComponent
	{
		public GameObject GoalTile;
	
		void Update()
		{
			if (_pathToFollow == null || _pathToFollow.Length == 0)
			{
				GoTo(GoalTile);
			}
			base.Update ();
		}
	}
	
}
