using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using WorldActionSystem;
using HighlightingSystem;
namespace WorldActionSystem
{
    public class HideAndUnHideEventRegister : MonoBehaviour
    {
        [SerializeField]
        private string key;
        [SerializeField]
        private List<GameObject> m_Objs;

        private string hideKey { get { return "HideObjects"; } }
        private string showKey { get { return "UnHideObjects"; } }

        private void Start()
        {
            EventController.AddDelegate<string>(hideKey, HideGameObjects);
            EventController.AddDelegate<string>(showKey, UnHideGameObjects);
        }

        private void OnDestroy()
        {
            EventController.RemoveDelegate<string>(hideKey, HideGameObjects);
            EventController.RemoveDelegate<string>(showKey, UnHideGameObjects);
        }

        public void HideGameObjects(string key)
        {
            if (this.key == key)
            {
                for (int i = 0; i < m_Objs.Count; i++)
                {
                    m_Objs[i].SetActive(false);
                }
            }

        }
        public void UnHideGameObjects(string key)
        {
            if (this.key == key)
            {
                for (int i = 0; i < m_Objs.Count; i++)
                {
                    m_Objs[i].SetActive(true);
                }
            }
        }
    }
}