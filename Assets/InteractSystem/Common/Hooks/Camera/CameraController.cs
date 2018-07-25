using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace InteractSystem
{
    public class ActionSingleLenton<T> where T : ActionSingleLenton<T>, new()
    {
        protected static T instance = default(T);
        protected static object lockHelper = new object();
        protected ActionSystem actionSystem;
        public static T Instence
        {
            get
            {
                if (instance == null)
                {
                    lock (lockHelper)
                    {
                        if (instance == null || !instance.actionSystem || !instance.actionSystem.gameObject)
                        {
                            instance = new T();
                            if (ActionSystem.Instence)
                            {
                                instance.InitInstence(ActionSystem.Instence);
                            }
                            else
                            {
                                Debug.LogWarning("ActionSystem Don`t Exists!");
                            }
                        }
                    }
                }
                return instance;
            }
        }
        protected virtual void InitInstence(ActionSystem actionSystem)
        {
            this.actionSystem = actionSystem;
        }
    }
    public class CameraController : ActionSingleLenton<CameraController>
    {
        private List<CameraNode> cameraNodes = new List<CameraNode>();
        private Camera viewCamera;
        private Camera mainCamera;
        private CameraNode currentNode;
        public Camera currentCamera
        {
            get
            {
                if (mainCamera != null && mainCamera.gameObject && mainCamera.gameObject.activeSelf)
                {
                    return mainCamera;
                }
                else
                {
                    return viewCamera;
                }
            }
        }
        private Transform viewCameraParent;
        private ViewCamera _cameraView;
        private ViewCamera cameraView
        {
            get
            {
                if (_cameraView == null)
                {
                    _cameraView = viewCamera.GetComponent<ViewCamera>();
                }
                return _cameraView;
            }
        }
        internal Camera GetActiveCamera(bool useOperateCamera)
        {
            if (!useOperateCamera || viewCamera == null)
            {
                return mainCamera;
            }
            else
            {
                return viewCamera;
            }
        }
        internal const string defultID = "defult";
        private Coroutine lastCoroutine;
        private UnityAction lastAction;
        public event UnityAction<Transform> onCameraMoveTo;
        private const float defultSpeed = 5;
        private const float maxTime = 2f;

        protected override void InitInstence(ActionSystem actionSystem)
        {
            base.InitInstence(actionSystem);
            viewCameraParent = this.actionSystem.transform;
            mainCamera = Camera.main;
            Debug.Assert(mainCamera);
            viewCamera = UnityEngine.Object.Instantiate(mainCamera);
            viewCamera.gameObject.AddComponent<ViewCamera>();
            viewCamera.transform.SetParent(this.actionSystem.transform);
            viewCamera.gameObject.SetActive(!mainCamera.isActiveAndEnabled);
        }

        private void OnMainCameraCallBack()
        {
            if (lastAction != null) lastAction.Invoke();
        }
        public void RegistNode(CameraNode node)
        {
            if (!cameraNodes.Contains(node))
            {
                cameraNodes.Add(node);
            }
        }
        internal void RemoveNode(CameraNode node)
        {
            if (cameraNodes.Contains(node))
            {
                cameraNodes.Remove(node);
            }
        }

        public void SetViewCameraQuick(string id = null)
        {
            StopStarted(false);

            if (string.IsNullOrEmpty(id))
            {
                return;
            }
            else if (id == defultID)
            {
                currentNode = null;
                if (currentCamera != mainCamera)
                {
                    if (mainCamera)
                    {
                        viewCamera.gameObject.SetActive(false);
                        mainCamera.gameObject.SetActive(true);
                        viewCamera.transform.SetParent(viewCameraParent);
                    }
                }
            }
            else
            {
                var node = cameraNodes.Find(x => x != null && x.ID == id);
                if (node == null || node == currentNode)
                {
                    currentNode = node;
                    if (currentNode != null)
                    {
                        SetTransform(currentNode.transform);
                    }
                }
                else
                {
                    viewCamera.transform.SetParent(node.transform);
                    SetCameraInfo(node);
                    currentNode = node;
                }
            }
        }

        public void SetViewCameraAsync(UnityAction onComplete, string id = null)
        {
            StopStarted(false);

            lastAction = onComplete;

            if (string.IsNullOrEmpty(id))
            {
                OnStepComplete();
            }
            else if (id == defultID)
            {
                currentNode = null;
                if (currentCamera != mainCamera)
                {
                    lastCoroutine = actionSystem.StartCoroutine(MoveCameraToMainCamera(OnStepComplete));
                }
                else
                {
                    OnStepComplete();
                }
            }
            else
            {
                var node = cameraNodes.Find(x => x != null && x.ID == id);
                if (node == null || node == currentNode)
                {
                    currentNode = node;
                    if (currentNode != null)
                    {
                        SetTransform(currentNode.transform);
                    }
                    OnStepComplete();
                }
                else
                {
                    lastCoroutine = actionSystem.StartCoroutine(MoveCameraToNode(node, OnStepComplete));
                }
            }
        }

        IEnumerator MoveCameraToMainCamera(UnityAction onComplete)
        {
            if (mainCamera != null)
            {
                viewCamera.transform.SetParent(actionSystem.transform);
                var startPos = viewCamera.transform.position;
                var startRot = viewCamera.transform.rotation;
                var distence = Vector3.Distance(startPos, mainCamera.transform.position);
                var time = Mathf.Clamp((distence / defultSpeed), 0, maxTime);
                for (float i = 0; i < time; i += Time.deltaTime)
                {
                    viewCamera.transform.position = Vector3.Lerp(startPos, mainCamera.transform.position, i / time);
                    viewCamera.transform.rotation = Quaternion.Lerp(startRot, mainCamera.transform.rotation, i / time);
                    yield return null;
                }

                SetTransform(mainCamera.transform);

                viewCamera.gameObject.SetActive(false);
                mainCamera.gameObject.SetActive(true);
                viewCamera.transform.SetParent(viewCameraParent);
            }

            if (onComplete != null) onComplete.Invoke();
        }

        IEnumerator MoveCameraToNode(CameraNode target, UnityAction onComplete)
        {
            if (mainCamera != null)
            {
                viewCamera.transform.SetParent(actionSystem.transform);

                if (!viewCamera.gameObject.activeSelf)
                {
                    SetTransform(mainCamera.transform);
                    mainCamera.gameObject.SetActive(false);
                    viewCamera.gameObject.SetActive(true);
                }
            }


            cameraView.enabled = false;

            var startPos = viewCamera.transform.position;
            var startRot = viewCamera.transform.rotation;

            var distence = Vector3.Distance(startPos, target.transform.position);
            var time = Mathf.Clamp((distence / target.Speed), 0, maxTime);
            for (float i = 0; i < time; i += Time.deltaTime)
            {
                viewCamera.transform.position = Vector3.Lerp(startPos, target.transform.position, i / time);
                viewCamera.transform.rotation = Quaternion.Lerp(startRot, target.transform.rotation, i / time);
                yield return null;
            }
            viewCamera.transform.SetParent(target.transform);

            SetCameraInfo(target);

            currentNode = target;


            if (onCameraMoveTo != null)
            {
                onCameraMoveTo.Invoke(target.transform);
            }

            if (onComplete != null) onComplete.Invoke();
        }
        private void SetTransform(Transform target)
        {
            cameraView.transform.position = target.transform.position;
            cameraView.transform.rotation = target.transform.rotation;
        }
        private void SetCameraInfo(CameraNode target)
        {
            if (target.MoveAble)
            {
                cameraView.enabled = true;
                cameraView.distence = target.Distence;
                cameraView.SetSelf(target.transform.position, target.Rotation);
            }
            else
            {
                cameraView.enabled = false;
            }
            cameraView.targetView = target.CameraField;
        }
        public void OnDestroy()
        {
            lastAction = null;
            lastCoroutine = null;
            cameraNodes.Clear();
        }
        /// <summary>
        /// 只结束开始
        /// [选择性清理节点缓存]
        /// </summary>
        /// <param name="force"></param>
        public void StopStarted(bool force)
        {
            if (lastCoroutine != null)
            {
                actionSystem.StopCoroutine(lastCoroutine);
                lastCoroutine = null;
            }
            if (force)
            {
                currentNode = null;
            }
            OnStepComplete();
        }
        private void OnStepComplete()
        {
            if (lastAction != null)
            {
                var action = lastAction;
                lastAction = null;
                action.Invoke();
            }
        }
    }

}