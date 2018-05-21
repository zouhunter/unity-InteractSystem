using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem.Actions
{
    public class BlastAnim : ColonyAnimItem
    {
        [SerializeField]
        protected float targetRange = 1;
        [SerializeField]
        protected Transform center;

        protected override void InitState()
        {
            var length = viewItems.Length;
            startPositions = new Vector3[length];
            targetPositions = new Vector3[length];
            startRotations = new Quaternion[length];

            for (int i = 0; i < length; i++)
            {
                startPositions[i] = viewItems[i].transform.localPosition;
                var centerPos = viewItems[i].transform.parent.InverseTransformPoint(center.transform.position);
                targetPositions[i] = centerPos + (viewItems[i].transform.localPosition - centerPos) * targetRange;
                startRotations[i] = viewItems[i].transform.localRotation;
            }
        }

    }
}
