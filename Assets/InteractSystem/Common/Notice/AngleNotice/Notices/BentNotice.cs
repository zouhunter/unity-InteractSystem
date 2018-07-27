using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractSystem.Notice;

namespace InteractSystem.Notice
{
    public class BentNotice: AnimAngleNotice
    {
        [SerializeField, Attributes.CustomField("旋转中心(相对)")]
        protected Vector3 center;
        [SerializeField]
        protected float radius = 1;
        [SerializeField, Attributes.CustomField("旋转角度")]
        protected float angle;
       

        protected override void UpdateAngleState(Coordinate target,GameObject viewObj,float step)
        {
            var centerPosition = target.position + center;
            var rotation = Quaternion.Euler(target.eulerAngles);
            var axis = rotation * Vector3.forward;
            var startPos = rotation * Vector3.up * radius + centerPosition;
            var targetPosition = Quaternion.Euler(axis * angle * step) * (startPos - centerPosition) + centerPosition;
            viewObj.transform.position = targetPosition;
        }
      
    }
}
