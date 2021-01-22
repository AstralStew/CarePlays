using System.Collections;
using UnityEngine;

namespace Paperticket {
    public class RyanAnimController : BaseAnimController {

        public enum RyanAnimations { T_Pose, Standing_Idle, Posing_For_Photo, Holding_Fish_Pose, Idle_Weight_Shift, Thats_Too_Deadly, Stand_To_Sit, Sitting_Idle_1,
                                        Sitting_Idle_2, Sitting_Clap, Sitting_Cheer, Sit_To_Stand}

        public enum RyanFaceAnimations { IdleBlink, Talking, Astonished, Grin }

        [Header("BODY CONTROLS")]
        [Space(10)]
        [SerializeField] RyanAnimations startingPose = RyanAnimations.Standing_Idle;
        [Space(10)]
        [SerializeField] [Min(0)] float startDelay = 0;
        [SerializeField] bool randomiseDelay = false;

        [Header("FACE CONTROLS")]
        [Space(10)]
        [SerializeField] Animator faceAnimator = null;
        [Space(5)]
        [SerializeField] RyanFaceAnimations startingFace = RyanFaceAnimations.IdleBlink;

        int currentFaceIndex = 0;

        public override void OnEnable() {
            if (startDelay <= 0) {
                animator.enabled = true;
                faceAnimator.enabled = true;
                SetAnimation((int)startingPose);
                SetFaceAnimation(startingFace);
            } else {
                StartCoroutine(SetAfterDelay());
                StartCoroutine(SetFaceAfterDelay());
            }
        }


        #region Public calls

        public void SetAnimation( RyanAnimations ryanAnimations ) {

            if (backToStartPoseCo != null) StopCoroutine(backToStartPoseCo);

            SetAnimation((int)ryanAnimations);

        }

        public void PlayAnimationOnce( RyanAnimations ryanAnimations, float waitTime ) {

            SetAnimation((int)ryanAnimations);
            if (backToStartPoseCo != null) StopCoroutine(backToStartPoseCo);
            backToStartPoseCo = StartCoroutine(SetBackToStartingPose(waitTime));

        }


        public void SetFaceAnimation( RyanFaceAnimations ryanFaceAnimation ) {

            if (backToStartFaceCo != null) StopCoroutine(backToStartFaceCo);

            faceAnimator.SetInteger("animationIndex", (int)ryanFaceAnimation);
            currentFaceIndex = (int)ryanFaceAnimation;
        }

        public void PlayFaceAnimationOnce( RyanFaceAnimations ryanFaceAnimation, float waitTime ) {

            SetFaceAnimation(ryanFaceAnimation);
            if (backToStartFaceCo != null) StopCoroutine(backToStartFaceCo);
            backToStartFaceCo = StartCoroutine(SetBackToStartingFace(waitTime));

        }


        #endregion
        


        #region Internal coroutines

        Coroutine backToStartPoseCo;
        IEnumerator SetBackToStartingPose( float waitTime ) {
            yield return new WaitForSeconds(waitTime);
            SetAnimation((int)startingPose);
            backToStartPoseCo = null;
        }

        Coroutine backToStartFaceCo;
        IEnumerator SetBackToStartingFace( float waitTime ) {

            yield return new WaitForSeconds(waitTime);
            SetFaceAnimation(startingFace);
            backToStartFaceCo = null;
        }




        IEnumerator SetAfterDelay() {

            if (randomiseDelay) {
                yield return new WaitForSeconds(Random.value * startDelay);
            } else {
                yield return new WaitForSeconds(startDelay);
            }

            animator.enabled = true;
            SetAnimation((int)startingPose);
        }

        IEnumerator SetFaceAfterDelay() {

            if (randomiseDelay) {
                yield return new WaitForSeconds(Random.value * startDelay);
            } else {
                yield return new WaitForSeconds(startDelay);
            }

            faceAnimator.enabled = true;
            SetFaceAnimation(startingFace);
        }


        #endregion

    }

}