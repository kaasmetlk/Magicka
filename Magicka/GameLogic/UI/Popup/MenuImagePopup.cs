using System;
using System.Collections.Generic;
using Magicka.GameLogic.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PolygonHead.Effects;

namespace Magicka.GameLogic.UI.Popup
{
	// Token: 0x02000427 RID: 1063
	public class MenuImagePopup : MenuBasePopup
	{
		// Token: 0x060020E6 RID: 8422 RVA: 0x000E9AB0 File Offset: 0x000E7CB0
		static MenuImagePopup()
		{
			VertexPositionTexture[] array = new VertexPositionTexture[]
			{
				new VertexPositionTexture(new Vector3(0f, 0f, 0f), new Vector2(0f, 0f)),
				new VertexPositionTexture(new Vector3(1f, 0f, 0f), new Vector2(1f, 0f)),
				new VertexPositionTexture(new Vector3(1f, 1f, 0f), new Vector2(1f, 1f)),
				new VertexPositionTexture(new Vector3(0f, 1f, 0f), new Vector2(0f, 1f))
			};
			lock (Game.Instance.GraphicsDevice)
			{
				MenuImagePopup.sVertices = new VertexBuffer(Game.Instance.GraphicsDevice, array.Length * VertexPositionTexture.SizeInBytes, BufferUsage.WriteOnly);
				MenuImagePopup.sVertices.SetData<VertexPositionTexture>(array);
				MenuImagePopup.sDeclaration = new VertexDeclaration(Game.Instance.GraphicsDevice, VertexPositionTexture.VertexElements);
			}
			MenuImagePopup.sPagesTexture = Game.Instance.Content.Load<Texture2D>("UI/Menu/Pages");
		}

		// Token: 0x060020E7 RID: 8423 RVA: 0x000E9C28 File Offset: 0x000E7E28
		public MenuImagePopup(Texture2D iTexture, Vector2 iTextureUV, Vector2 iTextureSize)
		{
			this.mTexture = iTexture;
			Vector2 value = new Vector2((float)this.mTexture.Width, (float)this.mTexture.Height);
			this.mTextureOffset = iTextureUV / value;
			this.mTextureScale = iTextureSize / value;
			this.mSize = iTextureSize;
			this.mHitBox.Width = (int)this.mSize.X;
			this.mHitBox.Height = (int)this.mSize.Y;
			this.UpdateBoundingBox();
			this.mTransform = Matrix.Identity;
			this.mTransform.M11 = this.mSize.X;
			this.mTransform.M22 = this.mSize.Y;
			this.mTransform.M41 = this.mPosition.X;
			this.mTransform.M42 = this.mPosition.Y;
			this.mRenderSections = new List<RenderSection>();
			this.mRenderSections.Add(new RenderSection(Vector2.Zero, Vector2.One, Vector2.Zero, Vector2.One));
		}

		// Token: 0x060020E8 RID: 8424 RVA: 0x000E9D60 File Offset: 0x000E7F60
		public void EnableThreePatch(Vector2 iInsets, Vector2 iProportions)
		{
			this.mDrawMode = PopupDrawMode.ThreePatch;
			this.mRenderModeSettings = new RenderModeSettings(new Vector4(iInsets, 0f, 0f), new Vector4(iProportions, 0f, 0f));
			this.mRenderSections.Clear();
			this.CreateThreePatch();
		}

		// Token: 0x060020E9 RID: 8425 RVA: 0x000E9DB0 File Offset: 0x000E7FB0
		private void CreateThreePatch()
		{
			float x = 1f - (this.mRenderModeSettings.Proportions.X + this.mRenderModeSettings.Proportions.Y);
			this.mRenderSections.Add(new RenderSection(Vector2.Zero, new Vector2(this.mRenderModeSettings.Insets.X, 1f), Vector2.Zero, new Vector2(this.mRenderModeSettings.Proportions.X, 1f)));
			this.mRenderSections.Add(new RenderSection(new Vector2(this.mRenderModeSettings.Insets.X, 0f), new Vector2(this.mRenderModeSettings.Insets.X + this.mRenderModeSettings.Insets.Y, 1f), new Vector2(this.mRenderModeSettings.Proportions.X, 0f), new Vector2(x, 1f)));
			this.mRenderSections.Add(new RenderSection(new Vector2(1f - this.mRenderModeSettings.Insets.Y, 0f), new Vector2(this.mRenderModeSettings.Insets.Y, 1f), new Vector2(1f - this.mRenderModeSettings.Proportions.Y, 0f), new Vector2(this.mRenderModeSettings.Proportions.Y, 1f)));
		}

