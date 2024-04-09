using System;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Serializers {
    /// <summary>
    /// Serializable material's texture.
    /// </summary>
    [Serializable]
    public class STexture {
        [JsonProperty] private byte[] _textureData;
        [JsonProperty] private TextureFormat _format;
        [JsonProperty] private int _width;
        [JsonProperty] private int _height;

        /// <summary>
        /// Empty constructor required to deserialize the component.
        /// </summary>
        public STexture() { }
        
        /// <summary>
        /// Main constructor. If the texture uses the alpha channel, the image is serialized as PNG.
        /// </summary>
        /// <param name="texture">material's texture</param>
        /// <param name="alpha">if the texture uses the alpha channel or not</param>
        public STexture(Texture2D texture, bool alpha = false) {
            _format = texture.format;
            _textureData = alpha
                ? texture.EncodeToPNG()
                : texture.EncodeToJPG();
            _width = texture.width;
            _height = texture.height;
        }

        /// <summary>
        /// Build the texture from the serialized fields.
        /// </summary>
        /// <returns>material's texture</returns>
        public Texture2D GetTexture() {
            var texture = new Texture2D(_width, _height, _format, false);
            texture.LoadImage(_textureData);
            texture.Apply();

            return texture;
        }
    }
}