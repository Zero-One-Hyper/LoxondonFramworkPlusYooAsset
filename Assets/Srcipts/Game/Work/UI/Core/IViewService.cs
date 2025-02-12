using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LP.Framework
{
    public interface IViewService : IService
    {
        Dictionary<string, IViewBase> ViewDict { get; set; }

        Dictionary<ViewLayer, Transform> LayerDict { get; set; }

        IViewBase OpenView<T>(params object[] args) where T : IViewBase;

        IViewBase OpenView<T>(string path, params object[] args) where T : IViewBase;

        void CloseView(string viewName);

        void CloseView<T>() where T : IViewBase;

        void ShowView(string viewName);

        void ShowView<T>() where T : IViewBase;

        void HideView(string viewName);

        void HideView<T>() where T : IViewBase;

        IViewBase GetView<T>() where T : IViewBase;

        bool CheckMouseOnUI(Vector2 mousePosition);
    }
}
