using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using Magicka.Graphics;
using Magicka.Graphics.Effects;
using Magicka.Levels.Triggers;
using Magicka.Localization;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;

namespace Magicka.Levels
{
	// Token: 0x02000333 RID: 819
	public class TimedObjectiveRuleset : IRuleset
	{
		// Token: 0x06001906 RID: 6406 RVA: 0x000A43D4 File Offset: 0x000A25D4
		public TimedObjectiveRuleset(GameScene iScene, XmlNode iNode)
		{
			this.mGameScene = iScene;
			string text = "timestorage";
			string assetName = "UI/HUD/Dialog_Say";
			int num = 32;
			for (int i = 0; i < iNode.Attributes.Count; i++)
			{
				XmlAttribute xmlAttribute = iNode.Attributes[i];
				if (xmlAttribute.Name.Equals("texture", StringComparison.OrdinalIgnoreCase))
				{
					assetName = xmlAttribute.Value;
				}
				else if (xmlAttribute.Name.Equals("bordersize", StringComparison.OrdinalIgnoreCase))
				{
					num = int.Parse(xmlAttribute.Value);
				}
				else if (xmlAttribute.Name.Equals("timer", StringComparison.OrdinalIgnoreCase))
				{
					text = xmlAttribute.Value;
				}
			}
			this.mTimeStorageID = text.ToLowerInvariant().GetHashCodeCustom();
			this.mObjectives = new List<TimedObjectiveRuleset.Objective>(4);
			for (int j = 0; j < iNode.ChildNodes.Count; j++)
			{
				XmlNode xmlNode = iNode.ChildNodes[j];
				if (xmlNode.Name.Equals("Objective", StringComparison.OrdinalIgnoreCase))
				{
					int num2 = 0;
					int num3 = 0;
					int score = 1;
					int maxValue = 0;
					for (int k = 0; k < xmlNode.Attributes.Count; k++)
					{
						XmlAttribute xmlAttribute2 = xmlNode.Attributes[k];
						if (xmlAttribute2.Name.Equals("name", StringComparison.OrdinalIgnoreCase))
						{
							if (!string.IsNullOrEmpty(xmlAttribute2.Value))
							{
								num3 = xmlAttribute2.Value.ToLowerInvariant().GetHashCodeCustom();
							}
						}
						else if (xmlAttribute2.Name.Equals("id", StringComparison.OrdinalIgnoreCase))
						{
							if (!string.IsNullOrEmpty(xmlAttribute2.Value))
							{
								num2 = xmlAttribute2.Value.ToLowerInvariant().GetHashCodeCustom();
							}
						}
						else if (xmlAttribute2.Name.Equals("max", StringComparison.OrdinalIgnoreCase))
						{
							maxValue = int.Parse(xmlAttribute2.Value);
						}
						else if (xmlAttribute2.Name.Equals("score", StringComparison.OrdinalIgnoreCase))
						{
							score = int.Parse(xmlAttribute2.Value);
						}
					}
					if (num3 != 0 && num2 != 0)
					{
						TimedObjectiveRuleset.Objective item = default(TimedObjectiveRuleset.Objective);
						item.CounterID = num2;
						item.NameID = num3;
						item.Value = 0;
						item.Score = score;
						item.MaxValue = maxValue;
						this.mObjectives.Add(item);
					}
				}
				else if (xmlNode.Name.Equals("timebonus", StringComparison.OrdinalIgnoreCase))
				{
					this.mTimeBonus = default(TimedObjectiveRuleset.TimeBonus);
					this.mTimeBonus.MaxTime = 0f;
					this.mTimeBonus.MinTime = 0f;
					this.mTimeBonus.MaxTimeBonus = 1f;
					this.mTimeBonus.MinTimeBonus = 1f;
					this.mTimeBonus.CounterID = 0;
					for (int l = 0; l < xmlNode.Attributes.Count; l++)
					{
						XmlAttribute xmlAttribute3 = xmlNode.Attributes[l];
						if (xmlAttribute3.Name.Equals("minTime", StringComparison.OrdinalIgnoreCase))
						{
							this.mTimeBonus.MinTime = float.Parse(xmlAttribute3.Value, CultureInfo.InvariantCulture.NumberFormat);
						}
						else if (xmlAttribute3.Name.Equals("minTimeBonus", StringComparison.OrdinalIgnoreCase))
						{
							this.mTimeBonus.MinTimeBonus = float.Parse(xmlAttribute3.Value, CultureInfo.InvariantCulture.NumberFormat);
						}
						else if (xmlAttribute3.Name.Equals("maxTime", StringComparison.OrdinalIgnoreCase))
						{
							this.mTimeBonus.MaxTime = float.Parse(xmlAttribute3.Value, CultureInfo.InvariantCulture.NumberFormat);
						}
						else if (xmlAttribute3.Name.Equals("maxTimeBonus", StringComparison.OrdinalIgnoreCase))
						{
							this.mTimeBonus.MaxTimeBonus = float.Parse(xmlAttribute3.Value, CultureInfo.InvariantCulture.NumberFormat);
						}
						else if (xmlAttribute3.Name.Equals("counter", StringComparison.OrdinalIgnoreCase))
						{
							this.mTimeBonus.CounterID = xmlAttribute3.Value.ToLowerInvariant().GetHashCodeCustom();
						}
					}
				}
			}
			Point screenSize = RenderManager.Instance.ScreenSize;
			lock (Game.Instance.GraphicsDevice)
			{
				this.mGUIEffect = new GUIBasicEffect(Game.Instance.GraphicsDevice, null);
				this.mTextBoxEffect = new TextBoxEffect(Game.Instance.GraphicsDevice, Game.Instance.Content);
			}
			this.mGUIEffect.SetScreenSize(screenSize.X, screenSize.Y);
			this.mGUIEffect.Color = Vector4.One;
			this.mTextBoxEffect.BorderSize = (float)num;
			this.mTextBoxEffect.ScreenSize = new Vector2((float)screenSize.X, (float)screenSize.Y);
			this.mTextBoxEffect.Texture = Game.Instance.Content.Load<Texture2D>(assetName);
			this.mTextBoxEffect.Scale = 1f;
			this.mTextBoxEffect.Color = Vector4.One;
			BitmapFont font = FontManager.Instance.GetFont(MagickaFont.Maiandra16);
			this.mRenderData = new TimedObjectiveRuleset.RenderData[3];
			for (int m = 0; m < 3; m++)
			{
				this.mRenderData[m] = new TimedObjectiveRuleset.RenderData(this.mGUIEffect, this.mTextBoxEffect, font, Game.Instance.Content.Load<Texture2D>(assetName), num);
			}
		}

