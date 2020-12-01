using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using PathCreation;
using System.IO;
using System;
using UnityEditor;

namespace Paperticket {
    public class WE02FishingGame : MonoBehaviour {

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
        [Space(15)]
        [SerializeField] Transform fishStartPos;

        Transform playerRig;
        //Transform playerHead;

        [Header("GENERAL CONTROLS")]

        [Space(10)]
        [SerializeField] bool autostart = false;
        [SerializeField] [Range(0, 1)] float startingProgress = 0.1f;
        [Space(10)]
        [SerializeField] float externalSpeedMod = 1;
        [SerializeField] float externalCheckRate = 0.2f;
        [SerializeField] float externalFadeOut = 0.5f;
        [Space(10)]
        [SerializeField] ProgressEvent[] progressEvents;


        [Header("ROD CONTROLS")]
        [Space(10)]
        [SerializeField] float velocitySensitivity = 0.8f;
        [SerializeField] AnimationCurve reelSensitivity;
        [SerializeField] float reelSpeed = 4;
        [Space(10)]
        [SerializeField] float rodMaxDistance = 1;
        [SerializeField] float rodBlendWeightMultiplier = 50;


        [Header("OBJECT CONTROLS")]
        [Space(10)]
        [SerializeField] AnimationCurve objectsHeightCurve;
        [SerializeField] AnimationCurve fadeHeightCurve;
        [SerializeField] Gradient backgroundColors;
        [Space(10)]
        [SerializeField] float fadeSpriteZOffset = 1.31f;
        [SerializeField] float objectsZOffset = 1.72f;



        [Header("LINE CONTROLS")]
        [Space(10)]
        [SerializeField] float fishHeight = -1.69f;
        [SerializeField] float lineZOffset = 1f;
        [SerializeField] float lineYOffset = 3f;
        [SerializeField] Vector3 fishToLineOffset;
        [SerializeField] float fishRotationMultiplier = 25;
        [Space(10)]
        [SerializeField] float lineXMultiplier = 1f;
        [SerializeField] float lineStartSpeed = 0.02f;
        [SerializeField] float lineEndMaxDelta = 0.8f;
        [SerializeField] float lineEndSmoothTime = 0.6f;



        [Header("FINISHING CONTROLS")]

        [Space(10)]
        [SerializeField] [Range(0, 1)] float finishProgress = 0.989f;
        [SerializeField] float finishDuration = 1;
        [SerializeField] float finishPlayerSpeed = 1.3f;

        [SerializeField] UnityEvent finishEvents;



        [Header("READ ONLY")]
        [Space(10)]
        [SerializeField] bool gameActive = false;
        [SerializeField] bool PlayerControl = false;
        [Space(10)]
        [SerializeField] float reelVelocity = 0;
        //[SerializeField] float reelAngular;
        [SerializeField] [Range(0, 1)] float reelTotal = 0;
        [Space(10)]
        [SerializeField] float rodDistance = 0;
        [Space(10)]
        [SerializeField] [Range(-1, 1)] float targetLineX = 0;
        [SerializeField] [Range(-1, 1)] float lineStartX = 0;
        [SerializeField] [Range(-1, 1)] float lineEndX = 0;
        [Space(10)]
        [SerializeField] [Range(0, 1)] float progress = 0;
        [Space(10)]
        [SerializeField] bool impeded = false;



        [Header("DEBUG")]
        [Space(10)]
        [SerializeField] bool debugging = false;
        [SerializeField] bool debugScroll = false;
        [SerializeField] [Range(0.1f, 10)] float debugScrollSpeed = 1.2f;



        [SerializeField] UnityEvent onKnockback;
        bool knockbackPlayed;
        [SerializeField] UnityEvent onBubbles;
        bool bubblesPlayed;
        [SerializeField] UnityEvent onCoral;
        bool coralPlayed;

        // PUBLIC VARIABLES  

        public bool GameActive {
            get { return gameActive; }
            set { gameActive = true; }
        }



