using Network.Player;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Network {
    /// <summary>
    /// Simulation preferences.
    /// </summary>
    public class Prefs : MonoBehaviour {
        [Tooltip("Time to wait between each object sending (sec)")]
        public float sendDelay;

        [Tooltip("Time to wait before detecting for new objects in zone (sec)")]
        public float zoneDetectionDelay;

        [Tooltip("Time to wait between each zone detection radius update (sec)")]
        public float radiusUpdateDelay;

        [Tooltip("Time to wait between transform updates (sec)")]
        public float updateDelay;

        public int[] zones;

        private static Prefs _instance;
        public static Prefs Singleton => _instance ??= FindObjectOfType<Prefs>();

        /// <summary>
        /// Use priority queue or not.
        /// </summary>
        [Header("Test Preferences (Server Side)")]
        public bool priorityQueue = true;

        /// <summary>
        /// Send objects only in the Area of Interest of the player
        /// </summary>
        public bool aoi = true;

        /// <summary>
        /// Priority enqueuing types:
        /// Circular Areas of Interest: objects are enqueued using 3 different circular AoIs, high priority object are loaded
        /// even in further areas
        /// Screen Presence: obly visible objects from the client side are enqueued with priority depending on how much space their
        /// bounding volumes occupy on screen
        /// </summary>
        public enum PriorityType
        {
            CircularAreasOfInterest,
            ScreenPresence
        };

        public PriorityType priorityType = PriorityType.CircularAreasOfInterest;

        public Dropdown priorityTypeDropdown;

        private void Start() {
            NetworkManager.Singleton.OnServerStarted += () => {
                // if (!multipleZones)
                var x = aoi ? zones[0] : 1000;

                if (!priorityQueue)
                    zones[2] = zones[1] = zones[0] = x;
            };
        }

        /// <summary>
        /// Textarea GUI input field.
        /// </summary>
        public TMP_InputField delayTextArea;
        /// <summary>
        /// Update sendDelay parameter from Server textarea in GUI.
        /// </summary>
        public void SetDelay() {
            if (float.TryParse(delayTextArea.text, out var n)) {
                sendDelay = n;
                delayTextArea.textComponent.color = Color.black;
            }
            else {
                delayTextArea.textComponent.color = Color.red;
            }
            
        }
        
        /// <summary>
        /// Change priority queue usage from GUI
        /// </summary>
        /// <param name="t">GUI value</param>
        public static void TogglePriorityQueues(bool t) => Singleton.priorityQueue = t;
        
        /// <summary>
        /// Change zones usage based from GUI
        /// </summary>
        /// <param name="t">GUI value</param>
        public static void ToggleAoI(bool t) => Singleton.aoi = t;

        /// <summary>
        /// Event callback for priorityType selection
        /// </summary>
        /// <param name="newValue"></param>
        public void PriorityTypeValueChanged(int newValue)
        {
            priorityType = (PriorityType)newValue;
        }
    }

}