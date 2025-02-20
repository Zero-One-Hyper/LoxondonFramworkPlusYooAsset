using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Threading.Tasks;
using Loxodon.Framework.Contexts;
using LP.Framework;
using UnityEngine;
using YooAsset;

public interface ISceneGameObjectService : IService
{
    void AddGameObject(string name, GameObject go);
    Task<GameObject> TryGetSceneGameObject(string name);
    void LoadOfficeCence();
    void LoadProvincialMap();
    void UnLoadProvincialMap();
}

public class SceneGameObjectService : ISceneGameObjectService
{
    private ApplicationContext _context;

    private Dictionary<string, GameObject> _sceneGameObjects = new Dictionary<string, GameObject>();
    private List<string> _sceneGameObjectNameList = new List<string>();

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

    public async Task<GameObject> TryGetSceneGameObject(string name)
    {
        if (_sceneGameObjectNameList.Contains(name))
        {
            //Debug.Log($"包括 {name}");
            //所有场景GameObject列表包含name，等待加载完成
            while (!_sceneGameObjects.ContainsKey(name))
            {
                //延时几秒
                //Debug.Log("延时");
                await Task.Delay(50);
            }
            return _sceneGameObjects[name];
        }

        var resService = _context.GetService<IAssetLoadUtil>();

        _sceneGameObjectNameList.Add(name);
        GameObject prefab = await resService.ResourceLoadAsync<GameObject>(name);
        GameObject go = GameObject.Instantiate(prefab);
        //Debug.Log($"实例化 {name}");
        go.name = name;
        //go.transform.position = Vector3.zero;
        this.AddGameObject(name, go);
        return go;
    }


    public async void LoadOfficeCence()
    {
        var resService = _context.GetService<IAssetLoadUtil>();
        //加载场景
        GameObject officeScene = await TryGetSceneGameObject("DigitalScene");
        //GameObject OfficeScene = GameObject.Instantiate(digitalScenePrefab);

        officeScene.name = "OfficeScene";
        this.AddGameObject("OfficeScene", officeScene);
        //为场景加载光照信息
        MeshLightingDataLoader lightingDataLoader = new MeshLightingDataLoader();
        lightingDataLoader.Init(officeScene);

        //加载门
        GameObject door = await TryGetSceneGameObject("DigitalScene");
        //GameObject door = GameObject.Instantiate(doorPrefab);
        door.name = "Doors";
        this.AddGameObject("Doors", door);

        //加载Collider
        GameObject doorColliderGameObject = await TryGetSceneGameObject("DoorColliders");
        //GameObject doorColliderGameObject = GameObject.Instantiate(doorColliderPrefab);
        doorColliderGameObject.name = "DoorColliders";
        this.AddGameObject("DoorColliders", doorColliderGameObject);

        //加载寻路
        GameObject meshSurfaceGameObject = await TryGetSceneGameObject("MeshSurface");
        //GameObject meshSurfaceGameObject = GameObject.Instantiate(meshSurfacePrefab);
        meshSurfaceGameObject.name = "MeshSurface";
        this.AddGameObject("MeshSurface", meshSurfaceGameObject);

        //加载player
        GameObject playerGameObject = await TryGetSceneGameObject("PlayerRoot");
        //GameObject playerGameObject = GameObject.Instantiate(playerPrefab);
        playerGameObject.name = "PlayerRoot";
        this.AddGameObject("PlayerRoot", playerGameObject);
    }

    public void UnLoadOfficeScene()
    {
        var resService = _context.GetService<IAssetLoadUtil>();
        //办公室场景 模型
        if (_sceneGameObjects.TryGetValue("OfficeScene", out GameObject officeScene))
        {
        }

        //门 模型
        if (_sceneGameObjects.TryGetValue("Doors", out GameObject doorGo))
        {
        }

        //门 collider
        if (_sceneGameObjects.TryGetValue("DoorColliders", out GameObject doorColliderGameObject))
        {
        }

        //寻路（可以留着？）
        if (_sceneGameObjects.TryGetValue("MeshSurface", out GameObject meshSurfaceGameObject))
        {
        }

        //player可以留着？
        if (_sceneGameObjects.TryGetValue("PlayerRoot", out GameObject playerGameObject))
        {
        }
    }

    public async void LoadProvincialMap()
    {
        if (!_sceneGameObjects.ContainsKey("ProvinceScene"))
        {
            var resService = _context.GetService<IAssetLoadUtil>();
            //var provincePrefab = resService.ResourcesLoad<GameObject>("Prefabs/ShanXiProvince.prefab");
            var provincePrefab = await resService.ResourceLoadAsync<GameObject>("ShanXiProvince");
            var go = GameObject.Instantiate(provincePrefab);
            go.name = "ProvinceScene";
            this.AddGameObject("ProvinceScene", go);
        }
        else
        {
            GameObject provinceScene = await this.TryGetSceneGameObject("ProvinceScene");
            provinceScene.SetActive(true);
        }
    }

    public void UnLoadProvincialMap()
    {
        if (_sceneGameObjects.TryGetValue("ProvinceScene", out GameObject provinceScene))
        {
            provinceScene.SetActive(false);
        }
    }
}