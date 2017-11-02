using UnityEngine;
using System.Collections;
namespace WorldActionSystem
{
    public enum ControllerType
    {
        Install = 1,
        Match = 1<<1,
        Click = 1<<2,
        Rotate = 1<<3,
        Connect = 1<<4,
        Rope = 1<<5
    }
}