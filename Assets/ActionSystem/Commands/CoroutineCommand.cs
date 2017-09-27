using UnityEngine;

namespace WorldActionSystem
{
    public abstract class CoroutionCommand : ActionCommand
    {
        protected ICoroutineCtrl coroutineCtrl;
        protected Coroutine coroutine;

        protected abstract ICoroutineCtrl CreateCtrl();

        public override void StartExecute(bool forceAuto)
        {
            base.StartExecute(forceAuto);

            if (coroutineCtrl == null)
                coroutineCtrl = CreateCtrl();

            coroutineCtrl.StartExecute(forceAuto);
            if (coroutine == null){
                coroutine = StartCoroutine(coroutineCtrl.Update());
            }
        }
        public override void EndExecute()
        {
            base.EndExecute();
            if (coroutineCtrl == null) return;
            coroutineCtrl.EndExecute();
            if (coroutine != null){
                StopCoroutine(coroutineCtrl.Update());
            }
        }
        public override void UnDoExecute()
        {
            base.UnDoExecute();

            if (coroutineCtrl == null) return;
            coroutineCtrl.UnDoExecute();
            if (coroutine != null)
            {
                StopCoroutine(coroutineCtrl.Update());
            }
        }
    }
}