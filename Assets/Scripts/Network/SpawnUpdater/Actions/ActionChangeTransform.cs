using System;
using UnityEngine;

namespace Network.Objects {
    /// <summary>
    /// Action to change an object's transform.
    /// </summary>
    public class ActionChangeTransform : IAction {
        /// <summary>
        /// Position of the object when activated.
        /// </summary>
        public Vector3 position;
        /// <summary>
        /// Rotation of the object when activated.
        /// </summary>
        public Vector3 rotation;
        /// <summary>
        /// Scale of the object when activated.
        /// </summary>
        public Vector3 scale;
        /// <summary>
        /// Target object's transform.
        /// </summary>
        public Transform target;

        private Vector3 _initPosition;
        private Vector3 _initRotation;
        private Vector3 _initScale;
        private bool _state;
        
        private void Start() {
            _initPosition = target.localPosition;
            _initRotation = target.localRotation.eulerAngles;
            _initScale = target.localScale;
        }

        /// <inheritdoc />
        public override void Activate() {
            if (_state) {
                target.localPosition = _initPosition;
                target.localRotation = Quaternion.Euler(_initRotation);
                target.localScale = _initScale;
            }
            else {
                target.localPosition = position;
                target.localRotation = Quaternion.Euler(rotation);
                target.localScale = scale;
            }

            _state = !_state;
        }
    }
}