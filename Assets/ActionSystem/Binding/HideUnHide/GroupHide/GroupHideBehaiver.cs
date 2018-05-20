using UnityEngine;
using System.Collections.Generic;

namespace WorldActionSystem.Binding
{
    public class GroupHideBehaiver : MonoBehaviour
    {
        [SerializeField]
        private GameObject[] views;
        [SerializeField]
        private string key;
        private bool active;

        private void Awake()
        {
            active = true;
        }

        private void Update()
        {
            if (GroupHide.Contains(key))
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
            if (this.active != active)
            {
                this.active = active;
                foreach (var item in views)
                {
                    item.SetActive(active);
                }
            }
        }
    }

}