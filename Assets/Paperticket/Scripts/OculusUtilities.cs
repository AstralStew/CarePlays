using Oculus.Platform;
using Paperticket;
using System.Collections;
using UnityEngine;

namespace Paperticket {

    public class OculusUtilities : MonoBehaviour {

        public static OculusUtilities instance = null;

        [SerializeField] bool debugging = false;

        [Header("CONTROLS")]
        [Space(5)]
        [SerializeField] OVRPlugin.ColorSpace colorSpace = OVRPlugin.ColorSpace.Quest;

        void Awake() {
            // Create an instanced version of this script, or destroy it if one already exists
            if (instance == null) {
                instance = this;
            } else if (instance != this) {
                Destroy(gameObject);
            }


        }

        public void SetColorGamut() {

            //OVRPlugin.ColorSpace HmdColorSpace;
            //HmdColorSpace = OVRPlugin.GetHmdColorDesc();

            if (debugging) Debug.Log("[OculusUtilities] Setting color gamut to: " + colorSpace);

            OVRPlugin.SetClientColorDesc(colorSpace);
        }






        public IEnumerator DoingHaptics( float frequency, float amplitude, float duration, Hand hand ) {

            if (hand != Hand.Right) OVRInput.SetControllerVibration(frequency, amplitude, OVRInput.Controller.LTouch);
            else if (hand != Hand.Left) OVRInput.SetControllerVibration(frequency, amplitude, OVRInput.Controller.RTouch);

            yield return new WaitForSeconds(duration);

            if (hand != Hand.Right) OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.LTouch);
            else if (hand != Hand.Left) OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);
        }

    }

}