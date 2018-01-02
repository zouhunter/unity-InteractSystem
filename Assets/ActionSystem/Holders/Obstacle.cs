using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{
    public class Obstacle : MonoBehaviour
    {
        private void Start()
        {
            gameObject.layer = Layers.obstacleLayer;
        }
    }
}
