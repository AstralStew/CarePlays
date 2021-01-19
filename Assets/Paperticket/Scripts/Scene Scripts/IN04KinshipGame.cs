using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Paperticket {
    public class IN04KinshipGame : MonoBehaviour {

        [Header("REFERENCES")]
        [SerializeField] KinshipPerson[] selectablePeople;
        [SerializeField] KinshipPerson[] lockedPeople;
        [SerializeField] KinshipChair[] allChairs;
        [SerializeField] Transform selectionMarker; 

        [Header("CONTROLS")]
        [Space(10)]
        [SerializeField] float markerHeight;
        [SerializeField] float timeToReset;
        [SerializeField] float timeBetweenCharacters;
        [SerializeField] UnityEvent2 OnReset;
        [SerializeField] UnityEvent2 OnFinish;
        [SerializeField] UnityEvent2 OnExtraChair;


        [Header("LIVE VARIABLES")]
        [Space(10)]
        public bool GameActive = false;
        [SerializeField] KinshipPerson currentPerson = null;
        [SerializeField] int peopleIndex = 0;

        
                

        // Start is called before the first frame update
        void Start() {
            peopleIndex = 0;
            currentPerson = selectablePeople[0];
        }

        public void StartGame() {
            currentPerson.SelectPerson();
            GameActive = true;
            TransformMarker();
        }

        public void SetPersonToChair( KinshipChair chair ) {
            if (!GameActive) return;

            // Move person to chair and disable it
            currentPerson.SeatPerson(chair.attachPoint);                        
            chair.DisableChair();
            selectionMarker.gameObject.SetActive(false);

            // Check if we are up to Malcolm yet
            if (currentPerson == selectablePeople[3]) {

                // End the game if the chair can seat Malcolm
                if (chair.CanSeatMalcolm) {

                    if (chair.IsExtraSeat && OnExtraChair != null) OnExtraChair.Invoke();

                    EndGame();
                }

                // Reset the game if the chair cannot seat Malcolm
                else {
                    foreach (KinshipPerson person in selectablePeople) person.WrongChoice();
                    foreach (KinshipPerson person in lockedPeople) person.WrongChoice();
                    StartCoroutine(WaitToReset());
                }

            // Otherwise, set the next person as active
            } else {
                StartCoroutine(WaitBetweenCharacters());
            }

        }

        IEnumerator WaitToReset() {

            // Deactivate the game to avoid accidental selects
            GameActive = false;

            // Wait a few secs then reset
            yield return new WaitForSeconds(timeToReset);

            peopleIndex = 0;
            currentPerson = selectablePeople[0];

            if (OnReset != null) OnReset.Invoke();
        }


        IEnumerator WaitBetweenCharacters() {

            // Wait a few secs then select next character
            yield return new WaitForSeconds(timeBetweenCharacters);

            // Set the next person as active
            peopleIndex += 1;
            currentPerson = selectablePeople[peopleIndex];
            currentPerson.SelectPerson();
            TransformMarker();


        }

        void EndGame() {
            
            GameActive = false;

            foreach (KinshipPerson person in selectablePeople) person.Finish();
            foreach (KinshipPerson person in lockedPeople) person.Finish();

            if (OnFinish != null) OnFinish.Invoke();

        }

        void TransformMarker() {

            if (GameActive) {
                selectionMarker.position = currentPerson.transform.position + (Vector3.up * markerHeight);
                selectionMarker.gameObject.SetActive(true);
            } else {
                selectionMarker.gameObject.SetActive(false);
            }

        }

        
    }

}