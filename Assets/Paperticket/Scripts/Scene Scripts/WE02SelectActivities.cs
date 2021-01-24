using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Paperticket {
    public class WE02SelectActivities : MonoBehaviour {

        [SerializeField] int activationThreshold;
        [SerializeField] UnityEvent2 activationEvent;

        int currentCount;

        public void AddToCounter() {
            currentCount += 1;

            if (currentCount >= activationThreshold) {

                if (activationEvent != null) activationEvent.Invoke();


            }
        }


    }

}