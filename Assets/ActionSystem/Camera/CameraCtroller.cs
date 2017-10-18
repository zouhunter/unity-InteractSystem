using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace WorldActionSystem
{
    public class CameraController
    {
        private static List<CameraNode> cameraNodes = new List<CameraNode>();
        public static Camera viewCamera { get; set; }
        private static Camera mainCamera { get;set;}
        private static CameraNode currentNode;
        private static Camera activeCamera { get; set; }
        private static Transform viewCameraParent;
        private static ActionSystem holder;
        private static Coroutine coroutine;
        public  static void Clean()
        {
            cameraNodes.Clear();
        }
        public static void Init(ActionSystem holder)
        {
            CameraController.holder = holder;
            viewCameraParent = holder.transform;
            CameraController.viewCamera = holder.viewCamera;
            CameraController.mainCamera = Camera.main;
            SetTransform(viewCamera.transform, mainCamera.transform);
            viewCamera.gameObject.SetActive(mainCamera == null);
        }
        public static void RegistNode(CameraNode node)
        {
            if (!cameraNodes.Contains(node))
            {
                cameraNodes.Add(node);
            }
        }
        public static void SetViewCamera(UnityAction onComplete,string id = null)
        {
            StopCoroutine();
            var node = cameraNodes.Find(x =>x != null && x.ID == id);
            if (node != currentNode)
            {
                if (node == null)//移动到主摄像机
                {
                    coroutine= holder.StartCoroutine(MoveCameraToMainCamera(onComplete));
                }
                else //移动到新坐标
                {
                    coroutine= holder.StartCoroutine(MoveCameraToNode(node, onComplete));
                }
                currentNode = node;
            }
            else
            {
                onComplete.Invoke();
            }
        }

        internal static Camera GetViewCamera(string cameraID)
        {
            if(string.IsNullOrEmpty(cameraID))
            {
                return mainCamera;
            }
            else if (cameraNodes.Find(x=>x.ID == cameraID))
            {
                return viewCamera;
            }
            else
            {
                return mainCamera;
            }
        }

        static IEnumerator MoveCameraToMainCamera(UnityAction onComplete)
        {
            if(mainCamera == null)
            {
                yield break;
            }
            else
            {
                var startPos = viewCamera.transform.position;
                var startRot = viewCamera.transform.rotation;
                for (float i = 0; i < 1; i += Time.deltaTime)
                {
                    viewCamera.transform.position = Vector3.Lerp(startPos, mainCamera.transform.position, i);
                    viewCamera.transform.rotation = Quaternion.Lerp(startRot, mainCamera.transform.rotation, i);
                    yield return null;
                }
                SetTransform(viewCamera.transform, mainCamera.transform);

                viewCamera.gameObject.SetActive(false);
                mainCamera.gameObject.SetActive(true);
                viewCamera.transform.SetParent(viewCameraParent);
            }
            onComplete.Invoke();
        }
        static IEnumerator MoveCameraToNode(CameraNode target, UnityAction onComplete)
        {
            if(mainCamera != null)
            {
                viewCamera.gameObject.SetActive(true);
                mainCamera.gameObject.SetActive(false);
            }

            var startPos = viewCamera.transform.position;
            var startRot = viewCamera.transform.rotation;
            for (float i = 0; i < target.LerpTime; i +=Time.deltaTime)
            {
                viewCamera.transform.position = Vector3.Lerp(startPos, target.transform.position, i);
                viewCamera.transform.rotation = Quaternion.Lerp(startRot, target.transform.rotation, i);
                yield return null;
            }
            SetTransform(viewCamera.transform, target.transform);
            viewCamera.transform.SetParent(target.transform);
            onComplete.Invoke();
        }
        private static void SetTransform(Transform obj,Transform target)
        {
            obj.position = target.position;
            obj.rotation = target.rotation;
        }
        private static void StopCoroutine()
        {
            if(coroutine != null)
            {
                holder.StopCoroutine(coroutine);
                coroutine = null;
            }
        }
    }

}