        public bool Impeded {
            get { return impeded; }
            set {
                impeded = value;
                if (impededCbeckCo == null) impededCbeckCo = StartCoroutine(ImpededCheck());
                else ResetImpeded();
            }
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




        public float ExternalSpeedMod {
            get { return externalSpeedMod; }
            set {
                if (externalSpeedMod != 1 && value < externalSpeedMod) return;
                externalSpeedMod = value;

                // Play the "ouch" or "weee" sounds if necessary
                if (value < 1 && progress > 0.2 && !coralPlayed && onCoral != null) {
                    coralPlayed = true;
                    onCoral.Invoke();
                } else if (value > 1 && !bubblesPlayed && onBubbles != null) {
                    bubblesPlayed = true;
                    onBubbles.Invoke();
                }

                // Start a new set of checks if we aren't checking yet
                if (externalModCo == null) {
                    externalModCo = StartCoroutine(CheckingExternalMod());

                    // Check for and cancel any fading of the external speed, then start a new set of checks
                } else if (externalFadeCo != null) {
                    StopCoroutine(externalFadeCo);
                    externalFadeCo = null;
                    StopCoroutine(externalModCo);
                    externalModCo = externalModCo = StartCoroutine(CheckingExternalMod());

                    // Otherwise just reset the currently running timer
                } else externalCheckTimer = externalCheckRate;
            }
        }
        Coroutine externalModCo;
        Coroutine externalFadeCo;
        float externalCheckTimer = 0;
        IEnumerator CheckingExternalMod() {
            yield return new WaitForFixedUpdate();
            externalCheckTimer = externalCheckRate;
            while (externalCheckTimer > 0) {
                yield return new WaitForFixedUpdate();
                externalCheckTimer -= Time.fixedDeltaTime;
            }
            externalFadeCo = StartCoroutine(FadingExternalMod());
        }
        IEnumerator FadingExternalMod() {
            float t = externalFadeOut;
            float startMod = externalSpeedMod;
            while (t > 0) {
                yield return new WaitForFixedUpdate();
                externalSpeedMod = Mathf.Lerp(startMod, 1f, 1f - t);
                t -= Time.fixedDeltaTime;
            }
            externalSpeedMod = 1;
            externalFadeCo = null;
            externalModCo = null;
        }






        void Awake() {

            if (backgroundSprite == null) Debug.LogError("[WE02FishingGame] ERROR -> No background sprite registered!");
            if (objectSprites == null) Debug.LogError("[WE02FishingGame] ERROR -> No object sprite registered!");

            playerRig = PTUtilities.instance.playerRig.transform;
            //playerHead = PTUtilities.instance.headProxy;

            if (autostart) {
                gameActive = true;
                PlayerControl = true;
            }

            progress = startingProgress;
        }


        void FixedUpdate() {
            if (!gameActive) return;

            ResolveProgress();

            // Only do the following if the player is able to control the rod
            if (PlayerControl) {

                // Work out how much the player is reeling in
                CalculateReeling();

                // Animate the fishing rod in the player's hand
                UpdateRodGraphics();

                // Check for any of the events keyed to go off at a certain amount of progress
                CheckProgressEvents();

                // Check if the game has finished, to transition to the final events
                CheckForFinish();
            }


        }


        void ResolveProgress() {

            // Convert the real fishing rod position into a target position
            CalculateTarget();

            // Move the global Objects transform and change background color 
            UpdateObjectsAndBackground();

            // Move the fishing line renderer
            UpdateFishingLine();

            // Move/flip the player fish (must come after CalculateFishingLine)
            UpdatePlayerFish();


        }
        void CalculateTarget() {
            if (!Application.isPlaying) return;

            rodDistance = playerRig.InverseTransformPoint(rodTarget.position).x;

            //headDistance = playerRig.InverseTransformPoint(playerHead.position).x;

            targetLineX = Mathf.Clamp(rodDistance / rodMaxDistance, -1, 1) * lineXMultiplier;

        }
        void CalculateReeling() {
            if (!Application.isPlaying) return;

            // Save the current controller velocity and apply senitivity curve 
            reelVelocity = Mathf.Clamp01(PTUtilities.instance.ControllerVelocity.magnitude / velocitySensitivity);
            reelTotal = reelSensitivity.Evaluate(reelVelocity);

            //if (reelTotal > 0.05f) progress += (reelTotal * reelSpeed * 0.0001f * (impeded ? 0.4f : 1f));
            if (externalSpeedMod <= 1) {
                if (reelTotal > 0.05f) progress += reelTotal * reelSpeed * 0.0001f * externalSpeedMod;
            } else {
                progress += ((reelTotal * reelSpeed) + (externalSpeedMod - 1)) * 0.0001f;
            }

        }
        void UpdateObjectsAndBackground() {

            // Evaluate background colours
            backgroundSprite.color = backgroundColors.Evaluate(progress);

            // Move all underwater objects and the fade sprite 
            objectSprites.localPosition = new Vector3(0, objectsHeightCurve.Evaluate(progress), objectsZOffset);
            fadeSprite.localPosition = new Vector3(0, fadeHeightCurve.Evaluate(progress), fadeSpriteZOffset);


        }
        Vector3 lineStartPos = Vector3.zero;
        Vector3 lineEndPos = Vector3.zero;
        float smoothVel = 0;
        void UpdateFishingLine() {

            // Move the end of the line towards the start
            lineStartX = Mathf.Lerp(lineStartX, targetLineX, lineStartSpeed * (PlayerControl ? 1f : 0.5f));

            // Calculate the fishing line positions
            lineStartPos = new Vector3(lineStartX, lineYOffset, lineZOffset);

            // Move the fishing line renderer
            fishingLine.SetPosition(0, lineStartPos);

            // Only move the end of the line if the player has control                        
            if (PlayerControl) {
                lineEndX = Mathf.SmoothDamp(lineEndX, lineStartX, ref smoothVel, lineEndSmoothTime, lineEndMaxDelta);
                lineEndPos = new Vector3(lineEndX, fishHeight, lineZOffset);
                fishingLine.SetPosition(1, lineEndPos);
            }

        }

        float fishRotation = 0;
        void UpdatePlayerFish() {

            // Move the fish to track the end of the fishing line
            playerFish.localPosition = lineEndPos + fishToLineOffset;

            // Check which side of the line the fish is on and rotate accordingly
            fishRotation = fishRotationMultiplier * (lineEndX - lineStartX);
            fishSprite.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, fishRotation));

        }




