using Paperticket;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class IN01ModuleSelection : MonoBehaviour {

    public UnityEvent2 HonestyComplete;
    public UnityEvent2 ChoiceComplete;
    public UnityEvent2 CulturalComplete;
    public UnityEvent2 PrivacyComplete;
    public UnityEvent2 ReportComplete;



    void OnEnable() {
        
        if (CareplaysManager.instance.IN01HonestyComplete) {
            if (CulturalComplete != null) HonestyComplete.Invoke();
        }

        if (CareplaysManager.instance.IN01ChoiceComplete) {
            if (CulturalComplete != null) ChoiceComplete.Invoke();
        }

        if (CareplaysManager.instance.IN01CulturalComplete) {
            if (CulturalComplete != null) CulturalComplete.Invoke();
        }

        if (CareplaysManager.instance.IN01PrivacyComplete) {
            if (CulturalComplete != null) PrivacyComplete.Invoke();
        }

        if (CareplaysManager.instance.IN01ReportComplete) {
            if (CulturalComplete != null) ReportComplete.Invoke();
        }

    }

}
