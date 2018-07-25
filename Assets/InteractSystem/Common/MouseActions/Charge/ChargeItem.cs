using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace InteractSystem.Actions
{
    public class ChargeItem : ActionItem
    {
        public const string layer = "i:chargeItem";
        public override bool OperateAble
        {
            get
            {
                return targets.Count == 0;
            }
        }
        [SerializeField]
        private ChargeData[] startDatas;
        [SerializeField]
        private List<ChargeData> _completeDatas;
        [SerializeField]
        private float _capacity = 1;
        public float capacity { get { return _capacity; } }

        public List<ChargeData> completeDatas { get { return _completeDatas; } }
        private List<ChargeData> _currentList = new List<ChargeData>();
        public ChargeEvent onCharge { get; set; }
        public List<ChargeData> currentList { get { return _currentList; } }
        private int index;
        private ElementController elementCtrl { get { return ElementController.Instence; } }
        public ClickAbleFeature clickAbleFeature = new ClickAbleFeature();
        public CompleteAbleItemFeature completeAbleFeature = new CompleteAbleItemFeature();

        protected override List<ActionItemFeature> RegistFeatures()
        {
            var features = base.RegistFeatures();
            clickAbleFeature.Init(this, layer);
            features.Add(clickAbleFeature);
           
            completeAbleFeature.Init(this, AutoExecute);
            features.Add(completeAbleFeature);

            return features;
        }

        protected override void Start()
        {
            base.Start();
            InitStartData();
        }
        protected override void OnSetActive(UnityEngine.Object target)
        {
            base.OnSetActive(target);
            //激活可用的tool
            //ActiveElements(this);
        }
        protected override void OnSetInActive(UnityEngine.Object target)
        {
            base.OnSetInActive(target);
            //CompleteElements(this, false);
            CompleteCurrentCharge();
        }

        private void CompleteCurrentCharge()
        {
            var currentListArray = currentList.ToArray();
            foreach (var item in currentListArray)
            {
                var temp = item;
                temp.value = -item.value;
                Charge(temp, null);
            }
            currentList.Clear();
            foreach (var item in completeDatas)
            {
                Charge(item, null);
            }
        }
        public override void UnDoChanges(UnityEngine.Object target)
        {
            base.UnDoChanges(target);
            //CompleteElements(this, true);
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
        public virtual void Charge(ChargeData data, UnityAction onComplete)
        {
            var complete = completeDatas.Find(x => x.type == data.type);
            if (!string.IsNullOrEmpty(complete.type))
            {
                if (onCharge != null)
                    onCharge.Invoke(transform.position, data, onComplete);
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
            completeAbleFeature.OnComplete(lockList[0]);
        }

        public void AutoExecute(Graph.OperaterNode node)
        {
            index = 0;
            AutoCompleteInternal();
        }

        private void AutoCompleteInternal()
        {
            if (index < completeDatas.Count)
            {
                CompleteOneElement(completeDatas[index], AutoCompleteInternal);
                index++;
            }
            else
            {
                completeAbleFeature.OnComplete(firstLock);
            }
        }
        private void CompleteOneElement(ChargeData complete, UnityAction onComplete)
        {
            var currents = currentList.FindAll(x => x.type == complete.type);
            float total = 0;
            foreach (var item in currents)
            {
                total += item.value;
            }

            if (complete.value - total > 0)
            {
                var tools = elementCtrl.GetElements<ChargeTool>();
                var tool = tools.Find(x => x.CanLoad(complete.type) && x.Actived);
                UnityAction chargeObjAction = () =>
                {
                    ChargeCurrentObj(tool, () =>
                    {
                        CompleteOneElement(complete, onComplete);
                    });
                };
                if (!tool.charged)
                {
                    ChargeOneTool(tool, chargeObjAction);
                }
                else
                {
                    chargeObjAction.Invoke();
                }
            }
            else
            {
                onComplete.Invoke();
                Debug.Log("Charge Complete:" + complete.type);
            }
        }
        private void ChargeOneTool(ChargeTool tool, UnityAction onComplete)
        {
            var resources = elementCtrl.GetElements<ChargeResource>();
            var chargeResource = resources.Find(x => tool.CanLoad(x.type) && x.Actived);
            var value = Mathf.Min(tool.capacity, chargeResource.current);
            var type = chargeResource.type;
            tool.PickUpAble = false;
            tool.LoadData(chargeResource.transform.position, new ChargeData(type, value), () => {
                tool.PickUpAble = true;
            });
            chargeResource.Subtruct(value, () => { onComplete.Invoke(); });
        }

        private void ChargeCurrentObj(ChargeTool tool, UnityAction onComplete)
        {
            var data = tool.data;
            ChargeData worpData = JudgeLeft(data);
            if (!string.IsNullOrEmpty(worpData.type))
            {
                tool.PickUpAble = false;
                tool.OnCharge(transform.position, worpData.value, () => { tool.PickUpAble = true; });
                Charge(worpData, () => {
                    onComplete();
                });
            }
        }
        

        private void InitStartData()
        {
            foreach (var item in startDatas)
            {
                Charge(item, null);
            }
        }
    }
}