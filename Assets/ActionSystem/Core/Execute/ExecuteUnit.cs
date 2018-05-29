using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace WorldActionSystem
{
    /// <summary>
    /// 执行单元，记录节点之间的关系
    /// </summary>
    public class ExecuteUnit
    {
        public Graph.ActionNode node;
        public List<ExecuteUnit> parentUnits = new List<ExecuteUnit>();
        public List<List<ExecuteUnit>> childUnits = new List<List<ExecuteUnit>>();
        
        /// <summary>
        /// 便于检索
        /// </summary>
        private List<Graph.ActionNode> childNodesList = new List<Graph.ActionNode>();

        public ExecuteUnit(Graph.ActionNode parentNode)
        {
            this.node = parentNode;
        }

        public void AddParentUnit(ExecuteUnit parentUnit)
        {
            if (!parentUnits.Contains(parentUnit))
            {
                parentUnits.Add(parentUnit);
            }
        }

        public void AppendChildNodes(List<ExecuteUnit> childUnits)
        {
            var list = new List<ExecuteUnit>();
            list.AddRange(childUnits);
            this.childUnits.Add(list);
            this.childNodesList.AddRange(list.Select(x=>x.node));
            foreach (var unit in childUnits){
                unit.AddParentUnit(this);
            }
        }

        public bool HaveChildNode(Graph.ActionNode node)
        {
            if (childUnits == null) return false;
            return childNodesList.Contains(node);
        }

        internal object GetPositon(ExecuteUnit unit)
        {
           return childUnits.Find(x => x.Contains(unit));
        }
    }
}