using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

namespace Paperticket {

    public class SceneUtilities : MonoBehaviour {

        public static SceneUtilities instance = null;

        public delegate void SceneAlmostReady();
        public static event SceneAlmostReady OnSceneAlmostReady;

        public delegate void SceneLoaded();
        public static event SceneLoaded OnSceneLoad;

        public delegate void SceneUnloaded();
        public static event SceneUnloaded OnSceneUnload;


        [Header("Controls")]

        [SerializeField] bool _LoadFirstScene = false;
        public string _FirstSceneName = "";

        [SerializeField] bool _Debug = false;

        AsyncOperation asyncOperation = null;

        [SerializeField] bool convergeDynamicGI = false;

        string lastSceneStarted = "";


        void Awake() {
            if (instance == null) {
                instance = this;
            } else if (instance != this) {
                Destroy(gameObject);
            }

            // Load the intro scene
            if (_LoadFirstScene) {
                StartCoroutine(LoadingFirstScene());
            }
        }

        IEnumerator LoadingFirstScene() {
            if (_Debug) Debug.Log("[SceneUtilities] Loading the first scene: " + _FirstSceneName);

            BeginLoadScene(_FirstSceneName);
            yield return new WaitUntil(() => lastSceneStarted == _FirstSceneName);

            // Wait a sec then finish loading the intro scene
            yield return new WaitForSeconds(0.5f);
            FinishLoadScene(true);

        }






        #region Public variables

        public bool CheckSceneLoaded( string sceneName ) {
            if (SceneManager.GetSceneByName(sceneName).isLoaded) {
                if (_Debug) Debug.Log("[SceneUtilities] " + sceneName + " is loaded!");
                return true;
            }
            return false;
        }


        public bool CheckSceneActive( string sceneName ) {
            if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName(sceneName) && DynamicGI.isConverged) {
                if (_Debug) Debug.Log("[SceneUtilities] " + sceneName + " is active!");
                return true;
            }
            return false;
        }

        public int SceneCount {
            get { return SceneManager.sceneCount; }
        }

        /// <summary>
        /// Get the progress on the current scene load
        /// </summary>
        public float GetSceneProgress {
            get {
                if (asyncOperation != null) {
                    return asyncOperation.progress;
                } else {
                    return 0;
                }
            }
        }

        #endregion


        #region Public Calls

        public void LoadScene( string sceneToLoad, bool setActive ) {
            if (_Debug) Debug.Log("[SceneUtilities] Attempting to load scene '" + sceneToLoad + "'" + (setActive ? ", and set it as the active scene" : "" ));
            StartCoroutine(LoadingScene(sceneToLoad, setActive));

        }

        public void BeginLoadScene( string sceneToLoad ) {
            if (_Debug) Debug.Log("[SceneUtilities] Attempting to begin loading scene '" + sceneToLoad + "'");
            StartCoroutine(BeginLoadingScene(sceneToLoad));
        }

        public void FinishLoadScene( bool setSceneActive ) {
            if (_Debug) Debug.Log("[SceneUtilities] Attempting to finish loading '" + lastSceneStarted + "'");
            StartCoroutine(FinishLoadingScene(setSceneActive));
        }

        public void LoadSceneExclusive( string sceneToLoad ) {

            if (_Debug) Debug.Log("[SceneUtilities] Attempting to load scene '"+sceneToLoad+"' exclusively");
            StartCoroutine(LoadingSceneExclusive(sceneToLoad));
        }

        public void UnloadScene( string scene ) {
            if (_Debug) Debug.Log("[SceneUtilities] Attempting to unload scene '"+scene+"' asynchronously (enforcing a max scene count of 2)");
            StartCoroutine(UnloadingScene(scene, 2, true));
        }

        public void UnloadScene( string scene, int maxSceneCount, bool forceSceneCleanup ) {
            if (_Debug) Debug.Log("[SceneUtilities] Attempting to unload scene '"+scene+"' asynchronously (enforcing a max scene count of "+maxSceneCount+")");
            StartCoroutine(UnloadingScene(scene, maxSceneCount, forceSceneCleanup));
        }

        public void UnloadActiveScene (int maxSceneCount, bool forceSceneCleanup ) {
            if (_Debug) Debug.Log("[SceneUtilities] Attempting to unload scene '"+SceneManager.GetActiveScene().name+"' asynchronously (enforcing a max scene count of "+maxSceneCount+")");
            StartCoroutine(UnloadingScene(SceneManager.GetActiveScene().name, maxSceneCount, forceSceneCleanup));
        }

        public void ForceUnloadUnusedAssets() {
            StartCoroutine(FlushingUnusedAssets());
        }


        #endregion


        #region Loading / Unloading Couroutines

        IEnumerator LoadingScene( string sceneToLoad, bool setActive ) {

            yield return BeginLoadingScene(sceneToLoad);
            yield return FinishLoadingScene(setActive);

        }


