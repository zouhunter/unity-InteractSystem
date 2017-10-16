using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{
    public abstract class ActionHook : MonoBehaviour
    {
        protected bool _complete;
        public bool Complete { get { return _complete; } }
        protected bool _started;
        public bool Started { get { return _started; } }
        protected bool auto;
        [SerializeField]
        private int queueID;
        public int QueueID
        {
            get
            {
                return queueID;
            }
        }
        public UnityAction<int> onEndExecute;

        public virtual void OnStartExecute(bool auto = false)
        {
            this.auto = auto;
            if (!_started)
            {
                _started = true;
                _complete = false;
                gameObject.SetActive(true);
            }
            else
            {
                Debug.Log("already started", gameObject);
            }
        }

        public virtual void OnEndExecute()
        {
            if (!_complete)
            {
                _started = true;
                _complete = true;
                if (onEndExecute != null)
                {
                    onEndExecute.Invoke(queueID);
                }
            }
            else
            {
                Debug.Log("already completed", gameObject);
            }

        }

        public virtual void OnUnDoExecute()
        {
            _started = false;
            _complete = false;
        }
    }
}