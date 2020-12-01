using System.Collections;
using UnityEngine;

namespace Paperticket {
    public class TaliaAnimController : BaseAnimController {

        public enum TaliaAnimations { T_Pose, Standing_Idle, Acknowledging, Head_Nod, Hard_Head_Nod, Thoughtful_Head_Shake, Talking_1,
                                Talking_2, Knows_Some_Things, Waving, Weight_Shift, Stand_To_Sit, Sitting_Idle, Sitting_Talking,
                                Sitting_Clap_1, Sitting_Clap_2}

        [Header("CONTROLS")]
        [Space(10)]
        [SerializeField] TaliaAnimations startingPose = TaliaAnimations.Standing_Idle;
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

        public void SetAnimation (TaliaAnimations taliaAnimations ) {

            SetAnimation((int)taliaAnimations);

        }

        public void PlayAnimationOnce (TaliaAnimations taliaAnimations ) {

            SetAnimation((int)taliaAnimations);
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