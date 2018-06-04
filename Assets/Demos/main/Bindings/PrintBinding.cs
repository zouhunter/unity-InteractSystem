using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

using WorldActionSystem;
using WorldActionSystem.Graph;

public class PrintBinding : WorldActionSystem.Binding.ActionBinding
{
    public override void OnStartExecuteInternal(OperateNode node, bool auto)
    {
        base.OnStartExecuteInternal(node, auto);
        Debug.Log("[printBinding :OnStartExecuteInternal]" + node);
    }
    public override void OnBeforeEnd(OperateNode node, bool force)
    {
        base.OnBeforeEnd(node, force);
        Debug.Log("[printBinding :OnBeforeEnd]" + node);
    }
    public override void OnUnDoExecuteInternal(OperateNode node)
    {
        base.OnUnDoExecuteInternal(node);
        Debug.Log("[printBinding :OnUnDoExecuteInternal]" + node);
    }
}
