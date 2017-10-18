using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{

    public class CameraNode : MonoBehaviour
    {
        [SerializeField,Range(1,10)]
        private float _lerpTime = 1;//缓慢移入
        public string ID { get { return name; } }
        public float LerpTime { get { return _lerpTime; } }
        private void Awake()
        {
            CameraController.RegistNode(this);
        }
    }

}