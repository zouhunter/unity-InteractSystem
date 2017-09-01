﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{

    /// <summary>
    /// 安装过程中控制解锁
    /// </summary>
    public class InstallTarget : MonoBehaviour
    {
        /// <summary>
        /// 按步骤将子目标点进行记录
        /// </summary>
        Dictionary<string, List<InstallObj>> installDic = new Dictionary<string, List<InstallObj>>();
        List<InstallObj> currInstallObjs = new List<InstallObj>();
        bool allChildRecord;
        void Start()
        {
            SaveTargetObjects();
        }
        /// <summary>
        /// 保存安装坐标字典
        /// </summary>
        void SaveTargetObjects()
        {
            InstallObj installPos;
            foreach (Transform item in transform)
            {
                installPos = item.GetComponent<InstallObj>();
                //记录步骤对象
                if (installDic.ContainsKey(installPos.StepName))
                {
                    installDic[installPos.StepName].Add(installPos);
                }
                else
                {
                    installDic[installPos.StepName] = new List<InstallObj>() { (installPos) };
                }
            }
            allChildRecord = true;
        }
        /// <summary>
        /// 获取坐标字典
        /// </summary>
        /// <param name="onDicCrateed"></param>
        public void GetInstallDicAsync(UnityAction<Dictionary<string, List<InstallObj>>> onDicCrateed)
        {
            if (allChildRecord)
            {
                onDicCrateed(installDic);
            }
            else
            {
                StartCoroutine(WaitToEnd(onDicCrateed));
            }
        }
        IEnumerator WaitToEnd(UnityAction<Dictionary<string, List<InstallObj>>> onDicCrateed)
        {
            yield return new WaitWhile(() => allChildRecord == false);
            onDicCrateed(installDic);
        }

        /// <summary>
        /// 激活当前步骤所有安装坐标
        /// </summary>
        /// <param name="step"></param>
        public bool SetStapActive(string step)
        {
            if (installDic.TryGetValue(step, out currInstallObjs))
            {
                foreach (var item in currInstallObjs)
                {
                    item.StartExecute();
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 是否是当前安装步骤
        /// </summary>
        /// <param name="installObj"></param>
        /// <returns></returns>
        public bool IsInstallStep(InstallObj installObj)
        {
            return currInstallObjs.Contains(installObj);
        }

        /// <summary>
        /// 是否已经安装了零件
        /// </summary>
        /// <param name="installObj"></param>
        /// <returns></returns>
        public bool HaveInstallObjInstalled(InstallObj installObj)
        {
            return installObj.Installed;
        }

        /// <summary>
        /// 获取当前未安装完成的坐标
        /// </summary>
        /// <returns></returns>
        public List<InstallObj> GetNotInstalledPosList()
        {
            {
                List<InstallObj> installPoss = new List<InstallObj>();
                InstallObj item;
                for (int i = 0; i < currInstallObjs.Count; i++)
                {
                    item = currInstallObjs[i];
                    if (!currInstallObjs[i].Installed)
                    {
                        installPoss.Add(item);
                    }
                }
                return installPoss;
            }
        }
        /// <summary>
        /// 获取当前步骤需要自动安装的坐标
        /// </summary>
        /// <returns></returns>
        public List<InstallObj> GetNeedAutoInstallObjList()
        {
            List<InstallObj> installPoss = new List<InstallObj>();
            InstallObj item;
            for (int i = 0; i < currInstallObjs.Count; i++)
            {
                item = currInstallObjs[i];
                if (currInstallObjs[i].autoInstall)
                {
                    installPoss.Add(item);
                }
            }
            return installPoss;
        }
        /// <summary>
        /// 当前步骤完成与否
        /// </summary>
        /// <returns></returns>
        public bool AllElementInstalled()
        {
            bool allInstall = true;
            for (int i = 0; i < currInstallObjs.Count; i++)
            {
                allInstall &= currInstallObjs[i].Installed;
            }
            return allInstall;
        }

        public List<InstallObj> GetInstalledPosList()
        {
            List<InstallObj> installPoss = new List<InstallObj>();
            InstallObj item;
            for (int i = 0; i < currInstallObjs.Count; i++)
            {
                item = currInstallObjs[i];
                if (currInstallObjs[i].Installed)
                {
                    installPoss.Add(item);
                }
            }
            return installPoss;
        }

        internal void SetSepComplete(string step)
        {
            if (installDic.TryGetValue(step, out currInstallObjs))
            {
                foreach (var item in currInstallObjs)
                {
                    item.EndExecute();
                }
            }
        }

        internal void SetSepUnDo(string step)
        {
            if (installDic.TryGetValue(step, out currInstallObjs))
            {
                foreach (var item in currInstallObjs)
                {
                    item.UnDoExecute();
                }
            }
        }
    }

}