using System;
using System.Net;
using System.Text.RegularExpressions;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using TMPro;
using Network.Http;
using System.Linq;
using System.Net.Sockets;
using UnityEngine.SocialPlatforms;

namespace Network {
    /// <summary>
    /// Network initial GUI interface.
    /// </summary>
    public class NetworkUI : MonoBehaviour {

        public TMP_InputField IpAddressInputField;

        public ComponentClientHttp componentClientHttp;

        public Canvas ClientCanvas;
        public ConsoleToGUI consoleToGUI;

        private void Start() {
            IpAddressInputField.text = NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address;

            NetworkManager.Singleton.OnClientDisconnectCallback += clientId => {
                Debug.LogError($"CLIENT {clientId} DISCONNECTED!");
            };

            consoleToGUI.clientCanvas = ClientCanvas;
        }

        public void StartServer() {
            NetworkManager.Singleton.StartServer();
        }

        public void StartClient() {
            Debug.Log("Start client");
            
            //ConvertUnityTransportAddress(NetworkManager.Singleton.NetworkConfig.NetworkTransport as UnityTransport);

            Debug.Log("Deactivate canvas");
            gameObject.SetActive(false);
            GameObject.Find("Canvas Server").SetActive(false);

            //turn on canvas for Oculus console debugging
            //ClientCanvas.gameObject.SetActive(true);
            //consoleToGUI.switchGUI();

            Debug.Log("networkmanager start client");
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

        public void SetIP(string ip)
        {
            NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address = ip;
            componentClientHttp.address = ip;
            Debug.Log("Ip string set: " + ip);
        }

        public static string GetLocalIPAddress()
        {
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                return endPoint.Address.ToString();
            }
        }

        public void IpInputFieldAddChar(string value)
        {
            IpAddressInputField.text += value;
        }

        public void IpInputFieldBackSpace()
        {
            IpAddressInputField.text = IpAddressInputField.text.Substring(0, IpAddressInputField.text.Length - 1);
        }

    }
}