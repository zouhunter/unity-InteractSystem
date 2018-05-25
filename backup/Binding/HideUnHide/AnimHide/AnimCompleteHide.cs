using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem.Binding
{
    [RequireComponent(typeof(AnimPlayer))]
    public class AnimCompleteHide : MonoBehaviour
    {
        public List<GameObject> hideTargets;
        AnimPlayer target;
        private void Awake()
        {
            target = GetComponent<AnimPlayer>();
            target.onPlayComplete.AddListener(HideElements);
        }

        private void HideElements()
        {
            foreach (var item in hideTargets)
            {
                item.gameObject.SetActive(false);
            }
        }
    }
}