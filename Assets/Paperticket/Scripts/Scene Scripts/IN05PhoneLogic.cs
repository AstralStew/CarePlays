using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Paperticket {
    public class IN05PhoneLogic : MonoBehaviour {

        [Header("REFERENCES")]
        [SerializeField] Transform photoSprites;
        [SerializeField] SpriteRenderer[] indicatorKnobs;
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

        [Header("PHOTO CONTROLS")]
        [Space(10)]
        [SerializeField] Vector3 photosStartPosition;
        [SerializeField] Vector3 translationPerMove;
        [Space(10)]
        [SerializeField] float moveDuration;


        [Header("READ ONLY")]

        int currentPhotoIndex = 0; 

        
        // Start is called before the first frame update
        void Awake() {

        }



        public void SubmitAnswer (bool askPermission ) {

            if (askPermission == shouldAsk[currentPhotoIndex]) {
                //StartCoroutine(CorrectAnswer());

                if (onCorrect != null) onCorrect.Invoke();
            } else {
                //StartCoroutine(IncorrectAnswer());

                if (onIncorrect != null) onIncorrect.Invoke();
            }
        }

        //Coroutine answerCo;
        //IEnumerator CorrectAnswer() {

        //    answerCo = null;
        //    yield return null;
        //}

        //IEnumerator IncorrectAnswer() {

        //    //float t = 0;
        //    //Vector3 postButtonPos = PostButton.localPosition;
        //    //Vector3 askButtonPos = AskButton.localPosition;
            
        //    //while (t < buttonShakeCurve.keys[buttonShakeCurve.length - 1].time) {
        //    //    yield return null;
        //    //    PostButton.localPosition = postButtonPos + ( Vector3.right * buttonShakeCurve.Evaluate(t));
        //    //    AskButton.localPosition = askButtonPos + (Vector3.right * buttonShakeCurve.Evaluate(t));
        //    //    t += Time.deltaTime;                
        //    //}
        //    //PostButton.localPosition = postButtonPos;
        //    //AskButton.localPosition = askButtonPos;

           




        //    answerCo = null;
        //}

        public void MoveSelection() {
            // Cancel if out of photos
            if (currentPhotoIndex >= numberOfPhotos - 1) return;

            // Cancel if currently transitioning
            if (movingCo != null) return;

            // Start a new transition
            movingCo = StartCoroutine(movingSelection());

        }

        Coroutine movingCo;
        IEnumerator movingSelection() {

            float t = 0;
            Vector3 startPos = photoSprites.localPosition;
            Vector3 endPos = startPos + translationPerMove;

            // Change the colours of the indicator knobs
            StartCoroutine(PTUtilities.instance.FadeColorTo(indicatorKnobs[currentPhotoIndex], Color.white, moveDuration));
            StartCoroutine(PTUtilities.instance.FadeColorTo(indicatorKnobs[currentPhotoIndex + 1], Color.black, moveDuration));

            while (t < moveDuration) {
                yield return null;
                t += Time.deltaTime;

                photoSprites.localPosition = Vector3.Lerp(startPos, endPos, t / moveDuration);
            }
            photoSprites.localPosition = endPos;
            currentPhotoIndex += 1;

            movingCo = null;
        }







    }
}
