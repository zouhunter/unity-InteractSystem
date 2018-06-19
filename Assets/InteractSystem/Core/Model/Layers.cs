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

namespace InteractSystem
{
    /// <summary>
    /// this Layers may changed
    /// </summary>
    public class Layers
    {
        public const string pickUpElementLayer = "i:pickupelement";
        public const string rotateItemLayer = "i:rotateitem";
        public const string clickItemLayer = "i:clickitem";
        public const string connectItemLayer = "i:connectitem";
        public const string obstacleLayer = "i:obstacle";
        public const string ropePosLayer = "i:ropepos";
        public const string ropeNodeLayer = "i:ropenode";
        public const string chargeResourceLayer = "i:chargeresource";
        public const string chargeObjLayer = "i:chargeobj";
        public const string placePosLayer = "i:placepos";
        public const string dragItemLayer = "i:dragitem";
        public const string linknodeLayer = "i:linknode";
        public const string detachItemLayer = "i:detachitem";
    }

}