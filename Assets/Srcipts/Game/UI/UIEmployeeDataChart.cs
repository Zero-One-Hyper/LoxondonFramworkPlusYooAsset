using DG.Tweening;
using Loxodon.Framework.Binding.Builder;
using Loxodon.Framework.Contexts;
using Loxodon.Framework.Views;
using LP.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIEmployeeDataChart : ViewBase
{
    private CanvasGroup _canvasGroup;
    private Button _spaceButton;
    private TMP_Text _job;//职位
    private TMP_Text _name;//姓名
    private TMP_Text _department;//部门
    private TMP_Text _state;//状态
    [SerializeField]
    private TMP_Text _introduction;//简介
    [SerializeField]
    private Button _employeeCloseButton;
    private Button _noEmployeeCloseButton;
    //private ContentSizeFitter _contextSizeFitter;
    //private ContentSizeFitter _introductionSizeFitter;
    //private ContentSizeFitter _introductionTextSizeFitter;

    private Transform _haveEmplpyeePannel;
    private Transform _noEmplpyeePannel;

    private EmployeeData _employeeData;

    public override void InitUI(params object[] args)
    {
        base.InitUI(args);
        this._canvasGroup = this.GetComponent<CanvasGroup>();

        _haveEmplpyeePannel = this.transform.GetChild(1);
        _noEmplpyeePannel = this.transform.GetChild(2);

        Transform employeeContext = _haveEmplpyeePannel.GetChild(1);
        this._name = employeeContext.GetChild(0).GetChild(1).GetChild(0).GetComponent<TMP_Text>();
        this._department = employeeContext.GetChild(1).GetChild(1).GetChild(0).GetComponent<TMP_Text>();
        this._job = employeeContext.GetChild(2).GetChild(1).GetChild(0).GetComponent<TMP_Text>();
        this._state = employeeContext.GetChild(3).GetChild(1).GetChild(0).GetComponent<TMP_Text>();
        this._introduction = employeeContext.GetChild(4).GetChild(1).GetChild(1).GetComponent<TMP_Text>();

        this._employeeCloseButton = _haveEmplpyeePannel.GetChild(2).GetComponent<Button>();
        this._employeeCloseButton.onClick.AddListener(this.OnClickCloseButton);

        this._noEmployeeCloseButton = _noEmplpyeePannel.GetChild(2).GetComponent<Button>();
        this._noEmployeeCloseButton.onClick.AddListener(this.OnClickCloseButton);

        //点击空白处关闭
        this._spaceButton = this.transform.GetChild(0).GetComponent<Button>();
        this._spaceButton.onClick.AddListener(this.OnClickCloseButton);

        _noEmplpyeePannel.gameObject.SetActive(false);

        //处理参数
        if (args != null)
        {
            this.SetData(args[0] as EmployeeData);
        }
        _canvasGroup.alpha = 0;
    }
    private void Start()
    {
        //BindingSet<UIEmployeeDataChart, EmployeeData> bindingSet;
        //bindingSet = this.CreateBindingSet<UIEmployeeDataChart, EmployeeData>();
    }

    private void OnClickCloseButton()
    {
        this._canvasGroup.DOFade(0, 0.5f).OnComplete(() =>
        {
            var viewService = Context.GetApplicationContext().GetService<IViewService>();
            viewService.CloseView<UIEmployeeDataChart>();
            //销毁会由ViewManager触发 并触发OnClose
        });
    }
    public override void OnShow()
    {
        base.OnShow();
        if (this._employeeData != null)
        {
            RefreshUI();
        }
        this._canvasGroup.DOFade(1, 0.5f);
    }

    public override void OnClose()
    {
        base.OnClose();
        //由ViewManager触发
    }

    public void SetData(EmployeeData data)
    {
        _employeeData = data;
    }

    //刷新UI
    public override void RefreshUI()
    {
        if (string.IsNullOrEmpty(this._employeeData.name))
        {
            //认为当前没有员工
            this._noEmplpyeePannel.gameObject.SetActive(true);
            this._haveEmplpyeePannel.gameObject.SetActive(false);
            return;
        }

        this._noEmplpyeePannel.gameObject.SetActive(false);
        this._haveEmplpyeePannel.gameObject.SetActive(true);
        _name.text = this._employeeData.name;
        _department.text = this._employeeData.department;
        _job.text = this._employeeData.posts;
        _state.text = this._employeeData.state;
        this._employeeData.introduction = this._employeeData.introduction.Replace("\n", "");
        this._employeeData.introduction = this._employeeData.introduction.Replace(" ", "");
        if (this._employeeData.introduction.Length > 100)
        {
            string str = this._employeeData.introduction.Substring(0, 100);
            _introduction.text = str;
        }
        else
        {
            _introduction.text = this._employeeData.introduction;
        }
    }
    /*
    文字文文字文字文文字文字文文字文字文文字文字文文字文字文文字文字文文字文字文文字文字文文字文字文文字文字文文字文字文文字文字文文字文字文文字文字文文字文字文文字文字文文字文字文文字文字文文字文字文文字
    */
}
