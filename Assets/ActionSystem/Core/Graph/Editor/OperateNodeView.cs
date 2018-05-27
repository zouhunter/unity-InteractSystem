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