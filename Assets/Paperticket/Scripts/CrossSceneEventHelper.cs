using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Paperticket;

public class CrossSceneEventHelper : MonoBehaviour
{
           
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

    public void SetControllerBeam( bool toggle ) {
        PTUtilities.instance.ControllerBeamActive = toggle;
    }


    public void MatchHeadsetTransform (Transform target ) {

        target.position = PTUtilities.instance.HeadsetPosition();
        target.rotation = PTUtilities.instance.HeadsetRotation();

    }

    // GENERAL EVENTS

    public void DestroyGameObject( GameObject objectToDestroy ) {
        Destroy(objectToDestroy);
    }


    public void ShakeTransform( Transform target, Vector3 shakeAmount, float duration) {
        StartCoroutine(PTUtilities.instance.ShakeTransform(target, shakeAmount, duration));
    }


    public void FadeAudioSourceIn(AudioSource source ) {
        StartCoroutine(PTUtilities.instance.FadeAudioTo(source, 1f, 0.5f));
    }

    public void FadeAudioSourceOut( AudioSource source ) {
        StartCoroutine(PTUtilities.instance.FadeAudioTo(source, 0f, 0.5f));
    }
    public void FadeAudioSource( AudioSource source, float volume, float duration ) {
        StartCoroutine(PTUtilities.instance.FadeAudioTo(source, volume, duration));
    }




    public void FadeMeshIn( MeshRenderer mesh ) {
        StartCoroutine(PTUtilities.instance.FadeAlphaTo(mesh, 1, 0.5f));
    }
    public void FadeMeshOut( MeshRenderer mesh ) {
        StartCoroutine(PTUtilities.instance.FadeAlphaTo(mesh, 0, 0.5f));
    }
    public void FadeMesh( MeshRenderer mesh, float alpha, float duration ) {
        StartCoroutine(PTUtilities.instance.FadeAlphaTo(mesh, alpha, duration));
    }
    public void FadeMeshColor( MeshRenderer mesh, Color color, float duration ) {
        StartCoroutine(PTUtilities.instance.FadeColorTo(mesh, color, duration));
    }


    public void FadeSpriteIn( SpriteRenderer sprite ) {
        StartCoroutine(PTUtilities.instance.FadeAlphaTo(sprite, 1, 1.5f));
    }
    public void FadeSpriteOut( SpriteRenderer sprite ) {
        StartCoroutine(PTUtilities.instance.FadeAlphaTo(sprite, 0, 1.5f));
    }
    public void FadeSprite( SpriteRenderer sprite, float alpha, float duration ) {
        StartCoroutine(PTUtilities.instance.FadeAlphaTo(sprite, alpha, duration));
    }
    public void FadeSpriteColor( SpriteRenderer sprite, Color color, float duration ) {
        StartCoroutine(PTUtilities.instance.FadeColorTo(sprite, color, duration));
    }


    public void FadeTextIn( TextMeshPro text ) {
        StartCoroutine(PTUtilities.instance.FadeAlphaTo(text, 1, 1.5f));
    }

    public void FadeTextOut( TextMeshPro text ) {
        StartCoroutine(PTUtilities.instance.FadeAlphaTo(text, 0, 1.5f));
    }
    public void FadeText( TextMeshPro text, float alpha, float duration ) {
        StartCoroutine(PTUtilities.instance.FadeAlphaTo(text, alpha, duration));
    }
    public void FadeTextColor( TextMeshPro text, Color color, float duration ) {
        StartCoroutine(PTUtilities.instance.FadeColorTo(text, color, duration));
    }



    Coroutine headFadeCo;
    public void FadeHeadsetColor( Color color, float duration ) {
        if (headFadeCo != null) StopCoroutine(headFadeCo);
        headFadeCo = StartCoroutine(PTUtilities.instance.FadeColorTo(PTUtilities.instance.headGfx, color, duration));
    }

    public void FadeHeadsetIn( float duration ) {
        if (headFadeCo != null) StopCoroutine(headFadeCo);
        headFadeCo = StartCoroutine(PTUtilities.instance.FadeAlphaTo(PTUtilities.instance.headGfx, 1f, duration));
    }

    public void FadeHeadsetOut( float duration ) {
        if (headFadeCo != null) StopCoroutine(headFadeCo);
        headFadeCo = StartCoroutine(PTUtilities.instance.FadeAlphaTo(PTUtilities.instance.headGfx, 0f, duration));
    }



    public void FadeHeadsetToBlack(float duration ) {
        FadeHeadsetColor(Color.black, duration);
    }

    public void FadeHeadsetToWhite( float duration ) {
        FadeHeadsetColor(Color.white, duration);
    }

    //public void FadeHeadsetToBlue( float duration ) {
    //    FadeHeadsetColor(Color.Lerp(Color.cyan, Color.white,0.5f), duration);
    //}

    public void TeleportPlayer(Vector3 worldPosition, Vector3 forwardDirection) {
        PTUtilities.instance.TeleportPlayer(worldPosition, forwardDirection);
    }

    public void TeleportPlayer(Transform targetTransform ) {
        PTUtilities.instance.TeleportPlayer(targetTransform);
    }

}
