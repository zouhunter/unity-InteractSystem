using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using NodeGraph;
using NodeGraph.DataModel;

namespace WorldActionSystem.Graph
{
    [CustomNodeView(typeof(AnimNode))]
    public class AnimNodeView : NodeView
    {
        public override Node target
        {
            get
            {
                return base.target;
            }

            set
            {
                base.target = value;
            }
        }

    }

}