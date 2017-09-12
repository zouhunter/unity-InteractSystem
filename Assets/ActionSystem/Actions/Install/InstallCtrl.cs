using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{

    public class InstallCtrl
    {
        public UnityAction<string> onInstallError;
        public UnityAction onComplete;

        private MonoBehaviour trigger;
        public ElementGroup elementGroup { get; set; }
        public bool Active { get; private set; }
        IHighLightItems highLight;
        private InstallItem pickedUpObj;
        private bool pickedUp;
        private InstallObj installPos;
        private Ray ray;
        private RaycastHit hit;
        private RaycastHit[] hits;
        private bool installAble;
        private string resonwhy;
        private float distence;
        //private string currStepName;
        private List<InstallObj> installObjs;
        private Coroutine coroutine;
        public InstallCtrl(MonoBehaviour trigger,float distence,bool hightLightOn,List<InstallObj> installObjs)
        {
            this.trigger = trigger;
            highLight = new ShaderHighLight();
            highLight.SetState(hightLightOn);
            this.distence = distence;
            this.installObjs = installObjs;
        }

        #region 鼠标操作事件
        IEnumerator Update()
        {
            elementGroup.onInstall += OnEndInstall;

            while (true)
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
                yield return null;
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
                if (pickedUpObj != null && elementGroup.PickUpObject(pickedUpObj))
                {
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
                if (!HaveInstallObjInstalled(poss[i]) && IsInstallStep(poss[i]) && elementGroup.CanInstallToPos(poss[i]))
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
                        else if (!elementGroup.CanInstallToPos(installPos))
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
                elementGroup.InstallPickedUpObject(installPos);
            }
            else
            {
                OnInstallErr(resonwhy);
                elementGroup.PickDownPickedUpObject();
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
            if (Physics.Raycast(disRay, out disHit, dis,1<<Setting.obstacleLayer))
            {
                pickedUpObj.transform.position = disHit.point;
            }
            else
            {
                pickedUpObj.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, dis));
            }
        }
        #endregion

        private void OnEndInstall()
        {
            if (AllElementInstalled())
            {
                List<InstallObj> posList = GetInstalledPosList();
                elementGroup.SetCompleteNotify(posList);
                if(onComplete!= null) onComplete();
            }
        }

        /// <summary>
        /// 结束当前步骤安装
        /// </summary>
        /// <param name="stepName"></param>
        public void EndInstall()
        {
            List<InstallObj> posList = GetNotInstalledPosList();
            elementGroup.QuickInstallObjListObjects(posList);
            SetSepComplete();
        }

        public void SetStapActive()
        {
            SetObjsActive();
            List<InstallObj> posList = GetNotInstalledPosList();
            elementGroup.SetStartNotify(posList);
            if (coroutine == null) coroutine = trigger.StartCoroutine(Update());
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

            if (posList != null) elementGroup.InstallObjListObjects(posList);

            pickedUp = false;
        }

        public void UnInstall(string stepName)
        {
            SetStapActive();
            List<InstallObj> posList = GetInstalledPosList();
            elementGroup.UnInstallObjListObjects(posList);
            SetSepUnDo();
        }

        public void QuickUnInstall()
        {
            SetStapActive();
            List<InstallObj> posList = GetInstalledPosList();
            elementGroup.QuickUnInstallObjListObjects(posList);
            SetSepUnDo();
        }

        private void OnInstallErr(string err)
        {
           if(onInstallError != null)  onInstallError(err);
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
            return installObjs.Contains(obj);
        }
        private void SetSepComplete()
        {
            if (coroutine != null)
                trigger.StopCoroutine(coroutine);
            coroutine = null;
            elementGroup.onInstall -= OnEndInstall;
        }
        private bool AllElementInstalled()
        {
            var notInstalls = installObjs.FindAll(x => !x.Installed);
            return notInstalls.Count == 0;
        }
        private void SetObjsActive()
        {
            foreach (var item in installObjs)
            {
                item.StartExecute();
            }
        }
        private void SetSepUnDo()
        {
            foreach (var item in installObjs)
            {
                item.UnDoExecute();
            }
        }
    }

}