using System;
using System.IO;
using Magicka.GameLogic.GameStates;
using Magicka.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;

namespace Magicka.Achievements
{
	// Token: 0x02000471 RID: 1137
	internal class AchievementPopupWindow : AchievementWindow
	{
		// Token: 0x06002261 RID: 8801 RVA: 0x000F6664 File Offset: 0x000F4864
		public AchievementPopupWindow()
		{
			GraphicsDevice graphicsDevice = Game.Instance.GraphicsDevice;
			if (AchievementPopupWindow.sEffect == null)
			{
				lock (graphicsDevice)
				{
					TextureCreationParameters @default = TextureCreationParameters.Default;
					@default.Format = SurfaceFormat.Dxt3;
					AchievementPopupWindow.sTexture = Texture2D.FromFile(graphicsDevice, "content/connectui/popup.png", @default);
					AchievementPopupWindow.sEffect = new GUIBasicEffect(graphicsDevice, null);
				}
				TextReader textReader = File.OpenText("content/connectui/popup.txt");
				int num = int.Parse(textReader.ReadLine());
				if (num < 0 || num > 1)
				{
					textReader.Close();
					throw new Exception("Invalid value in popup.txt");
				}
				textReader.ReadLine();
				Rectangle rectangle = default(Rectangle);
				rectangle.X = int.Parse(textReader.ReadLine());
				rectangle.Y = int.Parse(textReader.ReadLine());
				rectangle.Width = int.Parse(textReader.ReadLine());
				rectangle.Height = int.Parse(textReader.ReadLine());
				AchievementPopupWindow.sRectangleWidth = (float)rectangle.Width;
				textReader.Close();
				Vector2 vector = default(Vector2);
				vector.X = 1f / (float)AchievementPopupWindow.sTexture.Width;
				vector.Y = 1f / (float)AchievementPopupWindow.sTexture.Height;
				Vector4[] array = new Vector4[8];
				array[0].X = 0f;
				array[0].Y = 0f;
				array[0].Z = (float)rectangle.X * vector.X;
				array[0].W = (float)rectangle.Y * vector.Y;
				array[1].X = (float)rectangle.Width;
				array[1].Y = 0f;
				array[1].Z = (float)(rectangle.X + rectangle.Width) * vector.X;
				array[1].W = (float)rectangle.Y * vector.Y;
				array[2].X = (float)rectangle.Width;
				array[2].Y = (float)rectangle.Height;
				array[2].Z = (float)(rectangle.X + rectangle.Width) * vector.X;
				array[2].W = (float)(rectangle.Y + rectangle.Height) * vector.Y;
				array[3].X = 0f;
				array[3].Y = (float)rectangle.Height;
				array[3].Z = (float)rectangle.X * vector.X;
				array[3].W = (float)(rectangle.Y + rectangle.Height) * vector.Y;
				array[4].X = 0f;
				array[4].Y = 0f;
				array[4].Z = 0f;
				array[4].W = 0f;
				array[5].X = 41f;
				array[5].Y = 0f;
				array[5].Z = 1f;
				array[5].W = 0f;
				array[6].X = 41f;
				array[6].Y = 41f;
				array[6].Z = 1f;
				array[6].W = 1f;
				array[7].X = 0f;
				array[7].Y = 41f;
				array[7].Z = 0f;
				array[7].W = 1f;
				lock (graphicsDevice)
				{
					AchievementPopupWindow.sVertices = new VertexBuffer(graphicsDevice, array.Length * 4 * 4, BufferUsage.WriteOnly);
					AchievementPopupWindow.sVertices.SetData<Vector4>(array);
					AchievementPopupWindow.sVertexDeclaration = new VertexDeclaration(graphicsDevice, new VertexElement[]
					{
						new VertexElement(0, 0, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.Position, 0),
						new VertexElement(0, 8, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 0)
					});
				}
			}
			this.mAchievementFont = FontManager.Instance.GetFont(MagickaFont.PDX_UI_Bold);
			this.mScoreFont = FontManager.Instance.GetFont(MagickaFont.PDX_Points);
			this.mUnlockedText = new Text(64, this.mAchievementFont, TextAlign.Left, false);
			this.mAchievementText = new Text(128, this.mAchievementFont, TextAlign.Left, false);
			this.mScoreText = new Text(32, this.mScoreFont, TextAlign.Right, false);
			this.mRenderData = new AchievementPopupWindow.RenderData[3];
			for (int i = 0; i < 3; i++)
			{
				this.mRenderData[i] = new AchievementPopupWindow.RenderData(this.mUnlockedText, this.mAchievementText, this.mScoreText);
			}
		}

