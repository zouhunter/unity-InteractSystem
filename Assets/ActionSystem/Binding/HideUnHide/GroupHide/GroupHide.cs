using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace WorldActionSystem.Binding
{
    public class GroupHide : MonoBehaviour
    {
        [SerializeField]
        private GameObject[] views;
        [SerializeField]
        private string key;
        private bool active;
        public readonly static List<string> HideKeys = new List<string>();

        private void Awake()
        {
            active = true;
        }

        private void Update()
        {
            if (HideKeys.Contains(key))
            {
                ToggleHide(false);
            }
            else
            {
                ToggleHide(true);
            }
        }

        private void ToggleHide(bool active)
        {
            if(this.active != active)
            {
                this.active = active;
                foreach (var item in views)
                {
                    item.SetActive(active);
                }
            }
        }

        public void Record(string key)
        {
            if (string.IsNullOrEmpty(key)) return;
            if (!HideKeys.Contains(key))
            {
                HideKeys.Add(key);
            }
        }

        public void Remove(string key)
        {
            if (string.IsNullOrEmpty(key)) return;

            if (HideKeys.Contains(key))
            {
                HideKeys.Remove(key);
            }
        }

    }
}
