using UnityEngine.Events;
namespace InteractSystem
{
    /// <summary>
    /// 状态控制器
    /// </summary>
    public interface ICommandController
    {
        ActionCommand CurrCommand { get; }
		bool HaveCommand(string stepName);
        bool StartExecuteCommand(UnityAction<bool> onEndExecute,bool forceAuto);//返回操作成功与否
        bool EndExecuteCommand();
        void OnEndExecuteCommand(string step);//外部触发结束
        bool UnDoCommand();
        bool ToTargetCommand(string step);
        bool ExecuteMutliCommand(int step);
        void ToAllCommandStart();
        void ToAllCommandEnd();
    }
}