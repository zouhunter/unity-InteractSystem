using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;
//using System.Tuples;

public class InstallObjectPosSetter : MonoBehaviour
{
    [System.Serializable]
    public class PosTemp
    {
        [System.Serializable]
        public class TransformTemp
        {
            public Vector3 position;
            public Vector3 eular;
            public Vector3 size;
        }

        public string key;
        public List<TransformTemp> objTransforms = new List<TransformTemp>();
    }

    public List<Transform> objectList = new List<Transform>();
    public List<PosTemp> switchList = new List<PosTemp>();
    void Start()
    {
        string key = "defult";

        PosTemp data = switchList.Find(x => x.key == key);
        Transform titem;
        if (data != null)
        {
            for (int i = 0; i < objectList.Count; i++)
            {
                titem = objectList[i];
                var item = data.objTransforms[i];
                titem.localPosition = item.position;
                titem.localEulerAngles = item.eular;
                titem.localScale = item.size;
            }
        }
    }
}
