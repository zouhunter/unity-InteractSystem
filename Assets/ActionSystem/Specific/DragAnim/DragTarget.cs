using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{
    public class DragTarget :MonoBehaviour
    {
        /// <summary>
        /// 按步骤将子目标点进行记录
        /// </summary>
        Dictionary<string, List<DragPos>> installDic = new Dictionary<string, List<DragPos>>();
        List<DragPos> currInstallPoss = new List<DragPos>();
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
            DragPos installPos;
            foreach (Transform item in transform)
            {
                installPos = item.GetComponent<DragPos>();
                //记录步骤对象
                if (installDic.ContainsKey(installPos.stapName))
                {
                    installDic[installPos.stapName].Add(installPos);
                }
                else
                {
                    installDic[installPos.stapName] = new List<DragPos>() { (installPos) };
                }
            }
            allChildRecord = true;
        }
        /// <summary>
        /// 获取坐标字典
        /// </summary>
        /// <param name="onDicCrateed"></param>
        public void GetInstallDicAsync(UnityAction<Dictionary<string, List<DragPos>>> onDicCrateed)
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
        IEnumerator WaitToEnd(UnityAction<Dictionary<string, List<DragPos>>> onDicCrateed)
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
        public bool IsInstallStep(DragPos installObj)
        {
            return currInstallPoss.Contains(installObj);
        }

        /// <summary>
        /// 是否已经安装了零件
        /// </summary>
        /// <param name="installObj"></param>
        /// <returns></returns>
        public bool HaveInstallPosInstalled(DragPos installObj)
        {
            return installObj.Installed;
        }

        /// <summary>
        /// 获取当前未安装完成的坐标
        /// </summary>
        /// <returns></returns>
        public List<DragPos> GetNotInstalledPosList()
        {
            {
                List<DragPos> installPoss = new List<DragPos>();
                DragPos item;
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
        public List<DragPos> GetNeedAutoInstallPosList()
        {
            List<DragPos> installPoss = new List<DragPos>();
            DragPos item;
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

        public List<DragPos> GetInstalledPosList()
        {
            List<DragPos> installPoss = new List<DragPos>();
            DragPos item;
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