using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using Utils;
using static Utils.ShaderFeatures;

namespace Serializers {
    /// <summary>
    /// Serializable object's material.
    /// </summary>
    public class SMaterial : SComponent {
        [JsonProperty] private int _materialNo;
        [JsonProperty] private int _mode;
        [JsonProperty] private Dictionary<string, STexture> _textures = new();
        [JsonProperty] private SColor32 _color;
        [JsonProperty] private bool _emission;
        [JsonProperty] private SColor32 _emissionColor;
        [JsonProperty] private float _cutoff;

        /// <summary>
        /// Empty constructor required to deserialize the component.
        /// </summary>
        public SMaterial() { }

        /// <summary>
        /// Main constructor.
        /// </summary>
        /// <param name="material">object's material</param>
        /// <param name="materialNo">index of the material in the object's mesh renderer</param>
        public SMaterial(Material material, int materialNo) {
            Name = $"{material.name}_{materialNo}";
            Id = Ids.GetID(Name);

            _materialNo = materialNo;
            _mode = (int) material.GetFloat(Mode);
            _cutoff = material.GetFloat(Cutoff);
            _color = (Color32) material.color;
            if (material.IsKeywordEnabled(Emission)) {
                _emission = true;
                _emissionColor = (Color32) material.GetColor(EmissionColor);
            }

            foreach (var name in material.GetTexturePropertyNames()) {
                var texture = material.GetTexture(name);
                if (texture != null /*&& name=="_MainTex"*/) {
                    _textures.Add(name, new STexture((Texture2D) texture, _mode > 0));
                }
            }
        }

        /// <inheritdoc />
        /// Before attaching it, the component is built using the information of its fields.
        public override void AttachTo(SObject sObj) {
            // Build Material
            var mr = sObj.Obj.GetComponent<MeshRenderer>();
            var material = new Material(Shader.Find("Standard")) {
                name = Name
            };
            material.SetMode(_mode);
            material.SetFloat(Cutoff, _cutoff);
            material.color = (Color32) _color;
            if (_emission) {
                material.EnableKeyword(Emission);
                material.SetColor(EmissionColor, (Color32) _emissionColor);
            }

            foreach (var (textureName, texture) in _textures) {
                material.SetTexture(textureName, texture.GetTexture());
            }

            // Update Materials Array
            Material[] mats = mr.materials;
            Material[] newMats;

            // if need a bigger array
            if (_materialNo > mats.Length - 1) {
                // create new array
                newMats = new Material[_materialNo + 1];
                // copy old materials
                for (var i = 0; i < mats.Length; i++) {
                    newMats[i] = mats[i];
                }
            }
            else {
                newMats = mats;
            }

            newMats[_materialNo] = material;
            mr.materials = newMats;
        }

        /// <inheritdoc />
        public override byte[] Serialize() {
            var json = JsonConvert.SerializeObject(this, SerialSettings);
            return Encoding.ASCII.GetBytes(json);
        }
    }
}