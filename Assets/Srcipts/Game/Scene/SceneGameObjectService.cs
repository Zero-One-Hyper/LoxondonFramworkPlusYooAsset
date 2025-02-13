using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Loxodon.Framework.Contexts;
using LP.Framework;
using UnityEngine;

public interface ISceneGameObjectService : IService
{
    void AddGameObject(string name, GameObject go);
    void TryGetSceneGameObject(string name, Action<GameObject> callback);
}

public class SceneGameObjectService : ISceneGameObjectService
{
    private ApplicationContext _context;

    private Dictionary<string, GameObject> _sceneGameObjects = new Dictionary<string, GameObject>();

    public void Init()
    {
        _context = Context.GetApplicationContext();
    }

    public void AddGameObject(string name, GameObject go)
    {
        if (!_sceneGameObjects.ContainsKey(name))
        {
            _sceneGameObjects.TryAdd(name, go);
        }
    }

    public void TryGetSceneGameObject(string name, Action<GameObject> callBack)
    {
        if (_sceneGameObjects.TryGetValue(name, out GameObject gameObject))
        {
            callBack?.Invoke(gameObject);
            return;
        }

        //加载物体
        var resService = _context.GetService<IAssetLoadUtil>();
        resService.LoadAssetAsync<GameObject>(name, handle =>
        {
            if (handle != null)
            {
                GameObject prefabGameObject = handle.InstantiateSync();
                prefabGameObject.name = name;
                prefabGameObject.transform.position = Vector3.zero;
                this.AddGameObject(name, prefabGameObject);
                XLog.I($"Prefab name is {prefabGameObject.name}");
                callBack?.Invoke(prefabGameObject);
            }
            else
            {
                XLog.E($"未找到预制体{name}");
            }
        });
    }

    public void LoadOfficeCence()
    {
        GameObject sceneGameObject = null;
        this.TryGetSceneGameObject("DigitalScene", gameObject => sceneGameObject = gameObject);


        //加载光照信息
        MeshLightingDataLoader lightingDataLoader = new MeshLightingDataLoader();
        lightingDataLoader.Init(sceneGameObject);

        //加载门 模型
        //var doorPrefab = resService.ResourcesLoad<GameObject>("Prefabs/Doors.Prefab");
        //var doorGo = GameObject.Instantiate(doorPrefab);
        //doorGo.name = "Doors";
        //this.AddGameObject("Doors", doorGo);
    }
}