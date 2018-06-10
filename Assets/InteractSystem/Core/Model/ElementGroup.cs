using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace InteractSystem
{
    public class ElementGroup : ScriptableObject
    {
        [SerializeField]//用户创建元素
        protected RunTimePrefabItem[] runTimeElements;
        [SerializeField]//自动创建元素
        protected AutoPrefabItem[] autoElements;
        [SerializeField]//环境元素
        protected Enviroment.EnviromentItem[] enviroments;
        protected Transform context;
        protected AutoElementCtrl autoElementCtrl;
        public void SetActive(Transform context)
        {
            this.context = context;
            autoElementCtrl = new AutoElementCtrl(context, autoElements);
            autoElementCtrl.Create();
            Enviroment.EnviromentCtrl.Instence.RegistElements(enviroments);
            ElementController.Instence.RegistRunTimeElements(runTimeElements);
        }
        public void SetInActive()
        {
            ElementController.Instence.RemoveRunTimeElements(runTimeElements);
            Enviroment.EnviromentCtrl.Instence.RemoveElements(enviroments);
            autoElementCtrl.Clear();
        }
    }
}
