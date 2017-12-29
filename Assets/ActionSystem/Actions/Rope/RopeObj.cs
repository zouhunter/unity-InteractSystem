using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace WorldActionSystem
{
    public class RopeObj : PlaceObj
    {
        public bool completeHide;
        public override ControllerType CtrlType
        {
            get
            {
                return ControllerType.Rope;
            }
        }

        [SerializeField]
        private List<Collider> ropeNode = new List<Collider>();
        private List<Collider> connected = new List<Collider>();
        private RopeItem ropeItem { get { return obj as RopeItem;  } set { obj = value; } }
        public bool Connected { get { return connected.Count == ropeNode.Count; } }
        public override int layer
        {
            get
            {
                return Layers.placePosLayer;
            }
        }
        private Transform angleTemp;
        private Coroutine antoCoroutine;
        public bool quickInstall;

        protected override void Awake()
        {
            base.Awake();
            RegistNodes();
            hideOnInstall = false;//强制不可以隐藏
        }
        protected override void Start()
        {
            base.Start();
            angleTemp = anglePos;
        }

        protected override void OnInstallComplete()
        {
            if (log) Debug.Log("OnInstallComplete");
            if (obj == this.obj)
            {
                ropeItem.RegistNodesInstallPos();

                //提示一个接头点
                if (auto)
                {
                    if(gameObject.activeInHierarchy)
                    {
                        antoCoroutine = StartCoroutine(AutoInstallAllRopeNode());
                    }
                    else
                    {
                        Collider current;
                        Collider currentTarget;
                        while ((current = SelectOneRopeNode(out currentTarget)) != null)
                        {
                            current.transform.position = currentTarget.transform.position;
                            connected.Add(currentTarget);
                        }
                    }
                }
                else
                {
                    SelectOneRopeNode();
                }
            }
        }

        protected override void OnUnInstallComplete()
        {
            if (AlreadyPlaced && this.obj == obj)
            {
                if (ropeItem != null){
                    ropeItem.QuickUnInstallAllRopeNode();
                }
                Detach();
                connected.Clear();
            }

        }

        private IEnumerator AutoInstallAllRopeNode()
        {
            Collider current;
            Collider currentTarget;
            while ((current = SelectOneRopeNode(out currentTarget)) != null)
            {
                var startPos = current.transform.position;

                if (quickInstall)
                {
                    current.transform.position = currentTarget.transform.position;
                }
                else
                {
                    for (float i = 0; i < 1f; i += Time.deltaTime)
                    {
                        current.transform.position = Vector3.Lerp(startPos, currentTarget.transform.position, i);
                        yield return null;
                    }
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
                    var collider = ropeItem.ropeNode.Find(x=>x.name == ropeNode[i].name);
                    var mark = connected.Find(x => x.name == collider.name);
                    if (mark == null)
                    {
                        angleCtrl.UnNotice(anglePos);
                        anglePos = collider.transform;
                        if (log) Debug.Log("Notice:" + anglePos);
                        break;
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

        public override void OnUnDoExecute()
        {
            base.OnUnDoExecute();
            if (antoCoroutine != null) StopCoroutine(antoCoroutine);
            if (AlreadyPlaced)
            {
                var ropeItem = this.ropeItem;
                ropeItem.QuickUnInstallAllRopeNode();
                var obj = Detach();
                obj.QuickUnInstall();
                obj.StepUnDo();
            }
            connected.Clear();
            anglePos = angleTemp;
        }

        public override void OnEndExecute(bool force)
        {
            base.OnEndExecute(force);

            if (antoCoroutine != null) StopCoroutine(antoCoroutine);

            if (!AlreadyPlaced)
            {
                PickUpAbleElement obj = elementCtrl.GetUnInstalledObj(Name);
                Attach(obj);
                obj.QuickInstall(this,true,true);
            }

            ropeItem.QuickInstallRopeNodes(ropeNode);

            if (completeHide)
                ropeItem.gameObject.SetActive(false);

            connected = new List<Collider>(ropeNode);
        }

        private void RegistNodes()
        {
            foreach (var item in ropeNode)
            {
                item.gameObject.layer = Layers.ropeNodeLayer;
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

        protected override void OnAutoInstall()
        {
            PickUpAbleElement obj = elementCtrl.GetUnInstalledObj(Name);
            Attach(obj);
            obj.NormalInstall(this);
        }
    }
}