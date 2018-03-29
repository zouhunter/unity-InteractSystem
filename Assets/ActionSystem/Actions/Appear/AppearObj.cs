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
    public class AppearObj : RuntimeObj
    {
        [System.Serializable]
        public class AppearHold
        {
            public string elementName;
            public Transform autoPos;
        }
        [SerializeField]
        private bool forceAuto;
        [SerializeField]
        private float spanTime = 1f;
        [SerializeField]
        private List<AppearHold> hoders;

        //可能需要用到的元素
        private ElementPool<ISupportElement> supportPool = new ElementPool<ISupportElement>();
        private ISupportElement[] finalGroup;
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

            UpdateElementPool();

            if (auto|| forceAuto)
            {
                StartCoroutine(AutoAppear());
            }
            else
            {
                TryComplete();
            }
        }
        public override void OnUnDoExecute()
        {
            base.OnUnDoExecute();

            supportPool.ForEach(ele =>
            {
                elementCtrl.UnLockElement(ele, this);
            });
            finalGroup = null;
            elementCtrl.ClearExtraCreated();
        }


        protected override void OnBeforeEnd(bool force)
        {
            base.OnBeforeEnd(force);
            var keys = new List<ISupportElement>();
            for (int i = 0; i < finalGroup.Length; i++)
            {
                var ele = finalGroup[i];
                if (ele != null)
                {
                    elementCtrl.LockElement(ele, this);
                    keys.Add(ele);
                }
            }
            elementCtrl.ClearExtraCreated();
        }
   
        /// <summary>
        /// 元素动态注册
        /// </summary>
        /// <param name="arg0"></param>
        protected override void OnRegistElement(ISupportElement arg0)
        {
            if (arg0.IsRuntimeCreated)
            {
                if (hoders.Find(x => x.elementName == arg0.Name) != null)
                {
                    supportPool.ScureAdd(arg0);
                    if(Started)
                    {
                        TryComplete();
                    }
                }
            }
        }

        protected override void OnRemoveElement(ISupportElement arg0)
        {
            if (arg0.IsRuntimeCreated)
            {
                var old = hoders.Find(x => x.elementName == arg0.Name);
                if (old != null)
                {
                    supportPool.ScureRemove(arg0);
                }
            }
        }
        /// <summary>
        /// 更新元素池
        /// </summary>
        private void UpdateElementPool()
        {
            var keys = new List<string>();
            foreach (var item in hoders)
            {
                if(!keys.Contains(item.elementName))
                {
                    keys.Add(item.elementName);
                    var elements = elementCtrl.GetElements<ISupportElement>(item.elementName);
                    if(elements!= null)
                    {
                        foreach (var ele in elements)
                        {
                            supportPool.ScureAdd(ele);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 当元素池中包含所有需要的元素时
        /// 说明当前步骤结束
        /// （）
        /// </summary>
        /// <returns></returns>
        private bool TryComplete()
        {
            var keys = new List<ISupportElement>();
            bool allComplete = true;

            if(finalGroup == null)
            {
                finalGroup = new ISupportElement[hoders.Count];
            }
            else
            {
                foreach (var item in finalGroup)
                {
                    if(item != null)
                    {
                        keys.Add(item);
                    }
                }
            }

            for (int i = 0; i < finalGroup.Length; i++)
            {
                if(finalGroup[i] == null)
                {
                    finalGroup[i] = supportPool.Find(x => x.Name == hoders[i].elementName && !keys.Contains(x));
                    if(finalGroup[i] != null)
                    {
                        keys.Add(finalGroup[i]);
                    }
                    else
                    {
                        allComplete = false;
                        break;
                    }
                }
            }

            if (allComplete)
            {
                OnEndExecute(false);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 自动创建并完成步骤
        /// </summary>
        /// <returns></returns>
        IEnumerator AutoAppear()
        {
            Debug.Log("AutoApper:" + this);
            var mark = new List<ISupportElement>();
            finalGroup = new ISupportElement[hoders.Count];
            for (int i = 0; i < hoders.Count; i++)
            {
                var hoder = hoders[i];
                var element = elementCtrl.TryCreateElement<ISupportElement>(hoders[i].elementName, system.transform);
                    if (element != null && hoder.autoPos)
                    {
                        element.Body.transform.position = hoder.autoPos.transform.position;
                        element.Body.transform.rotation = hoder.autoPos.transform.rotation;
                    }
                finalGroup[i] = element;
                mark.Add(element);
                
                if(element == null)
                {
                    Debug.LogError("can not create:" + hoders[i].elementName);
                }
                yield return new WaitForSeconds(spanTime);
            }

            TryComplete();
        }
    }

}