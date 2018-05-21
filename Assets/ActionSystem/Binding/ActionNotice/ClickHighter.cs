//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.Events;
//using System.Collections;
//using System.Collections.Generic;
//using WorldActionSystem;
//namespace WorldActionSystem.Binding
//{
//    [RequireComponent(typeof(Actions.ClickObj))]
//    public class ClickHighter : ActionHighLighter
//    {
//        [SerializeField]
//        private Color wrongColor = Color.red;
//        private Actions.ClickObj clickObj;
//        private Color temp;
//        protected override void Awake()
//        {
//            base.Awake();
//            clickObj = actionObj as Actions.ClickObj;
//            clickObj.onMouseEnter.AddListener(OnEnterClickObj);
//            clickObj.onMouseExit.AddListener(OnExitClickObj);
//            temp = color;
//        }
//        private void OnEnterClickObj()
//        {
//            if (!Config.highLightNotice) return;

//            if (clickObj.Started && !clickObj.Complete)
//            {
//                color = temp;
//                HighLight();
//            }
//            else
//            {
//                color = wrongColor;
//                HighLight();
//            }
//        }
//        private void OnExitClickObj()
//        {
//            UnHighLight();
//        }
//    }
//}