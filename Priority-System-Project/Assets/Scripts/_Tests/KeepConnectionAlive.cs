using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

/// <summary>
/// Keeps alive the connection between the Server and the Clients, by making the Client moving when the RTT is frozen.
/// </summary>
public class KeepConnectionAlive : NetworkBehaviour {
    private UnityTransport _transport;
    private readonly Dictionary<ulong, ulong> _prevRtt = new();

    private void Start() {
        NetworkManager.Singleton.OnServerStarted += () => {
            _transport = (UnityTransport) NetworkManager.Singleton.NetworkConfig.NetworkTransport;
            StartCoroutine(DoCheck());
        };
    }

    /// <summary>
    /// Checks the current RTT and the previous one.
    /// </summary>
    private IEnumerator DoCheck() {
        while (true) {
            foreach (var id in NetworkManager.Singleton.ConnectedClientsIds) {
                var rtt = _transport.GetCurrentRtt(id);
                if (_prevRtt.ContainsKey(id) && rtt == _prevRtt[id]) {
                    AwakeClientRpc(id);
                    _prevRtt[id] = 0;
                }
                else
                    _prevRtt[id] = rtt;
            }

            yield return new WaitForSeconds(0.5F);
        }
    }

    /// <summary>
    /// Make the Client send an update message to the Server.
    /// </summary>
    /// <param name="id">client id</param>
    [ClientRpc]
    private void AwakeClientRpc(ulong id) {
        if (NetworkManager.Singleton.LocalClientId != id) return;
        StartCoroutine(MoveAvatar());
    }

    /// <summary>
    /// Move the Client's avatar and came back to the initial position.
    /// </summary>
    private IEnumerator MoveAvatar() {
        var player = NetworkManager.Singleton.LocalClient.PlayerObject;
        var t = player.transform;
        t.position += Vector3.right * 0.03F;
        yield return new WaitForSeconds(0.1F);
        t.position -= Vector3.right * 0.03F;
    }
}