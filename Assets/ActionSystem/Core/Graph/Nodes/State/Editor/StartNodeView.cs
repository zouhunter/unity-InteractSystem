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
        private GUIStyle _activeStyle;
        private GUIStyle _inactiveStyle;
        private const float btnWidth = 15;

        public override GUIStyle ActiveStyle
        {
            get
            {
                if (_activeStyle == null)
                {

                    _activeStyle = new GUIStyle();
                    _activeStyle.normal.background = BackgroundContent.LoadTexture("on_node5") as Texture2D;
                    _activeStyle.border = new RectOffset(11, 11, 11, 19);
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

                    _inactiveStyle = new GUIStyle();
                    _inactiveStyle.normal.background = BackgroundContent.LoadTexture("node5") as Texture2D;
                    _inactiveStyle.border = new RectOffset(11, 11, 11, 19);
                }
                return _inactiveStyle;
            }
        }
        public override string Category
        {
            get
            {
                return "lunch";
            }
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