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

    [CustomNodeView(typeof(EndNode))]
    public class EndNodeView : ActionNodeView
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
                    _activeStyle.normal.background = BackgroundContent.LoadTexture("on_node7") as Texture2D;
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
                    _inactiveStyle.normal.background = BackgroundContent.LoadTexture("node7") as Texture2D;
                    _inactiveStyle.border = new RectOffset(11, 11, 11, 19);
                }
                return _inactiveStyle;
            }
        }
        public override string Category
        {
            get
            {
                return "complete";
            }
        }
    }
}