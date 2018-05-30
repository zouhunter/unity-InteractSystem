using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeGraph;
using NodeGraph.DataModel;

namespace WorldActionSystem.Graph
{
    public abstract class ActionNodeView : NodeView
    {
        protected const float btnWidth = 15;

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
        

        protected virtual void DrawAddNodes(Rect position,NodeData data)
        {
            var nodePostion = new Rect(position.x + position.width - 40, 25, btnWidth, btnWidth);

            if (GUI.Button(nodePostion, "+"))
            {
                var count = data.OutputPoints.Count;
                data.AddOutputPoint(count.ToString(), "actionconnect", 100);
            }

            for (int i = 0; i < data.OutputPoints.Count; i++)
            {
                if (i == 0) continue;
                nodePostion.y += 31.8f;

                if (GUI.Button(nodePostion, "-"))
                {
                    if (data.OutputPoints.Count > 1)
                    {
                        data.OutputPoints.RemoveAt(i);
                        for (int j = i; j < data.OutputPoints.Count; j++)
                        {
                            data.OutputPoints[j].Label = j.ToString();
                        }
                    }
                }
            }

        }
    }
}