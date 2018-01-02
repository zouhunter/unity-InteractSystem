using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace WorldActionSystem
{
    public class RopeObj : ActionObj
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
        private List<Collider> ropeNodeFrom = new List<Collider>();
        [SerializeField]
        private List<Collider> ropeNodeTo = new List<Collider>();
        private List<Collider> connected = new List<Collider>();
        public bool Connected { get { return connected.Count == ropeNodeFrom.Count; } }

        private Transform angleTemp;
        private Coroutine antoCoroutine;
        public UltimateRope rope;
        private Vector3[] ropeNodesInstallPos;
        private Vector3[] ropeNodesStartPos;
        private List<Collider> ropeList = new List<Collider>();
        private List<float> lengthList = new List<float>();

        protected void Awake()
        {
            RegistNodes();
            RegestRopeList();
            RecordStartPosAndSetLayer();
        }
        protected override void Start()
        {
            base.Start();
            angleTemp = anglePos;
        }
        public override void OnStartExecute(bool auto = false)
        {
            base.OnStartExecute(auto);
            if(auto)
            {
                antoCoroutine = StartCoroutine(AutoInstallAllRopeNode());
            }
        }
        public override void OnUnDoExecute()
        {
            base.OnUnDoExecute();
            if (antoCoroutine != null) StopCoroutine(antoCoroutine);
            //if (AlreadyPlaced)
            //{
            //    var ropeItem = this.ropeItem;
            //    ropeItem.QuickUnInstallAllRopeNode();
            //    var obj = Detach();
            //    obj.QuickUnInstall();
            //    obj.StepUnDo();
            //}
            connected.Clear();
            anglePos = angleTemp;
        }

        public override void OnEndExecute(bool force)
        {
            base.OnEndExecute(force);

            if (antoCoroutine != null) StopCoroutine(antoCoroutine);

            QuickInstallRopeNodes(ropeNodeFrom);

            if (completeHide)
                gameObject.SetActive(false);

            connected = new List<Collider>(ropeNodeFrom);
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
            if (connected.Count == ropeNodeFrom.Count)
            {
                OnEndExecute(false);
            }
            else
            {
                for (int i = 0; i < ropeNodeFrom.Count; i++)
                {
                    var collider = ropeNodeTo.Find(x=>x.name == ropeNodeFrom[i].name);
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
            if (connected.Count == ropeNodeFrom.Count)
            {
                OnEndExecute(false);
            }
            else
            {
                for (int i = 0; i < ropeNodeFrom.Count; i++)
                {
                    var collider = ropeNodeFrom[i];
                    var mark = connected.Find(x => x.name == collider.name);
                    if (mark == null)
                    {
                        target = ropeNodeFrom[i];
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

        public void OnPickupCollider(Collider collider)
        {
            for (int i = 0; i < ropeNodeFrom.Count; i++)
            {
                var cld = ropeNodeFrom[i];
                if (ropeNodeFrom[i].name == collider.name)
                {
                    if (connected.Contains(cld))
                    {
                        connected.Remove(cld);
                    }
                    angleCtrl.UnNotice(anglePos);
                    anglePos = ropeNodeFrom[i].transform;
                    break;
                }
            }
        }

        public bool CanInstallCollider(Collider collider)
        {
            bool havePos = false;
            for (int i = 0; i < ropeNodeFrom.Count; i++)
            {
                if (ropeNodeFrom[i].name == collider.name && !connected.Contains(ropeNodeFrom[i]))
                {
                    havePos = true;
                }
            }
            return havePos;
        }

 
        private void RegistNodes()
        {
            foreach (var item in ropeNodeFrom)
            {
                item.gameObject.layer = Layers.ropeNodeLayer;
            }
        }

        internal void QuickInstallRopeItem(Collider collider)
        {
            for (int i = 0; i < ropeNodeFrom.Count; i++)
            {
                if (ropeNodeFrom[i].name == collider.name && !connected.Contains(ropeNodeFrom[i]))
                {
                    connected.Add(ropeNodeFrom[i]);
                    collider.transform.position = ropeNodeFrom[i].transform.position;
                    break;
                }
            }
            SelectOneRopeNode();
        }

    
        private void OnEnable()
        {
            rope.Regenerate(true);
        }
        private void RecordStartPosAndSetLayer()
        {
            ropeNodesStartPos = new Vector3[ropeNodeTo.Count];
            for (int i = 0; i < ropeNodesStartPos.Length; i++)
            {
                ropeNodesStartPos[i] = ropeNodeTo[i].transform.position;
                ropeNodeTo[i].gameObject.layer = Layers.ropeNodeLayer;
            }
        }

        private void RegestRopeList()
        {
            ropeList.Add(rope.RopeStart.GetComponent<BoxCollider>());
            for (int i = 0; i < rope.RopeNodes.Count; i++)
            {
                ropeList.Add(rope.RopeNodes[i].goNode.GetComponent<BoxCollider>());
                lengthList.Add(rope.RopeNodes[i].fLength);
            }

        }

        public void RegistNodesInstallPos()
        {
            Debug.Log("RegistNodesInstallPos");
            ropeNodesInstallPos = new Vector3[ropeNodeTo.Count];
            for (int i = 0; i < ropeNodesInstallPos.Length; i++)
            {
                ropeNodesInstallPos[i] = ropeNodeTo[i].transform.position;
            }
        }
        /// <summary>
        /// 防止线被拉太长
        /// </summary>
        /// <param name="collider"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public bool TryMoveToPos(Collider collider, Vector3 pos)
        {
            if (rope.RopeNodes.Count == 0) return false;
            var id = ropeList.IndexOf(collider);
            if (id != -1)
            {
                bool canMove = true;
                var lastid = id - 1;
                var nextid = id + 1;
                if (lastid >= 0)
                {
                    var lastNode = ropeList[lastid];
                    canMove &= Vector3.Distance(pos, lastNode.transform.position) < 2 * lengthList[lastid];
                }
                if (nextid < ropeList.Count)
                {
                    var nextNode = ropeList[nextid];
                    canMove &= Vector3.Distance(pos, nextNode.transform.position) < 2 * lengthList[id];
                }
                if (canMove)
                {
                    collider.transform.position = pos;
                }
                return canMove;
            }
            return false;
        }
        /// 放回
        /// </summary>
        public void PickDownCollider(Collider collider)
        {
            Debug.Assert(collider != null);
            var id = ropeNodeTo.IndexOf(collider);
            collider.transform.position = ropeNodesInstallPos[id];
        }

        public void PickDownAllCollider()
        {
            for (int i = 0; i < ropeNodeTo.Count; i++)
            {
                ropeNodeTo[i].transform.position = ropeNodesInstallPos[i];
            }
        }

        public void QuickUnInstallAllRopeNode()
        {
            for (int i = 0; i < ropeNodeTo.Count; i++)
            {
                ropeNodeTo[i].transform.position = ropeNodesStartPos[i];
            }
        }

        internal void QuickInstallRopeNodes(List<Collider> ropeNode)
        {
            foreach (var target in ropeNode)
            {
                var obj = this.ropeNodeTo.Find(x => x.name == target.name);
                if (obj != null)
                {
                    obj.transform.position = target.transform.position;
                }
            }
        }
    }
}