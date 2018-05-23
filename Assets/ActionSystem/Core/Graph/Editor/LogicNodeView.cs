using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using NodeGraph;
using NodeGraph.DataModel;

namespace WorldActionSystem.Graph
{
    [CustomNodeView(typeof(LogicNode))]
    public class LogicNodeView : NodeView
    {
        public override Node target
        {
            get
            {
                return base.target;
            }

            set
            {
                base.target = value;
            }
        }
        private GUIStyle _activeStyle;
        private GUIStyle _inactiveStyle;
        public override GUIStyle ActiveStyle
        {
            get
            {
                if (target is LogicNode)
                {
                    _activeStyle = (target as LogicNode).activestyle;
                }
                return _activeStyle;
            }
        }
        public override GUIStyle InactiveStyle
        {
            get
            {
                if (target is LogicNode)
                {
                    _inactiveStyle = (target as LogicNode).style;
                }
                return _inactiveStyle;
            }
        }
    }

}