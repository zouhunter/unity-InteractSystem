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
        [SerializeField]
        private float _capacity =1;
        public float capacity { get { return _capacity; } }

        public List<ChargeData> completeDatas { get { return _completeDatas; } }
        private List<ChargeData> _currentList = new List<ChargeData>();
        public ChargeEvent onCharge { get; set; }
        public List<ChargeData> currentList { get { return _currentList; } }
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
            if (lockQueue.Contains(this))
            {
                lockQueue.Remove(this);
            }
        }
        public override void OnStartExecute(bool auto = false)
        {
            base.OnStartExecute(auto);
            ActiveElements(this);
            if (auto)
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
            CompleteElements(this, true);
            var currentListArray = currentList.ToArray();
            foreach (var item in currentListArray)
            {
                var temp = item;
                temp.value = -item.value;
                Charge(temp, null);
            }
            currentList.Clear();
            InitStartData();
        }

        /// <summary>
        /// 返回一个比0大的数
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual void Charge(ChargeData data,UnityAction onComplete)
        {
            var complete = completeDatas.Find(x => x.type == data.type);
            if (!string.IsNullOrEmpty(complete.type))
            {
                if (onCharge != null)
                    onCharge.Invoke(transform.position,data, onComplete);
                else
                {
                    if (onComplete != null) onComplete.Invoke();
                }
                _currentList.Add(data);
            }
        }
        /// <summary>
        /// 判断并填充
        /// </summary>
        /// <param name="complete"></param>
        /// <param name="data"></param>
        /// <param name="onCharge"></param>
        /// <returns></returns>
        public ChargeData JudgeLeft(ChargeData data)
        {
            var complete = completeDatas.Find(x => x.type == data.type);
            if (!string.IsNullOrEmpty(complete.type))
            {
                var currents = _currentList.FindAll(x => x.type == data.type);
                float full = 0;
                foreach (var item in currents)
                {
                    full += item.value;
                }
                float left = (full + data.value) - complete.value;
                left = left > 0 ? left : 0;
                var charge = new ChargeData(data.type, data.value - left);
                return charge;
            }
            else
            {
                return default(ChargeData);
            }
        }

        /// <summary>
        /// 结束与否判断
        /// </summary>
        public void JudgeComplete()
        {
            foreach (var item in completeDatas)
            {
                var currentItems = _currentList.FindAll(x => x.type == item.type);
                if (currentItems.Count == 0)
                {
                    return;
                }

                float full = 0;
                foreach (var charge in currentItems)
                {
                    full += charge.value;
                }
                if (full < item.value)
                {
                    return;
                }
            }

            OnEndExecute(false);
        }

        /// <summary>
        /// 自动填充
        /// </summary>
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
                        if (completeDatas.FindAll(y => tools[i].CanLoad(y.type)).Count == 0) return;

                        if (log) Debug.Log("ActiveElements:" + element.Name + (!tools[i].Started));

                        if (!tools[i].Started)
                        {
                            tools[i].StepActive();
                        }
                    }
                }

                var resources = ElementController.Instence.GetElements<ChargeResource>();
                if (resources != null)
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

                        if (completeDatas.FindAll(y => tools[i].CanLoad(y.type)).Count == 0) return;

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
            foreach (var item in startDatas){
                Charge(item,null);
            }
        }
    }
}