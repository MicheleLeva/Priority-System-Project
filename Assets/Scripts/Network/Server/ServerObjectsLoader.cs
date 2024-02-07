using Network.Objects;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Network.Server {
    /// <summary>
    /// Load the initial scene's prefabs on the Server, from the Resource folder.
    /// </summary>
    [RequireComponent(typeof(SendObjectQueue))]
    public class ServerObjectsLoader : NetworkBehaviour {

        public GameObject world;

        public static Dictionary<int, NetObject> netObjects = new Dictionary<int, NetObject>();

        private void Start() {
            NetworkManager.OnServerStarted += () => {
                var resources = Resources.LoadAll<GameObject>("Env");
                foreach (var res in resources) {
                    GameObject gameObject =  Instantiate(res, world.GetComponent<Transform>());
                    StartCoroutine(AddNetObjectsDict(gameObject));
                }
            };
        }

        private IEnumerator AddNetObjectsDict(GameObject gameObject)
        {
            yield return null;
            NetObject netObject = gameObject.GetComponent<NetObject>();
            netObjects.Add(netObject.id, netObject);
        }
    }
}