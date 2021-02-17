using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.XR;

namespace Paperticket {
    [RequireComponent(typeof(ParentConstraint))]
    public class ConstrainToController : MonoBehaviour {

        enum controllerType { LeftController, RightController, Head}

        ParentConstraint constraint;

        //[SerializeField] Hand hand;
        [SerializeField] controllerType controller = controllerType.LeftController;
        
        [SerializeField] Vector3 positionOffset;
        [SerializeField] Vector3 rotationOffset;
        [Space(15)]
        [SerializeField] bool AffectXPosition = true;
        [SerializeField] bool AffectYPosition = true;
        [SerializeField] bool AffectZPosition = true;
        [Space(15)]
        [SerializeField] bool AffectXRotation = true;
        [SerializeField] bool AffectYRotation = true;
        [SerializeField] bool AffectZRotation = true;


        // Start is called before the first frame update
        void Start() {

            constraint = GetComponent<ParentConstraint>();

            ConstraintSource source = new ConstraintSource();
            switch (controller) {
                case controllerType.LeftController:
                    source.sourceTransform = PTUtilities.instance.leftController.transform;
                    break;
                case controllerType.RightController:
                    source.sourceTransform = PTUtilities.instance.rightController.transform;
                    break;
                case controllerType.Head:
                    source.sourceTransform = PTUtilities.instance.headProxy;
                    break;
                default:
                    Debug.LogError("[ConstrainToController] ERROR -> Bad ControllerType passed as constraint transform! Cancelling");
                    return;
            }
        

            source.weight = 1;

            constraint.AddSource(source);
            constraint.SetTranslationOffset(0, positionOffset);
            constraint.SetRotationOffset(0, rotationOffset);
            
            // Check which translation axis to include
            if (AffectXPosition) {
                if (AffectYPosition) {
                    if (AffectZPosition) constraint.translationAxis = Axis.X | Axis.Y | Axis.Z;
                    else constraint.translationAxis = Axis.X | Axis.Y;                         
                } else if (AffectZPosition) constraint.translationAxis = Axis.X | Axis.Z;
                else constraint.translationAxis = Axis.X;
            } else if (AffectYPosition) {
                if (AffectZPosition) constraint.translationAxis = Axis.Y | Axis.Z;
                else constraint.translationAxis = Axis.Y;
            } else if (AffectZPosition) constraint.translationAxis = Axis.Z;
            else constraint.translationAxis = Axis.None;

            // Check which rotation axis to include
            if (AffectXRotation) {
                if (AffectYRotation) {
                    if (AffectZRotation) constraint.rotationAxis = Axis.X | Axis.Y | Axis.Z;
                    else constraint.rotationAxis = Axis.X | Axis.Y;
                } else if (AffectZRotation) constraint.rotationAxis = Axis.X | Axis.Z;
                else constraint.rotationAxis = Axis.X;
            } else if (AffectYRotation) {
                if (AffectZRotation) constraint.rotationAxis = Axis.Y | Axis.Z;
                else constraint.rotationAxis = Axis.Y;
            } else if (AffectZRotation) constraint.rotationAxis = Axis.Z;
            else constraint.rotationAxis = Axis.None;


            constraint.constraintActive = true;


        }

        void OnEnable() {
            OVRManager.InputFocusLost += QuestFocusLost;
            OVRManager.InputFocusAcquired += QuestFocusAcquired;
        }

        void OnDisable() {
            OVRManager.InputFocusLost -= QuestFocusLost;
            OVRManager.InputFocusAcquired -= QuestFocusAcquired;
        }


        void QuestFocusLost() {
            if (controller != controllerType.Head) {
                constraint.constraintActive = false;
            }
        }

        void QuestFocusAcquired() {
            constraint.constraintActive = true;
        }

    }

}