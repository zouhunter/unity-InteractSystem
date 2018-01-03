using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace WorldActionSystem
{
    public class LinkObj : ActionObj, IPickUpAbleItem
    {
        [SerializeField]
        private List<LinkItem> linkItems;
        public LinkItem[] LinkItems { get { return linkItems.ToArray(); } }

        [SerializeField]
        private List<LinkGroup> defultLink;

        private Vector3[] startPositions;
        private Quaternion[] startRotation;
        private Coroutine coroutine;
        public override ControllerType CtrlType
        {
            get
            {
                return ControllerType.Link;
            }
        }

        [SerializeField]
        private Collider _handle;
        public Collider Collider { get { return _handle; } }
        public bool PickUpAble
        {
            get
            {
                return Started && !Complete;
            }

            set
            {

            }
        }

        protected override void Start()
        {
            base.Start();
            InitLinkItems();
            InitLayer();
        }

        void InitLinkItems()
        {
            startPositions = new Vector3[linkItems.Count];
            startRotation = new Quaternion[linkItems.Count];
            for (int i = 0; i < startPositions.Length; i++)
            {
                startPositions[i] = linkItems[i].transform.localPosition;
                startRotation[i] = linkItems[i].transform.localRotation;
            }
        }
        void InitLayer()
        {
            Collider.gameObject.layer = Layers.pickUpElementLayer;
        }
        public override void OnStartExecute(bool auto = false)
        {
            base.OnStartExecute(auto);
            if (auto)
            {
                if (coroutine == null)
                {
                    StartCoroutine(AutoLinkItems());
                }
            }
            else
            {
                foreach (var item in linkItems)
                {
                    item.PickUpAble = true;
                }
            }
        }
        protected override void OnBeforeEnd(bool force)
        {
            base.OnBeforeEnd(force);
            foreach (var item in linkItems)
            {
                item.PickUpAble = false;
            }
        }
        private IEnumerator AutoLinkItems()
        {
            List<LinkPort> stoped = new List<LinkPort>();

            for (int i = 0; i < defultLink.Count; i++)
            {
                var linkGroup = defultLink[i];
                var portA = linkGroup.portA;
                var portB = linkGroup.portB;

                if (!stoped.Contains(portB))//将B移向A
                {
                    yield return MoveBToA(portA, portB);
                    stoped.Add(portB);
                    if (!stoped.Contains(portA))
                    {
                        stoped.Add(portA);
                    }
                }
                else if (!stoped.Contains(portA))//将A移向B
                {
                    stoped.Add(portA);
                    yield return MoveBToA(portB, portA);
                }
            }
        }

        IEnumerator MoveBToA(LinkPort portA, LinkPort portB)
        {
            var linkInfoA = portA.connectAble.Find(x => x.itemName == portB.Body.Name);
            var linkInfoB = portB.connectAble.Find(x => x.itemName == portA.Body.name);

            portB.Attach(portA);

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

        public void OnPickUp()
        {
        }

        public void OnPickStay()
        {
        }

        public void OnPickDown()
        {
        }

        public void SetPosition(Vector3 pos)
        {
            transform.position = pos;
        }
    }
}