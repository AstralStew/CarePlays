using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Paperticket;
using TMPro;

public class ObjectDebugInfo : MonoBehaviour {

    [SerializeField] TextMeshPro tmp = null;

    [Header("CONTROLS")]
    [Space(5)]
    [SerializeField] Transform target = null;

    [Header("LIVE VARIABLES")]
    [Space(5)]
    public bool Show = true;


    private void OnEnable() {
        if (target == null) {
            Debug.LogWarning("[ObjectDebugInfo] No target assigned! Disabling.");
            gameObject.SetActive(false);
        }
    }


    void Update() {
        if (Show == true) UpdateText();    
    }


    public void UpdateText() {


        tmp.text = "[DEBUG INFO: " + target.name + "]\n " +
                    "Position = " + target.position.ToString() + " Rotation = " + target.rotation.eulerAngles.ToString() + "\n " +
                    "Local Position = " + target.localPosition.ToString() + " Rotation = " + target.localRotation.eulerAngles.ToString() + "\n " +
                    "Parent = " + target.parent.ToString();


    }



}
