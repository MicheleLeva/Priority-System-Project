using Network.Objects;

namespace Utils {
    /// <summary>
    /// Class to generate object's IDs.
    /// </summary>
    public static class Ids {
        /// <summary>
        /// Get object identification code (used for gameObjects).
        /// </summary>
        /// <param name="netObject">object's NetObject component</param>
        /// <returns>object id</returns>
        public static int GetID(this NetObject netObject) {
            var parent = netObject.transform.parent;
            var name = netObject.name +
                       (parent != null
                           ? $">{parent}"
                           : "");
            return name.GetHashCode();
        }

        /// <summary>
        /// Get ID from string name (used for components).
        /// </summary>
        /// <param name="name">string name</param>
        /// <returns>component id</returns>
        public static int GetID(string name) {
            return name.GetHashCode();
        }
    }
}