using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{

    public class RotateAnimController:IActionCtroller
    {
        private RotObj selectedObj;
        private RaycastHit hit;
        private Ray ray;
        private float distence = 10;
        private Camera objCamera;

        public RotateAnimController(ActionCommand trigger)
        {
            InitCommand(trigger);
        }

        private ActionCommand trigger { get; set; }

        public IEnumerator Update()
        {
            while (true)
            {
                yield return new WaitForFixedUpdate();

                if (TrySelectRotateObj())
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        yield return TransformSelected(selectedObj);
                    }
                }
            }
        }

        public void SetViewCamera(Camera objCamera)
        {
            this.objCamera = objCamera;
        }
        private bool TrySelectRotateObj()
        {
            if (objCamera == null) return false;

            ray = objCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, distence, (1 << Setting.rotateItemLayer)))
            {
                selectedObj = hit.collider.GetComponent<RotObj>();
            }

            return selectedObj != null;
        }

        IEnumerator TransformSelected(RotObj selectedObj)
        {
            Vector3 originalTargetPosition = selectedObj.transform.position;
            Vector3 axis = selectedObj.Direction;
            Vector3 previousMousePosition = Vector3.zero;
            while (!Input.GetMouseButtonUp(0) && selectedObj.Started)
            {
                ray = objCamera.ScreenPointToRay(Input.mousePosition);
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

                yield return null;
            }

           yield return selectedObj.Clamp();

            if (selectedObj.TryMarchRot())
            {
                //if (!SetNextRotateAble())
                //{
                //    trigger.Complete();
                //}
                Debug.Log("Match");
            }
        }
        private bool IsInCercle(Vector3 pos)
        {
            return Vector3.Distance(selectedObj.transform.position, pos) < selectedObj.triggerRadius;
        }

        public void InitCommand(ActionCommand trigger)
        {
            this.trigger = trigger;
        }

        public void OnStartExecute(bool forceAuto)
        {
            throw new NotImplementedException();
        }

        public void OnEndExecute()
        {
            throw new NotImplementedException();
        }

        public void OnUnDoExecute()
        {
            throw new NotImplementedException();
        }
    }

}