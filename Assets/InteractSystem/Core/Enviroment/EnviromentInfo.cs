using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace InteractSystem.Enviroment
{

    [System.Serializable]
    public class EnviromentInfo
    {
#if UNITY_EDITOR
        [HideInInspector]
        public int instenceID;//实例id
        [HideInInspector]
        public string guid;//对象索引
#endif
        public string enviromentName;//共用的环境控制对象
        public bool originalState;//初始状态
        public bool startState;//命令开始时的状态
        public bool completeState;//命令结束时的状态
        public Coordinate coordinate;//坐标参数
        public bool ignore;
        private string _id;
        public string ID
        {
            get
            {
                if (string.IsNullOrEmpty(_id))
                {
                    _id = string.Format("[{0}][{1}]", enviromentName, coordinate.StringValue);
                }
                return _id;
            }
        }
    }
}