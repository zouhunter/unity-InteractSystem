using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace WorldActionSystem
{

    [AddComponentMenu(MenuName.LinkObj)]
    public class LinkObj : ActionObj
    {

        public override ControllerType CtrlType
        {
            get
            {
                return ControllerType.Link;
            }
        }

        [SerializeField]
        private string[] linkItemsName;
        [SerializeField]
        private List<LinkGroup> defultLink;

        private Coroutine coroutine;
        public LinkHold[] linkedItems { get; private set; }

        private List<LinkItem> linkPool = new List<LinkItem>();
        protected override void Start()
        {
            base.Start();
            TryFindElements();
        }
        /// <summary>
        /// 从场景中找到已经存在的元素
        /// </summary>
        private void TryFindElements()
        {
            linkedItems = new LinkHold[linkItemsName.Length];
            for (int i = 0; i < linkedItems.Length; i++)
            {
                linkedItems[i] = new LinkHold();
            }

            linkPool.Clear();
            for (int i = 0; i < linkItemsName.Length; i++)
            {
                var links = elementCtrl.GetElements<LinkItem>(linkItemsName[i]);
                if (links != null)
                {
                    foreach (var item in links)
                    {
                        if (!linkPool.Contains(item))
                        {
                            linkPool.Add(item);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="arg0"></param>
        protected override void OnRegistElement(ISupportElement arg0)
        {
            base.OnRegistElement(arg0);
            if (arg0 is LinkItem)
            {
                var linkItem = arg0 as LinkItem;
                if (!linkPool.Contains(linkItem))
                {
                    linkPool.Add(linkItem);
                    if (Started)
                    {
                        linkItem.StepActive();
                        linkItem.BindingTarget = this;
                        ActiveOneLinkItem();
                    }
                    arg0.IsRuntimeCreated = true;
                }
                else
                {
                    arg0.IsRuntimeCreated = false;
                }
            }
        }
        /// <summary>
        /// 注销
        /// </summary>
        /// <param name="arg0"></param>
        protected override void OnRemoveElement(ISupportElement arg0)
        {
            base.OnRemoveElement(arg0);
            if (arg0.IsRuntimeCreated && arg0 is LinkItem)
            {
                var linkItem = arg0 as LinkItem;
                if (linkPool.Contains(linkItem))
                {
                    linkPool.Remove(linkItem);
                }
            }
        }

        /// <summary>
        /// 激活连接点
        /// </summary>
        public void ActiveOneLinkItem()
        {
            var notLinked = linkPool.Where(x => x != null && x.BindingTarget == this && !HaveConnected(x)).FirstOrDefault();
            if (notLinked != null)
            {
                angleCtrl.UnNotice(anglePos);
                anglePos = notLinked.transform;
            }
            else
            {
                TryComplete();
            }
        }

        /// <summary>
        /// 判断是否已经连接过了
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private bool HaveConnected(LinkItem item)
        {
            return Array.Find(linkedItems, x => x.linkItem == item) != null;
        }

        /// <summary>
        /// 激活匹配点
        /// </summary>
        /// <param name="pickedUp"></param>
        public void TryActiveLinkPort(LinkItem pickedUp)
        {
            for (int i = 0; i < pickedUp.ChildNodes.Count; i++)
            {
                var node = pickedUp.ChildNodes[i];
                if (node.ConnectedNode == null && node.connectAble.Count > 0)
                {
                    for (int j = 0; j < node.connectAble.Count; j++)
                    {
                        var info = node.connectAble[j];

                        var otheritem = (from x in linkPool
                                         where (x != null && x != pickedUp && x.Name == info.itemName && x.BindingTarget == this)
                                         select x).FirstOrDefault();

                        if (otheritem != null)
                        {
                            var otherNode = otheritem.ChildNodes[info.nodeId];
                            if (otherNode != null && otherNode.ConnectedNode == null)
                            {
                                angleCtrl.UnNotice(anglePos);
                                anglePos = otherNode.transform;
                            }
                        }
                    }
                }
            }

        }

        public override void OnStartExecute(bool auto = false)
        {
            base.OnStartExecute(auto);

            OnStepActive();

            if (auto)
            {
                if (coroutine == null)
                {
                    coroutine = StartCoroutine(AutoLinkItems());
                    Debug.Log("StartCoroutine");
                }
            }
            else
            {
                ActiveOneLinkItem();
            }
        }

        /// <summary>
        /// 获取需要数量的linkItem
        /// </summary>
        private void OnStepActive()
        {
            foreach (var item in linkItemsName)
            {
                var linkItem = linkPool.Find(x => !x.Started && x.Name == item);
                if(linkItem != null)
                {
                    linkItem.BindingTarget = this;
                    linkItem.StepActive();
                }
                else
                {
                    linkItem = elementCtrl.TryCreateElement<LinkItem>(item);
                }
            }
        }


        protected override void OnBeforeEnd(bool force)
        {
            base.OnBeforeEnd(force);
            if (coroutine != null)
                StopCoroutine(coroutine);

            foreach (var item in linkedItems)
            {
                item.linkItem.PickUpAble = false;
            }
        }

        public override void OnUnDoExecute()
        {
            base.OnUnDoExecute();
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }
            LinkUtil.DetachConnectedPorts(linkedItems, transform);
            ResetConnected();
        }
        public void TryComplete()
        {
            bool allConnected = true;
            foreach (var item in linkedItems){
                allConnected &= (item.linkItem != null);
            }
            if(allConnected)
            {
                OnEndExecute(false);
            }
        }

        private void ResetConnected()
        {
            for (int i = 0; i < linkedItems.Length; i++)
            {
                var item = linkedItems[i];
                if(item.linkItem != null)
                {
                    item.linkItem.StepUnDo();
                    linkedItems[i] = null;
                }
            }
        }

        private IEnumerator AutoLinkItems()
        {
            for (int i = 0; i < defultLink.Count; i++)
            {
                var linkGroup = defultLink[i];

                var itemA = TryUseOneLinkItem(linkGroup.ItemA);
                var itemB = TryUseOneLinkItem(linkGroup.ItemB);

                if (itemA == null) continue;
                if (itemB == null) continue;

                var portA = itemA.ChildNodes[linkGroup.portA];
                var portB = itemB.ChildNodes[linkGroup.portB];

                angleCtrl.UnNotice(anglePos);
                anglePos = portA.transform;

                yield return MoveBToA(portA, portB);
                LinkUtil.AttachNodes(portB, portA);
                LinkUtil.RecordToDic(linkedItems, portA);
                LinkUtil.RecordToDic(linkedItems, portB);
            }
            TryComplete();
        }

        private LinkItem TryUseOneLinkItem(int id)
        {
            if (linkedItems.Length <= id) return null;
           

            if (linkedItems[id].linkItem == null)
            {
                var mayLinks = linkPool.FindAll(x => x.Name == linkItemsName[id] && x.BindingTarget == this);
                foreach (var linkItem in mayLinks)
                {
                    if (Array.Find(linkedItems, x => x.linkItem == linkItem) == null)
                    {
                        linkedItems[id].linkItem = linkItem;
                        break;
                    }
                }
            }

            if (linkedItems[id].linkItem == null)
            {
                linkedItems[id].linkItem = elementCtrl.TryCreateElement<LinkItem>(linkItemsName[id]);
            }

            if (linkedItems[id].linkItem != null)
            {
                linkedItems[id].linkItem.Connected = true;
            }

            return linkedItems[id].linkItem;
        }

        private IEnumerator MoveBToA(LinkPort portA, LinkPort portB)
        {
            //var linkInfoA = portA.connectAble.Find(x => x.itemName == portB.Body.Name);
            var linkInfoB = portB.connectAble.Find(x => x.itemName == portA.Body.Name);

            var pos = portA.Body.Trans.TransformPoint(linkInfoB.relativePos);
            var forward = portA.Body.Trans.TransformDirection(linkInfoB.relativeDir);
            var startPos = portB.Body.transform.localPosition;
            var startforward = portB.Body.transform.forward;

            for (float j = 0; j < 1f; j += Time.deltaTime)
            {
                portB.Body.transform.localPosition = Vector3.Lerp(startPos, pos, j);
                portB.Body.transform.forward = Vector3.Lerp(startforward, forward, j);
                yield return null;
            }

        }
    }
}