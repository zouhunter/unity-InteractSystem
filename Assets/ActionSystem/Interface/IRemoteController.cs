using UnityEngine.Events;
namespace WorldActionSystem
{
    /// <summary>
    /// 状态控制器
    /// </summary>
    public interface IRemoteController
    {
        ActionCommand CurrCommand { get; }
        bool StartExecuteCommand(UnityAction onEndExecute,bool forceAuto);//返回操作成功与否
        bool EndExecuteCommand();
        bool UnDoCommand();
        bool ToTargetCommand(string stap);
        bool ExecuteMutliCommand(int stap);
        void ToAllCommandStart();
        void ToAllCommandEnd();
    }
}