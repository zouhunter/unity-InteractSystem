using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace InteractSystem
{

    public class InteractObject : ScriptableObject
    {
        public ActionSystem actionSystem
        {
            get
            {
                return ActionSystem.Instence;
            }
        }
        public CoroutineController coroutineCtrl
        {
            get
            {
                return CoroutineController.Instence;
            }
        }
    }

}