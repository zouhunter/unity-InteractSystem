using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace WorldActionSystem
{
    public abstract class PlaceItem : ActionObj
    {
        public virtual string Name { get { return name; } }
        public virtual GameObject Go { get { return gameObject; } }
        public virtual bool AlreadyPlaced { get { return obj != null; } }
        public abstract int layer { get; }
        public virtual PickUpAbleElement obj { get; protected set; }
        public virtual void Attach(PickUpAbleElement obj)
        {
            this.obj = obj;
        }
        public virtual PickUpAbleElement Detach()
        {
            PickUpAbleElement old = obj;
            obj = default(PickUpAbleElement);
            return old;
        }
        protected virtual void Awake()
        {
            gameObject.layer = layer;
        }
    }

    public abstract class PlaceController : IActionCtroller
    {
        public UnityAction<string> UserError { get; set; }
        protected IHighLightItems highLight;
        protected PickUpAbleElement pickedUpObj;
        protected bool pickedUp;
        protected PlaceItem installPos;
        protected Ray ray;
        protected RaycastHit hit;
        protected RaycastHit[] hits;
        protected bool installAble;
        protected string resonwhy;
        protected float hitDistence { get { return Setting.hitDistence; } }
        protected List<PlaceItem> placeitems = new List<PlaceItem>();
        protected Camera viewCamera { get { return CameraController.ActiveCamera; } }
        protected bool activeNotice { get { return Setting.highLightNotice; } }
        protected Ray disRay;
        protected RaycastHit disHit;
        protected float elementDistence;
        protected abstract int PlacePoslayerMask { get; }//1 << Setting.installPosLayer

        public PlaceController(PlaceItem[] placeitems)
        {
            highLight = new ShaderHighLight();
            this.placeitems.AddRange(placeitems);
        }

        #region 鼠标操作事件
        public virtual void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                OnLeftMouseClicked();
            }
            else if (pickedUp)
            {
                UpdatePlaceState();
                MoveWithMouse(elementDistence += Input.GetAxis("Mouse ScrollWheel"));
            }
        }

        protected virtual void OnLeftMouseClicked()
        {
            if (!pickedUp)
            {
                SelectAnElement();
            }
            else
            {
                TryPlaceObject();
            }
        }

        /// <summary>
        /// 在未屏幕锁的情况下选中一个没有元素
        /// </summary>
        void SelectAnElement()
        {
            ray = viewCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, hitDistence, (1 << Setting.pickUpElementLayer)))
            {
                pickedUpObj = hit.collider.GetComponent<PickUpAbleElement>();
                if (pickedUpObj != null)
                {
                    if (pickedUpObj.Installed){
                        pickedUpObj.NormalUnInstall();
                    }
                    pickedUpObj.OnPickUp();
                    pickedUp = true;
                    elementDistence = Vector3.Distance(viewCamera.transform.position, pickedUpObj.transform.position);
                }
            }
        }

        public void UpdatePlaceState()
        {
            ray = viewCamera.ScreenPointToRay(Input.mousePosition);
            hits = Physics.RaycastAll(ray, hitDistence, PlacePoslayerMask);
            if (hits != null || hits.Length > 0)
            {
                bool hited = false;
                for (int i = 0; i < hits.Length; i++)
                {
                    if (hits[i].collider.name == pickedUpObj.name)
                    {
                        hited = true;
                        installPos = hits[i].collider.GetComponent<PlaceItem>();
                        installAble = CanPlace(installPos, pickedUpObj, out resonwhy);
                    }
                }
                if (!hited)
                {
                    installAble = false;
                    resonwhy = "零件放置位置不正确";
                }
            }

            if (installAble)
            {
                //可安装显示绿色
                if (activeNotice) highLight.HighLightTarget(pickedUpObj.Render, Color.green);
            }
            else
            {
                //不可安装红色
                if (activeNotice) highLight.HighLightTarget(pickedUpObj.Render, Color.red);
            }
        }

        /// <summary>
        /// 尝试安装元素
        /// </summary>
        void TryPlaceObject()
        {
            ray = viewCamera.ScreenPointToRay(Input.mousePosition);
            if (installAble)
            {
                PlaceObject(installPos, pickedUpObj);
            }
            else
            {
                PlaceWrong(pickedUpObj);
                UserError(resonwhy);
            }

            pickedUp = false;
            installAble = false;
            if (activeNotice) highLight.UnHighLightTarget(pickedUpObj.Render);
        }

        protected abstract bool CanPlace(PlaceItem placeItem, PickUpAbleElement element, out string why);

        protected abstract void PlaceObject(PlaceItem pos, PickUpAbleElement pickup);

        protected abstract void PlaceWrong(PickUpAbleElement pickup);

        /// <summary>
        /// 跟随鼠标
        /// </summary>
        void MoveWithMouse(float dis)
        {
            disRay = viewCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(disRay, out disHit, dis, 1 << Setting.obstacleLayer))
            {
                pickedUpObj.transform.position = disHit.point;
            }
            else
            {
                pickedUpObj.transform.position = viewCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, dis));
            }
        }

        #endregion

        public void OnStartExecute(bool forceauto)
        {
            SetStartNotify();
        }
        public void OnEndExecute()
        {
            SetCompleteNotify(false);
        }
        public void OnUnDoExecute()
        {
            SetCompleteNotify(true);
        }

        /// <summary>
        /// 将可安装元素全部显示出来
        /// </summary>
        private void SetStartNotify()
        {
            var keyList = new List<string>();
            foreach (var pos in placeitems)
            {
                if (!keyList.Contains(pos.Name))
                {
                    keyList.Add(pos.Name);
                    List<PickUpAbleElement> listObjs = ElementController.GetElements(pos.Name);
                    if (listObjs == null) throw new UnityException("元素配制错误:没有:" + pos.Name);
                    for (int j = 0; j < listObjs.Count; j++)
                    {
                        if (!listObjs[j].Installed)
                        {
                            listObjs[j].StepActive();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 结束指定步骤
        /// </summary>
        /// <param name="poss"></param>
        private void SetCompleteNotify(bool undo)
        {
            var keyList = new List<string>();
            foreach (var pos in placeitems)
            {
                if (!keyList.Contains(pos.Name))
                {
                    List<PickUpAbleElement> listObjs = ElementController.GetElements(pos.Name);
                    if (listObjs == null) throw new UnityException("元素配制错误:没有:" + pos.Name);
                    for (int j = 0; j < listObjs.Count; j++)
                    {
                        if (!listObjs[j].Installed && undo)
                        {
                            listObjs[j].StepUnDo();
                        }
                        else
                        {
                            listObjs[j].StepComplete();
                        }
                    }
                }
            }
        }
    }
}