		// Token: 0x060020EA RID: 8426 RVA: 0x000E9F31 File Offset: 0x000E8131
		public void EnableNinePatch(Vector4 iInsets, Vector4 iProportions)
		{
			this.mDrawMode = PopupDrawMode.NinePatch;
			this.mRenderModeSettings = new RenderModeSettings(iInsets, iProportions);
			this.mRenderSections.Clear();
			this.CreateNinePatch();
		}

		// Token: 0x060020EB RID: 8427 RVA: 0x000E9F58 File Offset: 0x000E8158
		private void CreateNinePatch()
		{
			float x = 1f - (this.mRenderModeSettings.Insets.X + this.mRenderModeSettings.Insets.Y);
			float y = 1f - (this.mRenderModeSettings.Insets.Z + this.mRenderModeSettings.Insets.W);
			float x2 = 1f - (this.mRenderModeSettings.Proportions.X + this.mRenderModeSettings.Proportions.Y);
			float y2 = 1f - (this.mRenderModeSettings.Proportions.Z + this.mRenderModeSettings.Proportions.W);
			this.mRenderSections.Add(new RenderSection(Vector2.Zero, new Vector2(this.mRenderModeSettings.Insets.X, this.mRenderModeSettings.Insets.Z), Vector2.Zero, new Vector2(this.mRenderModeSettings.Proportions.X, this.mRenderModeSettings.Proportions.Z)));
			this.mRenderSections.Add(new RenderSection(new Vector2(this.mRenderModeSettings.Insets.X, 0f), new Vector2(x, this.mRenderModeSettings.Insets.Z), new Vector2(this.mRenderModeSettings.Proportions.X, 0f), new Vector2(x2, this.mRenderModeSettings.Proportions.Z)));
			this.mRenderSections.Add(new RenderSection(new Vector2(1f - this.mRenderModeSettings.Insets.Y, 0f), new Vector2(this.mRenderModeSettings.Insets.Y, this.mRenderModeSettings.Insets.Z), new Vector2(1f - this.mRenderModeSettings.Proportions.Y, 0f), new Vector2(this.mRenderModeSettings.Proportions.Y, this.mRenderModeSettings.Proportions.Z)));
			this.mRenderSections.Add(new RenderSection(new Vector2(0f, this.mRenderModeSettings.Insets.Z), new Vector2(this.mRenderModeSettings.Insets.X, y), new Vector2(0f, this.mRenderModeSettings.Proportions.Z), new Vector2(this.mRenderModeSettings.Proportions.X, y2)));
			this.mRenderSections.Add(new RenderSection(new Vector2(this.mRenderModeSettings.Insets.X, this.mRenderModeSettings.Insets.Z), new Vector2(x, y), new Vector2(this.mRenderModeSettings.Proportions.X, this.mRenderModeSettings.Proportions.Z), new Vector2(x2, y2)));
			this.mRenderSections.Add(new RenderSection(new Vector2(1f - this.mRenderModeSettings.Insets.Y, this.mRenderModeSettings.Insets.Z), new Vector2(this.mRenderModeSettings.Insets.Y, y), new Vector2(1f - this.mRenderModeSettings.Proportions.Y, this.mRenderModeSettings.Proportions.Z), new Vector2(this.mRenderModeSettings.Proportions.Y, y2)));
			this.mRenderSections.Add(new RenderSection(new Vector2(0f, 1f - this.mRenderModeSettings.Insets.W), new Vector2(this.mRenderModeSettings.Insets.X, this.mRenderModeSettings.Insets.W), new Vector2(0f, 1f - this.mRenderModeSettings.Proportions.W), new Vector2(this.mRenderModeSettings.Proportions.X, this.mRenderModeSettings.Proportions.W)));
			this.mRenderSections.Add(new RenderSection(new Vector2(this.mRenderModeSettings.Insets.X, 1f - this.mRenderModeSettings.Insets.W), new Vector2(x, this.mRenderModeSettings.Insets.W), new Vector2(this.mRenderModeSettings.Proportions.X, 1f - this.mRenderModeSettings.Proportions.W), new Vector2(x2, this.mRenderModeSettings.Proportions.W)));
			this.mRenderSections.Add(new RenderSection(new Vector2(1f - this.mRenderModeSettings.Insets.Y, 1f - this.mRenderModeSettings.Insets.W), new Vector2(this.mRenderModeSettings.Insets.Y, this.mRenderModeSettings.Insets.W), new Vector2(1f - this.mRenderModeSettings.Proportions.Y, 1f - this.mRenderModeSettings.Proportions.W), new Vector2(this.mRenderModeSettings.Proportions.Y, this.mRenderModeSettings.Proportions.W)));
		}

