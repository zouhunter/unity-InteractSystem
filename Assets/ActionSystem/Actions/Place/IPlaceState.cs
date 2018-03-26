using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{
    public interface IPlaceState
    {
        ControllerType CtrlType { get; }
        void PlaceObject(PlaceObj pos, PlaceElement pickup);
        bool CanPlace(PlaceObj placeItem, PickUpAbleItem element, out string why);
        void PlaceWrong(PlaceElement pickup);
    }
}