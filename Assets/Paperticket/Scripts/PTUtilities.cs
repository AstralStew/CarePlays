using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

namespace Paperticket {

    public enum GameEventOption { OnButtonDown, OnButtonUp, OnTriggerPress, OnTriggerUp }
    public enum Hand { Left, Right }

    public class PTUtilities : MonoBehaviour {

        [Header("References")]

        public static PTUtilities instance = null;

        public AudioMixer _ResonanceMaster;

        public XRRig playerRig;



        [Header("Controls")]
        
        //[SerializeField] Vector3 velocityTest;
        //[SerializeField] float velocitySensitivity;
        //[SerializeField] Vector3 angularTest;
        //[SerializeField] float angularSensitivity;
        //[SerializeField] Vector3 accelerationTest;
        //[SerializeField] float accelerationSensitivity;
        //[SerializeField] Vector3 angularAccelerationTest;
        //[SerializeField] float angularAccelerationSensitivity;

        public bool _Debug;

        

        [Header("Read Only")]

        public Transform headProxy;
        public XRController controller;
        MeshRenderer controllerRenderer;

        enum Handedness { Left, Right, Both }

        
        public Vector3 HeadsetPosition() {
            return headProxy.position;
        }

        public Vector3 HeadsetForward() {
            return headProxy.forward;
        }

        public Quaternion HeadsetRotation() {
            return Quaternion.Euler(new Vector3(0, headProxy.rotation.eulerAngles.y, 0));
        }
        public Vector3 HeadsetRotationAsEuler() {
            return new Vector3(0, headProxy.rotation.eulerAngles.y, 0);
        }

        public bool ControllerTriggerButton;
        public bool ControllerPrimaryButton;
        public Vector3 ControllerVelocity;
        public Vector3 ControllerAngularVelocity;
        public Vector3 ControllerAcceleration;
        public Vector3 ControllerAngularAcceleration;
        
        [HideInInspector] public bool SetupComplete;


        //public bool LeftControllerBeamActive {
        //    get { return leftController.GetComponent<XRInteractorLineVisual>().enabled; }
        //    set { leftController.GetComponent<XRInteractorLineVisual>().enabled = value; }
        //}

        public bool ControllerBeamActive {
            get { return controller.GetComponent<XRInteractorLineVisual>().enabled; }
            set { controller.GetComponent<XRInteractorLineVisual>().enabled = value; }
        }


        void Awake() {

            // Create an instanced version of this script, or destroy it if one already exists
            if (instance == null) {
                instance = this;
            } else if (instance != this) {
                Destroy(gameObject);
            }
                       
            StartCoroutine(Setup());
            
        }
        IEnumerator Setup() {
            if (_Debug) Debug.Log("[PTUtilities] Starting setup...");

            // Make sure our player rig is defined
            while (playerRig == null) {
                Debug.LogError("[PTUtilities] ERROR -> No Player Rig defined! Something has gone wrong!");
                yield return null;
            }


            // Grab the player's head camera            
            while (headProxy == null) {
                if (_Debug) Debug.Log("[PTUtilities] Looking for Head Proxy object...");
                headProxy = playerRig.cameraGameObject.transform;
                yield return null;
                //enabled = false;
            }
            if (_Debug) Debug.Log("[PTUtilities] Head Proxy found!");


            // Grab the controller
            while (controller == null) {
                if (_Debug) Debug.Log("[PTUtilities] Looking for Controller object...");
                foreach (XRController cont in playerRig.GetComponentsInChildren<XRController>()) {
                    if (cont.controllerNode == XRNode.LeftHand || cont.controllerNode == XRNode.RightHand) controller = cont;
                }
                yield return null;
            }
            if (_Debug) Debug.Log("[PTUtilities] Controller found!");


            // Grab the controller renderer            
            while (controllerRenderer == null) {
                if (_Debug) Debug.Log("[PTUtilities] Looking for Controller Renderer object...");
                controllerRenderer = controller.modelTransform.GetComponentInChildren<MeshRenderer>();
                yield return null;
            }
            if (_Debug) Debug.Log("[PTUtilities] Controller Renderer found!");

            // Finish setup
            SetupComplete = true;
            if (_Debug) Debug.Log("[PTUtilities] Setup complete!");
        }


