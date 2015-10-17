using System;
using Assets.Scripts.Components;
using UnityEngine;

namespace Assets.Scripts.Actions
{
    class RotateAction : IAction
    {
        private LevelDataComponent.TileRotation _directionGoal;
        private float _angleMovement;
        private readonly float _wantedAngle;

        private readonly SpriteRenderer _spriteToRotate;

        public RotateAction(AgentComponent agent, LevelDataComponent.TileRotation directionGoal )
        {
            Agent = agent;
            _directionGoal = directionGoal;

            var rotationIntensity = 0;
            rotationIntensity -= ((int) Agent.CurrentDirection + 1) % (int) LevelDataComponent.TileRotation.Size;
			rotationIntensity += ((int) directionGoal + 1) % (int) LevelDataComponent.TileRotation.Size;

            _angleMovement = rotationIntensity * 90.0f;
            if (Math.Abs(_angleMovement - (-270)) < 0.001f)
            {
                _angleMovement = 90;
            }
            else if (Math.Abs(_angleMovement - 270) < 0.001f)
            {
                _angleMovement = -90;
            }

            _spriteToRotate = Agent.GetComponentInChildren<SpriteRenderer>();
            var initialAngle = _spriteToRotate.transform.localEulerAngles.z;
            _wantedAngle = initialAngle - _angleMovement;
        }

        public AgentComponent Agent { get; set; }
        public bool Completed { get; set; }

        public void Update()
        {
            if (Mathf.Abs(_angleMovement) <= 0.01f)
            {
                _spriteToRotate.transform.localEulerAngles = new Vector3(0.0f, 0.0f, _wantedAngle);
                Agent.CurrentDirection = _directionGoal;
                Completed = true;
            }
            else
            {
                var deltaAngle = Agent.RotationSpeed * Time.deltaTime * _angleMovement > 0 ? -1 : 1;
                _spriteToRotate.transform.Rotate(Vector3.forward, deltaAngle);
                _angleMovement += deltaAngle;
            }
        }
    }
}