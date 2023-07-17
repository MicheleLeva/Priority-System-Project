using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Network.Objects;
using Network.Server;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using Utils;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Network.Player {
    /// <summary>
    /// Detect objects in the circular AoI around the player and enqueue them.
    /// </summary>
    public class PlayerObjectsDetector : NetworkBehaviour {
        /// <summary>
        /// Radius of the circular AoI (it is updated by <see cref="CollidersCreator"/>)
        /// </summary>
        public float radius;
        private SendObjectQueue _objectQueue;
        private int _level;
        private List<NetObject> _previous = new();
        private ulong _clientId;
        private Prefs _prefs;

        private HashSet<NetObject> frustumCollidingObjects = new HashSet<NetObject>();

        public enum PriorityType
        {
            CircularAreasOfInterest,
            ScreenPresence
        };

        public PriorityType priorityType = PriorityType.CircularAreasOfInterest;

        /// <summary>
        /// Instantiate and attach a new <see cref="PlayerObjectsDetector"/>.
        /// </summary>
        /// <param name="where">gameObject to which attach the component (child of player's avatar)</param>
        /// <param name="objectQueue">sending queue</param>
        /// <param name="level">priority level of the zone (can be 0, 1, 2)</param>
        /// <param name="clientId">client id</param>
        /// <returns></returns>
        public static PlayerObjectsDetector CreateComponent(
            GameObject where,
            SendObjectQueue objectQueue,
            int level,
            ulong clientId,
            PriorityType pType = PriorityType.CircularAreasOfInterest) {
            // _
            var coll = where.AddComponent<PlayerObjectsDetector>();
            coll._objectQueue = objectQueue;
            coll._level = level;
            coll.radius = Prefs.Singleton.zones[level];
            coll._clientId = clientId;
            coll.priorityType = pType;
            return coll;
        }

        private void Start() {
            _prefs = Prefs.Singleton;


            StartCoroutine(ObjectsDetectionCycle());
        }


        // -------------------> Send Objects CYCLE
        /// <summary>
        /// Periodically detects new objects to enqueue.
        /// </summary>
        private IEnumerator ObjectsDetectionCycle() {
            yield return new WaitForSeconds(.5F);
            while (true) {
                DetectObjects();
                yield return new WaitForSeconds(Prefs.Singleton.zoneDetectionDelay);
            }
        }

        /// <summary>
        /// Using OverlapSphere function, detect objects in the AoI. Then, if the objects are new, enqueue them.
        /// If some object exited the AoI, then remove it from the queue.
        /// </summary>
        private void DetectObjects() {
            if (priorityType.Equals(PriorityType.CircularAreasOfInterest))
            {
                var t = transform;

                // Get colliding objects with right priority
                var collidingObjects = Physics.OverlapSphere(t.position, radius)
                    .Where(c =>
                        c.TryGetComponent<NetObject>(out var o)
                        && (!_prefs.priorityQueue || o.priority == _level))
                    .Select(c =>
                        c.GetComponent<NetObject>())
                    .ToArray();

                var selected = collidingObjects.Except(_previous).ToList();
                SendNewObjects(selected);
                var removed = _previous.Except(collidingObjects).ToList();
                Debug.Log("selected = " + selected.Count + "; removed = " + removed.Count);
                DeleteOldObjects(removed);

                // Update `previous` list
                _previous = collidingObjects.ToList();
            }
            else if (priorityType.Equals(PriorityType.ScreenPresence))
            {
                //TODO: test this
                var selected = frustumCollidingObjects.Except(_previous).ToList();
                SendNewObjects(selected);
                var removed = _previous.Except(frustumCollidingObjects).ToList();
                DeleteOldObjects(removed);

                // Update `previous` list
                _previous = frustumCollidingObjects.ToList();
                //TODO: set their priority depending on screen presence
                //TODO: add them to the queue once at a time
            }
            
        }

        /// <summary>
        /// If a NetObjects collides with the frustum collider, it gets added to the global list frustumCollidingObjects
        /// </summary>
        /// <param name="collision">Object collided</param>
        private void OnCollisionEnter(Collision collision)
        {
            //TODO: find another way to get objects inside of mesh at initialization!
            Debug.Log("Collision!");
            if (priorityType.Equals(PriorityType.ScreenPresence))
            {
                if(collision.gameObject.TryGetComponent<NetObject>(out var o))
                frustumCollidingObjects.Add(o);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("Collision!");
            if (priorityType.Equals(PriorityType.ScreenPresence))
            {
                if (other.gameObject.TryGetComponent<NetObject>(out var o))
                    frustumCollidingObjects.Add(o);
            }
        }

        /// <summary>
        /// Add new objects to the queue.
        /// </summary>
        /// <param name="selected">list of objects to enqueue</param>
        private void SendNewObjects(List<NetObject> selected) {
            foreach (var networkObj in selected) {
                var playerPos = NetworkManager.Singleton.ConnectedClients[_clientId].PlayerObject.transform.GetChild(0)
                    .position;
                var objPos = networkObj.GetComponent<MeshRenderer>().bounds.ClosestPoint(playerPos);
                var distance = (objPos - playerPos).magnitude;

                // Debug.LogError($"{networkObj.name}: {playerPos},{objPos},{distance}");
                _objectQueue.Add(
                    _clientId,
                    networkObj.gameObject,
                    Priority.Calc(networkObj.priority, distance, networkObj.facing.Count)
                );
                // if (networkObj.facing >= 1) 
                //     Debug.LogError($"facing = {networkObj.facing}");
            }
        }

        /// <summary>
        /// Delete old objects from the queue.
        /// </summary>
        /// <param name="removed">list of objects to remove</param>
        private void DeleteOldObjects(List<NetObject> removed) {
            foreach (var networkObject in removed) {
                _objectQueue.Delete(_clientId, networkObject.gameObject);
            }
        }

#if UNITY_EDITOR
        void OnDrawGizmosSelected() {
            var position = transform.position;
            Handles.color = (_level) switch {
                0 => Color.green,
                1 => Color.yellow,
                2 => Color.red,
                _ => Color.white
            };
            Handles.DrawWireDisc(position, Vector3.up, radius);
        }
#endif
    }
}