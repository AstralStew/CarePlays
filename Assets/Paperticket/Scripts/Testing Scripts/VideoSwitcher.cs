using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Paperticket {

    public class VideoSwitcher : MonoBehaviour {

        [SerializeField] VideoController videoController;
        [SerializeField] string NewVideoName;
        [SerializeField] bool debug;
        
        [ContextMenu ("Set New Video")] 
        void SetNewVideoButton() {
            SetNewVideo(NewVideoName);
        }

        public void SetNewVideo(string videoName) {
            if (debug) Debug.Log("[VideoSwitcher] Setting new video: " + videoName);
            videoController.SetNextVideo(videoName);

        }

    }

}