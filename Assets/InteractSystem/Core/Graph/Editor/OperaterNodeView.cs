using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeGraph;
using NodeGraph.DataModel;

namespace InteractSystem.Graph
{
    [CustomNodeView(typeof(OperaterNode))]
    public class OperaterNodeView : ActionNodeView
    {
        public override string Category
        {
            get
            {
                return target.GetType().Name;
            }
        }
        protected override GUIStyle CreateActiveStyle()
        {
            var activeStyle = new GUIStyle();
            activeStyle.normal.background = GraphUtil.BackgroundContent.LoadTexture("on_node4") as Texture2D;
            activeStyle.border = new RectOffset(11, 11, 11, 19);
            activeStyle.normal.textColor = Color.blue;
            return activeStyle;
        }
        protected override GUIStyle CreateInactiveStyle()
        {
            var inactiveStyle = new GUIStyle();
            inactiveStyle.normal.background = GraphUtil.BackgroundContent.LoadTexture("node4") as Texture2D;
            inactiveStyle.border = new RectOffset(11, 11, 11, 19);
            return inactiveStyle;
        }
        public OperaterNode node { get { return target as OperaterNode; } }

        public override void OnNodeGUI(Rect position, NodeData data)
        {
            base.OnNodeGUI(position, data);
            DrawAddNodes(position, data);
            data.Name = Get_Name();
        }

        private string Get_Name()
        {
            var field = typeof(OperaterNode).GetField("_name", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.GetField);
            if (field == null || node == null || !(node is OperaterNode)|| field.GetValue(node) == null)
            {
                return null;
            }
            return field.GetValue(node).ToString();
        }
    }
}