
using UnityEngine;
using UnityEngine.UI;
namespace ET
{
	public  class DlgRoleViewComponent : Entity,IAwake,IDestroy 
	{
		public UnityEngine.RectTransform EGBackGroundRectTransform
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_EGBackGroundRectTransform == null )
     			{
		    		this.m_EGBackGroundRectTransform = UIFindHelper.FindDeepChild<UnityEngine.RectTransform>(this.uiTransform.gameObject,"EGBackGround");
     			}
     			return this.m_EGBackGroundRectTransform;
     		}
     	}

		public UnityEngine.UI.Button E_DeleteRoleButton
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_E_DeleteRoleButton == null )
     			{
		    		this.m_E_DeleteRoleButton = UIFindHelper.FindDeepChild<UnityEngine.UI.Button>(this.uiTransform.gameObject,"EGBackGround/E_DeleteRole");
     			}
     			return this.m_E_DeleteRoleButton;
     		}
     	}

		public UnityEngine.UI.Image E_DeleteRoleImage
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_E_DeleteRoleImage == null )
     			{
		    		this.m_E_DeleteRoleImage = UIFindHelper.FindDeepChild<UnityEngine.UI.Image>(this.uiTransform.gameObject,"EGBackGround/E_DeleteRole");
     			}
     			return this.m_E_DeleteRoleImage;
     		}
     	}

		public UnityEngine.UI.Button E_EnterGameButton
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_E_EnterGameButton == null )
     			{
		    		this.m_E_EnterGameButton = UIFindHelper.FindDeepChild<UnityEngine.UI.Button>(this.uiTransform.gameObject,"EGBackGround/E_EnterGame");
     			}
     			return this.m_E_EnterGameButton;
     		}
     	}

		public UnityEngine.UI.Image E_EnterGameImage
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_E_EnterGameImage == null )
     			{
		    		this.m_E_EnterGameImage = UIFindHelper.FindDeepChild<UnityEngine.UI.Image>(this.uiTransform.gameObject,"EGBackGround/E_EnterGame");
     			}
     			return this.m_E_EnterGameImage;
     		}
     	}

		public UnityEngine.UI.LoopHorizontalScrollRect E_RoleListLoopHorizontalScrollRect
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_E_RoleListLoopHorizontalScrollRect == null )
     			{
		    		this.m_E_RoleListLoopHorizontalScrollRect = UIFindHelper.FindDeepChild<UnityEngine.UI.LoopHorizontalScrollRect>(this.uiTransform.gameObject,"EGBackGround/E_RoleList");
     			}
     			return this.m_E_RoleListLoopHorizontalScrollRect;
     		}
     	}

		public UnityEngine.UI.InputField E_RoleNameInputField
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_E_RoleNameInputField == null )
     			{
		    		this.m_E_RoleNameInputField = UIFindHelper.FindDeepChild<UnityEngine.UI.InputField>(this.uiTransform.gameObject,"EGBackGround/E_RoleName");
     			}
     			return this.m_E_RoleNameInputField;
     		}
     	}

		public UnityEngine.UI.Image E_RoleNameImage
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_E_RoleNameImage == null )
     			{
		    		this.m_E_RoleNameImage = UIFindHelper.FindDeepChild<UnityEngine.UI.Image>(this.uiTransform.gameObject,"EGBackGround/E_RoleName");
     			}
     			return this.m_E_RoleNameImage;
     		}
     	}

		public UnityEngine.UI.Button E_CreateRoleButton
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_E_CreateRoleButton == null )
     			{
		    		this.m_E_CreateRoleButton = UIFindHelper.FindDeepChild<UnityEngine.UI.Button>(this.uiTransform.gameObject,"EGBackGround/E_CreateRole");
     			}
     			return this.m_E_CreateRoleButton;
     		}
     	}

		public UnityEngine.UI.Image E_CreateRoleImage
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_E_CreateRoleImage == null )
     			{
		    		this.m_E_CreateRoleImage = UIFindHelper.FindDeepChild<UnityEngine.UI.Image>(this.uiTransform.gameObject,"EGBackGround/E_CreateRole");
     			}
     			return this.m_E_CreateRoleImage;
     		}
     	}

		public void DestroyWidget()
		{
			this.m_EGBackGroundRectTransform = null;
			this.m_E_DeleteRoleButton = null;
			this.m_E_DeleteRoleImage = null;
			this.m_E_EnterGameButton = null;
			this.m_E_EnterGameImage = null;
			this.m_E_RoleListLoopHorizontalScrollRect = null;
			this.m_E_RoleNameInputField = null;
			this.m_E_RoleNameImage = null;
			this.m_E_CreateRoleButton = null;
			this.m_E_CreateRoleImage = null;
			this.uiTransform = null;
		}

		private UnityEngine.RectTransform m_EGBackGroundRectTransform = null;
		private UnityEngine.UI.Button m_E_DeleteRoleButton = null;
		private UnityEngine.UI.Image m_E_DeleteRoleImage = null;
		private UnityEngine.UI.Button m_E_EnterGameButton = null;
		private UnityEngine.UI.Image m_E_EnterGameImage = null;
		private UnityEngine.UI.LoopHorizontalScrollRect m_E_RoleListLoopHorizontalScrollRect = null;
		private UnityEngine.UI.InputField m_E_RoleNameInputField = null;
		private UnityEngine.UI.Image m_E_RoleNameImage = null;
		private UnityEngine.UI.Button m_E_CreateRoleButton = null;
		private UnityEngine.UI.Image m_E_CreateRoleImage = null;
		public Transform uiTransform = null;
	}
}
