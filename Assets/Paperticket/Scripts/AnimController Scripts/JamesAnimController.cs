using System.Collections;
using UnityEngine;

namespace Paperticket {
    public class JamesAnimController : BaseAnimController {

        public enum JamesAnimations { T_Pose, Idle_1, Idle_2, Roll, Pose_For_Photo, Clapping, Cheer_1, Cheer_2 }

        [Header("CONTROLS")]
        [Space(10)]
        [SerializeField] JamesAnimations startingPose = JamesAnimations.Idle_1;
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

        public void SetAnimation ( JamesAnimations jamesAnimations ) {

            SetAnimation((int)jamesAnimations);

        }

        public void PlayAnimationOnce ( JamesAnimations jamesAnimations ) {

            SetAnimation((int)jamesAnimations);
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