using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Paperticket;

public class CrossSceneEventHelper : MonoBehaviour
{
    

    public void LoadNextScene (string sceneName) {
        SceneUtilities.instance.BeginLoadScene(sceneName);
        SceneUtilities.OnSceneAlmostReady += LoadSceneCallback;

    }

    void LoadSceneCallback() {

        SceneUtilities.OnSceneAlmostReady -= LoadSceneCallback;
        SceneUtilities.instance.FinishLoadScene(true);
        SceneUtilities.instance.UnloadScene(gameObject.scene.name);

    }

}
