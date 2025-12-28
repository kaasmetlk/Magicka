using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Magicka.GameLogic.Controls;
using Magicka.Graphics;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using SteamWrapper;

namespace Magicka.GameLogic.GameStates.Menu
{
	// Token: 0x020005DF RID: 1503
	internal class NetworkChat
	{
		// Token: 0x17000A9F RID: 2719
		// (get) Token: 0x06002CDC RID: 11484 RVA: 0x0016042C File Offset: 0x0015E62C
		public static NetworkChat Instance
		{
			get
			{
				if (NetworkChat.sSingelton == null)
				{
					lock (NetworkChat.sSingeltonLock)
					{
						if (NetworkChat.sSingelton == null)
						{
							NetworkChat.sSingelton = new NetworkChat();
						}
					}
				}
				return NetworkChat.sSingelton;
			}
		}

		// Token: 0x06002CDD RID: 11485 RVA: 0x00160480 File Offset: 0x0015E680
		private NetworkChat()
		{
			GraphicsDevice graphicsDevice = Game.Instance.GraphicsDevice;
			lock (graphicsDevice)
			{
				this.mEffect = new GUIBasicEffect(graphicsDevice, null);
			}
			this.mFont = FontManager.Instance.GetFont(MagickaFont.MenuDefault);
			this.mTitle = new Text(256, FontManager.Instance.GetFont(MagickaFont.MenuDefault), TextAlign.Center, false, true);
			this.mTitle.DefaultColor = Vector4.One;
			this.mTitle.ShadowAlpha = 0.75f;
			this.mTitle.ShadowsOffset = Vector2.One;
			this.mLog = new StringBuilder(2048, 2048);
			this.mLogText = new Text(2048, this.mFont, TextAlign.Left, true, true);
			this.mLogText.DefaultColor = Vector4.One;
			this.mLogText.ShadowAlpha = 0.75f;
			this.mLogText.ShadowsOffset = Vector2.One;
			this.mMessage = new StringBuilder(202, 202);
			this.mMessageText = new Text(202, this.mFont, TextAlign.Left, true, false);
			this.mMessageText.DefaultColor = Vector4.One;
			this.mMessageText.ShadowAlpha = 0.75f;
			this.mMessageText.ShadowsOffset = Vector2.One;
			this.mScrollBar = new MenuScrollBar(default(Vector2), 1f, 0);
			this.mScrollBar.TextureOffset = new Vector2(-384f, 224f);
			this.mTitleActive = false;
			VertexPositionColor[] array = new VertexPositionColor[Defines.QUAD_COL_VERTS_TL.Length];
			Defines.QUAD_COL_VERTS_TL.CopyTo(array, 0);
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Color.A = 127;
			}
			lock (graphicsDevice)
			{
				this.mBackgroundVertices = new VertexBuffer(graphicsDevice, VertexPositionColor.SizeInBytes * array.Length, BufferUsage.WriteOnly);
				this.mBackgroundVertices.SetData<VertexPositionColor>(array);
				this.mBackgroundDeclaration = RenderManager.Instance.CreateVertexDeclaration(VertexPositionColor.VertexElements);
			}
		}

		// Token: 0x06002CDE RID: 11486 RVA: 0x001606C0 File Offset: 0x0015E8C0
		public void Set(int iWidth, int iHeight, Texture2D iBackgroundTexture, Rectangle iBackgroundRect, BitmapFont iFont, bool iDropShadow, int iMargin, bool iScrollBarVisible, float iMessageTTL)
		{
			this.mMargin = iMargin;
			this.mSize.X = iWidth;
			this.mSize.Y = iHeight;
			this.mScrollBarVisible = iScrollBarVisible;
			this.mBackgroundTexture = iBackgroundTexture;
			if (this.mBackgroundTexture != null)
			{
				this.mBackgroundTextureOffset.X = (float)iBackgroundRect.X / (float)iBackgroundTexture.Width;
				this.mBackgroundTextureOffset.Y = (float)iBackgroundRect.Y / (float)iBackgroundTexture.Height;
				this.mBackgroundTextureScale.X = (float)iBackgroundRect.Width / (float)iBackgroundTexture.Width;
				this.mBackgroundTextureScale.Y = (float)iBackgroundRect.Height / (float)iBackgroundTexture.Height;
			}
			if (this.mTitleActive)
			{
				this.mScrollBar.Height = (float)this.mSize.Y - 64f;
				this.mVisibleLines = (this.mSize.Y - this.mFont.LineHeight * 2 - this.mMargin) / this.mFont.LineHeight;
			}
			else
			{
				this.mScrollBar.Height = (float)this.mSize.Y;
				this.mVisibleLines = (this.mSize.Y - this.mMargin) / this.mFont.LineHeight;
			}
			this.mMessageTTL = iMessageTTL;
			for (int i = 0; i < this.mMessageTTLs.Count; i++)
			{
				this.mMessageTTLs[i] = Math.Min(this.mMessageTTLs[i], iMessageTTL);
			}
			this.mTitle.DrawShadows = iDropShadow;
			this.mLogText.DrawShadows = iDropShadow;
			this.mMessageText.DrawShadows = iDropShadow;
			this.mTitle.Font = iFont;
			this.mLogText.Font = iFont;
			this.mMessageText.Font = iFont;
			this.mScrollBar.SetMaxValue(0);
			this.mMessage.Length = 0;
			this.mMessageText.SetText(null);
		}

