using System.Collections;
using UnityEngine;

namespace Paperticket {
    public class GabiAnimController : BaseAnimController {

        public enum GabiAnimations { T_Pose, StandingIdle, WeightShift, ThoughtfulNod, ThoughtfulShake, SittingIdle, 
                                        SittingClapping, SittingTalking }

        [Header("CONTROLS")]
        [Space(10)]
        [SerializeField] GabiAnimations startingPose = GabiAnimations.StandingIdle;
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

        public void SetAnimation ( GabiAnimations gabiAnimations ) {

            SetAnimation((int)gabiAnimations);

        }

        public void PlayAnimationOnce ( GabiAnimations gabiAnimations ) {

            SetAnimation((int)gabiAnimations);
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