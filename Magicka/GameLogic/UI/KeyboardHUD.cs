using System;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;

namespace Magicka.GameLogic.UI
{
	// Token: 0x0200048F RID: 1167
	internal class KeyboardHUD
	{
		// Token: 0x1700086F RID: 2159
		// (get) Token: 0x06002360 RID: 9056 RVA: 0x000FD190 File Offset: 0x000FB390
		public static KeyboardHUD Instance
		{
			get
			{
				if (KeyboardHUD.mSingelton == null)
				{
					lock (KeyboardHUD.mSingeltonLock)
					{
						if (KeyboardHUD.mSingelton == null)
						{
							KeyboardHUD.mSingelton = new KeyboardHUD();
						}
					}
				}
				return KeyboardHUD.mSingelton;
			}
		}

		// Token: 0x06002361 RID: 9057 RVA: 0x000FD1E4 File Offset: 0x000FB3E4
		private KeyboardHUD()
		{
			Texture2D texture2D = null;
			lock (Game.Instance.GraphicsDevice)
			{
				texture2D = Game.Instance.Content.Load<Texture2D>("UI/HUD/hud");
			}
			GUIBasicEffect guibasicEffect = new GUIBasicEffect(Game.Instance.GraphicsDevice, null);
			guibasicEffect.TextureEnabled = true;
			guibasicEffect.Color = new Vector4(1f);
			float num = 1f / (float)texture2D.Width;
			float num2 = 1f / (float)texture2D.Height;
			VertexPositionTexture[] array = new VertexPositionTexture[66];
			float num3 = 50f * num;
			float num4 = 50f * num2;
			float x = 50f;
			float y = 50f;
			float num5 = 156f * num2;
			VertexPositionTexture vertexPositionTexture = default(VertexPositionTexture);
			for (int i = 0; i < 8; i++)
			{
				int num6 = i * 6;
				float num7 = (float)(i % 5);
				int num8 = i / 5;
				vertexPositionTexture.Position.X = 0f;
				vertexPositionTexture.Position.Y = y;
				vertexPositionTexture.TextureCoordinate.X = num7 * num3;
				vertexPositionTexture.TextureCoordinate.Y = num5 + (float)num8 * num4 + num4;
				array[num6] = vertexPositionTexture;
				vertexPositionTexture.Position.X = 0f;
				vertexPositionTexture.Position.Y = 0f;
				vertexPositionTexture.TextureCoordinate.X = num7 * num3;
				vertexPositionTexture.TextureCoordinate.Y = num5 + (float)num8 * num4;
				array[num6 + 1] = vertexPositionTexture;
				vertexPositionTexture.Position.X = x;
				vertexPositionTexture.Position.Y = 0f;
				vertexPositionTexture.TextureCoordinate.X = num7 * num3 + num3;
				vertexPositionTexture.TextureCoordinate.Y = num5 + (float)num8 * num4;
				array[num6 + 2] = vertexPositionTexture;
				vertexPositionTexture.Position.X = x;
				vertexPositionTexture.Position.Y = 0f;
				vertexPositionTexture.TextureCoordinate.X = num7 * num3 + num3;
				vertexPositionTexture.TextureCoordinate.Y = num5 + (float)num8 * num4;
				array[num6 + 3] = vertexPositionTexture;
				vertexPositionTexture.Position.X = x;
				vertexPositionTexture.Position.Y = y;
				vertexPositionTexture.TextureCoordinate.X = num7 * num3 + num3;
				vertexPositionTexture.TextureCoordinate.Y = num5 + (float)num8 * num4 + num4;
				array[num6 + 4] = vertexPositionTexture;
				vertexPositionTexture.Position.X = 0f;
				vertexPositionTexture.Position.Y = y;
				vertexPositionTexture.TextureCoordinate.X = num7 * num3;
				vertexPositionTexture.TextureCoordinate.Y = num5 + (float)num8 * num4 + num4;
				array[num6 + 5] = vertexPositionTexture;
			}
			x = 25f;
			y = 25f;
			num3 = 25f * num;
			num4 = 25f * num2;
			vertexPositionTexture.Position.X = 0f;
			vertexPositionTexture.Position.Y = y;
			vertexPositionTexture.TextureCoordinate.X = 0f;
			vertexPositionTexture.TextureCoordinate.Y = num4;
			array[48] = vertexPositionTexture;
			vertexPositionTexture.Position.X = 0f;
			vertexPositionTexture.Position.Y = 0f;
			vertexPositionTexture.TextureCoordinate.X = 0f;
			vertexPositionTexture.TextureCoordinate.Y = 0f;
			array[49] = vertexPositionTexture;
			vertexPositionTexture.Position.X = x;
			vertexPositionTexture.Position.Y = 0f;
			vertexPositionTexture.TextureCoordinate.X = num3;
			vertexPositionTexture.TextureCoordinate.Y = 0f;
			array[50] = vertexPositionTexture;
			vertexPositionTexture.Position.X = x;
			vertexPositionTexture.Position.Y = 0f;
			vertexPositionTexture.TextureCoordinate.X = num3;
			vertexPositionTexture.TextureCoordinate.Y = 0f;
			array[51] = vertexPositionTexture;
			vertexPositionTexture.Position.X = x;
			vertexPositionTexture.Position.Y = y;
			vertexPositionTexture.TextureCoordinate.X = num3;
			vertexPositionTexture.TextureCoordinate.Y = num4;
			array[52] = vertexPositionTexture;
			vertexPositionTexture.Position.X = 0f;
			vertexPositionTexture.Position.Y = y;
			vertexPositionTexture.TextureCoordinate.X = 0f;
			vertexPositionTexture.TextureCoordinate.Y = num4;
			array[53] = vertexPositionTexture;
			float num9 = 25f * num;
			x = 50f;
			y = 50f;
			vertexPositionTexture.Position.X = 0f;
			vertexPositionTexture.Position.Y = y;
			vertexPositionTexture.TextureCoordinate.X = num9;
			vertexPositionTexture.TextureCoordinate.Y = num4;
			array[54] = vertexPositionTexture;
			vertexPositionTexture.Position.X = 0f;
			vertexPositionTexture.Position.Y = 0f;
			vertexPositionTexture.TextureCoordinate.X = num9;
			vertexPositionTexture.TextureCoordinate.Y = 0f;
			array[55] = vertexPositionTexture;
			vertexPositionTexture.Position.X = x;
			vertexPositionTexture.Position.Y = 0f;
			vertexPositionTexture.TextureCoordinate.X = num9 + num3;
			vertexPositionTexture.TextureCoordinate.Y = 0f;
			array[56] = vertexPositionTexture;
			vertexPositionTexture.Position.X = x;
			vertexPositionTexture.Position.Y = 0f;
			vertexPositionTexture.TextureCoordinate.X = num9 + num3;
			vertexPositionTexture.TextureCoordinate.Y = 0f;
			array[57] = vertexPositionTexture;
			vertexPositionTexture.Position.X = x;
			vertexPositionTexture.Position.Y = y;
			vertexPositionTexture.TextureCoordinate.X = num9 + num3;
			vertexPositionTexture.TextureCoordinate.Y = num4;
			array[58] = vertexPositionTexture;
			vertexPositionTexture.Position.X = 0f;
			vertexPositionTexture.Position.Y = y;
			vertexPositionTexture.TextureCoordinate.X = num9;
			vertexPositionTexture.TextureCoordinate.Y = num4;
			array[59] = vertexPositionTexture;
			num9 = 50f * num;
			x = 37f;
			y = 25f;
			vertexPositionTexture.Position.X = 0f;
			vertexPositionTexture.Position.Y = y;
			vertexPositionTexture.TextureCoordinate.X = num9;
			vertexPositionTexture.TextureCoordinate.Y = num4;
			array[60] = vertexPositionTexture;
			vertexPositionTexture.Position.X = 0f;
			vertexPositionTexture.Position.Y = 0f;
			vertexPositionTexture.TextureCoordinate.X = num9;
			vertexPositionTexture.TextureCoordinate.Y = 0f;
			array[61] = vertexPositionTexture;
			vertexPositionTexture.Position.X = x;
			vertexPositionTexture.Position.Y = 0f;
			vertexPositionTexture.TextureCoordinate.X = num9 + num3;
			vertexPositionTexture.TextureCoordinate.Y = 0f;
			array[62] = vertexPositionTexture;
			vertexPositionTexture.Position.X = x;
			vertexPositionTexture.Position.Y = 0f;
			vertexPositionTexture.TextureCoordinate.X = num9 + num3;
			vertexPositionTexture.TextureCoordinate.Y = 0f;
			array[63] = vertexPositionTexture;
			vertexPositionTexture.Position.X = x;
			vertexPositionTexture.Position.Y = y;
			vertexPositionTexture.TextureCoordinate.X = num9 + num3;
			vertexPositionTexture.TextureCoordinate.Y = num4;
			array[64] = vertexPositionTexture;
			vertexPositionTexture.Position.X = 0f;
			vertexPositionTexture.Position.Y = y;
			vertexPositionTexture.TextureCoordinate.X = num9;
			vertexPositionTexture.TextureCoordinate.Y = num4;
			array[65] = vertexPositionTexture;
			VertexBuffer vertexBuffer;
			VertexDeclaration iVertexDeclaration;
			lock (Game.Instance.GraphicsDevice)
			{
				vertexBuffer = new VertexBuffer(Game.Instance.GraphicsDevice, VertexPositionTexture.SizeInBytes * array.Length, BufferUsage.WriteOnly);
				vertexBuffer.SetData<VertexPositionTexture>(array);
				iVertexDeclaration = new VertexDeclaration(Game.Instance.GraphicsDevice, VertexPositionTexture.VertexElements);
			}
			this.mIcons = new KeyboardHUD.Icon[8];
			for (int j = 0; j < this.mIcons.Length; j++)
			{
				this.mIcons[j].Enabled = true;
				this.mIcons[j].Saturation = 1f;
				this.mIcons[j].Intensity = 1f;
				this.mIcons[j].Cooldown = 0f;
				this.mIcons[j].ResetTimer = 0f;
			}
			this.mKeyTexts = new Text[8];
			for (int k = 0; k < 8; k++)
			{
				this.mKeyTexts[k] = new Text(32, FontManager.Instance.GetFont(MagickaFont.Maiandra14), TextAlign.Center, false);
				this.mKeyTexts[k].SetText("X");
			}
			this.mRenderData = new KeyboardHUD.RenderData[3];
			for (int l = 0; l < 3; l++)
			{
				this.mRenderData[l] = new KeyboardHUD.RenderData(guibasicEffect, texture2D, vertexBuffer, iVertexDeclaration, VertexPositionTexture.SizeInBytes, this.mKeyTexts);
				this.mRenderData[l].Icons = new KeyboardHUD.Icon[8];
			}
			this.UpdateControls();
		}

