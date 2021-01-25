using System.Collections;
using UnityEngine;

namespace Paperticket {
    public class BasicAnimController : BaseAnimController {

        [Header("CONTROLS")]
        [Space(10)]
        [SerializeField] int startingPose = 0;
        [Space(10)]
        [SerializeField] [Min(0)] float startDelay = 1;
        [SerializeField] bool randomiseDelay = true;

        public override void OnEnable() {
            if (startDelay <= 0) SetAnimation(startingPose);
            else StartCoroutine(SetAfterDelay());
        }

        IEnumerator SetAfterDelay() {

            if (randomiseDelay) {
                yield return new WaitForSeconds(Random.value * startDelay);
            } else {
                yield return new WaitForSeconds(startDelay);
            }

            SetAnimation(startingPose);
        }

        public void PlayAnimationOnce( int animationIndex ) {

            SetAnimation(animationIndex);
            if (backToStartPoseCo != null) StopCoroutine(backToStartPoseCo);
            backToStartPoseCo = StartCoroutine(SetBackToStartingPose(0.01f));

        }


        public void PlayAnimationOnce( int animationIndex, float waitTime ) {

            SetAnimation(animationIndex);
            if (backToStartPoseCo != null) StopCoroutine(backToStartPoseCo);
            backToStartPoseCo = StartCoroutine(SetBackToStartingPose(waitTime));

        }

        Coroutine backToStartPoseCo;
        IEnumerator SetBackToStartingPose(float waitTime) {
            yield return new WaitForSeconds(waitTime);
            SetAnimation(startingPose);
            backToStartPoseCo = null;
        }

    }

}