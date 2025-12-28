using System;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;

namespace Magicka.GameLogic.UI
{
	// Token: 0x020000C6 RID: 198
	public class SpellWheel
	{
		// Token: 0x0600061D RID: 1565 RVA: 0x00021E10 File Offset: 0x00020010
		public SpellWheel(Player iPlayer, PlayState iPlayState)
		{
			this.mFadeTimers = new float[4];
			this.mMoveTimers = new float[4];
			this.mIcons = new SpellWheel.Icon[8];
			for (int i = 0; i < 8; i++)
			{
				this.mIcons[i].Cooldown = 0f;
				this.mIcons[i].Enabled = true;
				this.mIcons[i].Intensity = 1f;
				this.mIcons[i].ResetTimer = 0f;
				this.mIcons[i].Saturation = 1f;
			}
			this.mEnabledElements = TutorialManager.Instance.EnabledElements;
			this.mPlayer = new WeakReference(iPlayer);
			Texture2D texture2D = Game.Instance.Content.Load<Texture2D>("UI/HUD/hud");
			GUIBasicEffect guibasicEffect = new GUIBasicEffect(Game.Instance.GraphicsDevice, null);
			guibasicEffect.SetScreenSize(RenderManager.Instance.ScreenSize.X, RenderManager.Instance.ScreenSize.Y);
			guibasicEffect.Texture = texture2D;
			guibasicEffect.TextureEnabled = true;
			guibasicEffect.Color = new Vector4(1f);
			float num = 1f / (float)texture2D.Height;
			float num2 = 1f / (float)texture2D.Width;
			int num3 = 48;
			int num4 = 6;
			VertexPositionTexture[] array = new VertexPositionTexture[num3 + num4];
			Vector2 vector = new Vector2(50f * num2, 49.5f * num);
			Vector2 vector2 = new Vector2(200f * num2, 106f * num);
			int num5 = 25;
			int num6 = 25;
			array[0].Position.X = (float)(-(float)num5);
			array[0].Position.Y = (float)num6;
			array[0].TextureCoordinate.X = vector2.X + 0f;
			array[0].TextureCoordinate.Y = vector2.Y + vector.Y;
			array[1].Position.X = (float)(-(float)num5);
			array[1].Position.Y = (float)(-(float)num6);
			array[1].TextureCoordinate.X = vector2.X + 0f;
			array[1].TextureCoordinate.Y = vector2.Y + 0f;
			array[2].Position.X = (float)num5;
			array[2].Position.Y = (float)(-(float)num6);
			array[2].TextureCoordinate.X = vector2.X + vector.X;
			array[2].TextureCoordinate.Y = vector2.Y + 0f;
			array[3].Position.X = (float)num5;
			array[3].Position.Y = (float)(-(float)num6);
			array[3].TextureCoordinate.X = vector2.X + vector.X;
			array[3].TextureCoordinate.Y = vector2.Y + 0f;
			array[4].Position.X = (float)num5;
			array[4].Position.Y = (float)num6;
			array[4].TextureCoordinate.X = vector2.X + vector.X;
			array[4].TextureCoordinate.Y = vector2.Y + vector.Y;
			array[5].Position.X = (float)(-(float)num5);
			array[5].Position.Y = (float)num6;
			array[5].TextureCoordinate.X = vector2.X + 0f;
			array[5].TextureCoordinate.Y = vector2.Y + vector.Y;
			vector2 = new Vector2(0f, 156f * num);
			num5 = 25;
			num6 = 25;
			VertexPositionTexture vertexPositionTexture = default(VertexPositionTexture);
			for (int j = 0; j < 8; j++)
			{
				int num7 = num4 + j * 6;
				float num8 = (float)(j % 5);
				int num9 = j / 5;
				vertexPositionTexture.Position.X = (float)(-(float)num5);
				vertexPositionTexture.Position.Y = (float)num6;
				vertexPositionTexture.TextureCoordinate.X = vector2.X + num8 * vector.X;
				vertexPositionTexture.TextureCoordinate.Y = vector2.Y + (float)num9 * vector.Y + vector.Y;
				array[num7] = vertexPositionTexture;
				vertexPositionTexture.Position.X = (float)(-(float)num5);
				vertexPositionTexture.Position.Y = (float)(-(float)num6);
				vertexPositionTexture.TextureCoordinate.X = vector2.X + num8 * vector.X;
				vertexPositionTexture.TextureCoordinate.Y = vector2.Y + (float)num9 * vector.Y;
				array[num7 + 1] = vertexPositionTexture;
				vertexPositionTexture.Position.X = (float)num5;
				vertexPositionTexture.Position.Y = (float)(-(float)num6);
				vertexPositionTexture.TextureCoordinate.X = vector2.X + num8 * vector.X + vector.X;
				vertexPositionTexture.TextureCoordinate.Y = vector2.Y + (float)num9 * vector.Y;
				array[num7 + 2] = vertexPositionTexture;
				vertexPositionTexture.Position.X = (float)num5;
				vertexPositionTexture.Position.Y = (float)(-(float)num6);
				vertexPositionTexture.TextureCoordinate.X = vector2.X + num8 * vector.X + vector.X;
				vertexPositionTexture.TextureCoordinate.Y = vector2.Y + (float)num9 * vector.Y;
				array[num7 + 3] = vertexPositionTexture;
				vertexPositionTexture.Position.X = (float)num5;
				vertexPositionTexture.Position.Y = (float)num6;
				vertexPositionTexture.TextureCoordinate.X = vector2.X + num8 * vector.X + vector.X;
				vertexPositionTexture.TextureCoordinate.Y = vector2.Y + (float)num9 * vector.Y + vector.Y;
				array[num7 + 4] = vertexPositionTexture;
				vertexPositionTexture.Position.X = (float)(-(float)num5);
				vertexPositionTexture.Position.Y = (float)num6;
				vertexPositionTexture.TextureCoordinate.X = vector2.X + num8 * vector.X;
				vertexPositionTexture.TextureCoordinate.Y = vector2.Y + (float)num9 * vector.Y + vector.Y;
				array[num7 + 5] = vertexPositionTexture;
			}
			VertexBuffer vertexBuffer;
			VertexDeclaration vertexDeclaration;
			lock (Game.Instance.GraphicsDevice)
			{
				vertexBuffer = new VertexBuffer(Game.Instance.GraphicsDevice, VertexPositionTexture.SizeInBytes * array.Length, BufferUsage.None);
				vertexBuffer.SetData<VertexPositionTexture>(array);
				vertexDeclaration = new VertexDeclaration(Game.Instance.GraphicsDevice, VertexPositionTexture.VertexElements);
			}
			this.mRenderData = new SpellWheel.RenderData[3];
			for (int k = 0; k < 3; k++)
			{
				this.mRenderData[k] = new SpellWheel.RenderData();
				this.mIcons.CopyTo(this.mRenderData[k].Icons, 0);
				this.mRenderData[k].Effect = guibasicEffect;
				this.mRenderData[k].VertexBuffer = vertexBuffer;
				this.mRenderData[k].VertexDeclaration = vertexDeclaration;
				this.mRenderData[k].IconOverlayStartVertex = 0;
				this.mRenderData[k].IconOverlayPrimitiveCount = 2;
				this.mRenderData[k].IconsStartVertex = 6;
				this.mRenderData[k].IconsPrimitiveCount = 2;
			}
			this.Initialize(iPlayState);
		}

