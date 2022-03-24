using System;

namespace ET
{
    [MessageHandler]
    public class C2R_LoginRealmHandler : AMRpcHandler<C2R_LoginRealm, R2C_LoginRealm>
    {
        protected override async ETTask Run(Session session, C2R_LoginRealm request, R2C_LoginRealm response, Action reply)
        {
            Scene domainScene = session.DomainScene();
            if (domainScene.SceneType != SceneType.Realm)
            {
                Log.Error($"请求的Scene错误，当前场景为:{session.DomainScene().SceneType}");
                session.Dispose();
                return;
            }

            if (session.GetComponent<SessionLockingComponent>() != null)
            {
                response.Error = ErrorCode.ERR_RequestRepeadtedly;
                reply();
                session.Disconnect().Coroutine();
                return;
            }
            
            string token = domainScene.GetComponent<TokenComponent>().Get(request.AccountId);

            if (token == null || token != request.RealmTokenKey)
            {
                response.Error = ErrorCode.ERR_TokenError;
                reply();
                session?.Disconnect().Coroutine();
                return;
            }
            
            domainScene.GetComponent<TokenComponent>().Remove(request.AccountId);
            using (session.AddComponent<SessionLockingComponent>())
            {
                using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.LoginRealm, request.AccountId))
                {
                    // 取模固定分配一个Gate
                    StartSceneConfig config = RealmGateAddressHelper.GetGate(domainScene.Zone, request.AccountId);
                    
                    // 赂gate请求一个key，后面客户端使用这个key来和gate验证
                    G2R_GetLoginGateKey g2RGetLoginGateKey = (G2R_GetLoginGateKey) await MessageHelper.CallActor(config.InstanceId, new R2G_GetLoginGateKey()
                    {
                        AccountId = request.AccountId
                    });

                    if (g2RGetLoginGateKey.Error != ErrorCode.ERR_Success)
                    {
                        response.Error = g2RGetLoginGateKey.Error;
                        reply();
                        session?.Disconnect().Coroutine();
                        return;
                    }

                    response.GateSessionKey = g2RGetLoginGateKey.GateSessionKey;
                    response.GateAddress = config.OuterIPPort.ToString();
                    reply();
                    session?.Disconnect().Coroutine();
                }
            }
        }
    }
}