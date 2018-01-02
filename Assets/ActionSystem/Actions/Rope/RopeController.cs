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
    public class RopeCtrl : PlaceController
    {
        public override ControllerType CtrlType { get { return ControllerType.Rope; } }

        private RopeObj ropeObj { get { return installPos as RopeObj; } }

        protected override int PlacePoslayerMask
        {
            get
            {
                return 1 << Layers.placePosLayer;
            }
        }
        private RopeItem ropeItem;
        private Collider pickUpedRopeNode;
        private bool pickDownAble;

        public override void Update()
        {
            base.Update();
            if (ropeItem == null || pickUpedRopeNode == null)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    TrySelectNode();
                }
            }
            else
            {
                RopeNodeMoveWithMouse(elementDistence += Input.GetAxis("Mouse ScrollWheel"));
                UpdateInstallRopeNode();
            }
        }
        private void TrySelectNode()
        {
            ray = viewCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, hitDistence, (1 << Layers.ropeNodeLayer)))
            {
                var ropeItem = hit.collider.GetComponentInParent<RopeItem>();
                if (ropeItem != null && ropeItem.HaveBinding)//正在进行操作
                {
                    if (ropeObj == null && ropeItem)
                    {
                        installPos = ropeItem.BindingObj.GetComponent<RopeObj>();//.Find(x => x.obj == ropeItem);
                        pickedUpObj = ropeItem;
                    }
                    if (ropeObj != null)
                    {
                        ropeObj.OnPickupCollider(hit.collider);
                        this.ropeItem = ropeItem;
                        pickUpedRopeNode = hit.collider;
                        Debug.Log("Select: " + pickUpedRopeNode);

                        elementDistence = Vector3.Distance(viewCamera.transform.position, pickUpedRopeNode.transform.position);
                    }
                }
            }
        }

        private void UpdateInstallRopeNode()
        {
            if (Input.GetMouseButtonDown(0))
            {
                TryPlaceNode();
            }
            else
            {
                ray = viewCamera.ScreenPointToRay(Input.mousePosition);
                hits = Physics.RaycastAll(ray, hitDistence, (1 << Layers.ropeNodeLayer));
                if (hits != null || hits.Length > 0)
                {
                    bool hited = false;
                    for (int i = 0; i < hits.Length; i++)
                    {
                        if (pickUpedRopeNode == null) return;

                        if (hits[i].collider.name == pickUpedRopeNode.name && hits[i].collider != pickUpedRopeNode)
                        {
                            hited = true;
                            var ropeObj = hits[i].collider.GetComponentInParent<RopeObj>();
                            pickDownAble = CanPlaceNode(ropeObj, ropeItem, pickUpedRopeNode, out resonwhy);
                        }
                    }
                    if (!hited)
                    {
                        pickDownAble = false;
                        resonwhy = "零件放置位置不正确";
                    }
                }
            }

        }
        private void RopeNodeMoveWithMouse(float distence)
        {
            disRay = viewCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(disRay, out disHit, distence, 1 << Layers.obstacleLayer))
            {
                if (!ropeItem.TryMoveToPos(pickUpedRopeNode, disHit.point))
                {
                    ropeItem.PickDownCollider(pickUpedRopeNode);
                    pickUpedRopeNode = null;
                    ropeItem = null;

                }
            }
            else
            {
                var pos = disRay.GetPoint(elementDistence);
                if (!ropeItem.TryMoveToPos(pickUpedRopeNode, pos))
                {
                    ropeItem.PickDownCollider(pickUpedRopeNode);
                    pickUpedRopeNode = null;
                    ropeItem = null;
                }
            }
        }

        private void TryPlaceNode()
        {
            ray = viewCamera.ScreenPointToRay(Input.mousePosition);
            if (pickDownAble)
            {
                PlaceNode(pickUpedRopeNode);
            }
            else
            {
                PlaceNodeWrong(ropeItem, pickUpedRopeNode);
                OnUserError(resonwhy);
            }
            pickUpedRopeNode = null;
            ropeItem = null;
            pickDownAble = false;
        }

        private bool CanPlaceNode(RopeObj ropeObj, RopeItem ropeItem, Collider collider, out string resonwhy)
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
            else if (ropeObj.obj != ropeItem)
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
        private void PlaceNodeWrong(RopeItem ropeItem, Collider collider)
        {
            Debug.Log("PlaceNodeWrong");
            ropeItem.PickDownCollider(collider);
        }

        protected override bool CanPlace(PlaceObj placeItem, PickUpAbleElement element, out string why)
        {
            if (placeItem == null || !(placeItem is RopeObj))
            {
                why = "上标点未挂RopeObj脚本";
                Debug.LogError("【配制错误】:安装点未挂RopeObj脚本");
            }
            else if (!(element is RopeItem))
            {
                why = "对象不匹配";
                installAble = false;
                //Debug.LogError("【配制错误】:零件未挂RopeObj脚本");
            }
            else if (!placeItem.Started)
            {
                installAble = false;
                why = "放置顺序错误";
            }
            else if (placeItem.AlreadyPlaced)
            {
                installAble = false;
                why = "目标点已经放置";
            }
            else if (element.name != placeItem.Name)
            {
                installAble = false;
                why = "名称不匹配";
            }
            else
            {
                installAble = true;
                why = null;
            }
            return installAble;
        }

        protected override void PlaceObject(PlaceObj pos, PickUpAbleElement element)
        {
            pos.Attach(element);
            element.QuickInstall(installPos, false,true);
        }

        protected override void PlaceWrong(PickUpAbleElement pickup)
        {
            if (pickup.BindingObj != null)
            {
                pickup.QuickInstall(ropeObj, false);
            }
            else
            {
                pickup.NormalUnInstall();
            }
        }
    }
}