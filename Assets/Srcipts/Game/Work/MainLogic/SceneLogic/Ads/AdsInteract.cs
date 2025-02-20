using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(BoxCollider))]
public class AdsInteract : MonoBehaviour, IInteractCollider
{

    public IInteractive interactor { get; set; }

    private void OnClickAds()
    {
        InterActiveArgs args = new InterActiveArgs()
        {
            str = this.name.Split('-')[1],
            interactiveColliderType = InteractiveColliderType.MouseClick,
        };
        this.interactor.Interactive(args);
    }

    // External ray detection and click trigger
    public void ColliderInteractive(InteractiveColliderType interactiveColliderType)
    {
        if (interactiveColliderType == InteractiveColliderType.MouseClick)
        {
            OnClickAds();
        }
    }

    public void SetOwner(IInteractive ad)
    {
        this.interactor = ad;
    }
}