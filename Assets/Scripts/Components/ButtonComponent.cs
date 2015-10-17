using UnityEngine;

namespace Assets.Scripts.Components
{
    public class ButtonComponent : MonoBehaviour
    {
        public bool MustBeActivatedToPerform;
        public bool MustManageSprite;
        public bool PressedVisualEffect;
        public Vector3 SelectionVisualScale;

        public delegate void ClickAction();
        public event ClickAction OnClicked;

        public delegate void ButtonStateAction(bool status);
        public event ButtonStateAction OnStatusChanged;

        private GameObject _selectionVisual;
        private bool _activated;
        private bool _pressed;

        private Color _defaultColor;
        private Color _pressedColor;

        public bool Activated
        {
            get { return _activated; }
            set
            {
                bool statusChanged = _activated != value;
                _activated = value;

                if (MustManageSprite)
                {
                    GetComponent<SpriteRenderer>().enabled = value;
                }

                _selectionVisual.GetComponent<MeshRenderer>().enabled = value;

                if (statusChanged && OnStatusChanged != null)
                {
                    OnStatusChanged(value);
                }
            }
        }

        // Use this for initialization
        void Start () {
            if (MustManageSprite)
            {
                GetComponent<SpriteRenderer>().enabled = false;
            }

            _selectionVisual = Instantiate(Resources.Load("SelectionVisual")) as GameObject;
            if (_selectionVisual != null)
            {
                _selectionVisual.transform.parent = transform;
                _selectionVisual.transform.localPosition = Vector3.zero;
                _selectionVisual.transform.localScale = SelectionVisualScale;
                _selectionVisual.GetComponent<MeshRenderer>().enabled = false;

                _defaultColor = _selectionVisual.GetComponent<MeshRenderer>().material.color;
                _pressedColor = new Color(_defaultColor.r, _defaultColor.g, _defaultColor.b, 1.0f);
            }
            else
            {
                Debug.LogError("SelectionVisual could not be created");
            }
        }
	
        // Update is called once per frame
        void Update () {
	
        }

        public bool Press()
        {
            if (Activated || !MustBeActivatedToPerform)
            {
                _pressed = true;
                if (PressedVisualEffect)
                {
                    _selectionVisual.GetComponent<MeshRenderer>().material.color = _pressedColor;
                }
            }

            return _pressed;
        }

        public void Release()
        {
            if (_pressed)
            {
                _pressed = false;
                if (PressedVisualEffect)
                {
                    _selectionVisual.GetComponent<MeshRenderer>().material.color = _defaultColor;
                }

                if (OnClicked != null)
                {
                    OnClicked.Invoke();
                }
                
            }
        }
    }
}
