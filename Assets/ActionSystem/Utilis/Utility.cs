using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.Sprites;
using UnityEngine.Scripting;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.Assertions.Must;
using UnityEngine.Assertions.Comparers;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{

    public static class Utility
    {
        internal static void RetriveCommandCtrl(this Transform trans,ref CommandController ctrl)
        {
            if (ctrl == null){
                ctrl = trans.GetComponentInParent<ActionSystem>().CommandCtrl;
            }
        }
        internal static void RetriveElementCtrl(this Transform trans, ref ElementController ctrl)
        {
            if (ctrl == null){
                ctrl = trans.GetComponentInParent<ActionSystem>().ElementCtrl;
            }
        }
        internal static void RetriveConfig(this Transform trans, ref Config config)
        {
            if (config == null)
            {
                config = trans.GetComponentInParent<ActionSystem>().Config;
            }
        }
        internal static void RetriveEventCtrl(this Transform trans, ref EventController ctrl)
        {
            if (ctrl == null)
            {
                ctrl = trans.GetComponentInParent<ActionSystem>().EventCtrl;
            }
        }
        public static void RetriveCommand(Transform trans, List<ActionCommand> list)
        {
            var coms = trans.GetComponents<ActionCommand>();
            if (coms != null && coms.Length > 0)
            {
                foreach (var com in coms)
                {
                    if (!list.Contains(com)) list.Add(com);
                }
                return;
            }
            else
            {
                foreach (Transform child in trans)
                {
                    RetriveCommand(child, list);
                }
            }

        }

        public static void RetivePickElement(Transform trans, UnityAction<PickUpAbleElement> onRetive)
        {
            if (!trans.gameObject.activeSelf) return;
            var com = trans.GetComponent<PickUpAbleElement>();
            if (com)
            {
                onRetive(com);
                return;
            }
            else
            {
                foreach (Transform child in trans)
                {
                    RetivePickElement(child, onRetive);
                }
            }

        }

    }
}
