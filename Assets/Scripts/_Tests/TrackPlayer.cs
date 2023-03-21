using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TrackPlayer : MonoBehaviour {
    private Transform _target;
    private Vector3 _offset;

    public Vector3 camPos;
    public Quaternion camRot;
    public int ortSize;
    public Camera cam;

    void Start() {
        // ----------------------- AUTO START CLIENT
        // NetworkManager.Singleton.StartClient();
        // -----------------------------------------

        NetworkManager.Singleton.OnServerStarted += () => {
            var offset = cam.transform.parent;
            offset.rotation = camRot;
            offset.localPosition = camPos;
            // cam.orthographic = true;
            // cam.orthographicSize = ortSize;
            
            NetworkManager.Singleton.OnClientConnectedCallback += client => {
                _target = NetworkManager.Singleton.ConnectedClients[client]
                    .PlayerObject
                    .GetComponentInChildren<MeshRenderer>()
                    .transform;
                StartCoroutine(Delay());
            };
        };
    }

    IEnumerator Delay() {
        yield return new WaitForSeconds(1);


        _offset = transform.position - _target.position;
    }

    void Update() {
        if (_target != null && _offset != default) {
            transform.position = _target.position + _offset;
        }
    }
}