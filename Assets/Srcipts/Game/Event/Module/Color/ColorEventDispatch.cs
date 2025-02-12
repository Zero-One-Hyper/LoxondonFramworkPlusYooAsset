using Loxodon.Framework.Contexts;
using LP.Framework;
using UnityEngine;

public class ColorEventDispatch : MonoBehaviour
{
    /// <summary>
    /// 改变颜色
    /// </summary>
    /// <param name="color"></param>
    public void ChangeColor(string color)
    {
        Color c;
        if (ColorUtility.TryParseHtmlString(color, out c))
        {
            Context.GetApplicationContext().GetService<IEventHandleService>().Allocate<ColorEventArgs>()
                .Register(ColorEvent.ChangeTo, gameObject, c)
                .Invoke();
        }
        else
        {
            Debug.Log("Html 颜色表达式格式不对！");
        }
    }
}