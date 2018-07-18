using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

namespace InteractSystem
{
    public class CoroutineController:MonoBehaviour
    {
        private Coroutine delyCoroutine;
        private Dictionary<UnityAction, List<float>> delyActions = new Dictionary<UnityAction, List<float>>();
        //private Queue<UnityAction> mainThreadActions = new Queue<UnityAction>();
        private event UnityAction frameActions;
        private static bool log = false;
        private static CoroutineController _instence;
        public static CoroutineController Instence
        {
            get
            {
                if (_instence == null)
                {
                    _instence = ActionSystem.Instence.gameObject.AddComponent<CoroutineController>();
                }
                return _instence;
            }
        }

        private void Update()
        {
            if (frameActions != null)
            {
                frameActions.Invoke();
            }
            if (delyActions != null && delyActions.Count > 0)
            {
                DelyActions();
            }
        }
        public void RegistFrameAction(UnityAction action)
        {
            frameActions -= action;
            frameActions += action;
        }
        public void RemoveFrameAction(UnityAction action)
        {
            frameActions -= action;
        }
        public void DelyExecute(UnityAction action, float time)
        {
            if (log) Debug.Log("DelyExecute" + action + ":" + time);

            if (delyActions.ContainsKey(action))
            {
                delyActions[action].Add(time);
            }
            else
            {
                delyActions[action] = new List<float>() { time };
            }
        }

        public void Cansalce(UnityAction action)
        {
            if (delyActions.ContainsKey(action))
            {
                delyActions.Remove(action);
            }

            if (delyCoroutine != null && delyActions.Count == 0)
            {
                Debug.Log("Cansalce:" + delyCoroutine);
                StopCoroutine(delyCoroutine);
                delyCoroutine = null;
            }
        }

        private void DelyActions()
        {
            var keys = delyActions.Keys.Select(x => x).ToArray();

            foreach (var action in keys)
            {
                var timers = delyActions[action];
                for (int i = 0; i < timers.Count; i++)
                {
                    if ((timers[i] -= Time.deltaTime) < 0)
                    {
                        timers.RemoveAt(i);
                        action.Invoke();
                        break;
                    }
                }


                if (timers.Count == 0)
                {
                    delyActions.Remove(action);
                    if (log) Debug.Log("Remove:" + action);
                }
            }
        }

    }
}