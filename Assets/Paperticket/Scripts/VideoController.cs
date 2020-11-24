using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using UnityEngine.Events;
using UnityEngine.Profiling;

namespace Paperticket {

    public class VideoController : MonoBehaviour {



        [Header("REFERENCES")]

        //Grab the video player and audio source
        public VideoPlayer videoPlayer;
        private string completePath;


        [Header("VIDEO CONTROLS")]
        [Space(10)]
        [SerializeField] string currentVideoName;
        public bool autoPlay;
        [SerializeField] bool skipFramesOnDrop;


        [Header("PRE-START CONTROLS")]
        [Space(10)]
        [SerializeField] bool moveToHead;
        [SerializeField] bool rotateToHead;
        [SerializeField] Vector3 initialRotation;


        [Header("AUDIO CONTROLS")]
        [Space(10)]
        [SerializeField] bool externalAudio;
        [SerializeField] AudioSource externalAudioSource;


        [Header("FINISH CONTROLS")]
        [Space(10)]
        [SerializeField] float earlyFinishDuration = 0.5f;
        [Space(10)]
        [SerializeField] bool _UseFinishEvents;
        [SerializeField] UnityEvent2[] _FinishEvents;
        
        int finishIndexNo;


        [Header("READ ONLY")]
        [Space(5)]

        [SerializeField] string datapath;
        [Space(10)]
        public bool videoLoaded;
        public bool videoStarted;
        public bool playingVideo;
        public bool videoEnded;


        [Header("DEBUGGING")]
        [Space(10)]
        [SerializeField] bool debugging;
        [Space(10)]
        public float currentVideoTime;                // in seconds
        [SerializeField] private long currentFrames;
        [SerializeField] private long endFrames;

        

        // Use this for initialization
        void OnEnable() {

            // Set the video sphere position and rotation (if applicable)
            if (moveToHead) transform.position = PTUtilities.instance.HeadsetPosition();
            if (rotateToHead) transform.rotation = PTUtilities.instance.HeadsetRotation() * Quaternion.Euler(initialRotation);
            else transform.rotation = Quaternion.Euler(initialRotation);

            //Make sure the video doesn't skip frames (to keep the audio in sync)
            videoPlayer.skipOnDrop = skipFramesOnDrop;

            videoPlayer.source = VideoSource.VideoClip;
            StartCoroutine(LoadVideoClipFromBundle(currentVideoName));

        }


        // Update is called once per frame
        void Update() {

            if (videoPlayer.isPrepared) {

                // Shows if the video is currently playing
                playingVideo = videoPlayer.isPlaying;

                // The current time of the video playthrough
                currentVideoTime = (float)videoPlayer.time;
            }

            currentFrames = videoPlayer.frame;

            if (!videoPlayer.isLooping && videoStarted && !videoEnded && (videoPlayer.frame >= endFrames)) {
                FinishVideo();
            }

        }


                     
        #region PUBLIC VIDEO FUNCTIONS
               

        //-----------------------------------------------------------
        // PUBLIC VIDEO CONTROLS
        //-----------------------------------------------------------

        // Reset the video controller and get a new video lined up
        public void SetNextVideo( string newVideoName ) {

            if (debugging) Debug.Log("[VideoController] Switching the video to " + newVideoName);

            // Stop the video
            videoPlayer.Stop();            

            // Set the bools to false
            videoStarted = false;
            videoLoaded = false;
            videoEnded = false;

            // Set position and rotation if applicable
            if (moveToHead) transform.position = PTUtilities.instance.HeadsetPosition();
            if (rotateToHead) transform.rotation = PTUtilities.instance.HeadsetRotation() * Quaternion.Euler(initialRotation);
            else transform.rotation = Quaternion.Euler(initialRotation);

            // Load the prepare the next video
            StartCoroutine(LoadVideoClipFromBundle(newVideoName));

        }
        
        // Plays the video if it's not already playing 
        public void PlayVideo() {

            // If the video hasn't started yet...
            if (!videoStarted) {
                videoStarted = true;

                // Play the video
                videoPlayer.Play();
                if (debugging) Debug.Log("[VideoController] Playing the video");

                // If it exists, set audio time to 0 and play the audio
                if (externalAudio) {
                    externalAudioSource.Play();
                    externalAudioSource.time = 0f;
                }

                // If the video hasn't ended yet...
            } else if (!videoEnded) {

                // If we aren't playing, play the video
                if (!playingVideo) {
                    videoPlayer.Play();
                    if (debugging) Debug.Log("[VideoController] Were we interrupted? Playing the video again");
                }

                // Force the external audio time to match the video time
                if (externalAudio) {
                    externalAudioSource.time = (float)videoPlayer.time;
                    // Also, play the audio If not playing
                    if (!externalAudioSource.isPlaying) externalAudioSource.Play();
                }
            }
        }

        // Set the time of the video (only seems to work when playing)
        public void SetTime( float time ) {
            if (debugging) Debug.Log("[VideoController] Atempting to set time to " + time + " seconds...");

            // Check if the provided time is viable
            if (time < (endFrames / videoPlayer.frameRate)) {

                videoPlayer.time = time;
                if (externalAudio) externalAudioSource.time = time;

            } else Debug.LogError("[VideoController] ERROR -> Cannot SetTime to value greater than video duration!");

        }

