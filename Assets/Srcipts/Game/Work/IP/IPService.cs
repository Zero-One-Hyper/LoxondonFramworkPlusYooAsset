using System;
using DG.Tweening;
using Loxodon.Framework.Contexts;
using LP.Framework;
using UnitData.Const;
using UnityEngine;

public interface IIPService : IService
{
    void PlayIPTurnOffAnim();
    void SetRoamingIPModeActive(bool active);
    void ShowIPAnim(Action callBack);
}

public class IPService : IIPService
{
    private Context _context;
    private IWebService _webService;
    private GameObject _ipSceneRoot;
    private GameObject _ipSceneReceptionGameObject;
    private GameObject _ipScenePartical;
    private GameObject _ipRoamingRoot; //漫游IP展示后的父节点
    private GameObject _ipRoamingGameObject; //漫游IP

    private Vector3 _scneenIPDefaultPosition;

    //private Vector3 _romaingIPDefaultPosition = new Vector3(-0.04830003f, 0.5362f, 0.1194f);
    private Vector3 _romaingIPDefaultLoaclPosition;
    private Vector3 _romaingIPDefaultEularAngle;
    private Sequence _animSequence;
    private Action _animCallBack;
    private Camera _mainCamera;
    private bool _isShowingRomaingIP = false;
    private bool _isPlayingAnim = false;

    //private IPControll roamingIp;

    public async void Init()
    {
        _mainCamera = Camera.main;
        _context = Context.GetApplicationContext();
        _webService = _context.GetService<IWebService>();
        var sceneGameObjectService = _context.GetService<ISceneGameObjectService>();

        var officeSceneGameObject = await sceneGameObjectService.TryGetSceneGameObject("DigitalScene");
        _ipSceneRoot = officeSceneGameObject.transform.GetChild(0).gameObject;
        _ipSceneReceptionGameObject = _ipSceneRoot.transform.GetChild(1).gameObject;
        _ipScenePartical = _ipSceneReceptionGameObject.transform.GetChild(0).gameObject;
        _ipScenePartical.SetActive(false);


        //漫游IP
        GameObject palyerRoot = await sceneGameObjectService.TryGetSceneGameObject("PlayerRoot");
        _ipRoamingRoot = palyerRoot.transform.GetChild(0).GetChild(1).gameObject;
        _ipRoamingGameObject = palyerRoot.transform.GetChild(0).GetChild(2).gameObject;

        _scneenIPDefaultPosition = _ipSceneRoot.transform.position;
        _romaingIPDefaultLoaclPosition = _ipRoamingGameObject.transform.localPosition;
        _romaingIPDefaultEularAngle = _ipRoamingGameObject.transform.localEulerAngles;

        _ipSceneRoot.SetActive(true);
        _ipRoamingGameObject.SetActive(false);

        //注册
        var service = Context.GetApplicationContext().GetService<IEventHandleService>();
        service.AddListener(SceneEvent.SwitchIP, OnWebCallSwitchIP);
    }

    private void OnWebCallSwitchIP(EventArgs args)
    {
        var sceneEventArgs = args as SceneEventArgs;
        if (sceneEventArgs == null)
        {
            XLog.E("切换IP消息为空");
            return;
        }

        XLog.I($"接收到切换IP的消息{sceneEventArgs.Data}");
        SetRoamingIPModeActive(sceneEventArgs.Data == "true");
    }

