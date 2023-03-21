using UnityEngine;

namespace Network.Sync {
    /// <summary>
    /// Class to update material's colors changes on the Client. 
    /// </summary>
    public class SyncColorClient : BaseSyncClient {
        /// <inheritdoc />
        /// Actively ask the Server for an update (needed when the Client connects after a property
        /// has already been changed).
        protected override void AfterStart() {
            syncManager.AskForUpdateServerRpc(objectId);
        }

        /// <inheritdoc />
        public override bool TryApplySync(SyncData data) {
            if (data.Data is not ColorData d) return false;
            var material = GetComponent<MeshRenderer>().materials[d.colorIndex];
            material.color = (Color32) d.color;
            return true;
        }
    }
}