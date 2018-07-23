using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using InteractSystem.Graph;

namespace InteractSystem.Actions
{
    public static class ConnectUtil
    {
        public static bool TryConnect(ConnectItem itemA, ConnectItem itemB, ConnectNode.PointGroup pointInfo)
        {
            if (itemA.OperateAble && itemB.OperateAble)
            {
                itemA.OnConnectTo(itemB);
                itemB.OnConnectTo(itemA);
                Transform parent = itemA.GetInstanceID() > itemB.GetInstanceID() ? itemA.transform : itemB.transform;
                string targetName = itemA.GetInstanceID() > itemB.GetInstanceID() ? itemB.Name : itemA.name;
                var target = parent.Find(targetName);
                var lineRender = target == null ? null : target.GetComponent<LineRenderer>();
                if (lineRender == null)
                {
                    lineRender = new GameObject(targetName, typeof(LineRenderer)).GetComponent<LineRenderer>();
                    lineRender.transform.SetParent(parent);
                    UpdateLineStyle(lineRender, pointInfo.width, pointInfo.material);
                    lineRender.positionCount = 2;
                    var posA = itemA.RetriveFeature<ClickAbleFeature>().collider.transform.position;
                    var posB = itemB.RetriveFeature<ClickAbleFeature>().collider.transform.position;
                    lineRender.SetPositions(new Vector3[] { posA, posB });
                }
                //
                return true;
            }
            else
            {
                return false;
            }
        }



        public static void UpdateLineStyle(LineRenderer line, float lineWight, Material lineMaterial)
        {
#if UNITY_5_6_OR_NEWER
            line.textureMode = LineTextureMode.Tile;
            line.startWidth = lineWight;
            line.endWidth = lineWight * 0.8f;
#else
            line.SetVertexCount(1);
            line.SetWidth(lineWight, lineWight * 0.8f);
#endif
            line.material = lineMaterial;
        }

        public static bool HaveConnected(ConnectItem itemA, ConnectItem itemB)
        {
            return Array.Find(itemA.Connected, x => x == itemB) &&
                Array.Find(itemB.Connected, x => x == itemA);
        }

        public static bool TryDisconnect(ConnectItem itemA, ConnectItem itemB)
        {
            if (itemA != null && itemB != null)
            {
                itemA.OnDisConnectTo(itemB);
                itemB.OnDisConnectTo(itemA);

                Transform parent = itemA.GetInstanceID() > itemB.GetInstanceID() ? itemA.transform : itemB.transform;
                string targetName = itemA.GetInstanceID() > itemB.GetInstanceID() ? itemB.Name : itemA.name;
                var target = parent.Find(targetName);
                var lineRender = target == null ? null : target.GetComponent<LineRenderer>();
                if (lineRender != null){
                    lineRender.positionCount = 0;
                }

                return true;
            }
            return false;
        }
    }
}