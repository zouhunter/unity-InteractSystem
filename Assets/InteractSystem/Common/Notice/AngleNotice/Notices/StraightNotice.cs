using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractSystem.Notice;

namespace InteractSystem.Notice
{
    public class StraightNotice: AnimAngleNotice
    {
        [SerializeField, Attributes.CustomField("起始点")]
        protected Vector3 startPos;
        [SerializeField, Attributes.CustomField("终点")]
        protected Vector3 endPos;
        [SerializeField, Attributes.CustomField("旋转速度")]
        protected float rotateSpeed = 2f;

        protected override void UpdateAngleState(Coordinate target,GameObject angle,float step)
        {
            var p0 = target.position + startPos;
            var p1 = target.position + endPos;

            var pos = Vector3.Lerp(p0, p1,step );
            angle.transform.position = pos;
            angle.transform.Rotate(endPos - startPos, rotateSpeed);
        }

       
    }
}
