using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using HighlightingSystem;
namespace WorldActionSystem
{
    public class HightLightRegister : MonoBehaviour
    {

        [SerializeField]
        private string key;
        [SerializeField]
        private Color color = Color.green;
        [SerializeField]
        private List<GameObject> m_Objs;

        private string highLight { get { return "HighLightObjects"; } }
        private string unhighLight { get { return "UnHighLightObjects"; } }

        private List<Highlighter> highlighters = new List<Highlighter>();
        private void Start()
        {
            EventController.AddDelegate<string>(highLight, HighLightGameObjects);
            EventController.AddDelegate<string>(unhighLight, UnHighLightGameObjects);
            RegistItems();
        }

        private void OnDestroy()
        {
            EventController.RemoveDelegate<string>(highLight, HighLightGameObjects);
            EventController.RemoveDelegate<string>(unhighLight, UnHighLightGameObjects);
        }
        private void RegistItems()
        {
            foreach (var item in m_Objs)
            {
                var high = item.GetComponent<Highlighter>();
                if (high == null)
                {
                    high = item.AddComponent<Highlighter>();
                }

                if (!highlighters.Contains(high))
                {
                    highlighters.Add(high);
                    high.On();
                }
            }
        }

        public void HighLightGameObjects(string key)
        {
            if (this.key == key)
            {
                for (int i = 0; i < highlighters.Count; i++)
                {
                    var item = highlighters[i];
                    if (item)
                    {
                        item.FlashingOn(Color.white, color);
                    }
                }
            }

        }
        public void UnHighLightGameObjects(string key)
        {
            if (this.key == key)
            {
                for (int i = 0; i < highlighters.Count; i++)
                {
                    var item = highlighters[i];
                    if (item)
                    {
                        item.FlashingOff();
                        item.Off();
                    }
                }
            }
        }

    }
}