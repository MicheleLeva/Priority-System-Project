using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using Serializers;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

namespace Network.Http {
    /// <summary>
    /// Wrapper class for HTTP listener thread on the Server.
    /// </summary>
    public class ComponentServerHttp : MonoBehaviour {
        /// <summary>
        /// HTTP listener port.
        /// </summary>
        public int port = 9001;

        private Thread _serverThread;
        private HttpListener _httpListener;

        private void Start() {
            Application.runInBackground = true;
        }

        // -------------------------------------- Cache handler
        private readonly Dictionary<int, SComponent> _components = new();

        /// <summary>
        /// Add component to the local dictionary (required to be later accessed by the thread).
        /// </summary>
        /// <param name="sComp"></param>
        public void AddComponentToServerCache(SComponent sComp) {
            var added = _components.TryAdd(sComp.Id, sComp);
            // Debug.Log("[HTTP-Server] " + (added
            //     ? $"Added {sComp.Name}"
            //     : $"Already in Cache ({sComp.Name})"));
        }

        // -------------------------------------- HTTP Server

        /// <summary>
        /// Launches HTTP listener thread.
        /// </summary>
        public void StartServer() {
            _serverThread = new Thread(ServerCode);
            _serverThread.Start();
        }

        private void ServerCode() {
            // Enable HTTP Listener
            _httpListener = new HttpListener();
            Debug.Log($"PORT: {port}");
            _httpListener.Prefixes.Add($"http://*:{port}/");
            _httpListener.Prefixes.Add($"http://*:{port}/component/");
            foreach (var prefix in _httpListener.Prefixes) {
                Debug.Log($"[HTTP-Server] Server running on [{prefix}]");
            }

            _httpListener.Start();
            var result = _httpListener.BeginGetContext(HttpCallback, _httpListener);
        }

        private void HttpCallback(IAsyncResult result) {
            if (_httpListener == null) return;

            var context = _httpListener.EndGetContext(result);
            _httpListener.BeginGetContext(HttpCallback, _httpListener);

            ProcessRequest(context);
        }

        private void ProcessRequest(HttpListenerContext context) {
            Debug.Log($"[HTTP-Server] Processing: {context.Request.RemoteEndPoint}");

            // Receive the request
            var b = new byte[1024];
            var k = context.Request.InputStream.Read(b);
            var str = new StringBuilder();
            for (var i = 0; i < k; i++) {
                // Read stream
                str.Append((char) b[i]);
            }

            var key = int.Parse(str.ToString());

            // Search the required component
            if (_components.ContainsKey(key)) {
                // Debug.Log($"{key}-1");
                var sComp = _components[key];
                // Debug.Log($"{key}-2");
                var sBytes = sComp.Serialize();
                // Debug.Log($"{key}-3");

                // Send the component
                context.Response.OutputStream.Write(sBytes);
                // Debug.Log($"[HTTP-Server] Sending: {sComp.Name} [{sBytes.Length} B]");

                context.Response.OutputStream.Close();
            }
            else Debug.LogWarning($"[HTTP-Server] Component not in Cache! {key}");
        }

        private void OnDisable() {
            _httpListener?.Stop();
            _serverThread?.Abort();
        }
    }
}