		// Token: 0x0600061E RID: 1566 RVA: 0x0002265C File Offset: 0x0002085C
		public void Initialize(PlayState iPlayState)
		{
			this.mPlayState = iPlayState;
		}

		// Token: 0x0600061F RID: 1567 RVA: 0x00022668 File Offset: 0x00020868
		public void Enable(Elements iElement)
		{
			for (int i = 0; i < 11; i++)
			{
				Elements elements = Spell.ElementFromIndex(i);
				if ((elements & iElement) == elements)
				{
					this.mIcons[i].Enabled = true;
					this.mIcons[i].ResetTimer = 0.5f;
					if (i <= 7)
					{
						if (this.Player.Controller is DirectInputController)
						{
							(this.Player.Controller as DirectInputController).SetFadeTime(SpellWheel.ELEMENT_DIRECTIONS[i], 1.5f);
						}
						else
						{
							(this.Player.Controller as XInputController).SetFadeTime(SpellWheel.ELEMENT_DIRECTIONS[i], 1.5f);
						}
					}
				}
			}
			this.mMoveTime = 0f;
		}

		// Token: 0x06000620 RID: 1568 RVA: 0x00022728 File Offset: 0x00020928
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

		// Token: 0x06000621 RID: 1569 RVA: 0x00022780 File Offset: 0x00020980
		public void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			Avatar avatar = this.Player.Avatar;
			if (avatar == null || avatar.Dead || GlobalSettings.Instance.SpellWheel != SettingOptions.On)
			{
				return;
			}
			SpellWheel.RenderData renderData = this.mRenderData[(int)iDataChannel];
			for (int i = 0; i < this.mIcons.Length; i++)
			{
				if (this.mIcons[i].Enabled)
				{
					if (this.mIcons[i].ResetTimer > 0f)
					{
						SpellWheel.Icon[] array = this.mIcons;
						int num = i;
						array[num].ResetTimer = array[num].ResetTimer - iDeltaTime;
						this.mIcons[i].Intensity = MathHelper.Min(this.mIcons[i].Intensity + iDeltaTime * 20f, 20f);
						if (this.Player.Controller is DirectInputController)
						{
							this.mMoveTime = Math.Min(this.mMoveTime + iDeltaTime, 1f);
							(this.Player.Controller as DirectInputController).SetMoveTime(SpellWheel.ELEMENT_DIRECTIONS[i], this.mMoveTime);
						}
					}
					else
					{
						this.mIcons[i].Intensity = MathHelper.Min(this.mIcons[i].Intensity + iDeltaTime * 10f, 1f);
					}
					this.mIcons[i].Saturation = MathHelper.Min(this.mIcons[i].Saturation + iDeltaTime * 4f, 1f);
				}
				else
				{
					this.mIcons[i].Saturation = MathHelper.Max(this.mIcons[i].Saturation - iDeltaTime * 4f, 0f);
				}
			}
			this.mIcons.CopyTo(renderData.Icons, 0);
			renderData.WorldPosition = avatar.Position;
			renderData.Direction = this.mDirection;
			renderData.FadeTimers = this.mFadeTimers;
			renderData.MoveTimers = this.mMoveTimers;
			renderData.HelpTime = this.mHelpTimer;
			this.mPlayState.Scene.AddRenderableGUIObject(iDataChannel, renderData);
		}

