using UnityEngine.Events;
namespace WorldActionSystem
{
		
public interface IRemoteController
{
    ActionCommand CurrCommand { get; }
    bool StartExecuteCommand(UnityAction<bool> onEndExecute);
    bool EndExecuteCommand();
    bool UnDoCommand();
    bool ToTargetCommand(string commandName);
    bool ExecuteMutliCommand(int stap);
    void ToAllCommandStart();
    void ToAllCommandEnd();
} 
	}