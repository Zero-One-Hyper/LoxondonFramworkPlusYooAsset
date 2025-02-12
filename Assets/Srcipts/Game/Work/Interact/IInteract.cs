using UnityEngine;

public interface IInteractive
{
    void Interactive(InterActiveArgs data);
}

public interface IInteractCollider
{
    public IInteractive interactor { get; set; }
    void SetOwner(IInteractive owner);
    void ColliderInteractive(InteractiveColliderType interactiveType);
}

public enum DoorTriggerType
{
    Enter = 0,
    Stay = 1,
    Exit = 2,
}
public class InterActiveArgs
{
    public InteractiveColliderType interactiveColliderType;
    public DoorTriggerType doorTriggerType;
    public GameObject gameObject;
    public Vector3 vector3;
    public int index;
    public float floatValue;
    public string str;
}

public enum InteractiveColliderType
{
    MouseEnter,
    MouseExit,
    MouseClick,
}