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
        private GameObject ropeBody;
        [SerializeField]
        private List<Collider> ropeNodeFrom = new List<Collider>();
        [SerializeField]
        private List<Collider> ropeNodeTo = new List<Collider>();
        private List<Collider> connected = new List<Collider>();
        public bool Connected { get { return connected.Count == 2 * ropeNodeFrom.Count; } }

        private Transform angleTemp;
        private Coroutine antoCoroutine;
        public UltimateRope rope;
        private Vector3[] ropeNodeStartPos;
        private List<Collider> ropeList = new List<Collider>();
        private List<float> lengthList = new List<float>();

        protected void Awake()
        {
            RegistNodes();
            RegestRopeList();
        }

        private void OnEnable()
        {
            rope.Regenerate(true);
        }
       
        protected override void Start()
        {
            base.Start();
            angleTemp = anglePos;
        }
        
        private void RegistNodes()
        {
            ropeNodeStartPos = new Vector3[ropeNodeFrom.Count];

            for (int i = 0; i < ropeNodeStartPos.Length; i++)
            {
                ropeNodeStartPos[i] = ropeNodeFrom[i].transform.position;
                ropeNodeTo[i].gameObject.layer = Layers.ropePosLayer;
                ropeNodeFrom[i].gameObject.layer = Layers.ropeNodeLayer;
            }
        }

        /// <summary>
        /// 可动态注册安装点(比如绳子碰撞到某处)
        /// </summary>
        /// <param name="target"></param>
        public void RegistTarget(List<Vector3> target)
        {

        }

        private void RegestRopeList()
        {
            ropeList.Add(rope.RopeStart.GetComponent<Collider>());
            for (int i = 0; i < rope.RopeNodes.Count; i++)
            {
                ropeList.Add(rope.RopeNodes[i].goNode.GetComponent<Collider>());
                lengthList.Add(rope.RopeNodes[i].fLength);
            }
        }

        public override void OnStartExecute(bool auto = false)
        {
            base.OnStartExecute(auto);
            if(auto){
                antoCoroutine = StartCoroutine(AutoConnectRopeNodes());
            }
            else
            {
                NoticeSelectNode();
            }
        }

        public override void OnUnDoExecute()
        {
            base.OnUnDoExecute();

            if (antoCoroutine != null)
                StopCoroutine(antoCoroutine);

            PickDownAllCollider();

            connected.Clear();
            anglePos = angleTemp;

            if (completeHide){
                ropeBody.gameObject.SetActive(true);
            }
        }
 
        protected override void OnBeforeEnd(bool force)
        {
            base.OnBeforeEnd(force);

            if (antoCoroutine != null)
                StopCoroutine(antoCoroutine);

            QuickInstallRopeNodes(ropeNodeFrom);

            if (completeHide) {
                ropeBody.gameObject.SetActive(false);
            }
        }
        private IEnumerator AutoConnectRopeNodes()
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
                connected.Add(current);
            }

            OnEndExecute(false);
        }

        private void NoticeSelectNode()
        {
            if (connected.Count == ropeNodeFrom.Count)
            {
                OnEndExecute(false);
            }
            else
            {
                for (int i = 0; i < ropeNodeFrom.Count; i++)
                {
                    if (connected.Contains(ropeNodeFrom[i])) continue;
                    var ropeTo = ropeNodeTo.Find(x => x.name == ropeNodeFrom[i].name && !connected.Contains(x));
                    if (ropeTo != null)
                    {
                        angleCtrl.UnNotice(anglePos);
                        anglePos = ropeNodeFrom[i].transform;
                        if (log) Debug.Log("Notice:" + anglePos);
                        break;
                    }
                }
            }
        }

        private Collider SelectOneRopeNode(out Collider target)
        {
            if (!Connected)
            {
                for (int i = 0; i < ropeNodeFrom.Count; i++)
                {
                    var collider = ropeNodeFrom[i];
                    if(!connected.Contains(collider))
                    {
                        target = ropeNodeTo.Find(x => x.name == collider.name && !connected.Contains(x));
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
            var toCollider = ropeNodeTo.Find(x => x.name == collider.name && !connected.Contains(x));
            if(toCollider != null)
            {
                if (connected.Contains(collider)){
                    connected.Remove(collider);
                }
                angleCtrl.UnNotice(anglePos);
                anglePos = toCollider.transform;
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

        internal void QuickInstallRopeItem(Collider collider)
        {
            var nodeTo = ropeNodeTo.Find(x => x.name == collider.name && !connected.Contains(x));
            if(nodeTo)
            {
                connected.Add(nodeTo);
                connected.Add(collider);
                collider.transform.position = nodeTo.transform.position;
            }
            NoticeSelectNode();
        }

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

        public void PickDownCollider(Collider collider)
        {
            Debug.Assert(collider != null);
            var id = ropeNodeFrom.IndexOf(collider);
            collider.transform.position = ropeNodeStartPos[id];
        }

        public void PickDownAllCollider()
        {
            for (int i = 0; i < ropeNodeFrom.Count; i++)
            {
                ropeNodeFrom[i].transform.position = ropeNodeStartPos[i];
            }
        }

        internal void QuickInstallRopeNodes(List<Collider> ropeNodeFrom)
        {
            for (int i = 0; i < ropeNodeFrom.Count; i++)
            {
                var from = ropeNodeFrom[i];
                var obj = this.ropeNodeTo.Find(x => x.name == from.name && !connected.Contains(x));

                if (obj != null){
                    Debug.Log("form:" + from.transform.position);
                    Debug.Log("to:" + obj.transform.position);

                    from.transform.position = obj.transform.position;
                    connected.Add(obj);
                    connected.Add(from);
                }
            }
            if(completeHide)
            {
                Invoke("TryRegenerate", 0.1f);
            }
        }

        private void TryRegenerate()
        {
            rope.Regenerate(false);
        }
    }
}