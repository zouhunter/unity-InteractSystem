using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{

    /// <summary>
    /// 提供拿起安装和快速安装等功能
    /// </summary>
    public class InstallController /*: IInstallCtrl*/
    {
        InstallElements startParent;
        InstallTarget endParent;
        IHighLightItems HighLight;

        private InstallItem pickedUpObj;
        private bool pickedUp;
        private float distence { get { return startParent.Distence; } set { startParent.Distence = value; } }
        private InstallObj installPos;

        private Ray ray;
        private RaycastHit hit;
        private RaycastHit[] hits;
        private bool installAble;
        private string resonwhy;
        private string currStepName;
        public UserError InstallErr;
        public StepComplete onStepComplete;
        public InstallController(InstallElements startParent, InstallTarget endParent, StepComplete onStepComplete)
        {
            this.startParent = startParent;
            this.endParent = endParent;
            HighLight = new ShaderHighLight();
            this.onStepComplete = onStepComplete;
            startParent.onInstall = OnEndInstall;
        }

        public void SwitchHighLight(bool open)
        {
            HighLight.SetState(open);
        }

        #region 鼠标操作事件
        public void Reflesh()
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

        public void OnLeftMouseClicked()
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
            if (Physics.Raycast(ray, out hit, 100, (1 << Setting.installObjLayer)))
            {
                pickedUpObj = hit.collider.GetComponent<InstallItem>();
                if (pickedUpObj != null && startParent.PickUpObject(pickedUpObj))
                {
                    pickedUp = true;

                    if (!PickUpedCanInstall())
                    {
                        if (HighLight != null) HighLight.HighLightTarget(pickedUpObj.Render, Color.yellow);
                    }
                    else
                    {
                        if (HighLight != null) HighLight.HighLightTarget(pickedUpObj.Render, Color.cyan);
                    }
                }
            }
        }

        private bool PickUpedCanInstall()
        {
            bool canInstall = false;
            List<InstallObj> poss = endParent.GetNotInstalledPosList();
            for (int i = 0; i < poss.Count; i++)
            {
                if (!endParent.HaveInstallObjInstalled(poss[i]) && endParent.IsInstallStep(poss[i]) && startParent.CanInstallToPos(poss[i]))
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
                            Debug.LogError("零件未挂InstallObj脚本");
                        }
                        else if (!endParent.IsInstallStep(installPos))
                        {
                            installAble = false;
                            resonwhy = "当前安装步骤并非" + installPos.StepName;
                        }
                        else if (endParent.HaveInstallObjInstalled(installPos))
                        {
                            installAble = false;
                            resonwhy = "安装点已经安装了其他零件";
                        }
                        else if (!startParent.CanInstallToPos(installPos))
                        {
                            installAble = false;
                            resonwhy = "拿起零件和安装点不对应";
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
                    resonwhy = "不要乱放零件";
                }
            }

            if (installAble)
            {
                //可安装显示绿色
                if (HighLight != null) HighLight.HighLightTarget(pickedUpObj.Render, Color.green);
            }
            else
            {
                //不可安装红色
                if (HighLight != null) HighLight.HighLightTarget(pickedUpObj.Render, Color.red);
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
                startParent.InstallPickedUpObject(installPos);
            }
            else
            {
                OnInstallErr(resonwhy);
                startParent.PickDownPickedUpObject();
            }

            pickedUp = false;
            installAble = false;
            if (HighLight != null) HighLight.UnHighLightTarget(pickedUpObj.Render);
        }

        private Ray disRay;
        private RaycastHit disHit;
        private string obstacle = "Obstacle";
        /// <summary>
        /// 跟随鼠标
        /// </summary>
        void MoveWithMouse(float dis)
        {
            disRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(disRay, out disHit, dis, LayerMask.GetMask(obstacle)))
            {
                pickedUpObj.transform.position = disHit.point;
            }
            else
            {
                pickedUpObj.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, dis));
            }
        }
        #endregion

        private void OnEndInstall(InstallItem obj)
        {
            if (CurrStapComplete())
            {
                List<InstallObj> posList = endParent.GetInstalledPosList();
                startParent.SetCompleteNotify(posList);
                if (onStepComplete != null)
                    onStepComplete.Invoke(currStepName);
            }
        }

        /// <summary>
        /// 结束当前步骤安装
        /// </summary>
        /// <param name="stepName"></param>
        public void EndInstall(string stepName)
        {
            List<InstallObj> installed = endParent.GetInstalledPosList();
            startParent.QuickUnInstallObjListObjects(installed);
            List<InstallObj> posList = endParent.GetNotInstalledPosList();
            startParent.QuickInstallObjListObjects(posList);
            endParent.SetSepComplete(stepName);
        }

        public bool CurrStapComplete()
        {
            return endParent.AllElementInstalled();
        }

        public void SetStapActive(string stepName)
        {
            currStepName = stepName;
            if (endParent.SetStapActive(stepName))
            {
                List<InstallObj> posList = endParent.GetNotInstalledPosList();
                startParent.SetStartNotify(posList);
            }
            else
            {
                Debug.LogError("步骤不存在：" + currStepName);
            }
        }

        /// <summary>
        /// 自动安装部分需要进行自动安装的零件
        /// </summary>
        /// <param name="stepName"></param>
        public void AutoInstallWhenNeed(string stepName, bool autoInstall)
        {
            List<InstallObj> posList = null;
            if (autoInstall)
            {
                posList = endParent.GetNotInstalledPosList();
            }
            else
            {
                posList = endParent.GetNeedAutoInstallObjList();
            }

            if (posList != null) startParent.InstallObjListObjects(posList);

            pickedUp = false;
        }

        public void UnInstall(string stepName)
        {
            SetStapActive(stepName);
            List<InstallObj> posList = endParent.GetInstalledPosList();
            startParent.UnInstallObjListObjects(posList);
            endParent.SetSepUnDo(stepName);
        }

        public void QuickUnInstall(string stepName)
        {
            SetStapActive(stepName);
            List<InstallObj> posList = endParent.GetInstalledPosList();
            startParent.QuickUnInstallObjListObjects(posList);
            endParent.SetSepUnDo(stepName);
        }

        private void OnInstallErr(string err)
        {
            if (InstallErr != null)
            {
                InstallErr.Invoke(currStepName, err);
            }
        }
    }

}