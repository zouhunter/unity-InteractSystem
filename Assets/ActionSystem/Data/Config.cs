using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{
    [System.Serializable]
    public static class Config
    {
        public static int  autoExecuteTime = 3;
        public static int  hitDistence = 100;
        public static int  elementFoward = 1;
        public static bool highLightNotice = true;//高亮提示
        public static bool useOperateCamera = true;//使用专用相机
        public static bool angleNotice = true;//箭头提示
        public static bool quickMoveElement = false;//元素快速移动
        public static bool ignoreController = false;//忽略控制器
        public static Material lineMaterial = null;
        public static float lineWidth = 0.2f;
        public static Color highLightColor = Color.green;
        public static GameObject angleObj = null;
    }
}

