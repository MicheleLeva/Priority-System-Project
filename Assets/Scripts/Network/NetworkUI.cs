using System;
using System.Net;
using System.Text.RegularExpressions;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

namespace Network {
    /// <summary>
    /// Network initial GUI interface.
    /// </summary>
    public class NetworkUI : MonoBehaviour {
        private void Start() {
            NetworkManager.Singleton.OnClientDisconnectCallback += clientId => {
                Debug.LogError($"CLIENT {clientId} DISCONNECTED!");
            };
        }

        public void StartServer() {
            NetworkManager.Singleton.StartServer();
        }

        public void StartClient() {
            ConvertUnityTransportAddress(NetworkManager.Singleton.NetworkConfig.NetworkTransport as UnityTransport);
            gameObject.SetActive(false);
            GameObject.Find("Canvas Server").SetActive(false);
            NetworkManager.Singleton.StartClient();
        }

        /// <summary>
        /// Converts Unity Transport's url to address
        /// </summary>
        /// <param name="ut">Unity Transport</param>
        private static void ConvertUnityTransportAddress(UnityTransport ut) {
            var name = ut.ConnectionData.Address;
            ut.ConnectionData.Address = GetIP(name);
        }

        /// <summary>
        /// Convert string hostname to ip
        /// </summary>
        /// <param name="hostname">hostname</param>
        /// <returns>ip</returns>
        public static string GetIP(string hostname) {
            return Dns.GetHostAddresses(hostname)[0].ToString();
        }

    }
}