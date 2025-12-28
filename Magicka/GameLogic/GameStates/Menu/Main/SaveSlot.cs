using System;
using Magicka.Graphics;
using Magicka.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;

namespace Magicka.GameLogic.GameStates.Menu.Main
{
	// Token: 0x0200042F RID: 1071
	public class SaveSlot : MenuItem
	{
		// Token: 0x06002135 RID: 8501 RVA: 0x000EC148 File Offset: 0x000EA348
		public SaveSlot(Vector2 iPos, int iName, int iDesc, int iCurrentTime, bool iLooped, ulong iMagicks, bool iMythos)
		{
			this.mPosition = iPos;
			this.mMythos = iMythos;
			this.mEmptySlot = (iName == SubMenuCampaignSelect_SaveSlotSelect.NEWCAMP);
			this.mNameHash = iName;
			this.mTitleHash = iDesc;
			BitmapFont font = FontManager.Instance.GetFont(MagickaFont.MenuOption);
			this.mLineHeight = (float)font.LineHeight;
			this.UpdateBoundingBox();
			this.mName = new Text(64, font, TextAlign.Left, false);
			this.mTitle = new Text(64, font, TextAlign.Left, false);
			if (this.mEmptySlot)
			{
				this.mName.SetText(LanguageManager.Instance.GetString(iName));
			}
			else
			{
				this.mName.SetText(LanguageManager.Instance.GetString(iName) + ": ");
				this.mTitle.SetText(LanguageManager.Instance.GetString(iDesc));
			}
			this.mTime = new Text(8, font, TextAlign.Left, false);
			this.SetTimeText(this.mTime, iCurrentTime);
			this.mUnlockedMagicks = new Text(6, font, TextAlign.Left, false);
			this.mLooped = iLooped;
			if (SaveSlot.sVertices == null)
			{
				SaveSlot.sTexture = Game.Instance.Content.Load<Texture2D>("UI/Menu/Pages");
				SaveSlot.sVertices = new VertexBuffer(Game.Instance.GraphicsDevice, VertexPositionTexture.SizeInBytes * Defines.QUAD_TEX_VERTS_TL.Length, BufferUsage.None);
				SaveSlot.sVertices.SetData<VertexPositionTexture>(Defines.QUAD_TEX_VERTS_TL);
				SaveSlot.sVertexDeclaration = RenderManager.Instance.CreateVertexDeclaration(VertexPositionTexture.VertexElements);
			}
		}

		// Token: 0x06002136 RID: 8502 RVA: 0x000EC2B4 File Offset: 0x000EA4B4
		private void SetTimeText(Text iText, int iTime)
		{
			if (iTime >= 359999)
			{
				iText.Characters[0] = '9';
				iText.Characters[1] = '9';
				iText.Characters[2] = ':';
				iText.Characters[3] = '5';
				iText.Characters[4] = '9';
				iText.Characters[5] = ':';
				iText.Characters[6] = '5';
				iText.Characters[7] = '9';
			}
			else
			{
				int num = 7;
				for (int i = 0; i < 3; i++)
				{
					int num2 = iTime % 60;
					iTime /= 60;
					iText.Characters[num] = (char)(48 + num2 % 10);
					num--;
					iText.Characters[num] = (char)(48 + num2 / 10);
					num--;
					if (num > 0)
					{
						iText.Characters[num] = '.';
						num--;
					}
				}
			}
			iText.MarkAsDirty();
		}

		// Token: 0x06002137 RID: 8503 RVA: 0x000EC374 File Offset: 0x000EA574
		public static void GetPlayTime(char[] iText, int iIndex, int iPlayTime)
		{
			if (iPlayTime >= 362439)
			{
				iText[iIndex] = '9';
				iText[iIndex + 1] = '9';
				iText[iIndex + 2] = ':';
				iText[iIndex + 3] = '9';
				iText[iIndex + 4] = '9';
				iText[iIndex + 5] = ':';
				iText[iIndex + 6] = '9';
				iText[iIndex + 7] = '9';
				return;
			}
			int num = iIndex + 7;
			for (int i = 0; i < 3; i++)
			{
				int num2 = iPlayTime % 60;
				iPlayTime /= 60;
				iText[num] = (char)(48 + num2 % 10);
				num--;
				iText[num] = (char)(48 + num2 / 10);
				num--;
				if (num > 0)
				{
					iText[num] = ':';
					num--;
				}
			}
		}

		// Token: 0x06002138 RID: 8504 RVA: 0x000EC408 File Offset: 0x000EA608
		public static void GetPlayTime(out string oText, int iPlayTime)
		{
			if (iPlayTime >= 362439)
			{
				oText = "99:99:99";
				return;
			}
			oText = string.Format("{0:D2}:{1:D2}:{2:D2}", iPlayTime / 3600 % 60, iPlayTime / 60 % 60, iPlayTime % 60);
		}

