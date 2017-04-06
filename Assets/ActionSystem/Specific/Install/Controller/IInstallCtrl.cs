using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{

    public interface IInstallCtrl
    {
        //刷新状态
        void Reflesh();
        event UnityAction<string> InstallErr;
        void SwitchHighLight(bool ison);
        bool CurrStapComplete();
        void SetStapActive(string stapName);
        void AutoInstallWhenNeed(string stapName, bool fauseAuto = false);
        void EndInstall(string stapName);
        void UnInstall(string stapName);
        void QuickUnInstall(string stapName);

    }
}