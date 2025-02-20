using DG.Tweening;
using UnityEngine;

public class IP : MonoBehaviour
{
    [SerializeField]
    private Transform _followTarget;
    [SerializeField]
    private Vector3 _followOffset = new Vector3(-0.04830004f, 0.5362f, 0.1194f);
    private bool _isFollowing;

    public void Init(Transform followTarget)
    {
        this._followTarget = followTarget;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.transform.position = _followTarget.position + _followOffset;
    }

    // Update is called once per frame
    void Update()
    {
        if (_isFollowing)
        {
            this.transform.position = _followTarget.position + _followOffset;
            this.transform.LookAt(_followTarget.parent.position + Vector3.up * 0.55f);
        }
    }
    public void EnableFollow()
    {
        this.DOKill();
        _isFollowing = true;
    }

    public void DisableFollow()
    {
        _isFollowing = false;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(this.transform.position, this.transform.forward);
    }
#endif
}