		// Token: 0x1700063D RID: 1597
		// (get) Token: 0x06001907 RID: 6407 RVA: 0x000A4930 File Offset: 0x000A2B30
		internal int TimerID
		{
			get
			{
				return this.mTimeStorageID;
			}
		}

		// Token: 0x1700063E RID: 1598
		// (get) Token: 0x06001908 RID: 6408 RVA: 0x000A4938 File Offset: 0x000A2B38
		internal List<TimedObjectiveRuleset.Objective> Objectives
		{
			get
			{
				return this.mObjectives;
			}
		}

		// Token: 0x1700063F RID: 1599
		// (get) Token: 0x06001909 RID: 6409 RVA: 0x000A4940 File Offset: 0x000A2B40
		internal bool TimeSuccess
		{
			get
			{
				return this.mGameScene.Level.GetCounterValue(this.mTimeBonus.CounterID) > 0;
			}
		}

		// Token: 0x17000640 RID: 1600
		// (get) Token: 0x0600190A RID: 6410 RVA: 0x000A4960 File Offset: 0x000A2B60
		internal float GetBonusMultiplier
		{
			get
			{
				float bonus = TimedObjectiveRuleset.TimeBonus.GetBonus(ref this.mTimeBonus, this.mGameScene.Level.GetTimerValue(this.mTimeStorageID) / 60f);
				return bonus * (float)this.mGameScene.Level.GetCounterValue(this.mTimeBonus.CounterID);
			}
		}

		// Token: 0x0600190B RID: 6411 RVA: 0x000A49B5 File Offset: 0x000A2BB5
		public int GetAnyArea()
		{
			return TriggerArea.ANYID;
		}

