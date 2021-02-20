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

            Debug.Log("[OculusUtilities] OVRManager.loadedXRDevice = " + OVRManager.loadedXRDevice);

        }

        public void SetColorGamut() {

            //OVRPlugin.ColorSpace HmdColorSpace;
            //HmdColorSpace = OVRPlugin.GetHmdColorDesc();

            if (debugging) Debug.Log("[OculusUtilities] Setting color gamut to: " + colorSpace);

            OVRPlugin.SetClientColorDesc(colorSpace);
        }





        OVRPlugin.HapticsState hapticState;
        OVRPlugin.HapticsDesc hapticDesc;
        public IEnumerator DoingHaptics( float frequency, float amplitude, float duration, Hand hand ) {

            if (hand != Hand.Right) {
                if (debugging) Debug.Log("[OculusUtilities] Starting haptics on 'LTouch' controller'");

                hapticState = OVRPlugin.GetControllerHapticsState((uint)OVRInput.Controller.LTouch);
                hapticDesc = OVRPlugin.GetControllerHapticsDesc((uint)OVRInput.Controller.LTouch);
                if (debugging) Debug.Log("[OculusUtilities] Haptics state/desc of 'LTouch' pre-call: \n " +
                                          "SamplesAvailable = " + hapticState.SamplesAvailable + "\n " +
                                          "SamplesQueud = " + hapticState.SamplesQueued + "\n " +
                                          "MaximumBufferSamplesCount = " + hapticDesc.MaximumBufferSamplesCount + "\n " +
                                          "MinimumBufferSamplesCount = " + hapticDesc.MinimumBufferSamplesCount + "\n " +
                                          "MinimumSafeSamplesQueued = " + hapticDesc.MinimumSafeSamplesQueued + "\n " +
                                          "OptimalBufferSamplesCount = " + hapticDesc.OptimalBufferSamplesCount + "\n " +
                                          "SampleRateHz = " + hapticDesc.SampleRateHz + "\n " +
                                          "SampleSizeInBytes = " + hapticDesc.SampleSizeInBytes);

                OVRInput.SetControllerVibration(frequency, amplitude, OVRInput.Controller.LTouch);
            }

            if (hand != Hand.Left) {
                if (debugging) Debug.Log("[OculusUtilities] Starting haptics on 'RTouch' controller'");

                hapticState = OVRPlugin.GetControllerHapticsState((uint)OVRInput.Controller.RTouch);
                hapticDesc = OVRPlugin.GetControllerHapticsDesc((uint)OVRInput.Controller.RTouch);
                if (debugging) Debug.Log("[OculusUtilities] Haptics state/desc of 'RTouch' pre-call: \n " +
                                          "SamplesAvailable = " + hapticState.SamplesAvailable + "\n " +
                                          "SamplesQueud = " + hapticState.SamplesQueued + "\n " +
                                          "MaximumBufferSamplesCount = " + hapticDesc.MaximumBufferSamplesCount + "\n " +
                                          "MinimumBufferSamplesCount = " + hapticDesc.MinimumBufferSamplesCount + "\n " +
                                          "MinimumSafeSamplesQueued = " + hapticDesc.MinimumSafeSamplesQueued + "\n " +
                                          "OptimalBufferSamplesCount = " + hapticDesc.OptimalBufferSamplesCount + "\n " +
                                          "SampleRateHz = " + hapticDesc.SampleRateHz + "\n " +
                                          "SampleSizeInBytes = " + hapticDesc.SampleSizeInBytes);

                OVRInput.SetControllerVibration(frequency, amplitude, OVRInput.Controller.RTouch);
            }


            yield return new WaitForSeconds(duration / 2);


            if (hand != Hand.Right) {

                hapticState = OVRPlugin.GetControllerHapticsState((uint)OVRInput.Controller.LTouch);
                hapticDesc = OVRPlugin.GetControllerHapticsDesc((uint)OVRInput.Controller.LTouch);
                if (debugging) Debug.Log("[OculusUtilities] Haptics state/desc of 'LTouch' during-call: \n " +
                                          "SamplesAvailable = " + hapticState.SamplesAvailable + "\n " +
                                          "SamplesQueud = " + hapticState.SamplesQueued + "\n " +
                                          "MaximumBufferSamplesCount = " + hapticDesc.MaximumBufferSamplesCount + "\n " +
                                          "MinimumBufferSamplesCount = " + hapticDesc.MinimumBufferSamplesCount + "\n " +
                                          "MinimumSafeSamplesQueued = " + hapticDesc.MinimumSafeSamplesQueued + "\n " +
                                          "OptimalBufferSamplesCount = " + hapticDesc.OptimalBufferSamplesCount + "\n " +
                                          "SampleRateHz = " + hapticDesc.SampleRateHz + "\n " +
                                          "SampleSizeInBytes = " + hapticDesc.SampleSizeInBytes);
            }

            if (hand != Hand.Left) {

                hapticState = OVRPlugin.GetControllerHapticsState((uint)OVRInput.Controller.RTouch);
                hapticDesc = OVRPlugin.GetControllerHapticsDesc((uint)OVRInput.Controller.RTouch);
                if (debugging) Debug.Log("[OculusUtilities] Haptics state/desc of 'RTouch' during-call: \n " +
                                          "SamplesAvailable = " + hapticState.SamplesAvailable + "\n " +
                                          "SamplesQueud = " + hapticState.SamplesQueued + "\n " +
                                          "MaximumBufferSamplesCount = " + hapticDesc.MaximumBufferSamplesCount + "\n " +
                                          "MinimumBufferSamplesCount = " + hapticDesc.MinimumBufferSamplesCount + "\n " +
                                          "MinimumSafeSamplesQueued = " + hapticDesc.MinimumSafeSamplesQueued + "\n " +
                                          "OptimalBufferSamplesCount = " + hapticDesc.OptimalBufferSamplesCount + "\n " +
                                          "SampleRateHz = " + hapticDesc.SampleRateHz + "\n " +
                                          "SampleSizeInBytes = " + hapticDesc.SampleSizeInBytes);
            }


            yield return new WaitForSeconds(duration / 2);


            if (hand != Hand.Right) {
                if (debugging) Debug.Log("[OculusUtilities] Stopping haptics on 'LTouch' controller'");

                OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.LTouch);

                hapticState = OVRPlugin.GetControllerHapticsState((uint)OVRInput.Controller.LTouch);
                hapticDesc = OVRPlugin.GetControllerHapticsDesc((uint)OVRInput.Controller.LTouch);
                if (debugging) Debug.Log("[OculusUtilities] Haptics state/desc of 'LTouch' post-call: \n " +
                                          "SamplesAvailable = " + hapticState.SamplesAvailable + "\n " +
                                          "SamplesQueud = " + hapticState.SamplesQueued + "\n " +
                                          "MaximumBufferSamplesCount = " + hapticDesc.MaximumBufferSamplesCount + "\n " +
                                          "MinimumBufferSamplesCount = " + hapticDesc.MinimumBufferSamplesCount + "\n " +
                                          "MinimumSafeSamplesQueued = " + hapticDesc.MinimumSafeSamplesQueued + "\n " +
                                          "OptimalBufferSamplesCount = " + hapticDesc.OptimalBufferSamplesCount + "\n " +
                                          "SampleRateHz = " + hapticDesc.SampleRateHz + "\n " +
                                          "SampleSizeInBytes = " + hapticDesc.SampleSizeInBytes);
            }

            if (hand != Hand.Left) {
                if (debugging) Debug.Log("[OculusUtilities] Stopping haptics on 'RTouch' controller'");

                OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);

                hapticState = OVRPlugin.GetControllerHapticsState((uint)OVRInput.Controller.RTouch);
                hapticDesc = OVRPlugin.GetControllerHapticsDesc((uint)OVRInput.Controller.RTouch);
                if (debugging) Debug.Log("[OculusUtilities] Haptics state/desc of 'RTouch' post-call: \n " +
                                          "SamplesAvailable = " + hapticState.SamplesAvailable + "\n " +
                                          "SamplesQueud = " + hapticState.SamplesQueued + "\n " +
                                          "MaximumBufferSamplesCount = " + hapticDesc.MaximumBufferSamplesCount + "\n " +
                                          "MinimumBufferSamplesCount = " + hapticDesc.MinimumBufferSamplesCount + "\n " +
                                          "MinimumSafeSamplesQueued = " + hapticDesc.MinimumSafeSamplesQueued + "\n " +
                                          "OptimalBufferSamplesCount = " + hapticDesc.OptimalBufferSamplesCount + "\n " +
                                          "SampleRateHz = " + hapticDesc.SampleRateHz + "\n " +
                                          "SampleSizeInBytes = " + hapticDesc.SampleSizeInBytes);
            }
        }

    }

}