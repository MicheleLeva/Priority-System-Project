using System;
using System.Collections;
using Network.Objects;
using Network.Player;
using Network.SpawnUpdater;
using Network.Sync;
using PathCreation;
using Unity.Netcode;
using UnityEngine;

namespace _Tests {
    public class ObjectMove : MonoBehaviour {
        public PathCreator pathCreator;
        public float speed = 1;
        public int waitTime = 5;
        public bool move;

        private float _distanceTravelled;
        private bool _moving;
        private bool _started;
        private Transform _objTransform;

        private IEnumerator StartMoving() {
            yield return new WaitForSeconds(waitTime);
            /*
            try {
                _objTransform = FindObjectOfType<SyncTransformClient>().transform;
                _objTransform.GetComponent<SyncTransformClient>().SetOwnership(true);
                _moving = true;
            }
            catch (Exception _) { }
            */

            _objTransform = FindObjectOfType<NetworkPlayerMap>().transform.Find("Head");
            _objTransform.GetComponent<SyncTransformClient>().SetOwnership(true);
            _moving = true;
            
        }

        private void Update() {
            if (!move) return;
            
            if (!_started
                && NetworkManager.Singleton != null
                && NetworkManager.Singleton.IsClient
                && NetworkManager.Singleton.IsConnectedClient
               ) {
                StartCoroutine(StartMoving());
                _started = true;
            }

            if (_moving) {
                _distanceTravelled += speed * Time.deltaTime;
                _objTransform.position = pathCreator.path.GetPointAtDistance(_distanceTravelled);
                Debug.Log("path creator point" + pathCreator.path.GetPointAtDistance(_distanceTravelled));
                _objTransform.rotation = pathCreator.path.GetRotationAtDistance(_distanceTravelled);
                _objTransform.localRotation =
                    Quaternion.Euler(_objTransform.localRotation.eulerAngles + new Vector3(0, 0, 90));
                if (_distanceTravelled > pathCreator.path.length * 2) {
                    Logger.Move = true;
                    _moving = false;
                }
            }
        }
    }
}