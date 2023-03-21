using System;
using System.Linq;
using UnityEngine;
using Utils;

namespace Network.Sync {
    /// <summary>
    /// Structure that contains color properties.
    /// </summary>
    [Serializable]
    public struct ColorData {
        /// <summary>
        /// index of the material of the color.
        /// </summary>
        public int colorIndex;
        /// <summary>
        /// Serializable RGB32 color.
        /// </summary>
        public SColor32 color;
    }

    /// <summary>
    /// Class to sync material's colors changes. 
    /// </summary>
    public class SyncColor : BaseSync {
        private Color32[] _oldColors;
        private Material[] _materials;

        /// <inheritdoc />
        /// Instantiate initial color and materials.
        protected override void BeforeStart() {
            _oldColors = GetComponent<MeshRenderer>()
                .materials
                .Select(m => (Color32) m.color)
                .ToArray();
            _materials = GetComponent<MeshRenderer>().materials;
        }

        private int _toUpdate;

        /// <inheritdoc />
        public override void UpdateSync() {
            SendSync(new SyncData {
                Data = new ColorData {
                    colorIndex = _toUpdate,
                    color = (Color32) _materials[_toUpdate].color
                }
            });
        }

        /// <inheritdoc />
        protected override bool UpdateConditions() {
            for (var i = 0; i < _materials.Length; i++) {
                if (_materials[i].color == _oldColors[i]) continue;
                _oldColors[i] = _materials[i].color;
                _toUpdate = i;
                return true;
            }

            return false;
        }
    }
}