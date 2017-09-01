using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using WorldActionSystem;
public class HideAndUnHideInstall : MonoBehaviour {

    [SerializeField]
    private List<InstallObj> m_Objs;

    public void HideGameObjects()
    {
        for (int i = 0; i < m_Objs.Count; i++)
        {
            m_Objs[i].obj.gameObject.SetActive(false);
        }
    }
    public void UnHideGameObjects()
    {
        for (int i = 0; i < m_Objs.Count; i++)
        {
            m_Objs[i].obj.gameObject.SetActive(true);
        }
    }
}
