using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using NodeGraph;
using NodeGraph.DataModel;
using UnityEditor;

namespace WorldActionSystem.Graph
{
    [CustomNodeView(typeof(LogicNode))]
    public class LogicNodeView : NodeView
    {
        public override Node target
        {
            get
            {
                return base.target;
            }

            set
            {
                base.target = value;
            }
        }
        private Texture or_texture;
        private GUIStyle _activeStyle;
        private GUIStyle _inactiveStyle;
        public override float SuperHeight
        {
            get
            {
                return base.SuperHeight;
            }
        }
        public override float SuperWidth
        {
            get
            {
                return -40;
            }
        }
        public override GUIStyle ActiveStyle
        {
            get
            {
                if (_activeStyle == null)
                {

                    _activeStyle = new GUIStyle();
                    _activeStyle.normal.background = TextureContent.LoadTexture("operate") as Texture2D;
                    _activeStyle.border = new RectOffset(11, 11, 11, 11);
                }
                return _activeStyle;
            }
        }
        public override GUIStyle InactiveStyle
        {
            get
            {
                if (_inactiveStyle == null)
                {

                    _inactiveStyle = new GUIStyle();
                    _inactiveStyle.normal.background = TextureContent.LoadTexture("operate") as Texture2D;
                    _inactiveStyle.border = new RectOffset(11, 11, 11, 11);
                }
                return _inactiveStyle;
            }
        }
        public override string Category
        {
            get
            {
                return "";
            }
        }

        public override void OnNodeGUI(Rect position, NodeData data)
        {
            if (or_texture== null)
            {
                or_texture = TextureContent.LoadTexture("or");
                Debug.Log(or_texture);
            }
            var iconRect = new Rect(position.center - Vector2.one * 20, Vector2.one * 40);
            GUI.backgroundColor = Color.clear;
            GUI.Button(iconRect, or_texture, "AnimationKeyframeBackground");
            GUI.backgroundColor = Color.white;
            //EditorGUI.DrawTextureTransparent(iconRect, or_texture, ScaleMode.ScaleToFit);
        }
    }

}