		// Token: 0x06002362 RID: 9058 RVA: 0x000FDC68 File Offset: 0x000FBE68
		public void Reset()
		{
			for (int i = 0; i < this.mIcons.Length; i++)
			{
				this.mIcons[i].Enabled = true;
			}
		}

		// Token: 0x06002363 RID: 9059 RVA: 0x000FDC9C File Offset: 0x000FBE9C
		public void CoolDown(Elements iElement)
		{
			int num = Spell.ElementIndex(iElement);
			this.mIcons[num].Cooldown = 0.125f;
		}

		// Token: 0x06002364 RID: 9060 RVA: 0x000FDCC6 File Offset: 0x000FBEC6
		public bool IconCoolDown(Elements iElement)
		{
			return this.mIcons[Spell.ElementIndex(iElement)].Cooldown <= 0f;
		}

		// Token: 0x06002365 RID: 9061 RVA: 0x000FDCE8 File Offset: 0x000FBEE8
		public void Enable(Elements iElement)
		{
			for (int i = 0; i < 11; i++)
			{
				Elements elements = Spell.ElementFromIndex(i);
				if ((elements & iElement) == elements)
				{
					this.mIcons[i].Enabled = true;
				}
			}
		}

		// Token: 0x06002366 RID: 9062 RVA: 0x000FDD24 File Offset: 0x000FBF24
		public void Disable(Elements iElement)
		{
			for (int i = 0; i < this.mIcons.Length; i++)
			{
				Elements elements = Spell.ElementFromIndex(i);
				if ((elements & iElement) == elements)
				{
					this.mIcons[i].Enabled = false;
					this.mIcons[i].Intensity = 1f;
				}
			}
		}

