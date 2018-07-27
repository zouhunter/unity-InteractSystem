using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractSystem.Notice;

namespace InteractSystem.Notice
{
    public class AngleNotice : ActionNotice
    {
        [SerializeField, Attributes.CustomField("预制体")]
        protected GameObject prefab;

        protected AngleCtroller angleCtrl { get { return AngleCtroller.Instence; } }
        protected List<Coordinate> noticed = new List<Coordinate>();

        public override void Notice(Coordinate target)
        {
            if (target != null)
            {
                angleCtrl.Notice(target, prefab);
                if (!noticed.Contains(target))
                    noticed.Add(target);
            }
        }
        public override void UnNotice(Coordinate target)
        {
            if (target != null)
            {
                angleCtrl.UnNotice(target);
                if (noticed.Contains(target))
                    noticed.Remove(target);
            }
        }
      
    }
}
