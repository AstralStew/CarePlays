using System.Collections;
using UnityEngine;

namespace Paperticket {
    public class MalcolmAnimController : BaseAnimController {

        public enum MalcolmAnimations { T_Pose, StandingIdle, WeightShift, AnnoyedHeadShake, HardNodYes, HeadShakeNo, 
                                            ThoughtfulHeadShakeNo, ThoughtfulNodYes, SittingIdle1, SittingIdle2, SittingClapping }

        [Header("CONTROLS")]
        [Space(10)]
        [SerializeField] MalcolmAnimations startingPose = MalcolmAnimations.StandingIdle;
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

        public void SetAnimation ( MalcolmAnimations malcolmAnimations ) {

            SetAnimation((int)malcolmAnimations);

        }

        public void PlayAnimationOnce ( MalcolmAnimations malcolmAnimations ) {

            SetAnimation((int)malcolmAnimations);
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