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
using System.Linq;
namespace InteractSystem
{
    public static class Utility
    {
        public const string commandParentName = "Triggers";
        public const string pickupParentName = "Elements";

        internal static void SurchSystem(this Transform trans, ref ActionGroup system)
        {
            if (system == null)
            {
                system = trans.GetComponentInParent<ActionGroup>();
            }
        }

        public static void RetiveBehaiver<T>(Transform trans, UnityAction<T> onRetive) where T : MonoBehaviour
        {
            if (!trans.gameObject.activeSelf) return;
            var com = trans.GetComponent<T>();
            if (com)
            {
                onRetive(com);
                return;
            }
            else
            {
                foreach (Transform child in trans)
                {
                    RetiveBehaiver(child, onRetive);
                }
            }

        }

        public static Transform GetParent(Transform transform, bool containsCommand, bool containsPickup)
        {
            Transform parent = transform;
            if (containsCommand)
            {
                var commandParent = transform == null ? null : transform.Find(commandParentName);
                if (commandParent == null)
                {
                    commandParent = new GameObject(commandParentName).transform;
                    commandParent.SetParent(transform, false);
                }
                parent = commandParent;
            }
            else if (containsPickup)
            {
                var pickupParent = transform == null ? null : transform.Find(pickupParentName);
                if (pickupParent == null)
                {
                    pickupParent = new GameObject(pickupParentName).transform;
                    pickupParent.SetParent(transform, false);
                }
                parent = pickupParent;
            }
            return parent;
        }

        public static List<System.Type> GetSubInstenceTypes(System.Type baseType)
        {
            return (from type in typeof(ActionGroup).Assembly.GetTypes()
                    where type.IsSubclassOf(baseType)
                    where !type.IsAbstract
                    select type).ToList();
        }

        public static GameObject CreateRunTimeObject(GameObject prefab, Transform parent)
        {
            prefab.gameObject.SetActive(true);
            var created = Object.Instantiate(prefab);
            created.name = prefab.name;
            created.transform.SetParent(parent, false);
            return created;
        }

    }
}
