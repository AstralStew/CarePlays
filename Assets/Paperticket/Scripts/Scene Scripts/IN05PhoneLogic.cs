using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

namespace Paperticket {
    public class IN05PhoneLogic : MonoBehaviour {

        [Header("REFERENCES")]
        [SerializeField] Transform photoSprites;
        //[SerializeField] SpriteRenderer[] indicatorKnobs;
        [SerializeField] TextMeshPro photoNumberText;
        [SerializeField] Transform PostButton;
        [SerializeField] Transform AskButton;

        [Header("GAME CONTROLS")]

        [Space(10)]
        [SerializeField] int numberOfPhotos;
        [SerializeField] bool[] shouldAsk;
        [Space(10)]
        [SerializeField] AnimationCurve buttonShakeCurve;
        [Space(10)]
        [SerializeField] UnityEvent2 onCorrect;
        [SerializeField] UnityEvent2 onIncorrect;
        [SerializeField] UnityEvent2 onFinish;

        [Header("PHOTO CONTROLS")]
        [Space(10)]
        [SerializeField] Vector3 photosStartPosition;
        [SerializeField] Vector3 translationPerMove;
        [Space(10)]
        [SerializeField] float moveDuration;


        [Header("READ ONLY")]

        int currentPhotoIndex = 0;
        bool gameFinished = false;

        
        void Awake() {

        }



        public void SubmitAnswer (bool askPermission ) {
            if (gameFinished) return;

            if (askPermission == shouldAsk[currentPhotoIndex]) {
                //StartCoroutine(CorrectAnswer());

                if (onCorrect != null) onCorrect.Invoke();
            } else {
                //StartCoroutine(IncorrectAnswer());

                if (onIncorrect != null) onIncorrect.Invoke();
            }
        }


        public void MoveSelection() {
            if (gameFinished) return;

            // Check what photo we're up to
            if (currentPhotoIndex < numberOfPhotos - 1) {

                // Cancel if currently transitioning
                if (movingCo != null) return;

                // Start a new transition
                movingCo = StartCoroutine(movingSelection());
            
            // We're up to the last photo
            } else if (currentPhotoIndex == numberOfPhotos - 1) {

                // Send the final event
                if (onFinish != null) onFinish.Invoke();
                gameFinished = true;

            } else Debug.LogErrorFormat("[{0}] ERROR -> Attempted to go beyond the number of photos available! cancelling", this);

        }

        Coroutine movingCo;
        IEnumerator movingSelection() {

            float t = 0;
            Vector3 startPos = photoSprites.localPosition;
            Vector3 endPos = startPos + translationPerMove;

            // DEPRECATED Change the colours of the indicator knobs
            //StartCoroutine(PTUtilities.instance.FadeColorTo(indicatorKnobs[currentPhotoIndex], Color.white, moveDuration));
            //StartCoroutine(PTUtilities.instance.FadeColorTo(indicatorKnobs[currentPhotoIndex + 1], Color.black, moveDuration));


            while (t < moveDuration) {
                yield return null;
                t += Time.deltaTime;

                photoSprites.localPosition = Vector3.Lerp(startPos, endPos, t / moveDuration);
            }
            photoSprites.localPosition = endPos;
            currentPhotoIndex += 1;


            photoNumberText.text = (currentPhotoIndex + 1) + " of " + numberOfPhotos;

            movingCo = null;
        }



    }
}
