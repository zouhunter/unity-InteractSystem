using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{
    public class ChargeBinding : MonoBehaviour
    {
        [SerializeField]
        protected float animTime = 2;

        protected float timer = 0;
        protected bool asyncActive = false;
        protected UnityAction onComplete;

        protected virtual void Update()
        {
            if (asyncActive && timer < animTime)
            {
                timer += Time.deltaTime;
                if (timer > animTime)
                {
                    CompleteAsync();
                }
            }
        }
        protected virtual void StartAsync(UnityAction onComplete)
        {
            this.onComplete = onComplete;
            asyncActive = true;
            timer = 0;
        }

        protected virtual void CompleteAsync()
        {
            asyncActive = false;
            if (onComplete != null){
                var action = onComplete;
                onComplete = null;
                action.Invoke();
            }
        }
    }
}