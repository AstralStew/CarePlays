using Paperticket;
using UnityEngine;
using UnityEngine.Events;

public class WE03ChooseVideo : MonoBehaviour {
       

    [SerializeField] UnityEvent2 FirstVideo;
    [SerializeField] UnityEvent2 SecondVideo;


    void OnEnable() {

        int vidIndex = CareplaysManager.instance.WE03VideoIndex;

        if (vidIndex == 0) {
            if (FirstVideo != null) FirstVideo.Invoke();

        } else if (vidIndex == 1) {
            if (SecondVideo != null) SecondVideo.Invoke();

        } else Debug.LogError("[WE03ChooseVideo] ERROR -> Video index is out of bounds! Something has gone terribly wrong :(");
               
    }


}
