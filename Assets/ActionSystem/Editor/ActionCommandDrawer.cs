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
        SerializedProperty hitDistenceProp;
        SerializedProperty pointDistenceProp;

        SerializedProperty onBeforeActiveProp;
        SerializedProperty onBeforeUnDoProp;
        SerializedProperty onBeforePlayEndProp;

        bool drawLineInfo;
        bool drawHitDistence;
        List<ControllerType> activeCommands = new List<ControllerType>();

        private void OnEnable()
        {
            commandTypeProp = serializedObject.FindProperty("commandType");
            lineWightProp = serializedObject.FindProperty("lineWight");
            lineMaterialProp = serializedObject.FindProperty("lineMaterial");
            hitDistenceProp = serializedObject.FindProperty("hitDistence");
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
                EditorGUILayout.PropertyField(hitDistenceProp);
                EditorGUILayout.PropertyField(pointDistenceProp);
            }
            if (drawHitDistence)
            {
                EditorGUILayout.PropertyField(pointDistenceProp);
            }

        }
        private void SwitchDrawing()
        {
            drawLineInfo = false;
            drawHitDistence = false;
            activeCommands.Clear();
            var type = (ControllerType)commandTypeProp.intValue;
            if ((type & ControllerType.Click) == ControllerType.Click)
            {
                drawHitDistence = true;
                activeCommands.Add(ControllerType.Click);
            }
            if ((type & ControllerType.Connect) == ControllerType.Connect)
            {
                drawHitDistence = true;
                drawLineInfo = true;
                activeCommands.Add(ControllerType.Connect);
            }
            if ((type & ControllerType.Install) == ControllerType.Install)
            {
                drawHitDistence = true;
                activeCommands.Add(ControllerType.Install);
            }
            if ((type & ControllerType.Match) == ControllerType.Match)
            {
                drawHitDistence = true;
                activeCommands.Add(ControllerType.Match);
            }
            if ((type & ControllerType.Rotate) == ControllerType.Rotate)
            {
                drawHitDistence = true;
                activeCommands.Add(ControllerType.Rotate);
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