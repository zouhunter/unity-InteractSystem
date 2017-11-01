using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{

    public class InstallCtrl : IActionCtroller
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
        private float hitDistence;
        private List<InstallObj> installObjs = new List<InstallObj>();
        private float elementDistence;
        private Camera viewCamera { get { return CameraController.ActiveCamera; } }
        private bool activeNotice { get { return Setting.highLightNotice; } }
        public InstallCtrl(float hitDistence,float elementDistence, InstallObj[] installObjs)
        {
            highLight = new ShaderHighLight();
            this.hitDistence = hitDistence;
            this.elementDistence = elementDistence;
            this.installObjs.AddRange(installObjs);
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
                MoveWithMouse(elementDistence += Input.GetAxis("Mouse ScrollWheel"));
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
            ray = viewCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, hitDistence, (1 << Setting.pickUpElementLayer)))
            {
                pickedUpObj = hit.collider.GetComponent<PickUpAbleElement>();
                if (pickedUpObj != null)
                {
                    if (pickedUpObj.Installed){
                        pickedUpObj.NormalUnInstall();
                    }

                    pickedUpObj.OnPickUp();
                    pickedUp = true;
                    elementDistence = Vector3.Distance(viewCamera.transform.position, pickedUpObj.transform.position);
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
            ray = viewCamera.ScreenPointToRay(Input.mousePosition);
            hits = Physics.RaycastAll(ray, hitDistence, (1 << Setting.installPosLayer));
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
                if (activeNotice) highLight.HighLightTarget(pickedUpObj.Render, Color.green);
            }
            else
            {
                //不可安装红色
                if (activeNotice) highLight.HighLightTarget(pickedUpObj.Render, Color.red);
            }
        }

        /// <summary>
        /// 尝试安装元素
        /// </summary>
        void TryInstallObject()
        {
            ray = viewCamera.ScreenPointToRay(Input.mousePosition);
            if (installAble)
            {
                var status = installPos.Attach(pickedUpObj);
                if (status)
                {
                    pickedUpObj.QuickInstall(installPos.gameObject);
                }
                installPos.OnEndExecute(false);
            }
            else
            {
                pickedUpObj.NormalMoveBack();
                UserError(resonwhy);
            }

            pickedUp = false;
            installAble = false;
            if (activeNotice) highLight.UnHighLightTarget(pickedUpObj.Render);
        }

        private Ray disRay;
        private RaycastHit disHit;

        /// <summary>
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

        private List<InstallObj> GetNotInstalledPosList()
        {
            var list = installObjs.FindAll(x => !x.Installed);
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
        #endregion

        public void OnStartExecute(bool forceauto)
        {
            SetStartNotify();
        }
        public void OnEndExecute()
        {
            SetCompleteNotify(false);
        }
        public void OnUnDoExecute()
        {
            SetCompleteNotify(true);
        }

        /// <summary>
        /// 将可安装元素全部显示出来
        /// </summary>
        private void SetStartNotify()
        {
            List<InstallObj> posList = GetNotInstalledPosList();
            var keyList = new List<string>();
            foreach (var pos in posList)
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
        private void SetCompleteNotify(bool undo)
        {
            var keyList = new List<string>();
            foreach (var pos in installObjs)
            {
                if (!keyList.Contains(pos.name))
                {
                    List<PickUpAbleElement> listObjs = ElementController.GetElements(pos.name);
                    if (listObjs == null) throw new Exception("元素配制错误:没有:" + pos.name);
                    for (int j = 0; j < listObjs.Count; j++)
                    {
                        if (!listObjs[j].Installed && undo)
                        {
                            listObjs[j].StepUnDo();
                        }
                        else
                        {
                            listObjs[j].StepComplete();
                        }
                    }
                }
            }
        }
    }

}