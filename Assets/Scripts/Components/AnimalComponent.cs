using UnityEngine;

namespace Assets.Scripts.Components
{
    public class AnimalComponent : AgentComponent
	{
        public float ActionButtonRadius;

        private GameObject _specialActionButton;

        new void Start()
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

        new void Update()
		{
			base.Update ();
		}
	}
	
}
