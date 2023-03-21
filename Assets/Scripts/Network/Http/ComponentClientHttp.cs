using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Serializers;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

namespace Network.Http {
    /// <summary>
    /// Wrapper class for HTTP requests from Client to Server.
    /// </summary>
    public class ComponentClientHttp : MonoBehaviour {
        /// <summary>
        /// IP address of the Server.
        /// </summary>
        public string address = "127.0.0.1";
        /// <summary>
        /// Port of the Server.
        /// </summary>
        public int port = 9001;

        private Thread _clientThread;
        private HttpClient _httpClient;

        private void Start() {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.ConnectionClose = false;
        }

        /// <summary>
        /// Make a request for a component to the Server
        /// </summary>
        /// <param name="componentId">component id</param>
        /// <returns>async task containing the obtained component</returns>
        public async Task<SComponent> GetComponentFromHttpServer(int componentId) {
            
            // Debug.Log($"[HTTP-Client] Asking {componentId}");
            var response = await _httpClient.PostAsync(
                requestUri: $"http://{NetworkUI.GetIP(address)}:{port}/component",
                content: new StringContent(componentId.ToString())
            );
            
            response.EnsureSuccessStatusCode();
            // Debug.Log($"[HTTP-Client] Received response: {response.StatusCode}");
            var bytes = await response.Content.ReadAsByteArrayAsync();
            
            var comp = SComponent.Deserialize(bytes);
            // Debug.Log($"[HTTP-Client] Received component: {comp.Name}({comp.Id}) [{bytes.Length} B]");
            return comp;
        }

        private void OnDisable() {
            _httpClient.Dispose();
        }
    }
}