#if UNITY_EDITOR
using DG.Tweening;
using UnityEngine;

public class BezierTest : MonoBehaviour
{
    [SerializeField]
    private GameObject _ipSceneReceptionGameObject;
    [SerializeField]
    private GameObject _ipRoamingGameObject;

    public float controlPointUp;
    public float controlPointLeft;
    private Vector3 _scneenIPDefaultPosition;
    private Vector3 _romaingIPDefaultPosition;

    private void DoAnim()
    {
        _romaingIPDefaultPosition = _ipRoamingGameObject.transform.position;
        _scneenIPDefaultPosition = _ipSceneReceptionGameObject.transform.position;

        Vector3 controlPoint = _romaingIPDefaultPosition + ((_scneenIPDefaultPosition - _romaingIPDefaultPosition) * 0.5f);
        controlPoint += Vector3.up * 0.5f + Vector3.back * 0.5f;
        Vector3[] pathvec = Bezier2Path(_scneenIPDefaultPosition, controlPoint, _romaingIPDefaultPosition);
        _ipSceneReceptionGameObject.transform.DOPath(pathvec, 1.0f).OnComplete(() =>
        {
            _ipSceneReceptionGameObject.transform.position = _scneenIPDefaultPosition;
            //_isShowingRomaingIP = true;
            //发送打开IP的消息
            XLog.I("发送打开IP消息");
            //_webService.UnityCallWeb(Constant.UNITY_SEND_IP_OPEN_STATE, "true");
            //_animCallBack?.Invoke();
        });
    }
    private void OnDrawGizmos()
    {
        _romaingIPDefaultPosition = _ipRoamingGameObject.transform.position;
        _scneenIPDefaultPosition = _ipSceneReceptionGameObject.transform.position;

        Gizmos.color = Color.yellow;
        Vector3 controlPoint = _romaingIPDefaultPosition + ((_scneenIPDefaultPosition - _romaingIPDefaultPosition) * 0.5f);
        controlPoint += Vector3.up * controlPointUp + Vector3.back * controlPointLeft;
        Vector3[] pathvec = Bezier2Path(_scneenIPDefaultPosition, controlPoint, _romaingIPDefaultPosition);
        for (int i = 0; i < _pointCount; i++)
        {
            Gizmos.DrawSphere(pathvec[i], 0.02f);
        }
    }
    public float _pointCount = 10;
    //获取二阶贝塞尔曲线路径数组
    private Vector3[] Bezier2Path(Vector3 startPos, Vector3 controlPos, Vector3 endPos)
    {
        Vector3[] path = new Vector3[(int)_pointCount];
        for (int i = 1; i <= _pointCount; i++)
        {
            float t = i / _pointCount;
            path[i - 1] = Bezier2(startPos, controlPos, endPos, t);
        }
        return path;
    }
    // 2阶贝塞尔曲线
    public static Vector3 Bezier2(Vector3 startPos, Vector3 controlPos, Vector3 endPos, float t)
    {
        return (1 - t) * (1 - t) * startPos + 2 * t * (1 - t) * controlPos + t * t * endPos;
    }

    // 3阶贝塞尔曲线
    public static Vector3 Bezier3(Vector3 startPos, Vector3 controlPos1, Vector3 controlPos2, Vector3 endPos, float t)
    {
        float t2 = 1 - t;
        return t2 * t2 * t2 * startPos
            + 3 * t * t2 * t2 * controlPos1
            + 3 * t * t * t2 * controlPos2
            + t * t * t * endPos;
    }
}
#endif
