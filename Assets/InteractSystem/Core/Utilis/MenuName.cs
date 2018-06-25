using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InteractSystem
{
    public class MenuName
    {
        public const string BaseName = "WorldActionSystem/";
        public const string ActionBaseName = BaseName + "Actions/";
        public const string HookBaseName = BaseName + "Actions/";
        public const string PickUpBaseName = BaseName + "PickUpAble";

        public const string ActionSystem = BaseName + "InteractSystem";
        public const string ActionCommand = BaseName + "ActionCommand";
        public const string ActionGroup = BaseName + "ActionGroup";
        
        public const string ClickNode = ActionBaseName + "ClickNode";
        public const string ConnectObj = ActionBaseName + "ConnectObj";
        public const string InstallNode = ActionBaseName + "InstallNode";
        public const string MatchNode = ActionBaseName + "MatchNode";
        public const string LinkObj = ActionBaseName + "LinkObj";
        public const string LinkItem = ActionBaseName + "LinkItem";
        public const string RopeObj = ActionBaseName + "RopeObj";
        public const string RotObj = ActionBaseName + "RotObj";
        public const string DragObj = ActionBaseName + "DragObj";

        public const string AnimHook = ActionBaseName + "AnimHook";
        public const string TimeHook = ActionBaseName + "TimeHook";
        
    }
}