        IEnumerator BeginLoadingScene( string sceneToLoad ) {

            // Begin to load the new scene
            asyncOperation = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
            asyncOperation.allowSceneActivation = false;
            if (_Debug) Debug.Log("[SceneUtilities] Waiting for scene '" + sceneToLoad + "' to load...");

            // Wait until the new scene is almost loaded
            yield return new WaitUntil(() => asyncOperation.progress >= 0.9f);
            lastSceneStarted = sceneToLoad;

            // Send an event out for the caller script to pick up
            if (OnSceneAlmostReady != null) {
                if (_Debug) Debug.Log("[SceneUtilities] OnSceneAlmostReady event called");
                OnSceneAlmostReady();
            }

        }

       
        IEnumerator FinishLoadingScene( bool setSceneActive ) {

            // Finish loading the new scene
            asyncOperation.allowSceneActivation = true;
            while (!asyncOperation.isDone) {
                yield return null;
            }

            // Set the new scene as active
            if (setSceneActive) {
                Debug.Log("[SceneUtilities] Attempting to set '" + lastSceneStarted + "' as active");

                while (SceneManager.GetActiveScene().name != lastSceneStarted) {
                    SceneManager.SetActiveScene(SceneManager.GetSceneByName(lastSceneStarted));
                    yield return new WaitForSeconds(0.1f);
                }


                //yield return new WaitUntil(() => CheckSceneActive(lastSceneStarted));
                Debug.Log("[SceneUtilities] Set '" + lastSceneStarted + "' as active!");

            }

            // Wait until the dynamic GI is converged
            if (_Debug) Debug.Log("[SceneUtilities] Finished loading '" + lastSceneStarted + "'!");

            if (convergeDynamicGI) { 
                if (_Debug) Debug.Log("Waiting for dynamic GI to update");
                yield return new WaitUntil(() => DynamicGI.isConverged);
            }

            // Send an event out for the caller script to pick up
            if (OnSceneLoad != null) {
                if (_Debug) Debug.Log("[SceneUtilities] OnSceneLoad event called");
                OnSceneLoad();
            }


        }


        IEnumerator LoadingSceneExclusive ( string sceneToLoad ) {

            yield return StartCoroutine(UnloadingScene(SceneManager.GetActiveScene().name, 1, true));
            yield return BeginLoadingScene(sceneToLoad);
            yield return FinishLoadingScene(true);
        }



        IEnumerator UnloadingScene( string scene, int maxSceneCount, bool forceSceneCleanup ) {

            // Make sure the scene is not the manager scene
            if (scene == "ManagerScene") {
                Debug.LogError("[SceneUtilities] ERROR -> Cannot unload ManagerScene! It's too important! Stop that...");
                yield break;
            }

            // Unload the scene asynchronously
            asyncOperation = SceneManager.UnloadSceneAsync(scene);
            yield return new WaitUntil(() => !SceneManager.GetSceneByName(scene).isLoaded);
            yield return null;

            // Double check there are the right number of scenes loaded
            if (SceneManager.sceneCount > maxSceneCount) {
                if (_Debug) Debug.Log("[SceneUtilities] Too many scenes, waiting for scene cleanup to complete...");
                if (forceSceneCleanup) ForceSceneCleanup(maxSceneCount);
                yield return new WaitUntil(() => SceneManager.sceneCount <= maxSceneCount);
                if (_Debug) Debug.Log("[SceneUtilities] Scenes cleanup complete!");
            }

            // Flush any unloaded assets out of memory
            if (_Debug) Debug.Log("[SceneUtilities] '" + scene + "' unloaded, flushing unused assets");
            ForceUnloadUnusedAssets();


            // Send an event out for the caller script to pick up
            if (OnSceneUnload != null) {
                if (_Debug) Debug.Log("[SceneUtilities] OnSceneUnload event called");
                OnSceneUnload();
            }


        }



        #endregion




        #region Cleanup functions


        IEnumerator FlushingUnusedAssets() {

            if (_Debug) Debug.Log("[SceneUtilities] Flushing unused assets from memory");

            // Flush any unloaded assets out of memory
            asyncOperation = Resources.UnloadUnusedAssets();
            while (!asyncOperation.isDone) {
                yield return null;
            }

            if (_Debug) Debug.Log("[SceneUtilities] Unused assets flushed!");

        }
                       
        void ForceSceneCleanup( int maxSceneCount) {

            // Only the current scene and the ManagerScene are loaded
            if (SceneManager.sceneCount <= maxSceneCount) return;

            // Check which scenes have to be removed
            bool currentSceneFound = false;
            for (int i = 0; i < SceneManager.sceneCount; i++) {
                Scene scene = SceneManager.GetSceneAt(i);

                // Skip the ManagerScene
                if (scene.name == "ManagerScene") continue;

                // Skip the current scene if it's the first one found
                if (scene.name == lastSceneStarted && !currentSceneFound) {
                    currentSceneFound = true;
                    continue;
                }

                // Otherwise, unload the scene
                SceneManager.UnloadSceneAsync(scene);
                
            }
        }

        #endregion
    }

}


