using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{

    public class RotateAnimController
    {
        private RotObj selectedObj;
        private RaycastHit hit;
        private Ray ray;
        private float distence = 10;

        public UnityAction<RotObj> OnRotateOk;
        public UnityAction<RotObj> onStartRot;
        public UnityAction<RotObj> onEndRot;

        public IEnumerator StartRotateAnimContrl()
        {
            Vector3 worldPos = new Vector3();
            while (true)
            {
                yield return new WaitForFixedUpdate();

                worldPos.x = Input.mousePosition.x;
                worldPos.y = Input.mousePosition.y;
                worldPos.z = 10;
                ray = Camera.main.ScreenPointToRay(worldPos);

                if (TrySelectRotateObj())
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        yield return TransformSelected(selectedObj);
                    }
                }
            }
        }


        private bool TrySelectRotateObj()
        {
            if (Physics.Raycast(ray, out hit, distence, LayerMask.GetMask(Setting.rotateItemLayer)))
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
            if (onStartRot != null) onStartRot.Invoke(selectedObj);
            GameObject cube = new GameObject();
            while (!Input.GetMouseButtonUp(0) && selectedObj.RotateAble)
            {
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Vector3 mousePosition = GeometryUtil.LinePlaneIntersect(ray.origin, ray.direction, originalTargetPosition, axis);
                cube.transform.position = mousePosition;
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
                if (OnRotateOk != null) OnRotateOk.Invoke(selectedObj);
                Debug.Log("Match");
            }

            if (onEndRot != null) onEndRot.Invoke(selectedObj);
        }
        private bool IsInCercle(Vector3 pos)
        {
            return Vector3.Distance(selectedObj.transform.position, pos) < selectedObj.triggerRadius;
        }

    }

}