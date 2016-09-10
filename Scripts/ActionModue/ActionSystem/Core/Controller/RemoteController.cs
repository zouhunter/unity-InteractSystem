using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{
		
public class RemoteController : IRemoteController
{
    int index;
    bool started;
    public UnityAction<bool> onEndExecute;
    List<ActionCommand> commandList;
    public ActionCommand CurrCommand
    {
        get { return commandList[index]; }
    }
    public RemoteController(IEnumerable<ActionCommand> commandList)
    {
        index = 0;
        this.commandList = new List<ActionCommand>(commandList);
    }


    bool HaveCommand(int id)
    {
        if (id >= 0 && id < commandList.Count)
        {
            return true;
        }
        return false;
    }

    bool HaveNext()
    {
        if (index < commandList.Count - 1)
        {
            return true;
        }
        return false;
    }

    bool HaveLast()
    {
        if (index > 0)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 开启一个命令,并返回正常执行与否
    /// </summary>
    public bool StartExecuteCommand(UnityAction<bool> onEndExecute)
    {
        if (!started)
        {
            started = true;
            this.onEndExecute = onEndExecute;
            CurrCommand.StartExecute();
            return true;
        }
        else
        {
            this.onEndExecute = onEndExecute;
            return false;
        }
    }

    /// <summary>
    /// 结束已经开始的命令
    /// </summary>
    public bool EndExecuteCommand()
    {
        started = false;
        CurrCommand.EndExecute();

        bool haveNext = HaveNext();
        if (HaveNext())
        {
            index++;
        }

        if (onEndExecute != null)
        {
            onEndExecute(haveNext);
        }

        return haveNext;
    }

    /// <summary>
    /// 撤消操作，并返回能否继续撤销
    /// </summary>
    /// <returns></returns>
    public bool UnDoCommand()
    {
        bool haveLast = HaveLast();
        if (started)
        {
            started = false;
            CurrCommand.UnDoCommand();
        }
        else
        {
            if (haveLast)
            {
                index--;
                CurrCommand.UnDoCommand();
            }
        }

        return haveLast;
    }

    /// <summary>
    /// 执行多个步骤
    /// </summary>
    /// <param name="stap"></param>
    public bool ExecuteMutliCommand(int stap)
    {
        bool haveNext = true;
        if (stap != 0 && HaveCommand(stap + index))
        {
            if (stap > 0)
            {
                for (int i = 0; i < stap; i++)
                {
                    haveNext &= EndExecuteCommand();
                }
            }
            else
            {
                for (int i = 0; i < -stap; i++)
                {
                    haveNext &= UnDoCommand();
                }
            }
        }
        return haveNext;
    }
    /// <summary>
    /// 回到命令列表起始
    /// </summary>
    public void ToAllCommandStart()
    {
        ExecuteMutliCommand(-index);
    }
    /// <summary>
    /// 完成所有命令
    /// </summary>
    public void ToAllCommandEnd()
    {
        ExecuteMutliCommand(commandList.Count - 1 - index);
    }

    /// <summary>
    /// 执行到指定的步骤
    /// </summary>
    /// <param name="stapName"></param>
    public bool ToTargetCommand(string stapName)
    {
        bool haveNext = true;
        ActionCommand cmd = commandList.Find((x) => stapName == x.StapName);
        if (cmd != null)
        {
            int indexofCmd = commandList.IndexOf(cmd);
            haveNext &= ExecuteMutliCommand(indexofCmd - index);
        }
        return haveNext;
    }
}

	}