		// Token: 0x06002262 RID: 8802 RVA: 0x000F6BA0 File Offset: 0x000F4DA0
		public virtual void Show(AchievementData iAchievement)
		{
			this.mTime = 0f;
			this.mAchiementCode = iAchievement.Code;
			Point screenSize = RenderManager.Instance.ScreenSize;
			AchievementPopupWindow.sEffect.SetScreenSize(screenSize.X, screenSize.Y);
			this.mUnlockedText.SetText(AchievementsManager.Instance.GetTranslation(AchievementsManager.ACHIEVEMENT_UNLOCKED));
			this.mAchievementText.SetText(iAchievement.Name);
			this.mScoreText.SetText(AchievementsManager.Instance.GetTranslation(AchievementsManager.NUM_PP).Replace("%d", iAchievement.Points.ToString()));
			base.Show();
		}

		// Token: 0x06002263 RID: 8803 RVA: 0x000F6C4A File Offset: 0x000F4E4A
		public override void Show()
		{
			throw new Exception("NEVER call this overload directly!");
		}

		// Token: 0x06002264 RID: 8804 RVA: 0x000F6C56 File Offset: 0x000F4E56
		public override void Hide()
		{
			base.Hide();
		}

		// Token: 0x06002265 RID: 8805 RVA: 0x000F6C60 File Offset: 0x000F4E60
		public override void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			base.Update(iDataChannel, iDeltaTime);
			this.mTime += iDeltaTime;
			if (this.mVisible & this.mTime > 3f)
			{
				this.Hide();
			}
			AchievementPopupWindow.RenderData renderData = this.mRenderData[(int)iDataChannel];
			renderData.AchiementCode = this.mAchiementCode;
			renderData.Alpha = this.mAlpha;
			renderData.Position.X = (float)Math.Floor((double)(((float)RenderManager.Instance.ScreenSize.X - AchievementPopupWindow.sRectangleWidth) * 0.5f));
			renderData.Position.Y = 32f;
			GameStateManager.Instance.CurrentState.Scene.AddRenderableGUIObject(iDataChannel, renderData);
		}

		// Token: 0x040025A9 RID: 9641
		public static readonly Vector3 DEFAULT_COLOR = new Vector3(0.9098039f);

		// Token: 0x040025AA RID: 9642
		public static readonly Vector3 ACHIEVEMENT_COLOR = new Vector3(0.45882353f, 0.8980392f, 0.23137255f);

		// Token: 0x040025AB RID: 9643
		public static readonly Vector3 POINTS_COLOR = new Vector3(1f);

		// Token: 0x040025AC RID: 9644
		private static Texture2D sTexture;

		// Token: 0x040025AD RID: 9645
		private static float sRectangleWidth;

		// Token: 0x040025AE RID: 9646
		private static VertexBuffer sVertices;

		// Token: 0x040025AF RID: 9647
		private static VertexDeclaration sVertexDeclaration;

		// Token: 0x040025B0 RID: 9648
		private static GUIBasicEffect sEffect;

		// Token: 0x040025B1 RID: 9649
		private AchievementPopupWindow.RenderData[] mRenderData;

		// Token: 0x040025B2 RID: 9650
		private Text mUnlockedText;

		// Token: 0x040025B3 RID: 9651
		private Text mAchievementText;

		// Token: 0x040025B4 RID: 9652
		private Text mScoreText;

		// Token: 0x040025B5 RID: 9653
		private BitmapFont mAchievementFont;

		// Token: 0x040025B6 RID: 9654
		private BitmapFont mScoreFont;

		// Token: 0x040025B7 RID: 9655
		private string mAchiementCode;

		// Token: 0x040025B8 RID: 9656
		private float mTime;

		// Token: 0x02000472 RID: 1138
		private class RenderData : IRenderableGUIObject
		{
			// Token: 0x06002267 RID: 8807 RVA: 0x000F6D4D File Offset: 0x000F4F4D
			public RenderData(Text iUnlockedText, Text iAchievementText, Text iScoreText)
			{
				this.mUnlockedText = iUnlockedText;
				this.mAchievementText = iAchievementText;
				this.mScoreText = iScoreText;
			}

