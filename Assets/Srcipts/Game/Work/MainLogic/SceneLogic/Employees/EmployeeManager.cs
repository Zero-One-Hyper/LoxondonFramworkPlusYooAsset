using System.Collections.Generic;
using System.Threading.Tasks;
using Loxodon.Framework.Contexts;
using LP.Framework;
using UnitData.Const;
using UnityEngine;

public interface IEmployeeService : IService
{
    void AddShowEmployeeCubicle(EmployeeCubicle employeeCubicle);
    Task<bool> InitEmployeeCubicleData();
    void QueryEmployeeInfomation(string cubicleId);
    void RemoveShowEmployeeCubicle(EmployeeCubicle employeeCubicle);
}

public class EmployeeManager : IEmployeeService
{
    private LayerMask _cubicleColliderLayer;
    private ApplicationContext _context;

    private IViewService _viewManager;
    private bool _initializedCubicle = false;

    private List<EmployeeCubicle> _nearestEmployee = new List<EmployeeCubicle>();

    public bool HasInitialized()
    {
        return _initializedCubicle;
    }

    public void Init()
    {
        _cubicleColliderLayer = LayerMask.NameToLayer("InteractiveCollider");

        _context = Context.GetApplicationContext();
        _viewManager = _context.GetService<IViewService>();
        IInputService inputService = _context.GetService<IInputService>();
        inputService.RegisterCameraControl(InputActionType.Move, GameScene.Roaming, OnPlayerMove);

        var eventHandleService = _context.GetService<IEventHandleService>();
        eventHandleService.AddListener(EmployeeDataEvent.SingleEmployeeInfomation, OnReciveSingleEmployeeData);
    }

    //为每个模块添加脚本
    public async Task<bool> InitEmployeeCubicleData()
    {
        if (this.HasInitialized())
        {
            return true;
        }
        _initializedCubicle = true;
        var sceneGameObjectService = _context.GetService<ISceneGameObjectService>();
        var assetLoad = _context.GetService<IAssetLoadUtil>();

        //拿到工位模型
        GameObject root = await sceneGameObjectService.TryGetSceneGameObject("EmployeeCubicles");
        //加载工位数据
        var textAsset = await assetLoad.ResourceLoadAsync<TextAsset>("EmployeeCubicleData");

        if (textAsset == null || string.IsNullOrEmpty(textAsset.text))
        {
            XLog.E("工位数据加载失败");
            return false;
        }

        AllEmployeeCubicleDataes allData = JsonUtility.FromJson<AllEmployeeCubicleDataes>(textAsset.text);
        if (allData == null || allData.employeeCubicleDataList == null)
        {
            XLog.E("工位数据json反序列化失败");
            return false;
        }

        List<EmployeeCubicleData> datas = allData.employeeCubicleDataList;
        if (datas == null || datas.Count <= 0)
        {
            XLog.E("工位数据为空");
            return false;
        }

        for (int i = 0; i < root.transform.childCount; i++)
        {
            Transform employeeCubicleTransform = root.transform.GetChild(i);
            employeeCubicleTransform.gameObject.layer = _cubicleColliderLayer;
            EmployeeCubicle employeeCubicle = employeeCubicleTransform.gameObject.AddComponent<EmployeeCubicle>();
            employeeCubicle.Init(datas[i]);
        }


        XLog.I("加载工位数据");
        return true;
    }

    public void QueryEmployeeInfomation(string cubicleId)
    {
        IWebService webService = _context.GetService<IWebService>();
        XLog.I($"发送点击工位{cubicleId}事件");
        webService.UnityCallWeb(Constant.UNITY_CALL_QUERY_EMPLOYEE_INFOMATION, cubicleId);
    }

    //判断范围内最近的六个工位 添加经显示列表
    public void AddShowEmployeeCubicle(EmployeeCubicle employeeCubicle)
    {
        if (_nearestEmployee.Contains(employeeCubicle))
        {
            return;
        }

        //与列表中的工位做对比 
        if (_nearestEmployee.Count < 6)
        {
            //直接添加
            _nearestEmployee.Add(employeeCubicle);
            employeeCubicle.Show();
        }
        else
        {
            int farestEmployeeIndex = 0;
            for (int i = 1; i < _nearestEmployee.Count; i++)
            {
                //找其中最大的
                if (_nearestEmployee[i].GetDistanceToCamera() >
                    _nearestEmployee[farestEmployeeIndex].GetDistanceToCamera())
                {
                    farestEmployeeIndex = i;
                }
            }

            //对比添加的工位和最大的工位
            if (employeeCubicle.GetDistanceToCamera() < _nearestEmployee[farestEmployeeIndex].GetDistanceToCamera())
            {
                _nearestEmployee[farestEmployeeIndex].Hide();
                _nearestEmployee[farestEmployeeIndex] = employeeCubicle;
                employeeCubicle.Show();
            }
        }
    }

    public void RemoveShowEmployeeCubicle(EmployeeCubicle employeeCubicle)
    {
        if (_nearestEmployee.Contains(employeeCubicle))
        {
            employeeCubicle.Hide();
            _nearestEmployee.Remove(employeeCubicle);
        }
    }

    private void OnReciveSingleEmployeeData(EventArgs args)
    {
        //解析内容
        EmployeeDataArgs arg = args as EmployeeDataArgs;
        if (arg == null)
        {
            XLog.E("查询单个工位信息数据为空");
            return;
        }

        if (string.IsNullOrEmpty(arg.Json))
        {
            XLog.E("单个工位数据为空");
            return;
        }

        //解析数据
        AllEmployeeDatas allEmployeeDatas = JsonUtility.FromJson<AllEmployeeDatas>(arg.Json);
        if (allEmployeeDatas == null)
        {
            XLog.E("解析工位信息数据错误");
            return;
        }

        if (allEmployeeDatas.datas == null || allEmployeeDatas.datas.Count <= 0)
        {
            XLog.E("单个工位信息为空");
            return;
        }

        XLog.I("接收到工位信息");
        ShowUIEmployeeDataChart(allEmployeeDatas.datas[0]);
    }

    private void ShowUIEmployeeDataChart(EmployeeData data)
    {
        _viewManager.OpenView<UIEmployeeDataChart>(data);
    }

    private void OnPlayerMove(Vector2 vector)
    {
        _viewManager.CloseView<UIEmployeeDataChart>();
    }
}