		// Token: 0x0600190C RID: 6412 RVA: 0x000A49BC File Offset: 0x000A2BBC
		public void Update(float iDeltaTime, DataChannel iDataChannel)
		{
			if (iDataChannel == DataChannel.None)
			{
				return;
			}
			NetworkState state = NetworkManager.Instance.State;
			if (state == NetworkState.Server)
			{
				if (this.mNetworkDirty && this.mNetworkTimer <= 0f)
				{
					this.mNetworkTimer = 1f;
					this.NetworkUpdate();
				}
				this.mNetworkTimer -= iDeltaTime;
			}
			if (!this.mGameScene.PlayState.IsPaused && !this.mGameScene.PlayState.IsInCutscene && !this.mGameScene.PlayState.IsGameEnded)
			{
				this.mTimeElapsed = this.mGameScene.Level.GetTimerValue(this.mTimeStorageID);
			}
			for (int i = 0; i < this.mObjectives.Count; i++)
			{
				TimedObjectiveRuleset.Objective value = this.mObjectives[i];
				if (this.mGameScene.Level.GetCounterValue(value.CounterID) != value.Value)
				{
					this.mNetworkDirty = true;
					int counterValue = this.mGameScene.Level.GetCounterValue(value.CounterID);
					value.Value = counterValue;
					this.mObjectives[i] = value;
					break;
				}
			}
			if (this.mTimeElapsed - this.mLastUpdatedElapsedTime >= 1f)
			{
				this.mLastUpdatedElapsedTime = this.mTimeElapsed;
				TimeSpan timeSpan = TimeSpan.FromSeconds((double)this.mTimeElapsed);
				string text = LanguageManager.Instance.GetString(TimedObjectiveRuleset.LOC_TIME_ELAPSED);
				if (this.mTimeElapsed >= 60f && this.mTimeElapsed < 3600f)
				{
					text = text.Replace("#1;", string.Format("0:{0:00}:{1:00}", timeSpan.Minutes, timeSpan.Seconds));
				}
				else if (this.mTimeElapsed >= 3600f)
				{
					text = text.Replace("#1;", string.Format("{0:0}:{1:00}:{2:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds));
				}
				else
				{
					text = text.Replace("#1;", string.Format("0:00:{0:00}", timeSpan.Seconds));
				}
				for (int j = 0; j < 3; j++)
				{
					this.mRenderData[j].SetTime(text);
				}
			}
			if (!this.mGameScene.PlayState.IsInCutscene)
			{
				this.mGameScene.Scene.AddRenderableGUIObject(iDataChannel, this.mRenderData[(int)iDataChannel]);
			}
		}

		// Token: 0x0600190D RID: 6413 RVA: 0x000A4C24 File Offset: 0x000A2E24
		public void LocalUpdate(float iDeltaTime, DataChannel iDataChannel)
		{
			if (iDataChannel == DataChannel.None)
			{
				return;
			}
			if (!this.mGameScene.PlayState.IsPaused && !this.mGameScene.PlayState.IsInCutscene && !this.mGameScene.PlayState.IsGameEnded)
			{
				this.mTimeElapsed = this.mGameScene.Level.GetTimerValue(this.mTimeStorageID);
			}
			if (this.mTimeElapsed - this.mLastUpdatedElapsedTime >= 1f)
			{
				this.mLastUpdatedElapsedTime = this.mTimeElapsed;
				TimeSpan timeSpan = TimeSpan.FromSeconds((double)this.mTimeElapsed);
				string text = LanguageManager.Instance.GetString(TimedObjectiveRuleset.LOC_TIME_ELAPSED);
				if (this.mTimeElapsed >= 60f && this.mTimeElapsed < 3600f)
				{
					text = text.Replace("#1;", string.Format("0:{0:00}:{1:00}", timeSpan.Minutes, timeSpan.Seconds));
				}
				else if (this.mTimeElapsed >= 3600f)
				{
					text = text.Replace("#1;", string.Format("{0:0}:{1:00}:{2:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds));
				}
				else
				{
					text = text.Replace("#1;", string.Format("0:00:{0:00}", timeSpan.Seconds));
				}
				for (int i = 0; i < 3; i++)
				{
					this.mRenderData[i].SetTime(text);
				}
			}
			if (!this.mGameScene.PlayState.IsInCutscene)
			{
				this.mGameScene.Scene.AddRenderableGUIObject(iDataChannel, this.mRenderData[(int)iDataChannel]);
			}
		}

		// Token: 0x0600190E RID: 6414 RVA: 0x000A4DC4 File Offset: 0x000A2FC4
		public void Initialize()
		{
			string text = LanguageManager.Instance.GetString(TimedObjectiveRuleset.LOC_TIME_ELAPSED);
			text = text.Replace("#1;", "0:00:00");
			for (int i = 0; i < 3; i++)
			{
				this.mRenderData[i].SetTime(text);
			}
			this.mNetworkDirty = true;
			this.mGameScene.Level.AddTimer(this.mTimeStorageID, true, 0f);
			this.mTimeElapsed = this.mGameScene.Level.GetTimerValue(this.mTimeStorageID);
			this.mLastUpdatedElapsedTime = 0f;
			for (int j = 0; j < this.mObjectives.Count; j++)
			{
				TimedObjectiveRuleset.Objective value = this.mObjectives[j];
				int counterValue = this.mGameScene.Level.GetCounterValue(value.CounterID);
				value.Value = counterValue;
				this.mObjectives[j] = value;
			}
		}

		// Token: 0x0600190F RID: 6415 RVA: 0x000A4EA8 File Offset: 0x000A30A8
		public void DeInitialize()
		{
		}

		// Token: 0x06001910 RID: 6416 RVA: 0x000A4EAC File Offset: 0x000A30AC
		private unsafe void NetworkUpdate()
		{
			RulesetMessage rulesetMessage = default(RulesetMessage);
			rulesetMessage.Type = Rulesets.TimedObjective;
			rulesetMessage.NrOfByteItems = (byte)Math.Min(this.mObjectives.Count, 16);
			for (int i = 0; i < this.mObjectives.Count; i++)
			{
				(&rulesetMessage.Byte.FixedElementField)[i] = (byte)this.mGameScene.Level.GetCounterValue(this.mObjectives[i].CounterID);
			}
			rulesetMessage.Float01 = this.mGameScene.Level.GetTimerValue(this.mTimeBonus.CounterID);
			NetworkManager.Instance.Interface.SendMessage<RulesetMessage>(ref rulesetMessage);
			this.mNetworkDirty = false;
		}

		// Token: 0x06001911 RID: 6417 RVA: 0x000A4F68 File Offset: 0x000A3168
		unsafe void IRuleset.NetworkUpdate(ref RulesetMessage iMsg)
		{
			fixed (byte* ptr = &iMsg.Byte.FixedElementField)
			{
				for (int i = 0; i < (int)iMsg.NrOfByteItems; i++)
				{
					TimedObjectiveRuleset.Objective value = this.mObjectives[i];
					if (value.Value != (int)ptr[i])
					{
						this.mGameScene.Level.SetCounterValue(value.CounterID, (int)ptr[i]);
						value.Value = (int)ptr[i];
						this.mObjectives[i] = value;
					}
				}
				this.mGameScene.Level.SetTimer(this.mTimeBonus.CounterID, iMsg.Float01);
			}
		}

		// Token: 0x17000641 RID: 1601
		// (get) Token: 0x06001912 RID: 6418 RVA: 0x000A5006 File Offset: 0x000A3206
		public Rulesets RulesetType
		{
			get
			{
				return Rulesets.TimedObjective;
			}
		}

		// Token: 0x17000642 RID: 1602
		// (get) Token: 0x06001913 RID: 6419 RVA: 0x000A5009 File Offset: 0x000A3209
		public bool IsVersusRuleset
		{
			get
			{
				return false;
			}
		}

		// Token: 0x04001AC9 RID: 6857
		private const float NETWORK_UPDATE_FREQ = 1f;

		// Token: 0x04001ACA RID: 6858
		public static readonly int LOC_TIME_ELAPSED = "#challenge_time".GetHashCodeCustom();

		// Token: 0x04001ACB RID: 6859
		private bool mNetworkDirty;

		// Token: 0x04001ACC RID: 6860
		private float mNetworkTimer;

		// Token: 0x04001ACD RID: 6861
		private List<TimedObjectiveRuleset.Objective> mObjectives;

		// Token: 0x04001ACE RID: 6862
		private int mTimeStorageID;

		// Token: 0x04001ACF RID: 6863
		private float mTimeElapsed;

		// Token: 0x04001AD0 RID: 6864
		private TimedObjectiveRuleset.RenderData[] mRenderData;

		// Token: 0x04001AD1 RID: 6865
		private GUIBasicEffect mGUIEffect;

		// Token: 0x04001AD2 RID: 6866
		private TextBoxEffect mTextBoxEffect;

		// Token: 0x04001AD3 RID: 6867
		private GameScene mGameScene;

		// Token: 0x04001AD4 RID: 6868
		private float mLastUpdatedElapsedTime;

		// Token: 0x04001AD5 RID: 6869
		private TimedObjectiveRuleset.TimeBonus mTimeBonus;

		// Token: 0x02000334 RID: 820
		private class RenderData : IRenderableGUIObject
		{
			// Token: 0x06001915 RID: 6421 RVA: 0x000A5020 File Offset: 0x000A3220
			public RenderData(GUIBasicEffect iEffect, TextBoxEffect iBoxEffect, BitmapFont iFont, Texture2D iTimeTexture, int iBorderSize)
			{
				this.mEffect = iEffect;
				this.mTextBoxEffect = iBoxEffect;
				this.mTimeTexture = iTimeTexture;
				this.mBorderSize = iBorderSize;
				this.mTimeTextString = null;
				this.mTimeText = new Text(200, iFont, TextAlign.Left, false);
				this.mTimeText.DefaultColor = Color.Cyan.ToVector4();
				this.mTimeText.SetText("");
				this.mTimeRect = new Rectangle(16, 16, 0, 0);
				this.mFont = iFont;
				if (TimedObjectiveRuleset.RenderData.sVertexBuffer == null)
				{
					TimedObjectiveRuleset.RenderData.sIndexBuffer = new IndexBuffer(Game.Instance.GraphicsDevice, 108, BufferUsage.None, IndexElementSize.SixteenBits);
					TimedObjectiveRuleset.RenderData.sVertexBuffer = new VertexBuffer(Game.Instance.GraphicsDevice, 16 * VertexPositionNormalTexture.SizeInBytes, BufferUsage.None);
					TimedObjectiveRuleset.RenderData.sVertexDeclaration = RenderManager.Instance.CreateVertexDeclaration(VertexPositionNormalTexture.VertexElements);
					lock (Game.Instance.GraphicsDevice)
					{
						TimedObjectiveRuleset.RenderData.sIndexBuffer.SetData<ushort>(TextBox.INDICES);
						TimedObjectiveRuleset.RenderData.sVertexBuffer.SetData<VertexPositionNormalTexture>(TextBox.VERTICES);
					}
				}
			}

			// Token: 0x06001916 RID: 6422 RVA: 0x000A5148 File Offset: 0x000A3348
			public void ResetRect()
			{
				this.mTimeRect = new Rectangle(16, 16, 0, 0);
			}

			// Token: 0x06001917 RID: 6423 RVA: 0x000A515B File Offset: 0x000A335B
			public void SetTime(string iText)
			{
				this.mTimeTextString = iText;
			}

			// Token: 0x06001918 RID: 6424 RVA: 0x000A5164 File Offset: 0x000A3364
			public void Draw(float iDeltaTime)
			{
				if (this.mTimeTextString != null)
				{
					this.mTimeText.SetText(this.mTimeTextString);
					Vector2 vector = this.mFont.MeasureText(this.mTimeTextString, true);
					this.mTimeRect.Width = (int)vector.X;
					this.mTimeRect.Height = (int)vector.Y;
					this.mTimeRect.X = this.mTimeRect.Width / 2 + this.mBorderSize;
					this.mTimeRect.Y = this.mTimeRect.Height / 2 + this.mBorderSize;
					this.mTimeTextString = null;
				}
				Point screenSize = RenderManager.Instance.ScreenSize;
				this.mTextBoxEffect.GraphicsDevice.Indices = TimedObjectiveRuleset.RenderData.sIndexBuffer;
				this.mTextBoxEffect.GraphicsDevice.Vertices[0].SetSource(TimedObjectiveRuleset.RenderData.sVertexBuffer, 0, VertexPositionNormalTexture.SizeInBytes);
				this.mTextBoxEffect.GraphicsDevice.VertexDeclaration = TimedObjectiveRuleset.RenderData.sVertexDeclaration;
				this.mTextBoxEffect.Position = new Vector2((float)this.mTimeRect.X, (float)this.mTimeRect.Y);
				this.mTextBoxEffect.Size = new Vector2((float)this.mTimeRect.Width, (float)this.mTimeRect.Height);
				this.mTextBoxEffect.Texture = this.mTimeTexture;
				this.mTextBoxEffect.Begin();
				this.mTextBoxEffect.CurrentTechnique.Passes[0].Begin();
				this.mTextBoxEffect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 36, 0, 18);
				this.mTextBoxEffect.CurrentTechnique.Passes[0].End();
				this.mTextBoxEffect.End();
				this.mEffect.SetScreenSize(screenSize.X, screenSize.Y);
				this.mEffect.Begin();
				this.mEffect.CurrentTechnique.Passes[0].Begin();
				this.mTimeText.Draw(this.mEffect, (float)(this.mTimeRect.X - this.mTimeRect.Width / 2), (float)(this.mTimeRect.Y - this.mTimeRect.Height / 2));
				this.mEffect.CurrentTechnique.Passes[0].End();
				this.mEffect.End();
			}

			// Token: 0x17000643 RID: 1603
			// (get) Token: 0x06001919 RID: 6425 RVA: 0x000A53D5 File Offset: 0x000A35D5
			public int ZIndex
			{
				get
				{
					return 205;
				}
			}

			// Token: 0x04001AD6 RID: 6870
			private static IndexBuffer sIndexBuffer;

			// Token: 0x04001AD7 RID: 6871
			private static VertexBuffer sVertexBuffer;

			// Token: 0x04001AD8 RID: 6872
			private static VertexDeclaration sVertexDeclaration;

			// Token: 0x04001AD9 RID: 6873
			private TextBoxEffect mTextBoxEffect;

			// Token: 0x04001ADA RID: 6874
			private GUIBasicEffect mEffect;

			// Token: 0x04001ADB RID: 6875
			private Rectangle mTimeRect;

			// Token: 0x04001ADC RID: 6876
			private string mTimeTextString;

			// Token: 0x04001ADD RID: 6877
			private Text mTimeText;

			// Token: 0x04001ADE RID: 6878
			private Texture2D mTimeTexture;

			// Token: 0x04001ADF RID: 6879
			private int mBorderSize;

			// Token: 0x04001AE0 RID: 6880
			private BitmapFont mFont;
		}

		// Token: 0x02000335 RID: 821
		internal struct Objective
		{
			// Token: 0x04001AE1 RID: 6881
			public int CounterID;

			// Token: 0x04001AE2 RID: 6882
			public int NameID;

			// Token: 0x04001AE3 RID: 6883
			public int Value;

			// Token: 0x04001AE4 RID: 6884
			public int MaxValue;

			// Token: 0x04001AE5 RID: 6885
			public int Score;
		}

		// Token: 0x02000336 RID: 822
		private struct TimeBonus
		{
			// Token: 0x0600191A RID: 6426 RVA: 0x000A53DC File Offset: 0x000A35DC
			public static float GetBonus(ref TimedObjectiveRuleset.TimeBonus iBonus, float iTime)
			{
				if (iTime > iBonus.MaxTime)
				{
					iTime = iBonus.MaxTime;
				}
				else if (iBonus.MinTime > iTime)
				{
					iTime = iBonus.MinTime;
				}
				float amount = 1f - (iTime - iBonus.MinTime) / (iBonus.MaxTime - iBonus.MinTime);
				return MathHelper.Lerp(iBonus.MaxTimeBonus, iBonus.MinTimeBonus, amount);
			}

			// Token: 0x04001AE6 RID: 6886
			public int CounterID;

			// Token: 0x04001AE7 RID: 6887
			public float MinTime;

			// Token: 0x04001AE8 RID: 6888
			public float MaxTime;

			// Token: 0x04001AE9 RID: 6889
			public float MinTimeBonus;

			// Token: 0x04001AEA RID: 6890
			public float MaxTimeBonus;
		}
	}
}
