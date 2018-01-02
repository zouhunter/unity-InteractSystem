using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace WorldActionSystem
{
    /// <summary>
    /// 将ropeItem安装到指定RopeObj上
    /// 然后安装内部的点到对就的RopeObj上
    /// </summary>
    public class RopeCtrl : OperateController
    {
        public override ControllerType CtrlType { get { return ControllerType.Rope; } }
        public RopeObj ropeObj { get; private set; }
        private Collider pickUpedRopeNode;
        private bool pickDownAble;
        private float elementDistence;
        private Ray disRay;
        private RaycastHit disHit;
        private PlaceController placeCtrl;
        public Ray ray;
        public RaycastHit hit;
        public RaycastHit[] hits;
        public float hitDistence { get { return Config.hitDistence; } }

        public override void Update()
        {

            if (Input.GetMouseButtonDown(0))
            {
                if (pickUpedRopeNode == null)
                {
                    TrySelectNode();
                }
                else
                {
                    //放置
                }
            }
            else
            {
                RopeNodeMoveWithMouse(elementDistence += Input.GetAxis("Mouse ScrollWheel"));
            }
        }

        private void TrySelectNode()
        {
            ray = viewCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, hitDistence, (1 << Layers.ropeNodeLayer)))
            {
                ropeObj = hit.collider.GetComponentInParent<RopeObj>();
                if (ropeObj != null)//正在进行操作
                {
                    if (ropeObj != null)
                    {
                        ropeObj.OnPickupCollider(hit.collider);
                        pickUpedRopeNode = hit.collider;
                        Debug.Log("Select: " + pickUpedRopeNode);

                        elementDistence = Vector3.Distance(viewCamera.transform.position, pickUpedRopeNode.transform.position);
                    }
                }
            }
        }

        private void RopeNodeMoveWithMouse(float distence)
        {
            disRay = viewCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(disRay, out disHit, distence, 1 << Layers.obstacleLayer))
            {
                if (!ropeObj.TryMoveToPos(pickUpedRopeNode, disHit.point))
                {
                    ropeObj.PickDownCollider(pickUpedRopeNode);
                    pickUpedRopeNode = null;
                    ropeObj = null;
                }
            }
            else
            {
                var pos = disRay.GetPoint(elementDistence);
                if (!ropeObj.TryMoveToPos(pickUpedRopeNode, pos))
                {
                    ropeObj.PickDownCollider(pickUpedRopeNode);
                    pickUpedRopeNode = null;
                    ropeObj = null;
                }
            }
        }

        private void TryPlaceNode()
        {
            var ray = viewCamera.ScreenPointToRay(Input.mousePosition);
            if (pickDownAble)
            {
                PlaceNode(pickUpedRopeNode);
            }
            else
            {
                PlaceNodeWrong(ropeObj, pickUpedRopeNode);
                userErr(placeCtrl.resonwhy);
            }
            pickUpedRopeNode = null;
            ropeObj = null;
            pickDownAble = false;
        }

        public bool CanPlaceNode(RopeObj ropeObj, RopeObj ropeItem, Collider collider, out string resonwhy)
        {
            resonwhy = null;
            if (this.ropeObj != ropeObj)
            {
                resonwhy = "目标点非当前步骤";
            }
            else if (ropeObj == null)
            {
                resonwhy = "目标点父级没有挂RopeObj脚本";
            }
            else if (ropeObj.Connected)
            {
                resonwhy = "目标点已经完成连接";
            }
            else if (ropeObj != ropeItem)
            {
                resonwhy = "对象不匹配";
            }
            else if (!ropeObj.CanInstallCollider(collider))
            {
                resonwhy = "坐标点已经占用";
            }
            return resonwhy == null;
        }

        private void PlaceNode(Collider collider)
        {
            Debug.Log("PlaceNode");
            ropeObj.QuickInstallRopeItem(collider);
        }

        public void PlaceNodeWrong(RopeObj ropeItem, Collider collider)
        {
            Debug.Log("PlaceNodeWrong");
            ropeItem.PickDownCollider(collider);
        }
    }
}