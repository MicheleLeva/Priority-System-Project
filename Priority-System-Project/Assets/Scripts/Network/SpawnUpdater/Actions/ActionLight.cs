using System;
using UnityEngine;

namespace Network.Objects {
    /// <summary>
    /// Action to turn on/off an object's light.
    /// </summary>
    public class ActionLight : IAction {
        /// <summary>
        /// Light component of the object.
        /// </summary>
        public new Light light;
        /// <summary>
        /// Intensity of the light when turned on. 
        /// </summary>
        public float intensityOn;
        
        private bool _state;
        private float _intensityOff;

        private void Start() {
            if (light == null) {
                light = GetComponent<Light>();
            }

            _intensityOff = light.intensity;
        }

        /// <inheritdoc />
        public override void Activate() {
            light.intensity = _state ? _intensityOff : intensityOn;
            _state = !_state;
        }
    }
}