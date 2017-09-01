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
            rotAnimCtrl.onHover = OnHover;
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
                if (objDic.ContainsKey(obj.StepName))
                {
                    objDic[obj.StepName].Add(obj);
                }
                else
                {
                    objDic[obj.StepName] = new List<RotObj>() { obj };
                }
            }
        }

        internal void SetStepUnDo(string stepName)
        {
            var list = objDic[currStepName];
            foreach (var item in list)
            {
                item.UnDoExecute();
            }
        }
        private void OnHover(RotObj arg0)
        {
            Debug.Log("hover");
        }

        private void OnEndRot(RotObj arg0)
        {
            Debug.Log("OnEndRot");
        }

        private void OnStartRot(RotObj arg0)
        {
            Debug.Log("OnStartRot");
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
                item.EndExecute();
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
                    item.StartExecute();
                    rotAnimCtrl.SetViewCamera(item.ViewCamera);
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
                item.StartExecute();
            }
        }

    }

}