			// Token: 0x06002268 RID: 8808 RVA: 0x000F6D6C File Offset: 0x000F4F6C
			public void Draw(float iDeltaTime)
			{
				Vector4 color = default(Vector4);
				color.X = (color.Y = (color.Z = 1f));
				color.W = this.Alpha;
				AchievementPopupWindow.sEffect.Color = color;
				AchievementPopupWindow.sEffect.Texture = AchievementPopupWindow.sTexture;
				Matrix transform = default(Matrix);
				transform.M11 = 1f;
				transform.M22 = 1f;
				transform.M41 = this.Position.X;
				transform.M42 = this.Position.Y;
				transform.M44 = 1f;
				AchievementPopupWindow.sEffect.Transform = transform;
				AchievementPopupWindow.sEffect.GraphicsDevice.Vertices[0].SetSource(AchievementPopupWindow.sVertices, 0, 16);
				AchievementPopupWindow.sEffect.GraphicsDevice.VertexDeclaration = AchievementPopupWindow.sVertexDeclaration;
				AchievementPopupWindow.sEffect.Begin();
				AchievementPopupWindow.sEffect.CurrentTechnique.Passes[0].Begin();
				AchievementPopupWindow.sEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
				Texture2D achievementImage = AchievementsManager.Instance.GetAchievementImage(this.AchiementCode);
				if (achievementImage != null)
				{
					AchievementPopupWindow.sEffect.Texture = achievementImage;
					transform.M41 += 10f;
					transform.M42 += 10f;
					AchievementPopupWindow.sEffect.Transform = transform;
					AchievementPopupWindow.sEffect.CommitChanges();
					AchievementPopupWindow.sEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 4, 2);
				}
				Vector2 vector = FontManager.Instance.GetFont(MagickaFont.PDX_UI_Bold).MeasureText(this.mUnlockedText.Characters, true);
				color.X = AchievementPopupWindow.DEFAULT_COLOR.X;
				color.Y = AchievementPopupWindow.DEFAULT_COLOR.Y;
				color.Z = AchievementPopupWindow.DEFAULT_COLOR.Z;
				AchievementPopupWindow.sEffect.Color = color;
				this.mUnlockedText.Draw(AchievementPopupWindow.sEffect, this.Position.X + 71f, this.Position.Y + 19f);
				color.X = AchievementPopupWindow.ACHIEVEMENT_COLOR.X;
				color.Y = AchievementPopupWindow.ACHIEVEMENT_COLOR.Y;
				color.Z = AchievementPopupWindow.ACHIEVEMENT_COLOR.Z;
				AchievementPopupWindow.sEffect.Color = color;
				this.mAchievementText.Draw(AchievementPopupWindow.sEffect, this.Position.X + 71f + vector.X + 10f, this.Position.Y + 19f);
				color.X = AchievementPopupWindow.POINTS_COLOR.X;
				color.Y = AchievementPopupWindow.POINTS_COLOR.Y;
				color.Z = AchievementPopupWindow.POINTS_COLOR.Z;
				AchievementPopupWindow.sEffect.Color = color;
				this.mScoreText.Draw(AchievementPopupWindow.sEffect, this.Position.X + 770f, this.Position.Y + 17f);
				AchievementPopupWindow.sEffect.CurrentTechnique.Passes[0].End();
				AchievementPopupWindow.sEffect.End();
			}

			// Token: 0x17000834 RID: 2100
			// (get) Token: 0x06002269 RID: 8809 RVA: 0x000F709F File Offset: 0x000F529F
			public int ZIndex
			{
				get
				{
					return 2147483646;
				}
			}

			// Token: 0x040025B9 RID: 9657
			public string AchiementCode;

			// Token: 0x040025BA RID: 9658
			public Vector2 Position;

			// Token: 0x040025BB RID: 9659
			public float Alpha;

			// Token: 0x040025BC RID: 9660
			private Text mUnlockedText;

			// Token: 0x040025BD RID: 9661
			private Text mAchievementText;

			// Token: 0x040025BE RID: 9662
			private Text mScoreText;
		}
	}
}
