using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace InteractSystem.Actions
{
    /// <summary>
    /// (暂时没有考虑不足和溢出的问题)
    /// </summary>
    [NodeGraph.CustomNode("Operate/Charge", 11, "InteractSystem")]
    public class ChargeObj : Graph.OperaterNode
    {
        public QueueCollectNodeFeature completeAbleFeature = new QueueCollectNodeFeature(typeof(ChargeItem));

        protected override List<OperateNodeFeature> RegistFeatures()
        {
            var features = base.RegistFeatures();
            completeAbleFeature.SetTarget(this);
            completeAbleFeature.onActiveElement += OnActiveElement;
            completeAbleFeature.onUnDoElement += OnUnDoElement;
            completeAbleFeature.onInActiveElement += OnCompleteElement;
            features.Add(completeAbleFeature);
            return features;
        }

        private void OnCompleteElement(IActiveAble arg0)
        {
            CompleteSubElements(arg0 as ChargeItem, false);
        }

        private void OnUnDoElement(IActiveAble arg0)
        {
            CompleteSubElements(arg0 as ChargeItem, true);
        }

        private void OnActiveElement(IActiveAble arg0)
        {
            ActiveSubElements(arg0 as ChargeItem);
        }

        public override void OnStartExecute(bool auto = false)
        {
            base.OnStartExecute(auto);
            ChargeCtrl.Instence.RegistLock(this);
        }
        public override void OnUnDoExecute()
        {
            base.OnUnDoExecute();
            ChargeCtrl.Instence.RemoveLock(this);
        }
        public override void OnEndExecute(bool force)
        {
            base.OnEndExecute(force);
            ChargeCtrl.Instence.RemoveLock(this);
        }


        private void ActiveSubElements(ChargeItem element)
        {
            var actived = completeAbleFeature.elementPool.Find(x => x as ChargeItem != element && x.Name == element.Name);

            if (actived == null)
            {
                var tools = ElementController.Instence.GetElements<ChargeTool>();
                if (tools != null)
                {
                    for (int i = 0; i < tools.Count; i++)
                    {
                        if (element.completeDatas.FindAll(y => tools[i].CanLoad(y.type)).Count == 0) return;

                        if (log) Debug.Log("ActiveElements:" + element.Name + (!tools[i].Actived));

                        if (!tools[i].Actived)
                        {
                            tools[i].SetActive(this);
                        }
                    }
                }

                var resources = ElementController.Instence.GetElements<ChargeResource>();
                if (resources != null)
                {
                    for (int i = 0; i < resources.Count; i++)
                    {
                        if (element.completeDatas.FindAll(y => y.type == resources[i].type).Count == 0) continue;

                        if (log) Debug.Log("ActiveElements:" + element.Name + (!resources[i].Actived));

                        if (!resources[i].Actived)
                        {
                            resources[i].SetActive(this);
                        }
                    }
                }

            }

            if (!completeAbleFeature.elementPool.Contains(element)) completeAbleFeature.elementPool.Add(element);
        }


        private void CompleteSubElements(ChargeItem element, bool undo)
        {
            completeAbleFeature.elementPool.Remove(element);
            var active = completeAbleFeature.elementPool.Find(x => x.Name == element.Name);

            if (active == null)
            {
                var tools = ElementController.Instence.GetElements<ChargeTool>();
                if (tools != null)
                {
                    for (int i = 0; i < tools.Count; i++)
                    {
                        if (log) Debug.Log("CompleteElements:" + element.Name + tools[i].Actived);

                        if (element.completeDatas.FindAll(y => tools[i].CanLoad(y.type)).Count == 0) return;

                        if (tools[i].Actived)
                        {
                            if (undo)
                            {
                                tools[i].UnDoChanges(this);
                            }
                            else
                            {
                                tools[i].SetInActive(this);
                            }
                        }
                    }
                }

                var resources = ElementController.Instence.GetElements<ChargeResource>();
                if (resources != null)
                {
                    for (int i = 0; i < resources.Count; i++)
                    {
                        if (log) Debug.Log("CompleteElements:" + element.Name + resources[i].Actived);

                        if (element.completeDatas.FindAll(y => y.type == resources[i].type).Count == 0) continue;

                        if (resources[i].Actived)
                        {
                            if (undo)
                            {
                                resources[i].UnDoChanges(this);
                            }
                            else
                            {
                                resources[i].SetInActive(this);
                            }
                        }
                    }
                }
            }


        }

    }

}