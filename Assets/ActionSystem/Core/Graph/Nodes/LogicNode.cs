using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using NodeGraph;
using NodeGraph.DataModel;

namespace WorldActionSystem.Graph
{
    [CustomNode("Logic", 0, "ActionSystem")]
    public class LogicNode : NodeGraph.DataModel.Node
    {
        [SerializeField]
        private Point[] _inPoints;
        [SerializeField]
        private Point[] _outPoints;

        protected override IEnumerable<Point> inPoints
        {
            get
            {
                if (_inPoints == null)
                {
                    _inPoints = new Point[1];
                    _inPoints[0] = new Point("-", "", 100);
                }
                return _inPoints;
            }
        }
        protected override IEnumerable<Point> outPoints
{
            get
            {
                if (_outPoints == null)
                {
                    _outPoints = new Point[1];
                    _outPoints[0] = new Point("+", "", 1);
                }
                return _outPoints;
            }
        }
    }
}