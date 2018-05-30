using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeGraph;
using NodeGraph.DataModel;

namespace WorldActionSystem.Graph
{
    [CustomNodeView(typeof(OperateNode))]
    public class OperateNodeView : ActionNodeView
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
            activeStyle.normal.background = GraphUtil.BackgroundContent.LoadTexture("on_node4") as Texture2D;
            activeStyle.border = new RectOffset(11, 11, 11, 19);
            return activeStyle;
        }
        protected override GUIStyle CreateInactiveStyle()
        {
            var inactiveStyle = new GUIStyle();
            inactiveStyle.normal.background = GraphUtil.BackgroundContent.LoadTexture("node4") as Texture2D;
            inactiveStyle.border = new RectOffset(11, 11, 11, 19);
            return inactiveStyle;
        }
        public OperateNode node { get { return target as OperateNode; } }

        public override void OnNodeGUI(Rect position, NodeData data)
        {
            base.OnNodeGUI(position, data);
            DrawAddNodes(position, data);
        }
        public override void OnInspectorGUI(NodeGUI gui)
        {
            base.OnInspectorGUI(gui);

            if (target is OperateNode)
            {
                var node = target as OperateNode;
                if (node.Statu == ExecuteStatu.Executing)
                {
                    gui.ShowProgress();
                    gui.SetProgress(Time.time % 1f);
                }
                else
                {
                    gui.HideProgress();
                }
            }
        }
    }
}