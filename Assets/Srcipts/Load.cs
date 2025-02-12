using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;

/// <summary>
/// 脚本工作流程：
/// 1.下载资源，用yooAsset资源框架进行下载
///    1.资源文件，ab包
/// </summary>
public class Load : MonoBehaviour
{
    [SerializeField] private EPlayMode playMode = EPlayMode.WebPlayMode;
    private ResourcePackage _defaultResourcePackage;

    string _defaultPackageName = "DefaultPackage";

    private void Start()
    {
#if UNITY_WEBGL
        playMode = EPlayMode.WebPlayMode;
#endif
        StartCoroutine(InitYooAsset(StartGame));
    }

    private void StartGame()
    {
        StartCoroutine(LoadLauncher());
    }

    private IEnumerator LoadLauncher()
    {
        //实例化assetbundle的资源，还原资源上的热更脚本
        var package = YooAssets.GetPackage(_defaultPackageName);


        //AssetInfo[] assetInfos = package.GetAllAssetInfos();
        //foreach (var assetInfo in assetInfos)
        //{
        //    Debug.Log($"让我看看{assetInfo.AssetPath}");
        //}

        var handle = package.LoadAssetAsync<GameObject>("DefaultGameObject");
        yield return handle;
        handle.Completed += HandleCompleted;
    }

    private void HandleCompleted(AssetHandle handle)
    {
        GameObject go = handle.InstantiateSync();
        Debug.Log($"Prefab name is {go.name}");
    }

    IEnumerator InitYooAsset(Action onDownloadCompleteCallBack)
    {
        //初始化主资源
        YooAssets.Initialize();
        var package = YooAssets.TryGetPackage(_defaultPackageName);
        if (package == null)
        {
            package = YooAssets.CreatePackage(_defaultPackageName);
        }

        YooAssets.SetDefaultPackage(package);
        InitializationOperation initializationOperation = null;

        string defaultHostServer = GetHostServerURL();
        string fallbackHostServer = GetHostServerURL();
        RemoteService remoteServices = new RemoteService(defaultHostServer, fallbackHostServer);

        if (playMode == EPlayMode.EditorSimulateMode)
        {
            //Editor模式
            var simulateBuildResult = EditorSimulateModeHelper.SimulateBuild(_defaultPackageName);
            var packageRoot = simulateBuildResult.PackageRootDirectory;
            var editorParameters = new EditorSimulateModeParameters();
            editorParameters.EditorFileSystemParameters =
                FileSystemParameters.CreateDefaultEditorFileSystemParameters(packageRoot);
            initializationOperation = package.InitializeAsync(editorParameters);
        }
        else if (playMode == EPlayMode.HostPlayMode)
        {
            //联机模式
            var hostPlayModeParameters = new HostPlayModeParameters();
            var cacheFileSystemParams = FileSystemParameters.CreateDefaultCacheFileSystemParameters(remoteServices);
            var buildinFileSystemParams = FileSystemParameters.CreateDefaultBuildinFileSystemParameters();

            hostPlayModeParameters.BuildinFileSystemParameters = buildinFileSystemParams;
            hostPlayModeParameters.CacheFileSystemParameters = cacheFileSystemParams;
            initializationOperation = package.InitializeAsync(hostPlayModeParameters);
        }
        else if (playMode == EPlayMode.WebPlayMode)
        {
            //web模式
            var webPlayModeParameters = new WebPlayModeParameters();
            webPlayModeParameters.WebServerFileSystemParameters =
                FileSystemParameters.CreateDefaultWebServerFileSystemParameters();

            var webServerFileSystemParams = FileSystemParameters.CreateDefaultWebServerFileSystemParameters();
            var webRemoteFileSystemParams =
                FileSystemParameters.CreateDefaultWebRemoteFileSystemParameters(remoteServices); //支持跨域下载

            webPlayModeParameters.WebServerFileSystemParameters = webServerFileSystemParams;
            webPlayModeParameters.WebRemoteFileSystemParameters = webRemoteFileSystemParams;
            initializationOperation = package.InitializeAsync(webPlayModeParameters);
        }

        yield return initializationOperation;

        if (initializationOperation.Status == EOperationStatus.Succeed)
        {
            Debug.Log("资源包初始化成功！");
        }
        else
        {
            Debug.LogError($"资源包初始化失败：{initializationOperation.Error}");
        }

        //2.获取最新资源版本
        var operation = package.RequestPackageVersionAsync();
        yield return operation;
        Debug.Log($"最新版本为：{operation.PackageVersion}");
        if (operation.Status != EOperationStatus.Succeed)
        {
            Debug.LogError(operation.Error);
            yield break;
        }

        string packageVersion = operation.PackageVersion;
        Debug.Log($"Updated package version: {packageVersion}");

        //3.更新补丁清单
        //更新成功后自动保存版本号，作为下次初始化的版本
        // 也可以通过operation.SavePackageVersion()方法保存
        remoteServices.SetLatestVersion(packageVersion); //为远端资源地址查询服务类 设置版本 用于定位不同版本的包
        var updateOperation = package.UpdatePackageManifestAsync(packageVersion);
        yield return updateOperation;

        if (updateOperation.Status != EOperationStatus.Succeed)
        {
            //更新失败
            Debug.LogError(updateOperation.Error);
            yield break;
        }

        yield return Download();

        _defaultResourcePackage = package;
        onDownloadCompleteCallBack();
    }

