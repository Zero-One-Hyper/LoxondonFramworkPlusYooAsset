using System;
using System.Collections.Generic;
using Loxodon.Framework.Contexts;
using LP.Framework;
using UnitData.Const;
using UnityEngine;
using UnityEngine.InputSystem;

public class ProvinceContorller : MonoBehaviour, IInteractive
{
    private bool _isInit = false;
    //private bool _isClickHightLight = false;
    private bool _canMouseAction = true;
    private Context _context;
    private ICameraService _cameraService;
    private IRayCastService _rayCastService;
    private Camera _mainCamera;
    private ProvinceInteract _currentDistrict;
    private ProvinceInteract _currentSelectedDistrict;
    private ProvinceTextManager _provinceTextManager;
    private List<ProvinceInteract> _allDistricts = new List<ProvinceInteract>();
    //曲线相关
    private UICurvedLine _curvedLine;

    public void Init()
    {
        if (!_isInit)
        {
            //行政区
            Transform districtRoot = this.transform.GetChild(2);
            for (int i = 0; i < districtRoot.childCount; i++)
            {
                Transform go = districtRoot.GetChild(i);
                ProvinceInteract provinceInteract = go.gameObject.AddComponent<ProvinceInteract>();
                provinceInteract.SetOwner(this);
                provinceInteract.Init(i);
                _allDistricts.Add(provinceInteract);
            }

            _mainCamera = Camera.main;
            _context = Context.GetApplicationContext();
            _cameraService = _context.GetService<ICameraService>();
            IInputService inputService = _context.GetService<IInputService>();
            //注册鼠标点击事件
            inputService.RegisterMouseDoubleClickInteractive(OnMouseDoubleClick);

            //曲线控制
            _curvedLine = transform.GetChild(5).GetComponent<UICurvedLine>();
            _curvedLine.SetPointCount(0);

            _rayCastService = _context.GetService<IRayCastService>();
            _isInit = true;
            _provinceTextManager = new ProvinceTextManager();
            _provinceTextManager.Init(this.transform.GetChild(0).gameObject);
        }
    }

    private void Update()
    {
        if (!_canMouseAction)
        {
            return;
        }
        var mousePosition = Mouse.current.position.ReadValue();
        if (_rayCastService.DoMouseEnterRayCast(mousePosition, out IInteractCollider district))
        {
            //鼠标进入
            if (_currentDistrict != null)
            {
                if (_currentSelectedDistrict == null || _currentDistrict.GetId() != _currentSelectedDistrict.GetId())
                {
                    if (_currentDistrict.GetId() != ((ProvinceInteract)district).GetId())
                    {
                        //切换鼠标进入的行政区
                        _currentDistrict.ColliderInteractive(InteractiveColliderType.MouseExit);
                    }
                    else// if (_currentDistrict.GetId() == ((ProvinceInteract)district).GetId())
                    {
                        //防止多次触发
                        return;
                    }
                }
                if (_currentDistrict.GetId() == ((ProvinceInteract)district).GetId())
                {
                    return;
                }
            }
            district.ColliderInteractive(InteractiveColliderType.MouseEnter);
            _currentDistrict = district as ProvinceInteract;
        }
        else
        {
            if (_currentDistrict != null)
            {
                if (_currentSelectedDistrict == null || _currentDistrict.GetId() != _currentSelectedDistrict.GetId())
                {
                    //鼠标离开
                    _currentDistrict.ColliderInteractive(InteractiveColliderType.MouseExit);
                }
            }
            _currentDistrict = null;
        }
    }

    public void Interactive(InterActiveArgs data)
    {
        if (_canMouseAction)
        {
            switch (data.interactiveColliderType)
            {
                case InteractiveColliderType.MouseEnter:
                    OnMouseEnterDistrict(data);
                    break;
                case InteractiveColliderType.MouseExit:
                    OnMouseExitDistrict(data);
                    break;
                case InteractiveColliderType.MouseClick:
                    OnMouseClickDistrict(data);
                    break;
            }
        }
    }

