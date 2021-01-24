using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Paperticket {
    public class EventSender : MonoBehaviour {

        enum TriggerType { OnTriggerEnter, OnTriggerStay, Both}

        [SerializeField] bool debugging;

        [Header("TRIGGER CONTROLS")]
        [SerializeField] TriggerType triggerType;
        [SerializeField] LayerMask triggerLayers;
        [SerializeField] bool requireTag;
        [SerializeField] string tag;

        [Header("TIME CONTROLS")]
        [SerializeField] float timeBeforeEvent;
        [SerializeField] bool OneTimeUse = true;        
        bool used;

        [Header("EVENTS")]

        [SerializeField] UnityEvent OnEnterTriggered;
        [SerializeField] UnityEvent OnStayTriggered;

        private void OnTriggerEnter2D ( Collider2D collision ) {
            if (triggerType == TriggerType.OnTriggerStay || (OneTimeUse && used)) return;

            if (((1 << collision.gameObject.layer) & triggerLayers) != 0) {
                if (!requireTag || requireTag && collision.gameObject.tag == tag) {
                    StartCoroutine(CountdownToEvent(TriggerType.OnTriggerEnter));
                }
            }
        }

        private void OnTriggerStay2D( Collider2D collision ) {
            if (triggerType == TriggerType.OnTriggerEnter || (OneTimeUse && used)) return;

            if (((1 << collision.gameObject.layer) & triggerLayers) != 0) {
                if (!requireTag || requireTag && collision.gameObject.tag == tag) {
                    StartCoroutine(CountdownToEvent(TriggerType.OnTriggerStay));
                }
            }
        }


        private void OnTriggerEnter( Collider collision ) {
            if (triggerType == TriggerType.OnTriggerStay || (OneTimeUse && used)) return;

            if (((1 << collision.gameObject.layer) & triggerLayers) != 0) {
                if (!requireTag || requireTag && collision.gameObject.tag == tag) {
                    StartCoroutine(CountdownToEvent(TriggerType.OnTriggerEnter));
                }
            }
        }

        private void OnTriggerStay( Collider collision ) {
            if (triggerType == TriggerType.OnTriggerEnter || (OneTimeUse && used)) return;

            if (((1 << collision.gameObject.layer) & triggerLayers) != 0) {
                if (!requireTag || requireTag && collision.gameObject.tag == tag) {
                    StartCoroutine(CountdownToEvent(TriggerType.OnTriggerStay));
                }
            }
        }



        IEnumerator CountdownToEvent(TriggerType type) {
            if (debugging) Debug.Log("[EventSender] Counting down event...");
            used = false;

            // Wait if necessary
            if (timeBeforeEvent > 0) yield return new WaitForSeconds(timeBeforeEvent);
            else yield return null;


            // Trigger the event
            switch (type) {
                case TriggerType.OnTriggerEnter:
                    if (OnEnterTriggered != null) {
                        if (debugging) Debug.Log("[EventSender] OnEnterTriggered called!");
                        OnEnterTriggered.Invoke();
                    }
                    break;
                case TriggerType.OnTriggerStay:
                    if (OnStayTriggered != null) {
                        if (debugging) Debug.Log("[EventSender] OnStayTriggered called!");
                        OnStayTriggered.Invoke();
                    }
                    break;
                case TriggerType.Both:
                default:
                    Debug.LogError("[EventSender] Bad trigger type given! Ignoring.");
                    break;
            }

            
            // Destroy this script if this is a one time use, otherwise reset
            if (OneTimeUse) {
                if (debugging) Debug.Log("[EventSender] This event is set to OneTimeUse, disabling self");
                enabled = false;
            }
            used = true;
        }

    }
}