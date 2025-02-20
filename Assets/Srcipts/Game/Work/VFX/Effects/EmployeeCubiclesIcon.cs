using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[DisallowMultipleComponent]
public class EmployeeCubiclesIcon : MonoBehaviour, IInteractCollider
{
    public IInteractive interactor { get; set; }

    public void SetOwner(IInteractive owner)
    {
        this.interactor = owner;
    }
    private void OnClickEmployeeCubicle()
    {
        this.interactor.Interactive(null);
    }

    //外部射线检测后点击触发
    public void ColliderInteractive(InteractiveColliderType interactiveColliderType)
    {
        OnClickEmployeeCubicle();
    }
}
