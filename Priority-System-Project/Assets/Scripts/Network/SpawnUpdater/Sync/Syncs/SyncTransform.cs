using System;
using Unity.Netcode;
using UnityEngine;
using Utils;

namespace Network.Sync {
    /// <summary>
    /// Structure that contains transform properties.
    /// </summary>
    [Serializable]
    public struct TransformData {
        /// <summary>
        /// Object position.
        /// </summary>
        public SVector3 position;
        /// <summary>
        /// Object rotation.
        /// </summary>
        public SVector3 rotation;
        /// <summary>
        /// Object scale.
        /// </summary>
        public SVector3 scale;
    }

    /// <summary>
    /// Class to sync transform changes. 
    /// </summary>
    public class SyncTransform : BaseSync {
        private Vector3 _pos, _oldPos;
        private Quaternion _rot, _oldRot;
        private Vector3 _scale, _oldScale;
        private Transform _t;

        /// <inheritdoc />
        /// Get object's transform.
        protected override void AfterStart() {
            _t = transform;
        }

        /// <inheritdoc />
        public override void UpdateSync() {
            SendSync(new SyncData {Data = GetData()});
        }

        private TransformData GetData() =>
            new() {
                position = _pos,
                rotation = _rot.eulerAngles,
                scale = _scale
            };

        /// <summary>
        /// Update object's transform with the data received from a Client.
        /// </summary>
        /// <param name="payload">transform data</param>
        public void UpdateServerTransform(SyncData payload = null) {
            if (payload?.Data is not TransformData d) return;

            var rb = GetComponent<Rigidbody>();
            rb.MovePosition(d.position);
            rb.MoveRotation(Quaternion.Euler(d.rotation));
        }

        /// <inheritdoc />
        protected override bool UpdateConditions() {
            return
                NetworkManager.Singleton.ConnectedClients.Count > 0
                && Facing >= 0
                //&& Owner == ulong.MaxValue
                && IsMoving();
        }

        /// <returns>if the Object is moving or not</returns>
        private bool IsMoving() {
            UpdateLocalTransform();
            if (_pos == _oldPos && _rot == _oldRot && _scale == _oldScale) return false;
            _oldPos = _pos;
            _oldRot = _rot;
            _oldScale = _scale;
            return true;
        }

        private void UpdateLocalTransform() {
            _pos = _t.position;
            _rot = _t.rotation;
            _scale = _t.localScale;
        }
    }
}