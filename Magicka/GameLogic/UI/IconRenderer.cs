using System;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Gamers;
using Magicka.Graphics;
using Magicka.Graphics.Effects;
using Magicka.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;

namespace Magicka.GameLogic.UI
{
	// Token: 0x020000C9 RID: 201
	public class IconRenderer
	{
		// Token: 0x0600062E RID: 1582 RVA: 0x00023224 File Offset: 0x00021424
		public IconRenderer(Player iPlayer, PlayState iPlayState)
		{
			this.mPlayState = iPlayState;
			this.mPlayer = new WeakReference(iPlayer);
			this.mMaxIcons = 25;
			this.mMaxIconCount = 5;
			lock (Game.Instance.GraphicsDevice)
			{
				IconRenderer.mHUDTexture = Game.Instance.Content.Load<Texture2D>("UI/Hud/Hud");
			}
			IconRenderer.VertexPositionTextureIndex[] array = new IconRenderer.VertexPositionTextureIndex[6 + 6 * this.mMaxIcons + 6 * IconRenderer.sMaxMagickIconCount];
			IconRenderer.VertexPositionTextureIndex vertexPositionTextureIndex;
			vertexPositionTextureIndex.Index = 0f;
			Vector2 vector = new Vector2(8f, 16f);
			Vector2 vector2 = new Vector2(vector.X / (float)IconRenderer.mHUDTexture.Width, vector.Y / (float)IconRenderer.mHUDTexture.Height);
			Vector2 vector3 = new Vector2(248f / (float)IconRenderer.mHUDTexture.Width, 8f / (float)IconRenderer.mHUDTexture.Height);
			vertexPositionTextureIndex.Position = new Vector2(vector.X * -0.5f, vector.Y * -0.5f);
			vertexPositionTextureIndex.TexCoord = new Vector2(vector3.X, vector3.Y);
			array[0] = vertexPositionTextureIndex;
			vertexPositionTextureIndex.Position = new Vector2(vector.X * 0.5f, vector.Y * -0.5f);
			vertexPositionTextureIndex.TexCoord = new Vector2(vector2.X + vector3.X, vector3.Y);
			array[1] = vertexPositionTextureIndex;
			vertexPositionTextureIndex.Position = new Vector2(vector.X * -0.5f, vector.Y * 0.5f);
			vertexPositionTextureIndex.TexCoord = new Vector2(vector3.X, vector2.Y + vector3.Y);
			array[2] = vertexPositionTextureIndex;
			vertexPositionTextureIndex.Position = new Vector2(vector.X * 0.5f, vector.Y * -0.5f);
			vertexPositionTextureIndex.TexCoord = new Vector2(vector2.X + vector3.X, vector3.Y);
			array[3] = vertexPositionTextureIndex;
			vertexPositionTextureIndex.Position = new Vector2(vector.X * 0.5f, vector.Y * 0.5f);
			vertexPositionTextureIndex.TexCoord = new Vector2(vector2.X + vector3.X, vector2.Y + vector3.Y);
			array[4] = vertexPositionTextureIndex;
			vertexPositionTextureIndex.Position = new Vector2(vector.X * -0.5f, vector.Y * 0.5f);
			vertexPositionTextureIndex.TexCoord = new Vector2(vector3.X, vector2.Y + vector3.Y);
			array[5] = vertexPositionTextureIndex;
			vector3 = new Vector2(0f, 156f / (float)IconRenderer.mHUDTexture.Height);
			this.mIconSizeUV = new Vector2(50f / (float)IconRenderer.mHUDTexture.Width, 49f / (float)IconRenderer.mHUDTexture.Height);
			for (int i = 0; i < this.mMaxIcons + IconRenderer.sMaxMagickIconCount; i++)
			{
				vertexPositionTextureIndex.Index = (float)i;
				vertexPositionTextureIndex.Position = new Vector2(-12.5f, -12.5f);
				vertexPositionTextureIndex.TexCoord = new Vector2(0f, vector3.Y);
				array[6 + i * 6] = vertexPositionTextureIndex;
				vertexPositionTextureIndex.Position = new Vector2(12.5f, -12.5f);
				vertexPositionTextureIndex.TexCoord = new Vector2(this.mIconSizeUV.X, vector3.Y);
				array[7 + i * 6] = vertexPositionTextureIndex;
				vertexPositionTextureIndex.Position = new Vector2(-12.5f, 12.5f);
				vertexPositionTextureIndex.TexCoord = new Vector2(0f, vector3.Y + this.mIconSizeUV.Y);
				array[8 + i * 6] = vertexPositionTextureIndex;
				vertexPositionTextureIndex.Position = new Vector2(12.5f, -12.5f);
				vertexPositionTextureIndex.TexCoord = new Vector2(this.mIconSizeUV.X, vector3.Y);
				array[9 + i * 6] = vertexPositionTextureIndex;
				vertexPositionTextureIndex.Position = new Vector2(12.5f, 12.5f);
				vertexPositionTextureIndex.TexCoord = new Vector2(this.mIconSizeUV.X, vector3.Y + this.mIconSizeUV.Y);
				array[10 + i * 6] = vertexPositionTextureIndex;
				vertexPositionTextureIndex.Position = new Vector2(-12.5f, 12.5f);
				vertexPositionTextureIndex.TexCoord = new Vector2(0f, vector3.Y + this.mIconSizeUV.Y);
				array[11 + i * 6] = vertexPositionTextureIndex;
			}
			this.mBasicEffect = new GUIBasicEffect(Game.Instance.GraphicsDevice, null);
			this.mBasicEffect.Texture = IconRenderer.mHUDTexture;
			this.mBasicEffect.Color = new Vector4(1f);
			this.mBasicEffect.TextureEnabled = true;
			this.mInstancingEffect = new GUIHardwareInstancingEffect(Game.Instance.GraphicsDevice, Game.Instance.Content);
			this.mInstancingEffect.Texture = IconRenderer.mHUDTexture;
			Point screenSize = RenderManager.Instance.ScreenSize;
			this.mInstancingEffect.SetScreenSize(screenSize.X, screenSize.Y);
			this.mBasicEffect.SetScreenSize(screenSize.X, screenSize.Y);
			this.mIcons = new IconRenderer.Icon[this.mMaxIcons + IconRenderer.sMaxMagickIconCount];
			this.mCombineIcons = new IconRenderer.Icon[this.mMaxIcons];
			lock (Game.Instance.GraphicsDevice)
			{
				this.mVertexBuffer = new VertexBuffer(Game.Instance.GraphicsDevice, (6 + 6 * this.mMaxIcons + 6 * IconRenderer.sMaxMagickIconCount) * 20, BufferUsage.WriteOnly);
				this.mVertexBuffer.Name = "IconRendererVB";
				this.mVertexBuffer.SetData<IconRenderer.VertexPositionTextureIndex>(array);
				this.mVertexDeclaration = new VertexDeclaration(Game.Instance.GraphicsDevice, IconRenderer.VertexPositionTextureIndex.VertexElements);
			}
			this.mInstancingEffect.SetTechnique(GUIHardwareInstancingEffect.Technique.Sprites);
			this.mRenderData = new IconRenderer.RenderData[3];
			for (int j = 0; j < 3; j++)
			{
				IconRenderer.RenderData renderData = new IconRenderer.RenderData(this.mMaxIcons + IconRenderer.sMaxMagickIconCount);
				this.mRenderData[j] = renderData;
				renderData.mVertexDeclaration = this.mVertexDeclaration;
				renderData.mVertexBuffer = this.mVertexBuffer;
				renderData.mInstancingEffect = this.mInstancingEffect;
				renderData.mBasicEffect = this.mBasicEffect;
			}
		}

