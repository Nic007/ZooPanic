using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class AnimalComponent : AgentComponent
	{
        public TileIndex[] PathToDo;

        public float ActionButtonRadius;
        private GameObject _specialActionButton;

        void Start()
        {
            base.Start();

            _specialActionButton = Instantiate(Resources.Load("SpecialActionButton")) as GameObject;
            if (_specialActionButton == null)
            {
                Debug.LogError("SpecialActionButton not created!");
            }
            else
            {
                _specialActionButton.transform.parent = transform;
                _specialActionButton.transform.localPosition = new Vector3(-1 * ActionButtonRadius, 0, 0);

                var specButtonComp = _specialActionButton.GetComponent<ButtonComponent>();
                specButtonComp.MustBeActivatedToPerform = true;
                specButtonComp.MustManageSprite = true;
                specButtonComp.PressedVisualEffect = true;

                GetComponent<ButtonComponent>().OnStatusChanged += delegate(bool status)
                {
                    _specialActionButton.GetComponent<ButtonComponent>().Activated = status;
                };
            }
            
        }

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
