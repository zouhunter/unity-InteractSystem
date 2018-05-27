using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeGraph;
using NodeGraph.DataModel;
using UnityEditor;

namespace WorldActionSystem.Graph
{
    [CustomNodeView(typeof(ActionConnection))]
    public class ActionConnectionView : ConnectionView
    {
        private static Color normalColor = Color.cyan;
        private ActionConnection connection { get { return target as ActionConnection; } }
        static Texture _angleTexture;
        static Texture angleTexture
        {
            get
            {
                if (_angleTexture == null)
                {
                    _angleTexture = GraphUtil.PortContent.LoadTexture("ArrowRight@32x");
                }
                return _angleTexture;
            }
        }
        internal override void OnConnectionGUI(Vector2 startPoistion, Vector2 endPosition, Vector2 startTan, Vector2 endTan)
        {
            base.OnConnectionGUI(startPoistion, endPosition, startTan, endTan);
            var points = Handles.MakeBezierPoints(startPoistion, endPosition, startTan, endTan, 10);
            ConnectionGUIUtility.HandleMaterial.SetPass(0);
            var point1 = points[9] + Quaternion.Euler(0, 0, 30) * ((points[8] - points[9]).normalized * 15);
            var point2 = points[9] + Quaternion.Euler(0, 0, -30) * ((points[8] - points[9]).normalized * 15);
            Handles.color = Color.green;
            Handles.DrawLine(point1, point2);
            Handles.DrawLine(endPosition, point2);
            Handles.DrawLine(point1, endPosition);
        }
        internal override Color LineColor
        {
            get
            {
                return normalColor;
            }
        }
        internal override void OnDrawLabel(Vector2 centerPos, string label)
        {
            //base.OnDrawLabel(centerPos, label);

        }
    }
}