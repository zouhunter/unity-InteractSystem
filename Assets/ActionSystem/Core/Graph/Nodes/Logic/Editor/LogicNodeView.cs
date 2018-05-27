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

    [CustomNodeView(typeof(LogicNode))]
    public class LogicNodeView : ActionNodeView
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
        private Texture or_texture;
        public override float SuperHeight
        {
            get
            {
                return base.SuperHeight;
            }
        }
        public override float SuperWidth
        {
            get
            {
                return -40;
            }
        }

        protected override GUIStyle CreateActiveStyle()
        {
            var style = new GUIStyle();
            style.normal.background = GraphUtil.IconContents.LoadTexture("Generic@32x") as Texture2D;
            style.border = new RectOffset(11, 11, 11, 11);
            return style;
        }

        protected override GUIStyle CreateInactiveStyle()
        {
            var style = new GUIStyle();
            style.normal.background = GraphUtil.IconContents.LoadTexture("Scalar@32x") as Texture2D;
            style.border = new RectOffset(11, 11, 11, 11);
            return style;
        }

        public override string Category
        {
            get
            {
                return "";
            }
        }

        public override void OnNodeGUI(Rect position, NodeData data)
        {
            base.OnNodeGUI(position, data);
            if (or_texture == null)
            {
                or_texture = GraphUtil.IconContents.LoadTexture("or");
            }
            var iconRect = new Rect(position.x + position.width - 30, position.y, 30, 30);
            GUI.backgroundColor = Color.clear;
            if (GUI.Button(iconRect, or_texture))
            {

            }
            GUI.backgroundColor = Color.white;
            //EditorGUI.DrawTextureTransparent(iconRect, or_texture, ScaleMode.ScaleToFit);
        }
    }

}