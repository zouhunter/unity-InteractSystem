using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{
    public interface AnimPlayer
    {
        void Init(UnityAction onAutoPlayEnd);
        void Play(float duration);
        void EndPlay();
        void UnDoPlay();
    }

}
