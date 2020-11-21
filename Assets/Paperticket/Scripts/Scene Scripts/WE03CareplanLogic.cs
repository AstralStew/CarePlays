using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WE03CareplanLogic : MonoBehaviour {

    [SerializeField] AudioSource taliaSource;
    [SerializeField] AudioClip[] taliaClips;

    int clipIndex;

    void OnEnable() {

        if (taliaSource == null) {
            Debug.LogError("[WE03CareplanLogic] ERROR -> No TaliaSource audio source defined! Disabling...");
            enabled = false;
        }

        if (taliaClips.Length == 0) {
            Debug.LogError("[WE03CareplanLogic] No clips defined! Disabling...");
            enabled = false;
        }
    }

    public void PlayClip( int index ) {

        if (taliaSource.isPlaying) taliaSource.Stop();
        taliaSource.clip = taliaClips[index];

        taliaSource.Play();


    }

    public void PlayNextClip() {
        clipIndex += 1;
        PlayClip(clipIndex);
    }


}
