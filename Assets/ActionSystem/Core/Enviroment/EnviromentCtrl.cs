using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace WorldActionSystem.Enviroment
{
    public class EnviromentCtrl
    {
        private EnviromentItem[] environments;

        public EnviromentCtrl(EnviromentItem[] environments)
        {
            this.environments = environments;
        }

        internal void OrignalState()
        {
            //设置环境为初始状态
        }

        internal void StartState()
        {
            //设置环境为激活状态
        }
        internal void CompleteState()
        {
            //设置环境为结束状态
        }
    }
}
