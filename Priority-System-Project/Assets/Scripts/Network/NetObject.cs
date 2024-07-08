using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Utils;
using Network.Server;
using Network.Player;
using Unity.Collections.LowLevel.Unsafe;
using System.Diagnostics.Tracing;


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
        
        //debug variables
        public bool inFrustum;
        public bool error;
        private Gradient priorityGradientGreenToYellow = new Gradient();
        private Gradient priorityGradientYellowToRed = new Gradient(); 

        private void Start() {
            if (NetworkManager.Singleton.IsServer) {
                if (id == default)
                {
                    id = this.GetID();
                    AddNetObjectToGlobalDict();
                }

                //get object Axis-Aligned Bounding Box corners of this object for Screen Presence Priority calculations
                Bounds bounds = GetComponent<Renderer>().bounds.size.magnitude >= GetComponent<Collider>().bounds.size.magnitude ? 
                    GetComponent<Collider>().bounds : GetComponent<Renderer>().bounds;
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

            //Gradient preparation for Unity Editor debugging
            // Blend color from green at 0% to yellow at 0% to red at 100%
            var gradientColorGreenToYellow = new GradientColorKey[2];
            gradientColorGreenToYellow[0] = new GradientColorKey(Color.green, 0.0f);
            gradientColorGreenToYellow[1] = new GradientColorKey(Color.yellow, 1.0f);

            // Blend color from green at 0% to yellow at 0% to red at 100%
            var gradientColorYellowToRed = new GradientColorKey[2];
            gradientColorYellowToRed[0] = new GradientColorKey(Color.yellow, 0.0f);
            gradientColorYellowToRed[1] = new GradientColorKey(Color.red, 1.0f);

            // Alpha stays opaque
            var alphas = new GradientAlphaKey[2];
            alphas[0] = new GradientAlphaKey(1.0f, 1.0f);
            alphas[1] = new GradientAlphaKey(1.0f, 1.0f);

            priorityGradientGreenToYellow.SetKeys(gradientColorGreenToYellow, alphas);
            priorityGradientYellowToRed.SetKeys(gradientColorYellowToRed, alphas);
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

        private void OnDrawGizmos()
        {
            Bounds rendererBounds;

            int h = GlobalVariables.Instance != null ? GlobalVariables.Instance.highestAssignedPriority : 1;
            int l = GlobalVariables.Instance != null ? GlobalVariables.Instance.lowestAssignedPriority : 0;
            float m = l + ((h - l) / 2f);

            if (TryGetComponent(out Renderer r))
            {
                rendererBounds = r.bounds;

                if (GlobalVariables.Instance.seeFrustum)
                {
                    if (inFrustum)
                    {
                        Gizmos.color = Color.green;
                        Gizmos.DrawWireCube(rendererBounds.center, rendererBounds.size);
                    }
                    else
                    {
                        Gizmos.color = Color.red;
                        Gizmos.DrawWireCube(rendererBounds.center, rendererBounds.size);
                    }
                }

                if (GlobalVariables.Instance.seePriorities)
                {
                    if (isSentToClient)
                    {
                        Gizmos.color = Color.white;
                        Gizmos.DrawWireCube(rendererBounds.center, rendererBounds.size);
                    } 
                    else
                    {
                        //gate for sanity
                        if (priority <= 0) priority = 1;

                        if (priority < m) // find gradient for first half
                            Gizmos.color = priorityGradientGreenToYellow.Evaluate((priority - l) / ((h - l) / 2f));
                        else //find gradient for second half
                            Gizmos.color = priorityGradientYellowToRed.Evaluate((priority - m) / ((h - l) / 2f));
                        Gizmos.DrawWireCube(rendererBounds.center, rendererBounds.size);
                    }
                    
                }

            }

        }

    }
}