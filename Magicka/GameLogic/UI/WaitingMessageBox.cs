using System;
using Magicka.GameLogic.Controls;
using Magicka.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PolygonHead;

namespace Magicka.GameLogic.UI
{
	// Token: 0x0200039D RID: 925
	internal class WaitingMessageBox : MessageBox
	{
		// Token: 0x170006FA RID: 1786
		// (get) Token: 0x06001C4A RID: 7242 RVA: 0x000C0B4C File Offset: 0x000BED4C
		public static WaitingMessageBox Instance
		{
			get
			{
				if (WaitingMessageBox.sSingelton == null)
				{
					lock (WaitingMessageBox.sSingeltonLock)
					{
						if (WaitingMessageBox.sSingelton == null)
						{
							WaitingMessageBox.sSingelton = new WaitingMessageBox();
						}
					}
				}
				return WaitingMessageBox.sSingelton;
			}
		}

		// Token: 0x06001C4B RID: 7243 RVA: 0x000C0BA0 File Offset: 0x000BEDA0
		static WaitingMessageBox()
		{
			lock (Game.Instance.GraphicsDevice)
			{
				WaitingMessageBox.sVertexBuffer = new VertexBuffer(Game.Instance.GraphicsDevice, Defines.QUAD_COL_VERTS_C.Length * VertexPositionColor.SizeInBytes, BufferUsage.WriteOnly);
				WaitingMessageBox.sVertexBuffer.SetData<VertexPositionColor>(Defines.QUAD_COL_VERTS_C);
				WaitingMessageBox.sVertexDeclaration = new VertexDeclaration(Game.Instance.GraphicsDevice, VertexPositionColor.VertexElements);
			}
		}

		// Token: 0x06001C4C RID: 7244 RVA: 0x000C0C3C File Offset: 0x000BEE3C
		private WaitingMessageBox() : base("")
		{
			this.mDots = new Text(10, this.mMessage.Font, TextAlign.Left, false);
			this.mMessage.DefaultColor = this.mDots.DefaultColor;
		}

		// Token: 0x06001C4D RID: 7245 RVA: 0x000C0C90 File Offset: 0x000BEE90
		private WaitingMessageBox(int iMessage) : base(iMessage)
		{
		}

		// Token: 0x06001C4E RID: 7246 RVA: 0x000C0CA5 File Offset: 0x000BEEA5
		private WaitingMessageBox(string iMessage) : base(iMessage)
		{
		}

		// Token: 0x06001C4F RID: 7247 RVA: 0x000C0CBC File Offset: 0x000BEEBC
		public override void Draw(float iDeltaTime)
		{
			if (this.mDead)
			{
				this.mAlpha = Math.Max(this.mAlpha - iDeltaTime * 4f, 0f);
			}
			else
			{
				this.mAlpha = Math.Min(this.mAlpha + iDeltaTime * 4f, 1f);
			}
			Point screenSize = RenderManager.Instance.ScreenSize;
			MessageBox.sGUIBasicEffect.SetScreenSize(screenSize.X, screenSize.Y);
			MessageBox.sGUIBasicEffect.Begin();
			MessageBox.sGUIBasicEffect.CurrentTechnique.Passes[0].Begin();
			MessageBox.sGUIBasicEffect.GraphicsDevice.Vertices[0].SetSource(WaitingMessageBox.sVertexBuffer, 0, VertexPositionColor.SizeInBytes);
			MessageBox.sGUIBasicEffect.GraphicsDevice.VertexDeclaration = WaitingMessageBox.sVertexDeclaration;
			MessageBox.sGUIBasicEffect.VertexColorEnabled = true;
			MessageBox.sGUIBasicEffect.TextureEnabled = false;
			Matrix identity = Matrix.Identity;
			identity.M11 = (float)screenSize.X;
			identity.M22 = (float)screenSize.Y;
			identity.M41 = (float)screenSize.X * 0.5f;
			identity.M42 = (float)screenSize.Y * 0.5f;
			MessageBox.sGUIBasicEffect.Transform = identity;
			MessageBox.sGUIBasicEffect.Color = new Vector4(1f, 1f, 1f, this.mAlpha * 0.5f);
			MessageBox.sGUIBasicEffect.CommitChanges();
			MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
			MessageBox.sGUIBasicEffect.Color = new Vector4(1f, 1f, 1f, this.mAlpha);
			this.mMessage.Draw(MessageBox.sGUIBasicEffect, this.mCenter.X, this.mCenter.Y);
			this.mTimer += iDeltaTime * 2.5f;
			if (this.mTimer > 1f)
			{
				this.mTimer = 0f;
				this.mDotCount++;
				if (this.mDotCount > 4)
				{
					this.mDots.Clear();
					this.mDotCount = 0;
				}
				else
				{
					this.mDots.Append(".");
				}
			}
			this.mDots.Draw(MessageBox.sGUIBasicEffect, this.mDotPosition.X, this.mDotPosition.Y);
			MessageBox.sGUIBasicEffect.CurrentTechnique.Passes[0].End();
			MessageBox.sGUIBasicEffect.End();
		}

