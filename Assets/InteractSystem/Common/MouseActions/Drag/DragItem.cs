using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InteractSystem.Actions
{

    public class DragItem : ActionItem
    {
        public override bool OperateAble
        {
            get
            {
                return targets == null || targets.Count == 0;
            }
        }

        public Vector3 targetPos { get; private set; }
        public Vector3 startPos { get; private set; }

        [SerializeField, Attributes.CustomField("目标点")]
        private Transform targetHolder;
        [SerializeField, Attributes.CustomField("坍缩时间")]
        private float clampTime = 0.2f;
        [SerializeField,Attributes.CustomField("范围限制")]
        private bool clampHard;
        private float autoDragTime { get { return Config.Instence.autoExecuteTime; } }
        private bool auto;
        private CoroutineController coroutineCtrl { get { return CoroutineController.Instence; } }
        public ClickAbleFeature clickAbleFeature = new ClickAbleFeature();
        public CompleteAbleItemFeature completeAbleFeature = new CompleteAbleItemFeature();
        public const string layerName = "i:dragitem";

        protected override void Awake()
        {
            base.Awake();
            auto = false;
        }

        protected override List<ActionItemFeature> RegistFeatures()
        {
            var features = base.RegistFeatures();

            clickAbleFeature.Init(this, layerName);
            features.Add(clickAbleFeature);

            completeAbleFeature.Init(this, AutoExecute);
            features.Add(completeAbleFeature);

            return features;
        }
        protected override void Start()
        {
            base.Start();
            InitPositions();
        }
        public  void AutoExecute(Graph.OperaterNode node)
        {
            coroutineCtrl.StartCoroutine (AutoDrag());
        }

        IEnumerator AutoDrag()
        {
            auto = true;
            for (float i = 0; i < autoDragTime; i += Time.deltaTime)
            {
                transform.localPosition = Vector3.Lerp(startPos, targetPos, i / autoDragTime);
                yield return null;
            }
           completeAbleFeature. OnComplete(firstLock);
        }
        protected override void OnSetInActive(UnityEngine.Object target)
        {
            base.OnSetInActive(target);
            if (auto){
                coroutineCtrl.StopCoroutine(AutoDrag());
            }
            transform.localPosition = targetPos;
        }
        public override void UnDoChanges(UnityEngine.Object target)
        {
            base.UnDoChanges(target);
            if (auto) {
                coroutineCtrl.StopCoroutine(AutoDrag());
            }
            transform.localPosition = startPos;
        }

        internal void Clamp()
        {
            if (Vector3.Dot(transform.localPosition - startPos, targetPos - startPos) < 0)
            {
                if (gameObject.activeInHierarchy)
                    StartCoroutine(ClampInternal(startPos));
            }
            else if (Vector3.Distance(transform.localPosition, startPos) > Vector3.Distance(targetPos, startPos))
            {
                if (gameObject.activeInHierarchy)
                    StartCoroutine(ClampInternal(targetPos));
            }
            else
            {
                TryTrigger();
            }
        }

        IEnumerator ClampInternal(Vector3 pos)
        {
            var s_pos = transform.localPosition;
            for (float i = 0; i < clampTime; i += Time.deltaTime)
            {
                transform.localPosition = Vector3.Lerp(s_pos, pos, i / clampTime);
                yield return null;
            }
            TryTrigger();
        }

        private void TryTrigger()
        {
            if (Vector3.Distance(transform.localPosition, targetPos) < 0.2f)
            {
                completeAbleFeature.OnComplete(firstLock);
            }
        }

        internal void TryMove(Vector3 vector3)
        {
            if (clampHard)
            {
                var newpos = transform.localPosition + vector3;
                if (Vector3.Distance(newpos, startPos) > Vector3.Distance(targetPos, startPos))
                {
                    return;
                }
                if (Vector3.Dot(newpos - startPos, targetPos - startPos) < 0)
                {
                    return;
                }
            }
            transform.localPosition += vector3;
        }
        private void InitPositions()
        {
            startPos = transform.localPosition;
            targetPos = startPos + transform.InverseTransformPoint( targetHolder.position);
        }
    }

}