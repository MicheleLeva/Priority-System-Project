using System;
using UnityEngine;

namespace Utils {
    /// <summary>
    /// Serializable <see cref="Vector4"/> wrapper.
    /// </summary>
    [Serializable]
    public class SVector4 {
        public float x;
        public float y;
        public float z;
        public float w;

        /// <summary>
        /// Cast to <see cref="Vector4"/>
        /// </summary>
        /// <param name="sv4">serializable vector4</param>
        /// <returns>vector4</returns>
        public static implicit operator Vector4(SVector4 sv4) =>
            new() {
                x = sv4.x,
                y = sv4.y,
                z = sv4.z,
                w = sv4.w
            };
        
        /// <summary>
        /// Cast from <see cref="Vector4"/>
        /// </summary>
        /// <param name="v4">vector4</param>
        /// <returns>serializable vector4</returns>
        public static implicit operator SVector4(Vector4 v4) =>
            new() {
                x = v4.x,
                y = v4.y,
                z = v4.z,
                w = v4.w
            };
    }
}