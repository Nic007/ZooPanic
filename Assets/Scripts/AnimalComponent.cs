using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class AnimalComponent : AgentComponent
	{
        public TileIndex[] PathToDo;

		void Update()
		{
			if (_pathToFollow == null || _pathToFollow.Length == 0)
			{
			    _pathToFollow = PathToDo;
			    PathToDo = null;
			}

			base.Update ();
		}
	}
	
}
