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
        public int autoExecuteTime;
        public int hitDistence;
        public int elementFoward ;
        public bool highLightNotice;//高亮提示
        public bool angleNotice;//箭头提示
        public bool useOperateCamera;//使用专用相机
        public bool quickMoveElement;//元素快速移动
        public bool ignoreController;//忽略控制器
        public Config()
        {
            autoExecuteTime = 3;
            hitDistence = 20;
            elementFoward = 1;
            highLightNotice = true;
            useOperateCamera = true;
            angleNotice = true;
            quickMoveElement = false;
            ignoreController = false;
        }
    }
}

