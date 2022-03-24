using System;

namespace ET
{
    public class C2G_LoginGameGateHandler : AMRpcHandler<C2G_LoginGameGate, G2C_LoginGameGate>
    {
        protected override async ETTask Run(Session session, C2G_LoginGameGate request, G2C_LoginGameGate response, Action reply)
        {
            Scene domainScene = session.DomainScene();
            if (domainScene.SceneType != SceneType.Gate)
            {
                Log.Error($"请求的Scene错误，当前场景为:{session.DomainScene().SceneType}");
                session.Dispose();
                return;
            }
            session.RemoveComponent<SessionAcceptTimeoutComponent>();

            if (session.GetComponent<SessionLockingComponent>() != null)
            {
                response.Error = ErrorCode.ERR_RequestRepeadtedly;
                reply();
                session.Disconnect().Coroutine();
                return;
            }
            
            string token = domainScene.GetComponent<GateSessionKeyComponent>().Get(request.AccountId);

            if (token == null || token != request.GateTokenKey)
            {
                response.Error = ErrorCode.ERR_ConnectGateKeyError;
                response.Message = "Gate Key验证失败";
                reply();
                session?.Disconnect().Coroutine();
                return;
            }
            
            domainScene.GetComponent<GateSessionKeyComponent>().Remove(request.AccountId);

            long instanceId = session.InstanceId;
            using (session.AddComponent<SessionLockingComponent>())
            {
                using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.LoginGate, request.AccountId))
                {
                    if (instanceId != session.InstanceId)
                    {
                        return;
                    }
                    
                    // 通知登入中心服 记录本次登入的服务器Zone
                    StartSceneConfig loginCenterConfig = StartSceneConfigCategory.Instance.LoginCenterConfig;
                    L2G_AddLoginRecord l2GAddLoginRecord = (L2G_AddLoginRecord) await MessageHelper.CallActor(loginCenterConfig.InstanceId,
                        new G2L_AddLoginRecord() { AccountId = request.AccountId, ServerId = domainScene.Zone });

                    if (l2GAddLoginRecord.Error != ErrorCode.ERR_Success)
                    {
                        response.Error = l2GAddLoginRecord.Error;
                        reply();
                        session?.Disconnect().Coroutine();
                        return;
                    }

                    SessionStateComponent sessionStateComponent = session.GetComponent<SessionStateComponent>();
                    if (sessionStateComponent == null)
                    {
                        sessionStateComponent = session.AddComponent<SessionStateComponent>();
                    }

                    sessionStateComponent.State = SessionState.Normal;

                    Player player = domainScene.GetComponent<PlayerComponent>().Get(request.AccountId);

                    if (player == null)
                    {
                        player = domainScene.GetComponent<PlayerComponent>()
                                .AddChildWithId<Player, long, long>(request.RoleId, request.AccountId, request.RoleId);
                        player.PlayerState = PlayerState.Gate;
                        domainScene.GetComponent<PlayerComponent>().Add(player);
                        session.AddComponent<MailBoxComponent, MailboxType>(MailboxType.GateSession);
                    }
                    else
                    {
                        player.RemoveComponent<PlayerOfflineOutTimeComponent>();
                    }

                    var sessionPlayerComp = session.AddComponent<SessionPlayerComponent>();
                    sessionPlayerComp.PlayerId = player.Id;
                    sessionPlayerComp.PlayerInstanceId = player.InstanceId;
                    sessionPlayerComp.AccountId = request.AccountId;
                    player.SessionInstanceId = session.InstanceId;
                }
            }

            reply();
        }
    }
}