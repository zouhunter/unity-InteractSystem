using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace InteractSystem
{
    [System.Serializable]
    public class OptionalCommandItem
    {
        public bool ignore;
        public string commandName;
        public ActionCommand command;
    }
}