		// Token: 0x17000AA0 RID: 2720
		// (get) Token: 0x06002CDF RID: 11487 RVA: 0x001608B3 File Offset: 0x0015EAB3
		public Point Size
		{
			get
			{
				return this.mSize;
			}
		}

		// Token: 0x17000AA1 RID: 2721
		// (get) Token: 0x06002CE0 RID: 11488 RVA: 0x001608BB File Offset: 0x0015EABB
		public MenuScrollBar ScrollBar
		{
			get
			{
				return this.mScrollBar;
			}
		}

		// Token: 0x06002CE1 RID: 11489 RVA: 0x001608C4 File Offset: 0x0015EAC4
		public void Draw(ref Vector2 iPos)
		{
			if (this.mScrollValue != this.mScrollBar.Value)
			{
				this.mScrollValue = this.mScrollBar.Value;
				this.mDirty = true;
			}
			if (this.mDirty)
			{
				this.UpdateText();
			}
			Viewport viewport = this.mEffect.GraphicsDevice.Viewport;
			this.mEffect.SetScreenSize(viewport.Width, viewport.Height);
			if (this.mBackgroundTexture != null)
			{
				this.mEffect.Color = new Vector4(1f);
				this.mEffect.VertexColorEnabled = true;
				this.mEffect.TextureEnabled = false;
				Matrix transform = default(Matrix);
				transform.M11 = (float)this.mSize.X;
				transform.M22 = (float)this.mSize.Y;
				transform.M41 = iPos.X;
				transform.M42 = iPos.Y;
				transform.M44 = 1f;
				this.mEffect.Transform = transform;
				this.mEffect.GraphicsDevice.Vertices[0].SetSource(this.mBackgroundVertices, 0, VertexPositionColor.SizeInBytes);
				this.mEffect.GraphicsDevice.VertexDeclaration = this.mBackgroundDeclaration;
				this.mEffect.Begin();
				this.mEffect.CurrentTechnique.Passes[0].Begin();
				this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
			}
			else
			{
				this.mEffect.Begin();
				this.mEffect.CurrentTechnique.Passes[0].Begin();
			}
			this.mEffect.TextureOffset = default(Vector2);
			this.mEffect.TextureScale = Vector2.One;
			this.mEffect.Color = Vector4.One;
			this.mEffect.VertexColorEnabled = true;
			if (this.mScrollBarVisible)
			{
				if (this.mTitleActive)
				{
					this.mTitle.Draw(this.mEffect, iPos.X + (float)this.mSize.X * 0.5f, iPos.Y + (float)this.mMargin);
					this.mLogText.Draw(this.mEffect, iPos.X + (float)this.mMargin, iPos.Y + (float)(this.mMargin * 2) + (float)this.mFont.LineHeight);
				}
				else
				{
					this.mLogText.Draw(this.mEffect, iPos.X + (float)this.mMargin, iPos.Y + (float)this.mMargin);
				}
			}
			else
			{
				this.mLogText.Draw(this.mEffect, iPos.X + (float)this.mMargin, iPos.Y + (float)this.mSize.Y - (float)((this.mLineCount + 1) * this.mMessageText.Font.LineHeight));
			}
			if (this.mScrollBarVisible)
			{
				this.mMessageText.Draw(this.mEffect, iPos.X + (float)this.mMargin, iPos.Y + (float)this.mSize.Y - (float)this.mMessageText.Font.LineHeight - (float)this.mMargin);
			}
			else if (this.mActive)
			{
				this.mMessageText.Draw(this.mEffect, iPos.X + (float)this.mMargin, iPos.Y + (float)this.mSize.Y);
			}
			if (this.mScrollBarVisible)
			{
				this.mEffect.VertexColorEnabled = false;
				this.mScrollBar.Scale = 0.75f;
				this.mScrollBar.Height = ((float)this.mSize.Y - 64f) / 0.75f;
				this.mScrollBar.Position = new Vector2(iPos.X + (float)this.mSize.X - 32f, iPos.Y + (float)this.mSize.Y * 0.5f);
				this.mScrollBar.Draw(this.mEffect);
			}
			this.mEffect.CurrentTechnique.Passes[0].End();
			this.mEffect.End();
		}

