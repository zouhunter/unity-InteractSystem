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
        protected override GUIStyle CreateActiveStyle()
        {
            var style = new GUIStyle();
            style.normal.background = GraphUtil.BackgroundContent.LoadTexture("on_node7") as Texture2D;
            style.border = new RectOffset(11, 11, 11, 19);
            return style;
        }
        protected override GUIStyle CreateInactiveStyle()
        {
            var style = new GUIStyle();
            style.normal.background = GraphUtil.BackgroundContent.LoadTexture("node7") as Texture2D;
            style.border = new RectOffset(11, 11, 11, 19);
            return style;
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