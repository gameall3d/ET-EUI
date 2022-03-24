using System;


namespace ET
{
    public static class LoginHelper
    {
        public static async ETTask<int> Login(Scene zoneScene, string address, string account, string password)
        {
            //try
            //{
            //    // 创建一个ETModel层的Session
            //    R2C_Login r2CLogin;
            //    Session session = null;
            //    try
            //    {
            //        session = zoneScene.GetComponent<NetKcpComponent>().Create(NetworkHelper.ToIPEndPoint(address));
            //        {
            //            r2CLogin = (R2C_Login) await session.Call(new C2R_Login() { Account = account, Password = password });
            //        }
            //    }
            //    finally
            //    {
            //        session?.Dispose();
            //    }

            //    // 创建一个gate Session,并且保存到SessionComponent中
            //    Session gateSession = zoneScene.GetComponent<NetKcpComponent>().Create(NetworkHelper.ToIPEndPoint(r2CLogin.Address));
            //    gateSession.AddComponent<PingComponent>();
            //    zoneScene.AddComponent<SessionComponent>().Session = gateSession;

            //    G2C_LoginGate g2CLoginGate = (G2C_LoginGate)await gateSession.Call(
            //        new C2G_LoginGate() { Key = r2CLogin.Key, GateId = r2CLogin.GateId});

            //    Log.Debug("登陆gate成功!");

            //    await Game.EventSystem.PublishAsync(new EventType.LoginFinish() {ZoneScene = zoneScene});
            //}
            //catch (Exception e)
            //{
            //    Log.Error(e);
            //}

            A2C_LoginAccount a2CLoginAccount = null;
            Session accountSession = null;

            try
            {
                accountSession = zoneScene.GetComponent<NetKcpComponent>().Create(NetworkHelper.ToIPEndPoint(address));
                password = MD5Helper.StringMD5(password);
                a2CLoginAccount = (A2C_LoginAccount) await accountSession.Call(new C2A_LoginAccount() {  AccountName = account, Password = password });
            }
            catch (Exception e)
            {
                accountSession?.Dispose();
                Log.Error(e.ToString());
            }

            if (a2CLoginAccount.Error != ErrorCode.ERR_Success)
            {
                accountSession?.Dispose();
                return a2CLoginAccount.Error;
            }

            zoneScene.AddComponent<SessionComponent>().Session = accountSession;    // 保存Session
            zoneScene.GetComponent<SessionComponent>().Session.AddComponent<PingComponent>();

            AccountInfoComponent acountInfoComp = zoneScene.GetComponent<AccountInfoComponent>();
            acountInfoComp.Token = a2CLoginAccount.Token;
            acountInfoComp.AccountId = a2CLoginAccount.AccountId; 

            return ErrorCode.ERR_Success;
        }

        public static async ETTask<int> GetServerInfos(Scene zoneScene)
        {
            A2C_ServerInfosResponse a2CServerInfos = null;
            try
            {
                a2CServerInfos = (A2C_ServerInfosResponse) await zoneScene.GetComponent<SessionComponent>().Session.Call(new C2A_ServerInfosRequest()
                {
                    AccountId = zoneScene.GetComponent<AccountInfoComponent>().AccountId,
                    Token = zoneScene.GetComponent<AccountInfoComponent>().Token
                });
            }
            catch (Exception e)
            {
                Log.Error(e);
                return ErrorCode.ERR_NetWorkError;
            }

            if (a2CServerInfos.Error != ErrorCode.ERR_Success)
            {
                return a2CServerInfos.Error;
            }

            foreach (var serverInfoProto in a2CServerInfos.ServerInfoList)
            {
                ServerInfo serverInfo = zoneScene.GetComponent<ServerInfosComponent>().AddChild<ServerInfo>();
                serverInfo.FromMessage(serverInfoProto);
                zoneScene.GetComponent<ServerInfosComponent>().Add(serverInfo);
            }
            
            return ErrorCode.ERR_Success;
        }

