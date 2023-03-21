using System;
using Unity.VisualScripting;
using UnityEngine;
using Utils;

namespace Network.Sync {
    /// <summary>
    /// Structure that contains light properties.
    /// </summary>
    [Serializable]
    public struct LightData {
        /// <summary>
        /// Light intensity.
        /// </summary>
        public float intensity;
        /// <summary>
        /// Light color.
        /// </summary>
        public SColor32 color;
        /// <summary>
        /// Light type.
        /// </summary>
        public LightType type;
    }

    /// <summary>
    /// Class to sync light changes of an object. 
    /// </summary>
    public class SyncLight : BaseSync {
        private Light _light;
        private float _oldIntensity;
        private Color _oldColor;
        private LightType _oldType;

        /// <inheritdoc />
        /// Obtain the Light component of the object.
        protected override void BeforeStart() {
            _light = GetComponent<Light>();
        }

        /// <inheritdoc />
        public override void UpdateSync() {
            SendSync(new SyncData {
                Data = new LightData {
                    intensity = _light.intensity,
                    color = (Color32) _light.color,
                    type = _light.type
                }
            });
        }

        /// <inheritdoc />
        protected override bool UpdateConditions() {
            if (Math.Abs(_light.intensity - _oldIntensity) < .5F
                && _light.color == _oldColor
                && _light.type == _oldType) return false;
            _oldIntensity = _light.intensity;
            _oldColor = _light.color;
            _oldType = _light.type;
            return true;
        }
    }
}