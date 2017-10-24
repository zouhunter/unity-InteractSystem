using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using HighlightingSystem;

namespace WorldActionSystem {

    public class AngleCtroller : MonoBehaviour
    {
        public static AngleCtroller Instance;
        [SerializeField]
        protected GameObject viewObj;
        [SerializeField]
        protected Color highLightColor = Color.green;

        private Queue<GameObject> objectQueue = new Queue<GameObject>();
        private Dictionary<Transform, GameObject> actived = new Dictionary<Transform, GameObject>();
        private Dictionary<GameObject, Highlighter> highLightDic = new Dictionary<GameObject, Highlighter>();

        private void Awake()
        {
            Instance = this;
            viewObj.SetActive(false);
        }

        public void UnNotice(Transform target)
        {
            if (!Setting.highLightNotice) return;

            if (actived.ContainsKey(target))
            {
                HideAnAngle(actived[target]);
                actived.Remove(target);
            }
        }

        public void Notice(Transform target)
        {
            if (!Setting.highLightNotice) return;

            if (!actived.ContainsKey(target))
            {
                actived.Add(target, GetAnAngle(target));
            }
        }

        private GameObject GetAnAngle(Transform target)
        {
            GameObject angle = null;
            if (objectQueue.Count > 0)
            {
                angle = objectQueue.Dequeue();
            }
            else
            {
                angle = Instantiate(viewObj);
                angle.transform.SetParent(transform);
                Highlighter high = null;
                if (!highLightDic.ContainsKey(angle))
                {
                    high = angle.GetComponent<Highlighter>();
                    if (high == null){
                        high = angle.AddComponent<Highlighter>();
                    }
                    highLightDic.Add(angle, high);
                }
                high.On();
            }
            angle.SetActive(true);
            HighLighter(angle);
            CopyTranform(angle.transform, target);
            return angle;
        }

        public static void CopyTranform(Transform obj, Transform target)
        {
            obj.transform.position = target.transform.position;
            obj.transform.rotation = target.transform.rotation;
        }

        private void HideAnAngle(GameObject angle)
        {
            HighLighter(angle);
            angle.gameObject.SetActive(false);
            objectQueue.Enqueue(angle);
        }

        private void HighLighter(GameObject angle)
        {
            highLightDic[angle].FlashingOn(Color.white, highLightColor);
        }
        private void UnHighLighter(GameObject angle)
        {
            highLightDic[angle].FlashingOff();
        }
    }
}