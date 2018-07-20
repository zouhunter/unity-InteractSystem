using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using NodeGraph;
using NodeGraph.DataModel;
using UnityEditor;
using System;

namespace InteractSystem.Graph
{
    public class AcionGraphCtrl : NodeGraphController<ActionCommand>
    {
        public override string Group
        {
            get
            {
                return "InteractSystem";
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
        internal override void DrawNodeGUI(NodeGUI nodeGUI)
        {
            base.DrawNodeGUI(nodeGUI);

            if (nodeGUI.Data.Object is InteractSystem.Graph.OperaterNode)
            {
                var node = nodeGUI.Data.Object as InteractSystem.Graph.OperaterNode;
                if (node.Statu == ExecuteStatu.Executing)
                {
                    nodeGUI.ShowProgress();
                    nodeGUI.SetProgress((Time.time / 10) % 1);
                }
                else
                {
                    nodeGUI.HideProgress();
                }
            }

        }
        public override void SaveGraph(List<NodeData> nodes, List<ConnectionData> connections, bool resetAll = false)
        {
            //base.SaveGraph(nodes, connections, resetAll);
            UnityEngine.Assertions.Assert.IsNotNull(this);
            TargetGraph.ApplyGraph(nodes, connections);
            NodeGraphObj obj = TargetGraph;
            var allAssets = AllNeededAssets();
            SetSubAssets(allAssets, obj);
            UnityEditor.EditorUtility.SetDirty(obj);
        }

        private ScriptableObject[] AllNeededAssets()
        {
            var list = new List<ScriptableObject>();
            list.Add(TargetGraph);
            list.AddRange(TargetGraph.Nodes.ConvertAll(x => x.Object as ScriptableObject));
            list.AddRange(TargetGraph.Connections.ConvertAll(x => x.Object as ScriptableObject));
            return list.ToArray();
        }

        public static void SetSubAssets(ScriptableObject[] subAssets, ScriptableObject mainAsset)
        {
            var path = AssetDatabase.GetAssetPath(mainAsset);
            var oldAssets = AssetDatabase.LoadAllAssetsAtPath(path);

            foreach (ScriptableObject subAsset in subAssets)
            {
                if (subAsset == mainAsset) continue;

                if (System.Array.Find(oldAssets, x => x == subAsset) == null)
                {
                    if(subAsset is Graph.OperaterNode)
                    {
                        ScriptableObjUtility.AddSubAsset(subAsset, mainAsset, HideFlags.None);
                    }
                    else
                    {
                        ScriptableObjUtility.AddSubAsset(subAsset, mainAsset, HideFlags.HideInHierarchy);
                    }
                }
            }

            ScriptableObjUtility.ClearSubAssets(mainAsset, subAssets);
        }
    }

}