        public static async ETTask<int> CreateRole(Scene zoneScene, string name)
        {
            A2C_CreateRole a2CCreateRole = null;

            try
            {
                a2CCreateRole = (A2C_CreateRole) await zoneScene.GetComponent<SessionComponent>().Session.Call(new C2A_CreateRole()
                {
                    AccountId = zoneScene.GetComponent<AccountInfoComponent>().AccountId,
                    Token = zoneScene.GetComponent<AccountInfoComponent>().Token,
                    Name = name,
                    ServerId = (int)zoneScene.GetComponent<ServerInfosComponent>().CurSelectedServerId
                });
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
                return ErrorCode.ERR_NetWorkError;
            }

            if (a2CCreateRole.Error != ErrorCode.ERR_Success)
            {
                Log.Error(a2CCreateRole.Error.ToString());
                return a2CCreateRole.Error;
            }

            RoleInfo newRoleInfo = zoneScene.GetComponent<RoleInfosComponent>().AddChild<RoleInfo>();
            newRoleInfo.FromMessage(a2CCreateRole.RoleInfo);
            zoneScene.GetComponent<RoleInfosComponent>().RoleInfos.Add(newRoleInfo);
            
            return ErrorCode.ERR_Success;
        }

        public static async ETTask<int> GetRoles(Scene zoneScene)
        {
            A2C_GetRoles a2CGetRoles;
            try
            {
                a2CGetRoles = (A2C_GetRoles) await zoneScene.GetComponent<SessionComponent>().Session.Call(new C2A_GetRoles()
                {
                    AccountId = zoneScene.GetComponent<AccountInfoComponent>().AccountId,
                    Token = zoneScene.GetComponent<AccountInfoComponent>().Token,
                    ServerId = zoneScene.GetComponent<ServerInfosComponent>().CurSelectedServerId
                });
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
                return ErrorCode.ERR_NetWorkError;
            }

            if (a2CGetRoles.Error != ErrorCode.ERR_Success)
            {
                Log.Error(a2CGetRoles.Error.ToString());
                return a2CGetRoles.Error;
            }

            var roleInfosComp = zoneScene.GetComponent<RoleInfosComponent>();
            roleInfosComp.RoleInfos.Clear();

            foreach (var roleInfoProto in a2CGetRoles.RoleInfo)
            {
                RoleInfo roleInfo = roleInfosComp.AddChild<RoleInfo>();
                roleInfo.FromMessage(roleInfoProto);
                roleInfosComp.RoleInfos.Add(roleInfo);
            }
            
            return ErrorCode.ERR_Success;
        }

        public static async ETTask<int> DeleteRole(Scene zoneScene)
        {
            A2C_DeleteRole a2CDeleteRole = null;
            try
            {
                a2CDeleteRole = (A2C_DeleteRole) await zoneScene.GetComponent<SessionComponent>().Session.Call(new C2A_DeleteRole()
                {
                    AccountId = zoneScene.GetComponent<AccountInfoComponent>().AccountId,
                    Token = zoneScene.GetComponent<AccountInfoComponent>().Token,
                    RoleInfoId = zoneScene.GetComponent<RoleInfosComponent>().CurRoleId,
                    ServerId = zoneScene.GetComponent<ServerInfosComponent>().CurSelectedServerId
                });
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
                return ErrorCode.ERR_NetWorkError;
            }

            if (a2CDeleteRole.Error != ErrorCode.ERR_Success)
            {
                Log.Error(a2CDeleteRole.Error.ToString());
                return a2CDeleteRole.Error;
            }
            
            var roleInfosComp = zoneScene.GetComponent<RoleInfosComponent>();

            int index = roleInfosComp.RoleInfos.FindIndex((info) =>
            {
                return info.Id == a2CDeleteRole.DeletedRoleInfoId;
            });
            
            roleInfosComp.RoleInfos.RemoveAt(index);

            return ErrorCode.ERR_Success;
        }

