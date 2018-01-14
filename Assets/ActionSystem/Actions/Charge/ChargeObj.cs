using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace WorldActionSystem
{
    /// <summary>
    /// (暂时没有考虑不足和溢出的问题)
    /// </summary>
    public class ChargeObj : ActionObj
    {
        [SerializeField]
        private ChargeData[] startDatas;
        [SerializeField]
        private List<ChargeData> _completeDatas;

        public List<ChargeData> completeDatas { get { return _completeDatas; } }
        //[SerializeField]
        //private List<string> chargeTools;
        //[SerializeField]
        //private List<string> chargeResources;
        private List<ChargeData> currentList = new List<ChargeData>();
        public ChargeEvent onCharge;
        public override ControllerType CtrlType
        {
            get
            {
                return ControllerType.Charge;
            }
        }
        private static List<ChargeObj> lockQueue = new List<ChargeObj>();
        protected override void Start()
        {
            base.Start();
            InitStartData();
            InitLayer();
        }

        protected virtual void OnDestroy()
        {
            if (lockQueue.Contains(this)){
                lockQueue.Remove(this);
            }
        }
        public override void OnStartExecute(bool auto = false)
        {
            base.OnStartExecute(auto);
            ActiveElements(this);
            if(auto)
            {
                AutoComplete();
            }
        }
        protected override void OnBeforeEnd(bool force)
        {
            base.OnBeforeEnd(force);
            CompleteElements(this, false);
        }
        public override void OnUnDoExecute()
        {
            base.OnUnDoExecute();
            CompleteElements(this, false);
        }

        public virtual bool Charge(ChargeData data)
        {
            if (completeDatas.FindAll(x => x.type == data.type).Count > 0)
            {
                if (onCharge != null)
                    onCharge.Invoke(data);
                currentList.Add(data);
                JudgeComplete();
                return true;
            }
            else
            {
                return false;
            }
        }

        private void JudgeComplete()
        {
            foreach (var item in completeDatas)
            {
                var current = currentList.Find(x => x.type == item.type);
                if(string.IsNullOrEmpty(current.type) || current.value < item.value)
                {
                    return;
                }
            }

            OnEndExecute(false);
        }

        private void AutoComplete()
        {
            //找到一个tool和resourcee
            //让tool去吸然后来填
        }

        private void InitLayer()
        {
            GetComponentInChildren<Collider>().gameObject.layer = LayerMask.NameToLayer(Layers.chargeObjLayer);
        }
        private void ActiveElements(ChargeObj element)
        {
            var actived = lockQueue.Find(x => x.Name == element.Name);

            if (actived == null)
            {
                var tools = ElementController.Instence.GetElements<ChargeTool>();
                if (tools != null)
                {
                    for (int i = 0; i < tools.Count; i++)
                    {
                        if (tools[i].supportTypes.Find(x => completeDatas.FindAll(y => y.type == x).Count > 0) == null) continue;

                        if (log) Debug.Log("ActiveElements:" + element.Name + (!tools[i].Started));

                        if (!tools[i].Started)
                        {
                            tools[i].StepActive();
                        }
                    }
                }

                var resources = ElementController.Instence.GetElements<ChargeResource>();
                if(resources != null)
                {
                    for (int i = 0; i < resources.Count; i++)
                    {
                        if (completeDatas.FindAll(y => y.type == resources[i].type).Count == 0) continue;

                        if (log) Debug.Log("ActiveElements:" + element.Name + (!resources[i].Started));

                        if (!resources[i].Started)
                        {
                            resources[i].StepActive();
                        }
                    }
                }

            }
            lockQueue.Add(element);
        }

        private void CompleteElements(ChargeObj element, bool undo)
        {
            lockQueue.Remove(element);
            var active = lockQueue.Find(x => x.Name == element.Name);
            if (active == null)
            {
                var tools = ElementController.Instence.GetElements<ChargeTool>();
                if (tools != null)
                {
                    for (int i = 0; i < tools.Count; i++)
                    {
                        if (log) Debug.Log("CompleteElements:" + element.Name + tools[i].Started);

                        if (tools[i].supportTypes.Find(x => completeDatas.FindAll(y => y.type == x).Count > 0) == null) continue;

                        if (tools[i].Started)
                        {
                            if (undo)
                            {
                                tools[i].StepUnDo();
                            }
                            else
                            {
                                tools[i].StepComplete();
                            }
                        }
                    }
                }

                var resources = ElementController.Instence.GetElements<ChargeResource>();
                if (resources != null)
                {
                    for (int i = 0; i < resources.Count; i++)
                    {
                        if (log) Debug.Log("CompleteElements:" + element.Name + resources[i].Started);

                        if (completeDatas.FindAll(y => y.type == resources[i].type).Count == 0) continue;

                        if (resources[i].Started)
                        {
                            if (undo)
                            {
                                resources[i].StepUnDo();
                            }
                            else
                            {
                                resources[i].StepComplete();
                            }
                        }
                    }
                }
            }


        }

        private void InitStartData()
        {
            foreach (var item in startDatas)
            {
                Charge(item);
            }
        }
    }
}