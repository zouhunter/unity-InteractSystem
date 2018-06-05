using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace InteractSystem
{
    public class Obstacle : MonoBehaviour
    {
        private void Start()
        {
            SetAllChildColliderAsObstacle();
        }
        private void SetAllChildColliderAsObstacle()
        {
            var colliders = GetComponentsInChildren<Collider>();
            foreach (var item in colliders)
            {
                item. gameObject.layer =LayerMask.NameToLayer( Layers.obstacleLayer);
            }
        }
    }
}
