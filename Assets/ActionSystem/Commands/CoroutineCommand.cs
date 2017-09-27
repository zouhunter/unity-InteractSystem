using UnityEngine;

namespace WorldActionSystem
{
    public abstract class CoroutionCommand : IActionCommand
    {
        public string StepName { get { return stepName; } }
        protected ActionCommand trigger { get; set; }
        protected ICoroutineCtrl coroutineCtrl;
        protected Coroutine coroutine;
        private string stepName;

        public virtual void InitCommand(string stepName,ActionCommand trigger)
        {
            this.stepName = stepName;
            this.trigger = trigger;
        }

        protected abstract ICoroutineCtrl CreateCtrl();

        public virtual void StartExecute(bool forceAuto)
        {
            if (coroutineCtrl == null)
                coroutineCtrl = CreateCtrl();

            coroutineCtrl.StartExecute(forceAuto);
            if (coroutine == null){
                coroutine = trigger.StartCoroutine(coroutineCtrl.Update());
            }
        }
        public virtual void EndExecute()
        {
            if (coroutineCtrl == null) return;
            coroutineCtrl.EndExecute();
            if (coroutine != null)
            {
                trigger.StopCoroutine(coroutineCtrl.Update());
            }
        }
        public virtual void UnDoExecute()
        {
            if (coroutineCtrl == null) return;
            coroutineCtrl.UnDoExecute();
            if (coroutine != null)
            {
                trigger.StopCoroutine(coroutineCtrl.Update());
            }
        }
    }
}