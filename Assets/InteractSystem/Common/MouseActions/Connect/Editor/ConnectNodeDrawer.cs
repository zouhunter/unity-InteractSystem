using UnityEditor;

using UnityEditorInternal;

using UnityEngine;
namespace InteractSystem.Drawer
{
    [CustomEditor(typeof(Actions.ConnectNode))]
    public class ConnectNodeDrawer : OperateNodeDrawer
    {
        public SerializedProperty connectGroup_prop;
        public ReorderableList connectGroup_List;
        protected override void OnEnable()
        {
            base.OnEnable();
            InitDefultConnectList();
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
                    if (iterator.propertyPath == "connectGroup")
                    {
                        connectGroup_List.DoLayoutList();
                    }
                    else
                    {
                        EditorGUILayout.PropertyField(iterator, true);
                    }
                }
                enterChildern = false;
            }
        }
        private void InitDefultConnectList()
        {
            connectGroup_prop = serializedObject.FindProperty("connectGroup");
            connectGroup_List = new ReorderableList(serializedObject, connectGroup_prop);
            connectGroup_List.drawHeaderCallback = (rect) => { EditorGUI.LabelField(rect, "连接方案"); };
            connectGroup_List.drawElementCallback = DrawConnectGroupItem;
            connectGroup_List.elementHeight = EditorGUIUtility.singleLineHeight + ActionGUIUtil.padding * 4;
        }

        private void DrawConnectGroupItem(Rect rect, int index, bool isActive, bool isFocused)
        {
            var prop = connectGroup_prop.GetArrayElementAtIndex(index);
            var p1_prop = prop.FindPropertyRelative("p1");
            var p2_prop = prop.FindPropertyRelative("p2");
            var material_prop = prop.FindPropertyRelative("material");
            var width_prop = prop.FindPropertyRelative("width");

            rect = ActionGUIUtil.DrawBoxRect(rect, index.ToString());
            var leftRect = new Rect(rect.x, rect.y, rect.width * 0.4f, rect.height);
            var lineRect = new Rect(rect.x + rect.width * 0.2f, rect.y + ActionGUIUtil.padding, rect.width * 3f, rect.height);
            leftRect = ActionGUIUtil.PaddingRect(leftRect, 2);
            EditorGUI.LabelField(lineRect, "------------------------");

            var titleRect = new Rect(leftRect.x, leftRect.y, leftRect.width * 0.3f,  EditorGUIUtility.singleLineHeight);
            var contentRect = new Rect(leftRect.x + leftRect.width * 0.4f, leftRect.y, leftRect.width * 0.6f,  EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(titleRect, "A");
            p1_prop.stringValue = EditorGUI.TextField(contentRect, p1_prop.stringValue);

            titleRect.y += EditorGUIUtility.singleLineHeight ;// new Rect(rightRect.x, rightRect.y, rightRect.width * 0.3f,  EditorGUIUtility.singleLineHeight);
            contentRect.y += EditorGUIUtility.singleLineHeight;//= new Rect(rightRect.x + rightRect.width * 0.4f, rightRect.y, rightRect.width * 0.6f,  EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(titleRect, "B");
            p2_prop.stringValue = EditorGUI.TextField(contentRect, p2_prop.stringValue);


            var rightRect = new Rect(rect.x + rect.width * 0.5f, rect.y, rect.width * 0.5f, EditorGUIUtility.singleLineHeight);
            titleRect = new Rect(rightRect.x, rightRect.y, rightRect.width * 0.3f, rightRect.height);
            contentRect = new Rect(rightRect.x + rightRect.width * 0.4f, rightRect.y, rightRect.width * 0.6f, EditorGUIUtility.singleLineHeight);

            EditorGUI.LabelField(titleRect, "material");
            material_prop.objectReferenceValue = EditorGUI.ObjectField(contentRect, material_prop.objectReferenceValue, typeof(Material), false);

            titleRect.y += EditorGUIUtility.singleLineHeight;
            contentRect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.LabelField(titleRect, "width");
            width_prop.floatValue = EditorGUI.FloatField(contentRect, width_prop.floatValue);
        }

    }
}