		// Token: 0x06002139 RID: 8505 RVA: 0x000EC458 File Offset: 0x000EA658
		public void Set(int iName, int iDesc, int iTime, bool iLooped, ulong iMagicks, bool iMythos)
		{
			SaveSlot.GetPlayTime(this.mTime.Characters, 0, iTime);
			this.mTime.MarkAsDirty();
			this.mMythos = iMythos;
			this.mEmptySlot = (iName == SubMenuCampaignSelect_SaveSlotSelect.NEWCAMP);
			this.mNameHash = iName;
			if (this.mEmptySlot)
			{
				this.mName.SetText(LanguageManager.Instance.GetString(iName));
			}
			else
			{
				this.mName.SetText(LanguageManager.Instance.GetString(iName) + ": ");
				this.mTitle.SetText(LanguageManager.Instance.GetString(iDesc));
				this.mTitleHash = iDesc;
			}
			this.mLooped = iLooped;
			if (this.mMythos)
			{
				int num = Helper.CountSetBits(1040187402UL);
				int num2 = Helper.CountSetBits(iMagicks);
				this.mUnlockedMagicks.SetText(num2 + "/" + num);
				return;
			}
			int num3 = Helper.CountSetBits(8384510UL);
			int num4 = Helper.CountSetBits(iMagicks);
			this.mUnlockedMagicks.SetText(num4 + "/" + num3);
		}

		// Token: 0x0600213A RID: 8506 RVA: 0x000EC577 File Offset: 0x000EA777
		public override void Draw(GUIBasicEffect iEffect)
		{
			this.Draw(iEffect, 1f);
		}

		// Token: 0x0600213B RID: 8507 RVA: 0x000EC588 File Offset: 0x000EA788
		public override void Draw(GUIBasicEffect iEffect, float iScale)
		{
			iEffect.VertexColorEnabled = false;
			iEffect.GraphicsDevice.Vertices[0].SetSource(SaveSlot.sVertices, 0, VertexPositionTexture.SizeInBytes);
			iEffect.GraphicsDevice.VertexDeclaration = SaveSlot.sVertexDeclaration;
			iEffect.TextureEnabled = true;
			iEffect.TextureOffset = Vector2.Zero;
			iEffect.TextureScale = new Vector2(SaveSlot.sBackgroundSize.X / (float)SaveSlot.sTexture.Width, SaveSlot.sBackgroundSize.Y / (float)SaveSlot.sTexture.Height);
			iEffect.Texture = SaveSlot.sTexture;
			iEffect.Color = (this.mSelected ? new Vector4(1.1f) : Vector4.One);
			Matrix identity = Matrix.Identity;
			identity.M11 = SaveSlot.sBackgroundSize.X;
			identity.M22 = SaveSlot.sBackgroundSize.Y;
			identity.M41 = this.mPosition.X - 384f;
			identity.M42 = this.mPosition.Y - 128f;
			iEffect.Saturation = (this.mEmptySlot ? 0f : (this.mSelected ? 1.2f : 1f));
			iEffect.Transform = identity;
			iEffect.CommitChanges();
			iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
			if (!this.mEmptySlot)
			{
				if (this.mLooped)
				{
					iEffect.TextureOffset = SaveSlot.sLoopedOffset;
					iEffect.TextureScale = new Vector2(SaveSlot.sLoopedSize.X / (float)SaveSlot.sTexture.Width, SaveSlot.sLoopedSize.Y / (float)SaveSlot.sTexture.Height);
					identity.M11 = SaveSlot.sLoopedSize.X;
					identity.M22 = SaveSlot.sLoopedSize.Y;
					identity.M41 = this.mPosition.X + 256f;
					identity.M42 = this.mPosition.Y - 32f - 4f;
					iEffect.Transform = identity;
					iEffect.Saturation = (this.mLooped ? 1f : 0f);
					iEffect.CommitChanges();
					iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
					iEffect.Saturation = 1f;
				}
				iEffect.TextureOffset = SaveSlot.sMagickOffset;
				iEffect.TextureScale = new Vector2(SaveSlot.sMagickSize.X / (float)SaveSlot.sTexture.Width, SaveSlot.sMagickSize.Y / (float)SaveSlot.sTexture.Height);
				identity.M11 = SaveSlot.sMagickSize.X;
				identity.M22 = SaveSlot.sMagickSize.Y;
				identity.M41 = this.mPosition.X - 256f - 32f;
				identity.M42 = this.mPosition.Y - 32f + 64f - 10f;
				iEffect.Transform = identity;
				iEffect.CommitChanges();
				iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
				iEffect.TextureOffset = SaveSlot.sTimeOffset;
				iEffect.TextureScale = new Vector2(SaveSlot.sTimeSize.X / (float)SaveSlot.sTexture.Width, SaveSlot.sTimeSize.Y / (float)SaveSlot.sTexture.Height);
				identity.M11 = SaveSlot.sTimeSize.X;
				identity.M22 = SaveSlot.sTimeSize.Y;
				identity.M41 += 192f;
				iEffect.Transform = identity;
				iEffect.Saturation = (this.mLooped ? 1f : 0f);
				iEffect.CommitChanges();
				iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
				iEffect.Color = (this.mSelected ? MenuItem.COLOR_SELECTED : MenuItem.COLOR);
				this.mUnlockedMagicks.Draw(iEffect, this.mPosition.X - 256f + 64f - 16f, this.mPosition.Y + 48f - 8f);
				this.mTime.Draw(iEffect, this.mPosition.X - 256f + 64f + 128f + 48f - 16f, this.mPosition.Y + 48f - 8f);
				this.mTitle.Draw(iEffect, base.Position.X - 256f - 32f + 64f, this.mPosition.Y - 8f - 4f);
			}
			else
			{
				iEffect.Color = (this.mSelected ? MenuItem.COLOR_SELECTED : MenuItem.COLOR);
			}
			this.mName.Draw(iEffect, this.mPosition.X - 256f - 32f, this.mPosition.Y - 48f - 4f);
		}

