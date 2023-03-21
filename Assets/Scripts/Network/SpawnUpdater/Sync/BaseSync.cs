using System.Collections;
using Network.Objects;
using Network.SpawnUpdater;
using UnityEngine;

namespace Network.Sync {
    /// <summary>
    /// Base class for the Server's sync components. Each specific component class is responsible to update the Clients
    /// when a property of the objects is changed.
    /// </summary>
    public abstract class BaseSync : MonoBehaviour {
        private SyncManager _syncManager;
        /// <summary>
        /// NetObject component.
        /// </summary>
        private NetObject _netObj;
        /// <summary>
        /// Wrapper for the number of players facing the object.
        /// </summary>
        protected int Facing => _netObj.facing.Count;
        /// <summary>
        /// Owner Client (or Server) id.
        /// </summary>
        public ulong Owner => _netObj.owner;

        /// <summary>
        /// On start, find syncManager and the NetObject component of the object.
        /// </summary>
        protected void Start() {
            BeforeStart();
            _netObj = GetComponent<NetObject>();
            _syncManager = FindObjectOfType<SyncManager>();
            StartCoroutine(UpdateSyncCycle());
            AfterStart();
        }

        /// <summary>
        /// Operations preliminary to the <see cref="Start"/> method.
        /// </summary>
        protected virtual void BeforeStart() { }
        
        /// <summary>
        /// Operations after the <see cref="Start"/> method.
        /// </summary>
        protected virtual void AfterStart() { }


        private IEnumerator UpdateSyncCycle() {
            while (true) {
                if (UpdateConditions())
                    UpdateSync();
                yield return new WaitForSeconds(Prefs.Singleton.updateDelay);
            }
        }

        /// <summary>
        /// Check if the conditions to update a specific property are respected
        /// (e.g. if an object has moved more that a certain threshold).
        /// </summary>
        /// <returns></returns>
        protected abstract bool UpdateConditions();

        /// <summary>
        /// Create and send the property's data object.
        /// </summary>
        public abstract void UpdateSync();

        /// <summary>
        /// Send the sync update to the Clients
        /// </summary>
        /// <param name="data">object's property data</param>
        protected void SendSync(SyncData data) {
            _syncManager.SyncClientRpc(_netObj.id, data);
        }
    }
}