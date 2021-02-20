using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using PathCreation;
using System.IO;
using System;
using UnityEditor;

namespace Paperticket {
    public class WE02FishingGame2 : MonoBehaviour {

        [Header("REFERENCES")]

        [SerializeField] Transform rodTarget = null;
        [SerializeField] SkinnedMeshRenderer fishingRod = null;
        [SerializeField] AudioSource reelHandle = null;
        [SerializeField] Transform playerStartPos = null;

        [Space(15)]
        [SerializeField] Transform playerFish = null;
        [SerializeField] SpriteRenderer fishSprite = null;
        [SerializeField] Transform objectSprites = null;
        [SerializeField] LineRenderer fishingLine2D = null;
        [SerializeField] LineRenderer fishingLine3D = null;
        [SerializeField] Transform line3DTarget = null;

        Transform playerRig = null;
        //Transform playerHead;

        [Header("GENERAL CONTROLS")]

        [Space(10)]
        [SerializeField] bool autostart = false;
        [SerializeField] [Range(0,1)] float startingProgress = 0.026f;
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
        [SerializeField] float handleRotateSpeed = 1;


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
        [SerializeField] float line2DZOffset = 1f;
        [SerializeField] float line2DYOffset = 3f;
        [SerializeField] Vector3 fishToLine2DOffset;
        [SerializeField] float fishRotationMultiplier = 25;
        [Space(10)]
        [SerializeField] float line2DXMultiplier = 1f;
        [SerializeField] float line2DStartSpeed = 0.02f;
        [SerializeField] float line2DEndMaxDelta = 0.8f;
        [SerializeField] float line2DEndSmoothTime = 0.6f;
        [Space(10)]
        [SerializeField] Vector3 line3DMinPos = Vector3.zero;
        [SerializeField] Vector3 line3DMaxPos = Vector3.zero;
        [SerializeField] float line3DXMultiplier = 1f;

        [Header("FINISHING CONTROLS")]

        [Space(10)]
        [SerializeField] [Range(0, 1)] float finishProgress = 0.989f;
        [SerializeField] float finishDuration = 1;
        [SerializeField] float finishPlayerSpeed = 1.3f;

        [SerializeField] UnityEvent finishEvents = null;



        [Header("LIVE VARIABLES")]
        [Space(10)]
        [SerializeField] bool gameActive = false;
        [SerializeField] bool playerControl = false;
        [Space(10)]
        [SerializeField] float reelVelocity = 0;
        //[SerializeField] float reelAngular;
        [SerializeField] [Range(0, 1)] float reelTotal = 0;
        [Space(10)]
        [SerializeField] float rodDistance = 0;
        [Space(10)]
        [SerializeField] [Range(-1, 1)] float targetLine2DX = 0;
        [SerializeField] [Range(-1, 1)] float line2DStartX = 0;
        [SerializeField] [Range(-1, 1)] float line2DEndX = 0;
        [Space(5)]
        [SerializeField] [Range(-1, 1)] float targetLine3DX = 0;
        [SerializeField] [Range(-1, 1)] float line3DStartX = 0;
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
            set { gameActive = value; }
        }
        public bool PlayerControl {
            get { return playerControl; }
            set { playerControl = value; }
        }


