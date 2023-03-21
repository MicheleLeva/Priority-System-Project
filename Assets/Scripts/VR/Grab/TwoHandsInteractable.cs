using System;
using System.Net.Sockets;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.XR.Interaction.Toolkit;

namespace VR {
    /// <summary>
    /// Handles the interaction with the objects using two hands simultaneously.
    /// The Class allows to rotate the objects in a natural way.
    /// </summary>
    [CanSelectMultiple]
    public class TwoHandsInteractable : XRGrabNetworkInteractable {
        private int _handsCount;
        private Transform _hand1;
        private Transform _hand2;

        private Vector3 _positionOffset;

        /// <inheritdoc />
        /// When the class is initially launched, a pivot to the center is created.
        protected override void Awake() {
            base.Awake();
            forceGravityOnDetach = true;
            selectMode = InteractableSelectMode.Multiple;
            movementType = MovementType.Kinematic;

            var pivot = new GameObject("Pivot") {
                transform = {
                    localPosition = Vector3.zero,
                    localRotation = Quaternion.identity,
                    parent = transform
                }
            };
            attachTransform = pivot.transform;
            SetTracking(false);
        }
        
        /// <summary>
        /// Each frame, if the first hand holds the object keep the object attached to it; if the second hand is
        /// detected calculate the angle between the hands and update the pivot accordingly.
        /// </summary>
        private void Update() {
            switch (_handsCount) {
                case 1:
                    attachTransform.position = _hand1.position;
                    attachTransform.rotation = _hand1.rotation;
                    break;
                case 2:
                    // // Calculate angle hand1->hand2
                    var hand1Position = _hand1.position;
                    var hand2Position = _hand2.position;
                    var lookRotation = Quaternion.LookRotation(hand2Position - hand1Position);

                    // // Update pivot
                    attachTransform.position = hand1Position;
                    attachTransform.rotation = lookRotation;
                    break;
            }
        }

        /// <summary>
        /// When the first hand selects the object, the pivot is positioned on the hand,
        /// then the pivot is settled as parent of the object, to make it follow.
        /// When the second hand selects the object, the pivot is rotated using the angle from hand1 to hand2.
        /// </summary>
        /// <param name="args"></param>
        protected override void OnSelectEntered(SelectEnterEventArgs args) {
            base.OnSelectEntered(args);
            _handsCount = interactorsSelecting.Count;
            var handTransform = args.interactorObject.transform;

            switch (_handsCount) {
                case 1:
                    _hand1 = handTransform;
                    PivotToHand1();
                    SetParent(attachTransform, transform);
                    break;
                case 2:
                default:
                    _hand2 = handTransform;
                    // Rotate pivot
                    attachTransform.rotation = Quaternion.LookRotation(_hand2.position - _hand1.position);
                    SetParent(attachTransform, transform);
                    break;
            }
        }

        /// <summary>
        /// When both the hands leave the object, the hierarchy is restored, making the pivot child of the object.
        /// When only one hand leave the object, the pivot is settled to the remaining hand.
        /// </summary>
        /// <param name="args"></param>
        protected override void OnSelectExited(SelectExitEventArgs args) {
            _handsCount = interactorsSelecting.Count;

            switch (_handsCount) {
                case 0:
                    base.OnSelectExited(args);
                    SetParent(transform, attachTransform);
                    break;
                case 1:
                    SetParent(transform, attachTransform);
                    _hand1 = interactorsSelecting[0].transform;
                    PivotToHand1();
                    SetParent(attachTransform, transform);
                    break;
            }
        }

        private void PivotToHand1() {
            // Reset Pivot to Hand
            attachTransform.position = _hand1.position;
            attachTransform.rotation = _hand1.rotation;
        }

        private static void SetParent(Transform parent, Transform child) {
            parent.SetParent(null);
            child.SetParent(parent, true);
        }

        private void SetTracking(bool enable) {
            trackRotation = enable;
            trackPosition = enable;
        }
    }
}