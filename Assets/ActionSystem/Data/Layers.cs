using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.Sprites;
using UnityEngine.Scripting;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.Assertions.Must;
using UnityEngine.Assertions.Comparers;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{
    /// <summary>
    /// this Layers may changed
    /// </summary>
    public class Layers
    {
        internal const int pickUpElementLayer = 8;
        internal const int rotateItemLayer = 10;
        internal const int clickItemLayer = 11;
        internal const int connectItemLayer = 12;
        internal static int obstacleLayer = 13;
        internal static int ropeNodeLayer = 14;
        internal const int placePosLayer = 16;
        internal const int dragPosLayer = 17;
    }

}