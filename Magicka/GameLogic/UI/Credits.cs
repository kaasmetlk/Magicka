using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using Magicka.Achievements;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.GameStates.Menu.Main;
using Magicka.Graphics;
using Magicka.Levels;
using Magicka.Localization;
using Magicka.Storage;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;

namespace Magicka.GameLogic.UI
{
	// Token: 0x02000422 RID: 1058
	internal class Credits
	{
		// Token: 0x1700080E RID: 2062
		// (get) Token: 0x060020D8 RID: 8408 RVA: 0x000E8BBC File Offset: 0x000E6DBC
		public static Credits Instance
		{
			get
			{
				if (Credits.sSingelton == null)
				{
					lock (Credits.sSingeltonLock)
					{
						if (Credits.sSingelton == null)
						{
							Credits.sSingelton = new Credits();
						}
					}
				}
				return Credits.sSingelton;
			}
		}

		// Token: 0x060020D9 RID: 8409 RVA: 0x000E8C10 File Offset: 0x000E6E10
		private Credits()
		{
			this.mInitialized = false;
			this.mRenderData = new Credits.RenderData[3];
			for (int i = 0; i < 3; i++)
			{
				this.mRenderData[i] = new Credits.RenderData();
			}
		}

		// Token: 0x060020DA RID: 8410 RVA: 0x000E8C80 File Offset: 0x000E6E80
		public void Initialize(string fileName)
		{
			if (this.mInitialized)
			{
				return;
			}
			List<Credits.CreditData> list = new List<Credits.CreditData>(32);
			XmlDocument xmlDocument = new XmlDocument();
			if (fileName == null || fileName == "")
			{
				xmlDocument.Load("Content/Data/Credits.xml");
			}
			else
			{
				xmlDocument.Load(string.Format("Content/Data/{0}", fileName));
			}
			XmlNode xmlNode = null;
			for (int i = 0; i < xmlDocument.ChildNodes.Count; i++)
			{
				XmlNode xmlNode2 = xmlDocument.ChildNodes[i];
				if (xmlNode2.Name.Equals("credits", StringComparison.OrdinalIgnoreCase))
				{
					for (int j = 0; j < xmlNode2.Attributes.Count; j++)
					{
						if (xmlNode2.Attributes[j].Name.Equals("speed", StringComparison.OrdinalIgnoreCase))
						{
							this.SCROLL_SPEED = float.Parse(xmlNode2.Attributes[j].Value, CultureInfo.InvariantCulture.NumberFormat);
							this.DIV_SCROLL = 1f / this.SCROLL_SPEED;
						}
					}
					xmlNode = xmlNode2;
					break;
				}
			}
			for (int k = 0; k < xmlNode.ChildNodes.Count; k++)
			{
				XmlNode xmlNode3 = xmlNode.ChildNodes[k];
				if (xmlNode3.Name.Equals("credit", StringComparison.OrdinalIgnoreCase))
				{
					string iString = xmlNode3.InnerText.ToLowerInvariant();
					Credits.CreditData item;
					item.ID = iString.GetHashCodeCustom();
					item.COLOR = Vector3.One;
					item.FONT = MagickaFont.Maiandra18;
					item.PADDING = 0f;
					for (int l = 0; l < xmlNode3.Attributes.Count; l++)
					{
						XmlAttribute xmlAttribute = xmlNode3.Attributes[l];
						if (xmlAttribute.Name.Equals("color", StringComparison.OrdinalIgnoreCase))
						{
							string text = xmlAttribute.Value.Replace(" ", "");
							string[] array = text.Split(new char[]
							{
								','
							});
							item.COLOR.X = float.Parse(array[0], CultureInfo.InvariantCulture.NumberFormat);
							item.COLOR.Y = float.Parse(array[1], CultureInfo.InvariantCulture.NumberFormat);
							item.COLOR.Z = float.Parse(array[2], CultureInfo.InvariantCulture.NumberFormat);
						}
						else if (xmlAttribute.Name.Equals("font", StringComparison.OrdinalIgnoreCase))
						{
							item.FONT = (MagickaFont)Enum.Parse(typeof(MagickaFont), xmlAttribute.Value, true);
						}
						else if (xmlAttribute.Name.Equals("padding", StringComparison.OrdinalIgnoreCase))
						{
							item.PADDING = float.Parse(xmlAttribute.Value, CultureInfo.InvariantCulture.NumberFormat);
						}
					}
					list.Add(item);
				}
			}
			this.mTotalLength = 0f;
			this.mNrOfTexts = list.Count;
			this.mTexts = new List<Text>(this.mNrOfTexts);
			this.mTextDatas = new List<Credits.ElementData>(this.mNrOfTexts);
			int iTargetLineWidth = 400;
			int num = 0;
			for (int m = 0; m < this.mNrOfTexts; m++)
			{
				BitmapFont font = FontManager.Instance.GetFont(list[m].FONT);
				string text2 = LanguageManager.Instance.GetString(list[m].ID);
				text2 = font.Wrap(text2, iTargetLineWidth, true);
				string[] array2 = text2.Split(new char[]
				{
					'\n'
				});
				num += array2.Length - 1;
				for (int n = 0; n < array2.Length; n++)
				{
					text2 = array2[n];
					Text text3 = new Text(text2.Length + 1, font, TextAlign.Center, false);
					text3.DefaultColor = new Vector4(list[m].COLOR, 1f);
					text3.SetText(text2);
					this.mTexts.Add(text3);
					Credits.ElementData item2 = default(Credits.ElementData);
					item2.Alpha = 0f;
					item2.Height = font.MeasureText(text2, true).Y;
					item2.Padding = ((n == 0) ? list[m].PADDING : 0f);
					this.mTextDatas.Add(item2);
					this.mTotalLength += this.mTextDatas[m].Height + this.mTextDatas[m].Padding;
				}
			}
			this.mNrOfTexts += num;
			for (int num2 = 0; num2 < 3; num2++)
			{
				this.mRenderData[num2].Texts = this.mTexts;
				this.mRenderData[num2].TextDatas = this.mTextDatas;
			}
			this.mInitialized = true;
			this.mFadeInAlpha = 0f;
		}

