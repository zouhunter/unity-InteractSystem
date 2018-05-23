using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using NodeGraph;
namespace WorldActionSystem.Graph
{
    public class AcionGraphCtrl : NodeGraphController
    {
        public override string Group
        {
            get
            {
                return "ActionSystem";
            }
        }
    }

}
