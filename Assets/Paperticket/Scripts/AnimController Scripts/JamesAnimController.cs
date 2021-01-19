using System.Collections;
using UnityEngine;

namespace Paperticket {
    public class JamesAnimController : BaseAnimController {

        public enum JamesAnimations { T_Pose, Idle_1, Idle_2, Roll, Pose_For_Photo, Clapping, Cheer_1, Cheer_2 }

        public enum JamesFaceAnimations { IdleBlink, Talking, Celebration, Grin }

        [Header("BODY CONTROLS")]
        [Space(10)]
        [SerializeField] JamesAnimations startingPose = JamesAnimations.Idle_1;
        [Space(10)]
        [SerializeField] [Min(0)] float startDelay = 0;
        [SerializeField] bool randomiseDelay = false;

        [Header("FACE CONTROLS")]
        [Space(10)]
        [SerializeField] Animator faceAnimator = null;
        [Space(5)]
        [SerializeField] JamesFaceAnimations startingFace = JamesFaceAnimations.IdleBlink;

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
        public void SetAnimation( JamesAnimations jamesAnimations ) {

            SetAnimation((int)jamesAnimations);

        }

        public void PlayAnimationOnce( JamesAnimations jamesAnimations ) {

            SetAnimation((int)jamesAnimations);
            if (backToStartPoseCo != null) StopCoroutine(SetBackToStartingPose());
            backToStartPoseCo = StartCoroutine(SetBackToStartingPose());

        }


        public void SetFaceAnimation( JamesFaceAnimations jamesFaceAnimation ) {

            faceAnimator.SetInteger("animationIndex", (int)jamesFaceAnimation);
            currentFaceIndex = (int)jamesFaceAnimation;
        }

        public void PlayFaceAnimationOnce( JamesFaceAnimations jamesFaceAnimation ) {

            SetFaceAnimation(jamesFaceAnimation);
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