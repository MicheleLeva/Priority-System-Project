using Unity.Netcode;
using UnityEngine;

namespace Network.Server {
    /// <summary>
    /// Load the initial scene's prefabs on the Server, from the Resource folder.
    /// </summary>
    [RequireComponent(typeof(SendObjectQueue))]
    public class ServerObjectsLoader : NetworkBehaviour {
        private void Start() {
            NetworkManager.OnServerStarted += () => {
                var resources = Resources.LoadAll<GameObject>("Env");
                foreach (var res in resources) {
                    Instantiate(res);
                }
            };
        }
    }
}