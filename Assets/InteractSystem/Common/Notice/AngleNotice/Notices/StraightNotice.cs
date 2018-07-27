using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractSystem.Notice;

namespace InteractSystem.Notice
{
    public class StraightNotice: AnimAngleNotice
    {
        [SerializeField]
        protected GameObject anglePrefab;
     
        [SerializeField]
        protected Vector3 startPos;
        [SerializeField]
        protected Vector3 endPos;
        [SerializeField]
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
