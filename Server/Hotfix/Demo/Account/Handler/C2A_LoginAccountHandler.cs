using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ET
{
    public class C2A_LoginAccountHandler : AMRpcHandler<C2A_LoginAccount, A2C_LoginAccount>
    {
        protected override async ETTask Run(Session session, C2A_LoginAccount request, A2C_LoginAccount response, Action reply)
        {
            if (session.DomainScene().SceneType != SceneType.Account)
            {
                Log.Error($"请求的Scene错误， 当前Scene为：{session.DomainScene().SceneType}");
                session.Dispose();
                return;
            }
            session.RemoveComponent<SessionAcceptTimeoutComponent>();

            // 如果有锁，则视为同一个人的重复请求
            if (session.GetComponent<SessionLockingComponent>() != null)
            {
                response.Error = ErrorCode.ERR_RequestRepeadtedly;
                reply();
                session.Disconnect().Coroutine();
            }

            if (string.IsNullOrEmpty(request.AccountName) || string.IsNullOrEmpty(request.Password))
            {
                response.Error = ErrorCode.ERR_LoginInfoError;
                reply();
                session.Disconnect().Coroutine();
                return;
            }

            if (!Regex.IsMatch(request.AccountName, @"^[a-zA-Z0-9]{6,15}$"))
            {
                response.Error = ErrorCode.ERR_LoginInfoError;
                reply();
                session.Disconnect().Coroutine();
                return;
            }

            // 上锁，防止同一个人的重复请求
            using (session.AddComponent<SessionLockingComponent>())
            {
                using(await CoroutineLockComponent.Instance.Wait(CoroutineLockType.LoginAccount, request.AccountName.Trim().GetHashCode()))
                {
                    var accountInfoList = await DBManagerComponent.Instance.GetZoneDB(session.DomainZone()).Query<Account>(d => d.AccountName.Equals(request.AccountName.Trim()));
                    Account account = null;

                    if (accountInfoList != null && accountInfoList.Count > 0)
                    {
                        account = accountInfoList[0];
                        session.AddChild(account);
                        if (account.AccountType == (int)AccountType.BlackList)
                        {
                            response.Error = ErrorCode.ERR_LoginInfoError;
                            reply();
                            session.Disconnect().Coroutine();
                            account.Dispose();
                            return;
                        }

                        if (!account.Password.Equals(request.Password))
                        {
                            response.Error = ErrorCode.ERR_LoginInfoError;
                            reply();
                            session.Disconnect().Coroutine();
                            account.Dispose();
                            return;
                        }
                    }
                    else
                    {
                        account = session.AddChild<Account>();
                        account.AccountName = request.AccountName;
                        account.Password = request.Password;
                        account.CreateTime = TimeHelper.ServerNow();
                        account.AccountType = (int)AccountType.General;
                        await DBManagerComponent.Instance.GetZoneDB(session.DomainZone()).Save<Account>(account);
                    }

                    StartSceneConfig startSceneConfig = StartSceneConfigCategory.Instance.GetBySceneName(session.DomainZone(), "LoginCenter");
                    long loginCenterInstanceId = startSceneConfig.InstanceId;
                    var loginAccountResponse = (L2A_LoginAccountResponse) await ActorMessageSenderComponent.Instance.Call(loginCenterInstanceId,
                        new A2L_LoginAccountRequest() { AccountId = account.Id });

                    if (loginAccountResponse.Error != ErrorCode.ERR_Success)
                    {
                        response.Error = loginAccountResponse.Error;

                        reply();
                        session.Disconnect().Coroutine();
                        account?.Dispose();
                        return;
                    }

                    long accountSessionInstanceId = session.DomainScene().GetComponent<AccountSessionsComponent>().Get(account.Id);
                    Session otherSession = Game.EventSystem.Get(accountSessionInstanceId) as Session;
                    otherSession?.Send(new A2C_Disconnect() { Error = 0 });
                    otherSession?.Disconnect().Coroutine();
                    session.DomainScene().GetComponent<AccountSessionsComponent>().Add(account.Id, session.InstanceId);
                    session.AddComponent<AccountCheckoutTimeComponent, long>(account.Id);


                    string Token = TimeHelper.ServerNow().ToString() + RandomHelper.RandomNumber(int.MinValue, int.MaxValue);
                    TokenComponent tokenComp = session.DomainScene().GetComponent<TokenComponent>();
                    tokenComp.Remove(account.Id);
                    tokenComp.Add(account.Id, Token);

                    response.AccountId = account.Id;
                    response.Token = Token;

                    reply();
                    account?.Dispose();
                }
               
            }
        }
    }
}
