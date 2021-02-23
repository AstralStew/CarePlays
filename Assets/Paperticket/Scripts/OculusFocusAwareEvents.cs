using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Paperticket;

public class OculusFocusAwareEvents : MonoBehaviour {

    [SerializeField] bool debugging = false;

    [Header("EVENTS")]
    [Space(5)] 
    [SerializeField] UnityEvent2 OnFocusLost = null;
    [Space(5)]
    [SerializeField] UnityEvent2 OnFocusAcquired = null;

    public bool HasInputFocus {
        get { return OVRManager.hasInputFocus; }
    }


    //[SerializeField] bool controllerBeamState = false;



    // Start is called before the first frame update
    void OnEnable() {
        OVRManager.InputFocusLost += InputFocusLost;
        OVRManager.InputFocusAcquired += InputFocusAcquired;
    }
    void OnDisable() {
        OVRManager.InputFocusLost -= InputFocusLost;
        OVRManager.InputFocusAcquired -= InputFocusAcquired;
    }


    public void InputFocusLost() {
        if (debugging) Debug.Log("[OculusFocusAwareEvents] Input focus lost.");

        // Save the controller beam state and turn it off
        //controllerBeamState = PTUtilities.instance.ControllerBeamActive;
        //PTUtilities.instance.ControllerBeamActive = false;

        // Send off the focus lost event
        if (OnFocusAcquired != null) OnFocusLost.Invoke();

    }

    public void InputFocusAcquired() {
        if (debugging) Debug.Log("[OculusFocusAwareEvents] Input focus acquired.");

        // Load the controller beam state
        //PTUtilities.instance.ControllerBeamActive = controllerBeamState;

        // Send off the focus acquired event
        if (OnFocusAcquired != null) OnFocusAcquired.Invoke();
    }

}
