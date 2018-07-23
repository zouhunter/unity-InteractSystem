using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using InteractSystem.Actions;
using UnityEditorInternal;
using System;

namespace InteractSystem.Drawer
{
    [CustomEditor(typeof(DetachItem))]
    public class DetachItemDrawer : ActionItemDrawer
    {
        [InitializeOnLoadMethod]
        static void ImportLayer()
        {
            LayerTool.ImportLayers(DetachItem.layer);
        }

        private SerializedProperty rule_prop;

        private Type[] _supportRules;
        private Type[] supportRules
        {
            get
            {
                if(_supportRules == null)
                {
                    _supportRules = Utility.GetSubInstenceTypes(typeof(DetachRule)).ToArray();
                }
                return _supportRules;
            }
        }
        protected override void OnEnable()
        {
            base.OnEnable();
            rule_prop = serializedObject.FindProperty("rule");
        }
        protected override void OnDrawProperty(SerializedProperty property)
        {
            if(property.propertyPath == "rule")
            {
                using (var hor = new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.PropertyField(rule_prop,new GUIContent("规   则"));
                    if(GUILayout.Button("new",EditorStyles.miniButtonRight,GUILayout.Width(40)))
                    {
                        var options = Array.ConvertAll(supportRules, x => new GUIContent(x.FullName));
                        var rect = new Rect(Event.current.mousePosition, Vector2.zero);
                        EditorUtility.DisplayCustomMenu(rect, options, -1, (x, y, id) => {
                            var type = supportRules[id];
                            var instence = ScriptableObject.CreateInstance(type);
                            ProjectWindowUtil.CreateAsset(instence, string.Format("new {0}.asset", type.Name));

                            ActionGUIUtil.DelyAcceptObject(instence, (prefab) =>
                            {
                                rule_prop = new SerializedObject(target).FindProperty("rule");
                                rule_prop.objectReferenceValue = prefab;
                                rule_prop.serializedObject.ApplyModifiedProperties();
                            });
                        }, null);
                    }
                }
            }
            else
            {
                base.OnDrawProperty(property);
            }
        }
    }

}