		// Token: 0x06000622 RID: 1570 RVA: 0x0002299E File Offset: 0x00020B9E
		public void HelpTimer(float iHelpTimer)
		{
			this.mHelpTimer = iHelpTimer;
		}

		// Token: 0x06000623 RID: 1571 RVA: 0x000229A7 File Offset: 0x00020BA7
		internal void Direction(ControllerDirection iDirection)
		{
			this.mDirection = iDirection;
		}

		// Token: 0x06000624 RID: 1572 RVA: 0x000229B0 File Offset: 0x00020BB0
		public void MoveTimers(float[] iTimers)
		{
			this.mMoveTimers = iTimers;
		}

		// Token: 0x06000625 RID: 1573 RVA: 0x000229B9 File Offset: 0x00020BB9
		public void FadeTimers(float[] iTimers)
		{
			this.mFadeTimers = iTimers;
		}

		// Token: 0x06000626 RID: 1574 RVA: 0x000229C4 File Offset: 0x00020BC4
		internal static int GetDirectionIndex(ControllerDirection iDirection)
		{
			switch (iDirection)
			{
			case ControllerDirection.Right:
				return 0;
			case ControllerDirection.Up:
				return 1;
			case ControllerDirection.UpRight:
				break;
			case ControllerDirection.Left:
				return 2;
			default:
				if (iDirection == ControllerDirection.Down)
				{
					return 3;
				}
				break;
			}
			return -1;
		}

		// Token: 0x1700012F RID: 303
		// (get) Token: 0x06000627 RID: 1575 RVA: 0x000229FA File Offset: 0x00020BFA
		protected Player Player
		{
			get
			{
				return this.mPlayer.Target as Player;
			}
		}

		// Token: 0x040004A7 RID: 1191
		private static readonly Point[] DIRECTION_INDICES = new Point[]
		{
			new Point(2, 3),
			new Point(5, 6),
			new Point(4, 1),
			new Point(0, 7)
		};

		// Token: 0x040004A8 RID: 1192
		private static readonly int[] ELEMENT_DIRECTIONS = new int[]
		{
			3,
			2,
			0,
			0,
			2,
			1,
			1,
			3
		};

		// Token: 0x040004A9 RID: 1193
		private float[] mFadeTimers;

		// Token: 0x040004AA RID: 1194
		private float[] mMoveTimers;

		// Token: 0x040004AB RID: 1195
		private float mHelpTimer;

		// Token: 0x040004AC RID: 1196
		private ControllerDirection mDirection;

		// Token: 0x040004AD RID: 1197
		private Elements mEnabledElements;

		// Token: 0x040004AE RID: 1198
		private SpellWheel.Icon[] mIcons;

		// Token: 0x040004AF RID: 1199
		private float mMoveTime;

		// Token: 0x040004B0 RID: 1200
		private SpellWheel.RenderData[] mRenderData;

		// Token: 0x040004B1 RID: 1201
		private PlayState mPlayState;

		// Token: 0x040004B2 RID: 1202
		private WeakReference mPlayer;

