﻿using System.Collections;
using UnityEngine;
using TMPro;
using Paperticket;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class CrossSceneEventHelper : MonoBehaviour {

    [System.Serializable]
    public enum TrackingType { Head, LeftController, RightController, BetweenControllers }



    #region Careplays calls

    public void LoadCareScene( CareScene sceneToLoad ) {
        Debug.LogWarning("[CrossSceneEventHelper] Attempting to load new care scene: " + sceneToLoad.ToString());
        CareplaysManager.instance.LoadCareScene(sceneToLoad);
    }

    public void CompleteInductionModule( CareScene moduleToComplete ) {
        switch (moduleToComplete) {

            case CareScene.IN02_Choice:
                CareplaysManager.instance.IN01HonestyComplete = true;
                CareplaysManager.instance.IN01VideoIndex += 1;
                break;
            case CareScene.IN03_Reporting:
                CareplaysManager.instance.IN01ReportComplete = true;
                CareplaysManager.instance.IN01VideoIndex += 1;
                break;
            case CareScene.IN04_Cigarette:
                CareplaysManager.instance.IN01ChoiceComplete = true;
                CareplaysManager.instance.IN01VideoIndex += 1;
                break;
            case CareScene.IN05_Family:
                CareplaysManager.instance.IN01CulturalComplete = true;
                CareplaysManager.instance.IN01VideoIndex += 1;
                break;
            case CareScene.IN06_Privacy:
                CareplaysManager.instance.IN01PrivacyComplete = true;
                CareplaysManager.instance.IN01VideoIndex += 1;
                break;
            case CareScene.IN01_Modules:
            case CareScene.DesertMenu:
            case CareScene.WE01_Onboarding:
            case CareScene.WE02_Jetty:
            case CareScene.WE03_Dawn:
            case CareScene.WE04_Finale:
            default:
                Debug.LogError("CrossSceneEventHelper] ERROR -> Bad Induction module received: " + moduleToComplete);
                break;
        }
    }

    public void SetWE02VideoIndex(int index) {
        CareplaysManager.instance.WE02VideoIndex = index;
    }

    public void SetWE03VideoIndex( int index ) {
        CareplaysManager.instance.WE03VideoIndex = index;
    }

    public void ResetExperience() {
        CareplaysManager.instance.IN01ChoiceComplete = false;
        CareplaysManager.instance.IN01CulturalComplete = false;
        CareplaysManager.instance.IN01HonestyComplete = false;
        CareplaysManager.instance.IN01PrivacyComplete = false;
        CareplaysManager.instance.IN01ReportComplete = false;
        CareplaysManager.instance.IN01VideoIndex = 0;
        CareplaysManager.instance.WE02VideoIndex = 0;
        CareplaysManager.instance.WE03VideoIndex = 0;
    }


    #endregion

    #region Scene loading/unloading calls


    // NOTE: You shouldn't need to use these direct scene calls anymore
    // All loading/unloading is done with the above LoadCareScene function
    // (This also applies to the bundle loading/unloading further down)

    public void SwitchToScene( string sceneName, float invokeTime ) {
        StartCoroutine(WaitThenSwitchToScene(sceneName, invokeTime));
    }
    IEnumerator WaitThenSwitchToScene( string sceneName, float invokeTime ) {
        yield return new WaitForSeconds(invokeTime);
        SwitchToScene(sceneName);
    }

    public void SwitchToScene(string sceneName ) {
        SceneUtilities.instance.LoadSceneExclusive(sceneName);
    }



    public void LoadNextScene (string sceneName, float invokeTime ) {
        StartCoroutine(WaitThenLoadNextScene(sceneName, invokeTime));
    }
    IEnumerator WaitThenLoadNextScene( string sceneName, float invokeTime ) {
        yield return new WaitForSeconds(invokeTime);
        LoadNextScene(sceneName);
    }


    public void LoadNextScene( string sceneName ) {
        SceneUtilities.instance.BeginLoadScene(sceneName);
        SceneUtilities.OnSceneAlmostReady += LoadSceneCallback;

    }
    void LoadSceneCallback() {

        SceneUtilities.OnSceneAlmostReady -= LoadSceneCallback;
        SceneUtilities.instance.FinishLoadScene(true);
        SceneUtilities.instance.UnloadScene(gameObject.scene.name);

    }


    #endregion


    #region Headset / controller calls

    public void SetControllerBeam( bool toggle ) {
        PTUtilities.instance.ControllerBeamActive = toggle;
    }

    public void SetControllerInteractionLayers (LayerMask layerMask ) {
        PTUtilities.instance.ControllerBeamLayerMask = layerMask;
    }

    public void DoHaptics( Hand hand, float strength, float duration ) {

        //PTUtilities.instance.DoHaptics(hand, strength, duration);
        StartCoroutine(OculusUtilities.instance.DoingHaptics(strength, strength, duration, hand));

    }




    public void MatchHeadsetTransform (Transform target ) {

        target.position = PTUtilities.instance.HeadsetPosition();
        target.rotation = PTUtilities.instance.HeadsetRotation();

    }



    Coroutine headFadeCo;
    public void FadeHeadsetColor( Color color, float duration ) {
        if (headFadeCo != null) StopCoroutine(headFadeCo);
        headFadeCo = StartCoroutine(PTUtilities.instance.FadeColorTo(PTUtilities.instance.headGfx, color, duration, TimeScale.Scaled));
    }

    public void FadeHeadsetIn( float duration ) {
        if (headFadeCo != null) StopCoroutine(headFadeCo);
        headFadeCo = StartCoroutine(PTUtilities.instance.FadeAlphaTo(PTUtilities.instance.headGfx, 0f, duration, TimeScale.Scaled));
    }

    public void FadeHeadsetOut( float duration ) {
        if (headFadeCo != null) StopCoroutine(headFadeCo);
        headFadeCo = StartCoroutine(PTUtilities.instance.FadeAlphaTo(PTUtilities.instance.headGfx, 1f, duration, TimeScale.Scaled));
    }
       

    public void FadeHeadsetToBlack( float duration ) {
        FadeHeadsetColor(Color.black, duration);
    }

    public void FadeHeadsetToWhite( float duration ) {
        FadeHeadsetColor(Color.white, duration);
    }


    public void UnscaledFadeHeadsetColor( Color color, float duration ) {
        if (headFadeCo != null) StopCoroutine(headFadeCo);
        headFadeCo = StartCoroutine(PTUtilities.instance.FadeColorTo(PTUtilities.instance.headGfx, color, duration, TimeScale.Unscaled));
    }





    public void TeleportPlayer( Vector3 worldPosition, Vector3 forwardDirection ) {
        PTUtilities.instance.TeleportPlayer(worldPosition, forwardDirection);
    }

    public void TeleportPlayer( Transform targetTransform ) {
        PTUtilities.instance.TeleportPlayer(targetTransform);
    }
    public void TeleportPlayer( Transform targetTransform, bool rotatePlayer ) {
        PTUtilities.instance.TeleportPlayer(targetTransform, rotatePlayer);
    }


    #endregion



    #region Oculus calls







    #endregion



    #region General purpose calls


    public void DestroyGameObject( GameObject objectToDestroy ) {
        Destroy(objectToDestroy);
    }

    public void DestroyComponent (Component componentToDestroy ) {
        Destroy(componentToDestroy);
    }


    public void ShakeTransform( Transform target, Vector3 shakeAmount, float duration) {
        StartCoroutine(PTUtilities.instance.ShakeTransform(target, shakeAmount, duration, TimeScale.Scaled));
    }

    public enum CurveType { Constant, Linear, EaseInOut, EaseIn, EaseOut }

    public void MoveTransformViaCurve( Transform target, CurveType curveType, Vector3 moveAmount, float duration ) {

        AnimationCurve curve = new AnimationCurve();

        switch (curveType) {
            case CurveType.Constant:
                curve = AnimationCurve.Constant(0, 1, 1);
                break;
            case CurveType.Linear:
                curve = AnimationCurve.Linear(0, 0, 1, 1);
                break;
            case CurveType.EaseInOut:
                curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
                break;
            case CurveType.EaseIn:
                curve = PTUtilities.instance.easeInCurve;
                break;
            case CurveType.EaseOut:
                curve = PTUtilities.instance.easeOutCurve;
                break;
            default:
                Debug.LogError("[{0}] ERROR -> Bad CurveType recieved in MoveTransformViaCurve!!");
                break;
        }

        StartCoroutine(PTUtilities.instance.MoveTransformViaCurve (target, curve, moveAmount, duration, TimeScale.Scaled));
        
    }

    public void ScaleTransformViaCurve( Transform target, CurveType curveType, Vector3 scaleAmount, float duration ) {

        AnimationCurve curve = new AnimationCurve();

        switch (curveType) {
            case CurveType.Constant:
                curve = AnimationCurve.Constant(0, 1, 1);
                break;
            case CurveType.Linear:
                curve = AnimationCurve.Linear(0, 0, 1, 1);
                break;
            case CurveType.EaseInOut:
                curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
                break;
            case CurveType.EaseIn:
                curve = PTUtilities.instance.easeInCurve;
                break;
            case CurveType.EaseOut:
                curve = PTUtilities.instance.easeOutCurve;
                break;
            default:
                Debug.LogError("[{0}] ERROR -> Bad CurveType recieved in ScaleTransformViaCurve!!");
                break;
        }

        StartCoroutine(PTUtilities.instance.ScaleTransformViaCurve(target, curve, scaleAmount, duration, TimeScale.Scaled));

    }

    public void RotateTransformViaCurve( Transform target, CurveType curveType, Vector3 rotateAmount, float duration ) {

        AnimationCurve curve = new AnimationCurve();

        switch (curveType) {
            case CurveType.Constant:
                curve = AnimationCurve.Constant(0, 1, 1);
                break;
            case CurveType.Linear:
                curve = AnimationCurve.Linear(0, 0, 1, 1);
                break;
            case CurveType.EaseInOut:
                curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
                break;
            case CurveType.EaseIn:
                curve = PTUtilities.instance.easeInCurve;
                break;
            case CurveType.EaseOut:
                curve = PTUtilities.instance.easeOutCurve;
                break;
            default:
                Debug.LogError("[{0}] ERROR -> Bad CurveType recieved in RotateTransformViaCurve!!");
                break;
        }

        StartCoroutine(PTUtilities.instance.RotateTransformViaCurve(target, curve, rotateAmount, duration, TimeScale.Scaled));

    }



    public void UnscaledMoveTransformViaCurve( Transform target, CurveType curveType, Vector3 moveAmount, float duration ) {

        AnimationCurve curve = new AnimationCurve();

        switch (curveType) {
            case CurveType.Constant:
                curve = AnimationCurve.Constant(0, 1, 1);
                break;
            case CurveType.Linear:
                curve = AnimationCurve.Linear(0, 0, 1, 1);
                break;
            case CurveType.EaseInOut:
                curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
                break;
            case CurveType.EaseIn:
                curve = PTUtilities.instance.easeInCurve;
                break;
            case CurveType.EaseOut:
                curve = PTUtilities.instance.easeOutCurve;
                break;
            default:
                Debug.LogError("[{0}] ERROR -> Bad CurveType recieved in MoveTransformViaCurve!!");
                break;
        }

        StartCoroutine(PTUtilities.instance.MoveTransformViaCurve(target, curve, moveAmount, duration, TimeScale.Unscaled));

    }

    public void UnscaledScaleTransformViaCurve( Transform target, CurveType curveType, Vector3 scaleAmount, float duration ) {

        AnimationCurve curve = new AnimationCurve();

        switch (curveType) {
            case CurveType.Constant:
                curve = AnimationCurve.Constant(0, 1, 1);
                break;
            case CurveType.Linear:
                curve = AnimationCurve.Linear(0, 0, 1, 1);
                break;
            case CurveType.EaseInOut:
                curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
                break;
            case CurveType.EaseIn:
                curve = PTUtilities.instance.easeInCurve;
                break;
            case CurveType.EaseOut:
                curve = PTUtilities.instance.easeOutCurve;
                break;
            default:
                Debug.LogError("[{0}] ERROR -> Bad CurveType recieved in ScaleTransformViaCurve!!");
                break;
        }

        StartCoroutine(PTUtilities.instance.ScaleTransformViaCurve(target, curve, scaleAmount, duration, TimeScale.Unscaled));

    }

    public void UnscaledRotateTransformViaCurve( Transform target, CurveType curveType, Vector3 rotateAmount, float duration ) {

        AnimationCurve curve = new AnimationCurve();

        switch (curveType) {
            case CurveType.Constant:
                curve = AnimationCurve.Constant(0, 1, 1);
                break;
            case CurveType.Linear:
                curve = AnimationCurve.Linear(0, 0, 1, 1);
                break;
            case CurveType.EaseInOut:
                curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
                break;
            case CurveType.EaseIn:
                curve = PTUtilities.instance.easeInCurve;
                break;
            case CurveType.EaseOut:
                curve = PTUtilities.instance.easeOutCurve;
                break;
            default:
                Debug.LogError("[{0}] ERROR -> Bad CurveType recieved in RotateTransformViaCurve!!");
                break;
        }

        StartCoroutine(PTUtilities.instance.RotateTransformViaCurve(target, curve, rotateAmount, duration, TimeScale.Unscaled));

    }




    public void TeleportGameObject( GameObject targetObject, Transform targetTransform ) {
        targetObject.transform.position = targetTransform.position;
        targetObject.transform.rotation = targetTransform.rotation;
    }

    public void TeleportGameObject( GameObject targetObject, Transform targetTransform, bool rotateObject ) {
        targetObject.transform.position = targetTransform.position;
        if (rotateObject) targetObject.transform.rotation = targetTransform.rotation;
    }



    public void SetTimeScale (float timeScale ) {
        PTUtilities.instance.TimeScale = timeScale;
    }


    #endregion





    #region Mesh calls
    public void FadeMeshIn( MeshRenderer mesh, float duration ) {
        StartCoroutine(PTUtilities.instance.FadeAlphaTo(mesh, 1, duration, TimeScale.Scaled));
    }
    public void FadeMeshOut( MeshRenderer mesh, float duration ) {
        StartCoroutine(PTUtilities.instance.FadeAlphaTo(mesh, 0, duration, TimeScale.Scaled));
    }
    public void FadeMesh( MeshRenderer mesh, float alpha, float duration ) {
        StartCoroutine(PTUtilities.instance.FadeAlphaTo(mesh, alpha, duration, TimeScale.Scaled));
    }
    public void FadeMeshColor( MeshRenderer mesh, Color color, float duration ) {
        StartCoroutine(PTUtilities.instance.FadeColorTo(mesh, color, duration, TimeScale.Scaled));
    }

    public void SetMeshAlpha( MeshRenderer mesh, float alpha ) {

        Material mat = mesh.material;

        string propertyName = mat.HasProperty("_BaseColor") ? "_BaseColor" : mat.HasProperty("_Color") ? "_Color" : "";
        if (propertyName == "") {
            Debug.LogError("[CrossSceneEventHelper] ERROR -> Could not find property name of material! Cancelling set mesh alpha.");
            return;
        }

        if (mat.GetColor(propertyName).a != alpha) {
            Color col = mat.GetColor(propertyName);
            mat.SetColor(propertyName, new Color(col.r, col.g, col.b, alpha));
        }

        if (alpha == 0) mesh.enabled = false;
        else mesh.enabled = true;


    }



    public void UnscaledFadeMesh( MeshRenderer mesh, float alpha, float duration ) {
        StartCoroutine(PTUtilities.instance.FadeAlphaTo(mesh, alpha, duration, TimeScale.Unscaled));
    }
    public void UnscaledFadeMeshColor( MeshRenderer mesh, Color color, float duration ) {
        StartCoroutine(PTUtilities.instance.FadeColorTo(mesh, color, duration, TimeScale.Unscaled));
    }


    #endregion


    #region Sprite calls



    public void FadeSpriteIn( SpriteRenderer sprite ) {
        StartCoroutine(PTUtilities.instance.FadeAlphaTo(sprite, 1, 1.5f, TimeScale.Scaled));
    }
    public void FadeSpriteOut( SpriteRenderer sprite ) {
        StartCoroutine(PTUtilities.instance.FadeAlphaTo(sprite, 0, 1.5f, TimeScale.Scaled));
    }

    public void FadeSpriteIn( SpriteRenderer sprite, float duration ) {
        StartCoroutine(PTUtilities.instance.FadeAlphaTo(sprite, 1, duration, TimeScale.Scaled));
    }
    public void FadeSpriteOut( SpriteRenderer sprite, float duration ) {
        StartCoroutine(PTUtilities.instance.FadeAlphaTo(sprite, 0, duration, TimeScale.Scaled));
    }

    public void FadeSprite( SpriteRenderer sprite, float alpha, float duration ) {
        StartCoroutine(PTUtilities.instance.FadeAlphaTo(sprite, alpha, duration, TimeScale.Scaled));
    }
    public void FadeSpriteColor( SpriteRenderer sprite, Color color, float duration ) {
        StartCoroutine(PTUtilities.instance.FadeColorTo(sprite, color, duration, TimeScale.Scaled));
    }


    public void UnscaledFadeSprite( SpriteRenderer sprite, float alpha, float duration ) {
        StartCoroutine(PTUtilities.instance.FadeAlphaTo(sprite, alpha, duration, TimeScale.Unscaled));
    }
    public void UnscaledFadeSpriteColor( SpriteRenderer sprite, Color color, float duration ) {
        StartCoroutine(PTUtilities.instance.FadeColorTo(sprite, color, duration, TimeScale.Unscaled));
    }


    #endregion


    #region Text calls

    public void FadeTextIn( TextMeshPro text ) {
        StartCoroutine(PTUtilities.instance.FadeAlphaTo(text, 1, 1.5f, TimeScale.Scaled));
    }

    public void FadeTextOut( TextMeshPro text ) {
        StartCoroutine(PTUtilities.instance.FadeAlphaTo(text, 0, 1.5f, TimeScale.Scaled));
    }
    public void FadeText( TextMeshPro text, float alpha, float duration ) {
        StartCoroutine(PTUtilities.instance.FadeAlphaTo(text, alpha, duration, TimeScale.Scaled));
    }
    public void FadeTextColor( TextMeshPro text, Color color, float duration ) {
        StartCoroutine(PTUtilities.instance.FadeColorTo(text, color, duration, TimeScale.Scaled));
    }



    public void UnscaledFadeText( TextMeshPro text, float alpha, float duration ) {
        StartCoroutine(PTUtilities.instance.FadeAlphaTo(text, alpha, duration, TimeScale.Unscaled));
    }
    public void UnscaledFadeTextColor( TextMeshPro text, Color color, float duration ) {
        StartCoroutine(PTUtilities.instance.FadeColorTo(text, color, duration, TimeScale.Unscaled));
    }

    #endregion


    #region UI Image calls 


    public void FadeImageIn( Image image, float duration ) {
        StartCoroutine(PTUtilities.instance.FadeAlphaTo(image, 1, duration, TimeScale.Scaled));
    }
    public void FadeImageOut( Image image, float duration ) {
        StartCoroutine(PTUtilities.instance.FadeAlphaTo(image, 0, duration, TimeScale.Scaled));
    }
    public void FadeImage( Image image, float alpha, float duration ) {
        StartCoroutine(PTUtilities.instance.FadeAlphaTo(image, alpha, duration, TimeScale.Scaled));
    }
    public void FadeImageColor( Image image, Color color, float duration ) {
        StartCoroutine(PTUtilities.instance.FadeColorTo(image, color, duration, TimeScale.Scaled));
    }


    public void UnscaledFadeImage( Image image, float alpha, float duration ) {
        StartCoroutine(PTUtilities.instance.FadeAlphaTo(image, alpha, duration, TimeScale.Unscaled));
    }
    public void UnscaledFadeImageColor( Image image, Color color, float duration ) {
        StartCoroutine(PTUtilities.instance.FadeColorTo(image, color, duration, TimeScale.Unscaled));
    }


    #endregion


    #region Volume calls


    public void FadePostVolume( Volume volume, float targetWeight, float duration ) {
        StartCoroutine(PTUtilities.instance.FadePostVolumeTo(volume, targetWeight, duration, TimeScale.Scaled));
    }


    public void UnscaledFadePostVolume( Volume volume, float targetWeight, float duration ) {
        StartCoroutine(PTUtilities.instance.FadePostVolumeTo(volume, targetWeight, duration, TimeScale.Unscaled));
    }


    #endregion




    #region Audio calls

    public void PlayAudioClip( AudioClip clip, float volume ) {
        PTUtilities.instance.PlayAudioClip(clip, volume);
    }


    public void PlayAudioClip( AudioClip clip, Vector3 worldPosition, float volume ) {
        AudioSource.PlayClipAtPoint(clip, worldPosition, volume);

    }

    public void PlayAudioClip( AudioClip clip, Transform worldPosition, float volume ) {
        PlayAudioClip(clip, worldPosition.position, volume);
    }



    public void PlayAudioClip( AudioClip clip, Vector3 worldPosition, float volume, float minDistance, float maxDistance ) {

        AudioSource tempAudio = new GameObject("[tempaudio]", typeof(AudioSource)).GetComponent<AudioSource>();
        tempAudio.transform.position = worldPosition;

        tempAudio.clip = clip;
        tempAudio.spatialBlend = 1.0f;
        tempAudio.volume = volume;
        tempAudio.minDistance = minDistance;
        tempAudio.maxDistance = maxDistance;
        //tempAudio.outputAudioMixerGroup = PTUtilities.instance.audioMaster.FindMatchingGroups("Master")[0];

        tempAudio.Play();

        Destroy(tempAudio.gameObject, clip.length);
    }

    public void PlayAudioClip(AudioClip clip, Transform worldPosition, float volume, float minDistance, float maxDistance ) {
        PlayAudioClip(clip, worldPosition.position, volume, minDistance, maxDistance);
    }

    public void PlayAudioClip( AudioClip clip, TrackingType trackingType, float volume, float minDistance, float maxDistance ) {

        Vector3 position;
        switch (trackingType) {
            case TrackingType.Head:
                position = PTUtilities.instance.HeadsetPosition();
                break;
            case TrackingType.LeftController:
                position = PTUtilities.instance.leftController.transform.position;
                break;
            case TrackingType.RightController:
                position = PTUtilities.instance.rightController.transform.position;
                break;
            case TrackingType.BetweenControllers:
                position = PTUtilities.instance.rightController.transform.position + 
                                                (PTUtilities.instance.leftController.transform.position -
                                                    PTUtilities.instance.rightController.transform.position) / 2f;
                break;
            default:
                Debug.LogError("[CrossSceneEventHelper] ERROR -> Bad TrackingType passed into PlayAudioClip! Cancelling.");
                return;
        }

        PlayAudioClip(clip, position, volume, minDistance, maxDistance);

    }






    public void FadeAudioSourceIn( AudioSource source ) {
        StartCoroutine(PTUtilities.instance.FadeAudioTo(source, 1f, 0.5f, TimeScale.Scaled));
    }

    public void FadeAudioSourceOut( AudioSource source ) {
        StartCoroutine(PTUtilities.instance.FadeAudioTo(source, 0f, 0.5f, TimeScale.Scaled));
    }
    public void FadeAudioSource( AudioSource source, float volume, float duration ) {
        StartCoroutine(PTUtilities.instance.FadeAudioTo(source, volume, duration, TimeScale.Scaled));
    }


    public void UnscaledFadeAudioSource( AudioSource source, float volume, float duration ) {
        StartCoroutine(PTUtilities.instance.FadeAudioTo(source, volume, duration, TimeScale.Unscaled));
    }




    Coroutine mixerFadeCo;
    public void FadeAudioMasterIn( float duration ) {
        if (mixerFadeCo != null) StopCoroutine(mixerFadeCo);
        mixerFadeCo = StartCoroutine(PTUtilities.instance.FadeAudioMasterTo(1, duration, TimeScale.Scaled));
    }

    public void FadeAudioMasterOut( float duration ) {
        if (mixerFadeCo != null) StopCoroutine(mixerFadeCo);
        mixerFadeCo = StartCoroutine(PTUtilities.instance.FadeAudioMasterTo(0, duration, TimeScale.Scaled));
    }

    public void FadeAudioMaster (float volume, float duration ) {
        if (mixerFadeCo != null) StopCoroutine(mixerFadeCo);
        mixerFadeCo = StartCoroutine(PTUtilities.instance.FadeAudioMasterTo(volume, duration, TimeScale.Scaled));
    }



    public void UnscaledFadeAudioMaster( float volume, float duration ) {
        if (mixerFadeCo != null) StopCoroutine(mixerFadeCo);
        mixerFadeCo = StartCoroutine(PTUtilities.instance.FadeAudioMasterTo(volume, duration, TimeScale.Unscaled));
    }

    #endregion


    #region Asset bundle calls



    public void LoadAssetBundle(AssetBundles assetBundle ) {
        if (DataUtilities.instance.isBundleLoaded(assetBundle)) {
            Debug.Log("[CrossSceneEventHelper] AssetBundle '"+assetBundle+"' was already loaded, disregarding load request.");
            return;
        }
        DataUtilities.instance.LoadAssetBundle(assetBundle);
    }

    public void UnloadAssetBundle( AssetBundles assetBundle, bool unloadAllLoadedAssets ) {
        if (!DataUtilities.instance.isBundleLoaded(assetBundle)) {
            Debug.Log("[CrossSceneEventHelper] AssetBundle '" + assetBundle + "' is not loaded, disregarding unload request.");
            return;
        }
        DataUtilities.instance.UnloadAssetBundle(assetBundle, unloadAllLoadedAssets);
    }

    #endregion

}
