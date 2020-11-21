using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Paperticket {
    public class VideoEvent : MonoBehaviour {

        [SerializeField] bool debugging = false;

        [Header("REFERENCES")]
        [Space(5)]
        [SerializeField] VideoController videoController = null;

        [Header("CONTROLS")]
        [Space(10)]
        [SerializeField] float videoTimeBeforeEvent = 0;
        [SerializeField] bool OneTimeUse = true;


        [Header("EVENT")]
        [Space(5)]
        [SerializeField] UnityEvent2 OnEventTriggered = null;




        // Start is called before the first frame update
        void OnEnable() {

            if (!videoController) {
                Debug.LogError("[VideoEvent] ERROR -> No VideoController defined! Disabling...");
                enabled = false;
            }           

        }

        void Update() {

            if (videoController.currentVideoTime >= videoTimeBeforeEvent) {

                // Trigger the event
                if (OnEventTriggered != null) {
                    if (debugging) Debug.Log("[VideoEvent] OnEventTriggered called");
                    OnEventTriggered.Invoke();
                }

                // Destroy this script if this is a one time use, otherwise disable it
                if (OneTimeUse) {
                    if (debugging) Debug.Log("[VideoEvent] One time use is enabled, disabling this script");
                    Destroy(this);
                } else {
                    enabled = false;
                }
            }

        }




    }


}