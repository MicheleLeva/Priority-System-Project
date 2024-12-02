using System;
using System.IO;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Serializers {
    /// <summary>
    /// Serializable object's component (or asset).
    /// The component is serialized as JSON string, and then bytes before being sent to the Clients.
    /// </summary>
    [Serializable]
    public abstract class SComponent {
        /// <summary>
        /// JsonSerializer configurations.
        /// </summary>
        protected static readonly JsonSerializerSettings SerialSettings =
            new() {
                TypeNameHandling = TypeNameHandling.Objects,
                NullValueHandling = NullValueHandling.Ignore,
                // Formatting = Formatting.Indented
            };

        /// <summary>
        /// Object ID.
        /// </summary>
        [JsonProperty] public int Id { get; protected set; }
        /// <summary>
        /// Object name.
        /// </summary>
        [JsonProperty] public string Name { get; protected set; }

        /// <summary>
        /// Attach this component to a gameObject.
        /// </summary>
        /// <param name="gameObject">gameObject to which attach the component</param>
        public abstract void AttachTo(SObject gameObject);

        /// <summary>
        /// Serialize the component to JSON and convert it in bytes.
        /// </summary>
        /// <returns>byte array of the JSON-serialized component</returns>
        public abstract byte[] Serialize();

        /// <summary>
        /// Deserialize a byte array containing the serialized component.
        /// </summary>
        /// <param name="byteArray">byte array of the JSON-serialized component</param>
        /// <returns>serializable object's component</returns>
        public static SComponent Deserialize(byte[] byteArray) {
            if (byteArray == null) return null;
            var json = Encoding.ASCII.GetString(byteArray);
            // Debug.Log("[HTTP-Client]" + json);

            var comp = (SComponent) JsonConvert.DeserializeObject(json, SerialSettings);
            // Debug.Log($"[HTTP-Client] {comp}");

            return comp;
        }
        
        /// <summary>
        /// Deserialize a string with the component serialized in JSON format.
        /// </summary>
        /// <param name="json">string JSON-serialized component</param>
        /// <returns>serializable object's component</returns>
        public static SComponent Deserialize(string json) {
            // Debug.Log("[HTTP-Client]" + json);
            var comp = (SComponent) JsonConvert.DeserializeObject(json, SerialSettings);
            // Debug.Log($"[HTTP-Client] {comp}");

            return comp;
        }

        /// <summary>
        /// Get string of the JSON-formatted component.
        /// </summary>
        /// <returns>string containing the component serialized as JSON</returns>
        public override string ToString() {
            return JsonConvert.SerializeObject(this, SerialSettings);
        }

        /// <summary>
        /// Get length of the string in JSON format. 
        /// </summary>
        /// <returns>length of the JSON string</returns>
        public ulong GetSize() {
            return (ulong) Serialize().Length;
        }
    }
}