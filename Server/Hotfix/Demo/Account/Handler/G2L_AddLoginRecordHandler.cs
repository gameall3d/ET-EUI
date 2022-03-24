using System;

namespace ET
{
    public class G2L_AddLoginRecordHandler : AMActorRpcHandler<Scene, G2L_AddLoginRecord, L2G_AddLoginRecord>
    {
        protected override async ETTask Run(Scene scene, G2L_AddLoginRecord request, L2G_AddLoginRecord response, Action reply)
        {
            long accountId = request.AccountId;
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.LoginCenter, accountId.GetHashCode()))
            {
                var loginInfoRecordComp = scene.GetComponent<LoginInfoRecordComponent>();
                loginInfoRecordComp.Remove(accountId);
                loginInfoRecordComp.Add(accountId, request.ServerId);
            }

            reply();
        }
    }
}