		// Token: 0x06001C50 RID: 7248 RVA: 0x000C0F40 File Offset: 0x000BF140
		public void Show(int iMessage)
		{
			this.mMessageText = iMessage;
			this.mMessage.SetText(LanguageManager.Instance.GetString(iMessage));
			this.mDotPosition = this.mCenter;
			this.mDotPosition.X = this.mDotPosition.X + this.mMessage.Font.MeasureText(this.mMessage.Characters, true).X * 0.5f;
			if (this.mDead)
			{
				base.Show();
			}
		}

		// Token: 0x06001C51 RID: 7249 RVA: 0x000C0FBD File Offset: 0x000BF1BD
		public override void Show()
		{
			this.Show(WaitingMessageBox.LOC_WAITING_FOR_OTHER_PLAYERS);
		}

		// Token: 0x06001C52 RID: 7250 RVA: 0x000C0FCA File Offset: 0x000BF1CA
		public override void Kill()
		{
			this.OnAbort = null;
			base.Kill();
		}

		// Token: 0x06001C53 RID: 7251 RVA: 0x000C0FD9 File Offset: 0x000BF1D9
		public override void OnTextInput(char iChar)
		{
		}

		// Token: 0x06001C54 RID: 7252 RVA: 0x000C0FDB File Offset: 0x000BF1DB
		public override void OnMove(Controller iSender, ControllerDirection iDirection)
		{
		}

		// Token: 0x06001C55 RID: 7253 RVA: 0x000C0FDD File Offset: 0x000BF1DD
		public override void OnMouseMove(MouseState iNewState, MouseState iOldState)
		{
		}

		// Token: 0x06001C56 RID: 7254 RVA: 0x000C0FDF File Offset: 0x000BF1DF
		public override void OnMouseClick(MouseState iNewState, MouseState iOldState)
		{
		}

		// Token: 0x06001C57 RID: 7255 RVA: 0x000C0FE1 File Offset: 0x000BF1E1
		public override void OnSelect(Controller iSender)
		{
		}

		// Token: 0x06001C58 RID: 7256 RVA: 0x000C0FE3 File Offset: 0x000BF1E3
		public override void ControllerEsc(Controller iSender)
		{
			if (this.OnAbort != null)
			{
				this.OnAbort.Invoke();
			}
			this.Kill();
		}

		// Token: 0x06001C59 RID: 7257 RVA: 0x000C1000 File Offset: 0x000BF200
		public override void LanguageChanged()
		{
			if (this.mMessageText != 0)
			{
				this.mMessage.SetText(LanguageManager.Instance.GetString(this.mMessageText));
				this.mDotPosition = this.mCenter;
				this.mDotPosition.X = this.mDotPosition.X + this.mMessage.Font.MeasureText(this.mMessage.Characters, true).X * 0.5f;
			}
		}

		// Token: 0x04001E7F RID: 7807
		private const int NR_OF_SQR = 7;

		// Token: 0x04001E80 RID: 7808
		private const float SQR_SIZE = 32f;

		// Token: 0x04001E81 RID: 7809
		private const float SQR_PADD = 6f;

		// Token: 0x04001E82 RID: 7810
		public static readonly int LOC_WAITING_FOR_OTHER_PLAYERS = "#network_24".GetHashCodeCustom();

		// Token: 0x04001E83 RID: 7811
		private static WaitingMessageBox sSingelton;

		// Token: 0x04001E84 RID: 7812
		private static volatile object sSingeltonLock = new object();

		// Token: 0x04001E85 RID: 7813
		private new static VertexBuffer sVertexBuffer;

		// Token: 0x04001E86 RID: 7814
		private new static VertexDeclaration sVertexDeclaration;

		// Token: 0x04001E87 RID: 7815
		private float[] mAlphas = new float[7];

		// Token: 0x04001E88 RID: 7816
		private int mMessageText;

		// Token: 0x04001E89 RID: 7817
		private Text mDots;

		// Token: 0x04001E8A RID: 7818
		private Vector2 mDotPosition;

		// Token: 0x04001E8B RID: 7819
		private float mTimer;

		// Token: 0x04001E8C RID: 7820
		private int mDotCount;

		// Token: 0x04001E8D RID: 7821
		public Action OnAbort;
	}
}
