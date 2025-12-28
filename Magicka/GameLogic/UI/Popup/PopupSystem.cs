using System;
using System.Collections.Generic;
using Magicka.GameLogic.Controls;
using Magicka.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PolygonHead;
using PolygonHead.Effects;

namespace Magicka.GameLogic.UI.Popup
{
	// Token: 0x0200002D RID: 45
	internal sealed class PopupSystem : Singleton<PopupSystem>, IRenderableGUIObject
	{
		// Token: 0x17000059 RID: 89
		// (get) Token: 0x06000166 RID: 358 RVA: 0x0000A7F8 File Offset: 0x000089F8
		public bool IsDisplaying
		{
			get
			{
				return this.mCurrentPopup != null;
			}
		}

		// Token: 0x1700005A RID: 90
		// (get) Token: 0x06000167 RID: 359 RVA: 0x0000A806 File Offset: 0x00008A06
		public MenuBasePopup CurrentPopup
		{
			get
			{
				return this.mCurrentPopup;
			}
		}

		// Token: 0x1700005B RID: 91
		// (get) Token: 0x06000168 RID: 360 RVA: 0x0000A80E File Offset: 0x00008A0E
		public int ZIndex
		{
			get
			{
				return 300;
			}
		}

		// Token: 0x0600016A RID: 362 RVA: 0x0000A86C File Offset: 0x00008A6C
		public PopupSystem()
		{
			this.mPopupQueue = new Queue<MenuBasePopup>();
			this.mEffect = new GUIBasicEffect(Game.Instance.GraphicsDevice, null);
			this.mEffect.TextureEnabled = true;
			this.mEffect.Color = Vector4.One;
			ResolutionMessageBox.Instance.Complete += this.ChangeResolution;
			Game.Instance.Form.SizeChanged += delegate(object iObj, EventArgs iArgs)
			{
				if (this.mCurrentPopup != null)
				{
					this.mCurrentPopup.OnShow();
				}
			};
		}

		// Token: 0x0600016B RID: 363 RVA: 0x0000A901 File Offset: 0x00008B01
		private void ChangeResolution(ResolutionData iData)
		{
			PopupSystem.Resolution = new Vector2((float)iData.Width, (float)iData.Height);
			if (this.mCurrentPopup != null)
			{
				this.mCurrentPopup.OnShow();
			}
		}

		// Token: 0x0600016C RID: 364 RVA: 0x0000A930 File Offset: 0x00008B30
		public void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			lock (this.mLock)
			{
				if (this.mCurrentPopup == null)
				{
					if (this.mPopupQueue.Count > 0)
					{
						this.mCurrentPopup = this.mPopupQueue.Dequeue();
						this.mCurrentPopup.OnShow();
					}
				}
				else if (this.mDismissPopup)
				{
					this.mCurrentPopup.OnHide();
					this.mCurrentPopup = null;
					this.mDismissPopup = false;
				}
				else
				{
					this.mCurrentPopup.Update(iDataChannel, iDeltaTime);
				}
			}
		}

		// Token: 0x0600016D RID: 365 RVA: 0x0000A9CC File Offset: 0x00008BCC
		public void Draw(float iDeltaTime)
		{
			lock (this.mLock)
			{
				if (this.mCurrentPopup != null)
				{
					this.mEffect.SetScreenSize(RenderManager.Instance.ScreenSize.X, RenderManager.Instance.ScreenSize.Y);
					this.mEffect.Color = Vector4.One;
					this.mEffect.Begin();
					this.mEffect.CurrentTechnique.Passes[0].Begin();
					this.mCurrentPopup.Draw(this.mEffect);
					this.mEffect.CurrentTechnique.Passes[0].End();
					this.mEffect.End();
				}
			}
		}

		// Token: 0x0600016E RID: 366 RVA: 0x0000AAA4 File Offset: 0x00008CA4
		public void AddPopupToQueue(MenuBasePopup iPopup)
		{
			if (iPopup != null)
			{
				this.mPopupQueue.Enqueue(iPopup);
			}
		}

		// Token: 0x0600016F RID: 367 RVA: 0x0000AAB5 File Offset: 0x00008CB5
		public void ShowPopupImmediately(MenuBasePopup iPopup)
		{
			if (iPopup != null)
			{
				if (this.mCurrentPopup != null)
				{
					this.mCurrentPopup.OnHide();
				}
				this.mCurrentPopup = iPopup;
				this.mCurrentPopup.OnShow();
			}
		}

		// Token: 0x06000170 RID: 368 RVA: 0x0000AADF File Offset: 0x00008CDF
		public void DismissCurrentPopup()
		{
			this.mDismissPopup = true;
		}