		// Token: 0x0600062F RID: 1583 RVA: 0x00023934 File Offset: 0x00021B34
		public void Initialize(PlayState iPlayState)
		{
			this.mPlayState = iPlayState;
			this.TomeMagick = MagickType.None;
		}

		// Token: 0x06000630 RID: 1584 RVA: 0x00023944 File Offset: 0x00021B44
		public void SetCapacity(int iCapacity)
		{
			this.mMaxIconCount = iCapacity;
		}

		// Token: 0x06000631 RID: 1585 RVA: 0x00023950 File Offset: 0x00021B50
		public void AddIcon(Elements iElement, bool iFromSpellWheel)
		{
			if (this.mIconCount > 5 || GlobalSettings.Instance.SpellWheel == SettingOptions.Off)
			{
				return;
			}
			IconRenderer.Icon[] array = this.mIcons;
			IconRenderer.Icon icon = default(IconRenderer.Icon);
			icon.Color.X = 1f;
			icon.Color.Y = 1f;
			icon.Color.Z = 1f;
			icon.Color.W = 0f;
			int num = this.mIconCount;
			int num2 = this.mIconCount;
			int num3 = Defines.ElementIndex(iElement);
			for (int i = 0; i < num; i++)
			{
				if (array[i].Removed || array[i].Index < 0)
				{
					num2--;
				}
			}
			if (num2 >= this.mMaxIconCount)
			{
				icon.Index = -1;
			}
			else
			{
				icon.Index = 4;
				for (int j = 0; j < array.Length; j++)
				{
					array[j].Index = Math.Min(Math.Max(array[j].Index - 1, 5 - this.mMaxIconCount), 4);
				}
			}
			icon.State = (IconRenderer.Icon.AnimationState.Grow | IconRenderer.Icon.AnimationState.Glow);
			icon.Element = iElement;
			icon.TextureOffset.X = (float)(num3 % 5) * this.mIconSizeUV.X;
			icon.TextureOffset.Y = (float)(num3 / 5) * this.mIconSizeUV.Y;
			if (iFromSpellWheel)
			{
				icon.Position.X = (float)Math.Cos((double)Defines.ElementUIRadian[num3]) * 64f;
				icon.Position.Y = icon.Position.Y - (float)Math.Sin((double)Defines.ElementUIRadian[num3]) * 64f;
			}
			else if (this.mIconCount <= 4)
			{
				icon.Position = IconRenderer.sIconTargets[this.mIconCount];
			}
			else
			{
				icon.Position = default(Vector2);
			}
			array[num] = icon;
			this.mIconCount++;
		}

