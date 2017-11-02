using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{
    public static class Setting
    {
        internal const int elementFoward = 1;
        internal const int pickUpElementLayer = 8;
        internal const int installPosLayer = 9;
        internal const int matchPosLayer = 15;
        internal const int placePosLayer = 16;
        internal const int rotateItemLayer = 10;
        internal const int clickItemLayer = 11;
        internal const int connectItemLayer = 12;
        internal static int obstacleLayer = 13;
        internal static int ropeNodeLayer = 14;
        internal static int autoExecuteTime = 3;
        internal static int hitDistence = 20;
        internal static bool highLightNotice = true;//高亮提示
        internal static bool angleNotice = true;//箭头提示
        internal static bool useOperateCamera = true;//使用专用相机
        internal static bool ignoreMatch = false;//
        internal static bool ignoreInstall = false;

        internal static void ResetDefult()
        {
            highLightNotice = true;
            useOperateCamera = true;
            ignoreMatch = false;
            angleNotice = true;
            ignoreInstall = false;
        }
    }
}

