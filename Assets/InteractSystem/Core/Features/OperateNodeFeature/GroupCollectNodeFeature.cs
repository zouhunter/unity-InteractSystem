using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using InteractSystem.Actions;
using System;
using System.Linq;

namespace InteractSystem
{
    [System.Serializable]
    public class GroupCollectNodeFeature : CollectNodeFeature
    {
        public GroupCollectNodeFeature(Type type) : base(type) { }

        /// <summary>
        /// 从场景中找到已经存在的元素
        /// </summary>
        protected override void ActiveElements()
        {
            List<string> elementKeys = new List<string>();
            for (int i = 0; i < itemList.Count; i++)
            {
                var elementName = itemList[i];
                if (!elementKeys.Contains(elementName))
                {
                    elementKeys.Add(elementName);
                    var elements = elementCtrl.GetElements<ISupportElement>(elementName, false);
                    if (elements != null)
                    {
                        elements = elements.Where((x => SupportType(x.GetType()))).ToList();

                        foreach (var item in elements)
                        {
                            if (elementPool.Contains(item))
                            {
                                var activeAble = item as IActiveAble;
                                if (target.Statu == ExecuteStatu.Executing && activeAble.OperateAble)
                                {
                                    if (autoActive)
                                    {
                                        ActiveElement(activeAble);
                                    }
                                }
                            }
                            else
                            {
                                elementPool.ScureAdd(item);
                            }
                        }

                    }
                    else
                    {
                        Debug.Log("have no element name:" + elementName);
                    }

                }
            }
        }


        protected override void UnDoActivedElement()
        {
            ForEachElement((element)=> {
                var activeAble = element as IActiveAble;
                UndoElement(activeAble);
            });
        }

        protected override void InActivedElements()
        {
            ForEachElement((element) => {
                var activeAble = element as IActiveAble;
                SetInActiveElement(activeAble);
            });
        }

    }
}
