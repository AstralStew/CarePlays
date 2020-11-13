using Paperticket;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class ButtonInteractable : MonoBehaviour
{
    protected XRBaseInteractable baseInteractable;
    protected SpriteRenderer spriteRend;
    protected MeshRenderer meshRend;

    [Space(10)]
    [SerializeField] protected bool debugging;

    [Header("GRAPHICS")]

    [SerializeField] protected Renderer genRenderer;
    [Space(10)]
    [SerializeField] protected float fadeTime = 0.25f;
    [SerializeField] protected Color defaultColor = new Color(1, 1, 1, 0.5f);
    [SerializeField] protected Color hoveredColor = Color.white;
    [SerializeField] protected Color selectedColor = Color.grey;
    [Space(10)]
    [SerializeField] protected Color usedColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);

    [Header("CONTROLS")]

    [Space(10)]
    [SerializeField] protected bool oneUse = false;
    protected bool used;
    [Space(10)]
    [SerializeField] protected UnityEvent2 selectEvent;
    [Space(10)]
    [SerializeField] protected bool useSprite = true;

    protected Coroutine fadingCoroutine;

    void Awake() {

        baseInteractable = baseInteractable ?? GetComponent<XRBaseInteractable>() ?? GetComponentInChildren<XRBaseInteractable>(true);
        if (!baseInteractable) {
            if (debugging) Debug.LogError("[ButtonInteractable] ERROR -> No XRSimpleInteractable found on or beneath this button! Please add one. Disabling object.");
            gameObject.SetActive(false);
        }

        //spriteRend = spriteRend ?? GetComponentInChildren<SpriteRenderer>() ?? GetComponentInChildren<SpriteRenderer>(true);
        //if (!spriteRend && useSprite) {
        //    if (debugging) Debug.LogError("[ButtonInteractable] ERROR -> No sprite found on or beneath this button! Please add one. Disabling object.");
        //    gameObject.SetActive(false);
        //}

        genRenderer = genRenderer != null ? genRenderer : 
                       GetComponentInChildren<Renderer>() != null ? GetComponentInChildren<Renderer>() :
                       GetComponentInChildren<Renderer>(true) != null ? GetComponentInChildren<Renderer>(true) :
                       null;

        if (!genRenderer) {
            if (debugging) Debug.LogError("[ButtonInteractable] ERROR -> No renderer found on or beneath this button! Please add one. Disabling object.");
            gameObject.SetActive(false);
        } else {
            if (genRenderer as SpriteRenderer != null) {
                spriteRend = genRenderer as SpriteRenderer;
                useSprite = true;
                Debug.Log("RENDERER = SPRITE");
            } else if (genRenderer as MeshRenderer != null) {
                meshRend = genRenderer as MeshRenderer;
                useSprite = false;
                Debug.Log("RENDERER = MESH");
            }

            if (spriteRend == null && meshRend == null) {
                if (debugging) Debug.LogError("[ButtonInteractable] ERROR -> No appropriate renderer found on or beneath this button! Please add one. Disabling object.");
                gameObject.SetActive(false);
            }
        }

        

    }


    // Start is called before the first frame update
    void OnEnable() {

        Initialise();
    }

    void OnDisable() {

    }


    protected virtual void Initialise() {

        if (oneUse && used) {
            if (fadingCoroutine != null)StopCoroutine(fadingCoroutine);
            fadingCoroutine = StartCoroutine(PTUtilities.instance.FadeColorTo(spriteRend, usedColor, fadeTime));

        } else {
            Invoke("HoverOff", 0.5f);
        }

    }

    public virtual void HoverOn() { HoverOn(null); }
    public virtual void HoverOn ( XRBaseInteractor interactor ) {
        if (oneUse && used) return;
        
        if (fadingCoroutine != null) StopCoroutine(fadingCoroutine);
        if (useSprite) fadingCoroutine = StartCoroutine(PTUtilities.instance.FadeColorTo(spriteRend, hoveredColor, fadeTime));
        else fadingCoroutine = StartCoroutine(PTUtilities.instance.FadeColorTo(meshRend, hoveredColor, fadeTime));



        if (debugging) Debug.Log("[ButtonInteractable] Hovering on!");
    }

    public virtual void HoverOff() { HoverOff(null); }
    public virtual void HoverOff( XRBaseInteractor interactor ) {
        if (oneUse && used) return;
        
        if (fadingCoroutine != null) StopCoroutine(fadingCoroutine);
        if (useSprite) fadingCoroutine = StartCoroutine(PTUtilities.instance.FadeColorTo(spriteRend, defaultColor, fadeTime));
        else fadingCoroutine = StartCoroutine(PTUtilities.instance.FadeColorTo(meshRend, defaultColor, fadeTime));


        if (debugging) Debug.Log("[ButtonInteractable] Hovering off!");
    }

    public virtual void Select() { Select(null); }
    public virtual void Select( XRBaseInteractor interactor ) {
        if (oneUse && used) return;
        
        if (fadingCoroutine != null)StopCoroutine(fadingCoroutine);
        if (useSprite) fadingCoroutine = StartCoroutine(PTUtilities.instance.FadeColorTo(spriteRend, selectedColor, fadeTime));
        else fadingCoroutine = StartCoroutine(PTUtilities.instance.FadeColorTo(meshRend, selectedColor, fadeTime));

        used = true;
        if (selectEvent != null) selectEvent.Invoke();

        if (debugging) Debug.Log("[ButtonInteractable] Selected!");
    }




    #region UNUSED


    public virtual void Deselect() { Deselect(null); }
    public virtual void Deselect( XRBaseInteractor interactor ) {
        if (oneUse && used) return;
        
        if (oneUse) {
            if (fadingCoroutine != null) StopCoroutine(fadingCoroutine);
            if (useSprite) fadingCoroutine = StartCoroutine(PTUtilities.instance.FadeColorTo(spriteRend, usedColor, fadeTime));
            else fadingCoroutine = StartCoroutine(PTUtilities.instance.FadeColorTo(meshRend, usedColor, fadeTime));
        } else {
            if (fadingCoroutine != null) StopCoroutine(fadingCoroutine);
            if (useSprite) fadingCoroutine = StartCoroutine(PTUtilities.instance.FadeColorTo(spriteRend, defaultColor, fadeTime));
            else fadingCoroutine = StartCoroutine(PTUtilities.instance.FadeColorTo(meshRend, defaultColor, fadeTime));
        }


        if (debugging) Debug.Log("[ButtonInteractable] Deselected!");
    }

    #endregion

}
