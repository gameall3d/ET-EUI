using System;
using NLog.LayoutRenderers;

namespace ET
{
    public class C2G_EnterGameHandler : AMRpcHandler<C2G_EnterGame, G2C_EnterGame>
    {
        protected override async ETTask Run(Session session, C2G_EnterGame request, G2C_EnterGame response, Action reply)
        {
            Scene domainScene = session.DomainScene();
            if (domainScene.SceneType != SceneType.Gate)
            {
                Log.Error($"请求的Scene错误，当前场景为:{session.DomainScene().SceneType}");
                session.Dispose();
                return;
            }

            if (session.GetComponent<SessionLockingComponent>() != null)
            {
                response.Error = ErrorCode.ERR_RequestRepeadtedly;
                reply();
                return;
            }

            var sessionPlayerComponent = session.GetComponent<SessionPlayerComponent>();
            if (null == sessionPlayerComponent)
            {
                response.Error = ErrorCode.ERR_SessionPlayerError;
                reply();
                return;
            }

            Player player = Game.EventSystem.Get(sessionPlayerComponent.PlayerInstanceId) as Player;

            if (player == null || player.IsDisposed)
            {
                response.Error = ErrorCode.ERR_NonePlayerError;
                reply();
                return;
            }

            long instanceId = session.InstanceId;

            using (session.AddComponent<SessionLockingComponent>())
            {
                using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.LoginGate, player.AccountId.GetHashCode()))
                {
                    if (instanceId != session.InstanceId || player.IsDisposed)
                    {
                        response.Error = ErrorCode.ERR_SessionPlayerError;
                        reply();
                        return;
                    }

                    var sessionStateComp = session.GetComponent<SessionStateComponent>();
                    if (sessionStateComp != null && sessionStateComp.State == SessionState.Game)
                    {
                        response.Error = ErrorCode.ERR_SessionStateError;
                        reply();
                        return;
                    }

                    if (player.PlayerState == PlayerState.Game)
                    {
                        try
                        {
                            IActorResponse reqEnter = await MessageHelper.CallLocationActor(player.UnitId, new G2M_RequestEnterGameState());
                            if (reqEnter.Error == ErrorCode.ERR_Success)
                            {
                                reply();
                                return;
                            }
                            
                            Log.Error("二次登入失败 " + reqEnter.Error + "|" + reqEnter.Message);
                            response.Error = ErrorCode.ERR_ReEnterGameError;
                            await DisconnectHelper.KickPlayer(player, true);
                            reply();
                            session?.Disconnect().Coroutine();
                        }
                        catch (Exception e)
                        {
                            Log.Error("二次登入失败 " + e.ToString());
                            response.Error = ErrorCode.ERR_ReEnterGameError;
                            await DisconnectHelper.KickPlayer(player, true);
                            reply();
                            session?.Disconnect().Coroutine();
                            throw;
                        }
                        
                        return;
                    }

                    try
                    {
                        GateMapComponent gateMapComponent = player.AddComponent<GateMapComponent>();
                        gateMapComponent.Scene = await SceneFactory.Create(gateMapComponent, "GateMap", SceneType.Map);
                        
                        Unit unit = UnitFactory.Create(gateMapComponent.Scene, player.Id, UnitType.Player);
                        unit.AddComponent<UnitGateComponent, long>(session.InstanceId);
                        long unitId = unit.Id;

                        StartSceneConfig startSceneConfig = StartSceneConfigCategory.Instance.GetBySceneName(session.DomainZone(), "Map1");
                        await TransferHelper.Transfer(unit, startSceneConfig.InstanceId, startSceneConfig.Name);

                        player.UnitId = unitId;
                        response.UnitId = unitId;

                        reply();

                        SessionStateComponent sessionStateComponent = session.GetComponent<SessionStateComponent>();
                        sessionStateComponent.State = SessionState.Game;
                        player.PlayerState = PlayerState.Game;
                    }
                    catch (Exception e)
                    {
                        Log.Error($"角色进入游戏逻辑服出现问题 账号Id:{player.AccountId} 角色Id:{player.Id} 异常信息:{e.ToString()}");
                        response.Error = ErrorCode.ERR_EnterGameError;
                        reply();
                        await DisconnectHelper.KickPlayer(player, true);
                        session.Disconnect().Coroutine();
                    }
                }
            }
        }
    }
}