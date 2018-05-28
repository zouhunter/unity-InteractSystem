using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace WorldActionSystem
{
    /// <summary>
    /// 执行单元，记录节点之间的关系
    /// </summary>
    public class ExecuteUnit
    {
        public Graph.ActionNode node;
        public List<List<ExecuteUnit>> childUnits = new List<List<ExecuteUnit>>();
        
        /// <summary>
        /// 便于检索
        /// </summary>
        private List<Graph.ActionNode> childNodesList = new List<Graph.ActionNode>();

        public ExecuteUnit(Graph.ActionNode parentNode)
        {
            this.node = parentNode;
        }

        public void AppendChildNodes(List<ExecuteUnit> childNodes)
        {
            var list = new List<ExecuteUnit>();
            list.AddRange(childNodes);
            this.childUnits.Add(list);
            this.childNodesList.AddRange(list.Select(x=>x.node));
        }

        public bool HaveChildNode(Graph.ActionNode node)
        {
            if (childUnits == null) return false;
            return childNodesList.Contains(node);
        }
    }
}