		// Token: 0x06002CE2 RID: 11490 RVA: 0x00160D04 File Offset: 0x0015EF04
		public void AddMessage(string iMessage)
		{
			iMessage = this.mFont.Wrap(iMessage, this.mSize.X - this.mMargin * 2, true);
			MatchCollection matchCollection = Regex.Matches(iMessage, "\n");
			int num = matchCollection.Count + 1;
			if (this.mLog.Length > 0)
			{
				iMessage = '\n' + iMessage;
			}
			while (this.mLog.Length + iMessage.Length >= this.mLog.Capacity)
			{
				this.RemoveLine();
			}
			this.mLog.Append(iMessage);
			for (int i = 0; i < num; i++)
			{
				this.mMessageTTLs.Add(this.mMessageTTL);
			}
			matchCollection = Regex.Matches(this.mLog.ToString(), "\n");
			this.mLineCount = matchCollection.Count;
			this.mScrollBar.SetMaxValue(this.mLineCount - this.mVisibleLines);
			this.mScrollBar.Value = this.mScrollBar.MaxValue;
			this.mScrollValue = -1;
			this.mDirty = true;
		}

		// Token: 0x06002CE3 RID: 11491 RVA: 0x00160E18 File Offset: 0x0015F018
		private void UpdateText()
		{
			this.mDirty = false;
			int num = 0;
			int length = this.mLog.Length;
			if (this.mScrollBarVisible)
			{
				MatchCollection matchCollection = Regex.Matches(this.mLog.ToString(), "\n");
				this.mLineCount = matchCollection.Count;
				if (matchCollection.Count >= this.mVisibleLines)
				{
					num = matchCollection[this.mScrollBar.Value].Index + 1;
					if (this.mScrollBar.Value < this.mScrollBar.MaxValue)
					{
						length = matchCollection[this.mScrollBar.Value + this.mVisibleLines].Index - num;
					}
					else
					{
						length = this.mLog.Length - num;
					}
				}
			}
			string text = this.mLog.ToString(num, length);
			this.mLogText.SetText(text);
		}

		// Token: 0x06002CE4 RID: 11492 RVA: 0x00160EF0 File Offset: 0x0015F0F0
		private void RemoveLine()
		{
			int num = this.mLog.ToString().IndexOf('\n');
			if (num < 0)
			{
				this.mLog.Length = 0;
			}
			else
			{
				this.mLog.Remove(0, num + 1);
				this.mLineCount--;
			}
			this.mMessageTTLs.RemoveAt(0);
			this.mDirty = true;
		}

		// Token: 0x06002CE5 RID: 11493 RVA: 0x00160F54 File Offset: 0x0015F154
		public void TakeInput(Controller iSender, char iInput)
		{
			if (this.mActive)
			{
				if (iInput == '\b')
				{
					if (this.mMessage.Length > 0)
					{
						this.mMessage.Length--;
					}
				}
				else if ((this.mMessage.Length > 0 | iInput != ' ') && this.mMessage.Length < 50)
				{
					this.mMessage.Append(iInput);
				}
				this.mMessageText.SetText(this.mFont.Wrap(this.mMessage.ToString(), this.mSize.X - this.mMargin * 2, true));
			}
		}

		// Token: 0x06002CE6 RID: 11494 RVA: 0x00161000 File Offset: 0x0015F200
		public void Update(float iDeltaTime)
		{
			for (int i = 0; i < this.mMessageTTLs.Count; i++)
			{
				float num = this.mMessageTTLs[i];
				num -= iDeltaTime;
				if (num <= 0f)
				{
					this.RemoveLine();
					i--;
				}
				else
				{
					this.mMessageTTLs[i] = num;
				}
			}
			this.mTimer -= iDeltaTime;
			while (this.mTimer < 0f)
			{
				this.mTimer += 0.5f;
				this.mInputLine = !this.mInputLine;
				if (this.mInputLine)
				{
					this.mMessageText.Characters[this.mMessage.Length] = '_';
					this.mMessageText.Characters[this.mMessage.Length + 1] = '\0';
				}
				else
				{
					this.mMessageText.Characters[this.mMessage.Length] = '\0';
				}
				this.mMessageText.MarkAsDirty();
			}
		}