		// Token: 0x0600213C RID: 8508 RVA: 0x000ECA78 File Offset: 0x000EAC78
		protected override void UpdateBoundingBox()
		{
			this.mTopLeft.X = this.mPosition.X - 384f;
			this.mTopLeft.Y = this.mPosition.Y - 128f;
			this.mBottomRight.X = this.mPosition.X + 384f;
			this.mBottomRight.Y = this.mPosition.Y + 128f;
		}

		// Token: 0x0600213D RID: 8509 RVA: 0x000ECAF8 File Offset: 0x000EACF8
		public override void LanguageChanged()
		{
			if (this.mEmptySlot)
			{
				this.mName.SetText(LanguageManager.Instance.GetString(this.mNameHash));
				return;
			}
			this.mName.SetText(LanguageManager.Instance.GetString(this.mNameHash) + ": ");
			this.mTitle.SetText(LanguageManager.Instance.GetString(this.mTitleHash));
		}

		// Token: 0x1700081C RID: 2076
		// (get) Token: 0x0600213E RID: 8510 RVA: 0x000ECB69 File Offset: 0x000EAD69
		internal bool EmptySlot
		{
			get
			{
				return this.mEmptySlot;
			}
		}

		// Token: 0x040023C6 RID: 9158
		private const float BORDERWIDTH = 1f;

		// Token: 0x040023C7 RID: 9159
		public const float SLOTWIDTH = 768f;

		// Token: 0x040023C8 RID: 9160
		public const float SLOTHEIGHT = 256f;

		// Token: 0x040023C9 RID: 9161
		private static VertexBuffer sVertices;

		// Token: 0x040023CA RID: 9162
		private static VertexDeclaration sVertexDeclaration;

		// Token: 0x040023CB RID: 9163
		private static Texture2D sTexture;

		// Token: 0x040023CC RID: 9164
		private Text mName;

		// Token: 0x040023CD RID: 9165
		private int mNameHash;

		// Token: 0x040023CE RID: 9166
		private Text mTitle;

		// Token: 0x040023CF RID: 9167
		private int mTitleHash;

		// Token: 0x040023D0 RID: 9168
		private Text mTime;

		// Token: 0x040023D1 RID: 9169
		private Text mUnlockedMagicks;

		// Token: 0x040023D2 RID: 9170
		private bool mLooped;

		// Token: 0x040023D3 RID: 9171
		private bool mEmptySlot;

		// Token: 0x040023D4 RID: 9172
		private float mLineHeight;

		// Token: 0x040023D5 RID: 9173
		public static readonly Vector2 sBackgroundSize = new Vector2(784f, 240f);

		// Token: 0x040023D6 RID: 9174
		private static readonly Vector2 sLoopedSize = new Vector2(64f, 64f);

		// Token: 0x040023D7 RID: 9175
		private static readonly Vector2 sLoopedOffset = new Vector2(0.71875f, 0.09375f);

		// Token: 0x040023D8 RID: 9176
		private static readonly Vector2 sTimeSize = new Vector2(64f, 64f);

		// Token: 0x040023D9 RID: 9177
		private static readonly Vector2 sTimeOffset = new Vector2(0.6875f, 0.09375f);

		// Token: 0x040023DA RID: 9178
		private static readonly Vector2 sMagickSize = new Vector2(64f, 64f);

		// Token: 0x040023DB RID: 9179
		private static readonly Vector2 sMagickOffset = new Vector2(0.65625f, 0.09375f);

		// Token: 0x040023DC RID: 9180
		private bool mMythos;
	}
}
