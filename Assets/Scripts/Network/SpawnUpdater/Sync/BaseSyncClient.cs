using Network.Objects;
using Network.SpawnUpdater;
using Unity.Netcode;
using UnityEngine;

namespace Network.Sync {
    /// <summary>
    /// Base class for the Client's sync components. Each specific component class  is responsible to keep updated
    /// the current Client with object's properties updates received from the Server.
    /// </summary>
    public abstract class BaseSyncClient : MonoBehaviour {
        /// <summary>
        /// Sync Manager class.
        /// </summary>
        protected SyncManager syncManager;
        /// <summary>
        /// Owner client ID.
        /// </summary>
        public ulong owner = ulong.MaxValue;
        /// <summary>
        /// Local Client ID.
        /// </summary>
        protected ulong clientId;
        /// <summary>
        /// Object ID.
        /// </summary>
        protected int objectId;
        
        /// <summary>
        /// On start, find syncManager, local Client's and object's ids.
        /// </summary>
        protected void Start() {
            BeforeStart();
            syncManager = FindObjectOfType<SyncManager>();
            clientId = NetworkManager.Singleton.LocalClientId;
            objectId = GetComponent<NetObject>().id;
            AfterStart();
        }

        /// <summary>
        /// Operations preliminary to the <see cref="Start"/> method.
        /// </summary>
        protected virtual void AfterStart() {}

        /// <summary>
        /// Operations after the <see cref="Start"/> method.
        /// </summary>
        protected virtual void BeforeStart() { }

        /// <summary>
        /// Apply the given sync data to the object, if the data class matches.
        /// </summary>
        /// <param name="data">object's property sync data</param>
        /// <returns>if the data has been correctly applied or not</returns>
        public abstract bool TryApplySync(SyncData data);

        /// <summary>
        /// Ask the Server to give the ownership to the local Client.
        /// </summary>
        /// <param name="toClient">if false, the ownership is given to the Server (max ulong value)</param>
        public void SetOwnership(bool toClient = false) {
            owner = toClient ? clientId : ulong.MaxValue;
            // Lock object into server
            syncManager.RequireOwnershipServerRpc(objectId, owner);
        }
    }
}