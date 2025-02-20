using System;
using System.Collections.Generic;
using Loxodon.Framework.Contexts;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable]
public class AllEmployeeDatas
{
    public List<EmployeeData> datas;
}

[Serializable]
public class EmployeeData
{
    public string name;//姓名
    public string department;//部门
    public string posts;//职位
    public string state;//状态
    public string introduction;//简介
}
[Serializable]
public class EmployeeCubicleData
{
    public string cubicleIndex;
}
public class AllEmployeeCubicleDataes
{
    public List<EmployeeCubicleData> employeeCubicleDataList;
}

public class EmployeeCubicle : MonoBehaviour, IInteractive
{
    [SerializeField]
    private EmployeeData _employeeData;
    [SerializeField]
    private string _cubicleId;//工位id

    private Camera _mainCamera;
    private MeshRenderer _renderer;
    private CubicleCollider _collider;
    private EmployeeCubiclesIcon _employeeCubiclesIcon;
    private IEmployeeService _employeeService;
    private bool _isEmployeeInWork = false;

    private float _distanceToCamera;//距离摄像机的距离
    private Vector3 _positionSS;//屏幕坐标
    private bool _isShowing = false;
    private static string _emissionKeyWord = "_EMISSION";

    public void Init(EmployeeCubicleData config)
    {
        this._cubicleId = config.cubicleIndex;
        this._renderer = this.transform.GetChild(0).GetComponent<MeshRenderer>();
        _employeeService = Context.GetApplicationContext().GetService<IEmployeeService>();
    }

    private void Start()
    {
        this._mainCamera = Camera.main;
        _collider = this.gameObject.AddComponent<CubicleCollider>();
        _collider.SetOwner(this);
        _employeeCubiclesIcon = this.transform.GetChild(1).gameObject.AddComponent<EmployeeCubiclesIcon>();
        _employeeCubiclesIcon.SetOwner(this);
        this._employeeCubiclesIcon.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (this._employeeCubiclesIcon != null)
        {
            _positionSS = _mainCamera.WorldToViewportPoint(this.transform.position);
            if (_positionSS.x < 1 && _positionSS.x > 0 && _positionSS.y < 1 && _positionSS.y > 0 && _positionSS.z > 0)
            {
                //在屏幕范围内
                Vector3 direction = this._employeeCubiclesIcon.transform.position - this._mainCamera.transform.position;
                float distance = direction.magnitude;
                _distanceToCamera = distance;
                //且在范围内 
                if (distance < 5.0f)
                {
                    //找Manager控制是否显示
                    _employeeService.AddShowEmployeeCubicle(this);
                    return;
                }
            }
            _employeeService.RemoveShowEmployeeCubicle(this);
        }
    }

    public float GetDistanceToCamera()
    {
        return _distanceToCamera;
    }

    public void Show()
    {
        if (!_isShowing)
        {
            _isShowing = true;
            this._employeeCubiclesIcon.gameObject.SetActive(true);
        }
    }
    public void Hide()
    {
        if (_isShowing)
        {
            _isShowing = false;
            this._employeeCubiclesIcon.gameObject.SetActive(false);
        }
    }

    private void SetMaterialByCubicle()
    {
        //根据是否到岗选择屏幕是否点亮
        if (_isEmployeeInWork)
        {
            _renderer.material.EnableKeyword(_emissionKeyWord);
        }
        else
        {
            _renderer.material.DisableKeyword(_emissionKeyWord);
        }
    }

    private void OnEmployeeEnterCubicle()
    {
        _isEmployeeInWork = true;
        SetMaterialByCubicle();
    }

    private void OnEmployeeLeaveCubicle()
    {
        _isEmployeeInWork = false;
        SetMaterialByCubicle();
    }

    public void Interactive(InterActiveArgs data)
    {
        if (AutoRoaming.IsAutoRoaming)
        {
            //自动漫游过程中不允许交互
            return;
        }
        //找Manager发送消息
        var employeeManager = Context.GetApplicationContext().GetService<IEmployeeService>();
        employeeManager.QueryEmployeeInfomation(this._cubicleId);
    }

#if UNITY_EDITOR
    //用于EditorWindow
    public string GetCubicleId()
    {
        return this._cubicleId;
    }

#endif

}