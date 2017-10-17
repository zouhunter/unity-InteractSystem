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
        public  static void Clean()
        {
            cameraNodes.Clear();
        }
        public static void RegistCamera(Camera viewCamera)
        {
            viewCameraParent = viewCamera.transform.parent;
            CameraController.viewCamera = viewCamera;
            CameraController.mainCamera = Camera.main;
            viewCamera.gameObject.SetActive(mainCamera == null);
        }
        public static void RegistNode(CameraNode node)
        {
            if (!cameraNodes.Contains(node))
            {
                cameraNodes.Add(node);
            }
        }
        public static void SetViewCamera(string id = null)
        {
            var node = cameraNodes.Find(x =>x != null && x.ID == id);
            if (node != currentNode)
            {
                if (node == null)//移动到主摄像机
                {
                    currentNode.StartCoroutine(MoveCameraToMainCamera());
                }
                else //移动到新坐标
                {
                    node.StartCoroutine(MoveCameraToNode(node));
                }
                currentNode = node;
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

        static IEnumerator MoveCameraToMainCamera()
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
                viewCamera.transform.position = mainCamera.transform.position;
                viewCamera.transform.rotation = mainCamera.transform.rotation;

                viewCamera.gameObject.SetActive(false);
                mainCamera.gameObject.SetActive(true);
                viewCamera.transform.SetParent(viewCameraParent);
            }
           
            yield break;
        }
        static IEnumerator MoveCameraToNode(CameraNode target)
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
            viewCamera.transform.position = target.transform.position;
            viewCamera.transform.rotation = target.transform.rotation;
            viewCamera.transform.SetParent(target.transform);
        }
    }

}