		// Token: 0x020000C7 RID: 199
		protected class RenderData : IRenderableGUIObject, IPreRenderRenderer
		{
			// Token: 0x06000629 RID: 1577 RVA: 0x00022AAC File Offset: 0x00020CAC
			public RenderData()
			{
				this.MoveTimers = new float[4];
				this.FadeTimers = new float[4];
				this.Icons = new SpellWheel.Icon[8];
			}

			// Token: 0x0600062A RID: 1578 RVA: 0x00022AD8 File Offset: 0x00020CD8
			public void Draw(float iDeltaTime)
			{
				this.Effect.GraphicsDevice.Vertices[0].SetSource(this.VertexBuffer, 0, VertexPositionTexture.SizeInBytes);
				this.Effect.GraphicsDevice.VertexDeclaration = this.VertexDeclaration;
				this.Effect.SetScreenSize(RenderManager.Instance.ScreenSize.X, RenderManager.Instance.ScreenSize.Y);
				this.Effect.Begin();
				this.Effect.CurrentTechnique.Passes[0].Begin();
				if (this.HelpTime > 1E-45f)
				{
					float iAlpha = MathHelper.Clamp(this.HelpTime / 0.2f, 0f, 1f);
					for (int i = 0; i < 4; i++)
					{
						this.RenderDirectionIcons(i, iAlpha);
					}
				}
				else
				{
					for (int j = 0; j < 4; j++)
					{
						float iAlpha2 = MathHelper.Clamp(this.FadeTimers[j] / 0.2f, 0f, 1f);
						if (this.FadeTimers[j] > 1E-45f)
						{
							this.RenderDirectionIcons(j, iAlpha2);
						}
					}
				}
				this.Effect.CurrentTechnique.Passes[0].End();
				this.Effect.End();
			}

