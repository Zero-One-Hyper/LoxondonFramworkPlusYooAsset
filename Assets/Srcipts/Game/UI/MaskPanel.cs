using DG.Tweening;
using Loxodon.Framework.Contexts;
using LP.Framework;
using UnityEngine;
using UnityEngine.UI;

public class MaskPanel : ViewBase
{
    private Image _uiMask;
    public override void InitUI(params object[] args)
    {
        this.Layer = ViewLayer.Panel;
    }

    void Start()
    {
        _uiMask = this.transform.GetComponent<Image>();
        //注册
        var service = Context.GetApplicationContext().GetService<IEventHandleService>();
        service.AddListener(ShanXiUIEvent.SwitchUIMask, OnWebCallSwitchUIMask);
    }

    private void OnWebCallSwitchUIMask(EventArgs args)
    {
        ShanXiUIEventArgs data = args as ShanXiUIEventArgs;
        if (string.IsNullOrEmpty(data.Json))
        {
            XLog.E("开关UIMask消息 数据为空");
            return;
        }
        XLog.I("接收到切换Mask信息");
        if (data.Json == "true")
        {
            //打开UIMask
            this._uiMask.gameObject.SetActive(true);
            this._uiMask.DOFade(1, 0.5f);
            return;
        }
        else if (data.Json == "false")
        {
            //关闭UIMask
            this._uiMask.DOFade(0, 0.5f).OnComplete(
                () => this._uiMask.gameObject.SetActive(false)
                );
            return;
        }
        XLog.E("切换Mask 收到意外的参数");
    }
}
