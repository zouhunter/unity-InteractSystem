using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{
    public class Config
    {
        internal int autoExecuteTime = 3;
        internal int hitDistence = 20;
        internal int elementFoward = 1;
        internal bool highLightNotice = true;//高亮提示
        internal bool angleNotice = true;//箭头提示
        internal bool useOperateCamera = true;//使用专用相机
        internal bool quickMoveElement = false;//元素快速移动
        internal bool ignoreController = false;//忽略控制器

        internal void ResetDefult()
        {
            highLightNotice = true;
            useOperateCamera = true;
            angleNotice = true;
            quickMoveElement = false;
            ignoreController = false;
        }
    }
}

