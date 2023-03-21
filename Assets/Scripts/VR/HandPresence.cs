using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace VR {
    /// <summary>
    /// Class to display user's avatar hands. 
    /// </summary>
    public class HandPresence : MonoBehaviour {
        private static readonly int Trigger = Animator.StringToHash("Trigger");
        private static readonly int Grip = Animator.StringToHash("Grip");
        
        /// <summary>
        /// Show or not the VR controllers.
        /// </summary>
        public bool showController;
        /// <summary>
        /// Properties of the controllers.
        /// </summary>
        public InputDeviceCharacteristics controllerCharacteristics;
        /// <summary>
        /// Controller prefab model.
        /// </summary>
        public List<GameObject> controllerPrefabs;
        /// <summary>
        /// Hands prefab model.
        /// </summary>
        public GameObject handModelPrefab;

        private InputDevice _targetDevice;
        private GameObject _spawnedController;
        private GameObject _spawnedHandModel;
        private Animator _handAnimator;

        // Start is called before the first frame update
        void Start() {
            TryInitialize();
        }

        void TryInitialize() {
            List<InputDevice> devices = new List<InputDevice>();

            InputDevices.GetDevicesWithCharacteristics(controllerCharacteristics, devices);

            foreach (var item in devices) {
                Debug.Log(item.name + item.characteristics);
            }

            if (devices.Count > 0) {
                _targetDevice = devices[0];
                GameObject prefab = controllerPrefabs.Find(controller => controller.name == _targetDevice.name);
                if (prefab) {
                    _spawnedController = Instantiate(prefab, transform);
                }
                else {
                    Debug.Log("Did not find corresponding controller model");
                }

                _spawnedHandModel = Instantiate(handModelPrefab, transform);
                _handAnimator = _spawnedHandModel.GetComponent<Animator>();
            }
        }

        void UpdateHandAnimation() {
            if (_targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue)) {
                _handAnimator.SetFloat(Trigger, triggerValue);
            }
            else {
                _handAnimator.SetFloat(Trigger, 0);
            }

            if (_targetDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue)) {
                _handAnimator.SetFloat(Grip, gripValue);
            }
            else {
                _handAnimator.SetFloat(Grip, 0);
            }
        }

        // Update is called once per frame
        void Update() {
            if (!_targetDevice.isValid) {
                // TryInitialize();
            }
            else {
                if (showController) {
                    if (_spawnedHandModel)
                        _spawnedHandModel.SetActive(false);
                    if (_spawnedController)
                        _spawnedController.SetActive(true);
                }
                else {
                    if (_spawnedHandModel)
                        _spawnedHandModel.SetActive(true);
                    if (_spawnedController)
                        _spawnedController.SetActive(false);
                    UpdateHandAnimation();
                }
            }
        }
    }
}