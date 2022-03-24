﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace ET
{
    public static class DisconnectHelper
    {
        public static async ETTask Disconnect(this Session self)
        {
            if (self == null || self.IsDisposed)
            {
                return;
            }

            long instanceId = self.InstanceId;

            await TimerComponent.Instance.WaitAsync(1000);

            if (self.InstanceId != instanceId)
            {
                return;
            }

            self.Dispose();
        }

        public static async ETTask KickPlayer(Player player, bool isException = false)
        {
            if (player == null || player.IsDisposed)
            {
                return;
            }

            long instanceId = player.InstanceId;
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.LoginGate, player.AccountId.GetHashCode()))
            {
                if (player.IsDisposed || instanceId != player.InstanceId)
                {
                    return;
                }

                if (!isException)
                {
                    switch (player.PlayerState)
                    {
                        case PlayerState.Disconnect:
                            break;
                        case PlayerState.Gate:
                            break;
                        case PlayerState.Game:
                            // 通知游戏逻辑服下线Unit角色逻辑，并将数据存入数据库
                            M2G_RequestExitGame m2GRequestExitGame =
                                    (M2G_RequestExitGame) await MessageHelper.CallLocationActor(player.UnitId, new G2M_RequestExitGame());
                            
                            // 通知移除账号角色登入信息
                            long LoginCenterConfigSceneId = StartSceneConfigCategory.Instance.LoginCenterConfig.InstanceId;
                            L2G_RemoveLoginRecord l2GRemoveLoginRecord = (L2G_RemoveLoginRecord) await MessageHelper.CallActor(
                                LoginCenterConfigSceneId, new G2L_RemoveLoginRecord() { AccountId = player.AccountId, ServerId = player.DomainZone() });
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            player.PlayerState = PlayerState.Disconnect;
            player.DomainScene().GetComponent<PlayerComponent>()?.Remove(player.AccountId);
            player?.Dispose();
            
            // 为了让上面player的释放有足够的时间？有点奇怪
            await TimerComponent.Instance.WaitAsync(300);
        }
    }
}
