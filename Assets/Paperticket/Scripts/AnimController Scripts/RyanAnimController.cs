using System.Collections;
using UnityEngine;

namespace Paperticket {
    public class RyanAnimController : BaseAnimController {

        public enum RyanAnimations { T_Pose, Standing_Idle, Posing_For_Photo, Holding_Fish_Pose, Idle_Weight_Shift, Thats_Too_Deadly, Stand_To_Sit, Sitting_Idle_1,
                                        Sitting_Idle_2, Sitting_Clap, Sitting_Cheer, Sit_To_Stand}

        [Header("CONTROLS")]
        [Space(10)]
        [SerializeField] RyanAnimations startingPose = RyanAnimations.Standing_Idle;
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

        public void SetAnimation ( RyanAnimations ryanAnimations ) {

            SetAnimation((int)ryanAnimations);

        }

        public void PlayAnimationOnce ( RyanAnimations ryanAnimations ) {

            SetAnimation((int)ryanAnimations);
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