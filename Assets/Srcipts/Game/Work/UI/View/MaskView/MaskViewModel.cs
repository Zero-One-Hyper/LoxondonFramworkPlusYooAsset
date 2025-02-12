using System.Collections;
using System.Collections.Generic;
using Loxodon.Framework.Messaging;
using Loxodon.Framework.ViewModels;
using UnityEngine;

public class MaskViewModel : ViewModelBase
{
    public MaskViewModel() : this(null)
    {
    }

    public MaskViewModel(IMessenger messenger) : base(messenger)
    {

    }
}