		// Token: 0x1700080F RID: 2063
		// (get) Token: 0x060020DB RID: 8411 RVA: 0x000E9163 File Offset: 0x000E7363
		public bool IsActive
		{
			get
			{
				return this.mStarted;
			}
		}

		// Token: 0x060020DC RID: 8412 RVA: 0x000E916C File Offset: 0x000E736C
		public void Kill()
		{
			this.mStarted = false;
			this.mFadeInAlpha = 0f;
			this.mSkymapFadeInTime = 0f;
			this.mFadeOut = false;
			this.mInitialized = false;
			ControlManager.Instance.UnlimitInput();
			if (this.mSaveSlot != null)
			{
				SubMenuEndGame.Instance.Set(true, this.mSaveSlot);
				SaveData currentSaveData = SubMenuCampaignSelect_SaveSlotSelect.Instance.CurrentSaveData;
				if (currentSaveData != null && currentSaveData.CurrentPlayTime + this.mPlayTime <= 14400)
				{
					AchievementsManager.Instance.AwardAchievement(PlayState.RecentPlayState, "missionimprobable");
				}
			}
			this.mSaveSlot = null;
		}

		// Token: 0x060020DD RID: 8413 RVA: 0x000E9204 File Offset: 0x000E7404
		public void Start(SaveData iSaveSlot)
		{
			if (!this.mInitialized)
			{
				this.Initialize("");
			}
			this.mSaveSlot = iSaveSlot;
			this.mFadeInAlpha = 0f;
			this.mPosition = (float)RenderManager.Instance.ScreenSize.Y * 0.5f;
			this.mStartIndex = 0;
			float num = 0f;
			for (int i = 0; i < this.mNrOfTexts; i++)
			{
				num += this.mTextDatas[i].Height + this.mTextDatas[i].Padding;
				if (num > this.mPosition)
				{
					this.mEndIndex = i;
					break;
				}
			}
			this.mFadeOut = false;
			this.mFadingOut = false;
			this.mStarted = true;
			this.mSkymapColor = RenderManager.Instance.SkyMapColor;
			this.mFadeInAlpha = 0f;
			this.mSkymapFadeInTime = 0f;
		}

