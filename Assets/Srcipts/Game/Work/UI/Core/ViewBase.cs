using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LP.Framework
{
    public class ViewBase : MonoBase, IViewBase
    {
        public GameObject PanelObj { get; set; }
        public ViewLayer Layer;
        public object[] Args { get; set; }
        List<IViewBase> RecvList = new List<IViewBase>();

        protected RectTransform _rectTransform;
        public void Init()
        {
        }

        public virtual void InitUI(params object[] args)
        {
            this.Args = args;
            _rectTransform = this.GetComponent<RectTransform>();
        }

        public virtual void OnShow()
        {

        }

        public virtual void OnClose()
        {
        }

        public List<IViewBase> GetViewsList()
        {
            return RecvList;
        }

        public virtual void ReceiveMessage(XMessage msg)
        {

        }

        /// <summary>
        /// 刷新UI,根据实际运用情况设置
        /// </summary>
        public virtual void RefreshUI()
        {
        }
        public virtual void RefreshData(params object[] args)
        {

        }

        public void RegisterView(IViewBase view)
        {
            if (!RecvList.Contains(view))
                RecvList.Add(view);
        }

        public void RemoveView(IViewBase view)
        {
            if (RecvList.Contains(view))
                RecvList.Remove(view);
        }

        public void RemoveAllView()
        {
            RecvList.Clear();
        }

    }

    public enum ViewLayer
    {
        Panel,
        Chart,
        Tips,
    }
}
