using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

namespace Paperticket {
    [RequireComponent(typeof(ParentConstraint))]
    public class ConstrainToController : MonoBehaviour {

        ParentConstraint constraint;

        [SerializeField] Hand hand;
        
        [SerializeField] Vector3 positionOffset;
        [SerializeField] Vector3 rotationOffset;
        

        // Start is called before the first frame update
        void Start() {

            constraint = GetComponent<ParentConstraint>();

            ConstraintSource source = new ConstraintSource();
            if (hand == Hand.Left) source.sourceTransform = PTUtilities.instance.leftController.transform;
            else source.sourceTransform = PTUtilities.instance.rightController.transform;
            source.weight = 1;

            constraint.AddSource(source);
            constraint.SetTranslationOffset(0, positionOffset);
            constraint.SetRotationOffset(0, rotationOffset);

            constraint.constraintActive = true;

        }

    }

}