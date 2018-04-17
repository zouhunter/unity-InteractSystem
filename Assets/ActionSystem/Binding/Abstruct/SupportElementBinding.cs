using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem.Binding
{
    [ExecuteInEditMode]
    public class SupportElementBinding : MonoBehaviour
    {
        private ISupportElement supportElement;
        private void Awake()
        {
            supportElement = GetComponent<ISupportElement>();
            if(supportElement == null)
            {
                Debug.LogError("have no supportElement!");
                Destroy(this);
            }
        }
    }

}