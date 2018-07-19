using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace InteractSystem.Drawer
{
    [CustomEditor(typeof(ElementGroup))]
    public class ElementGroupDrawer : Editor
    {
        protected SerializedProperty script_prop;
        protected SerializedProperty runtimeElements_prop;
        protected SerializedProperty autoElements_prop;
        protected SerializedProperty enviroments_prop;

        protected ReorderListDrawer autoElemnts_list = new AutoElementListDrawer();
        protected ReorderListDrawer runtimeElements_list = new RuntimeElementListDrawer();
        protected ReorderListDrawer enviroments_list = new EnviromentItemListDrawer();

        protected GUIContent[] _secondSelectables;
        protected GUIContent[] SecondSelectables
        {
            get
            {
                if (_secondSelectables == null || _secondSelectables.Length == 0)
                {
                    _secondSelectables = new GUIContent[] {
                        new GUIContent("静态生成元素"),
                        new GUIContent("动态创建元素"),
                        new GUIContent("环境切换元素"),
                    };
                }
                return _secondSelectables;
            }
        }
        protected int secondSelected
        {
            get { return _selected.value; }
            set { _selected.value = value; }
        }
        private static Prefer.EditorPrefsInt _selected = new Prefer.EditorPrefsInt("prefer_ElementGroupDrawer_selected");
        private static Prefer.EditorPrefsBool _showAll = new Prefer.EditorPrefsBool("prefer_ElementGroupDrawer_showAll");
        public bool showAll
        {
            get { return _showAll.value; }
            set { _showAll.value = value; }
        }
        public bool showScript = true;
        private void OnEnable()
        {
            if (target == null)
            { DestroyImmediate(this); return; }
            FindPropertys();
            InitReorderLists();
        }

        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();
            serializedObject.Update();
            if (showScript)
            {
                EditorGUILayout.PropertyField(script_prop);

                var content = showAll ? "-" : "+";
                var style = showAll ? EditorStyles.toolbarDropDown : EditorStyles.toolbarPopup;
                if (GUILayout.Button(content, style))
                {
                    showAll = !showAll;
                }
            }


            if (showAll)
            {
                autoElemnts_list.DoLayoutList();
                runtimeElements_list.DoLayoutList();
                enviroments_list.DoLayoutList();
            }
            else
            {
                if (secondSelected == 0)
                {
                    autoElemnts_list.DoLayoutList();
                }
                else if (secondSelected == 1)
                {
                    runtimeElements_list.DoLayoutList();
                }
                else
                {
                    enviroments_list.DoLayoutList();
                }
            }
            serializedObject.ApplyModifiedProperties();
        }

        private void FindPropertys()
        {
            script_prop = serializedObject.FindProperty("m_Script");
            runtimeElements_prop = serializedObject.FindProperty("runTimeElements");
            autoElements_prop = serializedObject.FindProperty("autoElements");
            enviroments_prop = serializedObject.FindProperty("enviroments");
        }

        private void InitReorderLists()
        {
            autoElemnts_list.InitReorderList(autoElements_prop);
            autoElemnts_list.drawHeaderCallback = (rect) => DrawHeadSwitch(rect, 0);

            runtimeElements_list.InitReorderList(runtimeElements_prop);
            runtimeElements_list.drawHeaderCallback = (rect) => DrawHeadSwitch(rect, 1);

            enviroments_list.InitReorderList(enviroments_prop);
            enviroments_list.drawHeaderCallback = (rect) => DrawHeadSwitch(rect, 2);
        }

        private void DrawHeadSwitch(Rect rect, int defult)
        {
            var headRect = new Rect(rect.x, rect.y, rect.width * 0.3f, rect.height);
            EditorGUI.BeginChangeCheck();
            if (showAll)
            {
                GUI.contentColor = ActionGUIUtil.NormalColor;
                EditorGUI.LabelField(headRect, SecondSelectables[defult]);
                GUI.contentColor = Color.white;
            }
            else
            {
                GUI.contentColor = ActionGUIUtil.WarningColor;
                secondSelected = EditorGUI.Popup(headRect, secondSelected, SecondSelectables, EditorStyles.miniLabel);
                GUI.contentColor = Color.white;
            }
        }
    }
}
