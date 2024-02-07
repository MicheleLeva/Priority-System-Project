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
using static UnityEngine.UI.GridLayoutGroup;
using Unity.VisualScripting;
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
        private Camera playerCamera;
        float screenArea;
        private Prefs _prefs;

        /// <summary>
        /// Variables used by ScreenPresence priority system
        /// </summary>
        //set of objects currently colliding with the frustumCollider
        private HashSet<int> frustumCollidingObjectsIds = new HashSet<int>();
        //set of object to be sent
        private HashSet<int> toBeSent = new HashSet<int>();
        //set of objects already sent to the client
        private HashSet<int> sentObjects = new HashSet<int>();

        private int sendToQueueMax = 10;
        
        Prefs.PriorityType priorityType;

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
            Prefs.PriorityType pType = Prefs.PriorityType.CircularAreasOfInterest) {
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

            NetworkObject playerObj = NetworkManager.Singleton.ConnectedClients[_clientId].PlayerObject;
            playerCamera = playerObj.GetComponentInChildren<Camera>() ?? Camera.main;
            screenArea = playerCamera.pixelHeight * playerCamera.pixelWidth;
            Debug.Log($"Screen: pixelHeight = {playerCamera.pixelHeight}, pixelWidth = {playerCamera.pixelWidth}, area = {screenArea}");

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
        /// -- Areas of Interest: 
        /// Using OverlapSphere function, detect objects in the AoI. Then, if the objects are new, enqueue them.
        /// If some object exited the AoI, then remove it from the queue.
        /// -- Screen Presence:
        /// Use Frustum Mesh Collider to find objects to enqueue.
        /// </summary>
        private void DetectObjects() {
            if (priorityType.Equals(Prefs.PriorityType.CircularAreasOfInterest))
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
                //Debug.Log("selected = " + selected.Count + "; removed = " + removed.Count);
                DeleteOldObjects(removed);

                // Update `previous` list
                _previous = collidingObjects.ToList();
            }
            else if (priorityType.Equals(Prefs.PriorityType.ScreenPresence))
            {
                
                var selected = frustumCollidingObjectsIds.Except(sentObjects);
                if (selected.Count() > 0)
                {
                    if (selected.Count() > sendToQueueMax)
                        selected = selected.Take(sendToQueueMax);

                    var netObjects = selected.
                    Where(k => ServerObjectsLoader.netObjects.ContainsKey(k)).Select(k => ServerObjectsLoader.netObjects[k]).ToList();
                    SendNewObjects(netObjects);
                    sentObjects.AddRange(selected);
                    Debug.Log($"Number of FrustumCollidingObjectsIds = {frustumCollidingObjectsIds.Count()}, Selected {selected.Count()} objects, sent {netObjects.Count()} objects");

                    //var removed = _previous.Except(frustumCollidingObjects).ToList();
                    //DeleteOldObjects(removed);

                    // Update `previous` list
                    //_previous = frustumCollidingObjects.ToList();
                }

            }
            
        }

        /// <summary>
        /// If a NetObjects collides with the frustum collider, it gets added to the global list frustumCollidingObjects
        /// </summary>
        /// <param name="collision">Object collided</param>
        private void OnTriggerEnter(Collider other)
        {
            //Debug.Log("Collision!");
            if (priorityType.Equals(Prefs.PriorityType.ScreenPresence))
            {
                
                if (other.gameObject.TryGetComponent<NetObject>(out var o))
                {
                    frustumCollidingObjectsIds.Add(o.id);
                    /*
                    if (IsObjectCompletelyOccluded(other.gameObject))
                    {
                        Debug.Log($"Checking object {o.name} --> not occluded");
                        
                    } else
                        //Debug.Log($"Checking object {o.name} --> Occluded");
                        Debug.Log($"Occluded");
                    */

                }
                    
            }
        }

        /// <summary>
        /// If a NetObjects stops colliding with the frustum collider, it gets removed from the global list frustumCollidingObjects
        /// </summary>
        /// <param name="other">Object which stopped colliding</param>
        private void OnTriggerExit(Collider other)
        {
            if (priorityType.Equals(Prefs.PriorityType.ScreenPresence))
            {
                if (other.gameObject.TryGetComponent<NetObject>(out var o))
                    frustumCollidingObjectsIds.Remove(o.id);
            }
        }

        
        private bool IsObjectCompletelyOccluded(GameObject obj)
        {
            Vector3 playerPos = NetworkManager.Singleton.ConnectedClients[_clientId].PlayerObject.
                GetComponentInChildren<Collider>().transform.position;
            float distance = (playerPos - obj.transform.position).magnitude;

            //get all corners of object AABB
            Bounds bounds = obj.GetComponent<Renderer>().bounds;
            Vector3[] corners = new Vector3[8];
            corners[0] = bounds.min;
            corners[1] = bounds.max;
            corners[2] = new Vector3(corners[0].x, corners[0].y, corners[1].z);
            corners[3] = new Vector3(corners[0].x, corners[1].y, corners[0].z);
            corners[4] = new Vector3(corners[1].x, corners[0].y, corners[0].z);
            corners[5] = new Vector3(corners[0].x, corners[1].y, corners[1].z);
            corners[6] = new Vector3(corners[1].x, corners[0].y, corners[1].z);
            corners[7] = new Vector3(corners[1].x, corners[1].y, corners[0].z);

            //for each corners perform a raycast to player, if any reaches the player the object is not completely occluded --> false
            Ray ray;
            Color color = UnityEngine.Random.ColorHSV();
            foreach(Vector3 corner in corners)
            {
                ray = new Ray(corner, playerPos - corner);
                if (Physics.Raycast(ray, out RaycastHit hit, Vector3.Distance(corner, playerPos)))
                {
                    //Debug.DrawRay(corner, (playerPos - corner).normalized * hit.distance, color, 120);
                    //if the first object reached by the ray is the player then the object is not completely occluded
                    if ((hit.collider.transform.position - playerPos).magnitude <= 1)
                        return false;
                }
                    
            }

            //if all the corners are occluded but the center of the object is still visible, it is not completely occluded --> false
            //otherwise it is completely occluded
            ray = new Ray(playerPos, obj.transform.position - playerPos);
            if (Physics.Raycast(ray, out RaycastHit raycastHit, distance))
            {
                //Debug.DrawRay(obj.transform.position, (playerPos - obj.transform.position).normalized * raycastHit.distance, color, 120);
                //if the first object reached by the ray is the player then the object is not completely occluded
                if ((raycastHit.collider.transform.position - playerPos).magnitude <= 1)
                    return false;
            }
            return true;
        }


        /// <summary>
        /// Add new objects to the queue.
        /// </summary>
        /// <param name="selected">list of objects to enqueue</param>
        private void SendNewObjects(List<NetObject> selected) {

            var playerPos = NetworkManager.Singleton.ConnectedClients[_clientId].PlayerObject.transform.GetChild(0)
                    .position;

            Debug.Log($"Sending {selected.Count} objects to client {_clientId}");

            foreach (var networkObj in selected) {

                var objPos = networkObj.GetComponent<MeshRenderer>().bounds.ClosestPoint(playerPos);
                var distance = (objPos - playerPos).magnitude;

                if (priorityType.Equals(Prefs.PriorityType.CircularAreasOfInterest))
                {
                    // Debug.LogError($"{networkObj.name}: {playerPos}, {objPos}, {distance}");
                    _objectQueue.Add(
                        _clientId,
                        networkObj.gameObject,
                        Priority.Calc(networkObj.priority, distance, networkObj.facing.Count)
                    );

                    // if (networkObj.facing >= 1) 
                    //     Debug.LogError($"facing = {networkObj.facing}");
                }
                else if (priorityType.Equals(Prefs.PriorityType.ScreenPresence))
                {
                    //get object Axis-Aligned Bounding Box corners of object to send
                    GameObject obj = networkObj.gameObject;
                    Bounds bounds = obj.GetComponent<Renderer>().bounds;
                    Vector3[] corners = new Vector3[8];
                    corners[0] = bounds.min;
                    corners[1] = bounds.max;
                    corners[2] = new Vector3(corners[0].x, corners[0].y, corners[1].z);
                    corners[3] = new Vector3(corners[0].x, corners[1].y, corners[0].z);
                    corners[4] = new Vector3(corners[1].x, corners[0].y, corners[0].z);
                    corners[5] = new Vector3(corners[0].x, corners[1].y, corners[1].z);
                    corners[6] = new Vector3(corners[1].x, corners[0].y, corners[1].z);
                    corners[7] = new Vector3(corners[1].x, corners[1].y, corners[0].z);

                    //find the bottom leftmost corner and the top rightmost corner on screen for screen presence calculation
                    //init at topRight for search purposes
                    Vector3 bottomLeftP = new Vector3(playerCamera.pixelWidth, playerCamera.pixelHeight);
                    //init at bottomLeft for search purposes
                    Vector3 topRightP = Vector3.zero; 
                    foreach (Vector3 c in corners)
                    {
                        //get screenPoint of bounds corner
                        Vector3 screenP = playerCamera.WorldToScreenPoint(c);

                        //is the corner the bottom leftmost point?
                        if (screenP.x < bottomLeftP.x && screenP.y < bottomLeftP.y) bottomLeftP = screenP;

                        //is the corner the top rightmost point?
                        if (screenP.x > topRightP.x && screenP.y > topRightP.y) topRightP = screenP;
                    }

                    //Debug.LogWarning($"{networkObj.name}: bottomleft = {bottomLeftP}, topright = {topRightP}");

                    //sanification as points outside the screen have negative coordinates
                    if (bottomLeftP.x < 0) bottomLeftP.x = 0;
                    if (bottomLeftP.y < 0) bottomLeftP.y = 0;
                    if (topRightP.x > playerCamera.pixelWidth) topRightP.x = playerCamera.pixelWidth;
                    if (topRightP.y > playerCamera.pixelHeight) topRightP.y = playerCamera.pixelHeight;

                    //find screen presence percentage
                    float screenPresencePercentage = ((topRightP.x - bottomLeftP.x) * (topRightP.y - bottomLeftP.y)) / screenArea;

                    if (screenPresencePercentage >= 1 || screenPresencePercentage <= 0)
                        Debug.LogError($"Screen presence percentage of the object {networkObj.name} is weird = " +
                            $"{screenPresencePercentage}! " );

                    if (bottomLeftP.z < 0 && topRightP.z < 0)
                        Debug.LogError($"This object {networkObj.name} is behind the player!");

                    //Gets the distance of the object from the center of screen
                    float distanceFromScreenCenterPercentage = DistanceFromScreenCenterPercentage(
                        playerCamera.WorldToScreenPoint(obj.transform.position), playerCamera.pixelWidth, playerCamera.pixelHeight);

                    //calculate priority
                    int priority = Priority.CalcWithScreenPresence(screenPresencePercentage, distance, distanceFromScreenCenterPercentage);

                    /*Debug.LogWarning($"{networkObj.name}: distance = {distance}, screenPresencePercentage = {screenPresencePercentage}" +
                        $", Priority = {priority}");*/

                    //enqueue object with calculated priority
                    _objectQueue.Add(_clientId, networkObj.gameObject, priority);
                }

                networkObj.isSentToClient = true;
            }
        }

        /// <summary>
        /// Calculates how long is the distance of the object from the center of screen in percentage of the max distance
        /// </summary>
        /// <returns>Percentage of distance from screen center</returns>
        private float DistanceFromScreenCenterPercentage(Vector3 objectPositionOnScreen, float cameraWidth, float cameraHeight)
        {
            objectPositionOnScreen.z = 0; //depth is not needed
            Vector3 centerOfScreen = new(cameraWidth / 2f, cameraHeight / 2f, 0);
            float distance = Vector3.Distance(centerOfScreen, objectPositionOnScreen);
            float maxDistance = Vector3.Distance(centerOfScreen, new Vector3(cameraWidth, cameraHeight));
            return distance / maxDistance;
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