		// Token: 0x060020EC RID: 8428 RVA: 0x000EA4C8 File Offset: 0x000E86C8
		public override void Draw(GUIBasicEffect iEffect)
		{
			iEffect.GraphicsDevice.Vertices[0].SetSource(MenuImagePopup.sVertices, 0, VertexPositionTexture.SizeInBytes);
			iEffect.GraphicsDevice.VertexDeclaration = MenuImagePopup.sDeclaration;
			iEffect.VertexColorEnabled = false;
			iEffect.Color = this.mColour;
			iEffect.Texture = this.mTexture;
			iEffect.TextureEnabled = true;
			iEffect.Saturation = 1f;
			foreach (RenderSection renderSection in this.mRenderSections)
			{
				this.DrawSection(iEffect, renderSection.Position, renderSection.Size, renderSection.TextureOffset, renderSection.TextureSize);
			}
			iEffect.GraphicsDevice.Vertices[0].SetSource(null, 0, 0);
		}

		// Token: 0x060020ED RID: 8429 RVA: 0x000EA5B4 File Offset: 0x000E87B4
		protected void DrawSection(GUIBasicEffect iEffect, Vector2 iPositionOffset, Vector2 iSize, Vector2 iTextureOffset, Vector2 iTextureSize)
		{
			Vector2 positionFromAlignment = base.GetPositionFromAlignment();
			Matrix mTransform = this.mTransform;
			mTransform.M11 = this.mSize.X * iSize.X * this.mScale;
			mTransform.M22 = this.mSize.Y * iSize.Y * this.mScale;
			mTransform.M41 = positionFromAlignment.X + this.mSize.X * iPositionOffset.X * this.mScale;
			mTransform.M42 = positionFromAlignment.Y + this.mSize.Y * iPositionOffset.Y * this.mScale;
			iEffect.Transform = mTransform;
			iEffect.TextureOffset = this.mTextureOffset + this.mTextureScale * iTextureOffset;
			iEffect.TextureScale = this.mTextureScale * iTextureSize;
			iEffect.CommitChanges();
			iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
		}

		// Token: 0x060020EE RID: 8430 RVA: 0x000EA6AF File Offset: 0x000E88AF
		internal override void ControllerA(Controller iSender)
		{
			this.Dismiss();
			if (this.mOnPositiveClickDelegate != null)
			{
				this.mOnPositiveClickDelegate.Invoke();
				this.mOnPositiveClickDelegate = null;
			}
		}

		// Token: 0x060020EF RID: 8431 RVA: 0x000EA6D1 File Offset: 0x000E88D1
		internal override void ControllerMouseAction(Controller iSender, Point iScreenSize, MouseState iState, MouseState iOldState)
		{
			this.Dismiss();
			if (this.mOnPositiveClickDelegate != null)
			{
				this.mOnPositiveClickDelegate.Invoke();
				this.mOnPositiveClickDelegate = null;
			}
		}

		// Token: 0x0400236E RID: 9070
		private const string PAGES_TEXTURE = "UI/Menu/Pages";

		// Token: 0x0400236F RID: 9071
		private const string PAGES_TEXTURE_DEMO = "UI/Menu/Pages_demo";

		// Token: 0x04002370 RID: 9072
		protected static readonly VertexBuffer sVertices = null;

		// Token: 0x04002371 RID: 9073
		protected static readonly VertexDeclaration sDeclaration = null;

		// Token: 0x04002372 RID: 9074
		protected static readonly Texture2D sPagesTexture;

		// Token: 0x04002373 RID: 9075
		protected Texture2D mTexture;

		// Token: 0x04002374 RID: 9076
		protected Vector2 mTextureOffset = Vector2.Zero;

		// Token: 0x04002375 RID: 9077
		protected Vector2 mTextureScale = Vector2.One;

		// Token: 0x04002376 RID: 9078
		private PopupDrawMode mDrawMode;

		// Token: 0x04002377 RID: 9079
		private RenderModeSettings mRenderModeSettings;

		// Token: 0x04002378 RID: 9080
		private List<RenderSection> mRenderSections;
	}
}
