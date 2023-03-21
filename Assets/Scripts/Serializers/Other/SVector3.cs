using System;
using UnityEngine;

namespace Utils {
    /// <summary>
    /// Serializable <see cref="Vector3"/> wrapper.
    /// </summary>
    [Serializable]
    public class SVector3 {
        public float x;
        public float y;
        public float z;
        
        /// <summary>
        /// Cast to <see cref="Vector3"/>
        /// </summary>
        /// <param name="sv3">serializable vector3</param>
        /// <returns>vector3</returns>
        public static implicit operator Vector3(SVector3 sv3) =>
            new() {
                x = sv3.x,
                y = sv3.y,
                z = sv3.z
            };

        /// <summary>
        /// Cast from <see cref="Vector3"/>
        /// </summary>
        /// <param name="v3">vector3</param>
        /// <returns>serializable vector3</returns>
        public static implicit operator SVector3(Vector3 v3) =>
            new() {
                x = v3.x,
                y = v3.y,
                z = v3.z
            };
    }
}