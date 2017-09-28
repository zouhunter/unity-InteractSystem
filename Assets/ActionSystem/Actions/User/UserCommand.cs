using UnityEngine;
using System.Collections;
using System;

namespace WorldActionSystem
{

    public class UserCommand : ActionCommand
    {
        protected override IActionCtroller CreateCtrl()
        {
           return new UserController(this);
        }
    }

}