using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;


namespace WorldActionSystem
{
    /// <summary>
    /// 创建出指定的元素
    /// </summary>
    public class AppearObj : ActionObj
    {
        [System.Serializable]
        public class AppearHold
        {
            public string objName;
            public ISupportElement body;
        }

        [SerializeField]
        private List<AppearHold> objectNames;
        public override ControllerType CtrlType
        {
            get
            {
               return ControllerType.Create;
            }
        }

        /// <summary>
        /// 元素动态注册
        /// </summary>
        /// <param name="arg0"></param>
        protected override void OnRegistElement(ISupportElement arg0)
        {
            base.OnRegistElement(arg0);
            if(arg0.IsRuntimeCreated)
            {

                if (objectNames.Find(x=>x.body == arg0) == null)
                {
                   var hold = objectNames.Find(x => x.objName == arg0.Name && x.body == null);
                    Debug.Log(hold);
                    if (hold != null)
                    {
                        hold.body = arg0;
                        TryComplete();
                    }
                }
            }
        }

        protected override void OnRemoveElement(ISupportElement arg0)
        {
            base.OnRemoveElement(arg0);
            if (arg0.IsRuntimeCreated)
            {
                var old = objectNames.Find(x => x.body == arg0);
                old.body = null;
            }
        }

        private void TryComplete()
        {
            if (objectNames.Find(x => x.body == null) == null)
            {
                OnEndExecute(false);
            }
        }
    }

}