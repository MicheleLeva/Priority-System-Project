using System;
using System.Collections;
using Network.SpawnUpdater;
using Unity.Netcode;
using UnityEngine;

namespace Network.Server {
    /// <summary>
    /// Class that contains the loop to send the initial information of the objects to the Clients.
    /// </summary>
    [RequireComponent(typeof(SendObjectQueue))]
    public class SendObjectsLoop : MonoBehaviour {
        private SendObjectQueue _objectQueue;

        private void Start() {
            NetworkManager.Singleton.OnServerStarted += () => {
                _objectQueue = GetComponent<SendObjectQueue>();

                NetworkManager.Singleton.OnClientConnectedCallback +=
                    client => _objectQueue.AddClient(client);

                NetworkManager.Singleton.OnClientDisconnectCallback +=
                    client => _objectQueue.RemoveClient(client);

                StartCoroutine(Loop());
            };
        }

        /// <summary>
        /// Each cycle, get the first element of the queue and send it.
        /// </summary>
        private IEnumerator Loop() {
            var sm = FindObjectOfType<SpawnManager>();
            while (true) {
                foreach (var client in _objectQueue.Clients) {
                    if (_objectQueue.Size(client) > 0) {
                        sm.SendObject(client, _objectQueue.Get(client));
                    }
                }

                yield return new WaitForSeconds(Prefs.Singleton.sendDelay);
            }
        }
    }
}