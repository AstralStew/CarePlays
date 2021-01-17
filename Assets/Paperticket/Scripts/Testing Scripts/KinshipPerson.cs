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
        [SerializeField] Vector3 seatOffset;
        [Space(5)]
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
            if (OnSelected != null) OnSelected.Invoke();
        }

        public void SeatPerson (Transform chairTransform ) {
            transform.position = chairTransform.position;
            transform.rotation = chairTransform.rotation;
            transform.Translate(seatOffset, Space.Self);
            
            if (OnSeated != null) OnSeated.Invoke();
        }

        public void ResetPerson() {
            transform.position = startPos;
            transform.rotation = startRot;

            if (OnReset != null) OnReset.Invoke();
        }
        
        public void WrongChoice() {
            if (OnWrongChoice != null) OnWrongChoice.Invoke();
        }

        public void Finish() {
            if (OnFinish != null) OnFinish.Invoke();
        }


    }

}