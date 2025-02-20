using System;
using System.Collections;
using System.Text;
using DG.Tweening;
using Loxodon.Framework.Contexts;
using LP.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIDoorPermission : ViewBase
{
    private CanvasGroup _canvasGroup;
    private GameObject _notAllowedText;
    private Button _buttonEmptyPlace;
    private Button _buttonCloseUI;
    private Button _buttonOpenDoor;
    private GameObject _openDoorGameObject;
    private TMP_Text _passWordNotice;
    private TMP_InputField _passwordInput;
    private GameObject _passwordInputRoot;
    private Button _buttonAccept;
    private Button _buttonCancle;
    private GameObject _mask;
    private GameObject _noticeGameObject;
    private GameObject _passwordCorrect;
    private GameObject _passwordError;
    private PermissionData _permissionData;
    private string _currentRoomName;

    public override void InitUI(params object[] args)
    {
        base.InitUI(args);
        _canvasGroup = this.transform.GetComponent<CanvasGroup>();
        _buttonEmptyPlace = this.transform.GetChild(0).GetComponent<Button>();
        _mask = _buttonEmptyPlace.transform.GetChild(0).gameObject;
        _buttonCloseUI = this.transform.GetChild(2).GetComponent<Button>();

        _passwordInputRoot = this.transform.GetChild(1).gameObject;
        _passWordNotice = _passwordInputRoot.transform.GetChild(0).GetComponent<TMP_Text>();
        _passwordInput = _passwordInputRoot.transform.GetChild(1).GetComponent<TMP_InputField>();

        _noticeGameObject = _passwordInputRoot.transform.GetChild(2).gameObject;
        _passwordError = _noticeGameObject.transform.GetChild(0).gameObject;
        _passwordCorrect = _noticeGameObject.transform.GetChild(1).gameObject;

        _buttonAccept = _passwordInputRoot.transform.GetChild(3).GetChild(0).GetComponent<Button>();
        _buttonCancle = _passwordInputRoot.transform.GetChild(3).GetChild(1).GetComponent<Button>();

        _openDoorGameObject = this.transform.GetChild(3).gameObject;
        _buttonOpenDoor = _openDoorGameObject.transform.GetChild(0).GetComponent<Button>();
        _notAllowedText = this.transform.GetChild(4).gameObject;

        _buttonCloseUI.onClick.AddListener(OnClickButtonClose);
        _buttonEmptyPlace.onClick.AddListener(OnClickButtonClose);
        _buttonOpenDoor.onClick.AddListener(OnClickOpenDoorButton);
        _buttonAccept.onClick.AddListener(OnClickAcceptButton);
        _buttonCancle.onClick.AddListener(OnClickCancleButton);

        //防止输入密码时移动
        _passwordInput.onSelect.AddListener(OnInputFiledSelected);
        _passwordInput.onDeselect.AddListener(OnInputFiledDeselected);


        //_noticeGameObject.SetActive(false);
        _passwordError.SetActive(false);
        _passwordCorrect.SetActive(false);

        var inputService = Context.GetApplicationContext().GetService<IInputService>();
        inputService.RegisterUIEnterButtonClick(OnClickAcceptButton);

        _passwordInput.ActivateInputField();
        //处理参数
        if (args != null && args.Length > 0)
        {
            _permissionData = args[0] as PermissionData;
        }
    }

    private void Start()
    {
        this._passwordInput.asteriskChar = '·';
    }

    private void CloseUI()
    {
        this._canvasGroup.DOFade(0, 0.5f).OnComplete(() =>
        {
            var permissionManger = Context.GetApplicationContext().GetService<IPermissionsManagement>();
            permissionManger.CloseUIDoorPermission();
            //销毁会由ViewManager触发 并触发OnClose
        });
    }

    public override void OnShow()
    {
        base.OnShow();
        if (this._permissionData != null)
        {
            RefreshUI();
        }

        this._canvasGroup.DOFade(1, 0.5f);
    }

    public override void OnClose()
    {
        base.OnClose();
        //由ViewManager触发
        var inputService = Context.GetApplicationContext().GetService<IInputService>();
        inputService.DisRegisterUIEnterButtonClick(OnClickAcceptButton);
    }

    //刷新UI
    public override void RefreshUI()
    {
        if (this._permissionData == null)
        {
            XLog.E("刷新开门UI，但是许可数据为空");
            return;
        }

        _passWordNotice.text = $"请输入【{_permissionData.roomName}】门禁密码";
        if (this._permissionData.allowVisit == "false")
        {
            _passwordInputRoot.SetActive(false);
            _buttonCloseUI.gameObject.SetActive(false);
            //禁止访问
            _mask.SetActive(false);
            _notAllowedText.SetActive(true);
            _openDoorGameObject.SetActive(false);
            //自动导航停止导航
            OnNotAllowVisit();
            return;
        }

        //允许访问
        _notAllowedText.SetActive(false);
        if (this._permissionData.passwordRequire == "true")
        {
            _passwordInputRoot.SetActive(true);
            _buttonCloseUI.gameObject.SetActive(true);
            //需要密码
            _mask.SetActive(true);
            _openDoorGameObject.SetActive(false);
        }
        else
        {
            _buttonCloseUI.gameObject.SetActive(false);
            //不需要密码            
            _mask.SetActive(false);
            _openDoorGameObject.SetActive(true);
            _passwordInputRoot.SetActive(false);
            _passwordInput.gameObject.SetActive(false);
        }
    }

    private void OnClickButtonClose()
    {
        //显示提示
        IViewService viewService = Context.GetApplicationContext().GetService<IViewService>();
        if (AutoRoaming.IsAutoRoaming)
        {
            StringBuilder sb = new StringBuilder("固定路线漫游结束，已自动切换为自由漫游");
            var uITip = viewService.GetView<UITip>();
            if (uITip == null)
            {
                viewService.OpenView<UITip>(sb);
            }
            else
            {
                UITip tip = uITip as UITip;
                tip.RefreshData(sb);
            }
        }

        //点击关闭UI或者空白地方 关闭固定路径漫游
        var sceneLogicService = Context.GetApplicationContext().GetService<ISceneLogicService>();
        sceneLogicService.StopAutoRoaming();
        //打开自由漫游提示
        viewService.OpenView<UITipLeftDown>();
        //取消开关门数据
        sceneLogicService.OnCancleOpenDoor();

        var inputService = Context.GetApplicationContext().GetService<IInputService>();
        inputService.EnableMoveInput();
        CloseUI();
    }

    private void OnClickOpenDoorButton()
    {
        //开门
        var sceneLogicService = Context.GetApplicationContext().GetService<ISceneLogicService>();
        sceneLogicService.ControlDoor(true);

        var inputService = Context.GetApplicationContext().GetService<IInputService>();
        inputService.EnableMoveInput();
        CloseUI();
    }

    private void OnClickAcceptButton()
    {
        //验证密码
        string password = _passwordInput.text;
        var permissionsManagement = Context.GetApplicationContext().GetService<IPermissionsManagement>();
        _noticeGameObject.SetActive(true);
        if (permissionsManagement.CheckPassword(password))
        {
            _passwordCorrect.SetActive(true);
            _passwordError.SetActive(false);
            //开门
            var sceneLogicService = Context.GetApplicationContext().GetService<ISceneLogicService>();
            sceneLogicService.ControlDoor(true);

            var inputService = Context.GetApplicationContext().GetService<IInputService>();
            inputService.EnableMoveInput();
            CloseUI();
        }
        else
        {
            //弹出密码错误UI
            _passwordCorrect.SetActive(false);
            this.StopAllCoroutines();

            _noticeGameObject.SetActive(true);
            _passwordError.SetActive(true);
            //3秒后关闭ErrorUI
            StartCoroutine("ShowPasswordError");

            //密码错误 交点回到输入框中
            _passwordInput.ActivateInputField();
        }
    }

    private IEnumerator ShowPasswordError()
    {
        //3秒后关闭ErrorUI
        yield return new WaitForSeconds(3.0f);
        _passwordError.SetActive(false);
        _noticeGameObject.SetActive(false);
    }

    private void OnClickCancleButton()
    {
        //显示提示
        IViewService viewService = Context.GetApplicationContext().GetService<IViewService>();
        if (AutoRoaming.IsAutoRoaming)
        {
            StringBuilder sb = new StringBuilder("固定路线漫游结束，已自动切换为自由漫游");
            var uITip = viewService.GetView<UITip>();
            if (uITip == null)
            {
                viewService.OpenView<UITip>(sb);
            }
            else
            {
                UITip tip = uITip as UITip;
                tip.RefreshData(sb);
            }
        }

        //点击取消按钮 关闭固定路径
        var sceneLogicService = Context.GetApplicationContext().GetService<ISceneLogicService>();
        sceneLogicService.StopAutoRoaming();
        //打开自由漫游提示
        viewService.OpenView<UITipLeftDown>();
        //取消开门
        sceneLogicService.OnCancleOpenDoor();
        var inputService = Context.GetApplicationContext().GetService<IInputService>();
        inputService.EnableMoveInput();

        //关闭UI
        CloseUI();
    }

    private void OnNotAllowVisit()
    {
        //自动导航停止导航
        var sceneLogicService = Context.GetApplicationContext().GetService<ISceneLogicService>();
        sceneLogicService.OnRoomNotAllowVisit();
        IViewService viewService = Context.GetApplicationContext().GetService<IViewService>();
        StringBuilder sb = new StringBuilder("该房间暂不对外开放");
        var uITip = viewService.GetView<UITip>();
        if (uITip == null)
        {
            viewService.OpenView<UITip>(sb);
        }
        else
        {
            UITip tip = uITip as UITip;
            tip.RefreshData(sb);
        }

        //打开自由漫游提示
        viewService.OpenView<UITipLeftDown>();
        //取消开关门数据
        sceneLogicService.OnCancleOpenDoor();

        var inputService = Context.GetApplicationContext().GetService<IInputService>();
        inputService.EnableMoveInput();
    }

    private void OnInputFiledSelected(string text)
    {
        //Debug.Log(114514);
        var inputService = Context.GetApplicationContext().GetService<IInputService>();
        inputService.DisableMoveInput();
    }

    private void OnInputFiledDeselected(string text)
    {
        var inputService = Context.GetApplicationContext().GetService<IInputService>();
        inputService.EnableMoveInput();
    }
}