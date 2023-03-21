using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Network.Player {
    /// <summary>
    /// On start, update material color of the Player's Object.
    /// </summary>
    public class ChangeMaterial : NetworkBehaviour {
        /// <summary>
        /// Initial material of the avatar.
        /// </summary>
        public Material baseMat;

        private void Start() {
            if (!IsClient || !IsOwner) return;

            // Random Color
            var rndColor = new Color(
                Random.Range(0F, 1F),
                Random.Range(0F, 1F),
                Random.Range(0F, 1F)
            );

            SetColorServerRpc(rndColor);
        }

        [ServerRpc]
        private void SetColorServerRpc(Color rndColor) {
            var newMat = Instantiate(baseMat);
            newMat.color = rndColor;

            var mrs = GetComponentsInChildren<Renderer>();
            foreach (var mr in mrs) {
                mr.material = newMat;
            }

            SetColorClientRpc(rndColor);

            NetworkManager.Singleton.OnClientConnectedCallback += _ =>
                SetColorClientRpc(rndColor);
        }

        [ClientRpc]
        private void SetColorClientRpc(Color rndColor) {
            var newMat = Instantiate(baseMat);
            newMat.color = rndColor;

            var mrs = GetComponentsInChildren<Renderer>();
            foreach (var mr in mrs) {
                mr.material = newMat;
            }
        }
    }
}