        void FixedUpdate() {
            if (!SetupComplete) return;


            List<XRNodeState> nodes = new List<XRNodeState>();
            InputTracking.GetNodeStates(nodes);

            foreach (XRNodeState ns in nodes) {
                if (ns.nodeType == XRNode.RightHand) { 
                    ns.TryGetVelocity(out ControllerVelocity);
                    ns.TryGetAngularVelocity(out ControllerAngularVelocity);                    
                    ns.TryGetAcceleration(out ControllerAcceleration);                    
                    ns.TryGetAngularAcceleration(out ControllerAngularAcceleration);
                    
                }
            }

            //velocityTest = new Vector3 (Mathf.Round(velocityTest.x * 100f) / 100f, Mathf.Round(velocityTest.y * 100f) / 100f, Mathf.Round(velocityTest.z * 100f) / 100f);
            //angularTest = new Vector3(Mathf.Round(angularTest.x * 100f) / 100f, Mathf.Round(angularTest.y * 100f) / 100f, Mathf.Round(angularTest.z * 100f) / 100f);
            //accelerationTest = new Vector3(Mathf.Round(accelerationTest.x * 100f) / 100f, Mathf.Round(accelerationTest.y * 100f) / 100f, Mathf.Round(accelerationTest.z * 100f) / 100f);
            //angularAccelerationTest = new Vector3(Mathf.Round(angularAccelerationTest.x * 100f) / 100f, Mathf.Round(angularAccelerationTest.y * 100f) / 100f, Mathf.Round(angularAccelerationTest.z * 100f) / 100f);

            //velocitySensitivity = Mathf.Max(0.1f, velocitySensitivity);
            //angularSensitivity = Mathf.Max(0.1f, angularSensitivity);
            //accelerationSensitivity = Mathf.Max(0.1f, accelerationSensitivity);
            //angularAccelerationSensitivity = Mathf.Max(0.1f, angularAccelerationSensitivity);

            //Color finalCol = (Color.red * Mathf.Clamp01(velocityTest.magnitude / velocitySensitivity)) + (Color.blue * Mathf.Clamp01(angularTest.magnitude / angularSensitivity));
            //controllerRenderer.material.color = Color.Lerp(Color.white, finalCol, finalCol.maxColorComponent);


        }


        bool lastTriggerState;
        bool lastPrimaryState;
        void Update() {
            if (!SetupComplete) return;

            if (controller.inputDevice.TryGetFeatureValue(CommonUsages.triggerButton, out ControllerTriggerButton) && ControllerTriggerButton) {                
                if (!lastTriggerState) {
                    // send event          
                    if (_Debug) Debug.Log("[PTUtilities] Trigger button was just pressed.");
                }
            }
            lastTriggerState = ControllerTriggerButton;


            if (controller.inputDevice.TryGetFeatureValue(CommonUsages.primaryButton, out ControllerPrimaryButton) && ControllerPrimaryButton) {
                if (!lastPrimaryState) {
                    // send event          
                    if (_Debug) Debug.Log("[PTUtilities] Primary button was just pressed.");
                }
            }
            lastPrimaryState = ControllerPrimaryButton;

        }



        /// --------------------------------------- PUBLIC CALLS --------------------------------------- \\\
        // A list of general calls that perform common actions in the project


        /// <summary>
        /// Teleport the player to the specified position and rotation, taking into account player height for mobile
        /// </summary>
        /// <param name="position">The position to telelport to</param>
        /// <param name="rotation">The rotation when teleported</param>
        public void TeleportPlayer( Vector3 position, Quaternion rotation ) {

            transform.SetPositionAndRotation(position, rotation);

            //// Grab the teleport script (GO: TeleportSettings) 
            //VRTK_BasicTeleport teleport = VRTK_ObjectCache.registeredTeleporters[0];

            //// Force the teleport, ignoring any target checking or floor adjustment
            //teleport.ForceTeleport(position, HeadsetRotationToMatch(rotation));

        }






        // Toggle VRTK settings 

        /// <summary>
        /// Toggle the 3D model for the controller models
        /// </summary>
        /// <param name="toggle">True to enable, false to disable</param>
        public void ToggleControllerModel( Hand hand, bool toggle ) {

            //GameObject modelAlias = null;
            switch (hand) {
                case Hand.Left:
                    //VRTK_ObjectAppearance.ToggleRenderer(toggle, leftModelAlias);

                    break;
                case Hand.Right:
                    //VRTK_ObjectAppearance.ToggleRenderer(toggle, rightModelAlias);

                    break;
                default:
                    Debug.LogError("[PTUtilities] ERROR -> Bad hand choice, something's very wrong!");
                    break;
            }

        }

