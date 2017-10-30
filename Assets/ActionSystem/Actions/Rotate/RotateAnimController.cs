using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{

    public class RotateAnimController : IActionCtroller
    {
        public UnityAction<string> UserError { get; set; }
        private RotObj selectedObj;
        private RaycastHit hit;
        private Ray ray;
        private float distence = 10;
        private Camera viewCamera { get { return CameraController.ActiveCamera; } }

        public RotateAnimController(float distence)
        {
            this.distence = distence;
        }

        public void Update()
        {
            if (TrySelectRotateObj())
            {
                TransformSelected();
            }
        }
        private bool TrySelectRotateObj()
        {
            if (viewCamera == null) return false;

            ray = viewCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, distence, (1 << Setting.rotateItemLayer)))
            {
                selectedObj = hit.collider.GetComponent<RotObj>();
            }

            return selectedObj != null;
        }

        Vector3 originalTargetPosition;
        Vector3 axis ;
        Vector3 previousMousePosition;

        void TransformSelected()
        {
            if (Input.GetMouseButtonDown(0))
            {
                originalTargetPosition = selectedObj.transform.position;
                axis = selectedObj.Direction;
                previousMousePosition = Vector3.zero;
            }

            if (Input.GetMouseButton(0))
            {
                if (selectedObj.Started)
                {
                    ray = viewCamera.ScreenPointToRay(Input.mousePosition);
                    Vector3 mousePosition = GeometryUtil.LinePlaneIntersect(ray.origin, ray.direction, originalTargetPosition, axis);
                    if (previousMousePosition != Vector3.zero && mousePosition != Vector3.zero && IsInCercle(mousePosition))
                    {
                        var vec1 = previousMousePosition - selectedObj.transform.position;
                        var vec2 = mousePosition - selectedObj.transform.position;
                        float rotateAmount = (Vector3.Angle(Vector3.Cross(vec1, vec2), axis) < 180f ? 1 : -1)
                            * Vector3.Angle(vec1, vec2) * selectedObj.rotSpeed;
                        selectedObj.Rotate(rotateAmount);
                    }

                    previousMousePosition = mousePosition;
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                selectedObj.ClampAsync();

                if (selectedObj.TryMarchRot())
                {
                    selectedObj.OnEndExecute(false);
                }
            }
        }
        private bool IsInCercle(Vector3 pos)
        {
            return Vector3.Distance(selectedObj.transform.position, pos) < selectedObj.triggerRadius;
        }

        public void OnStartExecute(bool forceAuto)
        {
        }

        public void OnEndExecute()
        {
        }

        public void OnUnDoExecute()
        {
        }
    }

}