    private string GetHostServerURL()
    {
        //模拟下载地址，88为Nginx里面设置的端口号，项目名，平台名
        string hostServerIP = "http://192.168.31.18:88/BundleTest";
#if UNITY_EDITOR
        if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.Android)
            return $"{hostServerIP}/Bundle/Android";
        else if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.iOS)
            return $"{hostServerIP}/Bundle/IPhone";
        else if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.WebGL)
            return $"{hostServerIP}/Bundle/WebGL";
        else
            return $"{hostServerIP}/Bundle/PC";
#else
        if (Application.platform == RuntimePlatform.Android)
            return $"{hostServerIP}/Bundle/Android";
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
            return $"{hostServerIP}/Bundle/IPhone";
        else if (Application.platform == RuntimePlatform.WebGLPlayer)
            return $"{hostServerIP}/Bundle/WebGL";
        else
            return $"{hostServerIP}/Bundle/PC";
#endif
    }

    /// <summary>
    /// 远端资源地址查询服务类
    /// 一路传递到ReqyestWebRemotePackageVersionOperation。GetWebRequestURL
    /// </summary>
    private class RemoteService : IRemoteServices
    {
        private readonly string _defaultHostServer;
        private readonly string _fallBackHostServer;
        private string _latestVersion;

        public RemoteService(string defaultHostServer, string fallBackHostServer)
        {
            this._defaultHostServer = defaultHostServer;
            this._fallBackHostServer = fallBackHostServer;
        }

        public void SetLatestVersion(string latestVersion)
        {
            this._latestVersion = latestVersion;
        }

        public string GetRemoteMainURL(string fileName)
        {
            if (fileName.Contains("version"))
            {
                return _defaultHostServer + '/' + "Version" + '/' + fileName;
            }

            return _defaultHostServer + '/' + _latestVersion + '/' + fileName;
        }

        public string GetRemoteFallbackURL(string fileName)
        {
            if (fileName.Contains("version"))
            {
                return _defaultHostServer + '/' + "Version" + '/' + fileName;
            }

            return _fallBackHostServer + '/' + _latestVersion + '/' + fileName;
        }
    }

    //下载热更资源
    private IEnumerator Download()
    {
        int downloadMaxNum = 10;
        int faildTryAgain = 3;
        var package = YooAssets.GetPackage("DefaultPackage");
        if (package == null)
        {
            Debug.LogError("空");
        }

        var downloader = package.CreateResourceDownloader(downloadMaxNum, faildTryAgain);

        //没有需要下载的资源
        if (downloader.TotalDownloadCount == 0)
        {
            Debug.Log("没有需要下载的资源");
            yield break;
        }

        //需要下载的文件总数大小
        int totalDownloadCount = downloader.TotalDownloadCount;
        long totalDownloadBytes = downloader.TotalDownloadBytes;

        //注册回调
        downloader.DownloadErrorCallback = OnDownloadErrorFunction;
        downloader.DownloadUpdateCallback = OnDownloadUpdateFunction;
        downloader.DownloadFinishCallback = OnDownloadFinishFunction;
        downloader.DownloadFileBeginCallback = OnStartDownloadFileFunction;

        //开启下载
        downloader.BeginDownload();
        yield return downloader;

        //检测下载结果
        if (downloader.Status == EOperationStatus.Succeed)
        {
            //下载成功
            Debug.Log("更新完成");
        }
        else
        {
            //下载失败
            Debug.Log("更新失败");
        }
    }

    private void OnStartDownloadFileFunction(DownloadFileData data)
    {
        //开始下载
        Debug.Log($"开始下载：文件名：{data.FileName}，文件大小：{data.FileSize}");
    }

    private void OnDownloadFinishFunction(DownloaderFinishData data)
    {
        //下载结束（无论成功或失败）
        Debug.Log("下载" + (data.Succeed ? "成功" : "失败"));
    }

    private void OnDownloadUpdateFunction(DownloadUpdateData data)
    {
        //下载进度发生变化
        Debug.Log($"文件总数：{data.TotalDownloadCount}，已下载文件数：{data.CurrentDownloadCount}，" +
                  $"下载总大小：{data.TotalDownloadBytes}，已下载大小{data.CurrentDownloadBytes}");
    }

    private void OnDownloadErrorFunction(DownloadErrorData data)
    {
        //下载发生错误
        Debug.Log($"下载出错：包名:{data.PackageName} 文件名：{data.FileName}，错误信息：{data.ErrorInfo}");
    }
}