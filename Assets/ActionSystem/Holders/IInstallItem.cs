using UnityEngine;
using UnityEngine.Events;

namespace WorldActionSystem
{
    public interface IInstallItem
    {
        void NormalInstall(GameObject target);
        void QuickInstall(GameObject target);
        void NormalUnInstall();
        void QuickUnInstall();
        void StepComplete();
        void SetActive(bool v);
    }
}