        /// <summary>
        /// Toggle the 3D model for the controller models
        /// </summary>
        /// <param name="toggle">True to enable, false to disable</param>
        /// <param name="ignoredModel">True to enable, false to disable</param>
        public void ToggleControllerModel( Hand hand, bool toggle, GameObject ignoredModel ) {

            //GameObject modelAlias = null;
            switch (hand) {
                case Hand.Left:
                    //if (VRTK_SDKManager.instance.scriptAliasLeftController) {
                    //    modelAlias = VRTK_DeviceFinder.GetModelAliasController(VRTK_DeviceFinder.GetControllerLeftHand());
                    //    if (_Debug) Debug.Log("[PTUtilities] Setting left controller model to " + toggle);
                    //} else {
                    //    Debug.LogError("[PTUtilities] ERROR -> ScriptAliasLeftController is not bound in VRTK_SDKManager!");
                    //}
                    //VRTK_ObjectAppearance.ToggleRenderer(toggle, leftModelAlias, ignoredModel);
                    break;
                case Hand.Right:
                    //if (VRTK_SDKManager.instance.scriptAliasRightController) {
                    //    modelAlias = VRTK_DeviceFinder.GetModelAliasController(VRTK_DeviceFinder.GetControllerRightHand());
                    //    if (_Debug) Debug.Log("[PTUtilities] Setting right controller model to " + toggle);
                    //} else {
                    //    Debug.LogError("[PTUtilities] ERROR -> ScriptAliasRightController is not bound in VRTK_SDKManager!");
                    //}
                    //VRTK_ObjectAppearance.ToggleRenderer(toggle, rightModelAlias, ignoredModel);
                    break;
                default:
                    Debug.LogError("[PTUtilities] ERROR -> Bad hand choice, something's very wrong!");
                    break;
            }

            //VRTK_ObjectAppearance.ToggleRenderer(toggle, modelAlias, ignoredModel);

        }

        /// <summary>
        /// Toggle the renderer of the controller pointer beam
        /// </summary>
        /// <param name="toggle"> True to enable, false to disable</param>
        //public void TogglePointerRenderer( Hand hand, bool toggle ) {

        //    // Get the relevant pointer for the chosen hand
        //    VRTK_StraightPointerRenderer pointer = null;
        //    switch (hand) {
        //        case Hand.Left:
        //            pointer = leftScriptAlias.GetComponentInChildren<VRTK_StraightPointerRenderer>(true);

        //            break;
        //        case Hand.Right:
        //            pointer = rightScriptAlias.GetComponentInChildren<VRTK_StraightPointerRenderer>(true);

        //            break;
        //        default:
        //            Debug.LogError("[PTUtilities] ERROR -> Bad hand choice, something's very wrong!");
        //            break;
        //    }

        //    if (toggle) {
        //        pointer.gameObject.SetActive(true);
        //        pointer.ResetPointerObjects();
        //    } else {
        //        pointer.gameObject.SetActive(false);
        //    }


        //}


        // Haptics

        bool leftHapticsActivated;
        Coroutine leftHapticsCoroutine;
        bool rightHapticsActivated;
        Coroutine rightHapticsCoroutine;

