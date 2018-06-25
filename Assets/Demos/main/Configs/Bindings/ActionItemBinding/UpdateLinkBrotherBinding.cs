using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using InteractSystem.Binding;
using InteractSystem;
using InteractSystem.Actions;

public class UpdateLinkBrotherBinding : ActionItemBinding
{
    protected LinkItem linkItem;
    protected List<LinkItem> context = new List<LinkItem>();
    protected bool active;
    public override void OnActive(ActionItem target)
    {
        base.OnActive(target);
        linkItem = target.GetComponent<LinkItem>();
        active = true;
    }
    public override void Update()
    {
        base.Update();
        if (active && linkItem != null)
        {
            context.Clear();
            Debug.Log("UpdateBrotherPos");
            LinkUtil.UpdateBrotherPos(linkItem, context);
        }
    }
    public override void OnInActive(ActionItem target)
    {
        base.OnInActive(target);
        linkItem = null;
        active = false;
    }
}
