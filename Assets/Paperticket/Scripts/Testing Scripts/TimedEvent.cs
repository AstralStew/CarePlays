using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TimedEvent : MonoBehaviour
{

    [SerializeField] float TimeBeforeSceneChange;
    [SerializeField] bool OneTimeUse = true;
    [SerializeField] bool debug;

    float timeToChange;

    [Header("Events")]

    [SerializeField] UnityEvent OnEventTriggered;
    [SerializeField] UnityEvent OnEvent2Triggered;

    // Start is called before the first frame update
    void OnEnable()
    {
        timeToChange = Time.time + TimeBeforeSceneChange;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > timeToChange) {
            
            // Trigger the event
            if (OnEventTriggered != null) {
                if (debug) Debug.Log("[TimedEvent] OnEventTriggered called");
                OnEventTriggered.Invoke();
            }

            // Destroy this script if this is a one time use, otherwise reset
            if (OneTimeUse) {
                if (debug) Debug.Log("[TimedEvent] One time use is enabled, disabling this script");
                Destroy(this);
            } else {
                timeToChange = Time.time + TimeBeforeSceneChange;
            }
        }
    }

}
