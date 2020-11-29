using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Paperticket {
    public class BasicAnimController : BaseAnimController {

        [Header("CONTROLS")]
        [Space(10)]
        [SerializeField] int startingPose;

        public override void OnEnable() {
            SetAnimation(startingPose);
        }


    }

}