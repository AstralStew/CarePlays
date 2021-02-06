using Paperticket;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class IN01ModuleSelection : MonoBehaviour {

    [Header("REFERENCES")]
    [SerializeField] Transform buttonSelection = null;
    
    [Header("CONTROLS")]
    [Space(10)]
    [SerializeField] bool debugging = false;

    [Header("EVENTS")]
    [Space(20)]
    public UnityEvent2 HonestyComplete = null;
    public UnityEvent2 ChoiceComplete = null;
    public UnityEvent2 CulturalComplete = null;
    public UnityEvent2 PrivacyComplete = null;
    public UnityEvent2 ReportComplete = null;

    bool[] availableModules = null;

    void OnEnable() {

        if (buttonSelection == null) {
            Debug.LogError("[IN01ModuleSelection] ERROR -> No ButtonSelection transform set! Cannot autoselect, disabling.");
            gameObject.SetActive(false);
            return;
        }

        availableModules = new bool[] { true,true,true,true,true };


        if (CareplaysManager.instance.IN01HonestyComplete) {
            if (HonestyComplete != null) HonestyComplete.Invoke();
            availableModules[0] = false;
            if (debugging) Debug.Log("[IN01ModuleSelection] First module (Honesty) completed, disabling button...");
        }

        if (CareplaysManager.instance.IN01ReportComplete) {
            if (ReportComplete != null) ReportComplete.Invoke();
            availableModules[1] = false;
            if (debugging) Debug.Log("[IN01ModuleSelection] First module (Report) completed, disabling button...");
        }

        if (CareplaysManager.instance.IN01ChoiceComplete) {
            if (ChoiceComplete != null) ChoiceComplete.Invoke();
            availableModules[2] = false;
            if (debugging) Debug.Log("[IN01ModuleSelection] First module (Choice) completed, disabling button...");
        }

        if (CareplaysManager.instance.IN01CulturalComplete) {
            if (CulturalComplete != null) CulturalComplete.Invoke();
            availableModules[3] = false;
            if (debugging) Debug.Log("[IN01ModuleSelection] First module (Cultural) completed, disabling button...");
        }

        if (CareplaysManager.instance.IN01PrivacyComplete) {
            if (PrivacyComplete != null) PrivacyComplete.Invoke();
            availableModules[4] = false;
            if (debugging) Debug.Log("[IN01ModuleSelection] First module (Privacy) completed, disabling button...");
        }

    }

    public void AutoSelectNextModule() {

        if (debugging) Debug.Log("[IN01ModuleSelection] No module was selected by player, auto-selecting the next module:");

        for (int i = 0; i < availableModules.Length; i++) {

            if (availableModules[i]) {
                if (buttonSelection.GetChild(i) == null) {
                    Debug.LogError("[IN01ModuleSelection] ERROR -> No child found at index '"+i+"'! Cannot autoselect, disabling.");
                    gameObject.SetActive(false);
                    return;
                }
                if (buttonSelection.GetChild(i).GetComponent(typeof(LockableEvent)) == null) {
                    Debug.LogError("[IN01ModuleSelection] ERROR -> No LockableEvent component found on '"+buttonSelection.GetChild(i).name+"'! Cannot autoselect, disabling.");
                    gameObject.SetActive(false);
                    return;
                }
                if (debugging) Debug.Log("[IN01ModuleSelection] Child"+i+" picked ('" + buttonSelection.GetChild(i).name + "'), autoselecting module...");
                
                buttonSelection.GetChild(i).GetComponent<LockableEvent>().SendEvent(false);
                return;
            }

            Debug.LogError("[IN01ModuleSelection] ERROR -> Could not find any module that hasn't been completed?! Something has gone terribly wrong...");
        }


    }

}
