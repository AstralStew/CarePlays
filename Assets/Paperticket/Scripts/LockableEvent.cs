using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Paperticket {
    public class LockableEvent : MonoBehaviour {

        [SerializeField] bool debugging = false;

        [Header("CONTROLS")]
        [Space(10)]
        [SerializeField] [Min(0)] float timeBeforeEvent;

        [Header("LIVE VARIABLES")]
        [Space(10)]
        [SerializeField] bool locked;

        [Header("EVENT")]
        [Space(5)]
        [SerializeField] UnityEvent2 onEvent = null;


        public void ToggleLock(bool toggle) {
            locked = toggle;

            if (debugging) Debug.Log("[LockableEvent] Setting event status: " + (locked ? "locked" : "unlocked"));
        }

        public void SendEvent(bool lockEvent ) {
            if (locked || onEvent == null) return;

            if (timeBeforeEvent > 0) StartCoroutine(WaitForEvent());
            else onEvent.Invoke();

            if (lockEvent) locked = true;

            if (debugging) Debug.Log("[LockableEvent] Sending event! Event status: " + (locked ? "locked" : "unlocked"));
        }

        IEnumerator WaitForEvent() {
            yield return new WaitForSeconds(timeBeforeEvent);
            onEvent.Invoke();
        }
    }

}