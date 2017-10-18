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
        private float distence;
        private List<MatchObj> matchObjs;
        private bool isForceAuto;
        private ElementController elementCtrl { get; set; }
        private List<PickUpAbleElement> usedElements = new List<PickUpAbleElement>();
        public MatchCtrl(float distence, MatchObj[] matchObjs, ElementController elementCtrl)
        {
            highLight = new ShaderHighLight();
            this.distence = distence;
            this.matchObjs = new List<MatchObj>(matchObjs);
            this.elementCtrl = elementCtrl;
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
                MoveWithMouse(distence += Input.GetAxis("Mouse ScrollWheel"));
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
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100, (1 << Setting.pickUpElementLayer)))
            {
                pickedUpObj = hit.collider.GetComponent<PickUpAbleElement>();
                if (pickedUpObj != null && !pickedUpObj.Installed)
                {
                    pickedUp = true;

                    if (!PickUpedIsMatch())
                    {
                        if (highLight != null) highLight.HighLightTarget(pickedUpObj.Render, Color.yellow);
                    }
                    else
                    {
                        if (highLight != null) highLight.HighLightTarget(pickedUpObj.Render, Color.cyan);
                    }
                }
            }
        }

        /// <summary>
        ///匹配
        /// </summary>
        /// <returns></returns>
        private bool PickUpedIsMatch()
        {
            bool canInstall = false;
            List<MatchObj> poss = GetNotMatchedPosList();
            for (int i = 0; i < poss.Count; i++)
            {
                if (poss[i].obj == null && IsMatchStep(poss[i]) && pickedUpObj.name == poss[i].name)
                {
                    canInstall = true;
                }
            }
            return canInstall;
        }

        /// <summary>
        /// 更新匹配状态
        /// </summary>
        public void UpdateMatchState()
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            hits = Physics.RaycastAll(ray, 100, (1 << Setting.matchPosLayer));
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
                        else if (!IsMatchStep(matchPos))
                        {
                            matchAble = false;
                            resonwhy = "操作顺序错误";
                        }
                        else if (matchPos.Matched)
                        {
                            matchAble = false;
                            resonwhy = "已经触发结束";
                        }
                        else if (matchPos.name != pickedUpObj.name)
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
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (matchAble)
            {
                matchPos.Attach(pickedUpObj);
                pickedUpObj.QuickMoveTo(matchPos.gameObject);
                matchPos.OnEndExecute();
                pickedUpObj = null;
            }
            else
            {
                if (UserError != null) UserError(resonwhy);
            }

            pickedUp = false;
            matchAble = false;
        }
        /// <summary>
        /// 
        /// 跟随鼠标
        /// </summary>
        void MoveWithMouse(float dis)
        {
            disRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(disRay, out disHit, dis, 1 << Setting.obstacleLayer))
            {
                pickedUpObj.transform.position = disHit.point;
            }
            else
            {
                pickedUpObj.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, dis));
            }
        }
        #endregion

        #region Surch MatchObj

        private List<MatchObj> GetNotMatchedPosList()
        {
            var list = matchObjs.FindAll(x => !x.Matched);
            return list;

        }
        private List<MatchObj> GetNeedAutoMatchObjList()
        {
            var list = matchObjs.FindAll(x => x.autoMatch);
            return list;

        }
        private bool IsMatchStep(MatchObj obj)
        {
            return matchObjs.Contains(obj);
        }
        #endregion


        public void OnEndInstallElement(PickUpAbleElement element)
        {
            bool allComplete = true;
            foreach (var item in matchObjs)
            {
                if (isForceAuto)
                {
                    allComplete &= item.Matched;
                    if (!item.Complete)
                    {
                        //强制结束
                        item.OnEndExecute();
                    }
                }
                else
                {
                    allComplete &= item.Matched;
                    allComplete &= item.Complete;
                }
            }
            if (allComplete)
            {
                OnEndExecute();
            }
        }

        /// <summary>
        /// 快速安装 列表 
        /// </summary>
        /// <param name="posList"></param>
        public void QuickMatchObjListObjects(List<MatchObj> posList)
        {
            MatchObj pos;
            for (int i = 0; i < posList.Count; i++)
            {
                pos = posList[i];
                if (pos != null)
                {
                    PickUpAbleElement obj = ElementController.GetUnInstalledObj(pos.name);
                    obj.QuickMoveTo(pos.gameObject);
                    pos.Attach(obj);
                }
            }
        }

        public void OnStartExecute(bool forceAuto)
        {
            this.isForceAuto = forceAuto;
            SetStartNotify();
            List<MatchObj> posList = null;
            if (forceAuto)
            {
                posList = GetNotMatchedPosList();
            }
            else
            {
                posList = GetNeedAutoMatchObjList();
            }

            MatchObj pos;
            for (int i = 0; i < posList.Count; i++)
            {
                pos = posList[i];
                var obj = ElementController.GetUnInstalledObj(pos.name);
                pos.Attach(obj);
                obj.NormalMoveTo(pos.gameObject);
            }
            pickedUp = false;
        }

        public void OnEndExecute()
        {
            SetCompleteNotify();
            List<MatchObj> posList = GetNotMatchedPosList();
            QuickMatchObjListObjects(posList);
        }

        public void OnUnDoExecute()
        {
            SetCompleteNotify();
            foreach (var item in matchObjs)
            {
                if (item.Matched)
                {
                    var ele = item.Detach();
                    ele.NormalMoveBack();
                    item.OnUnDoExecute();
                }
            }
        }

        /// <summary>
        /// 将可安装元素全部显示出来
        /// </summary>
        private void SetStartNotify()
        {
            var keyList = new List<string>();
            foreach (var pos in matchObjs)
            {
                if (!keyList.Contains(pos.name))
                {
                    keyList.Add(pos.name);
                    List<PickUpAbleElement> listObjs = ElementController.GetElements(pos.name);
                    if (listObjs == null) throw new Exception("元素配制错误:没有:" + pos.name);
                    for (int j = 0; j < listObjs.Count; j++)
                    {
                        if (!listObjs[j].Installed)
                        {
                            listObjs[j].StepActive();
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 结束指定步骤
        /// </summary>
        /// <param name="poss"></param>
        private void SetCompleteNotify()
        {
            var keyList = new List<string>();
            foreach (var pos in matchObjs)
            {
                if (!keyList.Contains(pos.name))
                {
                    List<PickUpAbleElement> listObjs = ElementController.GetElements(pos.name);
                    if (listObjs == null) throw new Exception("元素配制错误:没有:" + pos.name);
                    for (int j = 0; j < listObjs.Count; j++)
                    {
                        if (listObjs[j].Installed)
                        {
                            listObjs[j].StepComplete();
                        }
                        else
                        {
                            listObjs[j].StepUnDo();
                        }
                    }
                }
            }
        }
    }

}