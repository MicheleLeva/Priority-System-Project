using UnityEngine;

namespace Network.Sync {
    /// <summary>
    /// Class to sync lights changes on the Client. 
    /// </summary>
    public class SyncLightClient : BaseSyncClient {
        /// <inheritdoc />
        /// Add new light component to the object if not present
        protected override void BeforeStart() {
            if (GetComponent<Light>() == null) {
                gameObject.AddComponent<Light>();
            }
        }

        /// <inheritdoc />
        /// Actively ask the Server for an update.(needed when the Client connects after a property
        /// has already been changed).
        protected override void AfterStart() {
            syncManager.AskForUpdateServerRpc(objectId);
        }


        /// <inheritdoc />
        public override bool TryApplySync(SyncData data) {
            if (data.Data is not LightData d) return false;

            var l = GetComponent<Light>();
            l.intensity = d.intensity;
            l.color = (Color32) d.color;
            l.type = d.type;
            return true;
        }
    }
}