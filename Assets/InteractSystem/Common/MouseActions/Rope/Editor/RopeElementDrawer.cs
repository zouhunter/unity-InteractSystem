using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using InteractSystem.Actions;
using UnityEditorInternal;
using System;

namespace InteractSystem.Drawer
{
    [CustomEditor(typeof(RopeElement))]
    public class RopeElementDrawer : ActionItemDrawer
    {
        [InitializeOnLoadMethod]
        static void ImportLayer()
        {
            LayerTool.ImportLayers(RopeElement.ropeItemLayer);
        }

        private SerializedProperty ropeNodeFrom_prop;
        private ObjectListDrawer<Collider> ropeNodeFromList = new ObjectListDrawer<Collider>("连接点（按名称匹配）");

        protected override void OnEnable()
        {
            base.OnEnable();
            InitropeNodeFrom();
        }

        private void InitropeNodeFrom()
        {
            ropeNodeFrom_prop = serializedObject.FindProperty("ropeNodeFrom");
            ropeNodeFromList.InitReorderList(ropeNodeFrom_prop);
        }

        protected override void OnDrawProperty(SerializedProperty property)
        {
            if (property.propertyPath == "ropeNodeFrom")
            {
                ropeNodeFromList.DoLayoutList();
            }
            else
            {
                base.OnDrawProperty(property);
            }
        }
    }
}