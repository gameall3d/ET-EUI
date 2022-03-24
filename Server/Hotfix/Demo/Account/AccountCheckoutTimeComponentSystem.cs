using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET
{
    [Timer(TimerType.AccountSessionCheckoutTime)]
    public class AccountSessionCheckoutTimer : ATimer<AccountCheckoutTimeComponent>
    {
        public override void Run(AccountCheckoutTimeComponent self)
        {
            try
            {
                self.DeleteSession();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }
    }

    public class AccountCheckoutTimeComponentAwakeSystem : AwakeSystem<AccountCheckoutTimeComponent, long>
    {
        public override void Awake(AccountCheckoutTimeComponent self, long accountId)
        {
            self.AccountId = accountId;
            TimerComponent.Instance.Remove(ref self.Timer);
            self.Timer = TimerComponent.Instance.NewOnceTimer(TimeHelper.ServerNow() + 60000, TimerType.AccountSessionCheckoutTime, self);
        }
    }

    public class AccountCheckoutTimeComponentDestroySystem : DestroySystem<AccountCheckoutTimeComponent>
    {
        public override void Destroy(AccountCheckoutTimeComponent self)
        {
            self.AccountId = 0;
            TimerComponent.Instance.Remove(ref self.Timer);
        }
    }
    public static class AccountCheckoutTimeComponentSystem
    {
        public static void DeleteSession(this AccountCheckoutTimeComponent self)
        {
            Session session = self.GetParent<Session>();
            long sessionInstanceId = session.DomainScene().GetComponent<AccountSessionsComponent>().Get(self.AccountId);
            if (session.InstanceId == sessionInstanceId)
            {
                session.DomainScene().GetComponent<AccountSessionsComponent>().Remove(self.AccountId);
            }

            session.Send(new A2C_Disconnect() { Error = 0 });
            session.Disconnect().Coroutine();
        }
    }
}
