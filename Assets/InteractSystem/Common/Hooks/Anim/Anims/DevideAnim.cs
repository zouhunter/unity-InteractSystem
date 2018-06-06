using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace InteractSystem.Hooks
{
    public class DevideAnim : ColonyAnimItem
    {
        [SerializeField]
        protected Transform start;
        [SerializeField]
        protected Transform target;

        protected override void InitState()
        {
            var length = viewItems.Length;
            startPositions = new Vector3[length];
            targetPositions = new Vector3[length];
            startRotations = new Quaternion[length];

            for (int i = 0; i < length; i++)
            {
                startPositions[i] = viewItems[i].transform.localPosition;
                startRotations[i] = viewItems[i].transform.localRotation;
                if(length > 1)
                {
                    var startPos = viewItems[i].transform.parent.InverseTransformPoint(start.position);
                    var endPos = viewItems[i].transform.parent.InverseTransformPoint(target.position);
                    targetPositions[i] = viewItems[i].transform.localPosition + startPos + ((endPos - startPos) * (i)) / (length - 1);
                }
                else
                {
                    targetPositions[i] = viewItems[i].transform.localPosition;
                }
            }
        }
       
    }
}