using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation;

namespace VR {
    /// <summary>
    /// Disable VR device simulator when using a real VR headset.
    /// </summary>
    public class XRSimulatorDisabler : MonoBehaviour {
        private void Start() {
            var inputDevices = new List<InputDevice>();
            InputDevices.GetDevices(inputDevices);
            inputDevices.ForEach(d => Debug.Log($"Device: {d.name}"));
            if (inputDevices.Count > 0) {
                GetComponent<XRDeviceSimulator>().enabled = false;
            }
        }
    }
}