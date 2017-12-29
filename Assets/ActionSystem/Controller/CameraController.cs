using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace WorldActionSystem
{
    internal class CameraController : MonoBehaviour
    {
        private static List<CameraNode> cameraNodes = new List<CameraNode>();
        public static Camera viewCamera { get; set; }
        private static Camera mainCamera { get; set; }
        private static CameraNode currentNode;
        private static Camera currentCamera
        {
            get
            {
                if (mainCamera != null && mainCamera.gameObject.activeSelf)
                {
                    return mainCamera;
                }
                else
                {
                    return viewCamera;
                }
            }
        }
        private static Transform viewCameraParent;
        public static CameraController Instence { get; private set; }
        private static ViewCamera _cameraView;
        private static ViewCamera cameraView
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
        protected static Config config { get; set; }
        internal static Camera ActiveCamera
        {
            get
            {
                if (!config.useOperateCamera || viewCamera == null)
                {
                    return mainCamera;
                }
                else
                {
                    return viewCamera;
                }
            }

        }
        internal const string defultID = "defult";
        private static Coroutine lastCoroutine;
        private static UnityAction lastAction;
        public static event UnityAction<Transform> onCameraMoveTo;
        private const float defultSpeed = 5;
        public void Awake()
        {
            Instence = this;
        }

        private void Start()
        {
            viewCameraParent = transform;
            if (viewCamera == null)
            {
                viewCamera = GetComponentInChildren<Camera>(true);
            }

            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.LogError("场景没有主摄像机");
            }

            viewCamera.gameObject.SetActive(mainCamera == null);
        }
        private void OnMainCameraCallBack()
        {
            if (lastAction != null) lastAction.Invoke();
        }
        public static void RegistNode(CameraNode node)
        {
            if (!cameraNodes.Contains(node))
            {
                cameraNodes.Add(node);
            }
        }

        public static void SetViewCamera(UnityAction onComplete, string id = null)
        {
            if (Instence == null || !Instence.gameObject.activeInHierarchy) return;

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
                    lastCoroutine = Instence.StartCoroutine(MoveCameraToMainCamera(OnStepComplete));
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
                    if(currentNode != null){
                        SetTransform(currentNode.transform);
                    }
                    OnStepComplete();
                }
                else
                {
                    lastCoroutine = Instence.StartCoroutine(MoveCameraToNode(node, OnStepComplete));
                }
            }
        }

        static IEnumerator MoveCameraToMainCamera(UnityAction onComplete)
        {
            if (mainCamera != null)
            {
                viewCamera.transform.SetParent(Instence.transform);
                var startPos = viewCamera.transform.position;
                var startRot = viewCamera.transform.rotation;
                var distence = Vector3.Distance(startPos, mainCamera.transform.position);
                var time = (distence / defultSpeed) ;
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

        static IEnumerator MoveCameraToNode(CameraNode target, UnityAction onComplete)
        {
            if (mainCamera != null)
            {
                viewCamera.transform.SetParent(Instence.transform);

                if (!viewCamera.gameObject.activeSelf)
                {
                    SetTransform(mainCamera.transform);
                    viewCamera.gameObject.SetActive(true);
                }
                mainCamera.gameObject.SetActive(false);
            }


            cameraView.enabled = false;

            var startPos = viewCamera.transform.position;
            var startRot = viewCamera.transform.rotation;

            var distence = Vector3.Distance(startPos, target.transform.position);
            var time = (distence / target.Speed) ;
            for (float i = 0; i < time; i += Time.deltaTime)
            {
                viewCamera.transform.position = Vector3.Lerp(startPos, target.transform.position, i/time);
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
        private static void SetTransform(Transform target)
        {
            cameraView.transform.position = target.transform.position;
            cameraView.transform.rotation = target.transform.rotation;
        }
        private static void SetCameraInfo(CameraNode target)
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
        public static void StopStarted(bool force)
        {
            if (lastCoroutine != null)
            {
                Instence.StopCoroutine(lastCoroutine);
                lastCoroutine = null;
            }
            if (force)
            {
                currentNode = null;
            }
            OnStepComplete();
        }
        private static void OnStepComplete()
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