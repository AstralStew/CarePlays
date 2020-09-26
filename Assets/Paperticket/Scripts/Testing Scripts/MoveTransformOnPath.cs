using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using UnityEditor.Experimental.GraphView;
using System.Globalization;

public class MoveTransformOnPath : MonoBehaviour
{
    
    [SerializeField] PathCreator path;

    [SerializeField] float speed;

    [SerializeField] EndOfPathInstruction endInstruction = EndOfPathInstruction.Stop;

    [SerializeField] bool flipOnReverse;

    [SerializeField] bool debugging;

    float currentTime;
    int targetTime = 1;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    float timeIncrement;
    void Update()
    {
        if (currentTime != targetTime) {
            timeIncrement = (targetTime==1?1:-1) * Time.deltaTime * speed;
            currentTime = Mathf.Clamp01(currentTime + timeIncrement);
            SetFishProgress(currentTime);
            return;
        }

        if (endInstruction == EndOfPathInstruction.Reverse) {
            targetTime = -targetTime + 1;
            if (flipOnReverse) {
                transform.Rotate(0, 180, 0);
            }
        } else if (endInstruction == EndOfPathInstruction.Loop) {
            timeIncrement = Time.deltaTime * speed;
            currentTime = Mathf.Clamp01(currentTime + timeIncrement);
            SetFishProgress(currentTime);            
        } else {
            enabled = false;
        }
               
        //switch (endInstruction) {
        //    case EndOfPathInstruction.Reverse:
        //        if (flipOnReverse) {
        //            transform.Rotate(0, 180, 0);
        //        }
        //        break;

        //    case EndOfPathInstruction.Loop:
        //    case EndOfPathInstruction.Stop:
        //    default:
        //        break;
        //}


        //if (path.path.GetClosestTimeOnPath(transform.position) != 1) {
        //    currentTime = path.path.GetClosestTimeOnPath(transform.position);
        //    SetFishProgress(currentTime);
        //}



        //if (currentTime != 1) {
        //    timeIncrement = Time.deltaTime * speed * (endInstruction == EndOfPathInstruction.Reverse) ? ;
        //    currentTime = Mathf.Clamp01(currentTime + timeIncrement);
        //    SetFishProgress(currentTime);
        //    return;
        //}

    }

    void SetFishProgress( float progress ) {

        transform.position = path.path.GetPointAtTime(progress, endInstruction);
        //transform.rotation = path.path.GetRotation(progress, EndOfPathInstruction.Stop);
    }
}
