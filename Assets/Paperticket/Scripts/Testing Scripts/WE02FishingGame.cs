using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using System.IO;

namespace Paperticket {
    public class WE02FishingGame : MonoBehaviour {
        
        [SerializeField] Transform progressBar;

        [SerializeField] PathCreator fishProgressPath;
        [SerializeField] Transform fish;

        [Header("Controller Settings")]

        [SerializeField] float velocitySensitivity = 1.5f;
        [SerializeField] float angularSensitivity = 30f;

        [Header("Game Controls")]
        [SerializeField] float maxProgress = 40;    

        [SerializeField] float fishEscapeRate = 7;
        [SerializeField] float controllerHookRate = 30;
        [SerializeField] AnimationCurve controllerHookCurve;

        [Space(10)]

        [SerializeField] float cubeMaxHeight = 10;

        [Space(10)]

        [SerializeField] bool debugging = false;

               
        [Header("Read Only")]
        [SerializeField] float currentProgress = 0;
        [SerializeField] float currentHook = 0;

        [SerializeField] float currentVelocity = 0;
        [SerializeField] float currentAngularVelocity = 0;


        // Start is called before the first frame update
        void OnEnable() {
            progressBar.gameObject.SetActive(true);

            StartCoroutine(DeepFishing());

        }

        void OnDisable() {
            progressBar.gameObject.SetActive(false);

            StopAllCoroutines();    
        }


        IEnumerator DeepFishing() {
            yield return null;

            // Set initial values
            currentProgress = 0;
            velocitySensitivity = Mathf.Max(0.1f, velocitySensitivity);
            angularSensitivity = Mathf.Max(0.1f, angularSensitivity);

            //progressBar.localScale = Vector3.one - Vector3.up;
            //progressBar.GetComponentInChildren<MeshRenderer>().material.color = Color.white;

            SetFishProgress(1f);

            if (debugging) Debug.Log("[WE02FishingGame] Starting deep fishing challenge!");

            // Wait until progress is full
            while (currentProgress < maxProgress) {

                // Combine the current velocities 
                currentVelocity = Mathf.Clamp01(PTUtilities.instance.ControllerVelocity.magnitude / velocitySensitivity);
                currentAngularVelocity = Mathf.Clamp01(PTUtilities.instance.ControllerAngularVelocity.magnitude / angularSensitivity);

                currentHook = controllerHookCurve.Evaluate((currentVelocity + currentAngularVelocity) / 2);

                // Add the rate of the fish getting away and clamp the result
                currentProgress = Mathf.Clamp(currentProgress + ((currentHook * controllerHookRate) - fishEscapeRate) * Time.deltaTime, 0, maxProgress);
                if (debugging) Debug.Log("[WE02FishingGame] Current progress = " + currentProgress + " / " + maxProgress);

                // Change the graphics of the progress cube
                //progressBar.localScale = new Vector3(progressBar.localScale.x, (currentProgress / maxProgress) * cubeMaxHeight, progressBar.localScale.z);

                SetFishProgress(1 - (currentProgress / maxProgress));

                yield return null; 
            }

            if (debugging) Debug.Log("[WE02FishingGame] Finished the deep fishing challenge!");

            StartCoroutine(ShallowFishing());
        }

        IEnumerator ShallowFishing() {
            yield return null;

            // Set initial values
            bool hauledFish = false;
            //progressBar.localScale = new Vector3(progressBar.localScale.x, cubeMaxHeight, progressBar.localScale.z);
            //progressBar.GetComponentInChildren<MeshRenderer>().material.color = Color.red;

            SetFishProgress(0f);


            if (debugging) Debug.Log("[WE02FishingGame] Starting shallow fishing challenge!");

            // Wait for the quicktime event
            while (!hauledFish) {
                                
                // Wait until the trigger and touchpad are both pressed
                if (PTUtilities.instance.ControllerPrimaryButton && PTUtilities.instance.ControllerTriggerButton) {

                    // Should have to haul the fish with velocity
                    hauledFish = true;
                }

                yield return null;
            }

            // Celebrate!
            //progressBar.GetComponentInChildren<MeshRenderer>().material.color = Color.green;
            yield return new WaitForSeconds(1f);

            if (debugging) Debug.Log("[WE02FishingGame] Finished the shallow fishing challenge!");

            StartCoroutine(DeepFishing());
        }


        void SetFishProgress(float progress) {
            
            fish.position = fishProgressPath.path.GetPointAtTime(progress, EndOfPathInstruction.Stop);
            fish.rotation = fishProgressPath.path.GetRotation(progress, EndOfPathInstruction.Stop);
        }

    }
}