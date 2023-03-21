using System.Collections.Generic;
using Network.Client;
using Serializers;
using UnityEngine;

namespace Network.Server {
    /// <summary>
    /// Cache of serialized objects on the Server.
    /// The cache is populated when the objects are sent to a Client, and used to sync objects' properties. 
    /// </summary>
    public class ServerCache : Dictionary<int, SObject> {
        private static ServerCache _instance;
        public static ServerCache Singleton => _instance ??= new ServerCache();
    }
}