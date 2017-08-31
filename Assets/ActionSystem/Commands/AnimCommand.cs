using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System;
namespace WorldActionSystem
{

    public class AnimCommand : ActionCommand
    {
        public AnimObj[] anims;
        

        public AnimCommand(string stapName, AnimObj[] anims) : base(stapName)
        {
            this.anims = anims;
        }

        public override void StartExecute(bool forceAuto)
        {
            foreach (var anim in anims){
                anim.PlayAnim();
            }
            base.StartExecute(forceAuto);
        }
        public override void EndExecute()
        {
            foreach (var anim in anims){
                anim.EndPlay();
            }
            base.EndExecute();
        }
        public override void UnDoCommand()
        {
            foreach (var anim in anims) {
                anim.UnDoPlay();
            }
            base.UnDoCommand();
        }
    }


}