		// Token: 0x060020DE RID: 8414 RVA: 0x000E92E4 File Offset: 0x000E74E4
		private void ScreenCaptureCallback()
		{
			RenderManager.Instance.EndTransition(Transitions.CrossFade, Color.White, 1f);
			SubMenuEndGame.Instance.CreditsEnd = true;
			Tome.Instance.PushMenuInstant(SubMenuEndGame.Instance, 1);
			Tome.Instance.ChangeState(Tome.OpenState.Instance);
			GameStateManager.Instance.PopState();
		}

		// Token: 0x060020DF RID: 8415 RVA: 0x000E933C File Offset: 0x000E753C
		public void Update(DataChannel iDataChannel, float iDeltaTime, PlayState iPlayState)
		{
			if (!this.mInitialized | !this.mStarted)
			{
				return;
			}
			if (this.mFadeOut)
			{
				if (!this.mFadingOut)
				{
					this.mFadingOut = true;
					this.mPlayTime = (int)iPlayState.PlayTime;
					iPlayState.Endgame(EndGameCondition.EndOffGame, false, false, 0f);
					RenderManager.Instance.TransitionEnd += this.OnTransitionEnd;
					RenderManager.Instance.BeginTransition(Transitions.Fade, Color.Black, 1f);
				}
				Credits.RenderData iObject = this.mRenderData[(int)iDataChannel];
				iPlayState.Scene.AddRenderableGUIObject(iDataChannel, iObject);
				return;
			}
			if (this.mSkymapFadeInTime < 1f)
			{
				this.mSkymapFadeInTime = Math.Min(this.mSkymapFadeInTime + iDeltaTime * 0.5f, 1f);
				Vector3 skyMapColor = iPlayState.Level.CurrentScene.SkyMapColor;
				Vector3.Lerp(ref this.mSkymapColor, ref this.mBlack, this.mSkymapFadeInTime, out skyMapColor);
				RenderManager.Instance.SkyMapColor = skyMapColor;
				return;
			}
			this.mFadeInAlpha = Math.Min(this.mFadeInAlpha + iDeltaTime * 2f, 1f);
			Credits.RenderData renderData = this.mRenderData[(int)iDataChannel];
			renderData.FadeInAlpha = this.mFadeInAlpha;
			if (this.mFadeInAlpha >= 1f)
			{
				float num = (float)RenderManager.Instance.ScreenSize.Y * 0.5f;
				this.mPosition -= iDeltaTime * this.DIV_SCROLL * num;
				if (this.mPosition <= -this.mTotalLength)
				{
					this.mFadeOut = true;
				}
				float num2 = this.mPosition;
				this.mStartIndex = 0;
				this.mEndIndex = this.mNrOfTexts;
				for (int i = 0; i < this.mNrOfTexts; i++)
				{
					Credits.ElementData value = this.mTextDatas[i];
					float height = value.Height;
					num2 += this.mTextDatas[i].Padding;
					value.Position = num2;
					float num3 = Math.Min(num2 / height, 1f);
					float num4 = Math.Max(Math.Min((num - (num2 + height)) / height, 1f), 0f);
					value.Alpha = num3 * num4;
					if (num3 <= 0f && i < this.mStartIndex)
					{
						this.mStartIndex = i;
					}
					else if (num4 <= 0f && i < this.mEndIndex)
					{
						this.mEndIndex = i;
					}
					this.mTextDatas[i] = value;
					num2 += height;
				}
				renderData.StartIndex = this.mStartIndex;
				renderData.EndIndex = this.mEndIndex;
			}
			iPlayState.Scene.AddRenderableGUIObject(iDataChannel, renderData);
		}

