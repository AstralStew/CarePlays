using System.Collections;
using UnityEngine;

namespace Paperticket {
    public class NellyAnimController : BaseAnimController {

        public enum NellyAnimations { TPose, SittingIdle, SittingClap1, SittingClap2, SittingTalkLeft, SittingTalkRight }

        [Header("CONTROLS")]
        [Space(10)]
        [SerializeField] NellyAnimations startingPose = NellyAnimations.SittingIdle;
        [Space(10)]
        [SerializeField] [Min(0)] float startDelay = 0;
        [SerializeField] bool randomiseDelay = false;

        public override void OnEnable() {
            if (startDelay <= 0) {
                animator.enabled = true;
                SetAnimation((int)startingPose);
            } else StartCoroutine(SetAfterDelay());
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

        public void SetAnimation ( NellyAnimations nellyAnimations ) {

            SetAnimation((int)nellyAnimations);

        }

        public void PlayAnimationOnce ( NellyAnimations nellyAnimations ) {

            SetAnimation((int)nellyAnimations);
            if (backToStartPoseCo != null) StopCoroutine(SetBackToStartingPose());
            backToStartPoseCo = StartCoroutine(SetBackToStartingPose());

        }

        Coroutine backToStartPoseCo;
        IEnumerator SetBackToStartingPose() {
            yield return new WaitForSeconds(0.01f);
            SetAnimation((int)startingPose);
            backToStartPoseCo = null;
        }
    }

}