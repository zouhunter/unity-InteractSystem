using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using InteractSystem.Graph;

namespace InteractSystem
{
    public static class CoodinateUtil
    {
        private static Dictionary<Transform, Coordinate> coordinateDic;
        public static Coordinate GetCoordinate(Transform target)
        {
            if (coordinateDic == null){
                coordinateDic = new Dictionary<Transform, Coordinate>();
            }
            if (!coordinateDic.ContainsKey(target)){
                coordinateDic.Add(target, new Coordinate());
            }

            var coordinate = coordinateDic[target];
            coordinate.eulerAngles = target.eulerAngles;
            coordinate.position = target.position;
            coordinate.localScale = target.localScale;
            return coordinate;
        }
    }
}