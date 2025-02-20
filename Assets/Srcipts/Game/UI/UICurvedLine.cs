using UnityEngine;

public class UICurvedLine : MonoBehaviour
{
    [SerializeField] private LineRenderer _lineRenderer;
    //相机位置X需要减少0.5
    [SerializeField] private Transform _startPoint; // 起点
    [SerializeField] private Transform _endPoint; // 终点
    private Vector3 _endposition;
    [SerializeField] private float _curvature = 0.5f; // 控制曲度（0-1之间）
    [SerializeField] private int _pointCount = 10; // 点的数量
    [SerializeField] private Vector3 _offset;
    private Camera _maincamera;

    private void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _maincamera=Camera.main;
    }

    public void SetStart(Transform startobj )
    {
        _startPoint = startobj;
    }

    private void OnValidate()
    {
        UpdateLine();
    }

    private void Update()
    {
        UpdateLine();
    }

    public void UpdateLine()
    {
       
        _lineRenderer.positionCount = _pointCount;
        // 确保至少有2个点
        if (_pointCount < 2)
        {
            return;
        }
        _endposition = GetWorldPositionSimple();
        
        // 计算控制点（用于贝塞尔曲线）
        Vector3 midPoint = (_startPoint.position + _endposition) * 0.5f;
        Vector3 perpendicular = Vector3.Cross(_endposition - _startPoint.position, Vector3.up).normalized;
        Vector3 controlPoint = midPoint +
                               perpendicular * (Vector3.Distance(_startPoint.position, _endposition) *
                                                _curvature);

        // 生成曲线上的点
        for (int i = 0; i < _pointCount; i++)
        {
            float t = i / (float)(_pointCount - 1);
            _lineRenderer.SetPosition(i,
                CalculateQuadraticBezierPoint(t, _startPoint.position, controlPoint, _endposition));
        }
    }

    // 计算二次贝塞尔曲线上的点
    private Vector3 CalculateQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        return u * u * p0 + 2 * u * t * p1 + t * t * p2;
    }

    // 设置点的数量默认十个
    public void SetPointCount(int count=10)
    {
        _pointCount = count;
        UpdateLine();
    }

    // 设置曲度
    public void SetCurvature(float curve)
    {
        _curvature = Mathf.Clamp(curve, -1f, 1f);
        UpdateLine();
    }
    //计算相机中的点
    public Vector3 GetWorldPositionSimple()
    {
        _offset.z=_maincamera.WorldToScreenPoint(Vector3.zero).z;
        Vector3 worldPos = _maincamera.ScreenToWorldPoint(_offset);
        worldPos.y = 0; // 强制Y坐标为指定值
        return worldPos;
    }
}