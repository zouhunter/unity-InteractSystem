using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using NodeGraph;
using NodeGraph.DataModel;
using System.Linq;

namespace WorldActionSystem.Structure
{
    public static class ExecuteUtil
    {
        public static bool log = true;
        private static Dictionary<Node, List<ExecuteUnit>> unitDic = new Dictionary<Node, List<ExecuteUnit>>();

        public static ExecuteUnit AnalysisGraph(NodeGraphObj graphObj)
        {
            if (log) Debug.Log("AnalysisGraph...");
            NodeData startNodeData = graphObj.Nodes.Where(node => node.Object is Graph.StartNode).First();
            Debug.Assert(startNodeData != null, "this is no start node!");
            unitDic.Clear();
            var executeUnit = CreateUnit(startNodeData.Object as Graph.StartNode);
            RetiveChildNode(graphObj, startNodeData, executeUnit);
            return executeUnit;
        }

        private static void RetiveChildNode(NodeGraphObj graphObj, NodeData parentNode, ExecuteUnit unit)
        {
            //按连接点分组
            var connectionGroup = graphObj.Connections.Where(connection => connection.FromNodeId == parentNode.Id).GroupBy(
                x =>
                {
                    var port = parentNode.OutputPoints.Find(y => y.Id == x.FromNodeConnectionPointId);
                    var id = parentNode.OutputPoints.IndexOf(port);
                    return id;
                }
            ).OrderBy(x=>x.Key).ToArray();

            foreach (var connections in connectionGroup)
            {
                if (log) Debug.Log("connection:" + connections.Key);
                var childNodes = new List<ExecuteUnit>();
                foreach (var connection in connections)
                {
                    var copyCount = (connection.Object as Graph.ActionConnection).copyCount;
                    var node = graphObj.Nodes.Find(x => x.Id == connection.ToNodeId);

                    for (int i = 0; i < copyCount + 1; i++)
                    {
                        var childUnit = CreateUnit(node.Object as Graph.ActionNode, i);
                        RetiveChildNode(graphObj, node, childUnit);
                        childNodes.Add(childUnit);
                    }

                }
                unit.AppendChildNodes(childNodes);
            }
        }
        private static ExecuteUnit CreateUnit(Graph.ActionNode origional, int index = 0)
        {
            if (origional == null) return null;
            else
            {
                if (!unitDic.ContainsKey(origional))
                {
                    unitDic[origional] = new List<ExecuteUnit>();
                }
                if (unitDic[origional].Count < index + 1)
                {
                    for (int i = unitDic[origional].Count; i < index + 1; i++)
                    {
                        unitDic[origional].Add(new ExecuteUnit(Graph.ActionNode.Instantiate(origional)));
                    }
                }

                return unitDic[origional][index];
            }
        }

    }


}