using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using NodeGraph;
using NodeGraph.DataModel;
using System.Linq;

namespace WorldActionSystem
{
    public class ExecuteGroup
    {
        public ExecuteUnit executeUnit { get; private set; }
        private NodeGraphObj graphObj { get; set; }

        public ExecuteGroup(NodeGraphObj graphObj)
        {
            this.graphObj = graphObj;
            AnalysisGraph(this.graphObj);
        }

        private void AnalysisGraph(NodeGraphObj graphObj)
        {
            Debug.Log("AnalysisGraph...");
            NodeData startNodeData = graphObj.Nodes.Where(node => node.Object is Graph.StartNode).First();
            Debug.Assert(startNodeData != null, "this is no start node!");
            executeUnit = new ExecuteUnit(startNodeData.Object as Graph.StartNode);
            RetiveChildNode(startNodeData, executeUnit);
        }

        private void RetiveChildNode(NodeGraph.DataModel.NodeData parentNode, ExecuteUnit unit)
        {
            var connectionGroup = graphObj.Connections.Where(connection => connection.FromNodeId == parentNode.Id).GroupBy(
                x =>
                {
                    var port = parentNode.OutputPoints.Find(y => y.Id == x.FromNodeConnectionPointId);
                    var id = parentNode.OutputPoints.IndexOf(port);
                    return id;
                }
            ).ToArray();

            foreach (var connections in connectionGroup)
            {
                var childNodes = new List<ExecuteUnit>();
                foreach (var connection in connections)
                {
                    var copyCount = (connection.Object as Graph.ActionConnection).copyCount;
                    var node = graphObj.Nodes.Find(x => x.Id == connection.ToNodeId);
                    
                    for (int i = -1; i < copyCount; i++)
                    {
                        var childUnit = CreateUnit(node.Object as Graph.ActionNode);
                        RetiveChildNode(node, childUnit);
                        childNodes.Add(childUnit);
                    }

                }
                unit.AppendChildNodes(childNodes);
            }
        }


        private ExecuteUnit CreateUnit(Graph.ActionNode origional)
        {
            if (origional == null) return null;
            else
            {
                var childUnit = new ExecuteUnit(Graph.ActionNode.Instantiate(origional));
                return childUnit;
            }
        }

    }


}