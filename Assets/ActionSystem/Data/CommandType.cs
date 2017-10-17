using UnityEngine;
using System.Collections;
namespace WorldActionSystem
{
    public enum CommandType
    {
        Install = 1,
        Match = 1<<1,
        Click = 1<<2,
        Rotate = 1<<3,
        Connect = 1<<4
    }
}