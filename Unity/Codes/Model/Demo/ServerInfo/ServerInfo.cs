namespace ET
{
    public enum ServerStaus
    {
        Normal = 0, // 正常状态
        Stop = 1,   // 停服状态
    }
    public class ServerInfo : Entity, IAwake
    {
        public int Status;
        public string ServerName;
    }
}