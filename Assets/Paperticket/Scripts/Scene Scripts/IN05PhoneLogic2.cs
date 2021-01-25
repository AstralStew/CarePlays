using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Paperticket {
    public class IN05PhoneLogic2 : MonoBehaviour {

        [Header("POST BUTTON")]
        [Space(5)]
        [SerializeField] UnityEvent2 PostEvent1 = null;
        [SerializeField] UnityEvent2 PostEvent2 = null;
        [SerializeField] UnityEvent2 PostEvent3 = null;
        [SerializeField] UnityEvent2 PostEvent4 = null;

        [Header("ASK FOR PERMISSION")]
        [Space(15)]
        [SerializeField] UnityEvent2 AskEvent1 = null;
        [SerializeField] UnityEvent2 AskEvent2 = null;
        [SerializeField] UnityEvent2 AskEvent3 = null;
        [SerializeField] UnityEvent2 AskEvent4 = null;


        int photoIndex = 0;


        public void AskForPermission() {

            if (photoIndex == 0 && AskEvent1 != null) AskEvent1.Invoke();
            else if(photoIndex == 1 && AskEvent2 != null) AskEvent2.Invoke();
            else if (photoIndex == 2 && AskEvent3 != null) AskEvent3.Invoke();
            else if (photoIndex == 3 && AskEvent4 != null) AskEvent4.Invoke();

        }

        public void PostPhoto() {

            if (photoIndex == 0 && PostEvent1 != null) PostEvent1.Invoke();
            else if (photoIndex == 1 && PostEvent2 != null) PostEvent2.Invoke();
            else if (photoIndex == 2 && PostEvent3 != null) PostEvent3.Invoke();
            else if (photoIndex == 3 && PostEvent4 != null) PostEvent4.Invoke();

        }

        public void NextPhoto() {
            photoIndex += 1;
        }



    }

}