using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace WorldActionSystem
{
    [AddComponentMenu(MenuName.ActionSystem)]
    public class ActionSystem : MonoBehaviour
    {
        #region Instence
        private static bool isQuit = false;
        private static ActionSystem _instence;
        internal static ActionSystem Instence
        {
            get
            {
                if (_instence == null && !isQuit)
                {
                    _instence = new GameObject("ActionSystem").AddComponent<ActionSystem>();
                }
                return _instence;
            }
        }
        private void OnApplicationQuit()
        {
            isQuit = true;
        }
        #endregion

        private static List<ActionGroup> actionGroup = new List<ActionGroup>();

        public static void RegistGroup(ActionGroup group)
        {
            if(!actionGroup.Contains(group))
            {
                group.transform.SetParent(Instence.transform);
                actionGroup.Add(group);
            }
        }

        public static void RemoveGroup(ActionGroup group)
        {
            if(!actionGroup.Contains(group))
            {
                actionGroup.Clear();
            }

            if(actionGroup.Count == 0)
            {
                Clean();
            }
        }

        private static void Clean()
        {
            if (_instence != null)
            {
                Destroy(_instence.gameObject);
            }
        }
    }

}