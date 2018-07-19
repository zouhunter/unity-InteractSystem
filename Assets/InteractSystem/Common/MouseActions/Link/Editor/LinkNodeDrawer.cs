using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;

namespace InteractSystem.Drawer
{
    [CustomEditor(typeof(Actions.LinkNode))]
    public class LinkNodeDrawer : OperateNodeDrawer
    {
        public SerializedProperty defultLink_prop;
        public ReorderableList defultLink_List;
        protected override void OnEnable()
        {
            base.OnEnable();
            InitDefultLinkList();
        }
        
        protected override void OnDrawDefult()
        {
            //base.OnDrawDefult();
            var iterator = serializedObject.GetIterator();
            var enterChildern = true;
            while (iterator.NextVisible(enterChildern))
            {
                if (!ignoredPaths.Contains(iterator.propertyPath))
                {
                    if(iterator.propertyPath == "defultLink")
                    {
                        defultLink_List.DoLayoutList();
                    }
                    else
                    {
                        EditorGUILayout.PropertyField(iterator, true);
                    }
                }
                enterChildern = false;
            }
        }
        private void InitDefultLinkList()
        {
            defultLink_prop = serializedObject.FindProperty("defultLink");
            defultLink_List = new ReorderableList(serializedObject, defultLink_prop);
            defultLink_List.drawHeaderCallback = (rect) => { EditorGUI.LabelField(rect, "连接方案"); };
            defultLink_List.drawElementCallback = DrawLinkGroupItem;
            defultLink_List.elementHeight = EditorGUIUtility.singleLineHeight + ActionGUIUtil.padding * 4;
        }

        private void DrawLinkGroupItem(Rect rect, int index, bool isActive, bool isFocused)
        {
            var prop = defultLink_prop.GetArrayElementAtIndex(index);
            var itemA_prop = prop.FindPropertyRelative("ItemA");
            var portA_prop = prop.FindPropertyRelative("portA");
            var itemB_prop = prop.FindPropertyRelative("ItemB");
            var portB_prop = prop.FindPropertyRelative("portB");

            rect = ActionGUIUtil.DrawBoxRect(rect, index.ToString());
            var leftRect = new Rect(rect.x, rect.y, rect.width * 0.5f, rect.height);
            var rightRect = new Rect(rect.x + rect.width * 0.5f, rect.y, rect.width * 0.5f, rect.height);
            var lineRect = new Rect(rect.x + rect.width * 0.45f, rect.y + ActionGUIUtil.padding, rect.width * 0.1f, rect.height);
            rightRect = ActionGUIUtil.DrawBoxRect(rightRect,"");
            leftRect = ActionGUIUtil.DrawBoxRect(leftRect,"");
            EditorGUI.LabelField(lineRect, "------------------------");

            var titleRect = new Rect(leftRect.x, leftRect.y, leftRect.width * 0.2f, leftRect.height);
            var contentRect = new Rect(leftRect.x + leftRect.width * 0.2f, leftRect.y, leftRect.width * 0.3f, leftRect.height);
            EditorGUI.LabelField(titleRect, "ID");
            itemA_prop.intValue = EditorGUI.IntField(contentRect, itemA_prop.intValue);
            titleRect.x += leftRect.width * 0.5f;
            contentRect.x += leftRect.width * 0.5f;

            EditorGUI.LabelField(titleRect, "Port");
            portA_prop.intValue = EditorGUI.IntField(contentRect, portA_prop.intValue);

            titleRect = new Rect(rightRect.x, rightRect.y, rightRect.width * 0.2f, rightRect.height);
            contentRect = new Rect(rightRect.x + rightRect.width * 0.2f, rightRect.y, rightRect.width * 0.3f, rightRect.height);
            EditorGUI.LabelField(titleRect, "ID");
            itemB_prop.intValue = EditorGUI.IntField(contentRect, itemB_prop.intValue);

            titleRect.x += rightRect.width * 0.5f;
            contentRect.x += rightRect.width * 0.5f;
            EditorGUI.LabelField(titleRect,"Port");
            portB_prop.intValue = EditorGUI.IntField(contentRect, portB_prop.intValue);
        }

    }

}