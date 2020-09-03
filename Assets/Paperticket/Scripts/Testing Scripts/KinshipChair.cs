using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Paperticket {
    public class KinshipChair : MonoBehaviour {

        protected XRBaseInteractable baseInteractable;

        [SerializeField] IN04KinshipGame KinshipGame;

        public bool Active = false;

        public bool CanSeatMalcolm = false;

        [SerializeField] bool debugging;

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
            defaultColor = mat.color;

        }

        public virtual void HoverOn() { HoverOn(null); }
        public virtual void HoverOn ( XRBaseInteractor interactor ) {
            if (!Active) return;

            mat.color = Color.green;
            if (debugging) Debug.Log("[KinshipChair] Hover on!");
        }

        public virtual void HoverOff() { HoverOff(null); }
        public virtual void HoverOff( XRBaseInteractor interactor ) {
            if (!Active) return;

            mat.color = defaultColor;
            if (debugging) Debug.Log("[KinshipChair] Hover off!");
        }

        public virtual void Select() { Select(null); }
        public virtual void Select( XRBaseInteractor interactor ) {
            if (!Active) return;
                
            KinshipGame.SetPersonToChair(this);
        }

        public void ResetChair() {
            Active = true;
            HoverOff();
        }

        public void DisableChair() {
            HoverOff();
            Active = false;
        }



    }
}