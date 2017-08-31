using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{
    public class RotGroup : MonoBehaviour
    {
        internal UnityAction onAllRotateOK;
        private RotObj[] objs;
        private Dictionary<string, List<RotObj>> objDic = new Dictionary<string, List<RotObj>>();
        private RotateAnimController rotAnimCtrl;
        private List<int> queueID = new List<int>();
        private string currStepName;
        private void Start()
        {
            InitObjects();
            rotAnimCtrl = new WorldActionSystem.RotateAnimController();
            rotAnimCtrl.OnRotateOk = OnRoateOK;
            rotAnimCtrl.onStartRot = OnStartRot;
            rotAnimCtrl.onEndRot = OnEndRot;
            StartCoroutine(rotAnimCtrl.StartRotateAnimContrl());
        }

      
        void InitObjects()
        {
            objs = gameObject.GetComponentsInChildren<RotObj>(true);
            foreach (var obj in objs)
            {
                if (objDic.ContainsKey(obj.stapName))
                {
                    objDic[obj.stapName].Add(obj);
                }
                else
                {
                    objDic[obj.stapName] = new List<RotObj>() { obj };
                }
            }
        }
        private void OnEndRot(RotObj arg0)
        {
            //throw new NotImplementedException();
            //Debug.Log("结束旋转：" + arg0.name);
        }

        private void OnStartRot(RotObj arg0)
        {
            //throw new NotImplementedException();
            //Debug.Log("开始旋转：" + arg0.name);
        }

        void OnRoateOK(RotObj obj)
        {
            if (!SetNextRotateAble()) {
                onAllRotateOK.Invoke();
            }
        }

        internal void SetHighLightState(bool on)
        {
            foreach (var obj in objs)
            {
                obj.SetHighLight(on);
            }
        }

        internal void SetRotateComplete(bool playAnim = false)
        {
            var list = objDic[currStepName];
            foreach (var item in list) {
                item.SetRotateEndState();
            }
            if (playAnim)
            {
                onAllRotateOK.Invoke();
            }
           
        }

        internal void SetRotateQueue(string stepName)
        {
            this.currStepName = stepName;
            queueID.Clear();
            var btns = objDic[stepName];
            foreach (var item in btns)
            {
                if (!queueID.Contains(item.queueID))
                {
                    queueID.Add(item.queueID);
                }
            }
            queueID.Sort();
            SetNextRotateAble();
        }

        private bool SetNextRotateAble()
        {
            if (queueID.Count > 0)
            {
                var id = queueID[0];
                queueID.RemoveAt(0);
                var items = objDic[currStepName];
                var neetActive = items.FindAll(x => x.queueID == id);
                foreach (var item in neetActive) {
                    item.SetActiveStep();
                }
                return true;
            }
            return false;
        }

        internal void ActiveStep(string currStepName)
        {
            this.currStepName = currStepName;
        }

        internal void SetRotateStart(string stepName)
        {
            var list = objDic[stepName];
            foreach (var item in list)
            {
                item.SetRotateStartState();
            }
        }

    }

}