		// Token: 0x06000171 RID: 369 RVA: 0x0000AAE8 File Offset: 0x00008CE8
		public void ForceDismissCurrentPopupIfMatches(MenuBasePopup iPopup)
		{
			if (this.mCurrentPopup == iPopup && !this.mCurrentPopup.CanDismiss)
			{
				this.DismissCurrentPopup();
			}
		}

		// Token: 0x06000172 RID: 370 RVA: 0x0000AB06 File Offset: 0x00008D06
		public void ControllerEvent(Controller iSender, KeyboardState iState, KeyboardState iOldState)
		{
		}

		// Token: 0x06000173 RID: 371 RVA: 0x0000AB08 File Offset: 0x00008D08
		public void ControllerMovement(Controller iSender, ControllerDirection iDirection)
		{
			if (this.mCurrentPopup != null && this.mCurrentPopup.Enabled)
			{
				switch (iDirection)
				{
				case ControllerDirection.Right:
					this.mCurrentPopup.ControllerRight(iSender);
					return;
				case ControllerDirection.Up:
					this.mCurrentPopup.ControllerUp(iSender);
					return;
				case ControllerDirection.UpRight:
					break;
				case ControllerDirection.Left:
					this.mCurrentPopup.ControllerLeft(iSender);
					return;
				default:
					if (iDirection != ControllerDirection.Down)
					{
						return;
					}
					this.mCurrentPopup.ControllerDown(iSender);
					break;
				}
			}
		}

		// Token: 0x06000174 RID: 372 RVA: 0x0000AB7C File Offset: 0x00008D7C
		public void ControllerMouseAction(Controller iSender, Point iScreenSize, MouseState iState, MouseState iOldState)
		{
			if (this.mCurrentPopup != null && this.mCurrentPopup.Enabled && iState.LeftButton == ButtonState.Released && iOldState.LeftButton == ButtonState.Pressed)
			{
				this.mCurrentPopup.ControllerMouseAction(iSender, iScreenSize, iState, iOldState);
			}
		}

		// Token: 0x06000175 RID: 373 RVA: 0x0000ABB6 File Offset: 0x00008DB6
		public void ControllerMouseMove(Controller iSender, Point iScreenSize, MouseState iState, MouseState iOldState)
		{
			if (this.mCurrentPopup != null && this.mCurrentPopup.Enabled)
			{
				this.mCurrentPopup.ControllerMouseMove(iSender, iScreenSize, iState, iOldState);
			}
		}

		// Token: 0x06000176 RID: 374 RVA: 0x0000ABDD File Offset: 0x00008DDD
		public void ControllerA(Controller iSender)
		{
			if (this.mCurrentPopup != null && this.mCurrentPopup.Enabled)
			{
				this.mCurrentPopup.ControllerA(iSender);
			}
		}

		// Token: 0x06000177 RID: 375 RVA: 0x0000AC00 File Offset: 0x00008E00
		public void ControllerB(Controller iSender)
		{
			if (this.mCurrentPopup != null && this.mCurrentPopup.Enabled && this.mCurrentPopup.CanDismiss)
			{
				this.DismissCurrentPopup();
			}
		}

		// Token: 0x06000178 RID: 376 RVA: 0x0000AC2A File Offset: 0x00008E2A
		public void ControllerX(Controller iSender)
		{
			if (this.mCurrentPopup != null && this.mCurrentPopup.Enabled)
			{
				this.mCurrentPopup.ControllerX(iSender);
			}
		}

		// Token: 0x06000179 RID: 377 RVA: 0x0000AC4D File Offset: 0x00008E4D
		public void ControllerY(Controller iSender)
		{
			if (this.mCurrentPopup != null && this.mCurrentPopup.Enabled)
			{
				this.mCurrentPopup.ControllerY(iSender);
			}
		}

		// Token: 0x04000125 RID: 293
		public static readonly Vector2 REFERENCE_SIZE = new Vector2(1920f, 1080f);

		// Token: 0x04000126 RID: 294
		public static Vector2 Resolution = new Vector2((float)RenderManager.Instance.ScreenSize.X, (float)RenderManager.Instance.ScreenSize.Y);

		// Token: 0x04000127 RID: 295
		private Queue<MenuBasePopup> mPopupQueue;

		// Token: 0x04000128 RID: 296
		private MenuBasePopup mCurrentPopup;

		// Token: 0x04000129 RID: 297
		private volatile object mLock = new object();

		// Token: 0x0400012A RID: 298
		private bool mDismissPopup;

		// Token: 0x0400012B RID: 299
		private GUIBasicEffect mEffect;
	}
}
