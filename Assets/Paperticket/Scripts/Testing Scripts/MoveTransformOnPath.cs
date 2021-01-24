using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using System.Globalization;

public class MoveTransformOnPath : MonoBehaviour {

    [Header("REFERENCES")]
    
    [SerializeField] PathCreator path;

    [Header("CONTROLS")]
    [Space(10)]
    [SerializeField] [Range(0,0.99f)] float startTime = 0;
    [SerializeField] float speed = 0;
    [Space(5)]
    [SerializeField] EndOfPathInstruction endInstruction = EndOfPathInstruction.Stop;
    [SerializeField] [Min(0)] float endPause = 0;
    [SerializeField] bool flipOnReverse = false;
    [Space(5)]
    [SerializeField] bool debugging = false;

    float currentTime = 0;
    int targetTime = 1;

    // Start is called before the first frame update
    void OnEnable() {

        if (startTime != 0) currentTime = startTime;

    }

    // Update is called once per frame
    float timeIncrement = 0;
    float pauseTime = 0;
    void Update() {

        // Wait until its been 1 full rotation
        if (currentTime != targetTime) {
            timeIncrement = (targetTime==1?1:-1) * Time.deltaTime * speed;
            currentTime = Mathf.Clamp01(currentTime + timeIncrement);
            SetFishProgress(currentTime);
            return;
        }

        if (endPause > 0) {
            if (pauseTime == 0) {
                pauseTime = Time.time;
                return;
            } else if (pauseTime + endPause > Time.time) {
                return;
            }            
        }

        if (endInstruction == EndOfPathInstruction.Reverse) {
            targetTime = -targetTime + 1;
            if (flipOnReverse) {
                transform.Rotate(0, 180, 0);
            }
        } else if (endInstruction == EndOfPathInstruction.Loop) {
            currentTime = 0;
            SetFishProgress(currentTime);            
        } else {
            enabled = false;
        }

        pauseTime = 0;

    }

    void SetFishProgress( float progress ) {

        transform.position = path.path.GetPointAtTime(progress, endInstruction);
    }
}
