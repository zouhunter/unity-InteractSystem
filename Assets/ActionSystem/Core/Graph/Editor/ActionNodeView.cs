using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeGraph;
using NodeGraph.DataModel;

namespace WorldActionSystem.Graph
{
    public abstract class ActionNodeView : NodeView
    {
        private GUIStyle _activeStyle;
        private GUIStyle _inactiveStyle;
        public override GUIStyle ActiveStyle
        {
            get
            {
                if (_activeStyle == null)
                {

                    _activeStyle = CreateActiveStyle();
                }
                return _activeStyle;
            }
        }
        public override GUIStyle InactiveStyle
        {
            get
            {
                if (_inactiveStyle == null)
                {
                    _inactiveStyle = CreateInactiveStyle();
                    return _inactiveStyle;
                }
                return _inactiveStyle;
            }
        }

        protected abstract GUIStyle CreateActiveStyle();
        protected abstract GUIStyle CreateInactiveStyle();
        
    }
}