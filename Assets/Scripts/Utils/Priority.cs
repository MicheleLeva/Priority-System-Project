using System;
using UnityEngine;

namespace Utils {
    /// <summary>
    /// Class to calculate the priority of the objects in the scene.
    /// </summary>
    public static class Priority {
        // -------------------------------------------------+
        // ---------- PRIORITY CALC PROPERTIES -------------+
        // -------------------------------------------------+
        // Gaussian
        private const float Mu = 4; // mean
        private const float Sigma = 40; // standard deviation

        // Multiplication Factors
        private const int FacingFactor = -1000;
        private const int GaussFactor = 1;

        private const int LevelFactor = 1;
        // -------------------------------------------------+

        /// <summary>
        /// Calculate object's priority
        /// </summary>
        /// <param name="level">object priority level (0, 1, 2)</param>
        /// <param name="distance">distance from the client's player</param>
        /// <param name="facing">number of player facing the object</param>
        /// <returns>object priority</returns>
        public static int Calc(int level, double distance, int facing) {
            // var gauss = Gauss(distance, Mu, Sigma);
            // var gaussFactor = 1 / (GaussFactor * gauss);

            var priority = (int) (
                (distance) * 
                (level + 1)
                // (distance) * gaussFactor + // distance
                // (level) * LevelFactor + // priority level
                // (facing) * FacingFactor // players facing
            );

            //Debug.Log($"PRIORITY: D: {distance} \tLV: {level} \tP:{priority} ");

            return priority;
        }

        static double Gauss(double x, double mu, double sigma) {
            return Math.Exp(-((x - mu) * (x - mu)) / (2 * sigma * sigma)) / Math.Sqrt(2 * Math.PI * sigma * sigma);
        }
    }

    // old: (int) ((level + 1) * distance) / ((facing ? 1 : 2) * 10);
}