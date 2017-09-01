using UnityEngine.Events;
namespace WorldActionSystem
{
    /// <summary>
    /// 状态控制器
    /// </summary>
    public interface IRemoteController
    {
        IActionCommand CurrCommand { get; }
        bool StartExecuteCommand(UnityAction onEndExecute,bool forceAuto);//返回操作成功与否
        bool EndExecuteCommand();
        bool UnDoCommand();
        bool ToTargetCommand(string step);
        bool ExecuteMutliCommand(int step);
        void ToAllCommandStart();
        void ToAllCommandEnd();
    }
}