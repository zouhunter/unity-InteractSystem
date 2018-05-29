using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem.Structure
{
    public class ActionStateMechine
    {
        public ExecuteState currentState { get; private set; }
        public Dictionary<State, ExecuteState> statemap = new Dictionary<State, ExecuteState>();

        public ActionStateMechine()
        {
            statemap.Add(State.Start,new StartState());
            statemap.Add(State.End,new EndState());
            statemap.Add(State.Logic,new LogicState());
            statemap.Add(State.Operate,new OperateState());

            foreach (var item in statemap) {
                item.Value.stateMechine = this;
            }
        }
        public void SetState(State state)
        {
            currentState = statemap[state];
        }

        public void Execute(ExecuteUnit unit)
        {
            if (unit.node is Graph.StartNode)
            {
                SetState(State.Start);
            }
            else if(unit.node is Graph.EndNode)
            {
                SetState(State.End);
            }
            else if(unit.node is Graph.LogicNode)
            {
                SetState(State.Logic);
            }
            else if(unit.node is Graph.OperateNode)
            {
                SetState(State.Operate);
            }

            currentState.Execute(unit);
        }
    }
}