		// Token: 0x06002367 RID: 9063 RVA: 0x000FDD7C File Offset: 0x000FBF7C
		public void UpdateControls()
		{
			this.mKeyTexts[0].SetText(KeyboardMouseController.KeyToString(KeyboardBindings.Earth));
			this.mKeyTexts[1].SetText(KeyboardMouseController.KeyToString(KeyboardBindings.Water));
			this.mKeyTexts[2].SetText(KeyboardMouseController.KeyToString(KeyboardBindings.Cold));
			this.mKeyTexts[3].SetText(KeyboardMouseController.KeyToString(KeyboardBindings.Fire));
			this.mKeyTexts[4].SetText(KeyboardMouseController.KeyToString(KeyboardBindings.Lightning));
			this.mKeyTexts[5].SetText(KeyboardMouseController.KeyToString(KeyboardBindings.Arcane));
			this.mKeyTexts[6].SetText(KeyboardMouseController.KeyToString(KeyboardBindings.Life));
			this.mKeyTexts[7].SetText(KeyboardMouseController.KeyToString(KeyboardBindings.Shield));
		}

		// Token: 0x06002368 RID: 9064 RVA: 0x000FDE24 File Offset: 0x000FC024
		public void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			for (int i = 0; i < this.mIcons.Length; i++)
			{
				if (this.mIcons[i].Enabled)
				{
					if (this.mIcons[i].Cooldown > 0f)
					{
						KeyboardHUD.Icon[] array = this.mIcons;
						int num = i;
						array[num].Cooldown = array[num].Cooldown - iDeltaTime;
						if (this.mIcons[i].Cooldown <= 0f)
						{
							this.mIcons[i].ResetTimer = 0.125f;
						}
						this.mIcons[i].Saturation = MathHelper.Min(this.mIcons[i].Saturation + iDeltaTime * 4f, 0.5f);
					}
					else if (this.mIcons[i].ResetTimer > 0f)
					{
						KeyboardHUD.Icon[] array2 = this.mIcons;
						int num2 = i;
						array2[num2].ResetTimer = array2[num2].ResetTimer - iDeltaTime;
						this.mIcons[i].Intensity = MathHelper.Min(this.mIcons[i].Intensity + iDeltaTime * 20f, 20f);
						this.mIcons[i].Saturation = MathHelper.Min(this.mIcons[i].Saturation + iDeltaTime * 4f, 1f);
					}
					else
					{
						this.mIcons[i].Saturation = MathHelper.Min(this.mIcons[i].Saturation + iDeltaTime * 10f, 1f);
						this.mIcons[i].Intensity = MathHelper.Min(this.mIcons[i].Intensity + iDeltaTime * 4f, 1f);
					}
				}
				else
				{
					this.mIcons[i].Saturation = MathHelper.Max(this.mIcons[i].Saturation - iDeltaTime * 4f, 0f);
				}
			}
			this.mIcons.CopyTo(this.mRenderData[(int)iDataChannel].Icons, 0);
			if (this.mUIEnabled)
			{
				GameStateManager.Instance.CurrentState.Scene.AddRenderableGUIObject(iDataChannel, this.mRenderData[(int)iDataChannel]);
			}
		}

