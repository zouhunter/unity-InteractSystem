using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace InteractSystem.Actions
{
    public class GravityDetachRule : DetachRule
    {
        private Rigidbody m_rigidbody;
        
        public override void OnDetach(DetachItem target)
        {
            if (m_rigidbody == null)
            {
                m_rigidbody = target.gameObject.AddComponent<Rigidbody>();
            }
        }

        public override void UnDoDetach()
        {
            if (m_rigidbody != null)
            {
                Destroy(m_rigidbody);
            }
        }
    }
}