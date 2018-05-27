using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using NodeGraph;
using NodeGraph.DataModel;
using UnityEditor;
using System;

namespace WorldActionSystem.Graph
{

    [CustomNodeView(typeof(StartNode))]
    public class StartNodeView : ActionNodeView
    {
        private const float btnWidth = 15;
        public override string Category
        {
            get
            {
                return "lunch";
            }
        }
        protected override GUIStyle CreateActiveStyle()
        {
            var activeStyle = new GUIStyle();
            activeStyle.normal.background = GraphUtil.BackgroundContent.LoadTexture("on_node5") as Texture2D;
            activeStyle.border = new RectOffset(11, 11, 11, 19);
            return activeStyle;
        }

        protected override GUIStyle CreateInactiveStyle()
        {
            var inactiveStyle = new GUIStyle();
            inactiveStyle.normal.background = GraphUtil.BackgroundContent.LoadTexture("node5") as Texture2D;
            inactiveStyle.border = new RectOffset(11, 11, 11, 19);
            return inactiveStyle;
        }
        public StartNode node { get { return target as StartNode; } }

        public override void OnNodeGUI(Rect position, NodeData data)
        {
            base.OnNodeGUI(position, data);
            var btnRect = new Rect(position.x + btnWidth, position.y + btnWidth, btnWidth, btnWidth);
            if (GUI.Button(btnRect,"+"))
            {
                var count = data.OutputPoints.Count;
                data.AddOutputPoint(count.ToString(), "actionconnect", 100);
            }
            btnRect.x += btnWidth;
            if (GUI.Button(btnRect,"-"))
            {
                if(data.OutputPoints.Count > 1)
                {
                    data.OutputPoints.RemoveAt(data.OutputPoints.Count - 1);
                }
            }
        }
    }

}