        public void ToggleControllerHaptics( Hand hand, bool toggle, float strength ) {

            switch (hand) {
                case Hand.Left:
                    if (toggle) {
                        if (!leftHapticsActivated) {
                            leftHapticsCoroutine = StartCoroutine(RunningHaptics(hand, strength));
                            leftHapticsActivated = true;
                        } else {
                            if (_Debug) Debug.Log("[PTUTilities] Haptics already active! Ignoring call");
                        }
                    } else {
                        if (leftHapticsActivated) {
                            StopCoroutine(leftHapticsCoroutine);
                            leftHapticsActivated = false;
                        } else {
                            if (_Debug) Debug.Log("[PTUTilities] Haptics already inactive! Ignoring call");
                        }
                    }

                    break;
                case Hand.Right:
                    if (toggle) {
                        if (!rightHapticsActivated) {
                            rightHapticsCoroutine = StartCoroutine(RunningHaptics(hand, strength));
                            rightHapticsActivated = true;
                        } else {
                            if (_Debug) Debug.Log("[PTUTilities] Haptics already active! Ignoring call");
                        }
                    } else {
                        if (rightHapticsActivated) {
                            StopCoroutine(rightHapticsCoroutine);
                            rightHapticsActivated = false;
                        } else {
                            if (_Debug) Debug.Log("[PTUTilities] Haptics already inactive! Ignoring call");
                        }
                    }

                    break;
                default:
                    Debug.LogError("[PTUtilities] ERROR -> Bad hand choice in haptics, something's very wrong!");
                    break;
            }
        }
        IEnumerator RunningHaptics( Hand hand, float strength ) {
            //VRTK_ControllerReference controllerReference = null;

            switch (hand) {
                case Hand.Left:
                    //  controllerReference = VRTK_ControllerReference.GetControllerReference(SDK_BaseController.ControllerHand.Left);
                    break;
                case Hand.Right:
                    //  controllerReference = VRTK_ControllerReference.GetControllerReference(SDK_BaseController.ControllerHand.Right);
                    break;
                default:
                    Debug.LogError("[PTUtilities] ERROR -> Bad hand choice, something's very wrong!");
                    break;
            }

            while (true) {
                // VRTK_ControllerHaptics.TriggerHapticPulse(controllerReference, strength);
                yield return null;
            }

        }


        /// <summary>
        /// Toggle the controller highlighter for the controller models
        /// </summary>
        /// <param name="toggle">True to enable, false to disable</param>
        //public void ToggleControllerHighlight( Hand hand,  bool toggle, SDK_BaseController.ControllerElements highlightedElement, Color highlightColor, float fadeDuration) {

        //   // VRTK_ControllerHighlighter controllerHighlighter = null;
        //    GameObject modelAlias = null;
        //    switch (hand) {
        //        case Hand.Left:
        //            //controllerHighlighter = leftScriptAlias.GetComponent<VRTK_ControllerHighlighter>();
        //            modelAlias = leftModelAlias;

        //            break;
        //        case Hand.Right:
        //           // controllerHighlighter = rightScriptAlias.GetComponent<VRTK_ControllerHighlighter>();
        //            modelAlias = rightModelAlias;

        //            break;
        //        default:
        //            Debug.LogError("[PTUtilities] ERROR -> Bad hand choice, something's very wrong!");
        //            break;
        //    }

        //    if (toggle) {
        //      //  controllerHighlighter.HighlightElement(highlightedElement, highlightColor, 0.05f);
        //      //  VRTK_ObjectAppearance.SetOpacity(modelAlias, 0.5f, fadeDuration);
        //    } else {
        //      //  controllerHighlighter.UnhighlightController();
        //     //   VRTK_ObjectAppearance.SetOpacity(modelAlias, 1f, fadeDuration);
        //    }
        //}

        /// <summary>
        /// Toggle the text on the controller models that designates left/right
        /// </summary>
        /// <param name="hand">The hand to toggle</param>
        /// <param name="toggle">True to enable, false to disable</param>
        public void ToggleControllerText( Hand hand, bool toggle ) {

            //controllerProxy.GetComponentInChildren<TextMeshPro>(true).gameObject.SetActive(toggle);

            //switch (hand) {
            //    case Hand.Left:
                    

            //        break;
            //    case Hand.Right:
            //        rightScriptAlias.GetComponentInChildren<TextMeshPro>(true).gameObject.SetActive(toggle);

            //        break;
            //    default:
            //        Debug.LogError("[PTUtilities] ERROR -> Bad hand choice in controller text, something's very wrong!");
            //        break;
            //}


        }



       





        // --------------------------------------- UTILITIES --------------------------------------- \\
        // The generalised helper ienumerators which change each setting over time

        // Helper coroutine for fading the alpha of a sprite
        public IEnumerator FadeAlphaTo( SpriteRenderer sprite, float targetAlpha, float duration ) {

            if (sprite.color.a != targetAlpha) {

                if (_Debug) Debug.Log("[PTUtilities] Fading Sprite " + sprite.name + " to alpha " + targetAlpha);

                if (!sprite.enabled) {
                    sprite.enabled = true;
                }

                float alpha = sprite.color.a;
                for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / duration) {
                    sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, Mathf.Lerp(alpha, targetAlpha, t));
                    yield return null;
                }
                sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, targetAlpha);

