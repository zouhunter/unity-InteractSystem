using UnityEngine;
using System.Collections;
using System;

namespace WorldActionSystem
{
    public class UserController : ActionCtroller
    {
        public UserController(ActionCommand trigger) : base(trigger)
        {
        }

        public override IEnumerator Update()
        {
            yield return null;
        }
        public override void OnEndExecute()
        {
            base.OnEndExecute();
            foreach (var item in actionObjs)
            {
                if(!item.Complete)
                {
                    item.OnEndExecute();
                }
            }
        }
    }
}
