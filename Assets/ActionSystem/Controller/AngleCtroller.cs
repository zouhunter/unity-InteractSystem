using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using HighlightingSystem;

namespace WorldActionSystem {

    public class AngleCtroller : MonoBehaviour
    {
        [SerializeField]
        protected GameObject viewObj;
        [SerializeField]
        protected Color highLightColor = Color.green;

        private Queue<GameObject> objectQueue = new Queue<GameObject>();
        private Dictionary<Transform, GameObject> actived = new Dictionary<Transform, GameObject>();
        private Dictionary<GameObject, Highlighter> highLightDic = new Dictionary<GameObject, Highlighter>();
        private ActionSystem _system;
        private ActionSystem system { get { transform.SurchSystem(ref _system);return _system; } }
        private void Awake()
        {
            viewObj.SetActive(false);
        }

        public void UnNotice(Transform target)
        {
            if (!Config.highLightNotice) return;

            if (actived.ContainsKey(target))
            {
                HideAnAngle(actived[target]);
                actived.Remove(target);
            }
        }

        public void Notice(Transform target,bool update = false)
        {
            if (!Config.highLightNotice) return;

            if (!actived.ContainsKey(target))
            {
                actived.Add(target, GetAnAngle(target));
            }
            else
            {
                if(update)
                {
                    CopyTranform(actived[target].transform, target);
                }
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
                if (!highLightDic.TryGetValue(angle,out high))
                {
                    high = InitHighLighter(angle);
                    highLightDic.Add(angle, high);
                }
                high.On();
            }
            CopyTranform(angle.transform, target);
            angle.SetActive(true);
            HighLighter(angle);
            return angle;
        }

        private static Highlighter InitHighLighter(GameObject angle)
        {
            Highlighter high = angle.GetComponent<Highlighter>();
            if (high == null)
            {
                high = angle.AddComponent<Highlighter>();
            }
            high.SeeThroughOn();
            return high;
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
            highLightDic[angle].SeeThroughOn();
        }
        private void UnHighLighter(GameObject angle)
        {
            highLightDic[angle].FlashingOff();
        }
    }
}