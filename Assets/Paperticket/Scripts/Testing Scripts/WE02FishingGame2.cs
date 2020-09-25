using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using System.IO;
using System;
using UnityEditor;

namespace Paperticket {
    public class WE02FishingGame2 : MonoBehaviour
    {

        [Header("REFERENCES")]

        [SerializeField] Transform rodTarget;
        [SerializeField] SkinnedMeshRenderer fishingRod;
        [Space(15)]
        [SerializeField] Transform playerFish;
        [SerializeField] SpriteRenderer fishSprite;
        [SerializeField] SpriteRenderer backgroundSprite;
        [SerializeField] Transform fadeSprite;
        [SerializeField] Transform objectSprites;
        [SerializeField] LineRenderer fishingLine;

        Transform playerRig;
        //Transform playerHead;


        [Header("CONTROLS")]
        [Space(10)]
        [SerializeField] float velocitySensitivity;
        //[SerializeField] float angularSensitivity;
        [SerializeField] AnimationCurve reelSensitivity;
        [SerializeField] float reelSpeed;
        [Space(10)]
        [SerializeField] float rodMaxDistance;
        [SerializeField] float rodBlendWeightMultiplier;
        [Space(10)]
        [SerializeField] AnimationCurve objectsHeightCurve;
        [SerializeField] AnimationCurve fadeHeightCurve;
        [SerializeField] Gradient backgroundColors;
        [Space(10)]
        [SerializeField] Vector3 fishToLineOffset;
        [SerializeField] float lineStartSpeed;
        [SerializeField] float lineEndMaxDelta;
        [SerializeField] float lineEndSmoothTime;

        [Header("READ ONLY")]
        [Space(10)]
        [SerializeField] float reelVelocity;
        //[SerializeField] float reelAngular;
        [SerializeField] [Range(0, 1)] float reelTotal;
        [Space(10)]
        [SerializeField] float rodDistance;
        [Space(10)]
        [SerializeField] [Range(-1, 1)] float targetLineX;
        [SerializeField] [Range(-1, 1)] float lineStartX;
        [SerializeField] [Range(-1, 1)] float lineEndX;
        [Space(10)]
        [SerializeField] [Range(0,1)] float progress;
        [Space(10)]
        [SerializeField] bool PlayerControl;

        [Header("DEBUG")]
        [Space(10)]
        [SerializeField] bool debugging;
        [SerializeField] bool debugScroll;
        [SerializeField] [Range(0.1f, 10)] float debugScrollSpeed;


        // PUBLIC VARIABLES

        [SerializeField] bool impeded;
        public bool Impeded {
            get { return impeded; } 
            set { impeded = value;
                if (impededCbeckCo == null) impededCbeckCo = StartCoroutine(ImpededCheck());
                else ResetImpeded();
            }
        }

        void Awake() {
                
            if (backgroundSprite == null) Debug.LogError("[WE02FishingGame] ERROR -> No background sprite registered!");
            if (objectSprites == null) Debug.LogError("[WE02FishingGame] ERROR -> No object sprite registered!");

            playerRig = PTUtilities.instance.playerRig.transform;
            //playerHead = PTUtilities.instance.headProxy;

            PlayerControl = true;
        }


        void FixedUpdate() {

            ResolveProgress();

        }

        Coroutine impededCbeckCo;
        float impedeTimer = 0.2f;
        IEnumerator ImpededCheck() {
            yield return new WaitForFixedUpdate();
            while (impedeTimer > 0) {
                yield return new WaitForFixedUpdate();
                impedeTimer -= Time.fixedDeltaTime;
            }
            impeded = false;
            impededCbeckCo = null;
        }
        void ResetImpeded() {
            impedeTimer = 0.2f;
        }




        void OnDrawGizmosSelected() {

            if (debugging) {

                if (debugScroll) { progress = (progress + (debugScrollSpeed * 0.0001f)) % 1; }

                if (Application.isEditor && !Application.isPlaying) ResolveProgress();

            }
        }


