using System.Collections.Generic;
using Serializers;
using UnityEngine;

namespace Network.Client {
    /// <summary>
    /// Cache on Client to store serialized objects already present in the scene.
    /// </summary>
    public class ClientCache : Dictionary<int, SObject> {
        private static ClientCache _instance;
        public static ClientCache Singleton => _instance ??= new ClientCache();
    }
    
}