using System;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.GameStates.Menu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PolygonHead;

namespace Magicka.GameLogic.UI
{
	// Token: 0x02000642 RID: 1602
	internal class ErrorMessageBox : MessageBox
	{
		// Token: 0x17000B74 RID: 2932
		// (get) Token: 0x06003095 RID: 12437 RVA: 0x0018E4B8 File Offset: 0x0018C6B8
		public static ErrorMessageBox Instance
		{
			get
			{
				if (ErrorMessageBox.sSingelton == null)
				{
					lock (ErrorMessageBox.sSingeltonLock)
					{
						if (ErrorMessageBox.sSingelton == null)
						{
							ErrorMessageBox.sSingelton = new ErrorMessageBox();
						}
					}
				}
				return ErrorMessageBox.sSingelton;
			}
		}

		// Token: 0x06003096 RID: 12438 RVA: 0x0018E50C File Offset: 0x0018C70C
		private ErrorMessageBox() : base(SubMenu.LOC_ANY)
		{
			this.mMessage.DefaultColor = MenuItem.COLOR;
			this.mOkbutton = new MenuTextItem(SubMenu.LOC_OK, new Vector2(this.mCenter.X, this.mCenter.Y + 64f), this.mMessage.Font, TextAlign.Center);
		}

		// Token: 0x06003097 RID: 12439 RVA: 0x0018E571 File Offset: 0x0018C771
		public override void Show()
		{
			throw new NotSupportedException();
		}

		// Token: 0x06003098 RID: 12440 RVA: 0x0018E578 File Offset: 0x0018C778
		public void Show(string iMessage)
		{
			this.mAlpha = 0f;
			this.mDead = false;
			DialogManager.Instance.AddMessageBox(this);
			this.mMessage.SetText(iMessage);
			this.mTimer = 5f;
		}

		// Token: 0x06003099 RID: 12441 RVA: 0x0018E5B0 File Offset: 0x0018C7B0
		public override void Draw(float iDeltaTime)
		{
			base.Draw(iDeltaTime);
			if (this.mTimer <= 0f)
			{
				this.mDead = true;
			}
			this.mTimer -= iDeltaTime;
			MessageBox.sGUIBasicEffect.Color = MenuItem.COLOR;
			this.mMessage.Draw(MessageBox.sGUIBasicEffect, this.mCenter.X, this.mCenter.Y);
			this.mOkbutton.Draw(MessageBox.sGUIBasicEffect);
			MessageBox.sGUIBasicEffect.CurrentTechnique.Passes[0].End();
			MessageBox.sGUIBasicEffect.End();
		}

		// Token: 0x17000B75 RID: 2933
		// (get) Token: 0x0600309A RID: 12442 RVA: 0x0018E64F File Offset: 0x0018C84F
		public override int ZIndex
		{
			get
			{
				return 2002;
			}
		}

		// Token: 0x0600309B RID: 12443 RVA: 0x0018E656 File Offset: 0x0018C856
		public override void OnTextInput(char iChar)
		{
		}

		// Token: 0x0600309C RID: 12444 RVA: 0x0018E658 File Offset: 0x0018C858
		public override void OnMove(Controller iSender, ControllerDirection iDirection)
		{
		}

		// Token: 0x0600309D RID: 12445 RVA: 0x0018E65A File Offset: 0x0018C85A
		public override void OnMouseMove(MouseState iNewState, MouseState iOldState)
		{
			this.mOkbutton.Selected = this.mOkbutton.InsideBounds(iNewState);
		}

		// Token: 0x0600309E RID: 12446 RVA: 0x0018E673 File Offset: 0x0018C873
		public override void OnMouseClick(MouseState iNewState, MouseState iOldState)
		{
			if (iNewState.LeftButton == ButtonState.Released && this.mOkbutton.InsideBounds(iNewState))
			{
				this.OnSelect(null);
			}
		}

		// Token: 0x0600309F RID: 12447 RVA: 0x0018E693 File Offset: 0x0018C893
		public override void OnSelect(Controller iSender)
		{
			this.mTimer = 0f;
		}

		// Token: 0x0400349B RID: 13467
		private static ErrorMessageBox sSingelton;

		// Token: 0x0400349C RID: 13468
		private static volatile object sSingeltonLock = new object();

		// Token: 0x0400349D RID: 13469
		private float mTimer;

		// Token: 0x0400349E RID: 13470
		private MenuTextItem mOkbutton;
	}
}
