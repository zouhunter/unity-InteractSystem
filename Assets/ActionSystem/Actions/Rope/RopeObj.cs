using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace WorldActionSystem
{
    public class RopeObj : PlaceItem
    {
        [SerializeField]
        private List<Collider> ropeNode = new List<Collider>();
        private float autoTime { get { return Setting.autoExecuteTime; } }
        private List<Collider> connected = new List<Collider>();

        [SerializeField]
        private RopeItem ropeItem;
        public bool Connected { get { return connected.Count == ropeNode.Count; } }
        public override PickUpAbleElement obj
        {
            get
            {
                return ropeItem;
            }
            protected set
            {
                ropeItem = value as RopeItem;
            }
        }

        public override int layer
        {
            get
            {
                return Setting.placePosLayer;
            }
        }
        private Transform angleTemp;
        private Coroutine antoCoroutine;
        private bool innerRopeItem;

        protected override void Awake()
        {
            base.Awake();
            TryAutoRegistRopeItem();
            RegistNodes();
            if (!innerRopeItem)
            {
                ElementController.onInstall += OnInstallComplete;
                ElementController.onUnInstall += OnUnInstallComplete;
            }

        }
        protected override void Start()
        {
            base.Start();
            angleTemp = anglePos;
        }

        private void OnDestroy()
        {
            if (!innerRopeItem)
            {
                ElementController.onInstall -= OnInstallComplete;
                ElementController.onUnInstall -= OnUnInstallComplete;
            }
        }

        private void OnInstallComplete(PickUpAbleElement obj)
        {
            if (obj == this.obj)
            {
                ropeItem.RegistNodesInstallPos();
                //提示一个接头点
                if (auto)
                {
                    antoCoroutine = StartCoroutine(AutoInstallAllRopeNode());
                }
                else
                {
                    SelectOneRopeNode();
                }
            }
        }
        private IEnumerator AutoInstallAllRopeNode()
        {
            Collider current;
            Collider currentTarget;
            while ((current = SelectOneRopeNode(out currentTarget)) != null)
            {
                var startPos = current.transform.position;

                for (float i = 0; i < 1f; i += Time.deltaTime)
                {
                    current.transform.position = Vector3.Lerp(startPos, currentTarget.transform.position, i);
                    yield return null;
                }
                connected.Add(currentTarget);
            }
        }
        private void SelectOneRopeNode()
        {
            Debug.Assert(ropeItem);
            if (connected.Count == ropeNode.Count)
            {
                OnEndExecute(false);
            }
            else
            {
                for (int i = 0; i < ropeNode.Count; i++)
                {
                    var collider = ropeItem.ropeNode[i];
                    var mark = connected.Find(x => x.name == collider.name);
                    if (mark == null)
                    {
                        angleCtrl.UnNotice(anglePos);
                        anglePos = collider.transform;
                        Debug.Log("Notice:" + anglePos);
                    }
                }
            }
        }
        private Collider SelectOneRopeNode(out Collider target)
        {
            Debug.Assert(ropeItem);
            if (connected.Count == ropeNode.Count)
            {
                OnEndExecute(false);
            }
            else
            {
                for (int i = 0; i < ropeNode.Count; i++)
                {
                    var collider = ropeItem.ropeNode[i];
                    var mark = connected.Find(x => x.name == collider.name);
                    if (mark == null)
                    {
                        target = ropeNode[i];
                        angleCtrl.UnNotice(anglePos);
                        anglePos = target.transform;
                        //Debug.Log("Notice:" + anglePos);
                        return collider;
                    }
                }
            }
            target = null;
            return null;
        }

        /// <summary>
        /// 点击到一个接头点
        /// </summary>
        /// <param name="collider"></param>
        public void OnPickupCollider(Collider collider)
        {
            if (ropeItem == null) return;
            for (int i = 0; i < ropeNode.Count; i++)
            {
                var cld = ropeNode[i];
                if (ropeNode[i].name == collider.name)
                {
                    if (connected.Contains(cld))
                    {
                        connected.Remove(cld);
                    }
                    angleCtrl.UnNotice(anglePos);
                    anglePos = ropeNode[i].transform;
                    break;
                }
            }
        }

        public bool CanInstallCollider(Collider collider)
        {
            bool havePos = false;
            for (int i = 0; i < ropeNode.Count; i++)
            {
                if (ropeNode[i].name == collider.name && !connected.Contains(ropeNode[i]))
                {
                    havePos = true;
                }
            }
            return havePos;
        }

        private void OnUnInstallComplete(PickUpAbleElement obj)
        {
            if (AlreadyPlaced && this.obj == obj)
            {
                if (ropeItem != null)
                {
                    ropeItem.QuickUnInstallAllRopeNode();
                }
                Detach();
                connected.Clear();
            }

        }
        public override void OnStartExecute(bool auto = false)
        {
            base.OnStartExecute(auto);
            if (innerRopeItem)
            {
                OnInstallComplete(obj);
            }
            else 
            {
                if(auto)
                {
                    PickUpAbleElement obj = ElementController.GetUnInstalledObj(name);
                    Attach(obj);
                    obj.NormalInstall(gameObject);
                }
            }
        }
        public override void OnUnDoExecute()
        {
            base.OnUnDoExecute();
            if (antoCoroutine != null) StopCoroutine(antoCoroutine);
            if (AlreadyPlaced)
            {
                var ropeItem = this.ropeItem;
                if (!innerRopeItem)
                {
                    var obj = Detach();
                    obj.QuickUnInstall();
                    ropeItem.QuickUnInstallAllRopeNode();
                }
                else
                {
                    ropeItem.PickDownAllCollider();
                }
                anglePos = angleTemp;
            }
            connected.Clear();
        }
        public override void OnEndExecute(bool force)
        {
            base.OnEndExecute(force);
            if (antoCoroutine != null) StopCoroutine(antoCoroutine);
            if (!AlreadyPlaced)
            {
                if (!innerRopeItem)
                {
                    PickUpAbleElement obj = ElementController.GetUnInstalledObj(name);
                    Attach(obj);
                    obj.QuickInstall(gameObject);
                }
            }
            ropeItem.QuickInstallRopeNodes(ropeNode);
            connected = new List<Collider>(ropeNode);
        }
        /// <summary>
        /// 利用控制器放置元素
        /// </summary>
        /// <param name="ropeItem"></param>
        /// <returns></returns>
        public bool TryRegistRopeItem(RopeItem ropeItem)
        {
            if (this.ropeItem == null && ropeItem != null)
            {
                this.ropeItem = ropeItem;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 支持提前放置好元素
        /// </summary>
        private void TryAutoRegistRopeItem()
        {
            if (ropeItem)
            {
                innerRopeItem = true;
                Attach(obj);
                obj.QuickInstall(gameObject);
            }
        }

        private void RegistNodes()
        {
            foreach (var item in ropeNode)
            {
                item.gameObject.layer = Setting.ropeNodeLayer;
            }
        }

        internal void QuickInstallRopeItem(Collider collider)
        {
            for (int i = 0; i < ropeNode.Count; i++)
            {
                if (ropeNode[i].name == collider.name && !connected.Contains(ropeNode[i]))
                {
                    connected.Add(ropeNode[i]);
                    collider.transform.position = ropeNode[i].transform.position;
                    break;
                }
            }
            SelectOneRopeNode();
        }
    }
}