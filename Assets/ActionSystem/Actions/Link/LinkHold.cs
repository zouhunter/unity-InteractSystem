using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{
    [System.Serializable]
    public class LinkHold
    {
        public string elementName;
       public LinkItem linkItem;
       public List<LinkPort> linkedPorts = new List<LinkPort>();
    }
}