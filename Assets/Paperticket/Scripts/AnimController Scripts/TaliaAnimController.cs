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

            SetAnimation((int)taliaAnimations);

        }


        public void PlayAnimationOnce (TaliaAnimations taliaAnimations ) {

            SetAnimation((int)taliaAnimations);
            if (backToStartPoseCo != null) StopCoroutine(SetBackToStartingPose());
            backToStartPoseCo = StartCoroutine(SetBackToStartingPose());

        }


        public void SetFaceAnimation( TaliaFaceAnimations taliaFaceAnimation ) {

            faceAnimator.SetInteger("animationIndex", (int)taliaFaceAnimation);
            currentFaceIndex = (int)taliaFaceAnimation;
        }

        public void PlayFaceAnimationOnce( TaliaFaceAnimations taliaFaceAnimation ) {

            SetFaceAnimation(taliaFaceAnimation);
            if (backToStartFaceCo != null) StopCoroutine(SetBackToStartingFace());
            backToStartFaceCo = StartCoroutine(SetBackToStartingFace());

        }

        #endregion


        #region Internal coroutines

        Coroutine backToStartPoseCo;
        IEnumerator SetBackToStartingPose() {
            yield return new WaitForSeconds(0.01f);
            SetAnimation((int)startingPose);
            backToStartPoseCo = null;
        }

        Coroutine backToStartFaceCo;
        IEnumerator SetBackToStartingFace() {

            yield return new WaitForSeconds(0.01f);
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