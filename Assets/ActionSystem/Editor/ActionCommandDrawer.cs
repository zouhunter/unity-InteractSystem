using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using Rotorz.ReorderableList;
using Rotorz.ReorderableList.Internal;

namespace WorldActionSystem
{
    [CustomEditor(typeof(ActionCommand)), CanEditMultipleObjects]
    public class ActionCommandDrawer : Editor
    {
        SerializedProperty commandTypeProp;

        SerializedProperty lineWightProp;
        SerializedProperty lineMaterialProp;
        SerializedProperty pointDistenceProp;

        SerializedProperty onBeforeActiveProp;
        SerializedProperty onBeforeUnDoProp;
        SerializedProperty onBeforePlayEndProp;

        bool drawLineInfo;
        bool drawPointDistence;
        List<ControllerType> activeCommands = new List<ControllerType>();

        private void OnEnable()
        {
            commandTypeProp = serializedObject.FindProperty("commandType");
            lineWightProp = serializedObject.FindProperty("lineWight");
            lineMaterialProp = serializedObject.FindProperty("lineMaterial");
            pointDistenceProp = serializedObject.FindProperty("pointDistence");

            onBeforeActiveProp = serializedObject.FindProperty("onBeforeActive");
            onBeforeUnDoProp = serializedObject.FindProperty("onBeforeUnDo");
            onBeforePlayEndProp = serializedObject.FindProperty("onBeforePlayEnd");
        }



        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (Selection.gameObjects.Length == 1)
            {
                serializedObject.Update();
                ModifyType();
                SwitchDrawing();
                DrawOptionDatas();
                DrawEvents();
                serializedObject.ApplyModifiedProperties();
            }
        }

        private void DrawOptionDatas()
        {
            EditorGUILayout.PropertyField(commandTypeProp);

            if (drawLineInfo)
            {
                EditorGUILayout.PropertyField(lineWightProp);
                EditorGUILayout.PropertyField(lineMaterialProp);
                EditorGUILayout.PropertyField(pointDistenceProp);
            }
            if (drawPointDistence)
            {
                EditorGUILayout.PropertyField(pointDistenceProp);
            }
        }
        private void SwitchDrawing()
        {
            drawLineInfo = false;
            drawPointDistence = false;
            activeCommands.Clear();
            var type = (ControllerType)commandTypeProp.intValue;
            if ((type & ControllerType.Click) == ControllerType.Click)
            {
                activeCommands.Add(ControllerType.Click);
            }
            if ((type & ControllerType.Connect) == ControllerType.Connect)
            {
                drawPointDistence = true;
                drawLineInfo = true;
                activeCommands.Add(ControllerType.Connect);
            }
            if ((type & ControllerType.Install) == ControllerType.Install)
            {
                activeCommands.Add(ControllerType.Install);
            }
            if ((type & ControllerType.Match) == ControllerType.Match)
            {
                activeCommands.Add(ControllerType.Match);
            }
            if ((type & ControllerType.Rotate) == ControllerType.Rotate)
            {
                activeCommands.Add(ControllerType.Rotate);
            }
            if((type & ControllerType.Rope) == ControllerType.Rope)
            {
                activeCommands.Add(ControllerType.Rope);
            }
        }

        private bool ModifyType()
        {
            var cmd = target as ActionCommand;
            var actionObjs = cmd.GetComponentsInChildren<ActionObj>(true);
            string err = "";
            if (actionObjs != null)
            {
                foreach (var item in actionObjs)
                {
                    if (item is ClickObj && !ContainsType(ControllerType.Click) )
                    {
                        err = cmd.name + " add ctrl of " + ControllerType.Click;
                        commandTypeProp.intValue |= (int)ControllerType.Click;
                    }
                    else if (item is RotObj && !ContainsType(ControllerType.Rotate))
                    {
                        err = cmd.name + " add ctrl of " + ControllerType.Rotate;
                        commandTypeProp.intValue |= (int)ControllerType.Rotate;
                    }
                    else if (item is InstallObj && !ContainsType(ControllerType.Install))
                    {
                        err = cmd.name + " add ctrl of " + ControllerType.Install;
                        commandTypeProp.intValue |= (int)ControllerType.Install;
                    }
                    else if (item is MatchObj && !ContainsType(ControllerType.Match))
                    {
                        err = cmd.name + " add ctrl of " + ControllerType.Match;
                        commandTypeProp.intValue |= (int)ControllerType.Match;
                    }
                    else if (item is ConnectObj && !ContainsType(ControllerType.Connect))
                    {
                        err = cmd.name + " add ctrl of " + ControllerType.Connect;
                        commandTypeProp.intValue |= (int)ControllerType.Connect;
                    }
                    else if (item is RopeObj && !ContainsType(ControllerType.Rope))
                    {
                        err = cmd.name + " add ctrl of " + ControllerType.Rope;
                        commandTypeProp.intValue |= (int)ControllerType.Rope;
                    }
                }
            }


            if (!String.IsNullOrEmpty(err))
            {
                Debug.Log(err, target);
                return true;
            }
            return false;
        }

        private bool ContainsType(ControllerType target)
        {
            return ((ControllerType)commandTypeProp.intValue & target) == target;
        }

        private void DrawEvents()
        {
            EditorGUILayout.PropertyField(onBeforeActiveProp);
            EditorGUILayout.PropertyField(onBeforeUnDoProp);
            EditorGUILayout.PropertyField(onBeforePlayEndProp);
        }
    }
}