        void UpdateRodGraphics() {

            fishingRod.SetBlendShapeWeight(0, rodBlendWeightMultiplier * (1 - progress));
            fishingRod.SetBlendShapeWeight(1, 0);

        }

        [Space(10)]
        int progressEventIndex = 0;
        void CheckProgressEvents() {

            for (int i = progressEventIndex; i < progressEvents.Length; i++) {
                if (debugging) Debug.Log("[WE02FishingGame2] Starting to check progress events");

                if (progress < progressEvents[i].threshold) {
                    if (debugging) Debug.Log("[WE02FishingGame2] Event(" + progressEventIndex + ") hasn't reached threshold(" + progress + "/" + progressEvents[i].threshold + "), stopping events here for now");

                    return;

                } else {
                    if (debugging) Debug.Log("[WE02FishingGame2] Event(" + progressEventIndex + ") reached threshold(" + progress + "/" + progressEvents[i].threshold + ")! Moving on...");

                    if (progressEvents[i].progressEvent != null) progressEvents[i].progressEvent.Invoke();

                    progressEventIndex += 1;
                }

                if (debugging) Debug.Log("[WE02FishingGame2] Finished checking progress events");
            }


        }

        void CheckForFinish() {
            if (progress >= finishProgress) {

                PlayerControl = false;
                gameActive = false;
                StopAllCoroutines();

                // Do Splash etc
                if (finishEvents != null) {
                    finishEvents.Invoke();
                }


                autoMoveCo = StartCoroutine(FinishAnimation());

            }
        }
        IEnumerator FinishAnimation() {

            float t = 0;
            while (t < finishDuration) {
                t += Time.fixedDeltaTime;

                fishingLine.transform.Translate(Vector3.up * finishPlayerSpeed * Time.fixedDeltaTime);
                playerFish.Translate(Vector3.up * finishPlayerSpeed * Time.fixedDeltaTime);

                yield return new WaitForFixedUpdate();
            }


        }









        Coroutine autoMoveCo;
        float knockbackDuration = 1.5f;


        public void Knockback( float speed ) {

            if (!PlayerControl) return;

            PlayerControl = false;

            fishSprite.color = Color.red;

            if (autoMoveCo != null) StopCoroutine(autoMoveCo);
            autoMoveCo = StartCoroutine(MoveAutomatically(-speed, knockbackDuration));

            // Play the "ouch" sound if haven't already
            if (!knockbackPlayed && onKnockback != null) {
                knockbackPlayed = true;
                onKnockback.Invoke();
            }

        }


        public void MoveToStart() {

            lineEndPos = new Vector3(lineEndX, -0.25f, lineZOffset);
            fishingLine.SetPosition(1, lineEndPos);

            //lineEndPos = transform.InverseTransformPoint(fishStartPos.position);

            if (autoMoveCo != null) StopCoroutine(autoMoveCo);
            autoMoveCo = StartCoroutine(MoveAutomatically(-progress, 2.5f));

        }

        IEnumerator MoveAutomatically( float adjustment, float duration ) {

            yield return null;

            float t = 0;
            float startProg = progress;
            while (t < duration) {

                progress = Mathf.Lerp(startProg, startProg + adjustment, AnimationCurve.EaseInOut(0f, 0f, 1f, 1f).Evaluate(Mathf.Clamp01(t / duration)));

                //progress += speed * 0.0001f;                
                t += Time.fixedDeltaTime;

                yield return new WaitForFixedUpdate();
            }
            progress = startProg + adjustment;

            fishSprite.color = Color.white;

            PlayerControl = true;
        }










        void OnDrawGizmosSelected() {

            if (debugging) {

                if (debugScroll) { progress = (progress + (debugScrollSpeed * 0.0001f)) % 1; }

                if (Application.isEditor && !Application.isPlaying) ResolveProgress();

            }
        }


    }
}