    private void OnMouseClickDistrict(InterActiveArgs data)
    {
        ProvinceInteract district = data.gameObject.GetComponent<ProvinceInteract>();

        //鼠标点击 click前必先触发mouseEnter
        if (_currentSelectedDistrict != null)
        {
            if (_currentSelectedDistrict.GetId() != ((ProvinceInteract)district).GetId())
            {
                //关闭当前
                _currentSelectedDistrict.ColliderInteractive(InteractiveColliderType.MouseExit);
                _provinceTextManager.DisActiveProvince(_currentSelectedDistrict.GetId());
            }
            else// if (_isClickHightLight)// && _currentDistrict.GetId() == ((ProvinceInteract)district).GetId()
            {
                //id相等 防止多次触发
                //return;
            }
        }
        _currentSelectedDistrict = district as ProvinceInteract;
        //摄像机操作 摄像机操作完成前鼠标不可操作
        _canMouseAction = false;
        _cameraService.SetCameraFocus(_currentSelectedDistrict.transform, () =>
        {
            //回调
            _canMouseAction = true;
        });
        //设置曲线,目前固定偏移
        _curvedLine.SetStart(_currentSelectedDistrict.transform);
        _curvedLine.SetPointCount();
        SendClickMessage(data.index, data.str);
    }

    private void OnMouseExitDistrict(InterActiveArgs data)
    {
        SendClickMessage(data.index, data.str);
        //判断是否离开的是选中的省
        if (_currentSelectedDistrict == null || _currentSelectedDistrict.GetId() != data.index)
        {
            _provinceTextManager.DisActiveProvince(data.index);
        }
    }

    private void OnMouseEnterDistrict(InterActiveArgs data)
    {
        SendClickMessage(data.index, data.str);
        //鼠标进入
        _provinceTextManager.AcitveProvince(data.index);
    }

    private void OnMouseDoubleClick(Vector2 vector)
    {
        if (!_canMouseAction)
        {
            return;
        }
        if (_currentSelectedDistrict != null)
        {
            //鼠标离开
            _currentSelectedDistrict.ColliderInteractive(InteractiveColliderType.MouseExit);
            _provinceTextManager.DisActiveProvince(_currentSelectedDistrict.GetId());
        }
        _currentSelectedDistrict = null;
        OnClickOtherPlace();//点击空白部分
        this._canMouseAction = false;
        _cameraService.SetCameraUnFocus(() =>
        {
            this._canMouseAction = true;
        });
        //关闭曲线
        _curvedLine.SetPointCount(0);
        //_isClickHightLight = false;
        //发送退出事件
        SendClickMessage(-1, "Exit");
    }
    private void OnClickOtherPlace()
    {
        SendClickMessage(-1, "Click");
    }

    private void SendClickMessage(int districtIndex, string interactiveType)
    {
        /*
        咸阳选区 = "0"     "MouseExit"  鼠标移出
        商洛选区 = "1"     "MouseEnter" 鼠标移入
        安康选区 = "2"     "Click"      鼠标点击
        宝鸡选区 = "3"     "DoubleClick"鼠标双击
        延安选区 = "4"     
        榆林选区 = "5"     
        汉中选区 = "6"     
        渭南选区 = "7"     
        西安选区 = "8"     
        铜川选区 = "9"     
        */
        var context = Context.GetApplicationContext();
        var webService = context.GetService<IWebService>();
        string data = "districtIndex:" + districtIndex.ToString() + ";" + "interactiveType:" + interactiveType;
        webService.UnityCallWeb(Constant.UNITY_CALL_INTERACT_DITRICK, data);
        //Debug.Log(data);
        XLog.I($"unity发送{interactiveType}行政区信息{districtIndex.ToString()}");
    }

}
