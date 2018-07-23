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
            public string p1;
            public string p2;
            public Material material;
            public float width = 0.1f;
        }

        [SerializeField, Attributes.CustomField("材质")]
        public Material lineMaterial;

        [SerializeField, Attributes.CustomField("线宽")]
        public float lineWight = 0.1f;

        protected RuntimeCollectNodeFeature<ConnectItem> collectNodeFeature;
        public PointGroup[] connectGroup;
        private string[] _elements;
        public string[] elements
        {
            get
            {
                if(_elements == null)
                {
                    var list = new List<string>();
                    for (int i = 0; i < connectGroup.Length; i++)
                    {
                        var group = connectGroup[i];
                        if (!list.Contains(group.p1))
                        {
                            list.Add(group.p1);
                        }
                        if (!list.Contains(group.p2))
                        {
                            list.Add(group.p2);
                        }
                    }
                    _elements = list.ToArray();
                }
                return _elements;
            }
        }

        protected float autoTime { get { return Config.Instence.autoExecuteTime; } }
       
        protected override List<OperateNodeFeature> RegistFeatures()
        {
            var features = base.RegistFeatures();
            _elements = null;
            collectNodeFeature = new RuntimeCollectNodeFeature<ConnectItem>(elements);
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
            var groupInfo = Array.Find(connectGroup, x => (x.p1 == itemA && x.p2 == itemB) || (x.p2 == itemA && x.p1 == itemB));
            return groupInfo;
        }

        public override void OnStartExecute(bool auto = false)
        {
            base.OnStartExecute(auto);
            ConnectCtrl.Instence.RegistLock(this);
        }

        protected override void OnStartExecuteInternal()
        {
            base.OnStartExecuteInternal();
            if (auto) {
                CoroutineController.Instence.StartCoroutine(AutoConnectItems());
            }
        }

        public override void OnUnDoExecute()
        {
            base.OnUnDoExecute();
            if (auto){
                CoroutineController.Instence.StopCoroutine(AutoConnectItems());
            }
            DisconnetConnected();
            UnLockElements();
            ConnectCtrl.Instence.RemoveLock(this);
        }

        public override void OnEndExecute(bool force)
        {
            base.OnEndExecute(force);
            if (auto)
            {
                CoroutineController.Instence.StopCoroutine(AutoConnectItems());
            }

            if (collectNodeFeature.finalGroup == null) {
                QuickConnectItems();
            }

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


        public void TryComplete()
        {
            var list = new ConnectItem[elements.Length];
            //找到所有的组合,并判断是否已经连接
            for (int i = 0; i < connectGroup.Length; i++)
            {
                var groupi = connectGroup[i];
                var itemAName = groupi.p1;
                var itemBName = groupi.p2;
                var itemAID = Array.IndexOf(elements, itemAName);
                var itemBID = Array.IndexOf(elements, itemBName);

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
                        list[itemAID] = itemA;
                        list[itemBID] = itemB;
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
            ConnectItem[] list = null;
            if (collectNodeFeature.finalGroup != null)
            {
                list = Array.ConvertAll(collectNodeFeature.finalGroup, x => x as ConnectItem);
            }
            else
            {
                list = new ConnectItem[elements.Length];
            }

            //找到所有的组合,并判断是否已经连接
            for (int i = 0; i < connectGroup.Length; i++)
            {
                var groupi = connectGroup[i];
                var idA = Array.IndexOf(elements, groupi.p1);
                var idB = Array.IndexOf(elements, groupi.p2);
                var itemA = list[idA] as ConnectItem;
                var itemB = list[idB] as ConnectItem;

                if (ConnectUtil.HaveConnected(itemA, itemB))
                {
                    continue;
                }

                var itemAName = groupi.p1;
                var itemBName = groupi.p2;

                itemA = itemA != null ? itemA : collectNodeFeature.elementPool.Find(x => x.Name == itemAName && x is ConnectItem && x.OperateAble) as ConnectItem;
                itemB = itemB != null ? itemB : collectNodeFeature.elementPool.Find(x => x.Name == itemBName && x is ConnectItem && x.OperateAble) as ConnectItem;

                Debug.Assert(itemA != null && itemB != null);

                var connected = ConnectUtil.TryConnect(itemA, itemB, groupi);

                Debug.Assert(connected);

                list[idA] = itemA;
                list[idB] = itemB;
            }

            OnConnectOK(list);
        }

        private void DisconnetConnected()
        {
            if (collectNodeFeature.finalGroup != null)
                for (int i = 0; i < connectGroup.Length; i++)
                {
                    var groupi = connectGroup[i];
                    var idA = Array.IndexOf(elements, groupi.p1);
                    var idB = Array.IndexOf(elements, groupi.p2);
                    var itemA = collectNodeFeature.finalGroup[idA] as ConnectItem;
                    var itemB = collectNodeFeature.finalGroup[idB] as ConnectItem;
                    ConnectUtil.TryDisconnect(itemA, itemB);
                }
        }

        private IEnumerator AutoConnectItems()
        {
            ConnectItem[] list = new ConnectItem[elements.Length];
            Debug.Log("自动连接未完成部分");
            for (int i = 0; i < connectGroup.Length; i++)
            {
                var groupi = connectGroup[i];
                var idA = Array.IndexOf(elements, groupi.p1);
                var idB = Array.IndexOf(elements, groupi.p2);
                var itemA = list[idA] as ConnectItem;
                var itemB = list[idB] as ConnectItem;

                if (ConnectUtil.HaveConnected(itemA, itemB))
                {
                    continue;
                }

                var itemAName = groupi.p1;
                var itemBName = groupi.p2;

                itemA = itemA != null ? itemA : collectNodeFeature.elementPool.Find(x => x.Name == itemAName && x is ConnectItem && x.OperateAble) as ConnectItem;
                itemB = itemB != null ? itemB : collectNodeFeature.elementPool.Find(x => x.Name == itemBName && x is ConnectItem && x.OperateAble) as ConnectItem;

                Debug.Assert(itemA != null && itemB != null);

                var parent = itemA.GetInstanceID() > itemB.GetInstanceID() ? itemA : itemB;

                var lineRender = ConnectUtil.TryConnect(itemA, itemB, groupi);
                for (float timer = 0; timer < autoTime; timer += Time.deltaTime)
                {
                    var pos = Vector3.Lerp(itemA.transform.position, itemB.transform.position, timer / autoTime);
                    lineRender.SetPositions(new Vector3[] { itemA.transform.position,pos});
                    yield return null;
                }

                list[idA] = itemA;
                list[idB] = itemB;
            }
            OnConnectOK(list);
            OnEndExecute(false);
        }


        private void OnConnectOK(ConnectItem[] combination)
        {
            //Debug.Log("OnCombinationOK");
            collectNodeFeature.finalGroup = combination;
            foreach (var item in combination)
            {
                if(item != null)
                {
                    item.RecordPlayer(this);
                    ElementController.Instence.LockElement(item, this);
                }
            }
        }

    }
}