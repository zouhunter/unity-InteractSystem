using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

namespace InteractSystem.Actions
{
    public class ChargeUtil : MonoBehaviour
    {
        /// <summary>
        /// 空间查找Resource
        /// </summary>
        /// <param name="item"></param>
        /// <param name="resourceItem"></param>
        public static bool FindResource(ChargeTool item, out ChargeResource resourceItem)
        {
            var resourceItems = SelectItems<ChargeResource>(ChargeResource.layer,CameraController.Instence.currentCamera.transform.position, item.transform.position, item.Range);
            if (resourceItems == null || resourceItems.Length == 0)
            {
                resourceItem = null;
                return false;
            }
            else
            {
                for (int i = 0; i < resourceItems.Length; i++)
                {
                    ChargeResource tempItem = resourceItems[i];
                    if(tempItem != null && tempItem.Active && item.CanLoad(tempItem.type) && tempItem.current > 0)
                    {
                        resourceItem = tempItem;
                        return true;
                    }
                }
                resourceItem = null;
                return false;
            }
        }
      
        public static bool FindChargeObj(ChargeTool item, out ChargeItem container)
        {
            var containerItems = SelectItems<ChargeItem>(ChargeItem.layer, CameraController.Instence.currentCamera.transform.position, item.transform.position, item.Range);
            if (containerItems == null || containerItems.Length == 0)
            {
                container = null;
                return false;
            }
            else
            {
                for (int i = 0; i < containerItems.Length; i++)
                {
                    ChargeItem tempItem = containerItems[i];
                    if (tempItem != null && tempItem.Active && tempItem.completeDatas.FindAll(x => x.type == item.data.type).Count > 0)
                    {
                        container = tempItem;
                        return true;
                    }
                }
                container = null;
                return false;
            }
        }
        private static T[] SelectItems<T>(string layer, Vector3 cameraPos, Vector3 worldCenter, float range) where T : MonoBehaviour
        {
            var dir = worldCenter - cameraPos;
            var quaternion = Quaternion.FromToRotation(Vector3.forward, dir.normalized);

            if (Input.GetKey(KeyCode.A))
            {
                var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.position = worldCenter;
                cube.transform.rotation = quaternion;
                var bs = new Vector3(range, range, 100);
                cube.transform.localScale = bs;
            }

            var boxSize = new Vector3(range, range, 100);
            var hits = Physics.BoxCastAll(worldCenter, boxSize * 0.5f, dir, quaternion, 0.01f, LayerMask.GetMask(layer));

            var items = from hit in hits
                        let port = hit.collider.gameObject.GetComponentInParent<T>()
                        where port != null
                        select port;

            return items.ToArray();
        }

    }
}

