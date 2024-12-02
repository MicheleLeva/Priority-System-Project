using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

namespace VR {
    /// <summary>
    /// Class to control player movements using the VR controller analogs.
    /// </summary>
    public class MovementProvider : LocomotionProvider {
        /// <summary>
        /// List of VR controllers.
        /// </summary>
        public List<XRController> controllers;
        /// <summary>
        /// Walk speed.
        /// </summary>
        public float speed = 1f;
        /// <summary>
        /// Gravity of the player.
        /// </summary>
        public float gravityMultiplayer = 1f;
        
        private CharacterController _characterController;
        private GameObject _head;

        /// <inheritdoc />
        /// When awake, find character controller and player head object.
        protected override void Awake() {
            _characterController = GetComponent<CharacterController>();
            _head = GetComponentInChildren<Camera>().gameObject;
        }

        void Start() {
            PositionController();
        }

        void Update() {
            PositionController();
        }

        private void PositionController() {
            var localPosition = _head.transform.localPosition;
            var headHeight = Mathf.Clamp(localPosition.y, 1, 2);
            _characterController.height = headHeight;

            var newCenter = Vector3.zero;
            newCenter.y = _characterController.height / 2;
            newCenter.y += _characterController.skinWidth;

            newCenter.x = localPosition.x;
            newCenter.z = localPosition.z;

            _characterController.center = newCenter;
        }

        private void CheckForInput() {
            foreach (var controller in controllers) {
                if (controller.enabled) {
                    CheckForMovement(controller.inputDevice);
                }
            }
        }

        private void CheckForMovement(InputDevice device) {
            if (device.TryGetFeatureValue(CommonUsages.primary2DAxis, out var position)) {
                StartMove(position);
            }
        }

        private void StartMove(Vector2 position) {
            var direction = new Vector3(position.x, 0, position.y);
            var headRotation = new Vector3(0, _head.transform.eulerAngles.y, 0);

            direction = Quaternion.Euler(headRotation) * direction;

            var movement = direction * speed;
            _characterController.Move(movement * Time.deltaTime);
        }

        private void ApplyGravity() {
            var gravity = new Vector3(0, Physics.gravity.y * gravityMultiplayer, 0);
            gravity.y *= Time.deltaTime;

            _characterController.Move(gravity);
        }
    }
}