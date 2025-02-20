using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Loxodon.Framework.Contexts;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LP.Framework
{
    public class ViewManager : IViewService
    {
        private IAssetLoadUtil _assetLoadUtil;
        public Dictionary<string, IViewBase> ViewDict { get; set; }
        public Dictionary<ViewLayer, Transform> LayerDict { get; set; }

        private List<GraphicRaycaster> _graphicRaycasters = new List<GraphicRaycaster>();
        private List<RaycastResult> _graphicRaycastResult = new List<RaycastResult>();

        private string _uiResPath = "Prefabs/UI/";

        public void Init()
        {
            InitLayer();
        }

        private void InitLayer()
        {
            ViewDict = new Dictionary<string, IViewBase>();
            var canvas = GameObject.FindObjectOfType<Canvas>()?.gameObject;
            if (canvas == null)
            {
                XLog.I("Canvas is null");
                return;
            }

            _graphicRaycasters.Add(canvas.gameObject.GetComponent<GraphicRaycaster>());

            LayerDict = new Dictionary<ViewLayer, Transform>();
            foreach (ViewLayer view in Enum.GetValues(typeof(ViewLayer)))
            {
                var name = view.ToString();
                var trans = canvas.transform.Find(name);
                LayerDict.Add(view, trans);
            }
        }

        public async Task<IViewBase> OpenView<T>(params object[] args) where T : IViewBase
        {
            //return OpenView<T>(_uiResPath, args, callBack);
            return await OpenView<T>(_uiResPath, args);
        }

        /// <summary>
        /// 打开面板
        /// </summary>
        /// <param name="path"></param>
        /// <param name="args"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<IViewBase> OpenView<T>(string path, params object[] args)
            where T : IViewBase
        {
            var name = typeof(T).ToString();
            if (ViewDict.ContainsKey(name))
            {
                IViewBase viewBase;
                if (ViewDict.TryGetValue(name, out viewBase))
                {
                    if (!viewBase.PanelObj.activeSelf)
                    {
                        viewBase.PanelObj.SetActive(true);
                        viewBase = viewBase.PanelObj.GetComponent<T>();
                        viewBase.InitUI(args);
                    }

                    return viewBase;
                }
            }


            if (_assetLoadUtil == null)
            {
                _assetLoadUtil = Context.GetApplicationContext().GetService<IAssetLoadUtil>();
            }

            //var uiGameObject = GameObject.Instantiate(Resources.Load<GameObject>(Path.Combine(path, name)));
            GameObject uiPrefab = await _assetLoadUtil.ResourceLoadAsync<GameObject>(name);
            GameObject uiGameObject = GameObject.Instantiate(uiPrefab);

            IViewBase view = uiGameObject.GetComponent<T>();
            view.InitUI(args);
            ViewDict.Add(name, view);
            view.PanelObj = uiGameObject;
            view.PanelObj.name = name;
            var trans = view.PanelObj.transform;
            var layer = (view as ViewBase).Layer;
            var transP = LayerDict[layer];
            trans.SetParent(transP, false);
            view.OnShow();
            return view;
            /*
            _assetLoadUtil.LoadAsset<GameObject>(name, handle =>
            {
                if (handle != null)
                {
                    if (ViewDict.TryGetValue(name, out IViewBase view))
                    {
                        return ;
                    }

                    GameObject uiGameObject = handle.InstantiateSync();

                    view = uiGameObject.GetComponent<T>();
                    view.InitUI(args);
                    ViewDict.Add(name, view);
                    view.PanelObj = uiGameObject;
                    view.PanelObj.name = name;
                    var trans = view.PanelObj.transform;
                    var layer = (view as ViewBase).Layer;
                    var transP = LayerDict[layer];
                    trans.SetParent(transP, false);
                    view.OnShow();
                    callBack?.Invoke(view);
                }
            });
            */
        }

        /// <summary>
        /// 关闭面板
        /// </summary>
        /// <param name="viewName"></param>
        public void CloseView(string viewName)
        {
            if (string.IsNullOrEmpty(viewName) || !ViewDict.ContainsKey(viewName))
                return;
            IViewBase v;
            ViewDict.TryGetValue(viewName, out v);
            v.OnClose();
            ViewDict.Remove(viewName);
            //GameObject.DestroyImmediate(v.PanelObj);
            GameObject.Destroy(v.PanelObj);
            Component.Destroy(v as ViewBase);
            Resources.UnloadUnusedAssets();
        }

        public void CloseView<T>() where T : IViewBase
        {
            var n = typeof(T).ToString();
            //Debug.Log($"关闭UI{n}");
            CloseView(n);
        }

        /// <summary>
        /// 激活显示面板
        /// </summary>
        /// <param name="viewName"></param>
        public void ShowView(string viewName)
        {
            if (string.IsNullOrEmpty(viewName) || !ViewDict.ContainsKey(viewName))
                return;
            IViewBase v = ViewDict[viewName];
            if (v == null)
                return;
            v.PanelObj.SetActive(true);
        }

        public void ShowView<T>() where T : IViewBase
        {
            var n = typeof(T).ToString();
            ShowView(n);
        }

        /// <summary>
        /// 隐藏面板
        /// </summary>
        /// <param name="viewName"></param>
        public void HideView(string viewName)
        {
            if (string.IsNullOrEmpty(viewName) || !ViewDict.ContainsKey(viewName))
                return;
            IViewBase v = ViewDict[viewName];
            if (v == null)
                return;
            v.PanelObj.SetActive(false);
        }

        public void HideView<T>() where T : IViewBase
        {
            var n = typeof(T).ToString();
            HideView(n);
        }

        /// <summary>
        /// 获取面板
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IViewBase GetView<T>() where T : IViewBase
        {
            var n = typeof(T).ToString();
            IViewBase v;
            ViewDict.TryGetValue(n, out v);
            return v;
        }

        //切换至省图模式
        public void EnterPanoramaModeSetUI()
        {
            IGuildTourService guildTourService = Context.GetApplicationContext().GetService<IGuildTourService>();
            guildTourService.OpenGuidlMap("false");
            CloseView<UIEmployeeDataChart>();
            CloseView<UIDoorPermission>();
            CloseView<UIMinMap>();
            CloseView<UITipLeftDown>();
            CloseView<UITip>();
        }

        public void EnterRoamingModeSetUI()
        {
            IGuildTourService guildTourService = Context.GetApplicationContext().GetService<IGuildTourService>();
            guildTourService.OpenMinMap();
        }

        public bool CheckMouseOnUI(Vector2 mousePosition)
        {
            _graphicRaycastResult.Clear();
            PointerEventData p = new PointerEventData(EventSystem.current) { position = mousePosition, };
            foreach (var cas in _graphicRaycasters)
            {
                cas.Raycast(p, _graphicRaycastResult);
            }

            return _graphicRaycastResult.Count > 0;
        }
    }
}