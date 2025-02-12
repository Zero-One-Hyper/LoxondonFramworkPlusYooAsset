using LP.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UITipLeftDown : ViewBase
{
    private CanvasGroup _canvasGroup;
    private Image _backGroundImage;
    private TMP_Text _tipText;

    public override void InitUI(params object[] args)
    {
        base.InitUI(args);
        _canvasGroup = this.transform.GetComponent<CanvasGroup>();
        _backGroundImage = this.transform.GetChild(0).GetComponentInChildren<Image>();
        _tipText = _backGroundImage.transform.GetChild(0).GetComponentInChildren<TMP_Text>();
        _canvasGroup.alpha = 1;
    }
    public override void OnShow()
    {
        base.OnShow();

    }

    public override void OnClose()
    {
        base.OnClose();
        //由ViewManager触发
    }

    //刷新UI
    public override void RefreshUI()
    {

    }
}