		// Token: 0x06000632 RID: 1586 RVA: 0x00023B54 File Offset: 0x00021D54
		public void TransformIconForCombine(Elements iResultingElement, int iSourceIndex, int iTargetIndex)
		{
			if (GlobalSettings.Instance.SpellWheel == SettingOptions.Off)
			{
				return;
			}
			IconRenderer.Icon[] array = this.mIcons;
			for (int i = 0; i <= iSourceIndex; i++)
			{
				if (i < array.Length && (array[i].Removed || (i < iSourceIndex && array[i].Index < 0)))
				{
					iSourceIndex++;
				}
			}
			for (int j = 0; j <= iTargetIndex; j++)
			{
				if (j < array.Length && (array[j].Removed || (j < iSourceIndex && array[j].Index < 0)))
				{
					iTargetIndex++;
				}
			}
			if (iSourceIndex >= this.mIconCount)
			{
				iSourceIndex = this.mIconCount - 1;
			}
			IconRenderer.Icon icon = array[iSourceIndex];
			IconRenderer.Icon.RemoveItem(array, iSourceIndex);
			this.mIconCount--;
			if (icon.Index >= 0)
			{
				for (int k = 0; k < iSourceIndex; k++)
				{
					array[k].Index = Math.Min(Math.Max(array[k].Index + 1, 5 - this.mMaxIconCount), 4);
				}
			}
			if (iTargetIndex < 0)
			{
				throw new Exception();
			}
			icon.Index = iTargetIndex;
			icon.Element = iResultingElement;
			int num = this.mCombineIconCount;
			this.mCombineIcons[num] = icon;
			this.mCombineIconCount++;
		}

		// Token: 0x06000633 RID: 1587 RVA: 0x00023CA8 File Offset: 0x00021EA8
		public void TransformIconForRemoval(int iSourceIndex, int iTargetIndex)
		{
			if (GlobalSettings.Instance.SpellWheel == SettingOptions.Off)
			{
				return;
			}
			IconRenderer.Icon[] array = this.mIcons;
			for (int i = 0; i <= iSourceIndex; i++)
			{
				if (i < array.Length && (array[i].Removed || (i < iSourceIndex && array[i].Index < 0)))
				{
					iSourceIndex++;
				}
			}
			for (int j = 0; j <= iTargetIndex; j++)
			{
				if (j < array.Length && (array[j].Removed || (j < iSourceIndex && array[j].Index < 0)))
				{
					iTargetIndex++;
				}
			}
			if (iSourceIndex >= array.Length)
			{
				return;
			}
			IconRenderer.Icon icon = array[iSourceIndex];
			IconRenderer.Icon.RemoveItem(array, iSourceIndex);
			this.mIconCount--;
			if (icon.Index >= 0)
			{
				for (int k = 0; k < iSourceIndex; k++)
				{
					array[k].Index = Math.Min(Math.Max(array[k].Index + 1, 5 - this.mMaxIconCount), 4);
				}
			}
			icon.Index = iTargetIndex;
			array[iTargetIndex].Removed = true;
			icon.Element = Elements.None;
			int num = this.mCombineIconCount;
			this.mCombineIcons[num] = icon;
			this.mCombineIconCount++;
		}

		// Token: 0x06000634 RID: 1588 RVA: 0x00023DF4 File Offset: 0x00021FF4
		public void Clear()
		{
			IconRenderer.Icon[] array = this.mIcons;
			int num = this.mIconCount;
			for (int i = 0; i < num; i++)
			{
				array[i].State = (IconRenderer.Icon.AnimationState.Shrink | IconRenderer.Icon.AnimationState.Fade);
				array[i].Removed = true;
			}
			array = this.mCombineIcons;
			num = this.mCombineIconCount;
			for (int j = 0; j < num; j++)
			{
				array[j].State = (IconRenderer.Icon.AnimationState.Shrink | IconRenderer.Icon.AnimationState.Fade);
				array[j].Removed = true;
			}
		}

		// Token: 0x06000635 RID: 1589 RVA: 0x00023E6C File Offset: 0x0002206C
		public void ClearElements(Elements iElements)
		{
			for (int i = 0; i < this.mIcons.Length; i++)
			{
				if ((this.mIcons[i].Element & iElements) != Elements.None)
				{
					this.mIcons[i].State = (IconRenderer.Icon.AnimationState.Shrink | IconRenderer.Icon.AnimationState.Fade);
					this.mIcons[i].Removed = true;
				}
			}
		}

