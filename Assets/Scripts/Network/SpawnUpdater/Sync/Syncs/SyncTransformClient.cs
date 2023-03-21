using System.Collections;
using UnityEngine;

namespace Network.Sync {
    /// <summary>
    /// Class to update transform changes on the Client. 
    /// </summary>
    public class SyncTransformClient : BaseSyncClient {
        private Vector3 _pos, _oldPos;
        private Quaternion _rot, _oldRot;
        private Vector3 _scale, _oldScale;
        private Transform _t;

        private new void Start() {
            base.Start();
            _t = transform;
            StartCoroutine(UpdateSync());
        }

        IEnumerator UpdateSync() {
            while (true) {
                if (owner == clientId && IsMoving()) {
                    var data = new SyncData {
                        Data = new TransformData {
                            position = _pos,
                            rotation = _rot.eulerAngles,
                            scale = _scale
                        }
                    };
                    SendSignal(objectId, data);
                }

                yield return new WaitForSeconds(Prefs.Singleton.updateDelay);
            }
        }

        /// <inheritdoc />
        public override bool TryApplySync(SyncData data) {
            if (data.Data is not TransformData d) return false;

            var rb = GetComponent<Rigidbody>();
            if (rb != null) {
                rb.MovePosition(d.position);
                rb.MoveRotation(Quaternion.Euler(d.rotation));
            }

            _t.localScale = d.scale;
            return true;
        }

        /// <summary>
        /// Send transform updates to the Server.
        /// </summary>
        /// <param name="id">client id</param>
        /// <param name="data">transform data</param>
        private void SendSignal(int id, SyncData data = null) {
            if (data != null) {
                syncManager.UpdateTransformServerRpc(id, data);
            }
        }

        /// <returns>if the Object is moving or not</returns>
        private bool IsMoving() {
            UpdateLocalTransform();
            if (_pos == _oldPos && _rot == _oldRot && _scale == _oldScale) {
                return false;
            }

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