using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ResetParentToOrigin : Editor {

    [MenuItem("Paperticket/Reset Selected Transform/Reset Position")]
    public static void ResetPosition() {
        ResetToOrigin(true, false, false);
    }

    [MenuItem("Paperticket/Reset Selected Transform/Reset Rotation")]
    public static void ResetRotation() {
        ResetToOrigin(false, true, false);
    }

    [MenuItem("Paperticket/Reset Selected Transform/Reset Scale")]
    public static void ResetScale() {
        ResetToOrigin(false, false, true);
    }

    [MenuItem("Paperticket/Reset Selected Transform/Reset All")]
    public static void ResetAll() {
        ResetToOrigin(true, true, true);
    }


    public static void ResetToOrigin (bool resetPosition, bool resetRotation, bool resetScale) {

        Transform activeTransform = Selection.activeTransform;
        Transform tempParent = new GameObject("[TempParent]").transform;

        Undo.RegisterFullObjectHierarchyUndo(activeTransform, "[ResetParentToOrigin] Save original state of hierarchy");

        List<Transform> children = new List<Transform>(); 
       
        for (int i = 0; i < activeTransform.childCount; i++) {
            children.Add(activeTransform.GetChild(i));
        }

        foreach (Transform child in children) {
            //child.SetParent(tempParent, true);
            Undo.SetTransformParent(child, tempParent, "[ResetParentToOrigin] Change childs parent to temp");
        }


        //Debug.Log("number of children 2 = " + activeTransform.childCount);
        
        if (resetPosition) activeTransform.position = Vector3.zero;
        if (resetRotation) activeTransform.rotation = Quaternion.identity;
        if (resetScale) activeTransform.localScale = Vector3.one;

        foreach (Transform child in children) {
            Undo.SetTransformParent(child, activeTransform, "[ResetParentToOrigin] Change childs parent to original");
            //child.SetParent(activeTransform, true);
        }

        //Debug.Log("number of children 3 = " + activeTransform.childCount);

        DestroyImmediate(tempParent.gameObject);


        //for (int i = 0; i < activeTransform.childCount; i++) {
        //    activeTransform.GetChild(0).SetParent(tempParent, true);
        //}

        //activeTransform.position = Vector3.zero;

        //for (int i = 0; i < tempParent.childCount; i++) {
        //    tempParent.GetChild(0).SetParent(activeTransform, true);
        //}

        //foreach (Transform child in tempParent) {
        //    child.parent = activeTransform;
        //}
        

    }



}
