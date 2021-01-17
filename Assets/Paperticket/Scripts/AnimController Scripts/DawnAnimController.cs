using System.Collections;
using UnityEngine;

namespace Paperticket {
    public class DawnAnimController : BaseAnimController {

        public enum DawnAnimations { T_Pose, StandingIdle, HeadNodYes, HeadShakeNo, SittingIdle, SittingClapping, SittingTalkLeft, SittingTalkRight }

        [Header("CONTROLS")]
        [Space(10)]
        [SerializeField] DawnAnimations startingPose = DawnAnimations.StandingIdle;
        [Space(10)]
        [SerializeField] [Min(0)] float startDelay = 0;
        [SerializeField] bool randomiseDelay = false;

        public override void OnEnable() {
            if (startDelay <= 0) SetAnimation((int)startingPose);
            else StartCoroutine(SetAfterDelay());
        }

        IEnumerator SetAfterDelay() {

            if (randomiseDelay) {
                yield return new WaitForSeconds(Random.value * startDelay);
            } else {
                yield return new WaitForSeconds(startDelay);
            }

            SetAnimation((int)startingPose);
        }

        public void SetAnimation ( DawnAnimations dawnAnimations ) {

            SetAnimation((int)dawnAnimations);

        }

        public void PlayAnimationOnce ( DawnAnimations dawnAnimations ) {

            SetAnimation((int)dawnAnimations);
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