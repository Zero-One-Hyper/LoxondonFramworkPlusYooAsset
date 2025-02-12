using System.Collections.Generic;

namespace UnitData.Const
{
    /// <summary>
    /// 管理全局常量
    /// </summary>
    public class Constant
    {
        /*---------------------AB 常量---------------------*/
        public const string TF_SHADER_NAME = "PBR_URP_Lit";
        public const string DEF_BUNDLE_PATH = "tractionsystem/res/";
        public const string UNIT_SO_DATA = "unit_so_data";
        public const string MAT_MESH_SO_DATA = "mat_mesh_so_data";
        public const string CONF_DEF_PATH = "conf/default";

        /*---------------------鼠标事件向Web传参Type---------------------*/
        public const string CALL_WEB_CLICK = "UnityClick";
        public const string CALL_WEB_INTRO_CLICK = "UnityIntroClick";
        public const string CALL_WEB_INTRO_ENTER = "UnityIntroEnter";
        public const string CALL_WEB_MOUSE_POSITION = "UnityMousePos";
        public const string CALL_WEB_INTRO_EXIT = "UnityIntroExit";
        public const string CALL_WEB_AUTO_ROUNDING = "UnityAutoRounding";
        public const string CALL_WEB_START = "UnityStarted";

        public const string CALL_WEB_TP_CLICK = "UnityTPClick";

        /*---------------------Web传参数据Json---------------------*/
        public const string CALL_WEB_CKICK_JSON_DATA = "BackGround";
        public const string CALL_WEB_START_JSON_DATA = "UnityOn";
        public const string WEB_CALL_OBJ_NAME = "JSbridge";

        //web发送切换场景消息
        public const string WEB_CALL_SWITCH_SCENE = "OnClickSwitchScene";
        public const string WEB_CALL_SWITCH_UI_MASK = "OnSwitchUIMask";
        //web发送单个工位信息
        public const string WEB_SEND_EMPLOYEE_INFOMATION = "OnWebSendEmployeeinfomation";
        public const string WEB_SEND_ROOM_PERMISSION = "OnWebSendRoomPermission";
        //web发送点击自动漫游
        public const string WEB_CALL_AUTO_ROUNDING = "OnClickSwithAutoRoaming";
        //web发送开关原点IP
        public const string WEB_CALL_SWITCH_IP = "OnClickSwitchIP";
        //web发送所有房间数据
        public const string WEB_SEND_ALL_ROOM_NAME = "OnSendAllDoorplateData";
        //web发送单个房间数据
        public const string WEB_SEND_SINGLE_ROOM_NAME = "OnSendSingleRoomplateData";

        /*---------------------unity向web传递Json---------------------*/
        //unity查询用户权限
        public const string UNITY_CALL_USER_PERMISSION = "OnUnityCallUsersPermission";
        //unity查询单个工位信息
        public const string UNITY_CALL_QUERY_EMPLOYEE_INFOMATION = "OnUnityQueryEmployeeInfomation";
        //unity发送交互单个行政区消息
        public const string UNITY_CALL_INTERACT_DITRICK = "OnUnityInterActiveDistrict";
        //unity发送点击某个广告牌消息
        public const string UNITY_CALL_CLICK_ADS = "OnUnityClickAds";
        //unity发送 加载场景完成
        public const string UNITY_CALL_LOAD_COMPLETE = "OnUnityLoadComplete";
        //unity发送打开、关闭导览图
        public const string UNITY_CALL_NAVIGATION_MAP = "OnUnityNavigationMap";
        //unity发送IP开关状态
        public const string UNITY_SEND_IP_OPEN_STATE = "OnUnitySetIPActive";
        //unity发送开关路径漫游
        public const string UNITY_SEND_SWITCH_AUTOROAMING = "OnUnitySwitchAutoRoaming";

        /*---------------------PlayerPrefs Key---------------------*/
        public const string KEY_OTHER_SCENE_SWITCH = "OtherSceneSwitch";

        /*---------------------Http 请求---------------------*/
        public const string HTTP_REQ_ADD_RESS = "https://127.0.0.1";

        public static Dictionary<EOPERATION, string> HTTP_URL_DIC = new Dictionary<EOPERATION, string>
        {
            { EOPERATION.LOGIN, "webapi/login" }, { EOPERATION.REGISTER, "webapi/register" }
            //
        };
    }
}