    public void SetRoamingIPModeActive(bool active)
    {
        if (_isPlayingAnim)
        {
            return;
        }

        //this._ipRoamingGameObject.transform.DOPause();
        if (active)
        {
            if (_isShowingRomaingIP)
            {
                //正在展示漫游IP
                return;
            }

            //打开漫游ip
            _ipSceneRoot.SetActive(false);
            _ipRoamingGameObject.SetActive(true);
            //_ipRoamingGameObject.transform.localPosition = _romaingIPDefaultLoaclPosition;
            //Debug.LogError(_ipRoamingGameObject.transform.position);
            //float endValue = _ipRoamingGameObject.transform.position.y - 0.25f;
            //_isPlayingAnim = true;
            //Debug.LogError(_ipRoamingGameObject.transform.position.y);
            _ipRoamingGameObject.transform.DOMoveY(1.55f - 0.25f, 0.25f).From().OnComplete(() =>
            {
                _isPlayingAnim = false;
            });
            _isShowingRomaingIP = true;
            XLog.I("发送打开IP消息");
            _webService.UnityCallWeb(Constant.UNITY_SEND_IP_OPEN_STATE, "true");
        }
        else
        {
            if (!_isShowingRomaingIP)
            {
                //没有展示漫游IP
                return;
            }
            //关闭漫游IP            
            //_ipRoamingGameObject.transform.localPosition = _romaingIPDefaultLoaclPosition;
            //float endValue = _ipRoamingGameObject.transform.position.y - 0.25f;

            _ipRoamingGameObject.transform.DOMoveY(1.55f - 0.25f, 0.25f).OnComplete(() =>
            {
                _ipSceneRoot.transform.position = _scneenIPDefaultPosition;
                _ipSceneRoot.SetActive(true);
                _ipRoamingGameObject.SetActive(false);
                _ipRoamingGameObject.transform.localPosition = _romaingIPDefaultLoaclPosition;
                _isShowingRomaingIP = false;
                _isPlayingAnim = false;
                XLog.I("发送关闭IP消息");
                _webService.UnityCallWeb(Constant.UNITY_SEND_IP_OPEN_STATE, "false");
            });
        }
    }

    //从电梯间出门 播放IP的动画
    public void ShowIPAnim(Action callBack)
    {
        if (_isShowingRomaingIP)
        {
            //正在展示IP时不显示
            callBack?.Invoke();
            return;
        }

        _animCallBack = callBack;
        //_romaingIPDefaultPosition = _ipRoamingGameObject.transform.position;
        _ipScenePartical.SetActive(true);
        SetAnimQueue();
        return;
    }

    public void PlayIPTurnOffAnim()
    {
        SetRoamingIPModeActive(false);
    }


    private void SetAnimQueue()
    {
        //计算贝塞尔控制点
        Vector3 currentRoamingIpPosition = _ipRoamingGameObject.transform.position;
        Vector3 controlPoint =
            currentRoamingIpPosition + ((_scneenIPDefaultPosition - currentRoamingIpPosition) * 0.5f);
        controlPoint += Vector3.up * 0.36f; // + Vector3.back * 0.1f;
        //计算贝塞尔曲线
        Vector3[] pathvec = BezierTool.Bezier2Path(_scneenIPDefaultPosition, controlPoint, currentRoamingIpPosition);
        _animSequence = DOTween.Sequence();
        _animSequence.Append(_ipSceneRoot.transform.DOPath(pathvec, 1.0f).SetEase(Ease.OutQuad));
        _animSequence.Join(_ipSceneRoot.transform.DORotate(new Vector3(10f, 90f, 0f), 0.2f));
        _animSequence.Join(_ipSceneReceptionGameObject.transform.DOScale(0.1f, 1.0f).SetEase(Ease.OutQuad)).OnComplete(
            () =>
            {
                //Vector3 roamingIpEularAn
                _ipRoamingGameObject.transform.eulerAngles = _ipSceneRoot.transform.eulerAngles;
                _ipRoamingGameObject.SetActive(true);
                _ipSceneRoot.SetActive(false);
                //场景IP回到原点并重设缩放、旋转
                _ipSceneReceptionGameObject.transform.localScale = Vector3.one;
                _ipSceneRoot.transform.localEulerAngles = new Vector3(0, 90, 0);
                _ipSceneRoot.transform.position = _scneenIPDefaultPosition;
                _isShowingRomaingIP = true;
                _ipRoamingGameObject.transform.DOLocalRotate(_romaingIPDefaultEularAngle, 0.5f).OnComplete(() =>
                {
                    //发送打开IP的消息
                    XLog.I("发送打开IP消息");
                    _webService.UnityCallWeb(Constant.UNITY_SEND_IP_OPEN_STATE, "true");
                    _animCallBack?.Invoke();
                });
            });

        //Insert在给定的时间后放置回调
        _animSequence.Insert(0.5f, _ipSceneRoot.transform.DORotate(new Vector3(0f, 73.382f, 0f), 1.0f));
    }
}