		// Token: 0x060020E0 RID: 8416 RVA: 0x000E95E0 File Offset: 0x000E77E0
		private void OnTransitionEnd(TransitionEffect iDeadTransition)
		{
			RenderManager.Instance.TransitionEnd -= this.OnTransitionEnd;
			this.mStarted = false;
			this.mFadeInAlpha = 0f;
			this.mSkymapFadeInTime = 0f;
			ControlManager.Instance.UnlimitInput();
			Texture2D screenShot = RenderManager.Instance.GetScreenShot(new Action(this.ScreenCaptureCallback));
			SubMenuEndGame.Instance.ScreenShot = screenShot;
			RenderManager.Instance.GetTransitionEffect(Transitions.CrossFade).SourceTexture1 = screenShot;
			SubMenuEndGame.Instance.Set(true, this.mSaveSlot);
			if (this.mSaveSlot != null && SubMenuCampaignSelect_SaveSlotSelect.Instance.GameType == GameType.Campaign && SubMenuCampaignSelect_SaveSlotSelect.Instance.CurrentSaveData.CurrentPlayTime + this.mPlayTime <= 14400)
			{
				AchievementsManager.Instance.AwardAchievement(PlayState.RecentPlayState, "missionimprobable");
			}
		}

		// Token: 0x04002341 RID: 9025
		private static Credits sSingelton;

		// Token: 0x04002342 RID: 9026
		private static volatile object sSingeltonLock = new object();

		// Token: 0x04002343 RID: 9027
		private float SCROLL_SPEED = 16f;

		// Token: 0x04002344 RID: 9028
		private float DIV_SCROLL = 0.0625f;

		// Token: 0x04002345 RID: 9029
		private int mNrOfTexts;

		// Token: 0x04002346 RID: 9030
		private bool mInitialized;

		// Token: 0x04002347 RID: 9031
		private bool mStarted;

		// Token: 0x04002348 RID: 9032
		private bool mFadeOut;

		// Token: 0x04002349 RID: 9033
		private bool mFadingOut;

		// Token: 0x0400234A RID: 9034
		private int mStartIndex;

		// Token: 0x0400234B RID: 9035
		private int mEndIndex;

		// Token: 0x0400234C RID: 9036
		private Vector3 mSkymapColor;

		// Token: 0x0400234D RID: 9037
		private Vector3 mBlack = new Vector3(0f, 0f, 0f);

		// Token: 0x0400234E RID: 9038
		private float mSkymapFadeInTime;

		// Token: 0x0400234F RID: 9039
		private float mFadeInAlpha;

		// Token: 0x04002350 RID: 9040
		private float mTotalLength;

		// Token: 0x04002351 RID: 9041
		private SaveData mSaveSlot;

		// Token: 0x04002352 RID: 9042
		private float mPosition;

		// Token: 0x04002353 RID: 9043
		private List<Text> mTexts;

		// Token: 0x04002354 RID: 9044
		private List<Credits.ElementData> mTextDatas;

		// Token: 0x04002355 RID: 9045
		private Credits.RenderData[] mRenderData;

		// Token: 0x04002356 RID: 9046
		private int mPlayTime;

		// Token: 0x02000423 RID: 1059
		protected class RenderData : IRenderableGUIObject
		{
			// Token: 0x060020E2 RID: 8418 RVA: 0x000E96C4 File Offset: 0x000E78C4
			static RenderData()
			{
				VertexPositionColor[] array = new VertexPositionColor[]
				{
					new VertexPositionColor(new Vector3(0f, 0f, 0f), new Color(0, 0, 0, byte.MaxValue)),
					new VertexPositionColor(new Vector3(1f, 0f, 0f), new Color(0, 0, 0, byte.MaxValue)),
					new VertexPositionColor(new Vector3(0f, 1f, 0f), new Color(0, 0, 0, byte.MaxValue)),
					new VertexPositionColor(new Vector3(1f, 1f, 0f), new Color(0, 0, 0, byte.MaxValue)),
					new VertexPositionColor(new Vector3(0f, 1.1f, 0f), new Color(0, 0, 0, 0)),
					new VertexPositionColor(new Vector3(1f, 1.1f, 0f), new Color(0, 0, 0, 0))
				};
				lock (Game.Instance.GraphicsDevice)
				{
					Credits.RenderData.sEffect = new GUIBasicEffect(Game.Instance.GraphicsDevice, RenderManager.Instance.GlobalDummyEffect.EffectPool);
					Credits.RenderData.sVertexBuffer = new VertexBuffer(Game.Instance.GraphicsDevice, VertexPositionColor.SizeInBytes * array.Length, BufferUsage.WriteOnly);
					Credits.RenderData.sVertexBuffer.SetData<VertexPositionColor>(array);
					Credits.RenderData.sVertexDeclaration = new VertexDeclaration(Game.Instance.GraphicsDevice, VertexPositionColor.VertexElements);
				}
			}

