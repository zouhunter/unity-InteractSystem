using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{

    public class MatchCtrl : IActionCtroller
    {
        public UnityAction<string> UserError;
        private IHighLightItems highLight;
        private PickUpAbleElement pickedUpObj;
        private bool pickedUp;
        private MatchObj matchPos;
        private Ray ray;
        private RaycastHit hit;
        private RaycastHit[] hits;
        private Ray disRay;
        private RaycastHit disHit;
        private bool matchAble;
        private string resonwhy;
        private float hitDistence { get { return Setting.hitDistence; } }
        private float pickDistence;
        private Camera viewCamera { get { return CameraController.ActiveCamera; } }

        public MatchCtrl()
        {
            highLight = new ShaderHighLight();
        }

        #region 鼠标操作事件
        public void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                OnLeftMouseClicked();
            }
            else if (pickedUp)
            {
                UpdateMatchState();
                MoveWithMouse(pickDistence += Input.GetAxis("Mouse ScrollWheel"));
            }

        }

        /// <summary>
        /// 选择或匹配
        /// </summary>
        private void OnLeftMouseClicked()
        {
            if (!pickedUp)
            {
                SelectAnElement();
            }
            else
            {
                TryMatchObject();
            }
        }

        /// <summary>
        /// 在未屏幕锁的情况下选中一个没有元素
        /// </summary>
        private void SelectAnElement()
        {
            ray = viewCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, hitDistence, (1 << Setting.pickUpElementLayer)))
            {
                pickedUpObj = hit.collider.GetComponent<PickUpAbleElement>();
                if (pickedUpObj != null && !pickedUpObj.Installed)
                {
                    pickedUpObj.OnPickUp();
                    pickedUp = true;
                    pickDistence = Vector3.Distance(viewCamera.transform.position, pickedUpObj.transform.position);
                }
            }
        }

        /// <summary>
        ///匹配
        /// </summary>
        /// <returns></returns>
        //private bool PickUpedIsMatch()
        //{
            //bool canInstall = false;
            //List<MatchObj> poss = GetNotMatchedPosList();
            //for (int i = 0; i < poss.Count; i++)
            //{
            //    if (poss[i].obj == null && IsMatchStep(poss[i]) && pickedUpObj.name == poss[i].Name)
            //    {
            //        canInstall = true;
            //    }
            //}
            //return canInstall;
        //}

        /// <summary>
        /// 更新匹配状态
        /// </summary>
        public void UpdateMatchState()
        {
            ray = viewCamera.ScreenPointToRay(Input.mousePosition);
            hits = Physics.RaycastAll(ray, hitDistence, (1 << Setting.matchPosLayer));
            if (hits != null || hits.Length > 0)
            {
                bool hited = false;
                for (int i = 0; i < hits.Length; i++)
                {
                    if (hits[i].collider.name == pickedUpObj.name)
                    {
                        hited = true;
                        matchPos = hits[i].collider.GetComponent<MatchObj>();
                        if (matchPos == null)
                        {
                            Debug.LogError("【配制错误】:零件未挂MatchObj脚本");
                        }
                        else if (!matchPos.Started)
                        {
                            matchAble = false;
                            resonwhy = "操作顺序错误";
                        }
                        else if (matchPos.Matched)
                        {
                            matchAble = false;
                            resonwhy = "已经触发结束";
                        }
                        else if (matchPos.Name != pickedUpObj.name)
                        {
                            matchAble = false;
                            resonwhy = "零件不匹配";
                        }
                        else
                        {
                            matchAble = true;
                        }
                    }
                }
                if (!hited)
                {
                    matchAble = false;
                    resonwhy = "零件放置位置不正确";
                }
            }

            if (matchAble)
            {
                //可安装显示绿色
                if (highLight != null) highLight.HighLightTarget(pickedUpObj.Render, Color.green);
            }
            else
            {
                //不可安装红色
                if (highLight != null) highLight.HighLightTarget(pickedUpObj.Render, Color.red);
            }
        }

        /// <summary>
        /// 尝试安装元素
        /// </summary>
        private void TryMatchObject()
        {
            if (highLight != null) highLight.UnHighLightTarget(pickedUpObj.Render);
            ray = viewCamera.ScreenPointToRay(Input.mousePosition);
            if (matchAble)
            {
                matchPos.Attach(pickedUpObj);
                pickedUpObj.QuickMoveTo(matchPos.gameObject);
            }
            else
            {
                if (UserError != null) UserError(resonwhy);
                pickedUpObj.NormalMoveBack();
            }
            pickedUpObj = null;
            pickedUp = false;
            matchAble = false;
        }
        /// <summary>
        /// 
        /// 跟随鼠标
        /// </summary>
        void MoveWithMouse(float dis)
        {
            disRay = viewCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(disRay, out disHit, dis, 1 << Setting.obstacleLayer))
            {
                pickedUpObj.transform.position = disHit.point;
            }
            else
            {
                pickedUpObj.transform.position = viewCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, dis));
            }
        }

        #endregion

        /// <summary>
        /// 将可安装元素全部显示出来
        /// </summary>
        //private void SetStartNotify()
        //{
        //    var keyList = new List<string>();
        //    foreach (var pos in matchObjs)
        //    {
        //        if (!keyList.Contains(pos.Name))
        //        {
        //            keyList.Add(pos.Name);
        //            List<PickUpAbleElement> listObjs = ElementController.GetElements(pos.Name);
        //            if (listObjs == null) throw new Exception("元素配制错误:没有:" + pos.Name);
        //            for (int j = 0; j < listObjs.Count; j++)
        //            {
        //                if (!listObjs[j].Installed)
        //                {
        //                    listObjs[j].StepActive();
        //                }
        //            }
        //        }
        //    }
        //}
        /// <summary>
        /// 结束指定步骤
        /// </summary>
        /// <param name="poss"></param>
        //private void SetCompleteNotify(bool undo)
        //{
        //    var keyList = new List<string>();
        //    foreach (var pos in matchObjs)
        //    {
        //        if (!keyList.Contains(pos.Name))
        //        {
        //            keyList.Add(pos.Name);
        //            List<PickUpAbleElement> listObjs = ElementController.GetElements(pos.Name);
        //            if (listObjs == null) throw new Exception("元素配制错误:没有:" + pos.Name);
        //            for (int j = 0; j < listObjs.Count; j++)
        //            {
        //                if (listObjs[j].Installed) continue;

        //                listObjs[j].QuickMoveBack();

        //                if(undo)
        //                {
        //                    listObjs[j].StepUnDo();
        //                }
        //                else
        //                {
        //                    listObjs[j].StepComplete();
        //                }
        //            }
        //        }
        //    }
        //}
    }

}