        public bool Impeded {
            get { return impeded; }
            set { impeded = value;
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
            set { if (externalSpeedMod != 1 && value < externalSpeedMod) return;
                externalSpeedMod = value;

                // Play the "ouch" sound if necessary
                if (value < 1 && progress > 0.2 && !coralPlayed && onCoral != null) {
                    coralPlayed = true;
                    //onCoral.Invoke();
                } else if (value > 1) {
                    // Switch to the Speed animation
                    fishSprite.GetComponent<BasicAnimController>().PlayAnimationOnce(2);
                    // Play the "weee" sound if necessary
                    if (!bubblesPlayed && onBubbles != null) {
                        bubblesPlayed = true;
                        //onBubbles.Invoke();
                    }
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

            fishSprite.GetComponent<BasicAnimController>().SetAnimation(0);
        }






        void Awake() {

            if (objectSprites == null) Debug.LogError("[WE02FishingGame] ERROR -> No object sprite registered!");

            playerRig = PTUtilities.instance.playerRig.transform;
            //playerHead = PTUtilities.instance.headProxy;

            if (autostart) {
                gameActive = true;
                playerControl = true;
            }

            progress = startingProgress;
        }


        void FixedUpdate() {
            if (!gameActive) return;

            ResolveProgress();

            // Only do the following if the player is able to control the rod
            if (playerControl) {

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

            // Convert the fishing rod position into the target
            CalculateTarget();

            // Move the global Objects transform and change background color 
            UpdateObjects();

            // Move the fishing line renderer
            UpdateFishingLines();

            // Move/flip the player fish (must come after CalculateFishingLine)
            UpdatePlayerFish();


        }
        void CalculateTarget() {
            if (!Application.isPlaying) return;

            rodDistance = playerStartPos.InverseTransformPoint(rodTarget.position).x;

            //headDistance = playerRig.InverseTransformPoint(playerHead.position).x;

            targetLine2DX = Mathf.Clamp(rodDistance / rodMaxDistance, -1, 1) * line2DXMultiplier;

            targetLine3DX = Mathf.Clamp(rodDistance / rodMaxDistance, -1, 1) * line3DXMultiplier;

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
        void UpdateObjects() {

            // Update the reeling handle and control reeling audio
            if (playerControl && reelTotal > 0.075f) {

                reelHandle.transform.Rotate(reelVelocity * externalSpeedMod * Vector3.left * handleRotateSpeed * Time.fixedDeltaTime);

                if (!reelHandle.isPlaying) reelHandle.Play();

            } else {
                reelHandle.transform.Rotate(Vector3.right * 0.25f * handleRotateSpeed * Time.fixedDeltaTime);
                if (reelHandle.isPlaying) reelHandle.Pause();
            }
            // Move all underwater objects
            objectSprites.localPosition = new Vector3(0, objectsHeightCurve.Evaluate(progress), objectsZOffset);


        }
        Vector3 line2DStartPos = Vector3.zero;
        Vector3 line2DEndPos = Vector3.zero;
        float smoothVel = 0;
        void UpdateFishingLines() {

            // Move the end of the line towards the start
            line2DStartX = Mathf.Lerp(line2DStartX, targetLine2DX, line2DStartSpeed * (playerControl ? 1f : 0.5f));

            // Calculate the fishing line positions
            line2DStartPos = new Vector3(line2DStartX, line2DYOffset, line2DZOffset);

            // Move the fishing line renderer
            fishingLine2D.SetPosition(0, line2DStartPos);

            // Only move the end of the line if the player has control                        
            if (playerControl) {
                line2DEndX = Mathf.SmoothDamp(line2DEndX, line2DStartX, ref smoothVel, line2DEndSmoothTime, line2DEndMaxDelta);
                line2DEndPos = new Vector3(line2DEndX, fishHeight, line2DZOffset);
                fishingLine2D.SetPosition(1, line2DEndPos);
            }


            //// Move the 3D fishing line starting point between curved and straight
            //line3DStartX = Mathf.Lerp(line3DStartX, targetLine3DX, line2DStartSpeed);
            //fishingLine3D.SetPosition(0, Vector3.Lerp(line3DMinPos, line3DMaxPos, (rodBlendWeightMultiplier - fishingRod.GetBlendShapeWeight(0)) / rodBlendWeightMultiplier));
            //fishingLine3D.SetPosition(1, fishingLine3D.transform.InverseTransformPoint(line3DTarget.position + (line3DStartX * line3DTarget.right)));

        }

        float fishRotation = 0;
        void UpdatePlayerFish() {

            // Move the fish to track the end of the fishing line
            playerFish.localPosition = line2DEndPos + fishToLine2DOffset;

            // Check which side of the line the fish is on and rotate accordingly
            fishRotation = fishRotationMultiplier * (line2DEndX - line2DStartX);
            //if (lineStartX < lineEndX) 
                
            //else 
            //    fishRotation = -25 * (lineStartX - lineEndX);


            fishSprite.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, fishRotation));

        }




        void UpdateRodGraphics() {

            fishingRod.SetBlendShapeWeight(0, rodBlendWeightMultiplier * (1 - progress));
            //fishingRod.SetBlendShapeWeight(1, 0);

        }

        [Space(10)]
        int progressEventIndex = 0;
        void CheckProgressEvents() {

            for (int i = progressEventIndex; i < progressEvents.Length; i++) {
                if (debugging) Debug.Log("[WE02FishingGame2] Starting to check progress events");

                if (progress < progressEvents[i].threshold) {
                    if (debugging) Debug.Log("[WE02FishingGame2] Event(" + progressEventIndex + ") hasn't reached threshold("+progress+"/"+progressEvents[i].threshold+"), stopping events here for now");

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

                playerControl = false;
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

                fishingLine2D.transform.Translate(Vector3.up * finishPlayerSpeed * Time.fixedDeltaTime);
                playerFish.Translate(Vector3.up * finishPlayerSpeed * Time.fixedDeltaTime);

                yield return new WaitForFixedUpdate();
            }
                     

        }









        Coroutine autoMoveCo;
        float knockbackDuration = 1.5f;


        public void Knockback( float speed ) {

            if (!playerControl) return;

            playerControl = false;

            //fishSprite.color = Color.red;
            fishSprite.GetComponent<BasicAnimController>().PlayAnimationOnce(1);

            if (autoMoveCo != null) StopCoroutine(autoMoveCo);
            autoMoveCo = StartCoroutine(MoveAutomatically(-speed, knockbackDuration));

            // Play the "ouch" sound if haven't already
            if (!knockbackPlayed && onKnockback != null) {
                knockbackPlayed = true;
                //onKnockback.Invoke();
            }

        }


        public void MoveToStart() {

            line2DEndPos = new Vector3(line2DEndX, -0.25f, line2DZOffset);
            fishingLine2D.SetPosition(1, line2DEndPos);

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

            //fishSprite.color = Color.white;
            fishSprite.GetComponent<BasicAnimController>().SetAnimation(0);

            playerControl = true;
        }










        void OnDrawGizmosSelected() {

            if (debugging) {

                if (debugScroll) { progress = (progress + (debugScrollSpeed * 0.0001f)) % 1; }

                if (Application.isEditor && !Application.isPlaying) ResolveProgress();

            }
        }


    }
}