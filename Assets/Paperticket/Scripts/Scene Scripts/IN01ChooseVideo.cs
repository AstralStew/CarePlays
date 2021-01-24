using Paperticket;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class IN01ChooseVideo : MonoBehaviour {
       

    [SerializeField] UnityEvent2 FirstVideo;
    [SerializeField] UnityEvent2 SecondVideo;
    [SerializeField] UnityEvent2 ThirdVideo;
    [SerializeField] UnityEvent2 FourthVideo;
    [SerializeField] UnityEvent2 FifthVideo;
    [SerializeField] UnityEvent2 SixthVideo;


    void OnEnable() {

        int vidIndex = CareplaysManager.instance.IN01VideoIndex;

        if (vidIndex == 0) {
            if (FirstVideo != null) FirstVideo.Invoke();

        } else if (vidIndex == 1) {
            if (SecondVideo != null) SecondVideo.Invoke();

        } else if (vidIndex == 2) {
            if (ThirdVideo != null) ThirdVideo.Invoke();

        } else if (vidIndex == 3) {
            if (FourthVideo != null) FourthVideo.Invoke();

        } else if (vidIndex == 4) {
            if (FifthVideo != null) FifthVideo.Invoke();

        } else if (vidIndex == 5) {
            if (SixthVideo != null) SixthVideo.Invoke();

        } else Debug.LogError("[IN01ChooseVideo] ERROR -> Video index is out of bounds! Something has gone terribly wrong :(");
               
    }


}
