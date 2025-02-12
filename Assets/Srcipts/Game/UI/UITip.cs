using System.Collections;
using System.Text;
using DG.Tweening;
using Loxodon.Framework.Contexts;
using LP.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UITip : ViewBase
{
    private CanvasGroup _canvasGroup;
    private Image _backGroundImage;
    private TMP_Text _tipText;
    private ContentSizeFitter _contentSizeFitterImage;
    private ContentSizeFitter _contentSizeFitterText;

    public override void InitUI(params object[] args)
    {
        base.InitUI(args);
        _canvasGroup = this.transform.GetComponent<CanvasGroup>();
        _backGroundImage = this.transform.GetChild(0).GetComponentInChildren<Image>();
        _tipText = _backGroundImage.transform.GetChild(0).GetComponentInChildren<TMP_Text>();
        _contentSizeFitterImage = _backGroundImage.gameObject.GetComponent<ContentSizeFitter>();
        _contentSizeFitterText = _tipText.gameObject.GetComponent<ContentSizeFitter>();
        _canvasGroup.alpha = 0;

        if (args != null && args.Length > 0)
        {
            StringBuilder tipText = args[0] as StringBuilder;
            this._tipText.text = tipText.ToString();
        }
    }
    public override void OnShow()
    {
        base.OnShow();

        this._canvasGroup.DOFade(1, 0.5f).OnComplete(() =>
        {
            StartCoroutine("OnStart");
        });
    }

    IEnumerator OnStart()
    {
        yield return new WaitForSeconds(3f);
        CloseUI();
    }

    private void CloseUI()
    {
        this._canvasGroup.DOFade(0, 0.5f).OnComplete(() =>
        {
            var viewService = Context.GetApplicationContext().GetService<IViewService>();
            viewService.CloseView<UITip>();
            //销毁会由ViewManager触发 并触发OnClose
        });
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
    public override void RefreshData(params object[] args)
    {
        if (args != null && args.Length > 0)
        {
            StringBuilder tipText = args[0] as StringBuilder;
            this._tipText.text = tipText.ToString();
        }
        this.StopAllCoroutines();
        this._canvasGroup.alpha = 1;
        _contentSizeFitterText.SetLayoutHorizontal();
        //_contentSizeFitterImage.SetLayoutHorizontal();
        this._canvasGroup.DOPause();
        StartCoroutine("OnStart");
    }
}
