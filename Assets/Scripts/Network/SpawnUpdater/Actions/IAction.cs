using UnityEngine;

namespace Network.Objects {
    /// <summary>
    /// Abstract class that represents a possible action that Clients can operate on the objects.
    /// </summary>
    public abstract class IAction : MonoBehaviour {
        /// <summary>
        /// Activate the action.
        /// </summary>
        public abstract void Activate();
    }
}