using System;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace VR {
    /// <summary>
    /// Character movement helper.
    /// </summary>
    public class CharacterMovementHelper : MonoBehaviour {
        private XROrigin _xrOrigin;
        private CharacterController _characterController;
        private CharacterControllerDriver _driver;

        private void Start() {
            _xrOrigin = GetComponent<XROrigin>();
            _characterController = GetComponent<CharacterController>();
            _driver = GetComponent<CharacterControllerDriver>();
        }

        private void Update() {
            UpdateCharacterController();
        }

        private void UpdateCharacterController() {
            if (_xrOrigin == null || _characterController == null)
                return;

            var height = Mathf.Clamp(_xrOrigin.CameraInOriginSpaceHeight, _driver.minHeight, _driver.maxHeight);

            var center = _xrOrigin.CameraInOriginSpacePos;
            center.y = height / 2f + _characterController.skinWidth;

            _characterController.height = height;
            _characterController.center = center;
        }
    }
}