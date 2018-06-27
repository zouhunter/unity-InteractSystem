using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace InteractSystem.Actions
{
    /// <summary>
    /// 创建出指定的元素
    /// </summary>
    [NodeGraph.CustomNode("Operate/Appear", 14, "InteractSystem")]
    public class AppearNode : Graph.OperaterNode
    {
        [SerializeField]
        private bool forceAuto;
        [SerializeField]
        private float spanTime = 1f;
        [SerializeField]
        private AutoAppearRule autoRule;
        protected CoroutineController coroutineCtrl { get { return CoroutineController.Instence; } }
        protected ElementController elementCtrl { get { return ElementController.Instence; } }

        [SerializeField]
        protected CollectNodeFeature collectNodeFeature = new CollectNodeFeature(typeof(ISupportElement));

        protected override List<OperateNodeFeature> RegistFeatures()
        {
            var features = base.RegistFeatures();
            collectNodeFeature.SetTarget(this);
            collectNodeFeature.onAddToPool = OnAddedToPool;
            features.Add(collectNodeFeature);
            return features;
        }

        public override void OnStartExecute(bool auto = false)
        {
            base.OnStartExecute(auto);
            if (log)
                Debug.Log("OnStartExecute:" + this);

            if(!TryComplete())
            {
                if (auto || forceAuto)
                {
                    Debug.Log("开启协程创建元素：" + this ,this);
                    coroutineCtrl.StartCoroutine(AutoAppear());
                }
            }
        }
        public override void OnUnDoExecute()
        {
            base.OnUnDoExecute();
            coroutineCtrl.StopCoroutine(AutoAppear());
            collectNodeFeature.elementPool.ForEach(ele =>{
                elementCtrl.UnLockElement(ele, this);
            });

            collectNodeFeature.finalGroup = null;
            elementCtrl.ClearExtraCreated();
        }

        protected override void OnBeforeEnd(bool force)
        {
            base.OnBeforeEnd(force);
            coroutineCtrl.StopCoroutine(AutoAppear());

            LockElements();
            elementCtrl.ClearExtraCreated();
        }

        protected void OnAddedToPool(ISupportElement arg0)
        {
            if(statu == ExecuteStatu.Executing)
            {
                TryComplete();
            }
        }

        /// <summary>
        /// 锁定元素
        /// </summary>
        private void LockElements()
        {
            if (collectNodeFeature. finalGroup == null)
                collectNodeFeature. finalGroup = new ActionItem[collectNodeFeature.itemList.Count];

            for (int i = 0; i < collectNodeFeature.finalGroup.Length; i++)
            {
                var ele = collectNodeFeature.finalGroup[i];
                if (ele == null || elementCtrl.IsLocked(ele))
                {
                    ele = collectNodeFeature.finalGroup[i] = CreateElement(collectNodeFeature.itemList[i]);
                    #region 由于注册需要在Start之后，怕来不及
                    var eles = ele.Body.GetComponentsInChildren<ISupportElement>();
                    Debug.Log("Create:" + ele);
                    elementCtrl.LockElement(ele, this);
                    elementCtrl.RegistElement(eles);
                    #endregion
                }
                else
                {
                    elementCtrl.LockElement(ele, this);
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

            if (collectNodeFeature. finalGroup == null)
            {
                collectNodeFeature.finalGroup = new ISupportElement[collectNodeFeature.itemList.Count];
            }
            else
            {
                foreach (var item in collectNodeFeature.finalGroup)
                {
                    if (item != null)
                    {
                        keys.Add(item);
                    }
                }
            }

            for (int i = 0; i < collectNodeFeature.finalGroup.Length; i++)
            {
                if (collectNodeFeature.finalGroup[i] == null)
                {
                    collectNodeFeature.finalGroup[i] = collectNodeFeature.elementPool.Find(x => x.Name == collectNodeFeature.itemList[i] && !keys.Contains(x) && !elementCtrl.IsLocked(x));
                    if (collectNodeFeature.finalGroup[i] != null)
                    {
                        keys.Add(collectNodeFeature.finalGroup[i]);
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
        private IEnumerator AutoAppear()
        {
            var mark = new List<ISupportElement>();
            if(collectNodeFeature.finalGroup == null) collectNodeFeature.finalGroup = new ActionItem[collectNodeFeature.itemList.Count];
            for (int i = 0; i < collectNodeFeature.itemList.Count; i++)
            {
                yield return new WaitForSeconds(spanTime);
                if(collectNodeFeature.finalGroup[i] == null)
                {
                    collectNodeFeature.finalGroup[i] = CreateElement(collectNodeFeature.itemList[i]);
                    mark.Add(collectNodeFeature.finalGroup[i]);
                }
            }
        }

        private ISupportElement CreateElement(string elementName)
        {
            var element = elementCtrl.TryCreateElement<ISupportElement>(elementName,Command.Context);
            if(autoRule) autoRule.OnCreate(element);
            Debug.Assert(element != null,elementName + " 为空！", this);
            return element;
        }
    }

}