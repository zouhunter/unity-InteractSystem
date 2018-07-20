using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace InteractSystem
{
    /// <summary>
    /// 预览参数
    /// </summary>
    public class PreviewSet
    {
        public Vector3 position;
        public Vector3 eulerAngle;
    }

    /// <summary>
    /// 预览控制器
    /// </summary>
    public class PreviewController
    {
        private static ActionSystem actionSystem;
        private Material _previewMaterail;
        protected Material previewmaterial {
            get
            {
                if(_previewMaterail == null)
                {
                    _previewMaterail = Config.Instence.previewMat;
                }
                if(_previewMaterail == null)
                {
                    _previewMaterail = new Material(Shader.Find("Standard"));
                }
                return _previewMaterail;
            }
        }
        private Dictionary<GameObject, List<GameObject>> actived = new Dictionary<GameObject, List<GameObject>>();
        private Dictionary<GameObject, Queue<GameObject>> pool = new Dictionary<GameObject, Queue<GameObject>>();
        private static float alpha { get { return Config.Instence.previewAlpha; } }
        private static PreviewController _instence;
        public static PreviewController Instence
        {
            get
            {

                if (_instence == null)
                {
                    _instence = new PreviewController(ActionSystem.Instence);
                }
                return _instence;
            }
        }
        public PreviewController(ActionSystem system)
        {
            actionSystem = system;
        }


        public void Notice(GameObject prefab, params PreviewSet[] sets)
        {
            if (prefab == null || sets == null || sets.Length == 0) return;

            var newItems = CreatePreivews(prefab, sets);

            if (!actived.ContainsKey(prefab))
            {
                actived.Add(prefab, newItems);
            }
            else
            {
                actived[prefab].AddRange(newItems);
            }
        }

        public void UnNotice(GameObject prefab)
        {
            if (prefab == null) return;

            if (actived.ContainsKey(prefab))
            {
                var instences = actived[prefab];
                foreach (var item in instences)
                {
                    if (item == null) continue;//有可能被销毁

                    if (!pool.ContainsKey(prefab))
                    {
                        pool[prefab] = new Queue<GameObject>();
                    }
                    item.gameObject.SetActive(false);
                    pool[prefab].Enqueue(item);
                }

                actived.Remove(prefab);
            }
        }

        public void UnNoticeAll()
        {
            var prefabs = new List<GameObject>(actived.Keys);
            foreach (var item in prefabs)
            {
                UnNotice(item);
            }
        }

        /// <summary>
        /// 按参数创建预览对象
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="sets"></param>
        /// <returns></returns>
        private List<GameObject> CreatePreivews(GameObject prefab, PreviewSet[] sets)
        {
            var lists = new List<GameObject>();
            foreach (var info in sets)
            {
                GameObject newObj = GetPreviewFromPool(prefab, info);
                if (newObj == null)
                {
                    newObj = CreatePreviewInternal(prefab, info);
                    ChargeMaterial(newObj, previewmaterial);
                }
                lists.Add(newObj);
            }

            return lists;
        }

        /// <summary>
        /// 从缓存池中加载对象
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="set"></param>
        /// <returns></returns>
        private GameObject GetPreviewFromPool(GameObject prefab, PreviewSet set)
        {
            GameObject instence = null;
            if (pool.ContainsKey(prefab) && pool[prefab].Count > 0)
            {
                instence = pool[prefab].Dequeue();
                if (instence)
                {
                    SetTranform(instence.transform, set);
                    instence.gameObject.SetActive(true);
                }
            }
            return instence;
        }
        /// <summary>
        /// 内部创建预制体
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="set"></param>
        /// <returns></returns>
        private static GameObject CreatePreviewInternal(GameObject prefab, PreviewSet set)
        {
            var renderObjs = prefab.GetComponentsInChildren<Renderer>();
            var instence = new GameObject(prefab.name);

            if(renderObjs.Length > 0)
            {
                foreach (var item in renderObjs)
                {
                    var render = Object.Instantiate(item);
                    render.transform.SetParent(instence.transform);
                }
            }
           
            SetTranform(instence.transform, set);
            ScaleInstence(instence, prefab);
            instence.transform.SetParent(actionSystem.transform);
            return instence;
        }

        private static void ScaleInstence(GameObject instence, GameObject prefab)
        {
            instence.transform.localScale = prefab.transform.localScale * 0.99f;
        }

        /// <summary>
        /// 设置物体的预览材质
        /// </summary>
        /// <param name="instence"></param>
        /// <param name="previewmaterial"></param>
        public static void ChargeMaterial(GameObject instence, Material previewmaterial)
        {
            var renders = instence.GetComponentsInChildren<Renderer>();
            if (renders != null && renders.Length > 0)
            {
                foreach (var render in renders)
                {
                    var newMaterials = new Material[render.materials.Length];
                    for (int i = 0; i < render.materials.Length; i++)
                    {
                        var oldMat = render.materials[i];
                        var newMat = new Material(previewmaterial);
                        #region Color
                        var color = oldMat.color;
                        //if (oldMat.HasProperty("_Color"))
                        //{
                        //    color = oldMat.GetColor("_Color");
                        //}
                        //else if(oldMat.HasProperty("_MainColor"))
                        //{
                        //    color = oldMat.GetColor("_MainColor");
                        //}
                        color.a = alpha;
                        //if(newMat.HasProperty("_MainColor"))
                        //{
                        //    newMat.SetColor("_MainColor", color);
                        //}
                        //if (newMat.HasProperty("_Color"))
                        //{
                        //    newMat.SetColor("_Color", color);
                        //}
                        newMat.color = color;
                        #endregion

                        #region Texture
                        Texture texture = oldMat.mainTexture;
                        //if (oldMat.HasProperty("_MainTex"))
                        //{
                        //    texture = oldMat.GetTexture("_MainTex");
                        //}
                        //else if (oldMat.HasProperty("_Texture"))
                        //{
                        //    texture = oldMat.GetTexture("_Texture");
                        //}

                        //if (newMat.HasProperty("_MainTex"))
                        //{
                        //    newMat.SetTexture("_MainTex", texture);
                        //}
                        //else if (newMat.HasProperty("_Texture"))
                        //{
                        //    newMat.SetTexture("_Texture", texture);
                        //}

                        newMat.mainTexture = texture;
                        #endregion

                        newMaterials[i] = newMat;
                    }
                    render.materials = newMaterials;
                }
            }
        }

        /// <summary>
        /// 设置物体的坐标
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="set"></param>
        public static void SetTranform(Transform obj, PreviewSet set)
        {
            obj.transform.position = set.position;
            obj.transform.eulerAngles = set.eulerAngle;
        }
    }
}