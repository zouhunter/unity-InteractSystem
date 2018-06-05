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
        public static bool log = false;
        private static List<ExecuteUnit> orignalUnits = new List<ExecuteUnit>();
        private static List<KeyValuePair<ExecuteUnit, int>> copyDic = new List<KeyValuePair<ExecuteUnit, int>>();
        private static Dictionary<ExecuteUnit, List<ExecuteUnit>> enviroment = new Dictionary<ExecuteUnit, List<ExecuteUnit>>();

        public static ExecuteUnit AnalysisGraph(ActionCommand graphObj)
        {
            if (log) Debug.Log("AnalysisGraph...");
            return DeepCopy(CreateOriginalUnit(graphObj));
        }
        /// <summary>
        /// 将copyDic中的unit进行反向复制
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        private static ExecuteUnit DeepCopy(ExecuteUnit unit)
        {
            copyDic.Reverse();
            foreach (var item in copyDic)
            {
                var count = item.Value;
                var copyOriginal = item.Key;
                enviroment[copyOriginal] = new List<ExecuteUnit>();
                for (int z = 0; z < count; z++)
                {
                    var newUnit = MakeCopy(copyOriginal, enviroment[copyOriginal]);
                    for (int i = 0; i < copyOriginal.parentUnits.Count; i++)
                    {
                        var position = copyOriginal.parentUnits[i].GetPositon(copyOriginal);
                        position.Add(newUnit);
                        newUnit.parentUnits.Add(copyOriginal.parentUnits[i]);
                    }
                }
            }
            return unit;
        }
        public static ExecuteUnit MakeCopy(ExecuteUnit original, List<ExecuteUnit> enviroment)
        {
            var node = ScriptableObject.Instantiate(original.node) as Graph.ActionNode;
            (node as Graph.ActionNode).SetContext(original.node.Context);//设置上下文
            var unit = CreateOringalUnit(node, enviroment);
            if(log) Debug.Log("MakeCopy:"+ unit);

            for (int i = 0; i < original.childUnits.Count; i++)
            {
                var childList = new List<ExecuteUnit>();
                for (int j = 0; j < original.childUnits[i].Count; j++)
                {
                    var childUnit = MakeCopy(original.childUnits[i][j], enviroment);
                    childList.Add(childUnit);
                }
                unit.AppendChildNodes(childList);
            }
            return unit;
        }

        private static ExecuteUnit CreateOringalUnit(Graph.ActionNode node, List<ExecuteUnit> copyEnviroment)
        {
            var unit = copyEnviroment.Find(x => x.node == node);
            if (unit == null)
            {
                unit = new ExecuteUnit(node);
                copyEnviroment.Add(unit);
            }
            return unit;
        }


        //private static ExecuteUnit CreateCopyExecuteUnit(ExecuteUnit unit, int count)
        //{
        //    var copyUnit = new ExecuteUnit(ScriptableObject.Instantiate(unit.node));
        //    for (int i = 0; i < unit.parentUnits.Count; i++)
        //    {
        //        var parent = unit.parentUnits[i];
        //        var orignalList = parent.GetPositon(unit);
        //        orignalList.Add(copyUnit);
        //        copyUnit.AddParentUnit(unit.parentUnits[i]);
        //    }
        //    return copyUnit;
        //}

        private static ExecuteUnit CreateOriginalUnit(ActionCommand graphObj)
        {
            if (graphObj == null || graphObj.Nodes == null) return null;
            NodeData startNodeData = graphObj.Nodes.Where(node => node.Object is Graph.StartNode).FirstOrDefault();
            if (startNodeData == null) return null;
            Debug.Assert(startNodeData != null, "this is no start node!");
            orignalUnits.Clear();
            copyDic.Clear();
            var executeUnit = new ExecuteUnit(startNodeData.Object as Graph.StartNode);
            RetiveChildNode(graphObj, startNodeData, executeUnit);
            return executeUnit;
        }

        private static void RetiveChildNode(ActionCommand graphObj, NodeData parentNode, ExecuteUnit unit)
        {
            //按连接点分组
            var connectionGroup = graphObj.Connections.Where(connection => connection.FromNodeId == parentNode.Id).GroupBy(
                x =>
                {
                    var port = parentNode.OutputPoints.Find(y => y.Id == x.FromNodeConnectionPointId);
                    var id = parentNode.OutputPoints.IndexOf(port);
                    return id;
                }
            ).OrderBy(x => x.Key).ToArray();

            foreach (var connections in connectionGroup)
            {
                if (log)
                    Debug.Log("connection:" + connections.Key);
                var childNodes = new List<ExecuteUnit>();
                foreach (var connection in connections)
                {
                    var copyCount = (connection.Object as Graph.ActionConnection).copyCount;
                    var node = graphObj.Nodes.Find(x => x.Id == connection.ToNodeId);
                    (node.Object as Graph.ActionNode).SetContext(graphObj);//设置上下文
                    var childUnit = CreateOringalUnit(node.Object as Graph.ActionNode, orignalUnits);
                    RetiveChildNode(graphObj, node, childUnit);
                    childNodes.Add(childUnit);

                    if (copyCount > 0)
                    {
                        copyDic.Add(new KeyValuePair<ExecuteUnit, int>(childUnit, copyCount));
                    }
                }
                unit.AppendChildNodes(childNodes);
            }
        }

    }


}