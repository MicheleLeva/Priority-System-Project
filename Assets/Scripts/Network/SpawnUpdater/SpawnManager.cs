using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Network.Client;
using Network.Http;
using Network.Objects;
using Network.Server;
using Serializers;
using Unity.Netcode;
using UnityEngine;
using VR;

namespace Network.SpawnUpdater {
    /// <summary>
    /// Class that manages objects' streaming and building functionalities.
    /// </summary>
    public class SpawnManager : NetworkBehaviour {
        // ------------------------------------- Server - Send Object
        public ComponentServerHttp componentServerHttp;
        public ComponentClientHttp componentClientHttp;

        private readonly ComponentsCache _componentsCache = ComponentsCache.Singleton;

        private readonly ClientCache _clientCache = ClientCache.Singleton;
        private readonly ServerCache _serverCache = ServerCache.Singleton;

        // ----------------------------------------------------------------------------
        // Server
        // ----------------------------------------------------------------------------
        private void Start() {
            NetworkManager.OnServerStarted += () => {
                Debug.Log("Server Started");
                componentServerHttp.StartServer();
            };
        }

        /// <summary>
        /// If not present, add object's components to the Server local buffer and send object's initial information
        /// to the Client.
        /// </summary>
        /// <param name="client">id of the client to which send the object</param>
        /// <param name="obj">object to send</param>
        public void SendObject(ulong client, GameObject obj) {
            // Object
            var sObj = new SObject(obj);
            _serverCache.TryAdd(sObj.Id, sObj);

            // Mesh
            if (obj.TryGetComponent<MeshFilter>(out var mf)) {
                var sMesh = new SMesh(mf.mesh);
                componentServerHttp.AddComponentToServerCache(sMesh);
                sObj.AddComponent(sMesh);
            }

            // Material & Texture
            if (obj.TryGetComponent<MeshRenderer>(out var mr)) {
                for (var i = 0; i < mr.materials.Length; i++) {
                    var material = mr.materials[i];
                    var sMaterial = new SMaterial(material, i);
                    componentServerHttp.AddComponentToServerCache(sMaterial);
                    sObj.AddComponent(sMaterial);
                }
            }

            // Send
            SendClientRpc(client, sObj);
        }

        // ----------------------------------------------------------------------------
        // Client
        // ----------------------------------------------------------------------------

        /// <summary>
        /// Client RPC to receive and build the object on the Client. Adds the needed components, searches in buffer
        /// or requires the object's assets and builds it.
        /// </summary>
        /// <param name="client">local client id, to check if the client needs to build that object</param>
        /// <param name="sObj">serialized object</param>
        [ClientRpc(Delivery = RpcDelivery.Reliable)]
        public void SendClientRpc(ulong client, SObject sObj) {
            if (NetworkManager.Singleton.LocalClientId != client) return;

            // Get or build Object
            if (TryGetFromObjectCache(ref sObj)) {
                Debug.Log("Getting object from cache");
                sObj.Obj.GetComponent<MeshRenderer>().enabled = true;
                HideObjectsNotOfPriority(Startup.ShowOnlyPriority, sObj.Obj.GetComponent<MeshRenderer>(),
                    sObj.Priority);
                return;
            }

            Logger.ObjDelays.Add(sObj.Id, Time.time);

            var obj = sObj.Obj;


            // Add NetObject (and eventually Sync)
            var netObj = obj.AddComponent<NetObject>();
            netObj.Setup(sObj.Priority, sObj.IsMovable, sObj.Id);
            obj.layer = sObj.Layer;

            obj.AddComponent<MeshFilter>();
            obj.AddComponent<MeshCollider>();
            var mr = obj.AddComponent<MeshRenderer>();

            HideObjectsNotOfPriority(Startup.ShowOnlyPriority, mr, netObj.priority);

            if (sObj.IsMovable) {
                obj.AddComponent<Rigidbody>().isKinematic = true;
                obj.AddComponent<TwoHandsInteractable>();
            }

            // Attach Components
            foreach (var id in sObj.GetComponentIds()
                         .Where(id => id != 0)) {
                AttachComponentToObject(id, sObj);
            }

            sObj.Obj.SetActive(true);
        }

        /// <summary>
        /// (for test only) Hide the object if has certain priorities.
        /// </summary>
        private void HideObjectsNotOfPriority(int showOnlyPriority, Renderer mr, int priority) {
            if (showOnlyPriority == -1) return;
            if (priority != showOnlyPriority)
                mr.enabled = false;
        }

        /// <summary>
        /// Search or adds the object to the Client's local cache.
        /// </summary>
        /// <param name="sObj">serialized object</param>
        /// <returns></returns>
        private bool TryGetFromObjectCache(ref SObject sObj) {
            if (_clientCache.ContainsKey(sObj.Id)) {
                sObj = _clientCache[sObj.Id];
                return true;
            }

            var obj = sObj.BuildObject();
            if (sObj.Parent != 0) {

                StartCoroutine(
                    WaitForParent(obj.transform, sObj.Parent, 2)
                );
            }

            _clientCache.Add(sObj.Id, sObj);
            return false;
        }

        /// <summary>
        /// Wait until the parent of an object is also received. Then adjust accordingly the object's transform.
        /// </summary>
        /// <param name="objTransform">object transform</param>
        /// <param name="parent">id of the parent object</param>
        /// <param name="delay">seconds to wait between checks</param>
        /// <returns></returns>
        private IEnumerator WaitForParent(Transform objTransform, int parent, int delay) {
            var scale = objTransform.localScale; // Memorize scale
            objTransform.localScale = Vector3.zero; //objects gest "hidden" by setting its scale to zero while waiting for its parent

            var found = false;
            while (!found) {
                found = _clientCache.ContainsKey(parent);
                yield return new WaitForSeconds(delay);
            }

            Debug.Log("Parent of object " + objTransform.gameObject.name + "has been found!");

            objTransform.parent = _clientCache[parent].Obj.transform;
            // Re-adjust scale
            objTransform.localScale = scale;
        }

        /// <summary>
        /// Attach an asset (component) to an object if present. If not present, wait until the asset is downloaded and then
        /// attach it.
        /// </summary>
        /// <param name="compId">component id </param>
        /// <param name="sObj">serialized object</param>
        private async Task AttachComponentToObject(int compId, SObject sObj) {
            SComponent sComponent = null;
            if (_componentsCache.ContainsKey(compId)) {
                if (_componentsCache.IsDownloaded(compId)) {
                    sComponent = _componentsCache.GetComp(compId);
                    sComponent.AttachTo(sObj);
                }
                else {
                    _componentsCache.AddAwaiter(compId, sObj);
                }
            }
            else {
                _componentsCache.InitComp(compId);
                sComponent = await componentClientHttp.GetComponentFromHttpServer(compId);
                _componentsCache.SetComp(compId, sComponent);
                sComponent.AttachTo(sObj);

                if (Logger.ObjTimes)
                    Logger.ObjTimesStr +=
                        $"{Logger.ObjDelays[sObj.Id]:f3}, " +
                        $"{Time.time:f3}, " +
                        $"{sObj.Name}, " +
                        $"{sObj.Priority}, " +
                        $"{sComponent.GetType()}, " +
                        $"{sComponent.GetSize()}\n";
            }
        }
    }
}