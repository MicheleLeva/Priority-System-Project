using Newtonsoft.Json;
using Unity.Netcode;

namespace Network.Sync {
    /// <summary>
    /// Serializable data class that represents a generic object's properties set.
    /// </summary>
    public class SyncData : INetworkSerializable {
        private string _data;

        private static readonly JsonSerializerSettings SerialSettings =
            new() {
                TypeNameHandling = TypeNameHandling.Objects,
                NullValueHandling = NullValueHandling.Ignore,
                // Formatting = Formatting.Indented
            };

        /// <summary>
        /// Object's data, automatically serialized and deserialized to json when accessed.
        /// </summary>
        public object Data {
            get => JsonConvert.DeserializeObject(_data, SerialSettings);
            set => _data = JsonConvert.SerializeObject(value, SerialSettings);
        }

        /// <summary>
        /// Get data as plain string.
        /// </summary>
        public string StringData => _data;

        /// <summary>
        /// Method required for the serialization with Unity NetCode, to be sent as RPC.
        /// </summary>
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
            serializer.SerializeValue(ref _data);
        }
    }
}