using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace InteractSystem
{
    [System.Serializable]
    public class Config
    {
        private static Config _defult;
        public static Config Global
        {
            get
            {
                if (_defult == null)
                {
                    _defult = new Config();
                }
                return _defult;
            }
            set
            {
                _defult = value;
            }
        }

        public int _autoExecuteTime = 3;
        public int _hitDistence = 100;
        public int _elementFoward = 1;
        public bool _highLightNotice = true;//高亮提示
        public bool _useOperateCamera = true;//使用专用相机
        public bool _angleNotice = true;//箭头提示
        public bool _previewNotice = true;//实例提示
        public bool _quickMoveElement = false;//元素快速移动
        public bool _ignoreController = false;//忽略控制器
        public Material _lineMaterial = null;
        public float _lineWidth = 0.2f;
        public Color _highLightColor = Color.green;
        public GameObject _angleObj = null;
        public GameObject[] _angleObjs = null;
        public Material _previewMat;//预览材质
        public  float _previewAlpha = 0.5f;
        public List<Type> _actionItemBindings = new List<Type>();

        public static int autoExecuteTime { get { return Global._autoExecuteTime; } }
        public static int hitDistence { get { return Global._hitDistence; } }
        public static int elementFoward { get { return Global._elementFoward; } }
        public static bool highLightNotice { get { return Global._highLightNotice; } }
        public static bool useOperateCamera { get { return Global._useOperateCamera; } }
        public static bool angleNotice { get { return Global._angleNotice; } }
        public static bool previewNotice { get { return Global._previewNotice; } }
        public static bool quickMoveElement { get { return Global._quickMoveElement; } }
        public static bool ignoreController { get { return Global._ignoreController; } }
        public static Material lineMaterial { get { return Global._lineMaterial; } }
        public static float lineWidth { get { return Global._lineWidth; } }
        public static Color highLightColor { get { return Global._highLightColor; } }
        public static GameObject angleObj { get { return Global._angleObj; } }
        public static Material previewMat { get {
                if(Global._previewMat == null)
                {
                    Global._previewMat = new Material(Shader.Find("Unlit/Transparent"));
                }
                return Global._previewMat;
            }
        }
        public static float previewAlpha
        {
            get { return Global._previewAlpha; }
        }
        public static List<Type> actionItemBindings
        {
            get
            {
                return Global._actionItemBindings;
            }
        }
    }
}