                if (targetAlpha == 0f) {
                    sprite.enabled = false;
                }
                yield return null;

                if (_Debug) Debug.Log("[PTUtilities] Sprite " + sprite.name + " successfully faded to alpha " + alpha);

            } else {
                if (_Debug) Debug.LogWarning("[PTUtilities] Sprite " + sprite.name + " already at alpha " + targetAlpha + ", cancelling fade");
            }

        }
        // Helper coroutine for fading the alpha of text
        public IEnumerator FadeAlphaTo( TextMeshPro textmesh, float targetAlpha, float duration ) {

            if (textmesh.color.a != targetAlpha) {

                if (_Debug) Debug.Log("[PTUtilities] Fading TextMesh " + textmesh.name + "to alpha " + targetAlpha);

                if (!textmesh.enabled) {
                    textmesh.enabled = true;
                }

                float alpha = textmesh.color.a;
                for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / duration) {
                    textmesh.color = new Color(textmesh.color.r, textmesh.color.g, textmesh.color.b, Mathf.Lerp(alpha, targetAlpha, t));
                    yield return null;
                }
                textmesh.color = new Color(textmesh.color.r, textmesh.color.g, textmesh.color.b, targetAlpha);

                if (targetAlpha == 0f) {
                    textmesh.enabled = false;
                }
                yield return null;

                if (_Debug) Debug.Log("[PTUtilities] TextMesh " + textmesh.name + "successfully faded to alpha " + targetAlpha);

            } else {
                if (_Debug) Debug.LogWarning("[PTUtilities] TextMesh " + textmesh.name + " already at alpha " + targetAlpha + ", cancelling fade");
            }

        }
        // Helper coroutine for fading the alpha of mesh renderer
        public IEnumerator FadeAlphaTo( MeshRenderer mRenderer, float targetAlpha, float duration ) {

            Material mat = mRenderer.material;
            Color col = mat.GetColor("_Color");

            if (col.a != targetAlpha) {

                if (_Debug) Debug.Log("[PTUtilities] Fading MeshRenderer " + mRenderer.name + "to alpha " + targetAlpha);

                if (!mRenderer.enabled) {
                    mRenderer.enabled = true;
                }

                float alpha = col.a;
                for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / duration) {
                    Color newColor = new Color(col.r, col.g, col.b, Mathf.Lerp(alpha, targetAlpha, t));
                    mat.SetColor("_Color", newColor);
                    yield return null;
                }
                mat.SetColor("_Color", new Color(col.r, col.g, col.b, targetAlpha));


                if (targetAlpha == 0f) {
                    mRenderer.enabled = false;
                }
                yield return null;

                if (_Debug) Debug.Log("[PTUtilities] MeshRenderer " + mRenderer.name + " successfully faded to alpha " + targetAlpha);

            } else {
                if (_Debug) Debug.LogWarning("[PTUtilities] MeshRenderer " + mRenderer.name + " already at alpha " + targetAlpha + ", cancelling fade");
            }
        }

        // Helper coroutine for fading the color of a sprite
        public IEnumerator FadeColorTo( SpriteRenderer sprite, Color targetColor, float duration ) {

            if (sprite.color != targetColor) {

                if (_Debug) Debug.Log("[PTUtilities] Fading Sprite " + sprite.name + "to color " + targetColor);

                if (!sprite.enabled) {
                    sprite.enabled = true;
                }

                Color color = sprite.color;
                for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / duration) {
                    sprite.color = Color.Lerp(color, targetColor, t);
                    yield return null;
                }
                sprite.color = targetColor;

                if (targetColor.a == 0f) {
                    sprite.enabled = false;
                }
                yield return null;

                if (_Debug) Debug.Log("[PTUtilities] Sprite " + sprite.name + "successfully faded to " + color);

            } else {
                if (_Debug) Debug.LogWarning("[PTUtilities] Sprite " + sprite.name + " already at color " + targetColor + ", cancelling fade");
            }

        }
        // Helper coroutine for fading the color of a sprite
        public IEnumerator FadeColorTo( MeshRenderer mRenderer, Color targetColor, float duration ) {

            Material mat = mRenderer.material;

            if (mat.GetColor("_Color") != targetColor) {

                if (_Debug) Debug.Log("[PTUtilities] Fading Sprite " + mRenderer.name + "to color " + targetColor);

                if (!mRenderer.enabled) {
                    mRenderer.enabled = true;
                }


                Color color = mat.GetColor("_Color");
                for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / duration) {
                    Color newColor = Color.Lerp(color, targetColor, t);
                    mat.SetColor("_Color", newColor);
                    yield return null;
                }
                mat.SetColor("_Color", targetColor);

                if (targetColor.a == 0f) {
                    mRenderer.enabled = false;
                }
                yield return null;

                if (_Debug) Debug.Log("[PTUtilities] MeshRenderer " + mRenderer.name + "successfully faded to " + color);

            } else {
                if (_Debug) Debug.LogWarning("[PTUtilities] MeshRenderer " + mRenderer.name + " already at color " + targetColor + ", cancelling fade");
            }

        }

        // Helper coroutine for fading audio source volume
        public IEnumerator FadeAudioTo( AudioSource audio, float targetVolume, float duration ) {
            float volume = audio.volume;
            if (!audio.isPlaying) {
                audio.Play();
            }
            for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / duration) {
                float newVolume = Mathf.Lerp(volume, targetVolume, t);
                audio.volume = newVolume;
                yield return null;
            }
            audio.volume = targetVolume;
            if (targetVolume == 0) {
                audio.Pause();
            }

        }



        // Fading audio

        bool fadingAudioListener;
        Coroutine fadeAudioListenerCoroutine;
        bool fadingResonanceListener;
        Coroutine fadeResonanceListenerCoroutine;

        /// <summary>
        /// Fades the volume of the Audio Listener to the target value over the duration
        /// </summary>
        /// <param name="volume">The target volume to fade to</param>
        /// <param name="duration">The duration of the fade in seconds</param>
        public void FadeAudioListener( float volume, float duration ) {

            // If an audio listener fade is already happening, cancel it and start the new one
            if (fadingAudioListener) StopCoroutine(fadeAudioListenerCoroutine);
            fadeAudioListenerCoroutine = StartCoroutine(FadeAudioListenerTo(volume, duration));

        }

        // Helper coroutine for fading audio listener volume
        IEnumerator FadeAudioListenerTo( float targetVolume, float duration ) {
            fadingAudioListener = true;


            float volume = AudioListener.volume;
            if (_Debug) Debug.Log("FadeAudioListenerTo Starting");

            for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / duration) {
                float newVolume = Mathf.Lerp(volume, targetVolume, t);
                AudioListener.volume = newVolume;

                if (_Debug) Debug.Log("AudioListener.volume = " + AudioListener.volume);
                yield return null;
            }
            AudioListener.volume = targetVolume;


            if (_Debug) Debug.Log("FadeAudioListenerTo Finished");
            fadingAudioListener = false;
        }

        IEnumerator FadeResonanceListenerTo( float targetVolume, float duration ) {
            fadingResonanceListener = true;

            float currentDB = 0; _ResonanceMaster.GetFloat("ResonanceMasterVolume", out currentDB);
            float targetDB = (targetVolume - 1) * 80;

            if (_Debug) Debug.Log("FadeAudioListenerTo Starting, currentDB = " + currentDB);

            for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / duration) {
                float newDB = Mathf.Lerp(currentDB, targetDB, t);
                _ResonanceMaster.SetFloat("ResonanceMasterVolume", newDB);
                if (_Debug) Debug.Log("Resonance Master Volume = " + newDB);
                yield return null;
            }
            _ResonanceMaster.SetFloat("ResonanceMasterVolume", targetDB);

            if (_Debug) Debug.Log("FadeAudioListenerTo Finished");
            fadingResonanceListener = false;
        }

        //bool flip = true;
        //void Update() {

        //    if (Input.GetKeyDown(KeyCode.M)) {
        //        if (flip) {
        //            FadeAudioListener(0f, 2f);
        //        } else {
        //            FadeAudioListener(1f, 2f);
        //        }
        //        flip = !flip;
        //    }

        //}

    }

}