		// Token: 0x06000636 RID: 1590 RVA: 0x00023EC8 File Offset: 0x000220C8
		public void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			Vector2 vector = new Vector2(1f);
			Vector2 vector2 = new Vector2(1.5f);
			Vector2 vector3 = default(Vector2);
			Vector4 vector4 = new Vector4(6f, 6f, 6f, 1f);
			Vector4 vector5 = new Vector4(1f);
			Vector4 vector6 = new Vector4(1f, 1f, 1f, 0f);
			float amount = MathHelper.Clamp(1f - (float)Math.Pow(0.0025, (double)iDeltaTime), 0f, 1f);
			int num = this.mIconCount;
			int num2 = this.mCombineIconCount;
			IconRenderer.Icon[] array = this.mIcons;
			IconRenderer.Icon[] array2 = this.mCombineIcons;
			int i = 0;
			while (i < num2)
			{
				IconRenderer.Icon icon = array2[i];
				if ((byte)(icon.State & IconRenderer.Icon.AnimationState.Glow) == 2)
				{
					Vector4.Lerp(ref icon.Color, ref vector4, amount, out array2[i].Color);
					goto IL_196;
				}
				if ((byte)(icon.State & IconRenderer.Icon.AnimationState.Fade) != 8)
				{
					Vector4.Lerp(ref icon.Color, ref vector5, amount, out array2[i].Color);
					goto IL_196;
				}
				Vector4.Lerp(ref icon.Color, ref vector6, amount, out array2[i].Color);
				float num3;
				Vector4.DistanceSquared(ref array2[i].Color, ref vector6, out num3);
				if (num3 >= 0.1f)
				{
					goto IL_196;
				}
				IconRenderer.Icon.RemoveItem(array2, i);
				i--;
				num2--;
				this.mCombineIconCount = num2;
				IL_423:
				i++;
				continue;
				IL_196:
				if ((byte)(icon.State & (IconRenderer.Icon.AnimationState.Grow | IconRenderer.Icon.AnimationState.Glow)) == 2)
				{
					if (icon.Element == Elements.None)
					{
						array[icon.Index].State = (IconRenderer.Icon.AnimationState.Shrink | IconRenderer.Icon.AnimationState.Fade);
					}
					else
					{
						array[icon.Index].Element = icon.Element;
						array[icon.Index].TextureOffset.X = (float)(Defines.ElementIndex(icon.Element) % 5) * this.mIconSizeUV.X;
						array[icon.Index].TextureOffset.Y = (float)(Defines.ElementIndex(icon.Element) / 5) * this.mIconSizeUV.Y;
						Vector4.DistanceSquared(ref array2[i].Color, ref vector4, out num3);
						if (num3 < 0.1f)
						{
							array2[i].State = IconRenderer.Icon.AnimationState.Fade;
						}
					}
				}
				if ((byte)(icon.State & IconRenderer.Icon.AnimationState.Grow) == 1)
				{
					Vector2.Lerp(ref icon.Scale, ref vector2, amount, out array2[i].Scale);
					Vector2.DistanceSquared(ref array2[i].Scale, ref vector2, out num3);
					if (num3 < 0.1f)
					{
						IconRenderer.Icon[] array3 = array2;
						int num4 = i;
						array3[num4].State = (array3[num4].State & ~(IconRenderer.Icon.AnimationState.Grow | IconRenderer.Icon.AnimationState.Glow));
						goto IL_423;
					}
					goto IL_423;
				}
				else if ((byte)(icon.State & IconRenderer.Icon.AnimationState.Shrink) == 4)
				{
					Vector2.Lerp(ref icon.Scale, ref vector3, amount, out array2[i].Scale);
					Vector2.DistanceSquared(ref array2[i].Scale, ref vector3, out num3);
					if (num3 < 0.1f)
					{
						IconRenderer.Icon.RemoveItem(array2, i);
						i--;
						num2--;
						this.mCombineIconCount = num2;
						goto IL_423;
					}
					goto IL_423;
				}
				else
				{
					if (icon.Index < 0)
					{
						array[i].State = (IconRenderer.Icon.AnimationState.Shrink | IconRenderer.Icon.AnimationState.Fade);
						goto IL_423;
					}
					Vector2 position = array[icon.Index].Position;
					Vector2.Lerp(ref icon.Position, ref position, amount, out array2[i].Position);
					Vector2.Lerp(ref icon.Scale, ref vector, amount, out array2[i].Scale);
					if ((byte)(array2[i].State & IconRenderer.Icon.AnimationState.Fade) == 8)
					{
						goto IL_423;
					}
					Vector2.DistanceSquared(ref array2[i].Position, ref position, out num3);
					if (num3 >= 0.75f)
					{
						goto IL_423;
					}
					if (icon.Element == Elements.None)
					{
						array2[i].State = (IconRenderer.Icon.AnimationState.Glow | IconRenderer.Icon.AnimationState.Shrink);
						goto IL_423;
					}
					array2[i].State = IconRenderer.Icon.AnimationState.Glow;
					goto IL_423;
				}
			}
			for (int j = 0; j < num; j++)
			{
				IconRenderer.Icon icon2 = array[j];
				if ((byte)(icon2.State & IconRenderer.Icon.AnimationState.Glow) == 2)
				{
					Vector4.Lerp(ref icon2.Color, ref vector4, amount, out array[j].Color);
				}
				else if ((byte)(icon2.State & IconRenderer.Icon.AnimationState.Fade) == 8)
				{
					Vector4.Lerp(ref icon2.Color, ref vector6, amount, out array[j].Color);
				}
				else
				{
					Vector4.Lerp(ref icon2.Color, ref vector5, amount, out array[j].Color);
				}
				if ((byte)(icon2.State & IconRenderer.Icon.AnimationState.Grow) == 1)
				{
					Vector2.Lerp(ref icon2.Scale, ref vector2, amount, out array[j].Scale);
					float num3;
					Vector2.DistanceSquared(ref array[j].Scale, ref vector2, out num3);
					if (num3 < 0.1f)
					{
						IconRenderer.Icon[] array4 = array;
						int num5 = j;
						array4[num5].State = (array4[num5].State & ~(IconRenderer.Icon.AnimationState.Grow | IconRenderer.Icon.AnimationState.Glow));
					}
				}
				else if ((byte)(icon2.State & IconRenderer.Icon.AnimationState.Shrink) == 4)
				{
					Vector2.Lerp(ref icon2.Scale, ref vector3, amount, out array[j].Scale);
					float num3;
					Vector2.DistanceSquared(ref array[j].Scale, ref vector3, out num3);
					if (num3 < 0.1f)
					{
						IconRenderer.Icon.RemoveItem(array, j);
						if (icon2.Index >= 0)
						{
							for (int k = 0; k < j; k++)
							{
								array[k].Index = Math.Min(Math.Max(array[k].Index + 1, 5 - this.mMaxIconCount), 4);
							}
						}
						for (int l = 0; l < num2; l++)
						{
							int index = array2[l].Index;
							if (index == j)
							{
								IconRenderer.Icon.RemoveItem(array2, l);
								l--;
								num2--;
								this.mCombineIconCount = num2;
							}
							else if (index > j)
							{
								array2[l].Index = Math.Min(Math.Max(index + 1, 5 - this.mMaxIconCount), 4);
							}
						}
						j--;
						num--;
						this.mIconCount = num;
					}
				}
				else if (icon2.Index < 0)
				{
					array[j].State = (IconRenderer.Icon.AnimationState.Shrink | IconRenderer.Icon.AnimationState.Fade);
				}
				else
				{
					int num6 = j;
					for (int m = 0; m < num6; m++)
					{
						if (array[m].Removed)
						{
							num6--;
						}
					}
					if (num6 < IconRenderer.sIconTargets.Length)
					{
						Vector2.Lerp(ref icon2.Position, ref IconRenderer.sIconTargets[num6], amount, out array[j].Position);
					}
					Vector2.Lerp(ref icon2.Scale, ref vector, amount, out array[j].Scale);
				}
			}
			IconRenderer.RenderData renderData = this.mRenderData[(int)iDataChannel];
			int num7 = 0;
			if (this.mIconCount < 0)
			{
				this.mIconCount = 0;
			}
			IconRenderer.Icon.CopyToArrays(this.mIcons, 0, num7, this.mIconCount, renderData.mIconPositions, renderData.mScales, renderData.mTextureOffsets, renderData.mColors, renderData.mSaturations);
			num7 += this.mIconCount;
			IconRenderer.Icon.CopyToArrays(this.mCombineIcons, 0, num7, this.mCombineIconCount, renderData.mIconPositions, renderData.mScales, renderData.mTextureOffsets, renderData.mColors, renderData.mSaturations);
			num7 += this.mCombineIconCount;
			Avatar avatar = this.Player.Avatar;
			if (avatar != null)
			{
				renderData.mWorldPosition = avatar.Position;
				IconRenderer.RenderData renderData2 = renderData;
				renderData2.mWorldPosition.Y = renderData2.mWorldPosition.Y - (avatar.Capsule.Length * 0.5f + avatar.Capsule.Radius);
			}
			renderData.mCombiningToRender = this.mCombineIconCount;
			renderData.mElementsToRender = num7;
			if (this.mMagickDirty)
			{
				if (this.mMagickTextAlpha > 0f)
				{
					this.mMagickTextAlpha = Math.Max(0f, this.mMagickTextAlpha - iDeltaTime * 4f);
				}
				else
				{
					this.mMagickDirty = false;
					this.mTomeMagickElements = SpellManager.Instance.GetMagickCombo(this.mDisplayedMagick);
					if (this.mTomeMagickElements != null)
					{
						for (int n = 0; n < 3; n++)
						{
							this.mRenderData[n].SetTomeMagick(this.mDisplayedMagick, this.mTomeMagickElements.Length);
						}
					}
					else
					{
						for (int num8 = 0; num8 < 3; num8++)
						{
							this.mRenderData[num8].SetTomeMagick(this.mDisplayedMagick, 0);
						}
					}
				}
			}
			else
			{
				this.mMagickTextAlpha = Math.Min(1f, this.mMagickTextAlpha + iDeltaTime * 4f);
			}
			renderData.mMagickAlpha = this.mMagickTextAlpha;
			if (this.mTomeMagickElements != null)
			{
				int num9 = this.mMaxIcons;
				int num10 = 0;
				while (num9 < this.mMaxIcons + this.mTomeMagickElements.Length)
				{
					int num11 = Defines.ElementIndex(this.mTomeMagickElements[num10]);
					this.mIcons[num9].Element = this.mTomeMagickElements[num10];
					this.mIcons[num9].Saturation = 0f;
					this.mIcons[num9].Color = new Vector4(1f, 1f, 1f, 0.5f * this.mMagickTextAlpha);
					this.mIcons[num9].Scale = new Vector2(1f, 1f);
					this.mIcons[num9].TextureOffset.X = (float)(num11 % 5) * this.mIconSizeUV.X;
					this.mIcons[num9].TextureOffset.Y = (float)(num11 / 5) * this.mIconSizeUV.Y;
					this.mIcons[num9].Position.X = IconRenderer.sIconTargets[num10].X;
					this.mIcons[num9].Position.Y = IconRenderer.sIconTargets[num10].Y;
					num9++;
					num10++;
				}
				IconRenderer.Icon.CopyToArrays(this.mIcons, this.mMaxIcons, this.mMaxIcons, 5, renderData.mIconPositions, renderData.mScales, renderData.mTextureOffsets, renderData.mColors, renderData.mSaturations);
			}
			if (KeyboardHUD.Instance.UIEnabled)
			{
				GameStateManager.Instance.CurrentState.Scene.AddRenderableGUIObject(iDataChannel, renderData);
			}
		}