        void ResolveProgress() {
            
            // Convert the fishing rod position into the target
            CalculateTarget();


            // Only do the following if the player is able to control the rod
            if (PlayerControl) {

                // Work out how much the player is reeling in
                CalculateReeling();
            }


            // Move the global Objects transform and change background color 
            UpdateObjectsAndBackground();

            // Move the fishing line renderer
            UpdateFishingLine();

            // Move/flip the player fish (must come after CalculateFishingLine)
            UpdatePlayerFish();

            // Animate the fishing rod in the player's hand
            UpdateRodGraphics();
        }










        //float headDistance;
        void CalculateTarget() {
            if (!Application.isPlaying) return;

            rodDistance = playerRig.InverseTransformPoint(rodTarget.position).x;

            //headDistance = playerRig.InverseTransformPoint(playerHead.position).x;

            targetLineX = Mathf.Clamp(rodDistance / rodMaxDistance,-1,1);

        }

        void CalculateReeling() {
            if (!Application.isPlaying) return;

            // Combine the current velocities 
            reelVelocity = Mathf.Clamp01(PTUtilities.instance.ControllerVelocity.magnitude / velocitySensitivity);
            //reelAngular = Mathf.Clamp01(PTUtilities.instance.ControllerAngularVelocity.magnitude / angularSensitivity);

            //reelTotal = reelSensitivity.Evaluate((reelVelocity + reelAngular) / 2);
            reelTotal = reelSensitivity.Evaluate(reelVelocity);

            if (reelTotal > 0.05f) progress += (reelTotal * reelSpeed * 0.0001f * (impeded ? 0.4f : 1f));
            //progress += 0.0002f + (reelTotal * reelSpeed * 0.0001f);

        }


        
        void UpdateObjectsAndBackground() {

            // Evaluate background colours
            backgroundSprite.color = backgroundColors.Evaluate(progress);

            // Move all underwater objects and the fade sprite 
            objectSprites.localPosition = new Vector3(0, objectsHeightCurve.Evaluate(progress), 5);
            fadeSprite.localPosition = new Vector3(0, fadeHeightCurve.Evaluate(progress), 4.5f);


        }



        Vector3 lineStartPos;
        Vector3 lineEndPos;
        float smoothVel;
        void UpdateFishingLine() {

            

            // Move the end of the line towards the start
            lineStartX = Mathf.Lerp(lineStartX, targetLineX, lineStartSpeed);

            // Calculate the fishing line positions
            lineStartPos = new Vector3(lineStartX, 1.02f, 5);

            // Move the fishing line renderer
            fishingLine.SetPosition(0, lineStartPos);

            // Only move the end of the line if the player has control
            if (PlayerControl) {
                lineEndX = Mathf.SmoothDamp(lineEndX, lineStartX, ref smoothVel, lineEndSmoothTime, lineEndMaxDelta);
                lineEndPos = new Vector3(lineEndX, -0.75f, 5);
                fishingLine.SetPosition(1, lineEndPos);
            }

        }



        void UpdatePlayerFish() {

            // Check which side of the line the fish is on
            if (lineStartX < lineEndX) {

                // Move the fish to track the end of the fishing line
                playerFish.localPosition = lineEndPos + fishToLineOffset;

                // Flip the fish sprite and rotate
                fishSprite.flipX = true;
                fishSprite.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, -25));

            } else {

                // Move the fish to track the end of the fishing line
                playerFish.localPosition = lineEndPos - fishToLineOffset;

                // Flip the fish sprite and rotate
                fishSprite.flipX = false;
                fishSprite.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 25));
            }

        }



        void UpdateRodGraphics() {

            fishingRod.SetBlendShapeWeight(0, rodBlendWeightMultiplier * (1 - progress));
            fishingRod.SetBlendShapeWeight(1, 0);


        }






        Coroutine knockbackCo;
        float knockbackDuration = 1.5f;
        public void Knockback( float speed) {

            PlayerControl = false;

            if (knockbackCo != null) StopCoroutine(knockbackCo);
            knockbackCo = StartCoroutine(MoveAutomatically(-speed, knockbackDuration));

        }

        IEnumerator MoveAutomatically(float speed, float duration ) {

            yield return null;

            fishSprite.color = Color.red;

            float t = 0;
            while (t < duration) {

                progress += speed * 0.0001f;                
                t += Time.deltaTime;

                yield return new WaitForFixedUpdate();
            }

            fishSprite.color = Color.white;

            PlayerControl = true;
        }
    }
}