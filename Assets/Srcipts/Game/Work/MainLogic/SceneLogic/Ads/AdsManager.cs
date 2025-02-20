using System.Collections.Generic;
using Loxodon.Framework.Contexts;
using LP.Framework;
using UnityEngine;

public interface IAddService : IService
{
    void Init();
}

public class AdsManager : IAddService
{
    private ApplicationContext _context;
    private bool _initializedCubicle = false;
    private LayerMask _interactiveColliderLayer;

    private bool HasInitialized()
    {
        return _initializedCubicle;
    }

    public void Init()
    {
        if (HasInitialized())
        {
            return;
        }

        this._context = Context.GetApplicationContext();

        _interactiveColliderLayer = LayerMask.NameToLayer("InteractiveCollider");

        InitAdsGameObject();
        _initializedCubicle = true;
    }

    private async void InitAdsGameObject()
    {
        var sceneGameObjectService = _context.GetService<ISceneGameObjectService>();
        GameObject ads = await sceneGameObjectService.TryGetSceneGameObject("AdsPop");
        if (ads != null)
        {
            for (int i = 0; i < ads.transform.childCount; i++)
            {
                var adsItem = ads.transform.GetChild(i).gameObject;
                //adsItem.layer = _interactiveColliderLayer;
                var adsItemScript = adsItem.AddComponent<Ads>();
                adsItemScript.Init();
            }
        }
    }

    private void LoadAdsGameObjectCallBack(GameObject ads)
    {
        if (ads != null)
        {
            for (int i = 0; i < ads.transform.childCount; i++)
            {
                var adsItem = ads.transform.GetChild(i).gameObject;
                //adsItem.layer = _interactiveColliderLayer;
                var adsItemScript = adsItem.AddComponent<Ads>();
                adsItemScript.Init();
            }
        }
    }
}

public class AdsDataConfig
{
    public List<string> adsData;
}