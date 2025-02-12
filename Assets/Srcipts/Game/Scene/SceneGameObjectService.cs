using System.Collections.Generic;
using System.ComponentModel.Design;
using Loxodon.Framework.Contexts;
using LP.Framework;
using UnityEngine;

public interface ISceneGameObjectService : IService
{
    void AddGameObject(string name, GameObject go);
    GameObject TryGetSceneGameObject(string name);
    void LoadOfficeCence();
    void LoadProvincialMap();
    void UnLoadProvincialMap();
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

    public GameObject TryGetSceneGameObject(string name)
    {
        if (_sceneGameObjects.TryGetValue(name, out GameObject gameObject))
        {
            return gameObject;
        }

        var resService = _context.GetService<IAssetLoadUtil>();
        //加载物体
        var prefab = resService.ResourcesLoad<GameObject>($"Prefabs/{name}.Prefab");
        if (prefab == null)
        {
            XLog.E($"未找到预制体{name}");
            return null;
        }
        var prefabGameObject = GameObject.Instantiate(prefab);
        prefabGameObject.name = name;
        prefabGameObject.transform.position = Vector3.zero;
        this.AddGameObject(name, prefabGameObject);

        return prefabGameObject;
    }

    public void LoadOfficeCence()
    {
        var resService = _context.GetService<IAssetLoadUtil>();
        var scenePrefab = resService.ResourcesLoad<GameObject>("Prefabs/DigitalScene.prefab");
        var go = GameObject.Instantiate(scenePrefab);
        go.name = "OfficeScene";
        this.AddGameObject("OfficeScene", go);

        //加载光照信息
        MeshLightingDataLoader lightingDataLoader = new MeshLightingDataLoader();
        lightingDataLoader.Init(go);

        //加载门 模型
        var doorPrefab = resService.ResourcesLoad<GameObject>("Prefabs/Doors.Prefab");
        var doorGo = GameObject.Instantiate(doorPrefab);
        doorGo.name = "Doors";
        this.AddGameObject("Doors", doorGo);

        //加载collider
        var colliderPrefab = resService.ResourcesLoad<GameObject>("Prefabs/DoorColliders.Prefab");
        var doorColliderGameObject = GameObject.Instantiate(colliderPrefab);
        doorColliderGameObject.name = "DoorColliders";
        this.AddGameObject("DoorColliders", doorColliderGameObject);

        //加载寻路
        var meshSurfacePrefab = resService.ResourcesLoad<GameObject>("Prefabs/MeshSurface.prefab");
        GameObject meshSurfaceGameObject = GameObject.Instantiate(meshSurfacePrefab);
        meshSurfaceGameObject.name = "MeshSurface";
        this.AddGameObject("MeshSurface", meshSurfaceGameObject);

        //加载player
        var playerPrefab = resService.ResourcesLoad<GameObject>("Prefabs/PlayerRoot.prefab");
        var playerGameObject = GameObject.Instantiate(playerPrefab);
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
    public void LoadProvincialMap()
    {
        if (!_sceneGameObjects.ContainsKey("ProvinceScene"))
        {
            var resService = _context.GetService<IAssetLoadUtil>();
            var provincePrefab = resService.ResourcesLoad<GameObject>("Prefabs/ShanXiProvince.prefab");
            var go = GameObject.Instantiate(provincePrefab);
            go.name = "ProvinceScene";
            this.AddGameObject("ProvinceScene", go);
        }
        else
        {
            var provinceScene = this.TryGetSceneGameObject("ProvinceScene");
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