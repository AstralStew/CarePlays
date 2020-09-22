using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using System.IO;
using System;

namespace Paperticket {
    public class WE02FishingGame2 : MonoBehaviour
    {

        [Header("References")]

        [SerializeField] Transform playerFish;
        [SerializeField] SpriteRenderer backgroundSprite;
        [SerializeField] Transform fadeSprite;
        [SerializeField] Transform objectSprites;
        [SerializeField] LineRenderer fishingLine;


        [Header("Controls")]
        [Space(10)]
        [SerializeField] AnimationCurve objectsHeightCurve;
        [SerializeField] AnimationCurve fadeHeightCurve;
        [SerializeField] Gradient backgroundColors;

        [Header("Read Only")]
        [Space(10)]
        [SerializeField] [Range(-1, 1)] float lineXPos;

        [SerializeField] [Range(0,1)] float progress;

        [Header("Debug")]
        [Space(10)]
        [SerializeField] bool debugging;
        [SerializeField] bool debugScroll;
        [SerializeField] [Range(1, 20)] float debugScrollSpeed;

        void Awake() {
                
            if (backgroundSprite == null) Debug.LogError("[WE02FishingGame] ERROR -> No background sprite registered!");
            if (objectSprites == null) Debug.LogError("[WE02FishingGame] ERROR -> No object sprite registered!");

        }

        void OnDrawGizmosSelected() {

            if (debugScroll) { progress = (progress + (debugScrollSpeed * 0.0001f)) % 1; }

            ResolveProgress();
        }


        void ResolveProgress() {

            backgroundSprite.color = backgroundColors.Evaluate(progress);

            objectSprites.localPosition = new Vector3(0, objectsHeightCurve.Evaluate(progress), 5);

            fadeSprite.localPosition = new Vector3(0, fadeHeightCurve.Evaluate(progress), 4.5f);


            fishingLine.SetPosition(0, new Vector3(lineXPos, 1.02f, 5));
            fishingLine.SetPosition(1, playerFish.localPosition);
        }


    }
}