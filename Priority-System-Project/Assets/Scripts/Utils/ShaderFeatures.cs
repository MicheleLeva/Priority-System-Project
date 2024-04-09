using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Utils {
    /// <summary>
    /// Class to enable specific shader feature according to the selected mode.
    /// </summary>
    public static class ShaderFeatures {
        /// <summary>
        /// Material shader mode.
        /// </summary>
        public static readonly int Mode = Shader.PropertyToID("_Mode");
        /// <summary>
        /// Material emission color.
        /// </summary>
        public static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
        /// <summary>
        /// Material cutoff mode.
        /// </summary>
        public static readonly int Cutoff = Shader.PropertyToID("_Cutoff");
        /// <summary>
        /// Material emission (enabled or disabled).
        /// </summary>
        public const string Emission = "_EMISSION";

        /// <summary>
        /// Set the material mode.
        /// </summary>
        /// <param name="material">object's material</param>
        /// <param name="mode">material's mode</param>
        public static void SetMode(this Material material, int mode) {
            switch (mode) {
                case 0: // opaque
                    material.SetInt("_SrcBlend", (int) UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int) UnityEngine.Rendering.BlendMode.Zero);
                    material.SetInt("_ZWrite", 1);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = -1;
                    break;
                case 1: // cutout
                    material.SetInt("_SrcBlend", (int) UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int) UnityEngine.Rendering.BlendMode.Zero);
                    material.SetInt("_ZWrite", 1);
                    material.EnableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = 2450;
                    break;
                case 2: // fade
                    material.SetInt("_SrcBlend", (int) UnityEngine.Rendering.BlendMode.SrcAlpha);
                    material.SetInt("_DstBlend", (int) UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_ZWrite", 0);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.EnableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = 3000;
                    break;
                case 3: // transparent
                    material.SetInt("_SrcBlend", (int) UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int) UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_ZWrite", 0);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = 3000;
                    break;
            }

            material.SetFloat("_Mode", mode);
        }
    }
}