using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeGraph;
using NodeGraph.DataModel;

namespace WorldActionSystem.Graph
{
    [CustomConnection("actionconnect")]
    public class ActionConnection : Connection
    {
        public int copyCount;
    }
}