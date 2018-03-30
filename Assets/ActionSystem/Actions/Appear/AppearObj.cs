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
            if (log) Debug.Log("OnStartExecute:" + this);
            UpdateElementPool();

            if (auto || forceAuto)
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
            elementCtrl.ClearExtraCreated();
        }

        protected override void OnBeforeEnd(bool force)
        {
            base.OnBeforeEnd(force);
            LockElements();
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
                    if (Started && !Complete)
                    {
                        arg0.StepActive();
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
        /// 锁定元素
        /// </summary>
        private void LockElements()
        {
            for (int i = 0; i < finalGroup.Length; i++)
            {
                var ele = finalGroup[i];
                if (ele == null || elementCtrl.IsLocked(ele))
                {
                    ele = finalGroup[i] = CreateElement(hoders[i].elementName, hoders[i].autoPos);
                    #region 由于注册需要在Start之后，怕来不及
                    var eles = ele.Body.GetComponentsInChildren<ISupportElement>();
                    Debug.Log("Create:" + ele,gameObject);
                    elementCtrl.RegistElement(eles);
                    #endregion
                }
                elementCtrl.LockElement(ele, this);
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
                if (!keys.Contains(item.elementName))
                {
                    keys.Add(item.elementName);
                    var elements = elementCtrl.GetElements<ISupportElement>(item.elementName);
                    if (elements != null)
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
            //Debug.Log("TryComplete");
            if (ExistEnoughObj())
            {
                OnEndExecute(false);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 是否已经有足够的对象
        /// </summary>
        /// <returns></returns>
        private bool ExistEnoughObj()
        {
            var keys = new List<ISupportElement>();
            bool allComplete = true;

            if (finalGroup == null)
            {
                finalGroup = new ISupportElement[hoders.Count];
            }
            else
            {
                foreach (var item in finalGroup)
                {
                    if (item != null)
                    {
                        keys.Add(item);
                    }
                }
            }

            for (int i = 0; i < finalGroup.Length; i++)
            {
                if (finalGroup[i] == null)
                {
                    finalGroup[i] = supportPool.Find(x => x.Name == hoders[i].elementName && !keys.Contains(x) && !elementCtrl.IsLocked(x));
                    if (finalGroup[i] != null)
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
            return allComplete;
        }

        /// <summary>
        /// 自动创建并完成步骤
        /// </summary>
        /// <returns></returns>
        IEnumerator AutoAppear()
        {
            var mark = new List<ISupportElement>();
            finalGroup = new ISupportElement[hoders.Count];
            for (int i = 0; i < hoders.Count; i++)
            {
                yield return new WaitForSeconds(spanTime);
                var element = CreateElement(hoders[i].elementName, hoders[i].autoPos);
                finalGroup[i] = element;
                mark.Add(element);
            }
        }

        private ISupportElement CreateElement(string elementName, Transform autoPos)
        {
            var element = elementCtrl.TryCreateElement<ISupportElement>(elementName, system.transform);
            if (element != null && autoPos)
            {
                element.Body.transform.position = autoPos.transform.position;
                element.Body.transform.rotation = autoPos.transform.rotation;
            }

            Debug.Assert(element != null,elementName, this);
            return element;
        }
    }

}