using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Utils;
using Network.Server;
using Network.Player;


namespace Network.Objects {
    /// <summary>
    /// Fundamental class for the objects that will be sent to the Clients.
    /// </summary>
    public class NetObject : MonoBehaviour {
        /// <summary>
        /// Object id.
        /// </summary>
        public int id;
        /// <summary>
        /// Object priority.
        /// </summary>
        public int priority;
        /// <summary>
        /// Is the object movable?
        /// </summary>
        public bool isMovable;
        /// <summary>
        /// Corners of the Bounds of the renderer, used for Screen Presence priority calculations
        /// </summary>
        public Vector3[] rendererBoundsCorners;
        /// <summary>
        /// which players are currently facing the object
        /// </summary>
        public HashSet<ulong> facing = new();
        /// <summary>
        /// client that owns this object
        /// </summary>
        public ulong owner;
        /// <summary>
        /// Has the object been sent to at least one client?
        /// </summary>
        public bool isSentToClient = false;
        /// <summary>
        /// Debug bool
        /// </summary>
        public bool error;

        private Gradient priorityGradient = new Gradient();

        private void Start() {
            if (NetworkManager.Singleton.IsServer) {
                if (id == default)
                {
                    id = this.GetID();
                    AddNetObjectToGlobalDict();
                }

                //get object Axis-Aligned Bounding Box corners of this object for Screen Presence Priority calculations
                Bounds bounds = GetComponent<Renderer>().bounds;
                rendererBoundsCorners = new Vector3[8];
                rendererBoundsCorners[0] = bounds.min;
                rendererBoundsCorners[1] = bounds.max;
                rendererBoundsCorners[2] = new Vector3(rendererBoundsCorners[0].x, rendererBoundsCorners[0].y, rendererBoundsCorners[1].z);
                rendererBoundsCorners[3] = new Vector3(rendererBoundsCorners[0].x, rendererBoundsCorners[1].y, rendererBoundsCorners[0].z);
                rendererBoundsCorners[4] = new Vector3(rendererBoundsCorners[1].x, rendererBoundsCorners[0].y, rendererBoundsCorners[0].z);
                rendererBoundsCorners[5] = new Vector3(rendererBoundsCorners[0].x, rendererBoundsCorners[1].y, rendererBoundsCorners[1].z);
                rendererBoundsCorners[6] = new Vector3(rendererBoundsCorners[1].x, rendererBoundsCorners[0].y, rendererBoundsCorners[1].z);
                rendererBoundsCorners[7] = new Vector3(rendererBoundsCorners[1].x, rendererBoundsCorners[1].y, rendererBoundsCorners[0].z);
            }
        }

        public void AddNetObjectToGlobalDict()
        {
            FindObjectOfType<ServerObjectsLoader>().AddNetObjectsToDict(gameObject);
        }

        /// <summary>
        /// Initialize an object when it is reconstructed on the Client.
        /// </summary>
        /// <param name="prior">object priority</param>
        /// <param name="mov">is movable?</param>
        /// <param name="sId">serialized object id</param>
        public void Setup(int prior, bool mov, int sId) {
            priority = prior;
            isMovable = mov;
            id = sId;
        }

        //------------------- Editor methods -------------------
        [ContextMenu("Set priority")]
        private void SetPriority() {
            var p = 3;

            if (name.Contains("Road")) p = 0;
            if (name.Contains("Ground")) p = 0;
            if (name.Contains("Building")) p = 0;
            if (name.Contains("Car")) p = 1;
            if (name.Contains("Palm")) p = 1;
            if (name.Contains("Tree")) p = 1;
            if (name.Contains("Street")) p = 2;

            for (var i = 0; i < transform.childCount; i++) {
                var c = transform.GetChild(i);
                if (c.name.Contains("Wheel") || c.name.Contains("Lights")) {
                    c.GetComponent<NetObject>().priority = 2;
                }
            }

            priority = p;
        }


        [ContextMenu("Set random color")]
        private void SetColor() {
            if (!name.Contains("Building")) return;
            var i = Random.Range(0, 4);
            SetColorOfIndex(i);
        }
        
        [ContextMenu("Set color 0")]
        private void SetColor0() {
            if (!name.Contains("Building")) return;
            SetColorOfIndex(0);
        }
        
        [ContextMenu("Set color 1")]
        private void SetColor1() {
            if (!name.Contains("Building")) return;
            SetColorOfIndex(1);
        }
        
        [ContextMenu("Set color 2")]
        private void SetColor2() {
            if (!name.Contains("Building")) return;
            SetColorOfIndex(2);
        }
        
        [ContextMenu("Set color 3")]
        private void SetColor3() {
            if (!name.Contains("Building")) return;
            SetColorOfIndex(3);
        }
        
        

        private void SetColorOfIndex(int i) {
            var grounds = Resources.LoadAll<Material>("Materials/Grounds");
            var levels = Resources.LoadAll<Material>("Materials/Levels");
            var roofs = Resources.LoadAll<Material>("Materials/Roofs");
            
            if (name.Contains("Ground"))
                GetComponent<MeshRenderer>().material = grounds[i];
            else if (name.Contains("Roof"))
                GetComponent<MeshRenderer>().material = roofs[i];
            else if (name.Contains("Level"))
                GetComponent<MeshRenderer>().material = levels[i];
            
        }

        public Vector3 colliderSize;
        public Vector3 rendererSize;

        private void OnDrawGizmos()
        {
            Bounds rendererBounds;
            int highestPriority = PlayerObjectsDetector.highestPriority;

            // Blend color from yellow at 0% to red at 100%
            var gradientColors = new GradientColorKey[2];
            gradientColors[0] = new GradientColorKey(Color.yellow, 0.0f);
            gradientColors[1] = new GradientColorKey(Color.red, 1.0f);

            // Alpha stays opaque
            var alphas = new GradientAlphaKey[2];
            alphas[0] = new GradientAlphaKey(1.0f, 1.0f);
            alphas[1] = new GradientAlphaKey(1.0f, 1.0f);

            priorityGradient.SetKeys(gradientColors, alphas);

            if (TryGetComponent(out Renderer r))
            {
                rendererBounds = r.bounds;
                if (isSentToClient)
                    Gizmos.color = Color.green;
                else
                    Gizmos.color = priorityGradient.Evaluate(priority / highestPriority);
                Gizmos.DrawWireCube(rendererBounds.center, rendererBounds.size);
                /*
                if (error)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireMesh(GetComponent<Mesh>());
                }*/
                rendererSize = rendererBounds.size;
            }

        }

    }
}