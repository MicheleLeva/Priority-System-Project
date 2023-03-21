using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Unity.Netcode;
using UnityEngine;
using Utils;

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
        /// which players are currently facing the object
        /// </summary>
        public HashSet<ulong> facing = new();
        /// <summary>
        /// client that owns this object
        /// </summary>
        public ulong owner;

        private void Start() {
            if (NetworkManager.Singleton.IsServer) {
                if (id == default) id = this.GetID();
            }
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
    }
}