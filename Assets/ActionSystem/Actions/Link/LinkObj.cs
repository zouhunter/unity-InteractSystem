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

        public LinkHold[] linkItems;
        [SerializeField]
        private List<LinkGroup> defultLink;
        private Coroutine coroutine;
        private List<string> elementKeys = new List<string>();
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
            linkPool.Clear();
            elementKeys.Clear();
            for (int i = 0; i < linkItems.Length; i++)
            {
                var elementName = linkItems[i].elementName;
                if (!elementKeys.Contains(elementName))
                {
                    elementKeys.Add(elementName);
                    var links = elementCtrl.GetElements<LinkItem>(elementName);
                    if (links != null)
                    {
                        foreach (var item in links)
                        {
                            if (!linkPool.Contains(item))
                            {
                                linkPool.Add(item);
                                item.linkObjs.Add(this);
                            }
                        }
                    }
                }
                
            }
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
                                         where (x != null && x != pickedUp && x.Name == info.itemName)
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
        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="arg0"></param>
        protected override void OnRegistElement(ISupportElement arg0)
        {
            base.OnRegistElement(arg0);
            if (arg0 is LinkItem)
            {
                if(elementKeys.Contains(arg0.Name))
                {
                    var linkItem = arg0 as LinkItem;
                    if (!linkPool.Contains(linkItem))
                    {
                        linkPool.Add(linkItem);
                        linkItem.linkObjs.Add(this);

                        if (Started){
                            linkItem.StepActive();
                            ActiveOneLinkItem();
                        }
                    }
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
                    linkItem.linkObjs.Add(this);
                    linkPool.Remove(linkItem);
                }
            }
        }

        /// <summary>
        /// 激活连接点
        /// </summary>
        public void ActiveOneLinkItem()
        {
            if(!TryComplete())
            {
                var notLinked = linkPool.Where(x => x != null && !HaveConnected(x)).FirstOrDefault();
                if (notLinked != null)
                {
                    angleCtrl.UnNotice(anglePos);
                    anglePos = notLinked.transform;
                }
            }
        }

        /// <summary>
        /// 判断是否已经连接过了
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private bool HaveConnected(LinkItem item)
        {
            bool connected = false;
            foreach (var child in item.ChildNodes)
            {
                connected |= (child.ConnectedNode != null);
            }
            return connected;
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
            foreach (var item in linkItems)
            {
                var linkItem = linkPool.Find(x => x.Name == item.elementName);
                if (linkItem != null)
                {
                    linkItem.StepActive();
                }
            }
        }


        protected override void OnBeforeEnd(bool force)
        {
            base.OnBeforeEnd(force);
            if (coroutine != null)
                StopCoroutine(coroutine);

            foreach (var item in linkItems)
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
            ResetConnected();
        }
        /// <summary>
        /// 找到满足条件的连接体
        /// </summary>
        public bool TryComplete()
        {
            Debug.Log("TryComplete");
            bool allConnected = true;
            var mark = new List<object>();

            foreach (var item in linkItems)
            {
                var linkItem = linkPool.Find(x =>x.Name == item.elementName && HaveConnected(x) && !mark.Contains(x));
                if(linkItem != null)
                {
                    item.linkItem = linkItem;
                    mark.Add(linkItem);
                    //item.linkedPorts = linkItem.Connected
                }
                else
                {
                    allConnected = false;
                    CleanRecord();
                    break;
                }
            }

            if (allConnected)
            {
                OnEndExecute(false);
                return true;
            }
            else
            {
                return false;
            }
        }
        private void CleanRecord()
        {
            foreach (var litem in linkItems)
            {
                litem.linkItem = null;
            }
        }

        private void ResetConnected()
        {
            for (int i = 0; i < linkItems.Length; i++)
            {
                var item = linkItems[i];
                if (item.linkItem != null)
                {
                    item.linkItem.StepUnDo();
                    linkItems[i].linkItem = null;
                    //linkItems[i].linkedPorts = new List<LinkPort>();
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
            }
            TryComplete();
        }

        private LinkItem TryUseOneLinkItem(int id)
        {
            if (linkItems.Length <= id) return null;

            if (linkItems[id].linkItem == null)
            {
                var mayLinks = linkPool.FindAll(x => x.Name == linkItems[id].elementName);
                foreach (var linkItem in mayLinks)
                {
                    if (Array.Find(linkItems, x => x.linkItem == linkItem) == null)
                    {
                        linkItems[id].linkItem = linkItem;
                        linkItems[id].linkItem.Used = true;
                        break;
                    }
                }
            }

            return linkItems[id].linkItem;
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