		// Token: 0x17000131 RID: 305
		// (get) Token: 0x06000637 RID: 1591 RVA: 0x00024994 File Offset: 0x00022B94
		protected Player Player
		{
			get
			{
				return this.mPlayer.Target as Player;
			}
		}

		// Token: 0x17000132 RID: 306
		// (get) Token: 0x06000638 RID: 1592 RVA: 0x000249A6 File Offset: 0x00022BA6
		// (set) Token: 0x06000639 RID: 1593 RVA: 0x000249B0 File Offset: 0x00022BB0
		public MagickType TomeMagick
		{
			get
			{
				return this.mDisplayedMagick;
			}
			set
			{
				if (this.Player.Gamer is NetworkGamer)
				{
					return;
				}
				MagickType magickType = value;
				while (magickType != MagickType.None && !SpellManager.Instance.IsMagickAllowed(this.Player, this.mPlayState.GameType, magickType))
				{
					int num = (int)(magickType + 1);
					magickType = (MagickType)((num >= 35) ? 0 : num);
					if (magickType == value)
					{
						magickType = this.mDisplayedMagick;
					}
				}
				if (magickType != this.mDisplayedMagick)
				{
					this.mMagickDirty = true;
					TutorialManager.Instance.SetTip(TutorialManager.Tips.MagicksScroll, TutorialManager.Position.Top);
				}
				this.mDisplayedMagick = magickType;
			}
		}

