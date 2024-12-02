using System;
using UnityEngine;

namespace Utils {
    /// <summary>
    /// Serializable <see cref="Color32"/> wrapper. 
    /// </summary>
    [Serializable]
    public class SColor32 {
        public byte a;
        public byte r;
        public byte g;
        public byte b;

        /// <summary>
        /// Cast to <see cref="Color32"/>.
        /// </summary>
        /// <param name="sCol">serializable color</param>
        /// <returns>color as Color32</returns>
        public static implicit operator Color32 (SColor32 sCol) {
            return new Color32 {
                a = sCol.a,
                r = sCol.r,
                g = sCol.g,
                b = sCol.b
            };
        }

        /// <summary>
        /// Cast from <see cref="Color32"/>.
        /// </summary>
        /// <param name="col">color as Color32</param>
        /// <returns>serializable color</returns>
        public static implicit operator SColor32(Color32 col) {
            return new SColor32 {
                a = col.a,
                r = col.r,
                g = col.g,
                b = col.b
            };
        }
    }
}