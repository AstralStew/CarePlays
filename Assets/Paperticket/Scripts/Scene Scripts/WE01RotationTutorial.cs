using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Paperticket {
    public class WE01RotationTutorial : MonoBehaviour {

        [Header("REFERENCES")]

        //[SerializeField] RectTransform progressBar;
        [SerializeField] Image progressImage;

        [Header("HAND CONTROLS")]
        [Space(10)]
        [SerializeField] float velocitySensitivity;
        [SerializeField] AnimationCurve rotationSensitivity;

        [Header("PROGRESS CONTROLS")]
        [Space(10)]
        [SerializeField] float progressSpeed;
        //[SerializeField] float progressMax;


        [Header("READ ONLY")]
        [Space(10)]
        [SerializeField] [Range(0, 1)] float progress;
        float rotateVelocity;
        float rotationTotal;
        bool finished;

        [Space(10)]        
        [SerializeField] List<ProgressEvent> progressEvents = new List<ProgressEvent>();



        // Update is called once per frame
        void Update() {
            if (finished) return;

            CalculateHandRotation();
            UpdateGraphics();
            CheckProgressEvents();

            if (progress >= 1) finished = true;

        }


        void CalculateHandRotation() {

            // Save the current controller velocity and apply senitivity curve 
            rotateVelocity = Mathf.Clamp01(PTUtilities.instance.ControllerVelocity.magnitude / velocitySensitivity);
            rotationTotal = rotationSensitivity.Evaluate(rotateVelocity);

            if (rotationTotal > 0.05f) progress = Mathf.Clamp01(progress + (rotationTotal * progressSpeed * 0.0001f));

        }

        void UpdateGraphics() {

            //progressBar.sizeDelta = new Vector2(progress * progressMax, progressBar.sizeDelta.y);
            progressImage.fillAmount = progress;

        }

        int progressEventIndex = 0;
        void CheckProgressEvents() {

            for (int i = progressEventIndex; i < progressEvents.Count; i++) {
                if (progress >= progressEvents[i].threshold) {
                    if (progressEvents[i].progressEvent != null) progressEvents[i].progressEvent.Invoke();
                    progressEventIndex += 1;
                }

            }

        }

    }
}