		// Token: 0x17000870 RID: 2160
		// (get) Token: 0x06002369 RID: 9065 RVA: 0x000FE078 File Offset: 0x000FC278
		// (set) Token: 0x0600236A RID: 9066 RVA: 0x000FE080 File Offset: 0x000FC280
		public bool UIEnabled
		{
			get
			{
				return this.mUIEnabled;
			}
			set
			{
				this.mUIEnabled = value;
			}
		}

		// Token: 0x04002666 RID: 9830
		private const float RESETTIME = 0.125f;

		// Token: 0x04002667 RID: 9831
		private const float BLOOMTIME_DIVISOR = 8f;

		// Token: 0x04002668 RID: 9832
		private const float COOLDOWN = 0.125f;

		// Token: 0x04002669 RID: 9833
		private const float COOLDOWN_DIVISOR = 8f;

		// Token: 0x0400266A RID: 9834
		private static KeyboardHUD mSingelton;

		// Token: 0x0400266B RID: 9835
		private static volatile object mSingeltonLock = new object();

		// Token: 0x0400266C RID: 9836
		private Text[] mKeyTexts;

		// Token: 0x0400266D RID: 9837
		private KeyboardHUD.Icon[] mIcons;

		// Token: 0x0400266E RID: 9838
		private KeyboardHUD.RenderData[] mRenderData;

		// Token: 0x0400266F RID: 9839
		private bool mUIEnabled = true;

		// Token: 0x02000490 RID: 1168
		protected class RenderData : IRenderableGUIObject
		{
			// Token: 0x0600236C RID: 9068 RVA: 0x000FE097 File Offset: 0x000FC297
			public RenderData(GUIBasicEffect iEffect, Texture2D iHudTexture, VertexBuffer iVertexBuffer, VertexDeclaration iVertexDeclaration, int iVertexStride, Text[] iKeyTexts)
			{
				this.mEffect = iEffect;
				this.mHudTexture = iHudTexture;
				this.mVertexBuffer = iVertexBuffer;
				this.mVertexDeclaration = iVertexDeclaration;
				this.mVertexStride = iVertexStride;
				this.mKeyTexts = iKeyTexts;
				this.mTransform = Matrix.Identity;
			}

