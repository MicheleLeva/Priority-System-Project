using System;
using System.Collections;
using Network.Client;
using Network.Player;
using Network.SpawnUpdater;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.VisualScripting;
using UnityEngine;
using Utils;

namespace Network.Server {
    /// <summary>
    /// Initialize frustum collider and objects detector. Keep updated the zones' radius according to
    /// the registered RTT.
    /// </summary>
    [RequireComponent(typeof(SendObjectQueue))]
    public class CollidersCreator : NetworkBehaviour {
        private SendObjectQueue _objectQueue;
        private UnityTransport _transport;

        private PlayerObjectsDetector _c0, _c1, _c2;

        private void Start() {
            _transport = (UnityTransport) NetworkManager.Singleton.NetworkConfig.NetworkTransport;
            NetworkManager.OnServerStarted += () => {
                _objectQueue = GetComponent<SendObjectQueue>();

                NetworkManager.OnClientConnectedCallback += OnClientConnected;
                if (IsHost) {
                    OnClientConnected(NetworkManager.Singleton.LocalClientId);
                }
            };
        }

        /// <summary>
        /// On new client connection, initialize colliders.
        /// </summary>
        /// <param name="clientId">client id</param>
        private void OnClientConnected(ulong clientId) {
            var player = NetworkManager.ConnectedClients[clientId].PlayerObject;
            var head = player.transform.GetChild(0);
            if(Prefs.Singleton.priorityType.Equals(Prefs.PriorityType.CircularAreasOfInterest))
                InitRingsColliders(head, clientId);
            InitFrustumCollider(head, clientId);
            
            if (Prefs.Singleton.priorityQueue)
            {
                if(Prefs.Singleton.priorityType.Equals(Prefs.PriorityType.CircularAreasOfInterest))
                    StartCoroutine(ResizeRadiusCycle(clientId));
            }
                
        }


        // Create sphere rings (on server)
        /// <summary>
        /// Create empty component and attach to it one new <see cref="PlayerObjectsDetector"/> for each zone.
        /// </summary>
        /// <param name="parent">head of the Client's player to which link the zones</param>
        /// <param name="clientId">client id</param>
        private void InitRingsColliders(Transform parent, ulong clientId) {
            var goRings = new GameObject("Collider") {
                transform = {
                    parent = parent,
                    localPosition = Vector3.zero,
                    localRotation = new Quaternion(),
                    localScale = Vector3.one
                }
            };

            if (Prefs.Singleton.priorityQueue) { //3 detector rings are attached, one for each priority Area of Interest
                _c0 = PlayerObjectsDetector.CreateComponent(goRings, _objectQueue, 0, clientId);
                _c1 = PlayerObjectsDetector.CreateComponent(goRings, _objectQueue, 1, clientId);
                _c2 = PlayerObjectsDetector.CreateComponent(goRings, _objectQueue, 2, clientId);
            }
            else {
                PlayerObjectsDetector.CreateComponent(goRings, _objectQueue, 2, clientId);
            }
        }

        /// <summary>
        /// Create the frustum mesh of the camera and initialize a new <see cref="PlayerFrustumCollider"/>,
        /// centered on the player head.
        /// </summary>
        /// <param name="parent">head of the Client's player to which link the frustum</param>
        /// <param name="clientId">client id</param>
        private void InitFrustumCollider(Transform parent, ulong clientId) {
            // if (NetworkManager.Singleton.LocalClientId != client) return; // only on one client

            var clients = NetworkManager.Singleton.ConnectedClients;
            if (!clients.ContainsKey(clientId)) return;
            var client = clients[clientId];

            var goFrustum = new GameObject("Frustum") {
                transform = {
                    parent = parent,
                    localPosition = Vector3.zero,
                    localRotation = new Quaternion(),
                    localScale = Vector3.one
                }
            };
            // add camera frustum as collider
            var frustumCollider = goFrustum.AddComponent<MeshCollider>();
            frustumCollider.sharedMesh = (
                    client.PlayerObject.GetComponentInChildren<Camera>()
                    ?? Camera.main
                )
                .GenerateFrustumMesh(100);
            frustumCollider.convex = true;
            frustumCollider.isTrigger = true;
            // add frustum script
            goFrustum.AddComponent<PlayerFrustumCollider>();
            //add player object detector script to frustum collider
            if (Prefs.Singleton.priorityType.Equals(Prefs.PriorityType.ScreenPresence))
            {
                StartCoroutine(EnablePlayerObjectsDetector(goFrustum, clientId, frustumCollider));

            }      
            var rb = goFrustum.AddComponent<Rigidbody>();
            rb.isKinematic = true;
        }

        /// <summary>
        /// Used as a coroutine to delay the initialization of the playerObjectsDetector
        /// It avoids client player objects jittering
        /// </summary>
        private IEnumerator EnablePlayerObjectsDetector(GameObject goFrustum, ulong clientId, Collider coll)
        {
            yield return new WaitForSeconds(0);
            Debug.Log("PlayerObjectsDetector for client " + clientId + "is now enabled.");
            PlayerObjectsDetector pod =
                    PlayerObjectsDetector.CreateComponent(goFrustum, _objectQueue, 2, clientId, Prefs.PriorityType.ScreenPresence);

            //reset collider to get new collision triggers
            coll.enabled = false;
            coll.enabled = true;
        }

        // -------------------> Update Radius CYCLE
        private IEnumerator ResizeRadiusCycle(ulong clientId) {
            while (true) {
                yield return new WaitForSeconds(Prefs.Singleton.radiusUpdateDelay);
                UpdateRadiusByRtt(clientId);
            }
        }

        /// <summary>
        /// Update AoIs' radius according to the current RTT of the Client.
        /// </summary>
        /// <param name="clientId">client id</param>
        private void UpdateRadiusByRtt(ulong clientId) {
            
            var rtt = _transport.GetCurrentRtt(clientId);
            //Debug.LogError($"RTT: {rtt}");

            // new zone radius
            var newRad = -(float) Math.Log(rtt / 1000F) * 30F;
            newRad = newRad > 0 ? newRad * .3F + _c0.radius * .7F : 0;

            _c0.radius = newRad;
            _c1.radius = newRad / 2;
            _c2.radius = newRad / 4;
        }
    }
}