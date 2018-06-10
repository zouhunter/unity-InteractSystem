using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using System.Linq;

namespace InteractSystem.Drawer
{
    public class EnviromentItemListDrawer : ReorderListDrawer
    {
        protected List<GameObject> dragedGameObject = new List<GameObject>();
        protected override void DrawElementCallBack(Rect rect, int index, bool isActive, bool isFocused)
        {
            rect = ActionGUIUtil.DrawBoxRect(rect, index.ToString());
            var prop = property.GetArrayElementAtIndex(index);
            EditorGUI.PropertyField(rect, prop, true);

            var objRect = new Rect(rect.x + rect.width - ActionGUIUtil.bigButtonWidth, rect.y, ActionGUIUtil.bigButtonWidth, rect.height);

            if (Event.current.type == EventType.DragUpdated && objRect.Contains(Event.current.mousePosition))
            {
                ActionGUIUtil.UpdateDragedObjects(".prefab", dragedGameObject);
            }
            else if (Event.current.type == EventType.DragPerform && objRect.Contains(Event.current.mousePosition))
            {
                foreach (var item in dragedGameObject)
                {
                    if (!AddOneElement(item))
                    {
                        EditorUtility.DisplayDialog("警告", "预制体重复,无法添加:" + item.name, "ok");
                    }
                }
            }
        }

        private bool AddOneElement(GameObject item)
        {
            if (!property.HaveElement("prefab", item))
            {
                var eprop = property.AddItem();
                var prefabProp = eprop.FindPropertyRelative("prefab");
                prefabProp.objectReferenceValue = item;
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override void DrawHeaderCallBack(Rect rect)
        {
            base.DrawHeaderCallBack(rect);
            var btnRect = new Rect(rect.x + rect.width - ActionGUIUtil.bigButtonWidth, rect.y, ActionGUIUtil.bigButtonWidth, rect.height);
            if (GUI.Button(btnRect, "analysis"))
            {
                AnalysisFromNodes();
            }
        }
        protected override float ElementHeightCallback(int index)
        {
            var prop = property.GetArrayElementAtIndex(index);
            return EditorGUI.GetPropertyHeight(prop, null, true) + ActionGUIUtil.padding * 2;
        }
        public override void DoLayoutList()
        {
            base.DoLayoutList();
            var rect = ActionGUIUtil.GetDragRect();
            if (Event.current.type == EventType.dragUpdated && rect.Contains(Event.current.mousePosition))
            {
                ActionGUIUtil.UpdateDragedObjects(".prefab", dragedGameObject);
            }
            else if (Event.current.type == EventType.DragPerform && rect.Contains(Event.current.mousePosition))
            {
                foreach (var item in dragedGameObject)
                {
                    if (!ActionEditorUtility.HaveElement(property, "prefab", item))
                    {
                        var prop = property.AddItem();
                        var prefabProp = prop.FindPropertyRelative("prefab");
                        var _name_prop = prop.FindPropertyRelative("_name");
                        _name_prop.stringValue = item.name;
                        prefabProp.objectReferenceValue = item;
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("警告", "预制体重复,无法添加:" + item.name, "ok");
                    }
                }
            }
        }

        protected void AnalysisFromNodes()
        {
            if(ActionGroupDrawer.Current != null && ActionGroupDrawer.Current.target != null)
            {
                AnalysisActionGroup(ActionGroupDrawer.Current.target as ActionGroup);
            }
            else if (Selection.assetGUIDs == null || Selection.assetGUIDs.Length == 0)
            {
                NoChoiseShow();
            }
            else
            {
                var commands = from guid in Selection.assetGUIDs
                               let path = AssetDatabase.GUIDToAssetPath(guid)
                               let obj = AssetDatabase.LoadAssetAtPath<ActionCommand>(path)
                               where obj != null
                               select obj;
                AnalysisAcionCommand(commands);
            }
        }

        protected void AnalysisAcionCommand(IEnumerable<ActionCommand> commands)
        {
            if (commands.Count() == 0)
            {
                NoChoiseShow();
            }
            else
            {
                foreach (var command in commands)
                {
                    var flag = System.Reflection.BindingFlags.GetField |
                        System.Reflection.BindingFlags.Instance |
                        System.Reflection.BindingFlags.NonPublic;
                    var enviroments = typeof(ActionCommand).GetField("environments", flag).GetValue(command) as Enviroment.EnviromentInfo[];
                    AnalysisEnviroments(enviroments);

                    var operateNodes = from node in command.Nodes
                                       where node.Object is Graph.OperaterNode
                                       select node.Object as Graph.OperaterNode;

                    foreach (var node in operateNodes)
                    {
                        enviroments = typeof(Graph.OperaterNode).GetField("environments", flag).GetValue(node) as Enviroment.EnviromentInfo[];
                        AnalysisEnviroments(enviroments);
                    }
                }
            }
        }

        protected void AnalysisActionGroup(ActionGroup group)
        {
            var flag = System.Reflection.BindingFlags.GetField |
                           System.Reflection.BindingFlags.Instance |
                           System.Reflection.BindingFlags.NonPublic;
            var actionCommands = typeof(ActionGroup).GetField("actionCommands", flag).GetValue(group) as OptionalCommandItem[];
            var commands = from commandItem in actionCommands
                           where commandItem.command != null
                           select commandItem.command;
            AnalysisAcionCommand(commands);
        }

        protected void AnalysisEnviroments(Enviroment.EnviromentInfo[] enviroments) 
        {
            if (enviroments != null)
            {
                foreach (var enviroment in enviroments)
                {
                    var guid = enviroment.guid;
                    if (!string.IsNullOrEmpty(guid))
                    {
                        var path = AssetDatabase.GUIDToAssetPath(guid);
                        if (!string.IsNullOrEmpty(path))
                        {
                            var go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                            if (go != null)
                            {
                                AddOneElement(go);
                            }
                        }
                    }
                }
            }

        }

        protected void NoChoiseShow()
        {
            EditorUtility.DisplayDialog("温馨提示", "请选中ActionCommand后重试！", "OK");
        }
    }
}
