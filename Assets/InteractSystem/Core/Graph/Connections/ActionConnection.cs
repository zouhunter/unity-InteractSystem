using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeGraph;
using NodeGraph.DataModel;

namespace InteractSystem.Graph
{
    [CustomConnection("actionconnect")]
    public class ActionConnection : Connection
    {
        [Attributes.Range(0,20)]
        public int copyCount;
    }
}