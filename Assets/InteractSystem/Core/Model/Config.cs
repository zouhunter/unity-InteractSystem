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
        private static Config Global
        {
            get
            {
                if (_defult == null)
                {
                    SetConfig();
                }
                return _defult;
            }
            set
            {
                _defult = value;
            }
        }

        private int _autoExecuteTime = 3;
        private int _hitDistence = 100;
        private int _elementFoward = 1;
        private bool _highLightNotice = true;//高亮提示
        private bool _useOperateCamera = true;//使用专用相机
        private bool _angleNotice = true;//箭头提示
        private bool _previewNotice = true;//实例提示
        private bool _quickMoveElement = false;//元素快速移动
        private bool _ignoreController = false;//忽略控制器
        private Material _lineMaterial = null;
        private float _lineWidth = 0.2f;
        private Color _highLightColor = Color.green;
        private GameObject _angleObj = null;
        private GameObject[] _angleObjs = null;
        private Material _previewMat;//预览材质
        private  float _previewAlpha = 0.5f;
        private List<Binding.OperaterBinding> _operateBindings = new List<Binding.OperaterBinding>();
        private List<Binding.ActionItemBinding> _actionItemBindings = new List<Binding.ActionItemBinding>();
        private List<Binding.CommandBinding> _commandBindings = new List<Binding.CommandBinding>();

        public static void SetConfig(Config config = null)
        {
            if (config == null)
            {
                _defult = new Config();
            }
            else
            {
                _defult = config;
            }
        }

        public static int autoExecuteTime { get { return Global._autoExecuteTime; } }
        public static int hitDistence { get { return Global._hitDistence; } }
        public static int elementFoward { get { return Global._elementFoward; } }
        public static bool highLightNotice { get { return Global._highLightNotice; }set { Global._highLightNotice = value; } }
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
        public static List< Binding.ActionItemBinding> actionItemBindings
        {
            get
            {
                return Global._actionItemBindings;
            }
        }
        public static List<Binding.CommandBinding> commandBindings
        {
            get
            {
                return Global._commandBindings;
            }
        }
        public static List<Binding.OperaterBinding> operateBindings
        {
            get
            {
                return Global._operateBindings;
            }
        }
    }
}

