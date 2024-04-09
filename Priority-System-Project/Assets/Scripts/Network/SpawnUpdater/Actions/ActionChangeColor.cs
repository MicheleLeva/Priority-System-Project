using System;
using UnityEngine;

namespace Network.Objects {
    /// <summary>
    /// Action to change material's color.
    /// </summary>
    public class ActionChangeColor : IAction {
        /// <summary>
        /// Index of the material in the object's mesh renderer.
        /// </summary>
        public int materialIndex;
        /// <summary>
        /// Color of the material when action activated.
        /// </summary>
        public Color colorOn;

        private Color _colorOff;
        private bool _status;
        private Material _material;

        private void Start() {
            var mr = GetComponent<MeshRenderer>();
            _material = mr.materials[materialIndex];
            _colorOff = _material.color;
        }

        /// <inheritdoc />
        public override void Activate() {
            _material.color = _status
                ? _colorOff
                : colorOn;
            _status = !_status;
        }
    }
}