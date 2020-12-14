using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Paperticket {

    #region public enums
    public enum AssetBundles { desert, menu, menuscene, we01, we01scene, we02, we02scene, we03, we03scene, we04, we04scene }
    public enum CareScene { DesertMenu, WE01_Onboarding, WE02_Jetty, WE03_Dawn, WE04_Finale }
    #endregion

    public class CareplaysManager : MonoBehaviour {

        public static CareplaysManager instance = null;

        public List<CareSceneInfo> careSceneManifest = null;

        [Space(10)]
        [SerializeField] bool loadFirstScene;
        public CareScene firstScene = CareScene.DesertMenu;

        [Space(10)] 
        [SerializeField] bool debugging = false;

        void Awake() {
            StartCoroutine(Initialising());
        }
        IEnumerator Initialising() {

            if (!debugging) debugging = true;

            // Set as the CareplaysManager static instance
            if (instance == null) {
                instance = this;
            } else if (instance != this) {
                Destroy(gameObject);
            }

            // Wait for other utility scripts to be ready
            yield return new WaitUntil(() => PTUtilities.instance != null);
            yield return new WaitUntil(() => SceneUtilities.instance != null);
            yield return new WaitUntil(() => DataUtilities.instance != null);
            yield return new WaitUntil(() => DataUtilities.instance.finishedInitialising = true);

            // Load the first careplays scene
            if (loadFirstScene) LoadCareScene(firstScene);
        }


        public void LoadCareScene( CareScene careScene ) {
            if (debugging) Debug.Log("[CareplaysManager] Attempting to load CareScene '"+careScene.ToString()+"'...");
            StartCoroutine(LoadingCareScene(careScene));
        }


        IEnumerator LoadingCareScene( CareScene careScene ) {
            CareSceneInfo newSceneInfo = null;
            string newSceneName = "";
            List<AssetBundles> newBundles = null;

            // Grab the scene info for the new scene from the manifest
            foreach (CareSceneInfo sceneInfo in careSceneManifest) {
                if (sceneInfo.careScene == careScene) {
                    newSceneInfo = sceneInfo;
                    break;
                }
            }
            if (newSceneInfo == null) {
                Debug.LogError("[CareplaysManager] ERROR -> No scene info found for CareScene '"+careScene.ToString()+"'! This is a fatal error :( ");
                yield break;
            }

            // Extract the scene info
            newSceneName = newSceneInfo.sceneName;
            newBundles = newSceneInfo.requiredBundles;

            if (debugging) Debug.Log("[CareplaysManager] CareScene '" + careScene.ToString() + "' info loaded! \n" +
                                     "CareScene name = " + newSceneName + "\n" +
                                     "Required bundles = " + newBundles.ToString());

            // Unload the current active scene            
            if (SceneUtilities.instance.SceneCount > 1) {
                if (debugging) Debug.Log("[CareplaysManager] Unloading the active scene...");
                SceneUtilities.instance.UnloadActiveScene(1, true);
                yield return new WaitUntil(() => SceneUtilities.instance.SceneCount == 1);
            } else if (debugging) Debug.Log("[CareplaysManager] No scenes to unload, skipping...");

            // Unload any asset bundles that are not required for the new scene
            if (DataUtilities.instance.GetLoadedBundles() != null) {
                if (debugging) Debug.Log("[CareplaysManager] Unloading the old bundles...");
                foreach (AssetBundles oldBundle in DataUtilities.instance.GetLoadedBundles()) {
                    if (!newBundles.Contains(oldBundle)) {
                        DataUtilities.instance.UnloadAssetBundle(oldBundle, true);
                        yield return new WaitUntil(() => !DataUtilities.instance.isBundleLoaded(oldBundle));
                    }
                    yield return null;
                }
            } else if (debugging) Debug.Log("[CareplaysManager] No old bundles to unload, skipping...");

            // Load any asset bundles that are required for the new scene
            foreach (AssetBundles newBundle in newBundles) {
                if (!DataUtilities.instance.isBundleLoaded(newBundle)) {
                    DataUtilities.instance.LoadAssetBundle(newBundle);
                    yield return new WaitUntil(() => DataUtilities.instance.isBundleLoaded(newBundle));
                }                
            }

            // Begin loading the next scene
            if (debugging) Debug.Log("[CareplaysManager] Loading the new scene...");
            SceneUtilities.instance.LoadScene(newSceneName, true);
            yield return new WaitUntil(() => SceneUtilities.instance.CheckSceneLoaded(newSceneName));
        }



    }


    #region CareSceneInfo class

    [Serializable]
    public class CareSceneInfo {

        public string sceneName;
        public CareScene careScene;
        public List<AssetBundles> requiredBundles;

        public CareSceneInfo() {
            sceneName = "Insert Scene Name";
            careScene = 0;
            requiredBundles = null;
        }
    }

    #endregion

}