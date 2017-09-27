using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace WorldActionSystem
{
    public abstract class MutiTimeTrigger : ActionTrigger
    {
        [Range(1,10)]
        public int repeat;
    }
}