			// Token: 0x0600236D RID: 9069 RVA: 0x000FE0D8 File Offset: 0x000FC2D8
			public void Draw(float iDeltaTime)
			{
				this.mScreenSize = RenderManager.Instance.ScreenSize;
				this.mEffect.Begin();
				this.mEffect.CurrentTechnique.Passes[0].Begin();
				float iXOffset = 25f;
				this.DrawIcon(0, 1, 0f, 0f);
				this.DrawIcon(0, 4, iXOffset, 60f);
				this.DrawIcon(1, 6, 0f, 0f);
				this.DrawIcon(1, 5, iXOffset, 60f);
				this.DrawIcon(2, 7, 0f, 0f);
				this.DrawIcon(2, 0, iXOffset, 60f);
				this.DrawIcon(3, 2, 0f, 0f);
				this.DrawIcon(3, 3, iXOffset, 60f);
				this.mEffect.CurrentTechnique.Passes[0].End();
				this.mEffect.End();
				this.mEffect.Saturation = 1f;
			}

			// Token: 0x0600236E RID: 9070 RVA: 0x000FE1D8 File Offset: 0x000FC3D8
			private void DrawIcon(int iPosition, int iElementIndex, float iXOffset, float iYOffset)
			{
				this.mEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertexBuffer, 0, this.mVertexStride);
				this.mEffect.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
				this.mEffect.SetScreenSize(this.mScreenSize.X, this.mScreenSize.Y);
				this.mEffect.ScaleToHDR = true;
				this.mEffect.Saturation = this.Icons[iElementIndex].Saturation;
				this.mEffect.Color = new Vector4(this.Icons[iElementIndex].Intensity, this.Icons[iElementIndex].Intensity, this.Icons[iElementIndex].Intensity, 1f);
				this.mEffect.TextureEnabled = true;
				this.mEffect.Texture = this.mHudTexture;
				this.mTransform.M41 = 140f + (float)((iPosition - 2) * 50) + iXOffset;
				this.mTransform.M42 = (float)this.mScreenSize.Y - 140f + iYOffset;
				this.mTransform.M41 = this.mTransform.M41 - 5f;
				this.mTransform.M42 = this.mTransform.M42 + 5f;
				this.mEffect.Transform = this.mTransform;
				this.mEffect.CommitChanges();
				this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 54, 2);
				this.mTransform.M41 = this.mTransform.M41 + 5f;
				this.mTransform.M42 = this.mTransform.M42 - 5f;
				this.mEffect.Transform = this.mTransform;
				this.mEffect.CommitChanges();
				this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, iElementIndex * 6, 2);
				this.mTransform.M41 = this.mTransform.M41 - 12f;
				this.mTransform.M42 = this.mTransform.M42 - 12f;
				this.mEffect.Transform = this.mTransform;
				this.mEffect.CommitChanges();
				this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 48, 2);
				this.mTransform.M41 = this.mTransform.M41 + 14f;
				this.mKeyTexts[iElementIndex].Draw(this.mEffect, ref this.mTransform);
			}

			// Token: 0x17000871 RID: 2161
			// (get) Token: 0x0600236F RID: 9071 RVA: 0x000FE456 File Offset: 0x000FC656
			public int ZIndex
			{
				get
				{
					return 600;
				}
			}

			// Token: 0x04002670 RID: 9840
			private GUIBasicEffect mEffect;

			// Token: 0x04002671 RID: 9841
			private Texture2D mHudTexture;

			// Token: 0x04002672 RID: 9842
			private VertexBuffer mVertexBuffer;

			// Token: 0x04002673 RID: 9843
			private VertexDeclaration mVertexDeclaration;

			// Token: 0x04002674 RID: 9844
			private int mVertexStride;

			// Token: 0x04002675 RID: 9845
			public KeyboardHUD.Icon[] Icons;

			// Token: 0x04002676 RID: 9846
			private Text[] mKeyTexts;

			// Token: 0x04002677 RID: 9847
			private Point mScreenSize;

			// Token: 0x04002678 RID: 9848
			private Matrix mTransform;
		}

		// Token: 0x02000491 RID: 1169
		public struct Icon
		{
			// Token: 0x04002679 RID: 9849
			public bool Enabled;

			// Token: 0x0400267A RID: 9850
			public float Saturation;

			// Token: 0x0400267B RID: 9851
			public float Cooldown;

			// Token: 0x0400267C RID: 9852
			public float ResetTimer;

			// Token: 0x0400267D RID: 9853
			public float Intensity;
		}
	}
}
