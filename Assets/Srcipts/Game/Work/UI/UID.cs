using System.IO;

public class UID
{
    public readonly static string UI_RES_PATH = "Prefabs/UI/";

    public static string GetUIName(UIID uiid)
    {
        return Path.Combine(UI_RES_PATH, uiid.ToString());
    }
}
public enum UIID
{
    MaskView
}