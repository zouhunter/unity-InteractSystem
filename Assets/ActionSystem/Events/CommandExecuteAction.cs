using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace WorldActionSystem.Events
{
    public delegate void CommandExecuteAction(string stepName, int totalCount, int currentID);
}