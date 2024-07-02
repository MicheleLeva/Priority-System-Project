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
                Debug.Log($"number of objects in resources = {resources.Length}");
                foreach (var res in resources) {
                    //if (res.name != "Road_Straight")
                    {
                        GameObject gObject = Instantiate(res, world.GetComponent<Transform>());
                        NetObject netObject = gObject.GetComponent<NetObject>();
                    }
                    
                }
            };
        }

        public void AddNetObjectsToDict(GameObject gameObject)
        {
            NetObject netObject = gameObject.GetComponent<NetObject>();
            netObjects.Add(netObject.id, netObject);
        }
    }
}