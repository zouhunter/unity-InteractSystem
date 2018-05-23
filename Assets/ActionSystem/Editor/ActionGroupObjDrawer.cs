//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.Events;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEditor;
//using UnityEditorInternal;

//namespace WorldActionSystem
//{
//    [CustomEditor(typeof(ActionGroupObj)), CanEditMultipleObjects]
//    public class ActionGroupObjDrawer : ActionGroupDrawerBase
//    {
//        protected override List<AutoPrefabItem> GetAutoPrefabs()
//        {
//            var group = target as ActionGroupObj;
//            return group.autoLoadElement;
//        }
//        protected override List<RunTimePrefabItem> GetRunTimePrefabs()
//        {
//            var group = target as ActionGroupObj;
//            return group.runTimeElements;
//        }
//    }

//}