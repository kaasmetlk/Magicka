using System;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.UI.Popup;
using Magicka.Graphics;
using Magicka.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PolygonHead;

namespace Magicka.GameLogic.GameStates.Persistent
{
	// Token: 0x02000576 RID: 1398
	public class PersistentGameState : GameState
	{
		// Token: 0x060029C1 RID: 10689 RVA: 0x001471FC File Offset: 0x001453FC
		public PersistentGameState() : base(new Camera(Vector3.Zero, Vector3.Forward, Vector3.Up, MagickCamera.DEFAULTFOV, MagickCamera.RATIO_16_9, 1f, 500f))
		{
			this.mPopupSystem = Singleton<PopupSystem>.Instance;
			for (DataChannel dataChannel = DataChannel.A; dataChannel < DataChannel.Count; dataChannel += 1)
			{
				this.mScene.AddRenderableGUIObject(dataChannel, this.mPopupSystem);
			}
		}

		// Token: 0x060029C2 RID: 10690 RVA: 0x00147261 File Offset: 0x00145461
		public override void OnEnter()
		{
		}

		// Token: 0x060029C3 RID: 10691 RVA: 0x00147263 File Offset: 0x00145463
		public override void OnExit()
		{
		}

		// Token: 0x060029C4 RID: 10692 RVA: 0x00147265 File Offset: 0x00145465
		public override void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			this.mPopupSystem.Update(iDataChannel, iDeltaTime);
		}

		// Token: 0x060029C5 RID: 10693 RVA: 0x00147274 File Offset: 0x00145474
		internal void ControllerEvent(Controller iSender, KeyboardState iState, KeyboardState iOldState)
		{
			if (this.mPopupSystem.IsDisplaying)
			{
				this.mPopupSystem.ControllerEvent(iSender, iState, iOldState);
			}
		}

		// Token: 0x060029C6 RID: 10694 RVA: 0x00147291 File Offset: 0x00145491
		internal void ControllerMovement(Controller iSender, ControllerDirection iDirection)
		{
			if (this.mPopupSystem.IsDisplaying)
			{
				Game.Instance.IsMouseVisible = false;
				this.mPopupSystem.ControllerMovement(iSender, iDirection);
			}
		}

		// Token: 0x060029C7 RID: 10695 RVA: 0x001472B8 File Offset: 0x001454B8
		internal void ControllerMouseAction(Controller iSender, Point iScreenSize, MouseState iState, MouseState iOldState)
		{
			if (this.mPopupSystem.IsDisplaying)
			{
				this.mPopupSystem.ControllerMouseAction(iSender, iScreenSize, iState, iOldState);
			}
		}

		// Token: 0x060029C8 RID: 10696 RVA: 0x001472D7 File Offset: 0x001454D7
		internal void ControllerMouseMove(Controller iSender, Point iScreenSize, MouseState iState, MouseState iOldState)
		{
			if (this.mPopupSystem.IsDisplaying)
			{
				this.mPopupSystem.ControllerMouseMove(iSender, iScreenSize, iState, iOldState);
			}
		}

		// Token: 0x060029C9 RID: 10697 RVA: 0x001472F6 File Offset: 0x001454F6
		internal void ControllerA(Controller iSender)
		{
			if (this.mPopupSystem.IsDisplaying)
			{
				this.mPopupSystem.ControllerA(iSender);
			}
		}

		// Token: 0x060029CA RID: 10698 RVA: 0x00147311 File Offset: 0x00145511
		internal void ControllerB(Controller iSender)
		{
			if (this.mPopupSystem.IsDisplaying)
			{
				this.mPopupSystem.ControllerB(iSender);
			}
		}

		// Token: 0x060029CB RID: 10699 RVA: 0x0014732C File Offset: 0x0014552C
		internal void ControllerX(Controller iSender)
		{
			if (this.mPopupSystem.IsDisplaying)
			{
				this.mPopupSystem.ControllerX(iSender);
			}
		}

		// Token: 0x060029CC RID: 10700 RVA: 0x00147347 File Offset: 0x00145547
		internal void ControllerY(Controller iSender)
		{
			if (this.mPopupSystem.IsDisplaying)
			{
				this.mPopupSystem.ControllerY(iSender);
			}
		}

		// Token: 0x060029CD RID: 10701 RVA: 0x00147364 File Offset: 0x00145564
		public bool IsolateControls()
		{
			bool result = false;
			if (this.mPopupSystem.IsDisplaying)
			{
				result = true;
			}
			return result;
		}

		// Token: 0x04002D2B RID: 11563
		private const float NEAR_PLANE = 1f;

		// Token: 0x04002D2C RID: 11564
		private const float FAR_PLANE = 500f;

		// Token: 0x04002D2D RID: 11565
		private PopupSystem mPopupSystem;
	}
}