			// Token: 0x060020E3 RID: 8419 RVA: 0x000E9890 File Offset: 0x000E7A90
			public void Draw(float iDeltaTime)
			{
				Point screenSize = RenderManager.Instance.ScreenSize;
				Credits.RenderData.sEffect.SetScreenSize(screenSize.X, screenSize.Y);
				Credits.RenderData.sEffect.GraphicsDevice.Vertices[0].SetSource(Credits.RenderData.sVertexBuffer, 0, VertexPositionColor.SizeInBytes);
				Credits.RenderData.sEffect.GraphicsDevice.VertexDeclaration = Credits.RenderData.sVertexDeclaration;
				Matrix transform = new Matrix((float)screenSize.X, 0f, 0f, 0f, 0f, (float)screenSize.Y * 0.5f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 1f);
				Credits.RenderData.sEffect.Transform = transform;
				Credits.RenderData.sEffect.Color = new Vector4(1f, 1f, 1f, this.FadeInAlpha);
				Credits.RenderData.sEffect.VertexColorEnabled = true;
				Credits.RenderData.sEffect.TextureEnabled = false;
				Credits.RenderData.sEffect.GraphicsDevice.RenderState.CullMode = CullMode.None;
				Credits.RenderData.sEffect.Begin();
				Credits.RenderData.sEffect.CurrentTechnique.Passes[0].Begin();
				Credits.RenderData.sEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 4);
				for (int i = this.StartIndex; i < this.EndIndex; i++)
				{
					Credits.RenderData.sEffect.Color = new Vector4(1f, 1f, 1f, this.TextDatas[i].Alpha);
					this.Texts[i].Draw(Credits.RenderData.sEffect, (float)screenSize.X * 0.5f, this.TextDatas[i].Position);
				}
				Credits.RenderData.sEffect.CurrentTechnique.Passes[0].End();
				Credits.RenderData.sEffect.End();
				Credits.RenderData.sEffect.GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;
			}

			// Token: 0x17000810 RID: 2064
			// (get) Token: 0x060020E4 RID: 8420 RVA: 0x000E9A9F File Offset: 0x000E7C9F
			public int ZIndex
			{
				get
				{
					return 200;
				}
			}

			// Token: 0x04002357 RID: 9047
			public const int ARRAY_SIZE = 32;

			// Token: 0x04002358 RID: 9048
			private static GUIBasicEffect sEffect;

			// Token: 0x04002359 RID: 9049
			private static VertexBuffer sVertexBuffer;

			// Token: 0x0400235A RID: 9050
			private static VertexDeclaration sVertexDeclaration;

			// Token: 0x0400235B RID: 9051
			public int StartIndex;

			// Token: 0x0400235C RID: 9052
			public int EndIndex;

			// Token: 0x0400235D RID: 9053
			public float FadeInAlpha;

			// Token: 0x0400235E RID: 9054
			public List<Text> Texts;

			// Token: 0x0400235F RID: 9055
			public List<Credits.ElementData> TextDatas;
		}

		// Token: 0x02000424 RID: 1060
		private struct CreditData
		{
			// Token: 0x04002360 RID: 9056
			public MagickaFont FONT;

			// Token: 0x04002361 RID: 9057
			public Vector3 COLOR;

			// Token: 0x04002362 RID: 9058
			public float PADDING;

			// Token: 0x04002363 RID: 9059
			public int ID;
		}

		// Token: 0x02000425 RID: 1061
		protected struct ElementData
		{
			// Token: 0x04002364 RID: 9060
			public float Alpha;

			// Token: 0x04002365 RID: 9061
			public float Position;

			// Token: 0x04002366 RID: 9062
			public float Padding;

			// Token: 0x04002367 RID: 9063
			public float Height;

			// Token: 0x04002368 RID: 9064
			public bool Index;
		}
	}
}
