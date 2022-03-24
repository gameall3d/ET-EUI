using UnityEngine;

namespace ET
{
    public enum RoleState
    {
        Normal = 0,
        Freeze,
    }
    
    public class RoleInfo : Entity, IAwake
    {
        public string Name;
        public int ServerId;
        public int State;
        public long AccountId;
        public long LastLoginTime;
        public long CreateTime;
    }
}