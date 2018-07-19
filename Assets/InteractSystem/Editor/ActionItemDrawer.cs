using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
namespace InteractSystem.Drawer
{

    [CustomEditor(typeof(ActionItem), true)]
    public class ActionItemDrawer : Editor
    {
        protected SerializedProperty bindings_Prop;
        protected SerializedProperty script_Prop;
        protected ReorderListDrawer bindingList = new ActionItemBindingListDrawer("1.功能绑定（继承OperateBinding）");
        protected Dictionary<int, List<SerializedProperty>> propDic = new Dictionary<int, List<SerializedProperty>>();

        private GUIContent[] _options;
        protected GUIContent[] options
        {
            get
            {
                if (_options == null)
                {
                    _options = new GUIContent[] {
                        new GUIContent("基本信息"),
                        new GUIContent("配制详情")
                    };
                }
                return _options;
            }
        }
        protected Prefer.EditorPrefsInt prefer_selected = new Prefer.EditorPrefsInt("actionitemdrawer_selected");
        protected int selected
        {
            get
            {
                return prefer_selected.value;
            }
            set
            {
                prefer_selected.value = value;
            }
        }

        protected virtual void OnEnable()
        {
            bindings_Prop = serializedObject.FindProperty("bindings");
            script_Prop = serializedObject.FindProperty("m_Script");
            bindingList.InitReorderList(bindings_Prop);
            CollectProperty();
        }

        public override void OnInspectorGUI()
        {
            DrawScript();
            DrawSwitch();
            serializedObject.Update();
            OnDrawPropertys();
            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void DrawScript()
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(script_Prop);
            EditorGUI.EndDisabledGroup();
        }

        protected virtual void DrawSwitch()
        {
            EditorGUI.BeginChangeCheck();
            selected = GUILayout.Toolbar(selected, options);
            if(EditorGUI.EndChangeCheck())
            {
                CollectProperty();
            }
        }
        protected virtual void CollectProperty()
        {
            var prop = serializedObject.GetIterator();
            var index = 0;
            var enterChildern = true;

            foreach (var item in propDic)
            {
                item.Value.Clear();
            }

            while (prop.NextVisible(enterChildern))
            {
                enterChildern = false;
                var property = serializedObject.FindProperty(prop.propertyPath);

                if (propDic.ContainsKey(index))
                {
                    propDic[index].Add(property);
                }
                else
                {
                    propDic[index] = new List<SerializedProperty>() { property };
                }

                if (prop.propertyPath == "bindings")
                {
                    index = 1;
                }
            }
        }

        protected virtual void OnDrawPropertys()
        {
            var props = propDic[selected];
            for (int i = 0; i < props.Count; i++)
            {
                OnDrawProperty(props[i]);
            }
        }

        protected virtual void OnDrawProperty(SerializedProperty property)
        {
            if(property.propertyPath == "m_Script")
            {
                return;
            }
            if (property.propertyPath == "bindings")
            {
                bindingList.DoLayoutList();
            }
            else
            {
                EditorGUILayout.PropertyField(property, true);
            }
        }
    }

}