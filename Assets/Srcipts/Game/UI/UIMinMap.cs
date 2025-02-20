using System;
using Loxodon.Framework.Contexts;
using LP.Framework;
using UnityEngine;
using UnityEngine.UI;

public class UIMinMap : UIMap
{
    private CanvasGroup _canvasGroup;
    private Button _clickButton;
    private Image _minMapImage;
    private Image _arrowImage;
    private Button _arrowHideButton;
    private Button _arrowShowButton;

    public override void InitUI(params object[] args)
    {
        //基类中处理参数
        base.InitUI(args);
        this._canvasGroup = this.GetComponent<CanvasGroup>();
        this._minMapImage = this.transform.GetChild(0).GetComponent<Image>();
        this._clickButton = this.transform.GetChild(0).GetComponent<Button>();
        this._arrowImage = _clickButton.transform.GetChild(0).GetComponent<Image>();

        _arrowHideButton = this.transform.GetChild(1).GetComponent<Button>();
        _arrowShowButton = this.transform.GetChild(2).GetComponent<Button>();

        this._clickButton.onClick.AddListener(this.OnClickOpenGuidButton);
        this._arrowHideButton.onClick.AddListener(this.HideMiniMap);
        this._arrowShowButton.onClick.AddListener(this.ShowMiniMap);
    }

    public override void OnShow()
    {
        base.OnShow();
        ShowMiniMap();
    }

    public override void OnClose()
    {
        base.OnClose();
        //由ViewManager触发
    }

    public void Update()
    {
        //更新小地图位置
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
        float xSize = _minMapImage.rectTransform.sizeDelta.x / sceneSize.x;
        float ySize = _minMapImage.rectTransform.sizeDelta.y / sceneSize.y;
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

    //刷新UI
    public override void RefreshUI()
    {
        //更新小地图位置
        if (_playerMoveCallBack == null)
        {
            XLog.E("未注册player位置回调");
            return;
        }

        Vector3 palyerRelativePosition = _playerMoveCallBack.Invoke();
        SetMinmapPosition(palyerRelativePosition);
    }

    private void HideMiniMap()
    {
        this._arrowHideButton.gameObject.SetActive(false);
        this._minMapImage.gameObject.SetActive(false);
        this._arrowShowButton.gameObject.SetActive(true);
    }
    private void ShowMiniMap()
    {
        this._arrowHideButton.gameObject.SetActive(true);
        this._minMapImage.gameObject.SetActive(true);
        this._arrowShowButton.gameObject.SetActive(false);
    }

    private void OnClickOpenGuidButton()
    {
        //打开引导界面
        var guildTourService = Context.GetApplicationContext().GetService<IGuildTourService>();
        guildTourService.OpenGuidlMap("true");
    }
}
