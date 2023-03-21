using System.Linq;
using Network.Client;
using Network.Objects;
using Network.Server;
using Network.Sync;
using Serializers;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

namespace Network.SpawnUpdater {
    public class SyncManager : NetworkBehaviour {
        // ----------------------------------------- Sync Owner
        /// <summary>
        /// Server RPC to set the owner of an object.
        /// </summary>
        /// <param name="id">object id</param>
        /// <param name="owner">owner client's id</param>
        [ServerRpc(RequireOwnership = false)]
        public void RequireOwnershipServerRpc(int id, ulong owner) {
            var obj = FindObject(id, IsServer).Obj;

            // obj.GetComponent<NetObject>().owner = owner;
            foreach (var netObject in obj.GetComponentsInChildren<NetObject>()) {
                netObject.owner = owner;
            }

            LockGravity(
                obj.GetComponent<Rigidbody>(),
                owner != ulong.MaxValue);
        }

        /// <summary>
        /// Enable or disable gravity for an object on the Server. Needed to avoid conflicts when a player
        /// is holding it.
        /// </summary>
        /// <param name="rigidbody">object's rigidbody</param>
        /// <param name="locking">enable or disable object's gravity?</param>
        private static void LockGravity(Rigidbody rigidbody, bool locking) {
            rigidbody.isKinematic = locking;
            rigidbody.angularDrag = locking ? 0 : .05F;
        }


        // ----------------------------------------- Sync Server to Clients
        /// <summary>
        /// Client RPC to update an object's property.
        /// </summary>
        /// <param name="id">object id</param>
        /// <param name="data">object's property data to sync</param>
        [ClientRpc(Delivery = RpcDelivery.Unreliable)]
        public void SyncClientRpc(int id, SyncData data) {
            var obj = FindObject(id, IsServer);

            obj?.Obj
                .GetComponents<BaseSyncClient>()
                .ToList()
                .ForEach(sync => sync.TryApplySync(data));
        }


        // ----------------------------------------- Sync Client to Server
        /// <summary>
        /// Server RPC to receive object's activation signals from the Clients.
        /// </summary>
        /// <param name="id">object id</param>
        [ServerRpc(RequireOwnership = false)]
        public void SendSignalServerRpc(int id) {
            var obj = FindObject(id, IsServer);

            obj?.Obj
                .GetComponents<IAction>()
                .ToList()
                .ForEach(a => a.Activate());
        }

        /// <summary>
        /// Server RPC to receive transform updates for an object from the Clients.
        /// </summary>
        /// <param name="id">object id</param>
        /// <param name="data">object's transform data to sync</param>
        [ServerRpc(RequireOwnership = false)]
        public void UpdateTransformServerRpc(int id, SyncData data) {
            var obj = FindObject(id, IsServer);

            obj?.Obj.GetComponent<SyncTransform>().UpdateServerTransform(data);
        }

        /// <summary>
        /// Server RPC to let the Clients actively ask object's properties updates to the Server.
        /// </summary>
        /// <param name="id">object id</param>
        [ServerRpc(RequireOwnership = false)]
        public void AskForUpdateServerRpc(int id) {
            var obj = FindObject(id, IsServer);
            obj?.Obj.GetComponents<BaseSync>()
                .ToList()
                .ForEach(s => {
                    if (s is not SyncTransform)
                        s.UpdateSync();
                });
        }

        // ----------------------------------------- Utils
        /// <summary>
        /// Find and return an object from the local cache (on both Server or Client). 
        /// </summary>
        /// <param name="id">object id</param>
        /// <param name="isServer">is Server or Client?</param>
        /// <returns></returns>
        private static SObject FindObject(int id, bool isServer) {
            SObject sObj;
            if (isServer) ServerCache.Singleton.TryGetValue(id, out sObj);
            else ClientCache.Singleton.TryGetValue(id, out sObj);
            return sObj;
        }
    }
}