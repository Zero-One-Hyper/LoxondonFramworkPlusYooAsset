using System;
using UnityEngine;
using UnityEngine.AI;

[DisallowMultipleComponent]
public class GuildLines : MonoBehaviour
{
    private LineRenderer _line;

    private void Awake()
    {
        _line = this.GetComponent<LineRenderer>();
    }

    public void SetLinePath(NavMeshPath path)
    {
        _line.positionCount = path.corners.Length;
        _line.SetPositions(path.corners);
    }
    public void Init()
    {
    }

}