using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

namespace Paperticket {
    public class CounterEvent : MonoBehaviour {

        [SerializeField] bool debugging = false;
        [Header("CONTROLS")]
        [Space(10)]
        [SerializeField] int eventThreshold = 1;
        [Space(10)]
        [SerializeField] float timeBeforeEvent = 0;
        [SerializeField] bool oneUse = true;
        [SerializeField] bool resetCounter = false;
        [Space(5)]
        [SerializeField] UnityEvent2 counterEvent = null;

        int currentCount = 0;
        bool finished = false;
        

        void Check() {
            if (finished) return;
            if (currentCount >= eventThreshold) {

                finished = true;

                if (counterEvent == null) return;

                if (timeBeforeEvent > 0) StartCoroutine(WaitForEvent());
                else SendEventAndFinish();
            }
        }

        void SendEventAndFinish() {
            counterEvent.Invoke();

            if (oneUse) {
                if (debugging) Debug.Log("[CounterEvent] One Use enabled, destroying this script");
                Destroy(this);
            } else if (resetCounter) {
                currentCount = 0;
            }
            finished = false;
        }

        IEnumerator WaitForEvent() {
            yield return new WaitForSeconds(timeBeforeEvent);
            SendEventAndFinish();
        }




        #region PUBLIC CALLS

        public void Increment() {
            currentCount += 1;
            Check();
        }

        public void Decrement() {
            currentCount -= 1;
            Check();
        }

        public void Add( int amount ) {
            currentCount += amount;
            Check();
        }
        public void Remove( int amount ) {
            currentCount -= amount;
            Check();
        }
        #endregion

    }
}