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

            SwitchDrawing();
            Warning();
        }



        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (Selection.gameObjects.Length == 1)
            {
                serializedObject.Update();
                DrawOptionDatas();
                DrawEvents();
                serializedObject.ApplyModifiedProperties();
            }
        }

        private void DrawOptionDatas()
        {
            SwitchDrawing();

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

        private void Warning()
        {
            var cmd = target as ActionCommand;
            var actionObjs = cmd.GetComponentsInChildren<ActionObj>(true);
            string err = "";
            foreach (var item in actionObjs)
            {
                if (item is ClickObj && !activeCommands.Contains(ControllerType.Click))
                {
                    err = cmd.name + " have no ctrl of " + ControllerType.Click;
                }
                else if (item is RotObj && !activeCommands.Contains(ControllerType.Rotate))
                {
                    err = cmd.name + " have no ctrl of " + ControllerType.Rotate;
                }
                else if (item is InstallObj && !activeCommands.Contains(ControllerType.Install))
                {
                    err = cmd.name + " have no ctrl of " + ControllerType.Install;
                }
                else if (item is MatchObj && !activeCommands.Contains(ControllerType.Match))
                {
                    err = cmd.name + " have no ctrl of " + ControllerType.Match;
                }
                else if (item is ConnectObj && !activeCommands.Contains(ControllerType.Connect))
                {
                    err = cmd.name + " have no ctrl of " + ControllerType.Connect;
                }
            }

            if (!String.IsNullOrEmpty(err))
            {
                Debug.LogError(err, target);
            }
        }
        private void DrawEvents()
        {
            EditorGUILayout.PropertyField(onBeforeActiveProp);
            EditorGUILayout.PropertyField(onBeforeUnDoProp);
            EditorGUILayout.PropertyField(onBeforePlayEndProp);
        }
    }
}