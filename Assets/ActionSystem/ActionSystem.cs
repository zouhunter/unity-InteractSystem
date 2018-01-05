using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace WorldActionSystem
{
    public class ActionSystem : MonoBehaviour
    {
        #region Instence
        private static bool isQuit = false;
        private static ActionSystem _instence;
        public static ActionSystem Instence
        {
            get
            {
                if (_instence == null && !isQuit)
                {
                    Debug.Log("_instence");
                    _instence = new GameObject("ActionSystem").AddComponent<ActionSystem>();
                }
                return _instence;
            }
        }
        private void OnApplicationQuit()
        {
            isQuit = true;
        }
        #endregion

        #region actionCtrl
        private ActionCtroller _actionCtrl;
        public ActionCtroller actionCtrl { get {
                if (_actionCtrl == null)
                {
                    _actionCtrl = new ActionCtroller(this, pickUpCtrl);
                }
                return _actionCtrl;
            } }
        #endregion

        #region CameraCtrl
        private CameraController _cameraCtrl;
        public CameraController cameraCtrl { get {
                if (_cameraCtrl == null)
                {
                    _cameraCtrl = new CameraController(this);
                }return _cameraCtrl;
            } }
        #endregion

        #region AngleCtrl
        private AngleCtroller _angleCtrl;
        public AngleCtroller angleCtrl { get {

                if (_angleCtrl == null)
                {
                    _angleCtrl = new AngleCtroller(this);
                }
                return _angleCtrl;
            } }
        #endregion

        #region PickUpCtrl
        private PickUpController _pickUpCtrl;
        public PickUpController pickUpCtrl
        {
            get
            {
                if (_pickUpCtrl == null)
                {
                    _pickUpCtrl = new WorldActionSystem.PickUpController(this);
                }
                return _pickUpCtrl;
            }
        }
        #endregion

        private List<ActionGroup> groupList = new List<ActionGroup>();
        private Dictionary<string, List<UnityAction<ActionGroup>>> waitDic = new Dictionary<string, List<UnityAction<ActionGroup>>>();

        private void Awake()
        {
            if (_instence == null)
            {
                _instence = this;
            }
        }

        public void RetriveAsync(string groupKey, UnityAction<ActionGroup> onRetrive)
        {
            if (onRetrive == null) return;
            var item = groupList.Find(x => x.groupKey == groupKey);
            if (item)
            {
                onRetrive.Invoke(item);
            }
            else
            {
                if (!waitDic.ContainsKey(groupKey))
                {
                    waitDic[groupKey] = new List<UnityAction<ActionGroup>>();
                }
                waitDic[groupKey].Add(onRetrive);
            }
        }

        internal void RegistGroup(ActionGroup actionGroup)
        {
            if (!groupList.Contains(actionGroup))
            {
                groupList.Add(actionGroup);
                actionGroup.transform.SetParent(transform);
            }
            if (waitDic.ContainsKey(actionGroup.groupKey))
            {
                var actions = waitDic[actionGroup.groupKey];
                waitDic.Remove(actionGroup.groupKey);
                foreach (var item in actions)
                {
                    item.Invoke(actionGroup);
                }
            }
        }
        public static void Clean()
        {
            if (_instence != null)
            {
                Destroy(_instence.gameObject);
            }
        }
        internal void RemoveGroup(ActionGroup actionGroup)
        {
            if (groupList.Contains(actionGroup))
            {
                groupList.Remove(actionGroup);
            }
        }
    }

}