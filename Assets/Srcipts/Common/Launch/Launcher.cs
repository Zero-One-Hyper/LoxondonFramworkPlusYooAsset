using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Loxodon.Framework;
using Loxodon.Framework.Asynchronous;
using Loxodon.Framework.Binding;
using Loxodon.Framework.Contexts;
using Loxodon.Framework.Security.Cryptography;
using Loxodon.Framework.Services;
using Loxodon.Framework.Views;
using LP.Framework;
using YooAsset;
using UnitData.Const;

public class Launcher : MonoBehaviour
{
    private IAppRun _appRun;

    private void Awake()
    {
        GlobalWindowManagerBase windowManagerBase = FindObjectOfType<GlobalWindowManagerBase>();
        if (windowManagerBase == null)
            XLog.E("not found the GlobalWindowManager");
        var context = Context.GetApplicationContext(); //获取应用上下文
        IServiceContainer container = context.GetContainer(); //服务容器
        BindingServiceBundle bundle = new BindingServiceBundle(container);
        bundle.Start(); //开启数据绑定服务
        //UI
        IUIViewLocator viewLocator = new ResourcesViewLocator();
        container.Register(viewLocator);

        //Http请求
        IHttpService httpService = new HttpControl();
        container.Register(httpService);

        //事件
        IEventHandleService eventHandleService = new EventManager();
        eventHandleService.Init();
        container.Register(eventHandleService);

        //资源加载辅助类
        IAssetLoadUtil loadUtil = new LoadAssetMgr();
        container.Register(loadUtil);
        //UI
        IViewService viewService = new ViewManager();
        viewService.Init();
        container.Register(viewService);
        //Bundle
#if Bundle
        IAssetBundleService bundleService = new AssetBundleManager();
#if UNITY_EDITOR
        bundleService.EPMode = EPlayMode.EditorSimulateMode;
#else
        bundleService.EPMode = EPlayMode.WebPlayMode;
#endif
        container.Register(bundleService);
        bundleService.Init();
#endif
        //游戏启动
        _appRun = new AppRun();
        _appRun.StartUp();

        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
#if UNITY_EDITOR
        this.gameObject.AddComponent<Test>();
#endif
        //向web发送unity加载完成消息
        var webService = Context.GetApplicationContext().GetService<IWebService>();
        webService.UnityCallWeb(Constant.UNITY_CALL_LOAD_COMPLETE, "");
        XLog.I("unity加载完成");
    }

    private void OnDestroy()
    {
        _appRun.Stop();
    }
}