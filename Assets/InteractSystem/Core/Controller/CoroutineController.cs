using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

namespace InteractSystem
{
    public class CoroutineController
    {
        private Coroutine delyCoroutine;
        private MonoBehaviour holder;
        private Dictionary<UnityAction, List<float>> delyActions = new Dictionary<UnityAction, List<float>>();
        private Queue<UnityAction> mainThreadActions = new Queue<UnityAction>();
        private static bool log = false;
        private static CoroutineController _instence;
        public static CoroutineController Instence
        {
            get
            {
                if (_instence == null)
                {
                    _instence = new CoroutineController(ActionSystem.Instence);
                }
                return _instence;
            }
        }
        public CoroutineController(MonoBehaviour holder)
        {
            this.holder = holder;
        }

        public void StartThread()
        {
            StartCoroutine(MainThread());
        }
        public void StopThread()
        {
            StopCoroutine(MainThread());
        }


        public void StartCoroutine(IEnumerator coroutine)
        {
            holder.StartCoroutine(coroutine);
        }

        public void StopCoroutine(IEnumerator coroutine)
        {
            holder.StopCoroutine(coroutine);
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

            if (delyCoroutine == null)
            {
                delyCoroutine = holder.StartCoroutine(DelyActionCoroutine());
            }
            else
            {
                Debug.Log("delyCoreoutine:" + "isRuning");
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
                holder.StopCoroutine(delyCoroutine);
                delyCoroutine = null;
            }
        }

        public void PushThreadActions(UnityAction action)
        {
            mainThreadActions.Enqueue(action);
        }

        private IEnumerator MainThread()
        {
            while (holder != null)
            {
                yield return new WaitForEndOfFrame();
                if (mainThreadActions.Count > 0)
                {
                    var action = mainThreadActions.Dequeue();
                    try
                    {
                        action.Invoke();
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }
                }
            }
        }

        private IEnumerator DelyActionCoroutine()
        {
            var waitHandle = new WaitForEndOfFrame();

            while (delyActions.Count > 0)
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
                yield return waitHandle;
            }
            delyCoroutine = null;
        }

    }
}