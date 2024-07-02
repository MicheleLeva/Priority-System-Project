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
        // Multiplication Weights (their sum must be 1)
        public static double distancePercentageWeight = 0.33d;
        public static double screenPresencePercentageWeight = 0.34d;
        public static double distanceFromScreenCenterPercWeight = 0.33d;

        //in priority calculation this is the highest priority
        //the greater it is than the number of NetObjects in the scene, the lesser is the chance to have collisions
        // (two or more NetObjects with the same priority --> race condition when added to the priority queue)
        public static int highestPriority = 1000000;

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
        /// Calculate object's priority based on how much it covers on the client's screen and its distances.
        /// </summary>
        /// <param name="screenPresencePercentage">Percentage of screen occupied by the object. Higher the value, more of the screen is occupied.
        /// Higher value --> higher priority -> We subtract it from 1</param>
        /// /// <param name="distancePercentage">Distance from player to object expressed in percentage of distance from furthest object. 
        /// Lower value --> higher priority -> No manipulation required</param>
        /// <param name="distanceFromScreenCenterPerc">Distance of the object from center of screen expressed in percentage of distance from center of screen to a corner.
        /// Lower value --> higher priority --> No manipulation required</param>
        /// <returns></returns>
        public static int CalcWithScreenPresence(double screenPresencePercentage, double distancePercentage, double distanceFromScreenCenterPerc)
        {

            //multiplication by 100 is to distribute better the priorities with weights due to integer casting
            //int priority = (int)(100f * distancePercentage * 100f * (1f - screenPresencePercentage) * 100f * distanceFromScreenCenterPerc);
            
            int priority = Mathf.RoundToInt((float) (highestPriority * (distancePercentageWeight * distancePercentage + 
                screenPresencePercentageWeight * (1f - screenPresencePercentage) + distanceFromScreenCenterPercWeight * distanceFromScreenCenterPerc)));

            /*Debug.Log($"Calculating priority: distancePercentage {distancePercentage}, screenPresencePercentage {screenPresencePercentage}, " +
                $"distanceFromScreenCenterPerc {distanceFromScreenCenterPerc} --> priority {priority}");*/

            return priority;
        }

        public static int CalcWithDistance(double distancePercentage) 
        {
            return Mathf.RoundToInt((float)(highestPriority * distancePercentage));
        }

        public static void SetWeights(double w1, double w2, double w3)
        {
            distancePercentageWeight = w1;
            screenPresencePercentageWeight = w2;
            distanceFromScreenCenterPercWeight = w3;
        }
    }
}