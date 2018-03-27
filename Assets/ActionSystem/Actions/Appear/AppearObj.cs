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
            public Transform autoPos;
            public ISupportElement element;
        }
        [SerializeField]
        private float spanTime = 1f;
        [SerializeField]
        private List<AppearHold> hoders;
        public override ControllerType CtrlType
        {
            get
            {
                return ControllerType.Create;
            }
        }

        public override void OnStartExecute(bool auto = false)
        {
            base.OnStartExecute(auto);
            if (auto)
            {
                StartCoroutine(AutoAppear());
            }
        }

        public override void OnUnDoExecute()
        {
            base.OnUnDoExecute();
            foreach (var item in hoders)
            {
                if (item.element != null && item.element.Body != null)
                {
                    item.element.Used = false;
                    item.element = null;
                }
            }
            ElementController.Instence.ClearRuntimeCreated();
        }
        public override void OnEndExecute(bool force)
        {
            base.OnEndExecute(force);
            foreach (var item in hoders)
            {
                if(item.element != null)
                {
                    item.element.Used = true;
                }
            }
        }
        /// <summary>
        /// 元素动态注册
        /// </summary>
        /// <param name="arg0"></param>
        protected override void OnRegistElement(ISupportElement arg0)
        {
            base.OnRegistElement(arg0);

            if (arg0.IsRuntimeCreated)
            {
                if (hoders.Find(x => x.element == arg0) == null)
                {
                    var hold = hoders.Find(x => x.objName == arg0.Name && x.element == null);
                    Debug.Log(hold);
                    if (hold != null)
                    {
                        hold.element = arg0;
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
                var old = hoders.Find(x => x.element == arg0);
                if (old != null)
                {
                    old.element = null;
                }
            }
        }

        private void TryComplete()
        {
            if (hoders.Find(x => x.element == null) == null)
            {
                OnEndExecute(false);
            }
        }

        IEnumerator AutoAppear()
        {
            bool haveError = false;
            for (int i = 0; i < hoders.Count; i++)
            {
                var hoder = hoders[i];
                if (hoder.element == null)
                {
                    var item = elementCtrl.TryCreateElement<ISupportElement>(hoders[i].objName);
                    if (item == null)
                    {
                        Debug.Log("can not create:" + hoders[i].objName);
                        haveError = true;
                    }
                    else
                    {
                        if (hoder.autoPos)
                        {
                            item.Body.transform.position = hoder.autoPos.transform.position;
                            item.Body.transform.rotation = hoder.autoPos.transform.rotation;
                        }

                        yield return new WaitForSeconds(spanTime);
                    }
                }
            }
            if (!haveError)
            {
                OnEndExecute(false);
            }
        }
    }

}