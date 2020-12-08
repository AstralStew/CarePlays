using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using UnityEngine;

namespace Paperticket {

    public class VideoSwitcher : MonoBehaviour {

        [SerializeField] VideoController videoController;
        [SerializeField] bool debugging;

        [Header("BUTTONS")]
        [Space(20)]
        [Tooltip("This works like a button! It should set itself to false afterwards!")] 
        [SerializeField] bool setNewVideo = false;
        [SerializeField] string newVideoName;
        [Space(10)]
        [Tooltip("This works like a button! It should set itself to false afterwards!")]
        [SerializeField] bool setTime;
        [SerializeField] [Min(0)] float newTime;

        private void OnValidate() {

            if (setNewVideo) {
                if (debugging) Debug.Log("[VideoSwitcher] Setting new video: " + newVideoName);
                videoController.SetNextVideo(newVideoName);
                setNewVideo = false;

            } else if (setTime) {
                if (debugging) Debug.Log("[VideoSwitcher] Setting new time: " + newTime);
                videoController.SetTime(newTime);
                setTime = false;
            }
        }

        //void SetNewVideoButton() {
        //    SetNewVideo(NewVideoName);
        //}

        //void SetTime() {
        //    videoController.SetTime(setTime);
        //}

        //public void SetNewVideo(string videoName) {
        //    if (debug) Debug.Log("[VideoSwitcher] Setting new video: " + newVideoName);
        //    videoController.SetNextVideo(newVideoName);
        //}

    }

}