// OLD FUNCTIONS


///// <summary>
///// Fades the volume of an Audio Source to the target value over the duration
///// </summary>
///// <param name="audioSource">The Audio Source to change</param>
///// <param name="volume">The target volume to fade to</param>
///// <param name="duration">The duration of the fade in seconds</param>
//public void FadeAudio( AudioSource audioSource, float volume, float duration ) {

//    StartCoroutine(FadeAudioTo(audioSource, volume, duration));

//}

///// <summary>
///// Fades the alpha of a sprite renderer to the target value over the duration
///// </summary>
///// <param name="sprite">The sprite renderer to change</param>
///// <param name="targetAlpha">The target alpha value to fade to</param>
///// <param name="duration">The duration of the fade in seconds</param>
//public void FadeAlpha( SpriteRenderer sprite, float targetAlpha, float duration ) {

//    if (_Debug) Debug.Log("[PTUtilities] Sprite " + sprite.name + "received");

//    StartCoroutine(FadeAlphaTo(sprite, targetAlpha, duration));

//}
///// <summary>
///// Fades the alpha of a TextMeshPro asset to the target value over the duration
///// </summary>
///// <param name="textmesh">The TextMeshPro asset to change</param>
///// <param name="targetAlpha">The target alpha value to fade to</param>
///// <param name="duration">The duration of the fade in seconds</param>
//public void FadeAlpha( TextMeshPro textmesh, float targetAlpha, float duration ) {

