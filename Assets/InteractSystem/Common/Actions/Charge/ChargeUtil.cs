using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

namespace InteractSystem.Common.Actions
{
    public class ChargeUtil : MonoBehaviour
    {
        /// <summary>
        /// 空间查找Resource
        /// </summary>
        /// <param name="item"></param>
        /// <param name="resourceItem"></param>
        /// <returns></returns>
        public static bool FindResource(ChargeTool item, out ChargeResource resourceItem)
        {
            Collider[] colliders = Physics.OverlapSphere(item.transform.position, item.Range, LayerMask.GetMask(Layers.chargeResourceLayer));
            if (colliders != null && colliders.Length > 0)
            {
                foreach (var collider in colliders)
                {
                    ChargeResource temp = collider.GetComponentInParent<ChargeResource>();
                    if(temp != null && item.CanLoad(temp.type))
                    {
                        resourceItem = temp;
                        return true;
                    }
                }
            }
            resourceItem = null;
            return false;
        }

        internal static bool FindChargeObj(ChargeTool item, out ChargeItem chargeObj)
        {
            Collider[] colliders = Physics.OverlapSphere(item.transform.position, item.Range, LayerMask.GetMask(Layers.chargeObjLayer));
            if (colliders != null && colliders.Length > 0)
            {
                foreach (var collider in colliders)
                {
                    ChargeItem temp = collider.GetComponentInParent<ChargeItem>();

                    if (temp != null&& temp.Active && temp.completeDatas.FindAll(x=>x.type == item.data.type).Count > 0)
                    {
                        chargeObj = temp;
                        return true;
                    }
                }
            }
            chargeObj = null;
            return false;
        }
    }
}

