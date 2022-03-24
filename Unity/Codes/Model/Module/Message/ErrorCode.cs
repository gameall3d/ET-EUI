namespace ET
{
    public static partial class ErrorCode
    {
        public const int ERR_Success = 0;

        // 1-11004 是SocketError请看SocketError定义
        //-----------------------------------
        // 100000-109999是Core层的错误

        // 110000以下的错误请看ErrorCore.cs

        // 这里配置逻辑层的错误码
        // 110000 - 200000是抛异常的错误
        // 200001以上不抛异常

        public const int ERR_NetWorkError = 200002;     // 网络异常
        public const int ERR_LoginInfoError = 200003;   // 登入错误
        public const int ERR_RequestRepeadtedly = 200004;   // 重复请求
        public const int ERR_TokenError = 200005;

        public const int ERR_RoleNameIsNull = 200006;
        public const int ERR_RoleNameConflict = 200007;
        public const int ERR_RoleNotExist = 200008;
        public const int ERR_RequestSceneTypeError = 200009;
        public const int ERR_ConnectGateKeyError = 200010;
        public const int ERR_OtherAccountLogin = 200011;
        public const int ERR_SessionPlayerError = 200012;
        public const int ERR_NonePlayerError = 200013;
        public const int ERR_SessionStateError = 200014;
        public const int ERR_EnterGameError = 200015;
        public const int ERR_ReEnterGameError = 200016;
    }
}