		// Token: 0x06002CE7 RID: 11495 RVA: 0x001610FC File Offset: 0x0015F2FC
		internal void SendMessage()
		{
			if (this.mMessage.Length > 0)
			{
				ChatMessage chatMessage = default(ChatMessage);
				chatMessage.Sender = SteamFriends.GetPersonaName();
				chatMessage.Message = this.mMessage.ToString();
				NetworkManager.Instance.Interface.SendMessage<ChatMessage>(ref chatMessage);
				NetworkChat.Instance.AddMessage(chatMessage.ToString());
				this.mMessage.Length = 0;
				this.mMessageText.SetText(null);
			}
		}

		// Token: 0x06002CE8 RID: 11496 RVA: 0x0016117D File Offset: 0x0015F37D
		internal void Clear()
		{
			this.mLog.Length = 0;
			this.mLogText.SetText(null);
			this.mMessage.Length = 0;
			this.mMessageText.SetText(null);
		}

		// Token: 0x06002CE9 RID: 11497 RVA: 0x001611AF File Offset: 0x0015F3AF
		internal void SetTitle(string iTitle)
		{
			if (iTitle.Length > 0)
			{
				this.mTitleActive = true;
			}
			else
			{
				this.mTitleActive = false;
			}
			this.mTitle.SetText(iTitle);
		}

		// Token: 0x17000AA2 RID: 2722
		// (get) Token: 0x06002CEA RID: 11498 RVA: 0x001611D6 File Offset: 0x0015F3D6
		// (set) Token: 0x06002CEB RID: 11499 RVA: 0x001611DE File Offset: 0x0015F3DE
		public bool Active
		{
			get
			{
				return this.mActive;
			}
			set
			{
				this.mActive = value;
			}
		}

		// Token: 0x0400308C RID: 12428
		private static NetworkChat sSingelton;

		// Token: 0x0400308D RID: 12429
		private static volatile object sSingeltonLock = new object();

		// Token: 0x0400308E RID: 12430
		private GUIBasicEffect mEffect;

		// Token: 0x0400308F RID: 12431
		private StringBuilder mLog;

		// Token: 0x04003090 RID: 12432
		private StringBuilder mMessage;

		// Token: 0x04003091 RID: 12433
		private List<float> mMessageTTLs = new List<float>(64);

		// Token: 0x04003092 RID: 12434
		private Text mTitle;

		// Token: 0x04003093 RID: 12435
		private bool mTitleActive;

		// Token: 0x04003094 RID: 12436
		private Text mLogText;

		// Token: 0x04003095 RID: 12437
		private Text mMessageText;

		// Token: 0x04003096 RID: 12438
		private BitmapFont mFont;

		// Token: 0x04003097 RID: 12439
		private float mTimer;

		// Token: 0x04003098 RID: 12440
		private bool mInputLine;

		// Token: 0x04003099 RID: 12441
		private int mMargin;

		// Token: 0x0400309A RID: 12442
		private MenuScrollBar mScrollBar;

		// Token: 0x0400309B RID: 12443
		private Point mSize;

		// Token: 0x0400309C RID: 12444
		private Texture2D mBackgroundTexture;

		// Token: 0x0400309D RID: 12445
		private bool mScrollBarVisible;

		// Token: 0x0400309E RID: 12446
		private Vector2 mBackgroundTextureOffset;

		// Token: 0x0400309F RID: 12447
		private Vector2 mBackgroundTextureScale;

		// Token: 0x040030A0 RID: 12448
		private VertexBuffer mBackgroundVertices;

		// Token: 0x040030A1 RID: 12449
		private VertexDeclaration mBackgroundDeclaration;

		// Token: 0x040030A2 RID: 12450
		private int mScrollValue;

		// Token: 0x040030A3 RID: 12451
		private int mVisibleLines;

		// Token: 0x040030A4 RID: 12452
		private float mMessageTTL;

		// Token: 0x040030A5 RID: 12453
		private int mLineCount;

		// Token: 0x040030A6 RID: 12454
		private bool mActive;

		// Token: 0x040030A7 RID: 12455
		private bool mDirty;
	}
}
