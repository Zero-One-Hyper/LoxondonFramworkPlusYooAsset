using System;
using UnityEngine;
using UnityEngine.EventSystems;


[DisallowMultipleComponent]
public class ProvinceInteract : MonoBehaviour, IInteractCollider
{
    private static int OpacityID = Shader.PropertyToID("_Opacity");
    private static float DefaultOpacity = 0f;
    private static float ActiveOpacity = 0.396f;

    private int _districtIndex;

    private MeshRenderer _renderer;
    private MaterialPropertyBlock _block;

    public IInteractive interactor { get; set; }

    void Awake()
    {
        this._renderer = this.transform.GetComponent<MeshRenderer>();
        this._block = new MaterialPropertyBlock();
    }

    public void ColliderInteractive(InteractiveColliderType interactiveColliderType)
    {
        switch (interactiveColliderType)
        {
            case InteractiveColliderType.MouseEnter:
                OnMouseEnterDistrict();
                break;
            case InteractiveColliderType.MouseExit:
                OnMouseExitDistrict();
                break;
            case InteractiveColliderType.MouseClick:
                OnMouseClickDistrict();
                break;
        }
    }

    public void SetOwner(IInteractive owner)
    {
        this.interactor = owner;
    }

    private void OnMouseExitDistrict()
    {
        this._block.SetFloat(OpacityID, DefaultOpacity);
        this._renderer.SetPropertyBlock(_block);

        InterActiveArgs data = new InterActiveArgs()
        {
            index = this._districtIndex,
            str = "MouseExit",
            interactiveColliderType = InteractiveColliderType.MouseExit,
        };
        interactor.Interactive(data);
    }

    private void OnMouseEnterDistrict()
    {
        this._block.SetFloat(OpacityID, ActiveOpacity);
        this._renderer.SetPropertyBlock(_block);

        InterActiveArgs data = new InterActiveArgs()
        {
            index = this._districtIndex,
            str = "MouseEnter",
            interactiveColliderType = InteractiveColliderType.MouseEnter,
        };
        interactor.Interactive(data);
    }

    private void OnMouseClickDistrict()
    {
        this._block.SetFloat(OpacityID, ActiveOpacity);
        this._renderer.SetPropertyBlock(_block);

        //发送消息
        InterActiveArgs data = new InterActiveArgs()
        {
            index = this._districtIndex,
            str = "Click",
            interactiveColliderType = InteractiveColliderType.MouseClick,
            gameObject = this.gameObject,
        };
        interactor.Interactive(data);
    }

    public void Init(int index)
    {
        this._districtIndex = index;

        this._block.SetFloat(OpacityID, DefaultOpacity);
        this._renderer.SetPropertyBlock(_block);
    }

    public int GetId()
    {
        return this._districtIndex;
    }
}
