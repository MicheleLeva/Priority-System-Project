using System;
using UnityEngine;

namespace Utils {
    /// <summary>
    /// Serializable <see cref="Vector2"/> wrapper.
    /// </summary>
    [Serializable]
    public class SVector2 {
        public float x;
        public float y;

        /// <summary>
        /// Cast to <see cref="Vector2"/>
        /// </summary>
        /// <param name="sv2">serializable vector2</param>
        /// <returns>vector2</returns>
        public static implicit operator Vector2(SVector2 sv2) =>
            new() {
                x = sv2.x,
                y = sv2.y
            };

        /// <summary>
        /// Cast from <see cref="Vector2"/>
        /// </summary>
        /// <param name="v2">vector2</param>
        /// <returns>serializable vector2</returns>
        public static implicit operator SVector2(Vector2 v2) =>
            new() {
                x = v2.x,
                y = v2.y
            };
    }
}