		// Token: 0x040004C6 RID: 1222
		private static Texture2D mHUDTexture;

		// Token: 0x040004C7 RID: 1223
		public static readonly int sMaxMagickIconCount = 5;

		// Token: 0x040004C8 RID: 1224
		private static readonly Vector2[] sIconTargets = new Vector2[]
		{
			new Vector2(-50f, 44f),
			new Vector2(-25f, 44f),
			new Vector2(0f, 44f),
			new Vector2(25f, 44f),
			new Vector2(50f, 44f)
		};

		// Token: 0x040004C9 RID: 1225
		private MagickType mDisplayedMagick;

		// Token: 0x040004CA RID: 1226
		private bool mMagickDirty;

		// Token: 0x040004CB RID: 1227
		private float mMagickTextAlpha;

		// Token: 0x040004CC RID: 1228
		private Elements[] mTomeMagickElements;

		// Token: 0x040004CD RID: 1229
		private IconRenderer.RenderData[] mRenderData;

		// Token: 0x040004CE RID: 1230
		private VertexBuffer mVertexBuffer;

		// Token: 0x040004CF RID: 1231
		private VertexDeclaration mVertexDeclaration;

		// Token: 0x040004D0 RID: 1232
		private GUIHardwareInstancingEffect mInstancingEffect;

		// Token: 0x040004D1 RID: 1233
		private GUIBasicEffect mBasicEffect;

		// Token: 0x040004D2 RID: 1234
		private int mMaxIconCount;

		// Token: 0x040004D3 RID: 1235
		private int mCombineIconCount;

		// Token: 0x040004D4 RID: 1236
		private int mIconCount;

		// Token: 0x040004D5 RID: 1237
		private IconRenderer.Icon[] mCombineIcons;

		// Token: 0x040004D6 RID: 1238
		private IconRenderer.Icon[] mIcons;

		// Token: 0x040004D7 RID: 1239
		private Vector2 mIconSizeUV;

		// Token: 0x040004D8 RID: 1240
		private int mMaxIcons;

		// Token: 0x040004D9 RID: 1241
		private PlayState mPlayState;

		// Token: 0x040004DA RID: 1242
		private WeakReference mPlayer;

		// Token: 0x020000CA RID: 202
		private class RenderData : IRenderableGUIObject, IPreRenderRenderer
		{
			// Token: 0x0600063B RID: 1595 RVA: 0x00024ADC File Offset: 0x00022CDC
			public RenderData(int iSize)
			{
				this.mIconPositions = new Vector2[iSize];
				this.mScales = new Vector3[iSize];
				this.mTextureOffsets = new Vector2[iSize];
				this.mColors = new Vector4[iSize];
				this.mSaturations = new float[iSize];
				this.mMagickAlpha = 0f;
				this.mMagickText = new Text(64, IconRenderer.RenderData.sFont, TextAlign.Center, false);
				this.mMagickText.SetText("");
			}

			// Token: 0x0600063C RID: 1596 RVA: 0x00024B5C File Offset: 0x00022D5C
			public void SetTomeMagick(MagickType iType, int iElements)
			{
				this.mDirtyMagickElementsToRender = iElements;
				if (iType == MagickType.None)
				{
					this.mMagickText.SetText("");
					return;
				}
				string @string = LanguageManager.Instance.GetString(Magick.NAME_LOCALIZATION[(int)iType]);
				this.mMagickText.SetText(@string);
			}

