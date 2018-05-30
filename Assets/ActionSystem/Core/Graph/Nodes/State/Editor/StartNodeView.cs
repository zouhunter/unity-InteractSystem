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
            DrawAddNodes(position, data);
        }
    }

}