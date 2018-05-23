using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace WorldActionSystem
{
    [AddComponentMenu(MenuName.ActionSystem)]
    public class ActionSystem : MonoBehaviour
    {
        #region Instence
        private static bool isQuit = false;
        private static ActionSystem _instence;
        internal static ActionSystem Instence
        {
            get
            {
                if (_instence == null && !isQuit)
                {
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
                    _actionCtrl = new ActionCtroller(this,pickupCtrl);
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

        #region PreviewCtrl
        private PreviewController _previewCtrl;
        public PreviewController previewCtrl
        {
            get
            {

                if (_previewCtrl == null)
                {
                    _previewCtrl = new PreviewController(this);
                }
                return _previewCtrl;
            }
        }
        #endregion

        #region PickUpCtrl
        private PickUpController _pickupCtrl;
        public PickUpController pickupCtrl
        {
            get
            {
                if (_pickupCtrl == null)
                {
                    _pickupCtrl = new PickUpController(this);
                }
                return _pickupCtrl;
            }
        }
        #endregion

        #region TimeCtrl
        private CoroutineController _corontineCtrl;
        public CoroutineController CoroutineCtrl
        {
            get
            {
                if(_corontineCtrl == null)
                {
                    _corontineCtrl = new CoroutineController(this);
                }
                return _corontineCtrl;
            }
        }
        #endregion

        private List<ActionGroup> actionGroup = new List<ActionGroup>();

        public void RegistGroup(ActionGroup group)
        {
            if(actionGroup.Contains(group))
            {
                actionGroup.Add(group);
            }
        }

        public void RemoveGroup(ActionGroup group)
        {
            if(!actionGroup.Contains(group))
            {
                actionGroup.Clear();
            }

            if(actionGroup.Count == 0)
            {
                Clean();
            }
        }

        private void Clean()
        {
            if (_instence != null)
            {
                Destroy(_instence.gameObject);
            }
        }
    }

}