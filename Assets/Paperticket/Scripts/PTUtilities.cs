using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;
using System;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Paperticket {

    public enum GameEventOption { OnButtonDown, OnButtonUp, OnTriggerPress, OnTriggerUp }
    public enum Hand { Left, Right }

    public class PTUtilities : MonoBehaviour {

        [Header("References")]

        public static PTUtilities instance = null;

        public AudioMixer _ResonanceMaster;

        public XRRig playerRig;

        public SpriteRenderer headGfx;



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

        [Space(10)]

        [SerializeField] AnimationCurve shakeTransformCurve;

        

        [Header("Read Only")]

        public Transform headProxy;
        public XRController rightController;
        public XRController leftController;
        XRRayInteractor rightRayInteractor;
        XRInteractorLineVisual rightRayVisual;


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


        public bool ControllerBeamActive {
            get { 
                return rightRayInteractor.enabled; 
            }
            set {
                rightRayInteractor.enabled = value;
                rightRayVisual.enabled = value; 
            }
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


            // Grab the right controller
            while (rightController == null) {
                if (_Debug) Debug.Log("[PTUtilities] Looking for Right Controller object...");
                foreach (XRController cont in playerRig.GetComponentsInChildren<XRController>()) {
                    if (cont.controllerNode == XRNode.RightHand) rightController = cont;
                }
                yield return null;
            }
            if (_Debug) Debug.Log("[PTUtilities] Right Controller found!");

            // Grab the left controller
            while (leftController == null) {
                if (_Debug) Debug.Log("[PTUtilities] Looking for Left Controller object...");
                foreach (XRController cont in playerRig.GetComponentsInChildren<XRController>()) {
                    if (cont.controllerNode == XRNode.LeftHand) leftController = cont;
                }
                yield return null;
            }
            if (_Debug) Debug.Log("[PTUtilities] Left Controller found!");

            // Grab the right controller's XR ray interactor            
            while (rightRayInteractor == null) {
                if (_Debug) Debug.Log("[PTUtilities] Looking for Right Ray Interactor object...");
                rightRayInteractor = rightController.GetComponent<XRRayInteractor>();
                yield return null;
            }
            if (_Debug) Debug.Log("[PTUtilities] Right Ray Interactor found!");

            // Grab the right controller's XR ray visual          
            while (rightRayVisual == null) {
                if (_Debug) Debug.Log("[PTUtilities] Looking for Right Ray Visual object...");
                rightRayVisual = rightController.GetComponent<XRInteractorLineVisual>();
                yield return null;
            }
            if (_Debug) Debug.Log("[PTUtilities] Right Ray Visual found!");


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

            if (rightController.inputDevice.TryGetFeatureValue(CommonUsages.triggerButton, out ControllerTriggerButton) && ControllerTriggerButton) {                
                if (!lastTriggerState) {
                    // send event          
                    if (_Debug) Debug.Log("[PTUtilities] Trigger button was just pressed.");
                }
            }
            lastTriggerState = ControllerTriggerButton;


            if (rightController.inputDevice.TryGetFeatureValue(CommonUsages.primaryButton, out ControllerPrimaryButton) && ControllerPrimaryButton) {
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
        /// Teleport the player to the specified position and rotation, taking into account camera position/rotation in rig
        /// </summary>
        /// <param name="position">The position to teleport to</param>
        /// <param name="forwardDirection">The foward direction to teleport to</param>
        public void TeleportPlayer( Vector3 position, Vector3 forwardDirection ) {
            
            playerRig.MoveCameraToWorldLocation(position);
            playerRig.MatchRigUpCameraForward(Vector3.up, forwardDirection);

        }
        /// <summary>
        /// Teleport the player to the specified position, taking into account camera position in rig
        /// </summary>
        /// <param name="position">The position to teleport to</param>
        public void TeleportPlayer( Vector3 position) {
            playerRig.MoveCameraToWorldLocation(position);
        }
        /// <summary>
        /// Teleport the player to match the specified transform, taking into account camera position in rig
        /// </summary>
        /// <param name="target">The transform to teleport to</param>
        public void TeleportPlayer (Transform target ) {
            TeleportPlayer(target.position, target.forward);
        }
        /// <summary>
        /// Teleport the player to match the specified transform, taking into account camera position in rig
        /// </summary>
        /// <param name="target">The transform to teleport to</param>
        /// <param name="rotatePlayer">Whether to rotate the player to match the transform or not</param>
        public void TeleportPlayer( Transform target, bool rotatePlayer) {
            if (rotatePlayer) TeleportPlayer(target.position, target.forward);
            else TeleportPlayer(target.position);
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

            string propertyName = "";
            propertyName = mat.HasProperty("_BaseColor") ? "_BaseColor" : mat.HasProperty("_Color") ? "_Color" : "";
            if (propertyName == "") {
                Debug.LogError("[PTUtilities] ERROR -> Could not find property name of mesh renderer to fade! Cancelling...");
            }

            Color col = mat.GetColor(propertyName);

            if (col.a != targetAlpha) {

                if (_Debug) Debug.Log("[PTUtilities] Fading MeshRenderer " + mRenderer.name + "to alpha " + targetAlpha);

                if (!mRenderer.enabled) {
                    mRenderer.enabled = true;
                }

                float alpha = col.a;
                for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / duration) {
                    Color newColor = new Color(col.r, col.g, col.b, Mathf.Lerp(alpha, targetAlpha, t));
                    mat.SetColor(propertyName, newColor);
                    yield return null;
                }
                mat.SetColor(propertyName, new Color(col.r, col.g, col.b, targetAlpha));


                if (targetAlpha == 0f) {
                    mRenderer.enabled = false;
                }
                yield return null;

                if (_Debug) Debug.Log("[PTUtilities] MeshRenderer " + mRenderer.name + " successfully faded to alpha " + targetAlpha);

            } else {
                if (_Debug) Debug.LogWarning("[PTUtilities] MeshRenderer " + mRenderer.name + " already at alpha " + targetAlpha + ", cancelling fade");
            }
        }
        // Helper coroutine for fading the alpha of an image
        public IEnumerator FadeAlphaTo( Image image, float targetAlpha, float duration ) {

            if (image.color.a != targetAlpha) {

                if (_Debug) Debug.Log("[PTUtilities] Fading Image " + image.name + " to alpha " + targetAlpha);

                if (!image.enabled) {
                    image.enabled = true;
                }

                float alpha = image.color.a;
                for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / duration) {
                    image.color = new Color(image.color.r, image.color.g, image.color.b, Mathf.Lerp(alpha, targetAlpha, t));
                    yield return null;
                }
                image.color = new Color(image.color.r, image.color.g, image.color.b, targetAlpha);

                if (targetAlpha == 0f) {
                    image.enabled = false;
                }
                yield return null;

                if (_Debug) Debug.Log("[PTUtilities] Image " + image.name + " successfully faded to alpha " + alpha);

            } else {
                if (_Debug) Debug.LogWarning("[PTUtilities] Image " + image.name + " already at alpha " + targetAlpha + ", cancelling fade");
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

            string propertyName = "";            
            propertyName = mat.HasProperty("_Color") ? "_Color" : mat.HasProperty("_BaseColor") ? "_BaseColor" : "";
            if (propertyName == "") {
                Debug.LogError("[PTUtilities] ERROR -> Could not find property name of mesh renderer to fade! Cancelling...");
            }

            if (mat.GetColor(propertyName) != targetColor) {

                if (_Debug) Debug.Log("[PTUtilities] Fading Sprite " + mRenderer.name + "to color " + targetColor);

                if (!mRenderer.enabled) {
                    mRenderer.enabled = true;
                }


                Color color = mat.GetColor(propertyName);
                for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / duration) {
                    Color newColor = Color.Lerp(color, targetColor, t);
                    mat.SetColor(propertyName, newColor);
                    yield return null;
                }
                mat.SetColor(propertyName, targetColor);

                if (targetColor.a == 0f) {
                    mRenderer.enabled = false;
                }
                yield return null;

                if (_Debug) Debug.Log("[PTUtilities] MeshRenderer " + mRenderer.name + "successfully faded to " + color);

            } else {
                if (_Debug) Debug.LogWarning("[PTUtilities] MeshRenderer " + mRenderer.name + " already at color " + targetColor + ", cancelling fade");
            }

        }

        // Helper coroutine for fading the color of a text mesh
        public IEnumerator FadeColorTo( TextMeshPro textMesh, Color targetColor, float duration ) {

            if (textMesh.color != targetColor) {

                if (_Debug) Debug.Log("[PTUtilities] Fading Text " + textMesh.name + "to color " + targetColor);

                if (!textMesh.enabled) {
                    textMesh.enabled = true;
                }

                Color color = textMesh.color;
                for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / duration) {
                    textMesh.color = Color.Lerp(color, targetColor, t);
                    yield return null;
                }
                textMesh.color = targetColor;

                if (targetColor.a == 0f) {
                    textMesh.enabled = false;
                }
                yield return null;

                if (_Debug) Debug.Log("[PTUtilities] Text " + textMesh.name + "successfully faded to " + color);

            } else {
                if (_Debug) Debug.LogWarning("[PTUtilities] Text " + textMesh.name + " already at color " + targetColor + ", cancelling fade");
            }

        }
        // Helper coroutine for fading the color of an image
        public IEnumerator FadeColorTo( Image image, Color targetColor, float duration ) {

            if (image.color != targetColor) {

                if (_Debug) Debug.Log("[PTUtilities] Fading Image " + image.name + "to color " + targetColor);

                if (!image.enabled) {
                    image.enabled = true;
                }

                Color color = image.color;
                for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / duration) {
                    image.color = Color.Lerp(color, targetColor, t);
                    yield return null;
                }
                image.color = targetColor;

                if (targetColor.a == 0f) {
                    image.enabled = false;
                }
                yield return null;

                if (_Debug) Debug.Log("[PTUtilities] Image " + image.name + "successfully faded to " + color);

            } else {
                if (_Debug) Debug.LogWarning("[PTUtilities] Image " + image.name + " already at color " + targetColor + ", cancelling fade");
            }

        }


        // Helper coroutine for fading volume weight
        public IEnumerator FadePostVolumeTo( Volume volume, float targetWeight, float duration ) {
            float weight = volume.weight;
            targetWeight = Mathf.Clamp01(targetWeight);

            if (!volume.enabled) volume.enabled = true;


            for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / duration) {
                float newWeight = Mathf.Lerp(weight, targetWeight, t);
                volume.weight = newWeight;
                yield return null;
            }
            volume.weight = targetWeight;
            if (targetWeight == 0) {
                volume.enabled = false;
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

        public IEnumerator ShakeTransform (Transform target, Vector3 shakeAmount, float duration ) {
            
            float t = 0;
            Vector3 initialPos = target.localPosition;
            //float curveTime = shakeTransformCurve.keys[shakeTransformCurve.length - 1].time;

            if (_Debug) Debug.Log("[PTUtilities] Shaking transform " + target.name);

            while (t < duration) {
                yield return null;
                target.localPosition = initialPos + (shakeAmount * shakeTransformCurve.Evaluate(t / duration));
                t += Time.deltaTime;
            }
            target.localPosition = initialPos;

            if (_Debug) Debug.Log("[PTUtilities] Finished shaking " + target.name);

        }

        public IEnumerator MoveTransformViaCurve( Transform target, AnimationCurve animCurve, Vector3 moveAmount, float duration ) {

            float t = 0;
            Vector3 initialPos = target.localPosition;
            //float curveTime = shakeTransformCurve.keys[shakeTransformCurve.length - 1].time;

            if (_Debug) Debug.Log("[PTUtilities] Moving transform " + target.name);

            while (t < duration) {
                yield return null;
                target.localPosition = initialPos + (moveAmount * animCurve.Evaluate(t / duration));
                t += Time.deltaTime;
            }
            target.localPosition = initialPos + moveAmount;

            if (_Debug) Debug.Log("[PTUtilities] Finished moving " + target.name);

        }

        public IEnumerator ScaleTransformViaCurve( Transform target, AnimationCurve animCurve, Vector3 scaleAmount, float duration ) {

            float t = 0;
            Vector3 initialScale = target.localScale;
            Vector3 finalScale = Vector3.Scale(initialScale, scaleAmount);
            //float curveTime = shakeTransformCurve.keys[shakeTransformCurve.length - 1].time;

            if (_Debug) Debug.Log("[PTUtilities] Scaling transform " + target.name);

            while (t < duration) {
                yield return null;
                //target.localScale = initialScale + Vector3.Scale(initialScale, scaleAmount) * animCurve.Evaluate(t / duration);
                target.localScale = Vector3.Lerp(initialScale, finalScale, animCurve.Evaluate(t / duration));
                t += Time.deltaTime;
            }
            target.localScale = finalScale;

            if (_Debug) Debug.Log("[PTUtilities] Finished scaling " + target.name);

        }


        public IEnumerator RotateTransformViaCurve ( Transform target, AnimationCurve animCurve, Vector3 rotateAmount, float duration) {

            float t = 0;
            Vector3 initialRot = target.localRotation.eulerAngles;

            if (_Debug) Debug.Log("[PTUtilities] Rotating transform " + target.name);

            while (t < duration) {
                yield return null;
                target.localRotation = Quaternion.Euler (initialRot + (rotateAmount * animCurve.Evaluate(t / duration)));
                t += Time.deltaTime;
            }
            target.localRotation = Quaternion.Euler(initialRot + rotateAmount);

            if (_Debug) Debug.Log("[PTUtilities] Finished rotating " + target.name);

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

    }

    [Serializable]
    public class ProgressEvent  {

        public string name;
        [Range(0, 1)] public float threshold;
        public UnityEvent2 progressEvent;

        public ProgressEvent( float progressThreshold, UnityEvent2 eventToSend ) {
            name = progressThreshold.ToString();
            threshold = progressThreshold;
            progressEvent = eventToSend;
        }

    }


}

