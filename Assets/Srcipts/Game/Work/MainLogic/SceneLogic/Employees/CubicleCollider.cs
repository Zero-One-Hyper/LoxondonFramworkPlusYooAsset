using UnityEngine;

public class CubicleCollider : MonoBehaviour, IInteractCollider
{

    public IInteractive interactor { get; set; }

    public void SetOwner(EmployeeCubicle owner)
    {
        this.interactor = owner;
    }
    private void OnClickEmployeeCubicle()
    {
        InterActiveArgs args = new InterActiveArgs() { };
        this.interactor.Interactive(args);
    }

    //外部射线检测后点击触发
    public void ColliderInteractive(InteractiveColliderType interactiveColliderType)
    {
        OnClickEmployeeCubicle();
    }

    public void SetOwner(IInteractive owner)
    {
        throw new System.NotImplementedException();
    }
}