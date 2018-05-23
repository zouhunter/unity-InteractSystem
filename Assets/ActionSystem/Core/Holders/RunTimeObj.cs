//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.Events;
//using System.Collections;
//using System.Collections.Generic;
//using System;

//namespace WorldActionSystem
//{
//    public abstract class RuntimeObj : ActionObj
//    {
//        protected override void Awake()
//        {
//            base.Awake();
//            elementCtrl.onRegistElememt += OnRegistElement;
//            elementCtrl.onRemoveElememt += OnRemoveElement;
//        }
//        protected override void OnDestroy()
//        {
//            base.OnDestroy();
//            elementCtrl.onRegistElememt -= OnRegistElement;
//            elementCtrl.onRemoveElememt -= OnRemoveElement;
//        }

//        protected abstract void OnRegistElement(ISupportElement arg0);
//        protected abstract void OnRemoveElement(ISupportElement arg0);
//    }
//    public abstract class RuntimeObj<T> : ActionObj where T : ActionItem, ISupportElement
//    {
//        protected ElementPool<T> elementPool = new ElementPool<T>();
//        public abstract List<string> NeedElements { get; }

//        protected override void Awake()
//        {
//            base.Awake();
//            elementPool.onAdd = OnAdd;
//            elementPool.onRemove = OnRemove;
//            elementCtrl.onRegistElememt += OnRegistElement;
//            elementCtrl.onRemoveElememt += OnRemoveElement;
//        }

//        protected virtual void OnRemove(T arg0)
//        {
            
//        }

//        protected virtual void OnAdd(T arg0)
//        {
//            if(Started && !Completed)
//            {
//                arg0.StepActive();
//            }
//        }

//        protected override void OnDestroy()
//        {
//            base.OnDestroy();
//            elementCtrl.onRegistElememt -= OnRegistElement;
//            elementCtrl.onRemoveElememt -= OnRemoveElement;
//        }
//        public override void OnStartExecute(bool auto = false)
//        {
//            base.OnStartExecute(auto);
//            UpdateElementPool();
//        }
//        public override void OnUnDoExecute()
//        {
//            base.OnUnDoExecute();
//            CompleteElements(true);
//        }
//        public override void OnEndExecute(bool force)
//        {
//            base.OnEndExecute(force);
//            CompleteElements(false);
//        }
//        /// <summary>
//        /// 从场景中找到已经存在的元素
//        /// </summary>
//        protected void UpdateElementPool()
//        {
//            List<string> elementKeys = new List<string>();
//            for (int i = 0; i < NeedElements.Count; i++)
//            {
//                var elementName = NeedElements[i];
//                if (!elementKeys.Contains(elementName))
//                {
//                    elementKeys.Add(elementName);
//                    var elements = elementCtrl.GetElements<T>(elementName);
//                    if (elements != null)
//                    {
//                        elementPool.ScureAdd(elements.ToArray());
//                    }
//                }
//            }
//        }

//        /// <summary>
//        /// 注册可点击元素
//        /// </summary>
//        /// <param name="arg0"></param>
//        protected void OnRegistElement(ISupportElement arg0)
//        {
//            //LinkObj
//            if (arg0 is T && NeedElements.Contains(arg0.Name))
//            {
//                var element = arg0 as T;
//                if (!elementPool.Contains(element))
//                {
//                    elementPool.ScureAdd(element);
//                }
//            }
//        }

//        /// <summary>
//        /// 移除可点击元素
//        /// </summary>
//        /// <param name="arg0"></param>
//        protected void OnRemoveElement(ISupportElement arg0)
//        {
//            if (arg0 is T && NeedElements.Contains(arg0.Name))
//            {
//                var element = arg0 as T;
//                if (elementPool.Contains(element))
//                {
//                    elementPool.ScureRemove(element);
//                }
//            }
//        }
//        protected void CompleteElements(bool undo)
//        {
//            foreach (var element in NeedElements)
//            {
//                var active = startedList.Find(x => x is RuntimeObj<T> && (x as RuntimeObj<T>).NeedElements.Contains(element));

//                if (active == null)
//                {
//                    var objs = elementPool.FindAll(x => x.Name == element);
//                    if (objs == null) return;
//                    for (int i = 0; i < objs.Count; i++)
//                    {
//                        if (log) Debug.Log("CompleteElements:" + element + objs[i].Active);

//                        if (objs[i].Active)
//                        {
//                            if (undo)
//                            {
//                                objs[i].StepUnDo();
//                            }
//                            else
//                            {
//                                objs[i].StepComplete();
//                            }
//                        }
//                    }
//                }
//            }

//        }



//    }
//}
