namespace ET
{
    public class RoleInfosComponentDestroySystem: DestroySystem<RoleInfosComponent>
    {
        public override void Destroy(RoleInfosComponent self)
        {
            foreach (var roleInfo in self.RoleInfos)
            {
                roleInfo?.Dispose();
            }
            
            self.RoleInfos.Clear();
            self.CurRoleId = 0;
        }
    }

    public static class RoleInfosComponentSystem
    {
        
    }
}