using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR;
using Random = UnityEngine.Random;

namespace Network.Player {
    /// <summary>
    /// Map Player Animations with the corresponding shared avatar. 
    /// </summary>
    public class NetworkPlayerMap : NetworkBehaviour {
        private static readonly int Trigger = Animator.StringToHash("Trigger");
        private static readonly int Grip = Animator.StringToHash("Grip");

        // Hands
        /// <summary>
        /// Left hand in Player prefab.
        /// </summary>
        public GameObject netLHand;
        /// <summary>
        /// Right hand in Player prefab.
        /// </summary>
        public GameObject netRHand;
        /// <summary>
        /// Head in Player prefab.
        /// </summary>
        public GameObject netHead;
        
        /// <summary>
        /// Hide or not the hands on the owner's Client.
        /// </summary>
        public bool hideHands;

        private GameObject _lHand;
        private GameObject _rHand;
        private GameObject _head;

        // Animators
        /// <summary>
        /// Left Hand animator in Player prefab.
        /// </summary>
        public Animator netLAnim;
        /// <summary>
        /// Right Hand animator in Player prefab.
        /// </summary>
        public Animator netRAnim;

        private Animator _lAnim;
        private Animator _rAnim;

        private enum HandPos {
            L,
            R
        }

        private Dictionary<HandPos, Animator> _netHands = new();
        /// <summary>
        /// Tolerance for the Player animation updates
        /// </summary>
        public float updateTolerance = 0.01F;

        void Start() {
            _lHand = GameObject.Find("LeftHand Controller");
            _rHand = GameObject.Find("RightHand Controller");
            _head = GameObject.Find("Main Camera");
            _lAnim = _lHand.GetComponentInChildren<Animator>();
            _rAnim = _rHand.GetComponentInChildren<Animator>();

            // Hide my own network player
            if (IsOwner) {
                Debug.Log("My Player Spawned");

                var inputDevices = new List<InputDevice>();
                InputDevices.GetDevices(inputDevices);
                inputDevices.ForEach(d => Debug.Log($"Device: {d.name}"));
                if (inputDevices.Count > 0) {
                    var renderers = GetComponentsInChildren<Renderer>();
                    if (hideHands)
                        foreach (var m in renderers) {
                            m.enabled = false;
                        }
                }
            }


            _netHands[HandPos.L] = netLAnim;
            _netHands[HandPos.R] = netRAnim;
        }

        void Update() {
            // Update position: synced using `Network Transform`
            if (IsOwner) {
                MapPosition(_lHand.transform, netLHand.transform);
                MapPosition(_rHand.transform, netRHand.transform);
                MapPosition(_head.transform, netHead.transform);
            }

            // Previous Animation Params

            // Update animation: synced manually using `Server RPC`
            if (IsClient && _lAnim != null && _rAnim != null) {
                UpdateAnimationClient();
            }
        }

        void MapPosition(Transform from, Transform to) {
            to.position = from.position;
            to.rotation = from.rotation;
        }

        void UpdateAnimationClient() {
            var lTrigger = _lAnim.GetFloat(Trigger);
            var lGrip = _lAnim.GetFloat(Grip);
            var rTrigger = _rAnim.GetFloat(Trigger);
            var rGrip = _rAnim.GetFloat(Grip);

            if (Math.Abs(lTrigger - netLAnim.GetFloat(Trigger)) > updateTolerance) {
                UpdateAnimationServerRpc(HandPos.L, Trigger, lTrigger);
                _lAnim.SetFloat(Trigger, lTrigger);
            }

            if (Math.Abs(lGrip - netLAnim.GetFloat(Grip)) > updateTolerance) {
                UpdateAnimationServerRpc(HandPos.L, Grip, lGrip);
                _lAnim.SetFloat(Grip, lGrip);
            }

            if (Math.Abs(rTrigger - netRAnim.GetFloat(Trigger)) > updateTolerance) {
                UpdateAnimationServerRpc(HandPos.R, Trigger, rTrigger);
                _rAnim.SetFloat(Trigger, rTrigger);
            }

            if (Math.Abs(rGrip - netRAnim.GetFloat(Grip)) > updateTolerance) {
                UpdateAnimationServerRpc(HandPos.R, Grip, rGrip);
                _rAnim.SetFloat(Grip, rGrip);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void UpdateAnimationServerRpc(HandPos hand, int param, float val) {
            _netHands[hand].SetFloat(param, val);
            Debug.Log("Server Animation updated");
        }
    }
}