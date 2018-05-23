using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

namespace WorldActionSystem
{
    public class CoroutineController
    {
        private Coroutine delyCoroutine;
        private MonoBehaviour holder;
        private Dictionary<UnityAction, List<float>> delyActions = new Dictionary<UnityAction, List<float>>();
        private Queue<UnityAction> mainThreadActions = new Queue<UnityAction>();

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
            if (delyActions.ContainsKey(action))
            {
                delyActions[action].Add(time);
            }
            else
            {
                delyActions[action] = new List<float>();
            }

            if (delyCoroutine == null)
            {
                delyCoroutine = holder.StartCoroutine(DelyActionCoroutine());
            }
        }

        public void Cansalce(UnityAction action)
        {
            if (delyActions.ContainsKey(action))
            {
                delyActions.Remove(action);
            }

            if(delyCoroutine != null && delyActions.Count == 0)
            {
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
            var keys = delyActions.Keys.Select(x=>x).ToArray();
            var waitHandle = new WaitForEndOfFrame();
            while (keys.Length > 0)
            {
                yield return waitHandle;

                foreach (var action in keys)
                {
                    if (delyActions.ContainsKey(action))
                    {
                        var timers = delyActions[action];
                      
                        if (timers == null || timers.Count == 0)
                        {
                            delyActions.Remove(action);
                            break;
                        }
                        else
                        {
                            for (int i = 0; i < timers.Count; i++)
                            {
                                if ((timers[i] -= Time.deltaTime) < 0)
                                {
                                    timers.Remove(i);
                                    action.Invoke();
                                    break;
                                }
                            }
                        }
                    }
                }

                keys = delyActions.Keys.Select(x => x).ToArray();
            }
        }

    }
}