        // Add or subtract a certain amount of time from the video
        public void AddTimeStep( float timeStep ) {
            if (debugging) Debug.Log("[VideoController] Atempting to step forward " + timeStep + " seconds...");

            // Set up an end buffer of 5 seconds
            float totalTime = endFrames - 5f;

            if (videoPlayer.time < (totalTime - timeStep)) {

                float newTime = Mathf.Clamp((float)videoPlayer.time + timeStep, 0f, totalTime - 10f);

                videoPlayer.time = newTime;

                if (externalAudio) {
                    externalAudioSource.time = newTime;
                }
            }
        }

        // Pause the video if it is playing
        public void PauseVideo() {

            if (!playingVideo) return;

            if (debugging) Debug.Log("[VideoController] Pausing the video");
            videoPlayer.Pause();

            if (externalAudio) {
                externalAudioSource.Pause();
            }

        }
        
        // A stop method so that it can be invoked on command
        public void StopVideo() {
            videoPlayer.Stop();
            if (externalAudio) {
                externalAudioSource.Stop();
            }

            videoEnded = true;
        }

        #endregion


        #region OTHER PUBLIC FUNCTIONS


        public void SetAudioVolume( float volume ) {

            if (debugging) Debug.Log("[VideoController] Setting audio volume");

            if (externalAudio) {
                externalAudioSource.volume = Mathf.Clamp01(volume);
            } else if (videoPlayer.audioOutputMode == VideoAudioOutputMode.AudioSource) {
                videoPlayer.GetTargetAudioSource(0).volume = Mathf.Clamp01(volume);
            } else {
                videoPlayer.SetDirectAudioVolume(0, Mathf.Clamp01(volume));
            }

        }

        public void SetInitialRotation (Vector3 rotation ) {

            initialRotation = rotation;

        }

        #endregion


        #region INTERNAL FUNCTIONS

        //-----------------------------------------------------------
        // EVENT HANDLERS
        //-----------------------------------------------------------

        // Let us know the video has finished playing (from event in update)
        void FinishVideo() {
            if (!videoPlayer.isLooping) {
                videoEnded = true;
                videoStarted = false;

                videoPlayer.Pause();

                if (_UseFinishEvents && _FinishEvents.Length > 0 && finishIndexNo < _FinishEvents.Length) {

                    _FinishEvents[finishIndexNo].Invoke();
                    finishIndexNo++;

                }
            } else {
                Invoke("StopVideo", 1f);
            }
        }


        void VideoPlayerErrorReceived( VideoPlayer source, string message ) {
            Debug.LogError("[VideoController] VideoPlayer on '" + source.gameObject.name + "' error received! Error = " + message);
            videoPlayer.errorReceived -= VideoPlayerErrorReceived;
        }

        #endregion


        #region INTERNAL COROUTINES

        // Set the clip of the new video to be played
        IEnumerator LoadVideoClipFromBundle( string clipName ) {
            if (debugging) Debug.Log("[VideoController] Setting the video clip");

            //Let other scripts know the video hasn't started yet
            videoStarted = false;

            // Load the asset bundle from the above path
            AssetBundle videoBundle = DataUtilities.instance._ExpansionAssetBundle;

            // Load the video clip from the asset bundle and wait until it's finished
            var assetLoadRequest = videoBundle.LoadAssetAsync<VideoClip>(clipName);
            yield return assetLoadRequest;

            // Treat the video as a VideoClip and give to the video player
            VideoClip clip = assetLoadRequest.asset as VideoClip;
            videoPlayer.clip = clip;
            currentVideoName = clipName;

            if (debugging) Debug.Log("[VideoController] Video clip set to '" + clip.name + "'");


            // Chucked this in here to test the above, put back in Start if necessary
            StartCoroutine(PreparingVideo());

        }

        // Start preparing the video and waits till its done
        IEnumerator PreparingVideo() {

            if (debugging) Debug.Log("[VideoController] Starting video preparation");

            // Setup error checking
            videoPlayer.errorReceived += VideoPlayerErrorReceived;

            //Prepare next video
            videoPlayer.Prepare();

            //Wait until video is prepared			
            if (debugging) Debug.Log("[VideoController] Preparing video");
            while (!videoPlayer.isPrepared) {
                yield return null;

            }
            if (debugging) Debug.Log("[VideoController] Video prepared!");

            videoLoaded = true;

            endFrames = (long)(videoPlayer.frameCount - (earlyFinishDuration * videoPlayer.frameRate)); // 15

            if (autoPlay) {
                if (debugging) Debug.Log("[VideoController] Autoplay is on, playing video!");
                PlayVideo();
            }
        }


        #endregion



    }

}











//// Set the URL/path of the new video to be played
//void SetVideoURL( string videoName ) {

//    if (_Debug) Debug.Log("[VideoController] Setting the video URL");

//    //Let other scripts know the video hasn't started yet
//    videoStarted = false;

//    // Set the datapath based on the platform
//    if (Application.platform == RuntimePlatform.WindowsPlayer) {
//        datapath = Application.dataPath + "/Videos/";
//    } else if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.WindowsEditor) {
//        datapath = Application.persistentDataPath + "/Videos/";
//    }

//    // Construct and set the complete path to the video file	
//    videoPlayer.url = completePath = datapath + videoName + ".mp4";

//    if (_Debug) Debug.Log("[VideoController] Video URL set to '" + completePath + "'");

//    // Chucked this in here to test the above, put back in Start if necessary
//    StartCoroutine(PreparingVideo());

//}
