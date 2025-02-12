using System.Collections;
using System.Collections.Generic;
using Loxodon.Framework.Binding;
using Loxodon.Framework.Binding.Builder;
using Loxodon.Framework.Interactivity;
using Loxodon.Framework.Views;
using UnityEngine;

public class MaskView : Window
{
    private MaskViewModel _maskWindowModel;
    protected override void OnCreate(IBundle bundle)
    {
        _maskWindowModel = new MaskViewModel();
        BindingSet<MaskView, MaskViewModel> bindingSet = this.CreateBindingSet(_maskWindowModel);
        //数据绑定
        
        bindingSet.Build();
    }
    
    protected void OnDismissRequest(object sender, InteractionEventArgs args)
    {
        this.Dismiss();
    }
}
