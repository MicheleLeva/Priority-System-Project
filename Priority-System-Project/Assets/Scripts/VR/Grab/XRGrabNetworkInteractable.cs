using System;
using Network.Objects;
using Network.SpawnUpdater;
using Network.Sync;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Utils;

namespace VR {
    /// <summary>
    /// Class to trigger the objects' position and rotation update to send through the network
    /// when the player is grabbing it.
    /// </summary>
    public class XRGrabNetworkInteractable : XRGrabInteractable {
        private BaseSyncClient[] _syncs;
        private SyncManager _syncManager;
        private int _objectId;

        /// <inheritdoc />
        /// Initialize base sync client, net object, sync manager.
        protected override void Awake() {
            base.Awake();
            _syncs = GetComponents<BaseSyncClient>();
            _objectId = GetComponent<NetObject>().id;
            _syncManager = FindObjectOfType<SyncManager>();
        }

        /// <inheritdoc />
        /// When a hand holds the object, ask the Server to set the ownership to this Client. 
        protected override void OnSelectEntered(SelectEnterEventArgs args) {
            foreach (var sync in _syncs) {
                sync.SetOwnership(true);
            }
        }

        /// <inheritdoc />
        /// When the hand leave the object, remove the ownership of the client. 
        protected override void OnSelectExited(SelectExitEventArgs args) {
            foreach (var sync in _syncs) {
                sync.SetOwnership(false);
            }
        }

        /// <inheritdoc />
        /// When select button is triggered on the object, send a signal to the Server to activate
        /// the object's behavior.
        protected override void OnActivated(ActivateEventArgs args) {
            _syncManager.SendSignalServerRpc(_objectId);
        }
    }
}