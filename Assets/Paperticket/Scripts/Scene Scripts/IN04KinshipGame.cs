using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Paperticket
{
    public class IN04KinshipGame : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] KinshipPerson[] selectablePeople;
        [SerializeField] KinshipPerson[] lockedPeople;
        [SerializeField] KinshipChair[] allChairs;

        [Header("Controls")]
        [SerializeField] float timeToReset;

        [Header("Read Only")]
        public bool GameActive = false;
        [SerializeField] KinshipPerson currentPerson = null;
        [SerializeField] int peopleIndex = 0;
                

        // Start is called before the first frame update
        void Start() {
            peopleIndex = 0;
            currentPerson = selectablePeople[0];
            currentPerson.SelectPerson();
            GameActive = true;
        }

        public void SetPersonToChair( KinshipChair chair ) {
            if (!GameActive) return;

            // Move person to chair and disable it
            currentPerson.SeatPerson(chair.transform);                        
            chair.DisableChair();

            // Check if we are up to Malcolm yet
            if (currentPerson == selectablePeople[3]) {
                
                // End the game if the chair can seat Malcolm
                if (chair.CanSeatMalcolm) EndGame();

                // Reset the game if the chair cannot seat Malcolm
                else {
                    currentPerson.WrongChoice();
                    lockedPeople[0].WrongChoice();
                    StartCoroutine(WaitToReset());
                }

            // Otherwise, set the next person as active
            } else {
                peopleIndex += 1;
                currentPerson = selectablePeople[peopleIndex];
                currentPerson.SelectPerson();
            }

        }

        IEnumerator WaitToReset() {

            // Deactivate the game to avoid accidental selects
            GameActive = false;

            // Wait a few secs then reset
            yield return new WaitForSeconds(timeToReset);
            ResetGame();
        }
        void ResetGame() {
            
            // Reset all people
            foreach (KinshipPerson person in selectablePeople) { person.ResetPerson(); }
            foreach (KinshipPerson person in lockedPeople) { person.ResetPerson(); }
            
            // Reset all chairs
            foreach (KinshipChair chair in allChairs) {
                chair.ResetChair();
            }

            peopleIndex = 0;
            currentPerson = selectablePeople[0];
            currentPerson.SelectPerson();
            GameActive = true;
        }

        void EndGame() {

            // Turn everyone green in celebration
            foreach (KinshipPerson person in selectablePeople) { person.SelectPerson(); }
            foreach (KinshipPerson person in lockedPeople) { person.SelectPerson(); }

            GameActive = false;
        }



        
    }

}