//    if (_Debug) Debug.Log("[PTUtilities] TextMesh " + textmesh.name + "received");

//    StartCoroutine(FadeAlphaTo(textmesh, targetAlpha, duration));

//}
///// <summary>
///// Fades the alpha of a mesh renderer's main material to the target value over the duration
///// </summary>
///// <param name="meshRenderer">The mesh renderer asset to change</param>
///// <param name="targetAlpha">The target alpha value to fade to</param>
///// <param name="duration">The duration of the fade in seconds</param>
//public void FadeAlpha( MeshRenderer meshRenderer, float targetAlpha, float duration ) {

//    if (_Debug) Debug.Log("[PTUtilities] Material " + meshRenderer.name + "received");

//    StartCoroutine(FadeAlphaTo(meshRenderer, targetAlpha, duration));

//}



///// <summary>
///// Fades the alpha of a sprite renderer to the target value over the duration
///// </summary>
///// <param name="sprite">The sprite renderer to change</param>
///// <param name="targetAlpha">The target alpha value to fade to</param>
///// <param name="duration">The duration of the fade in seconds</param>
//public void FadeColor( SpriteRenderer sprite, Color targetColor, float duration ) {

//    if (_Debug) Debug.Log("[PTUtilities] Sprite " + sprite.name + "received");

//    StartCoroutine(FadeColorTo(sprite, targetColor, duration));

//}
///// <summary>
///// Fades the alpha of a sprite renderer to the target value over the duration
///// </summary>
///// <param name="sprite">The sprite renderer to change</param>
///// <param name="targetAlpha">The target alpha value to fade to</param>
///// <param name="duration">The duration of the fade in seconds</param>
//public void FadeColor( SpriteRenderer sprite, Color targetColor, float duration, out Coroutine fadeCoroutine ) {

//    if (_Debug) Debug.Log("[PTUtilities] Sprite " + sprite.name + "received");

//    fadeCoroutine = StartCoroutine(FadeColorTo(sprite, targetColor, duration));

//}
///// <summary>
///// Fades the alpha of a sprite renderer to the target value over the duration
///// </summary>
///// <param name="sprite">The sprite renderer to change</param>
///// <param name="targetAlpha">The target alpha value to fade to</param>
///// <param name="duration">The duration of the fade in seconds</param>
//public void FadeColor( MeshRenderer meshRenderer, Color targetColor, float duration ) {

//    if (_Debug) Debug.Log("[PTUtilities] MeshRenderer " + meshRenderer.name + "received");

//    StartCoroutine(FadeColorTo(meshRenderer, targetColor, duration));

//}
///// <summary>
///// Fades the alpha of a sprite renderer to the target value over the duration
///// </summary>
///// <param name="sprite">The sprite renderer to change</param>
///// <param name="targetAlpha">The target alpha value to fade to</param>
///// <param name="duration">The duration of the fade in seconds</param>
//public void FadeColor( MeshRenderer meshRenderer, Color targetColor, float duration, out Coroutine fadeCoroutine ) {

//    if (_Debug) Debug.Log("[PTUtilities] MeshRenderer " + meshRenderer.name + "received");

//    fadeCoroutine = StartCoroutine(FadeColorTo(meshRenderer, targetColor, duration));

//}





