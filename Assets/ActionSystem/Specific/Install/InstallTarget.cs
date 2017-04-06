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
    /// 安装过程中控制解锁
    /// </summary>
    public class InstallTarget : MonoBehaviour, IInstallEnd
    {
        /// <summary>
        /// 按步骤将子目标点进行记录
        /// </summary>
        Dictionary<string, List<InstallPos>> installDic = new Dictionary<string, List<InstallPos>>();
        List<InstallPos> currInstallPoss = new List<InstallPos>();
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
            InstallPos installPos;
            foreach (Transform item in transform)
            {
                installPos = item.GetComponent<InstallPos>();
                //记录步骤对象
                if (installDic.ContainsKey(installPos.stapName))
                {
                    installDic[installPos.stapName].Add(installPos);
                }
                else
                {
                    installDic[installPos.stapName] = new List<InstallPos>() { (installPos) };
                }
            }
            allChildRecord = true;
        }
        /// <summary>
        /// 获取坐标字典
        /// </summary>
        /// <param name="onDicCrateed"></param>
        public void GetInstallDicAsync(UnityAction<Dictionary<string, List<InstallPos>>> onDicCrateed)
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
        IEnumerator WaitToEnd(UnityAction<Dictionary<string, List<InstallPos>>> onDicCrateed)
        {
            yield return new WaitWhile(() => allChildRecord == false);
            onDicCrateed(installDic);
        }

        /// <summary>
        /// 激活当前步骤所有安装坐标
        /// </summary>
        /// <param name="stap"></param>
        public bool SetStapActive(string stap)
        {
            if (installDic.TryGetValue(stap, out currInstallPoss))
            {
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
        public bool IsInstallStep(InstallPos installObj)
        {
            return currInstallPoss.Contains(installObj);
        }

        /// <summary>
        /// 是否已经安装了零件
        /// </summary>
        /// <param name="installObj"></param>
        /// <returns></returns>
        public bool HaveInstallPosInstalled(InstallPos installObj)
        {
            return installObj.Installed;
        }

        /// <summary>
        /// 获取当前未安装完成的坐标
        /// </summary>
        /// <returns></returns>
        public List<InstallPos> GetNotInstalledPosList()
        {
            {
                List<InstallPos> installPoss = new List<InstallPos>();
                InstallPos item;
                for (int i = 0; i < currInstallPoss.Count; i++)
                {
                    item = currInstallPoss[i];
                    if (!currInstallPoss[i].Installed)
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
        public List<InstallPos> GetNeedAutoInstallPosList(bool fauseAuto)
        {
            if (fauseAuto)
            {
                return currInstallPoss;
            }
            else
            {
                List<InstallPos> installPoss = new List<InstallPos>();
                InstallPos item;
                for (int i = 0; i < currInstallPoss.Count; i++)
                {
                    item = currInstallPoss[i];
                    if (currInstallPoss[i].autoInstall)
                    {
                        installPoss.Add(item);
                    }
                }
                return installPoss;
            }
        }
        /// <summary>
        /// 当前步骤完成与否
        /// </summary>
        /// <returns></returns>
        public bool AllElementInstalled()
        {
            bool allInstall = true;
            for (int i = 0; i < currInstallPoss.Count; i++)
            {
                allInstall &= currInstallPoss[i].Installed;
            }
            return allInstall;
        }

        public List<InstallPos> GetInstalledPosList()
        {
            List<InstallPos> installPoss = new List<InstallPos>();
            InstallPos item;
            for (int i = 0; i < currInstallPoss.Count; i++)
            {
                item = currInstallPoss[i];
                if (currInstallPoss[i].Installed)
                {
                    installPoss.Add(item);
                }
            }
            return installPoss;
        }
    }

}