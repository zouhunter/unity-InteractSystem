using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem.Enviroment
{

    [System.Serializable]
    public class EnviromentItem
    {
        public bool originalState;//初始状态
        public bool startState;//命令开始时的状态
        public bool completeState;//命令结束时的状态
        public Matrix4x4 matrix;//坐标参数
        public EnviromentObj enviromentObj;//共用的环境控制对象
        public bool ignore;

        public void SetOriginal()
        {

        }

        public void SetStart()
        {

        }

        public void SetComplete()
        {

        }
    }
}