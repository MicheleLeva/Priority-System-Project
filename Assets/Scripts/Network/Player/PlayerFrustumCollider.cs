using System;
using System.Collections;
using Network.Objects;
using Unity.Netcode;
using UnityEngine;

namespace Network.Player {
    /// <summary>
    /// Frustum collider component, linked to each Client's Avatar.
    /// It has the purpose to keep updated the number of Clients that are facing each object.
    /// </summary>
    public class PlayerFrustumCollider : NetworkBehaviour {
        private ulong _clientId;

        private void Start() {
            _clientId = GetComponentInParent<NetworkObject>().OwnerClientId;
        }

        private void OnTriggerEnter(Collider other) {
            UpdateFacing(other);
        }

        private void OnTriggerExit(Collider other) {
            StartCoroutine(DelayedChangeViewers(other));
        }

        private IEnumerator DelayedChangeViewers(Component other) {
            yield return new WaitForSeconds(1);
            UpdateFacing(other, false);
        }

        private void UpdateFacing(Component other, bool add = true) {
            if (other.TryGetComponent<NetObject>(out var obj)) {
                if (add) obj.facing.Add(_clientId);
                else obj.facing.Remove(_clientId);
            }
        }
    }
}