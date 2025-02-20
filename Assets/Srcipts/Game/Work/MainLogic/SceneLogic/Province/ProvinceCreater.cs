using System;
using Loxodon.Framework.Contexts;
using LP.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;


public interface IProvinceService
{
    void Init(GameObject provinceScene);
}

public class ProvinceCreator : IProvinceService
{
    private ProvinceContorller _province;

    public void Init(GameObject provinceScene)
    {
        if (_province == null)
        {
            _province = provinceScene.AddComponent<ProvinceContorller>();
            _province.Init();
        }
    }

}
