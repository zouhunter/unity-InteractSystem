using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{
    [System.Serializable]
    public class Config
    {
        public int autoExecuteTime = 3;
        public int hitDistence = 20;
        public int elementFoward = 1;
        public bool highLightNotice = true;//高亮提示
        public bool angleNotice = true;//箭头提示
        public bool useOperateCamera = true;//使用专用相机
        public bool quickMoveElement = false;//元素快速移动
        public bool ignoreController = false;//忽略控制器

        public void ResetDefult()
        {
            highLightNotice = true;
            useOperateCamera = true;
            angleNotice = true;
            quickMoveElement = false;
            ignoreController = false;
        }
    }
}