        public static async ETTask<int> GetRealmKey(Scene zoneScene)
        {
            A2C_GetRealmKey a2CGetRealmKey = null;
            
            try
            {
                a2CGetRealmKey = (A2C_GetRealmKey) await zoneScene.GetComponent<SessionComponent>().Session.Call(new C2A_GetRealmKey()
                {
                    AccountId = zoneScene.GetComponent<AccountInfoComponent>().AccountId,
                    Token = zoneScene.GetComponent<AccountInfoComponent>().Token,
                    ServerId = zoneScene.GetComponent<ServerInfosComponent>().CurSelectedServerId
                });
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
                return ErrorCode.ERR_NetWorkError;
            }
            
            if (a2CGetRealmKey.Error != ErrorCode.ERR_Success)
            {
                Log.Error(a2CGetRealmKey.Error.ToString());
                return a2CGetRealmKey.Error;
            }

            var accountInfoComponent = zoneScene.GetComponent<AccountInfoComponent>();
            accountInfoComponent.RealmKey = a2CGetRealmKey.RealmKey;
            accountInfoComponent.RealmAddress = a2CGetRealmKey.RealmAddress;
            // 断开和Login服务器的连接
            zoneScene.GetComponent<SessionComponent>().Session.Dispose();
            
            return ErrorCode.ERR_Success;
        }

        public static async ETTask<int> EnterGame(Scene zoneScene)
        {
            var accountInfoComp = zoneScene.GetComponent<AccountInfoComponent>();
            string realmAddress = accountInfoComp.RealmAddress;
            Log.Debug($"RealAdress: {realmAddress}");
            
            // 1.连接Realm，获取分配的Gate
            R2C_LoginRealm r2CLoginRealm;

            Session session = zoneScene.GetComponent<NetKcpComponent>().Create(NetworkHelper.ToIPEndPoint(realmAddress));
            try
            {
                r2CLoginRealm = (R2C_LoginRealm) await session.Call(new C2R_LoginRealm()
                {
                    AccountId = accountInfoComp.AccountId, RealmTokenKey = accountInfoComp.RealmKey
                });
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
                session?.Dispose();
                return ErrorCode.ERR_NetWorkError;
            }
            // 只是为了获取Gate信息，所以连一次就释放
            session?.Dispose();

            if (r2CLoginRealm.Error != ErrorCode.ERR_Success)
            {
                Log.Error(r2CLoginRealm.Error.ToString());
                return r2CLoginRealm.Error;
            }
            
            Log.Debug($"GateAdress: {r2CLoginRealm.GateAddress}");
            
            // 2.连接Gate服务器
            Session gateSession = zoneScene.GetComponent<NetKcpComponent>().Create(NetworkHelper.ToIPEndPoint(r2CLoginRealm.GateAddress));
            gateSession.AddComponent<PingComponent>();  //需要长连
            zoneScene.GetComponent<SessionComponent>().Session = gateSession;

            long curRoleId = zoneScene.GetComponent<RoleInfosComponent>().CurRoleId;
            G2C_LoginGameGate g2CLoginGameGate = null;

            try
            {
                g2CLoginGameGate = (G2C_LoginGameGate) await gateSession.Call(new C2G_LoginGameGate()
                {
                    AccountId = accountInfoComp.AccountId, GateTokenKey = r2CLoginRealm.GateSessionKey, RoleId = curRoleId
                });
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
                gateSession.Dispose();
                return ErrorCode.ERR_NetWorkError;
            }

            if (g2CLoginGameGate.Error != ErrorCode.ERR_Success)
            {
                Log.Error(g2CLoginGameGate.Error.ToString());
                gateSession.Dispose();
                return g2CLoginGameGate.Error;
            }
            
            Log.Debug($"登入Gate成功, PlayerId:{g2CLoginGameGate.PlayerId}");
            
            //3. 角色请求进入游戏逻辑服
            G2C_EnterGame g2CEnterGame = null;
            try
            {
                g2CEnterGame = (G2C_EnterGame) await gateSession.Call(new C2G_EnterGame());
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
                gateSession.Dispose();
                return ErrorCode.ERR_NetWorkError;
            }

            if (g2CEnterGame.Error != ErrorCode.ERR_Success)
            {
                Log.Error((g2CEnterGame.Error.ToString()));
                gateSession.Dispose();
                return g2CEnterGame.Error;
            }
            
            Log.Debug($"进入游戏逻辑服务器成功，UnitId:{g2CEnterGame.UnitId}");

            return ErrorCode.ERR_Success;
        }
    }
}