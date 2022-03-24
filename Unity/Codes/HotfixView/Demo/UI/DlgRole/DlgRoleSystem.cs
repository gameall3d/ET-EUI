using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ET
{
	public static  class DlgRoleSystem
	{

		public static void RegisterUIEvent(this DlgRole self)
		{
			self.View.E_CreateRoleButton.AddListenerAsync(() =>
			{
				return self.OnCreateRoleClickHandler();
			});
			self.View.E_DeleteRoleButton.AddListenerAsync(() =>
			{
				return self.OnDeleteRoleClickHandler();
			});
			self.View.E_RoleListLoopHorizontalScrollRect.AddItemRefreshListener(((transform, i) =>
			{
				self.OnRoleListRefreshHandler(transform, i);
			} ));
			
			self.View.E_EnterGameButton.AddListenerAsync(() =>
			{
				return self.OnEnterGameHandler();
			});
		}

		public static void ShowWindow(this DlgRole self, Entity contextData = null)
		{
			self.RefreshRoleItems();
		}
		
		public static void RefreshRoleItems(this DlgRole self)
		{
			var roleInfosComp = self.ZoneScene().GetComponent<RoleInfosComponent>();
			if (roleInfosComp.RoleInfos.Count > 0)
			{
				roleInfosComp.CurRoleId = roleInfosComp.RoleInfos[0].Id;
			}
			int count = roleInfosComp.RoleInfos.Count;
			self.AddUIScrollItems(ref self.ScrollItemRoleDict, count);
			self.View.E_RoleListLoopHorizontalScrollRect.SetVisible(true, count);
		}

		public static void OnRoleListRefreshHandler(this DlgRole self, Transform transform, int index)
		{
			Scroll_Item_Role item = self.ScrollItemRoleDict[index];
			item.BindTrans(transform);

			var roleInfosComp = self.ZoneScene().GetComponent<RoleInfosComponent>();
			var roleInfo = roleInfosComp.RoleInfos[index];
			item.ELabel_ContentText.text = roleInfo.Name;
			item.EButton_SelectButton.AddListener(() =>
			{
				self.OnRoleItemClickHandler(roleInfo.Id);
			});

			item.EButton_SelectImage.color = roleInfo.Id == roleInfosComp.CurRoleId? Color.green : Color.white;
		}

		public static async ETTask OnCreateRoleClickHandler(this DlgRole self)
		{
			string name = self.View.E_RoleNameInputField.text;

			if (string.IsNullOrEmpty(name))
			{
				Log.Error("Name is null");
				return;
			}

			try
			{
				int errorCode = await LoginHelper.CreateRole(self.ZoneScene(), name);
				if (errorCode != ErrorCode.ERR_Success)
				{
					Log.Error(errorCode.ToString());
					return;
				}
				
				self.RefreshRoleItems();
			}
			catch (Exception e)
			{
				Log.Error(e.ToString());
				return;
			}
		}

		public static void OnRoleItemClickHandler(this DlgRole self, long roleId)
		{
			var roleInfosComp = self.ZoneScene().GetComponent<RoleInfosComponent>();
			roleInfosComp.CurRoleId = roleId;
			self.View.E_RoleListLoopHorizontalScrollRect.RefillCells();
		}

		public static async ETTask OnDeleteRoleClickHandler(this DlgRole self)
		{
			var roleInfosComp = self.ZoneScene().GetComponent<RoleInfosComponent>();
			if (roleInfosComp.CurRoleId == 0)
			{
				Log.Error("请选择需要删除的角色");
				return;
			}

			try
			{
				int errorCode = await LoginHelper.DeleteRole(self.ZoneScene());
				if (errorCode != ErrorCode.ERR_Success)
				{
					Log.Error(errorCode.ToString());
					return;
				}
				
				self.RefreshRoleItems();
			}
			catch (Exception e)
			{
				Log.Error(e.ToString());
				return;
			}

			await ETTask.CompletedTask;
		}

		public static async ETTask OnEnterGameHandler(this DlgRole self)
		{
			if (self.ZoneScene().GetComponent<RoleInfosComponent>().CurRoleId == 0)
			{
				Log.Error("请选择要进行游戏的角色");
				return;
			}
			
			try
			{
				int errorCode = await LoginHelper.GetRealmKey(self.ZoneScene());
				if (errorCode != ErrorCode.ERR_Success)
				{
					Log.Error(errorCode.ToString());
					return;
				}


				errorCode = await LoginHelper.EnterGame(self.ZoneScene());
				if (errorCode != ErrorCode.ERR_Success)
				{
					Log.Error(errorCode.ToString());
					return;
				}
				
			 	self.ZoneScene().GetComponent<UIComponent>().HideWindow(WindowID.WindowID_Role);
				//self.ZoneScene().GetComponent<UIComponent>().ShowWindow(WindowID.WindowID_Role);
			}
			catch (Exception e)
			{
				Log.Error(e.ToString());
				return;
			}
		}

	}
}
