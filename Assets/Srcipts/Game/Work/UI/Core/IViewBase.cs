using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LP.Framework
{
    public interface IViewBase : IService, IRefreshUIService
    {
        GameObject PanelObj { get; set; }

        object[] Args { get; set; }
        void RegisterView(IViewBase view);

        void RemoveView(IViewBase view);

        void RemoveAllView();

        void InitUI(params object[] args);

        void OnShow();

        void OnClose();

        List<IViewBase> GetViewsList();

        void ReceiveMessage(XMessage msg);
    }
}