			// Token: 0x0600062B RID: 1579 RVA: 0x00022C18 File Offset: 0x00020E18
			private void RenderDirectionIcons(int iIndex, float iAlpha)
			{
				Point point = SpellWheel.DIRECTION_INDICES[iIndex];
				Vector2 vector = new Vector2(this.mPosition.X + (float)Math.Cos((double)Defines.ElementUIRadian[point.X]) * 64f, this.mPosition.Y - (float)Math.Sin((double)Defines.ElementUIRadian[point.X]) * 64f);
				Vector2 vector2 = new Vector2(this.mPosition.X + (float)Math.Cos((double)Defines.ElementUIRadian[point.Y]) * 64f, this.mPosition.Y - (float)Math.Sin((double)Defines.ElementUIRadian[point.Y]) * 64f);
				Vector2 vector3 = vector;
				Vector2 vector4 = vector2;
				if (this.MoveTimers[iIndex] < 0.2f)
				{
					Vector2 vector5 = new Vector2(this.mPosition.X + (float)Math.Cos((double)((float)iIndex * 1.5707964f)) * 64f, this.mPosition.Y - (float)Math.Sin((double)((float)iIndex * 1.5707964f)) * 64f);
					Vector2.Lerp(ref vector5, ref vector, this.MoveTimers[iIndex] / 0.2f, out vector3);
					Vector2.Lerp(ref vector5, ref vector2, this.MoveTimers[iIndex] / 0.2f, out vector4);
				}
				Matrix identity = Matrix.Identity;
				identity.M41 = vector4.X;
				identity.M42 = vector4.Y;
				this.Effect.Transform = identity;
				this.Effect.Saturation = this.Icons[point.Y].Saturation;
				this.Effect.Color = new Vector4(this.Icons[point.Y].Intensity, this.Icons[point.Y].Intensity, this.Icons[point.Y].Intensity, iAlpha);
				this.Effect.GraphicsDevice.RenderState.SourceBlend = Blend.SourceAlpha;
				this.Effect.GraphicsDevice.RenderState.DestinationBlend = Blend.InverseSourceAlpha;
				this.Effect.CommitChanges();
				this.Effect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, this.IconsStartVertex + point.Y * 6, 2);
				identity.M41 = vector3.X;
				identity.M42 = vector3.Y;
				this.Effect.Transform = identity;
				this.Effect.Saturation = this.Icons[point.X].Saturation;
				this.Effect.Color = new Vector4(this.Icons[point.X].Intensity, this.Icons[point.X].Intensity, this.Icons[point.X].Intensity, iAlpha);
				this.Effect.GraphicsDevice.RenderState.ColorWriteChannels = ColorWriteChannels.Alpha;
				this.Effect.GraphicsDevice.RenderState.SourceBlend = Blend.One;
				this.Effect.GraphicsDevice.RenderState.DestinationBlend = Blend.Zero;
				this.Effect.CommitChanges();
				this.Effect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, this.IconsStartVertex + point.X * 6, 2);
				Matrix.CreateRotationZ((float)iIndex * 1.5707964f, out identity);
				identity.M41 = vector4.X;
				identity.M42 = vector4.Y;
				this.Effect.Transform = identity;
				this.Effect.Saturation = this.Icons[point.Y].Saturation;
				this.Effect.Color = new Vector4(this.Icons[point.Y].Intensity, this.Icons[point.Y].Intensity, this.Icons[point.Y].Intensity, iAlpha);
				this.Effect.GraphicsDevice.RenderState.BlendFunction = BlendFunction.ReverseSubtract;
				this.Effect.GraphicsDevice.RenderState.DestinationBlend = Blend.One;
				this.Effect.CommitChanges();
				this.Effect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, this.IconOverlayStartVertex, 2);
				identity = Matrix.Identity;
				identity.M41 = vector3.X;
				identity.M42 = vector3.Y;
				this.Effect.Transform = identity;
				this.Effect.Saturation = this.Icons[point.X].Saturation;
				this.Effect.Color = new Vector4(this.Icons[point.X].Intensity, this.Icons[point.X].Intensity, this.Icons[point.X].Intensity, iAlpha);
				this.Effect.GraphicsDevice.RenderState.BlendFunction = BlendFunction.Add;
				this.Effect.GraphicsDevice.RenderState.ColorWriteChannels = ColorWriteChannels.All;
				this.Effect.GraphicsDevice.RenderState.SourceBlend = Blend.DestinationAlpha;
				this.Effect.GraphicsDevice.RenderState.DestinationBlend = Blend.InverseDestinationAlpha;
				this.Effect.CommitChanges();
				this.Effect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, this.IconsStartVertex + point.X * 6, 2);
				this.Effect.GraphicsDevice.RenderState.SourceBlend = Blend.SourceAlpha;
				this.Effect.GraphicsDevice.RenderState.DestinationBlend = Blend.InverseSourceAlpha;
				this.Effect.Color = new Vector4(1f, 1f, 1f, 1f);
			}

			// Token: 0x17000130 RID: 304
			// (get) Token: 0x0600062C RID: 1580 RVA: 0x0002320B File Offset: 0x0002140B
			public int ZIndex
			{
				get
				{
					return 100;
				}
			}

			// Token: 0x0600062D RID: 1581 RVA: 0x0002320F File Offset: 0x0002140F
			public void PreRenderUpdate(DataChannel iDataChannel, float iDeltaTime, ref Matrix iViewProjectionMatrix, ref Vector3 iCameraPosition, ref Vector3 iCameraDirection)
			{
				this.mPosition = MagickaMath.WorldToScreenPosition(ref this.WorldPosition, ref iViewProjectionMatrix);
			}

			// Token: 0x040004B3 RID: 1203
			public GUIBasicEffect Effect;

			// Token: 0x040004B4 RID: 1204
			public VertexBuffer VertexBuffer;

			// Token: 0x040004B5 RID: 1205
			public VertexDeclaration VertexDeclaration;

			// Token: 0x040004B6 RID: 1206
			public int IconsStartVertex;

			// Token: 0x040004B7 RID: 1207
			public int IconsPrimitiveCount;

			// Token: 0x040004B8 RID: 1208
			public int IconOverlayStartVertex;

			// Token: 0x040004B9 RID: 1209
			public int IconOverlayPrimitiveCount;

			// Token: 0x040004BA RID: 1210
			private Vector2 mPosition;

			// Token: 0x040004BB RID: 1211
			public Vector3 WorldPosition;

			// Token: 0x040004BC RID: 1212
			internal ControllerDirection Direction;

			// Token: 0x040004BD RID: 1213
			public float[] MoveTimers;

			// Token: 0x040004BE RID: 1214
			public float[] FadeTimers;

			// Token: 0x040004BF RID: 1215
			public float HelpTime;

			// Token: 0x040004C0 RID: 1216
			public SpellWheel.Icon[] Icons;
		}

		// Token: 0x020000C8 RID: 200
		public struct Icon
		{
			// Token: 0x040004C1 RID: 1217
			public bool Enabled;

			// Token: 0x040004C2 RID: 1218
			public float Saturation;

			// Token: 0x040004C3 RID: 1219
			public float Cooldown;

			// Token: 0x040004C4 RID: 1220
			public float ResetTimer;

			// Token: 0x040004C5 RID: 1221
			public float Intensity;
		}
	}
}
