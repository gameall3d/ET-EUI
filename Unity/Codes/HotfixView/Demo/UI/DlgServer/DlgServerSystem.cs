using System.Collections;
using System.Collections.Generic;
using System;
using System.Xml.Schema;
using UnityEngine;
using UnityEngine.UI;

namespace ET
{
	public static  class DlgServerSystem
	{

		public static void RegisterUIEvent(this DlgServer self)
		{
			self.View.E_EnterServerButton.AddListener(() =>
			{
				self.OnEnterServerClickHandler().Coroutine();
			});
			
			self.View.ELoopScrollList_ServerLoopVerticalScrollRect.AddItemRefreshListener((Transform transform, int index) =>
			{
				self.OnLoopListItemRefreshHandler(transform, index);
			});
		}

		public static async ETTask ShowWindow(this DlgServer self, Entity contextData = null)
		{
			int result = await LoginHelper.GetServerInfos(self.ZoneScene());
			if (result != ErrorCode.ERR_Success)
			{
				Log.Error("Can't get server info");
				return;
			}
			
			var serverInfosComp = self.ZoneScene().GetComponent<ServerInfosComponent>();
			List<ServerInfo> serverInfos = serverInfosComp.ServerInfoList;
			// 默认选择第一个
			if (serverInfos.Count > 0)
			{
				serverInfosComp.CurSelectedServerId = (int)serverInfos[0].Id;
			}
			
			self.AddUIScrollItems(ref self.ScrollItemServerDict, serverInfos.Count);
			self.View.ELoopScrollList_ServerLoopVerticalScrollRect.SetVisible(true, serverInfos.Count);
			

		}

		public static void HideWindow(this DlgServer self)
		{
			self.RemoveUIScrollItems(ref self.ScrollItemServerDict);
		}

		public static async ETTask OnEnterServerClickHandler(this DlgServer self)
		{
			if (self.ZoneScene().GetComponent<ServerInfosComponent>().CurSelectedServerId == 0)
			{
				Log.Error("请选择区服");
				return;
			}

			try
			{
				int errorCode = await LoginHelper.GetRoles(self.ZoneScene());
				if (errorCode != ErrorCode.ERR_Success)
				{
					Log.Error(errorCode.ToString());
					return;
				}
				
				self.ZoneScene().GetComponent<UIComponent>().HideWindow(WindowID.WindowID_Server);
				self.ZoneScene().GetComponent<UIComponent>().ShowWindow(WindowID.WindowID_Role);
			}
			catch (Exception e)
			{
				Log.Error(e.ToString());
			}
		}

		public static void OnLoopListItemRefreshHandler(this DlgServer self, Transform transform, int index)
		{
			Scroll_Item_Server itemServer = self.ScrollItemServerDict[index];
			itemServer.BindTrans(transform);

			var serverInfosComp = self.ZoneScene().GetComponent<ServerInfosComponent>();
			List<ServerInfo> serverInfos = serverInfosComp.ServerInfoList;
			var serverInfo = serverInfos[index];
			itemServer.ELabel_ContentText.text = serverInfo.ServerName;
			itemServer.EButton_SelectButton.onClick.AddListener(() =>
			{
				self.OnServerItemClickHandler(serverInfo.Id);
			});

			itemServer.EButton_SelectImage.color = serverInfo.Id == serverInfosComp.CurSelectedServerId? Color.green : Color.white;
		}

		public static void OnServerItemClickHandler(this DlgServer self, long serverId)
		{
			var serverInfosComp = self.ZoneScene().GetComponent<ServerInfosComponent>();
			serverInfosComp.CurSelectedServerId = (int)serverId;
			self.View.ELoopScrollList_ServerLoopVerticalScrollRect.RefillCells();
		}


		
	}
}
