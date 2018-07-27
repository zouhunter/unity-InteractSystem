using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace InteractSystem.Notice
{
    public abstract class ActionNotice : ScriptableObject
    {
        public virtual void Update() { }
        public abstract void Notice(Coordinate target);
        public abstract void UnNotice(Coordinate target);
    }
}