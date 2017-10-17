using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{

    public class InstallCtrl:IActionCtroller
    {
        public bool Active { get; private set; }
        public UnityAction<string> UserError { get; set; }
        IHighLightItems highLight;
        private PickUpAbleElement pickedUpObj;
        private bool pickedUp;
        private InstallObj installPos;
        private Ray ray;
        private RaycastHit hit;
        private RaycastHit[] hits;
        private bool installAble;
        private string resonwhy;
        private float distence;
        private List<InstallObj> installObjs = new List<InstallObj>();
        private ElementController elementCtrl { get; set; }
        private bool isForceAuto;
        public InstallCtrl(float distence, InstallObj[] installObjs,ElementController elementCtrl)
        {
            highLight = new ShaderHighLight();
            this.distence = distence;
            this.installObjs.AddRange(installObjs);
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
                UpdateInstallState();
                MoveWithMouse(distence += Input.GetAxis("Mouse ScrollWheel"));
            }
        }

        private void OnLeftMouseClicked()
        {
            if (!pickedUp)
            {
                SelectAnElement();
            }
            else
            {
                TryInstallObject();
            }
        }

        /// <summary>
        /// 在未屏幕锁的情况下选中一个没有元素
        /// </summary>
        void SelectAnElement()
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100, (1 << Setting.pickUpElementLayer)))
            {
                pickedUpObj = hit.collider.GetComponent<PickUpAbleElement>();
                if (pickedUpObj != null && !pickedUpObj.Installed)
                {
                    pickedUpObj.OnPickUp();
                    pickedUp = true;

                    if (!PickUpedCanInstall())
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

        private bool PickUpedCanInstall()
        {
            bool canInstall = false;
            List<InstallObj> poss = GetNotInstalledPosList();
            for (int i = 0; i < poss.Count; i++)
            {
                if (!HaveInstallObjInstalled(poss[i]) && IsInstallStep(poss[i]) && pickedUpObj.name == poss[i].name)
                {
                    canInstall = true;
                }
            }
            return canInstall;
        }


        public void UpdateInstallState()
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            hits = Physics.RaycastAll(ray, 100, (1 << Setting.installPosLayer));
            if (hits != null || hits.Length > 0)
            {
                bool hited = false;
                for (int i = 0; i < hits.Length; i++)
                {
                    if (hits[i].collider.name == pickedUpObj.name)
                    {
                        hited = true;
                        installPos = hits[i].collider.GetComponent<InstallObj>();
                        if (installPos == null)
                        {
                            Debug.LogError("【配制错误】:零件未挂InstallObj脚本");
                        }
                        else if (!IsInstallStep(installPos))
                        {
                            installAble = false;
                            resonwhy = "操作顺序错误";
                        }
                        else if (HaveInstallObjInstalled(installPos))
                        {
                            installAble = false;
                            resonwhy = "已经安装";
                        }
                        else if (pickedUpObj.name != installPos.name)
                        {
                            installAble = false;
                            resonwhy = "零件不匹配";
                        }
                        else
                        {
                            installAble = true;
                        }
                    }
                }
                if (!hited)
                {
                    installAble = false;
                    resonwhy = "零件放置位置不正确";
                }
            }

            if (installAble)
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
        void TryInstallObject()
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (installAble)
            {
                var status = installPos.Attach(pickedUpObj);
                if (status){
                    pickedUpObj.QuickInstall(installPos.gameObject);
                }
                installPos.TryEndExecute();
            }
            else
            {
                pickedUpObj.NormalMoveBack();
                UserError(resonwhy);
            }

            pickedUp = false;
            installAble = false;
            if (highLight != null) highLight.UnHighLightTarget(pickedUpObj.Render);
        }

        private Ray disRay;
        private RaycastHit disHit;

        /// <summary>
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

        public void OnOneElementEndInstall()
        {
            bool allComplete = true;
            foreach (var item in installObjs)
            {
                if(isForceAuto)
                {
                    allComplete &= item.Installed;
                    if (!item.Complete)
                    {
                        //强制结束
                        item.OnEndExecute();
                    }
                }
                else
                {
                    allComplete &= item.Installed;
                    allComplete &= item.Complete;
                }
              
            }
        }

        /// <summary>
        /// 结束指定步骤
        /// </summary>
        /// <param name="poss"></param>
        private void SetCompleteNotify()
        {
            List<InstallObj> posList = GetInstalledPosList();
            List<PickUpAbleElement> temp = new List<PickUpAbleElement>();
            foreach (var pos in posList)
            {
                List<PickUpAbleElement> listObjs = ElementController.GetElements(pos.name);
                if (listObjs != null)
                {
                    for (int j = 0; j < listObjs.Count; j++)
                    {
                        if (!listObjs[j].Installed && !temp.Contains(listObjs[j]))
                        {
                            listObjs[j].StepUpDo();
                        }
                        else if(listObjs[j].Installed)
                        {
                            listObjs[j].StepComplete();
                        }
                    }
                }
            }
        }
        private void SetUnDoNotify()
        {
            List<InstallObj> poss = GetInstalledPosList();
            //当前步骤结束
            foreach (var item in poss)
            {
                var pickUp = item.obj as PickUpAbleElement;
                pickUp.StepUpDo();
            }
        }

        /// <summary>
        /// 快速安装 列表 
        /// </summary>
        /// <param name="posList"></param>
        private void QuickInstallObjListObjects(List<InstallObj> posList)
        {
            InstallObj pos;
            for (int i = 0; i < posList.Count; i++)
            {
                pos = posList[i];
                if (pos != null && !pos.Installed)
                {
                    PickUpAbleElement obj = ElementController.GetUnInstalledObj(pos.name);
                    obj.QuickInstall(pos.gameObject);
                    pos.Attach(obj);
                }
            }
        }

        public void SetElemetsStart()
        {
            List<InstallObj> posList = GetNotInstalledPosList();
            List<PickUpAbleElement> temp = new List<PickUpAbleElement>();
            foreach (var pos in posList)
            {
                List<PickUpAbleElement> listObjs = ElementController.GetElements(pos.name);
                if (listObjs != null)
                {
                    for (int j = 0; j < listObjs.Count; j++)
                    {
                        if (!listObjs[j].Installed && !temp.Contains(listObjs[j]))
                        {
                            temp.Add(listObjs[j]);
                            listObjs[j].StepActive();
                            break;
                        }
                    }
                }
            }
        }
      
        /// <summary>
        /// 自动安装部分需要进行自动安装的零件
        /// </summary>
        /// <param name="stepName"></param>
        public void AutoInstallWhenNeed(bool forceAuto)
        {
            List<InstallObj> posList = null;
            if (forceAuto)
            {
                posList = GetNotInstalledPosList();
            }
            else
            {
                posList = GetNeedAutoInstallObjList();
            }

            if (posList != null)
            {
                InstallObjListObjects(posList);
            }

            pickedUp = false;
        }

        private void InstallObjListObjects(List<InstallObj> posList)
        {
            InstallObj pos;
            for (int i = 0; i < posList.Count; i++)
            {
                pos = posList[i];
                PickUpAbleElement obj = ElementController.GetUnInstalledObj(pos.name);
                pos.Attach(obj);
                obj.NormalInstall(pos.gameObject);
            }
        }
        /// <summary>
        /// uninstll
        /// </summary>
        /// <param name="posList"></param>
        public void UnInstallObjListObjects(List<InstallObj> posList)
        {
            InstallObj pos;
            for (int i = 0; i < posList.Count; i++)
            {
                pos = posList[i];
                var obj = pos.Detach();
                obj.NormalUnInstall();
            }
        }

        public void QuickUnInstall()
        {
            List<InstallObj> posList = GetInstalledPosList();
            QuickUnInstallObjListObjects(posList);
            foreach (var item in installObjs)
            {
                item.OnUnDoExecute();
            }
        }

        /// <summary>
        /// QuickUnInstall
        /// </summary>
        /// <param name="posList"></param>
        public void QuickUnInstallObjListObjects(List<InstallObj> posList)
        {
            foreach (var item in posList)
            {
                if (item.Installed)
                {
                    var obj = item.Detach();
                    obj.QuickUnInstall();
                }
            }
        }

        private List<InstallObj> GetInstalledPosList()
        {
            var list = installObjs.FindAll(x => x.Installed);
            return list;
        }
        private List<InstallObj> GetNotInstalledPosList()
        {
            var list = installObjs.FindAll(x => !x.Installed);
            return list;
        }
        private List<InstallObj> GetNeedAutoInstallObjList()
        {
            var list = installObjs.FindAll(x => x.autoInstall);
            return list;
        }
        private bool HaveInstallObjInstalled(InstallObj obj)
        {
            return obj.Installed;
        }
        private bool IsInstallStep(InstallObj obj)
        {
            return installObjs.Contains(obj) && obj.Started;
        }
        

        public void OnStartExecute(bool forceauto)
        {
            this.isForceAuto = forceauto;
            SetElemetsStart();
            AutoInstallWhenNeed(forceauto);
        }
        public void OnEndExecute()
        {
            List<InstallObj> posList = GetNotInstalledPosList();
            QuickInstallObjListObjects(posList);
            SetCompleteNotify();
        }
        public void OnUnDoExecute()
        {
            QuickUnInstall();
            List<InstallObj> posList = GetInstalledPosList();
            QuickUnInstallObjListObjects(posList);
            SetUnDoNotify();
        }
    }

}