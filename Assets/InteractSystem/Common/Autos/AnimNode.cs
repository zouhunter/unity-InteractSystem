using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using NodeGraph;
using System.Collections.Generic;

namespace InteractSystem.Auto
{
    [CustomNode("Auto/Anim", 5, "InteractSystem")]
    public class AnimNode : Graph.OperaterNode
    {

        [SerializeField,Attributes.CustomField("延时播放")]
        private float delyTime = 0f;
        [SerializeField,Attributes.Range(0.1f, 10f), Attributes.CustomField("播放速度")]
        private float speed = 1;
        [SerializeField, Attributes.CustomField("反向播放")]
        private bool opposite;
        private CoroutineController coroutineCtrl { get { return CoroutineController.Instence; } }
        private ElementController elementCtrl { get { return ElementController.Instence; } }
        [SerializeField]
        private QueueCollectNodeFeature collectNodeFeature = new QueueCollectNodeFeature(typeof(AnimPlayer));
        protected override List<OperateNodeFeature> RegistFeatures()
        {
            var features = base.RegistFeatures();
            collectNodeFeature.SetTarget(this);
            collectNodeFeature.onBeforeAutoExecute += OnBeforeAutoExecute;
            features.Add(collectNodeFeature);
            return features;
        }
        /// <summary>
        /// 对目标进行修改
        /// </summary>
        /// <param name="arg0"></param>
        private void OnBeforeAutoExecute(CompleteAbleItemFeature arg0)
        {
            var animPlayer = arg0.target as AnimPlayer;
            animPlayer.speed = speed;
            animPlayer.opposite = opposite;
        }

        /// <summary>
        /// 播放动画
        /// </summary>
        public override void OnStartExecute(bool auto)
        {
            base.OnStartExecute(auto);
            coroutineCtrl.DelyExecute(collectNodeFeature.AutoCompleteItems, delyTime);
        }
        

    }
}