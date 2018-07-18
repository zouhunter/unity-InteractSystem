using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using InteractSystem.Actions;
using UnityEditorInternal;
using System;

namespace InteractSystem.Drawer
{
    [CustomEditor(typeof(RopeItem))]
    public class RopeItemDrawer : ActionItemDrawer
    {
        [InitializeOnLoadMethod]
        static void ImportLayer()
        {
            LayerTool.ImportLayers(RopeItem.layer);
        }

        private SerializedProperty ropeNodeTo_prop;
        private ObjectListDrawer<Collider> ropeNodeToList = new ObjectListDrawer<Collider>("连接点（按名称匹配）");

        protected override void OnEnable()
        {
            base.OnEnable();
            InitRopeNodeTo();
        }

        private void InitRopeNodeTo()
        {
            ropeNodeTo_prop = serializedObject.FindProperty("ropeNodeTo");
            ropeNodeToList.InitReorderList(ropeNodeTo_prop);
        }

        protected override void OnDrawProperty(SerializedProperty property)
        {
            if(property.propertyPath == "ropeNodeTo")
            {
                ropeNodeToList.DoLayoutList();
            }
            else
            {
                base.OnDrawProperty(property);
            }
        }
    }

}