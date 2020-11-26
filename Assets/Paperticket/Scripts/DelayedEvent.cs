﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Paperticket {
    public class DelayedEvent : MonoBehaviour {

        [SerializeField] float timeBeforeEvent;
        [SerializeField] bool OneTimeUse = true;
        [SerializeField] bool debug;

        float timeToChange;

        [Header("Events")]

        [SerializeField] UnityEvent2 OnEventTriggered;

        // Start is called before the first frame update
        void OnEnable() {
            timeToChange = Time.time + timeBeforeEvent;
        }

        // Update is called once per frame
        void Update() {
            if (Time.time > timeToChange) {

                // Trigger the event
                if (OnEventTriggered != null) {
                    if (debug) Debug.Log("[TimedEvent] OnEventTriggered called");
                    OnEventTriggered.Invoke();
                }

                // Destroy this script if this is a one time use, otherwise reset
                if (OneTimeUse) {                   

                    if (GetComponents<Component>().Length > 2) {
                        if (debug) Debug.Log("[TimedEvent] One time use is enabled, disabling this script");
                        Destroy(this);
                    } else {
                        if (debug) Debug.Log("[TimedEvent] One time use is enabled, destroying this object");
                        Destroy(gameObject);
                    }
                    
                } else {
                    timeToChange = Time.time + timeBeforeEvent;
                }
            }
        }

    }

}