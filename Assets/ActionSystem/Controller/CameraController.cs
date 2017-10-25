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
        private static Camera activeCamera { get; set; }
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
        internal static Camera ActiveCamera
        {
            get
            {
                if (!Setting.useOperateCamera || viewCamera == null)
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
        public void Awake()
        {
            Instence = this;
        }
        private void Start()
        {
            viewCameraParent = transform;
            if (viewCamera == null){
                viewCamera = GetComponentInChildren<Camera>(true);
            }

            mainCamera = Camera.main;
            if (mainCamera == null){
                Debug.LogError("场景没有主摄像机");
            }
            viewCamera.gameObject.SetActive(mainCamera == null);
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
            if(lastAction != null)
            {
                StopLastCoroutine();
            }

            if (id == null)
            {
                OnStepComplete(onComplete);
            }

            else if (id == defultID)
            {
                lastAction = onComplete;
                lastCoroutine = Instence.StartCoroutine(MoveCameraToMainCamera(onComplete));
            }

            else
            {
                var node = cameraNodes.Find(x => x != null && x.ID == id);
                if(node == null || node == currentNode)
                {
                    OnStepComplete(onComplete);
                }
                else 
                {
                    currentNode = node;
                    lastAction = onComplete;
                    lastCoroutine = Instence.StartCoroutine(MoveCameraToNode(node, onComplete));
                }
            }
        }

        

        static IEnumerator MoveCameraToMainCamera(UnityAction onComplete)
        {
            if (mainCamera != null)
            {
                var startPos = viewCamera.transform.position;
                var startRot = viewCamera.transform.rotation;
                for (float i = 0; i < 1; i += Time.deltaTime)
                {
                    viewCamera.transform.position = Vector3.Lerp(startPos, mainCamera.transform.position, i);
                    viewCamera.transform.rotation = Quaternion.Lerp(startRot, mainCamera.transform.rotation, i);
                    yield return null;
                }

                SetTransform(mainCamera.transform);

                viewCamera.gameObject.SetActive(false);
                mainCamera.gameObject.SetActive(true);
                viewCamera.transform.SetParent(viewCameraParent);
            }
            OnStepComplete(onComplete);//
        }
        static IEnumerator MoveCameraToNode(CameraNode target,UnityAction onComplete)
        {
            if (mainCamera != null)
            {
                if (!viewCamera.gameObject.activeSelf){
                    SetTransform(mainCamera.transform);
                    viewCamera.gameObject.SetActive(true);
                }
                mainCamera.gameObject.SetActive(false);
            }

            
            cameraView.enabled = false;

            var startPos = viewCamera.transform.position;
            var startRot = viewCamera.transform.rotation;
            for (float i = 0; i < target.LerpTime; i += Time.deltaTime)
            {
                viewCamera.transform.position = Vector3.Lerp(startPos, target.transform.position, i);
                viewCamera.transform.rotation = Quaternion.Lerp(startRot, target.transform.rotation, i);
                yield return null;
            }
            Projects.Log("SetCameraInfo:" + target.name);

            viewCamera.transform.SetParent(target.transform);
            SetCameraInfo(target);
            OnStepComplete(onComplete);
        }
        private static void SetTransform(Transform target)
        {
            cameraView.SetSelf(target.transform.position, target.transform.rotation);
        }
        private static void SetCameraInfo(CameraNode target)
        {
            if (target.MoveAble)
            {
                cameraView.enabled = true;
                cameraView.distence = target.Distence;
                cameraView.SetSelf(target.transform.position, target.Rotation);
            }
            cameraView.targetView = target.CameraField;
        }
        public void OnDestroy()
        {
            cameraNodes.Clear();
        }
        private static void StopLastCoroutine()
        {
            if(lastCoroutine != null)
            {
                Instence.StopCoroutine(lastCoroutine);
                OnStepComplete(lastAction);
            }
        }
        private static void OnStepComplete(UnityAction onComplete)
        {
            if (onComplete != null)
            {
                Projects.Log("Camera CallBack ");
                onComplete.Invoke();
                onComplete = null;
                lastCoroutine = null;
                lastAction = null;
            }
        }
    }

}