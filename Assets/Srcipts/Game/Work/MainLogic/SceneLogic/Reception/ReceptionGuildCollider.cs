using UnityEngine;

public class ReceptionGuildCollider : MonoBehaviour, IInteractCollider
{

    public IInteractive interactor { get; set; }

    public void ColliderInteractive(InteractiveColliderType interactiveColliderType)
    {
        interactor.Interactive(null);
    }

    public void SetOwner(IInteractive owner)
    {
        this.interactor = owner;
    }
}
