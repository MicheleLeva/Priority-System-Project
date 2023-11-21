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

        /// <summary>
        /// Calculate object's priority based on how much it covers on the client's screen and its distance.
        /// </summary>
        /// <param name="screenPresence">Percentage of screen occupied by the object</param>
        /// <param name="distance">Distance between the object and the player camera</param>
        /// <returns></returns>
        public static int CalcWithScreenPresence(double screenPresence, double distance, double distanceFromScreenCenterPerc)
        {
            //multiplication by 1000 is to distribute better the priorities with similar distance and screen presence
            int priority = (int)(distance * 1000f * (1f - screenPresence) * distanceFromScreenCenterPerc);

            Debug.Log($"Calculating priority: distance {distance}, screenPresence {screenPresence}, " +
                $"distanceFromScreenCenterPerc {distanceFromScreenCenterPerc} --> priority {priority}");

            return priority;
        }

        static double Gauss(double x, double mu, double sigma) {
            return Math.Exp(-((x - mu) * (x - mu)) / (2 * sigma * sigma)) / Math.Sqrt(2 * Math.PI * sigma * sigma);
        }
    }

    // old: (int) ((level + 1) * distance) / ((facing ? 1 : 2) * 10);
}