			// Token: 0x0600063D RID: 1597 RVA: 0x00024BA4 File Offset: 0x00022DA4
			public void Draw(float iDeltaTime)
			{
				if (this.mMagickElementsToRender != this.mDirtyMagickElementsToRender)
				{
					this.mMagickElementsToRender = this.mDirtyMagickElementsToRender;
				}
				if (this.mElementsToRender == 0 && this.mMagickElementsToRender == 0)
				{
					return;
				}
				Point screenSize = RenderManager.Instance.ScreenSize;
				Matrix identity = Matrix.Identity;
				this.mBasicEffect.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
				this.mBasicEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertexBuffer, 0, 20);
				this.mBasicEffect.Texture = IconRenderer.mHUDTexture;
				this.mBasicEffect.SetTechnique(GUIBasicEffect.Technique.Texture2D);
				this.mBasicEffect.TextureOffset = default(Vector2);
				this.mBasicEffect.GraphicsDevice.RenderState.CullMode = CullMode.None;
				this.mBasicEffect.SetScreenSize(screenSize.X, screenSize.Y);
				identity.M41 = (float)Math.Floor((double)(this.mPosition.X - 62.5f));
				identity.M42 = this.mPosition.Y + 44f;
				this.mBasicEffect.Transform = identity;
				this.mBasicEffect.Begin();
				this.mBasicEffect.CurrentTechnique.Passes[0].Begin();
				this.mBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, 2);
				identity.M41 = (float)Math.Floor((double)(this.mPosition.X + 62.5f));
				identity.M11 *= -1f;
				this.mBasicEffect.Transform = identity;
				this.mBasicEffect.CommitChanges();
				this.mBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, 2);
				this.mBasicEffect.GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;
				this.mBasicEffect.Color = new Vector4(1f, 1f, 1f, this.mMagickAlpha);
				this.mBasicEffect.CommitChanges();
				this.mMagickText.Draw(this.mBasicEffect, this.mPosition.X, this.mPosition.Y + 44f + 10f);
				this.mBasicEffect.CurrentTechnique.Passes[0].End();
				this.mBasicEffect.End();
				this.mInstancingEffect.SetScreenSize(screenSize.X, screenSize.Y);
				this.mInstancingEffect.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
				this.mInstancingEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertexBuffer, 0, 20);
				this.mInstancingEffect.GraphicsDevice.RenderState.DestinationBlend = Blend.InverseSourceAlpha;
				this.mInstancingEffect.Scales = this.mScales;
				this.mInstancingEffect.TextureOffsets = this.mTextureOffsets;
				this.mInstancingEffect.Colors = this.mColors;
				this.mInstancingEffect.Saturations = this.mSaturations;
				if (this.mElementsToRender > 0 && this.mMagickElementsToRender > 0)
				{
					for (int i = 0; i < this.mElementsToRender; i++)
					{
						Vector2[] array = this.mIconPositions;
						int num = i;
						array[num].X = array[num].X + this.mPosition.X;
						Vector2[] array2 = this.mIconPositions;
						int num2 = i;
						array2[num2].Y = array2[num2].Y + this.mPosition.Y;
					}
					for (int j = 0; j < Math.Min(this.mElementsToRender - this.mCombiningToRender, 5); j++)
					{
						Vector2[] array3 = this.mIconPositions;
						int num3 = 25 + j;
						array3[num3].X = array3[num3].X + 2000f;
						Vector2[] array4 = this.mIconPositions;
						int num4 = 25 + j;
						array4[num4].Y = array4[num4].Y + 2000f;
					}
					for (int k = this.mElementsToRender - this.mCombiningToRender; k < this.mMagickElementsToRender; k++)
					{
						Vector2[] array5 = this.mIconPositions;
						int num5 = 25 + k;
						array5[num5].X = array5[num5].X + this.mPosition.X;
						Vector2[] array6 = this.mIconPositions;
						int num6 = 25 + k;
						array6[num6].Y = array6[num6].Y + this.mPosition.Y;
					}
					this.mInstancingEffect.Positions = this.mIconPositions;
					int count = this.mInstancingEffect.CurrentTechnique.Passes.Count;
					this.mInstancingEffect.Begin();
					for (int l = 0; l < count; l++)
					{
						EffectPass effectPass = this.mInstancingEffect.CurrentTechnique.Passes[l];
						effectPass.Begin();
						this.mInstancingEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 6, 2 * this.mElementsToRender);
						this.mInstancingEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 156, 2 * this.mMagickElementsToRender);
						effectPass.End();
					}
					this.mInstancingEffect.End();
					return;
				}
				if (this.mMagickElementsToRender > 0)
				{
					for (int m = this.mElementsToRender; m < this.mMagickElementsToRender; m++)
					{
						Vector2[] array7 = this.mIconPositions;
						int num7 = 25 + m;
						array7[num7].X = array7[num7].X + this.mPosition.X;
						Vector2[] array8 = this.mIconPositions;
						int num8 = 25 + m;
						array8[num8].Y = array8[num8].Y + this.mPosition.Y;
					}
					this.mInstancingEffect.Positions = this.mIconPositions;
					int count2 = this.mInstancingEffect.CurrentTechnique.Passes.Count;
					this.mInstancingEffect.Begin();
					for (int n = 0; n < count2; n++)
					{
						EffectPass effectPass2 = this.mInstancingEffect.CurrentTechnique.Passes[n];
						effectPass2.Begin();
						this.mInstancingEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 156, 2 * this.mMagickElementsToRender);
						effectPass2.End();
					}
					this.mInstancingEffect.End();
					return;
				}
				if (this.mElementsToRender > 0)
				{
					for (int num9 = 0; num9 < this.mElementsToRender; num9++)
					{
						Vector2[] array9 = this.mIconPositions;
						int num10 = num9;
						array9[num10].X = array9[num10].X + this.mPosition.X;
						Vector2[] array10 = this.mIconPositions;
						int num11 = num9;
						array10[num11].Y = array10[num11].Y + this.mPosition.Y;
					}
					this.mInstancingEffect.Positions = this.mIconPositions;
					int count3 = this.mInstancingEffect.CurrentTechnique.Passes.Count;
					this.mInstancingEffect.Begin();
					for (int num12 = 0; num12 < count3; num12++)
					{
						EffectPass effectPass3 = this.mInstancingEffect.CurrentTechnique.Passes[num12];
						effectPass3.Begin();
						this.mInstancingEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 6, 2 * this.mElementsToRender);
						effectPass3.End();
					}
					this.mInstancingEffect.End();
				}
			}

			// Token: 0x17000133 RID: 307
			// (get) Token: 0x0600063E RID: 1598 RVA: 0x00025298 File Offset: 0x00023498
			public int ZIndex
			{
				get
				{
					return 51;
				}
			}

			// Token: 0x0600063F RID: 1599 RVA: 0x0002529C File Offset: 0x0002349C
			public void PreRenderUpdate(DataChannel iDataChannel, float iDeltaTime, ref Matrix iViewProjectionMatrix, ref Vector3 iCameraPosition, ref Vector3 iCameraDirection)
			{
				this.mPosition = MagickaMath.WorldToScreenPosition(ref this.mWorldPosition, ref iViewProjectionMatrix);
			}

			// Token: 0x040004DB RID: 1243
			private const float mBracketXPos = 62.5f;

			// Token: 0x040004DC RID: 1244
			private static readonly BitmapFont sFont = FontManager.Instance.GetFont(MagickaFont.Maiandra14);

			// Token: 0x040004DD RID: 1245
			public GUIBasicEffect mBasicEffect;

			// Token: 0x040004DE RID: 1246
			public GUIHardwareInstancingEffect mInstancingEffect;

			// Token: 0x040004DF RID: 1247
			public VertexDeclaration mVertexDeclaration;

			// Token: 0x040004E0 RID: 1248
			public VertexBuffer mVertexBuffer;

			// Token: 0x040004E1 RID: 1249
			private Vector2 mPosition;

			// Token: 0x040004E2 RID: 1250
			public Vector3 mWorldPosition;

			// Token: 0x040004E3 RID: 1251
			public Vector2[] mIconPositions;

			// Token: 0x040004E4 RID: 1252
			public Vector3[] mScales;

			// Token: 0x040004E5 RID: 1253
			public Vector2[] mTextureOffsets;

			// Token: 0x040004E6 RID: 1254
			public Vector4[] mColors;

			// Token: 0x040004E7 RID: 1255
			public float[] mSaturations;

			// Token: 0x040004E8 RID: 1256
			public int mCombiningToRender;

			// Token: 0x040004E9 RID: 1257
			public int mElementsToRender;

			// Token: 0x040004EA RID: 1258
			private int mMagickElementsToRender;

			// Token: 0x040004EB RID: 1259
			private Text mMagickText;

			// Token: 0x040004EC RID: 1260
			public float mMagickAlpha;

			// Token: 0x040004ED RID: 1261
			private int mDirtyMagickElementsToRender;
		}

		// Token: 0x020000CB RID: 203
		private struct VertexPositionTextureIndex
		{
			// Token: 0x040004EE RID: 1262
			public const int SIZEINBYTES = 20;

			// Token: 0x040004EF RID: 1263
			public Vector2 Position;

			// Token: 0x040004F0 RID: 1264
			public Vector2 TexCoord;

			// Token: 0x040004F1 RID: 1265
			public float Index;

			// Token: 0x040004F2 RID: 1266
			public static readonly VertexElement[] VertexElements = new VertexElement[]
			{
				new VertexElement(0, 0, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.Position, 0),
				new VertexElement(0, 8, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 0),
				new VertexElement(0, 16, VertexElementFormat.Single, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 1)
			};
		}

		// Token: 0x020000CC RID: 204
		private struct Icon
		{
			// Token: 0x06000642 RID: 1602 RVA: 0x00025324 File Offset: 0x00023524
			public static void CopyToArrays(IconRenderer.Icon[] iSource, int iSourceStartIndex, int iTargetStartIndex, int iCount, Vector2[] iTargetPositions, Vector3[] iTargetScales, Vector2[] iTargetTextureOffsets, Vector4[] iTargetColors, float[] iSaturations)
			{
				for (int i = 0; i < iCount; i++)
				{
					IconRenderer.Icon icon = iSource[i + iSourceStartIndex];
					int num = i + iTargetStartIndex;
					iTargetPositions[num] = icon.Position;
					iTargetScales[num] = new Vector3(icon.Scale, 1f);
					iTargetTextureOffsets[num] = icon.TextureOffset;
					iTargetColors[num] = icon.Color;
					iSaturations[num] = icon.Saturation;
				}
			}

			// Token: 0x06000643 RID: 1603 RVA: 0x000253B8 File Offset: 0x000235B8
			public static void RemoveItem(IconRenderer.Icon[] iArray, int iItem)
			{
				for (int i = iItem + 1; i < iArray.Length; i++)
				{
					iArray[i - 1] = iArray[i];
				}
			}

			// Token: 0x040004F3 RID: 1267
			public Vector2 Position;

			// Token: 0x040004F4 RID: 1268
			public Vector2 Scale;

			// Token: 0x040004F5 RID: 1269
			public Vector2 TextureOffset;

			// Token: 0x040004F6 RID: 1270
			public Vector4 Color;

			// Token: 0x040004F7 RID: 1271
			public Elements Element;

			// Token: 0x040004F8 RID: 1272
			public float Saturation;

			// Token: 0x040004F9 RID: 1273
			public int Index;

			// Token: 0x040004FA RID: 1274
			public IconRenderer.Icon.AnimationState State;

			// Token: 0x040004FB RID: 1275
			public bool Removed;

			// Token: 0x020000CD RID: 205
			[Flags]
			public enum AnimationState : byte
			{
				// Token: 0x040004FD RID: 1277
				None = 0,
				// Token: 0x040004FE RID: 1278
				Grow = 1,
				// Token: 0x040004FF RID: 1279
				Glow = 2,
				// Token: 0x04000500 RID: 1280
				Shrink = 4,
				// Token: 0x04000501 RID: 1281
				Fade = 8
			}
		}
	}
}
