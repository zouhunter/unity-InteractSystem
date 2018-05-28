using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using NodeGraph;
using NodeGraph.DataModel;
using UnityEditor;
using System;
using System.Linq;

namespace WorldActionSystem.Graph
{

    [CustomNodeView(typeof(LogicNode))]
    public class LogicNodeView : ActionNodeView
    {
        private Texture or_texture;
        private Texture and_texture;
        private Texture exor_texture;
        private static GUIContent[] _optionContents;
        private static GUIContent[] optionContents
        {
            get
            {
                if (_optionContents == null)
                {
                    _optionContents = Enum.GetNames(typeof(LogicType)).Select(x => new GUIContent(x.ToString())).ToArray();
                }
                return _optionContents;
            }
        }
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
        public LogicNode logicNode
        {
            get
            {
                return target as LogicNode;
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
            var iconRect = new Rect(position.x, position.y, 30, 30);
            GUI.backgroundColor = Color.clear;
            if (GUI.Button(iconRect, SwitchTexture()))
            {
                EditorUtility.DisplayCustomMenu(new Rect(Event.current.mousePosition, Vector2.zero), optionContents, (int)logicNode.logicType, (x, y, index) =>
                   {
                       Undo.RecordObject(logicNode, "logicNode");
                       logicNode.logicType = (LogicType)index;
                       EditorUtility.SetDirty(logicNode);
                   }, null);
            }
            GUI.backgroundColor = Color.white;
        }

        private Texture SwitchTexture()
        {
            Texture current = null;
            if (logicNode.logicType == LogicType.And)
            {
                if (and_texture == null)
                {
                    and_texture = GraphUtil.IconContents.LoadTexture("And@32x");
                }
                current = and_texture;
            }
            else if (logicNode.logicType == LogicType.Or)
            {
                if (or_texture == null)
                {
                    or_texture = GraphUtil.IconContents.LoadTexture("Or@32x");
                }
                current = or_texture;
            }
            else if (logicNode.logicType == LogicType.ExclusiveOr)
            {
                if (exor_texture == null)
                {
                    exor_texture = GraphUtil.IconContents.LoadTexture("ExclusiveOr@32x");
                }
                current = exor_texture;
            }
            return current;
        }
    }

}