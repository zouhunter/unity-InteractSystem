using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace InteractSystem.Actions
{
    [NodeGraph.CustomNode("Operate/Connect", 13, "InteractSystem")]
    public class ConnectNode : Graph.OperaterNode
    {
        [System.Serializable]
        public class PointGroup
        {
            public int p1;
            public int p2;
            public Material material;
            public float width = 0.1f;
        }

        [SerializeField, Attributes.CustomField("材质")]
        public Material lineMaterial;

        [SerializeField, Attributes.CustomField("线宽")]
        public float lineWight = 0.1f;

        [SerializeField]
        protected CollectNodeFeature collectNodeFeature = new CollectNodeFeature(typeof(ConnectItem));
        public PointGroup[] connectGroup;

        protected float autoTime { get { return Config.Instence.autoExecuteTime; } }

        protected override List<OperateNodeFeature> RegistFeatures()
        {
            var features = base.RegistFeatures();

            collectNodeFeature.SetTarget(this);
            collectNodeFeature.onUpdateElement = OnUpdateFromPool;
            features.Add(collectNodeFeature);
            return features;
        }

        private void OnUpdateFromPool(ISupportElement arg0)
        {
            if (log)
            {
                Debug.Log("OnUpdateFromPool:" + arg0);
            }

            if (Statu == ExecuteStatu.Executing && !arg0.Active && arg0.OperateAble)
            {
                arg0.StepActive();
            }
        }

        public PointGroup GetConnectInfo(string itemA, string itemB)
        {
            var idA = collectNodeFeature.itemList.IndexOf(itemA);
            var idB = collectNodeFeature.itemList.IndexOf(itemB);
            if (idA >= 0 && idB >= 0)
            {
                var sid = idA | idB;
                var groupInfo = Array.Find(connectGroup, x => (x.p1 | x.p2) == sid);
                return groupInfo;
            }
            return null;
        }

        public override void OnStartExecute(bool auto = false)
        {
            base.OnStartExecute(auto);
            ConnectCtrl.Instence.RegistLock(this);
        }

        protected override void OnStartExecuteInternal()
        {
            base.OnStartExecuteInternal();
            if (auto)
            {
                CoroutineController.Instence.StartCoroutine(AutoConnectItems());
            }
        }

        public override void OnUnDoExecute()
        {
            base.OnUnDoExecute();
            DisconnetConnected();
            UnLockElements();
            ConnectCtrl.Instence.RemoveLock(this);
        }

        private void UnLockElements()
        {
            if (collectNodeFeature.finalGroup != null)
            {
                Array.ForEach(collectNodeFeature.finalGroup, item =>
                {
                    //解除本脚本对linkItem的锁定
                    (item as ConnectItem).RemovePlayer(this);
                    ElementController.Instence.UnLockElement(item, this);
                });
                collectNodeFeature.finalGroup = null;
            }
        }

        public override void OnEndExecute(bool force)
        {
            base.OnEndExecute(force);
            if (collectNodeFeature.finalGroup == null)
            {
                QuickConnectItems();
            }
            ConnectCtrl.Instence.RemoveLock(this);
        }

        public void TryComplete()
        {
            var list = new ConnectItem[collectNodeFeature.itemList.Count];
            //找到所有的组合,并判断是否已经连接
            for (int i = 0; i < connectGroup.Length; i++)
            {
                var groupi = connectGroup[i];
                var itemAName = collectNodeFeature.itemList[groupi.p1];
                var itemBName = collectNodeFeature.itemList[groupi.p2];
                ConnectItem itemA = collectNodeFeature.elementPool.Find(x => x.Name == itemAName && x is ConnectItem) as ConnectItem;
                ConnectItem itemB = collectNodeFeature.elementPool.Find(x => x.Name == itemBName && x is ConnectItem) as ConnectItem;

                if (itemA != null && itemB != null)
                {
                    if (!ConnectUtil.HaveConnected(itemA, itemB))
                    {
                        return;
                    }
                    else
                    {
                        list[groupi.p1] = itemA;
                        list[groupi.p2] = itemB;
                    }
                }
                else
                {
                    return;
                }
            }

            OnConnectOK(list);
            OnEndExecute(false);
        }


        private void QuickConnectItems()
        {
            throw new NotImplementedException();
        }

        private void DisconnetConnected()
        {
            if (collectNodeFeature.finalGroup != null)
                for (int i = 0; i < connectGroup.Length; i++)
                {
                    var groupi = connectGroup[i];
                    var itemA = collectNodeFeature.finalGroup[groupi.p1] as ConnectItem;
                    var itemB = collectNodeFeature.finalGroup[groupi.p2] as ConnectItem;
                    ConnectUtil.TryDisconnect(itemA, itemB);
                }
        }

        private string AutoConnectItems()
        {
            throw new NotImplementedException();
        }

        private void OnConnectOK(ConnectItem[] combination)
        {
            //Debug.Log("OnCombinationOK");
            collectNodeFeature.finalGroup = combination;
            foreach (var item in combination)
            {
                item.RecordPlayer(item);
                ElementController.Instence.LockElement(item, this);
            }
        }

    }
}