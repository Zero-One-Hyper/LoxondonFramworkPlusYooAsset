using System.Threading.Tasks;
using Loxodon.Framework.Contexts;
using UnityEngine;


namespace LP.Framework
{
    public class EmployeeCubicleRunLoad : IRunLoad
    {
        public async Task Load()
        {

            ApplicationContext context = Context.GetApplicationContext();

            IEmployeeService employeeManager = new EmployeeManager();
            employeeManager.Init();
            //尝试在其他地方加载工位的内容
            //employeeManager.InitEmployeeCubicleData();
            //注册
            Context.GetApplicationContext().GetContainer().Register<IEmployeeService>(employeeManager);
        }
    }
}