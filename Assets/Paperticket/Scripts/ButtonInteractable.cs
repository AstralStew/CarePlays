using Paperticket;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ButtonInteractable : MonoBehaviour
{
    protected XRBaseInteractable baseInteractable;
    protected SpriteRenderer sprite;
    
    [Header("Graphics")]

    [SerializeField] protected float fadeTime = 0.25f;
    [SerializeField] protected Color defaultColor = new Color(1, 1, 1, 0.5f);
    [SerializeField] protected Color hoveredColor = Color.white;
    [SerializeField] protected Color selectedColor = Color.grey;

    [Header("Usage")]

    [SerializeField] protected bool oneUse = false;
    protected bool used;
    [SerializeField] protected Color usedColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
    [SerializeField] protected bool debugging;

    protected Coroutine fadingCoroutine;

    void Awake() {

        baseInteractable = baseInteractable ?? GetComponent<XRBaseInteractable>() ?? GetComponentInChildren<XRBaseInteractable>(true);
        if (!baseInteractable) {
            if (debugging) Debug.LogError("[ButtonInteractable] ERROR -> No XRSimpleInteractable found on or beneath this button! Please add one. Disabling object.");
            gameObject.SetActive(false);
        }

        sprite = sprite ?? GetComponentInChildren<SpriteRenderer>() ?? GetComponentInChildren<SpriteRenderer>(true);
        if (!sprite) {
            if (debugging) Debug.LogError("[ButtonInteractable] ERROR -> No sprite found on or beneath this button! Please add one. Disabling object.");
            gameObject.SetActive(false);
        }

    }


    // Start is called before the first frame update
    void OnEnable() {

        Initialise();
    }

    void OnDisable() {
        //simpleInteractable.onHoverEnter.RemoveListener(HoverOn);
        //simpleInteractable.onHoverExit.RemoveListener(HoverOff);
        //simpleInteractable.onSelectEnter.RemoveListener(Select);
        //simpleInteractable.onSelectExit.RemoveListener(Deselect);
        //simpleInteractable.onActivate.RemoveListener(Select);
        //simpleInteractable.onDeactivate.RemoveListener(Deselect);
    }


    protected virtual void Initialise() {

        //simpleInteractable.onHoverEnter.AddListener(HoverOn);
        //simpleInteractable.onHoverExit.AddListener(HoverOff);
        //simpleInteractable.onSelectEnter.AddListener(Select);         //Start of selection (trigger on)
        //simpleInteractable.onSelectExit.AddListener(Deselect);        //End of selection (trigger off)
        //simpleInteractable.onActivate.AddListener(Select);            //Activation while selected (grip on)
        //simpleInteractable.onDeactivate.AddListener(Deselect);        //Deactivation while selected (grip off)

        if (oneUse && used) {
            if (fadingCoroutine != null) {
                StopCoroutine(fadingCoroutine);
            }
            fadingCoroutine = StartCoroutine(PTUtilities.instance.FadeColorTo(sprite, usedColor, fadeTime));

        } else {
            Invoke("HoverOff", 0.5f);
        }

    }

    public virtual void HoverOn() { HoverOn(null); }
    public virtual void HoverOn ( XRBaseInteractor interactor ) {
        if (oneUse && used) return;
        
        if (fadingCoroutine != null) {
            StopCoroutine(fadingCoroutine);
        }

        fadingCoroutine = StartCoroutine(PTUtilities.instance.FadeAlphaTo(sprite, 1f, fadeTime));

        if (debugging) Debug.Log("[ButtonInteractable] Hovering on!");
    }

    public virtual void HoverOff() { HoverOff(null); }
    public virtual void HoverOff( XRBaseInteractor interactor ) {
        if (oneUse && used) return;
        
        if (fadingCoroutine != null) {
            StopCoroutine(fadingCoroutine);
        }

        fadingCoroutine = StartCoroutine(PTUtilities.instance.FadeColorTo(sprite, defaultColor, fadeTime));

        if (debugging) Debug.Log("[ButtonInteractable] Hovering off!");
    }

    public virtual void Select() { Select(null); }
    public virtual void Select( XRBaseInteractor interactor ) {
        if (oneUse && used) return;
        
        if (fadingCoroutine != null) {
            StopCoroutine(fadingCoroutine);
        }

        fadingCoroutine = StartCoroutine(PTUtilities.instance.FadeColorTo(sprite, selectedColor, fadeTime));
        
        used = true;

        if (debugging) Debug.Log("[ButtonInteractable] Selected!");
    }

    public virtual void Deselect() { Deselect(null); }
    public virtual void Deselect( XRBaseInteractor interactor ) {
        if (oneUse && used) return;
        
        if (fadingCoroutine != null) {
            StopCoroutine(fadingCoroutine);
        }

        fadingCoroutine = StartCoroutine(PTUtilities.instance.FadeColorTo(sprite, defaultColor, fadeTime));

        if (debugging) Debug.Log("[ButtonInteractable] Deselected!");
    }


}
