using System;
using System.Collections.Generic;
using DG.Tweening;
using Loxodon.Framework.Contexts;
using LP.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIGuildTourWindow : UIMap
{
    private CanvasGroup _canvasGroup;
    private Button _closeButton;
    private RectTransform _mapRect;
    private Transform _roomButtonRoot;
    private Image _arrowImage;
    private Image _noticeImage;
    private EmployeeData _employeeData;

    public override void InitUI(params object[] args)
    {
        //基类中处理参数
        base.InitUI(args);
        this._canvasGroup = this.GetComponent<CanvasGroup>();

        this._closeButton = this.transform.GetChild(3).GetComponent<Button>();
        this._mapRect = this.transform.GetChild(1).GetChild(0).GetComponent<RectTransform>();
        this._arrowImage = this._mapRect.GetChild(0).GetComponent<Image>();
        this._noticeImage = this.transform.GetChild(4).GetComponent<Image>();
        this._noticeImage.gameObject.SetActive(false);

        _roomButtonRoot = this.transform.GetChild(2);
        var doorplateService = Context.GetApplicationContext().GetService<IDoorplateService>();

        for (int i = 0; i < _roomButtonRoot.childCount; i++)
        {
            if (!_roomButtonRoot.GetChild(i).TryGetComponent<CustomButton>(out CustomButton roomButton))
            {
                continue;
            }
            //CustomButton roomButton = _roomButtonRoot.GetChild(i).GetComponent<CustomButton>();
            roomButton.onClick.AddListener(() =>
            {
                //点击事件
                OnClickRoomButton(roomButton.name);
            });
            roomButton.onButtonEnter.AddListener(() =>
            {
                //鼠标进入事件                
                OnMouseEnterButton(roomButton.name);
            });
            roomButton.onButtonExit.AddListener(() =>
            {
                //鼠标离开事件
                OnMouseExitButton(roomButton.name);
            });
            roomButton.onButtonStay.AddListener(() =>
            {
                //鼠标停留事件
                OnMouseStayButton(roomButton.name);
            });

            //填充房间名
            if (!roomButton.name.Contains('m'))
            {
                XLog.E($"UI{roomButton.name}没有有效的命名");
                continue;
            }
            //填充房间名
            string roomIndex = roomButton.name.Split('m')[1];
            int index = Convert.ToInt32(roomIndex);
            string roomName = doorplateService.GetSingleDoorplateData(index);
            TMP_Text buttonText = roomButton.transform.GetChild(0).GetComponent<TMP_Text>();
            buttonText.text = roomName;
        }
        this._closeButton.onClick.AddListener(this.OnClickCloseButton);

        _canvasGroup.alpha = 0;
    }

    public override void OnShow()
    {
        base.OnShow();
        if (this._employeeData != null)
        {
            RefreshUI();
        }
        this._canvasGroup.DOFade(1, 0.5f);

        //关闭鼠标输入
        var inputService = Context.GetApplicationContext().GetService<IInputService>();
        inputService.SetMouseControlActive(false);

        //关闭自由漫游提示 若启动了自动漫游 提示会被关掉？
        var viewService = Context.GetApplicationContext().GetService<IViewService>();
        viewService.CloseView<UITipLeftDown>();
    }

    public override void OnClose()
    {
        //由ViewManager触发
        base.OnClose();
        //关闭鼠标输入
        var inputService = Context.GetApplicationContext().GetService<IInputService>();
        inputService.SetMouseControlActive(true);

    }

    public void Update()
    {
        //更新地图位置
        if (_playerMoveCallBack == null)
        {
            return;
        }
        Vector3 palyerRelativePosition = _playerMoveCallBack.Invoke();
        SetMinmapPosition(palyerRelativePosition);

        if (_playerForwardCallBack == null)
        {
            return;
        }
        float playerEularAngelY = _playerForwardCallBack.Invoke();
        SetMinMapRotation(playerEularAngelY);
    }


    protected override void SetMinmapPosition(Vector3 palyerRelativePosition)
    {
        //大地图大小
        Vector3 sceneSize = GuildTourService._mapSize;
        float xSize = _mapRect.sizeDelta.x / sceneSize.x;
        float ySize = _mapRect.sizeDelta.y / sceneSize.y;
        Vector2 playerPositionOffset = new Vector2(-palyerRelativePosition.x * xSize,
            -palyerRelativePosition.z * ySize);
        _arrowImage.rectTransform.anchoredPosition = playerPositionOffset;
        _arrowImage.rectTransform.localPosition = playerPositionOffset;
    }

    protected override void SetMinMapRotation(float playerAngleY)
    {
        Vector3 eularAngel = _arrowImage.rectTransform.localEulerAngles;
        eularAngel.z = 90f - playerAngleY;
        _arrowImage.rectTransform.localEulerAngles = eularAngel;
    }

    private void OnClickCloseButton()
    {
        //打开自由漫游提示 若启动了自动漫游 提示会被关掉？
        var viewService = Context.GetApplicationContext().GetService<IViewService>();
        viewService.OpenView<UITipLeftDown>();
        DoClose();
    }
    private void DoClose()
    {
        this._canvasGroup.DOFade(0, 0.5f).OnComplete(() =>
                {
                    var guildTourService = Context.GetApplicationContext().GetService<IGuildTourService>();
                    guildTourService.CloseeGuildMap();
                    //销毁会由ViewManager触发 并触发OnClose
                });
    }
    //刷新UI
    public override void RefreshUI()
    {
        var doorplateService = Context.GetApplicationContext().GetService<IDoorplateService>();

        for (int i = 0; i < _roomButtonRoot.childCount; i++)
        {
            Transform roomButton = _roomButtonRoot.GetChild(i);
            //填充房间名
            if (!roomButton.name.Contains('m') || roomButton.name == "Mask")
            {
#if UNITY_EDITOR
                XLog.E($"UI{roomButton.name}没有有效的命名");
#endif
                continue;
            }
            string roomIndex = roomButton.name.Split('m')[1];
            int index = Convert.ToInt32(roomIndex);
            if (index == 37)
            {
                //37号房间是电梯间
                continue;
            }
            string roomName = doorplateService.GetSingleDoorplateData(index);

            TMP_Text buttonText = roomButton.transform.GetChild(0).GetComponent<TMP_Text>();
            buttonText.text = roomName;
        }
    }
    private void OnClickRoomButton(string name)
    {
        var guildTourService = Context.GetApplicationContext().GetService<IGuildTourService>();
        //Debug.Log(name);
        guildTourService.GuildToRoom(Convert.ToInt32(name.Split('m')[1]));

        //触发关闭窗口
        DoClose();
    }

    private void OnMouseEnterButton(string name)
    {
        this._noticeImage.gameObject.SetActive(true);
    }

    private void OnMouseExitButton(string name)
    {

        this._noticeImage.gameObject.SetActive(false);

    }

    private void OnMouseStayButton(string name)
    {

        this._noticeImage.rectTransform.position = Input.mousePosition;
    }
}
