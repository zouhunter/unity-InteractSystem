using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using NodeGraph;
using NodeGraph.DataModel;

namespace WorldActionSystem.Graph
{
    public class AcionGraphCtrl : NodeGraphController
    {
        public override string Group
        {
            get
            {
                return "ActionSystem";
            }
        }

        protected override void JudgeNodeExceptions(NodeGraphObj m_targetGraph, List<NodeException> m_nodeExceptions)
        {
            base.JudgeNodeExceptions(m_targetGraph, m_nodeExceptions);
        }
        internal override string GetConnectType(ConnectionPointData output, ConnectionPointData input)
        {
            return base.GetConnectType(output, input);
        }
        protected override void BuildFromGraph(NodeGraphObj m_targetGraph)
        {
            base.BuildFromGraph(m_targetGraph);
        }
        internal override void Validate(NodeGUI node)
        {
            base.Validate(node);
        }
    }

}
