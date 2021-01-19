using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;

namespace Paperticket {
    public class KinshipChair : MonoBehaviour {

        protected XRBaseInteractable baseInteractable;

        [Header("REFERENCES")]
        [SerializeField] IN04KinshipGame KinshipGame;
        public Transform attachPoint;

        [Header("CONTROLS")]
        [Space(10)]
        public bool CanSeatMalcolm = false;
        public bool IsExtraSeat = false;
        [Space(5)]
        [SerializeField] bool debugging;
        [Space(5)]
        [SerializeField] UnityEvent2 OnHover;
        //[SerializeField] UnityEvent2 OnSeated;
        //[SerializeField] UnityEvent2 OnEmptied;

        [Header("LIVE VARIABLES")]
        [Space(15)]

        public bool Active = false;


        Material mat;
        Color defaultColor;

        // Start is called before the first frame update
        void Awake() {

            baseInteractable = baseInteractable ?? GetComponent<XRBaseInteractable>() ?? GetComponentInChildren<XRBaseInteractable>(true);
            if (!baseInteractable) {
                Debug.LogError("[KinshipChair] ERROR -> No XRSimpleInteractable found! Please add one. Disabling object.");
                gameObject.SetActive(false);
            }

            KinshipGame = KinshipGame ?? FindObjectOfType<IN04KinshipGame>();
            if (KinshipGame == null) {
                Debug.LogError("[KinshipChair] ERROR -> Could not find KinshipGame component in scene!");
                gameObject.SetActive(false);
            }

            MeshRenderer mesh = GetComponentInChildren<MeshRenderer>(true);
            if (mesh == null) {
                Debug.LogError("[KinshipChair] ERROR -> Could not find MeshRenderer in children!");
                gameObject.SetActive(false);
            }
            mat = mesh.material;
            defaultColor = mat.GetColor("_Color");

        }

        public virtual void HoverOn() { HoverOn(null); }
        public virtual void HoverOn ( XRBaseInteractor interactor ) {
            if (!Active) return;
            if (debugging) Debug.Log("[KinshipChair] Hover on!");

            if (OnHover != null) OnHover.Invoke();

            mat.SetColor("_Color", Color.cyan);
        }

        public virtual void HoverOff() { HoverOff(null); }
        public virtual void HoverOff( XRBaseInteractor interactor ) {
            if (!Active) return;
            if (debugging) Debug.Log("[KinshipChair] Hover off!");

            mat.SetColor("_Color", defaultColor);
        }

        public virtual void Select() { Select(null); }
        public virtual void Select( XRBaseInteractor interactor ) {
            if (!Active) return;
            if (debugging) Debug.Log("[KinshipChair] Selected!");

            KinshipGame.SetPersonToChair(this);

        }

        public void ResetChair() {
            if (debugging) Debug.Log("[KinshipChair] Reset!");

            Active = true;
            HoverOff();
        }

        public void DisableChair() {
            if (debugging) Debug.Log("[KinshipChair] Disabled!");

            HoverOff();
            Active = false;
        }



    }
}