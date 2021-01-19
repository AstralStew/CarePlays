using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Paperticket {
    public class KinshipPerson : MonoBehaviour {

        [Header("REFERENCES")]

        [SerializeField] IN04KinshipGame KinshipGame;

        [Header("CONTROLS")]
        [Space(10)]
        [SerializeField] Vector3 seatOffset = Vector3.zero;
        [Space(5)]
        [SerializeField] bool debugging = false;
        [Space(15)]
        [SerializeField] UnityEvent2 OnSelected;
        [SerializeField] UnityEvent2 OnSeated;
        [SerializeField] UnityEvent2 OnReset;
        [SerializeField] UnityEvent2 OnWrongChoice;
        [SerializeField] UnityEvent2 OnFinish;

        [Header("LIVE VARIABLES")]
        [Space(15)]
        public bool Active = false;
        Vector3 startPos = Vector3.zero;
        Quaternion startRot = Quaternion.identity;

        

        void Awake() {
            KinshipGame = KinshipGame ?? FindObjectOfType<IN04KinshipGame>();
            if (KinshipGame == null) {
                Debug.LogError("[KinshipPerson] ERROR -> Could not find KinshipGame component in scene!");
                enabled = false;
            }


            startPos = transform.position;
            startRot = transform.rotation;

        }

        public void SelectPerson() {
            if (debugging) Debug.Log("[KinshipPerson] Select Person called");

            if (OnSelected != null) OnSelected.Invoke();
        }

        public void SeatPerson (Transform chairTransform ) {
            if (debugging) Debug.Log("[KinshipPerson] Seat Person called");

            transform.position = chairTransform.position;
            transform.rotation = chairTransform.rotation;
            transform.Translate(seatOffset, Space.Self);
            
            if (OnSeated != null) OnSeated.Invoke();
        }

        public void ResetPerson() {
            if (debugging) Debug.Log("[KinshipPerson] Reset Person called");

            transform.position = startPos;
            transform.rotation = startRot;

            if (OnReset != null) OnReset.Invoke();
        }
        
        public void WrongChoice() {
            if (debugging) Debug.Log("[KinshipPerson] Wrong Choice called");

            if (OnWrongChoice != null) OnWrongChoice.Invoke();
        }

        public void Finish() {
            if (debugging) Debug.Log("[KinshipPerson] Finish called");

            if (OnFinish != null) OnFinish.Invoke();
        }


    }

}