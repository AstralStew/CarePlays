using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Paperticket {
    public class KinshipPerson : MonoBehaviour {

        [Header("References")]

        [SerializeField] IN04KinshipGame KinshipGame;

        [Header("Controls")]

        [SerializeField] Vector3 seatOffset;

        [Header("Read Only")]

        public bool Active = false;
        Vector3 startPos = Vector3.zero;
        Quaternion startRot = Quaternion.identity;

        Material mat;
        Color defaultColor;

        void Awake() {
            KinshipGame = KinshipGame ?? FindObjectOfType<IN04KinshipGame>();
            if (KinshipGame == null) {
                Debug.LogError("ERROR -> Could not find KinshipGame component in scene!");
                enabled = false;
            }

            MeshRenderer mesh = GetComponentInChildren<MeshRenderer>();
            if (mesh == null) {
                Debug.LogError("[KinshipChair] ERROR -> Could not find MeshRenderer in children!");
                gameObject.SetActive(false);
            }
            mat = mesh.material;
            defaultColor = mat.GetColor("_BaseColor");

            startPos = transform.position;
            startRot = transform.rotation;

        }

        public void SelectPerson() {
            mat.SetColor("_BaseColor", Color.green);
        }

        public void WrongChoice() {
            mat.SetColor("_BaseColor", Color.red);
        }

        public void SeatPerson (Transform chairTransform ) {
            transform.position = chairTransform.position;
            transform.rotation = chairTransform.rotation;
            transform.Translate(seatOffset, Space.Self);
            mat.SetColor("_BaseColor", defaultColor);
        }

        public void ResetPerson() {
            transform.position = startPos;
            transform.rotation = startRot;
            mat.SetColor("_BaseColor", defaultColor);
        }


    }

}