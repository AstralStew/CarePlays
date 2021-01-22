using System.Collections;
using UnityEngine;

namespace Paperticket {
    public class TaliaAnimController : BaseAnimController {

        public enum TaliaAnimations { T_Pose, Standing_Idle, Acknowledging, Head_Nod, Talking_1, Talking_2, Clap_Knows_Some_Things, Clap_Out,
                                        Stand_To_Sit, Sitting_Idle, Sit_To_Stand }

        public enum TaliaFaceAnimations { IdleBlink, GrinBlink, TalkingBlink, Smug }


        [Header("BODY CONTROLS")]
        [Space(10)]
        [SerializeField] TaliaAnimations startingPose = TaliaAnimations.Standing_Idle;
        [Space(5)]
        [SerializeField] [Min(0)] float startDelay = 0;
        [SerializeField] bool randomiseDelay = false;

        [Header("FACE CONTROLS")]
        [Space(10)]
        [SerializeField] Animator faceAnimator = null;
        [Space(5)]
        [SerializeField] TaliaFaceAnimations startingFace = TaliaFaceAnimations.IdleBlink;

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

        public void SetAnimation (TaliaAnimations taliaAnimations ) {

            if (backToStartPoseCo != null) StopCoroutine(backToStartPoseCo);

            SetAnimation((int)taliaAnimations);

        }


        public void PlayAnimationOnce (TaliaAnimations taliaAnimations, float waitTime ) {

            SetAnimation((int)taliaAnimations);
            if (backToStartPoseCo != null) StopCoroutine(backToStartPoseCo);
            backToStartPoseCo = StartCoroutine(SetBackToStartingPose(waitTime));

        }


        public void SetFaceAnimation( TaliaFaceAnimations taliaFaceAnimation ) {

            if (backToStartFaceCo != null) StopCoroutine(backToStartFaceCo);

            faceAnimator.SetInteger("animationIndex", (int)taliaFaceAnimation);
            currentFaceIndex = (int)taliaFaceAnimation;
        }

        public void PlayFaceAnimationOnce( TaliaFaceAnimations taliaFaceAnimation, float waitTime ) {

            SetFaceAnimation(taliaFaceAnimation);
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