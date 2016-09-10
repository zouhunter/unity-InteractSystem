using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using WorldActionSystem;
namespace WorldActionSystem
{
		
public abstract class ActionObj : MonoBehaviour
{
    protected IRemoteController RemoteCtrl
    {
        get
        {
            return ActionSystem.Instance.remoteController;
        }
    }
}

	}