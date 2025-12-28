using System;
using System.Collections.Generic;
using System.Xml;
using JigLibX.Geometry;
using Magicka.GameLogic;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.Entities.CharacterStates;
using Magicka.GameLogic.Entities.Items;
using Magicka.GameLogic.Spells;
using Magicka.GameLogic.Statistics;
using Magicka.GameLogic.UI;
using Magicka.Graphics;
using Magicka.Graphics.Effects;
using Magicka.Levels.Packs;
using Magicka.Levels.Triggers;
using Magicka.Localization;
using Magicka.Network;
using Magicka.PathFinding;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;

namespace Magicka.Levels
{
	// Token: 0x020000AF RID: 175
	public class SurvivalRuleset : IRuleset
	{
		// Token: 0x06000519 RID: 1305 RVA: 0x0001C6DC File Offset: 0x0001A8DC
		public SurvivalRuleset(GameScene iScene, XmlNode iNode)
		{
			for (int i = 0; i < 16; i++)
			{
				this.mScrollTexts[i] = new Text(200, SurvivalRuleset.SCROLL_FONT, TextAlign.Left, true);
				this.mScrollTexts[i].DrawShadows = true;
				this.mScrollTexts[i].ShadowAlpha = 1f;
				this.mScrollTexts[i].ShadowsOffset = new Vector2(1f, 1f);
				this.mScrollScores[i].Kill();
			}
			this.mNrOfActiveTexts = 0;
			this.mMagickTemplate = iScene.PlayState.Content.Load<CharacterTemplate>("Data/Characters/Luggage_magick");
			this.mItemTemplate = iScene.PlayState.Content.Load<CharacterTemplate>("Data/Characters/Luggage_item");
			this.mMagickConditions = new ConditionCollection();
			this.mItemConditions = new ConditionCollection();
			this.mGameScene = iScene;
			ItemPack[] itemPacks = PackMan.Instance.ItemPacks;
			for (int j = 0; j < itemPacks.Length; j++)
			{
				if (itemPacks[j].Enabled)
				{
					string[] items = itemPacks[j].Items;
					for (int k = 0; k < items.Length; k++)
					{
						this.mGameScene.PlayState.Content.Load<Item>(items[k]);
					}
				}
			}
			this.mLuggageItems = new List<int>(8);
			for (int l = 0; l < itemPacks.Length; l++)
			{
				if (itemPacks[l].Enabled)
				{
					this.mLuggageItems.AddRange(itemPacks[l].ItemIDs);
				}
			}
			MagickPack[] magickPacks = PackMan.Instance.MagickPacks;
			this.mLuggageMagicks = new List<MagickType>(8);
			for (int m = 0; m < magickPacks.Length; m++)
			{
				if (magickPacks[m].Enabled)
				{
					this.mLuggageMagicks.AddRange(magickPacks[m].Magicks);
				}
			}
			if (iNode.Name.Equals("Ruleset", StringComparison.OrdinalIgnoreCase))
			{
				for (int n = 0; n < iNode.Attributes.Count; n++)
				{
					if (iNode.Attributes[n].Name.Equals("waves", StringComparison.OrdinalIgnoreCase))
					{
						this.mTotalWavesNr = int.Parse(iNode.Attributes[n].Value);
					}
				}
			}
			this.mAreas = new List<string>();
			for (int num = 0; num < iNode.ChildNodes.Count; num++)
			{
				XmlNode xmlNode = iNode.ChildNodes[num];
				if (xmlNode.Name.Equals("spawnAreas", StringComparison.OrdinalIgnoreCase))
				{
					int count = xmlNode.ChildNodes.Count;
					for (int num2 = 0; num2 < count; num2++)
					{
						XmlNode xmlNode2 = xmlNode.ChildNodes[num2];
						this.mAreas.Add(xmlNode2.InnerText);
					}
				}
			}
			this.mWaves = new List<Wave>(this.mTotalWavesNr);
			PieEffect pieEffect = null;
			GUIBasicEffect guibasicEffect;
			Texture2D texture2D;
			lock (Game.Instance.GraphicsDevice)
			{
				guibasicEffect = new GUIBasicEffect(Game.Instance.GraphicsDevice, null);
				pieEffect = new PieEffect(Game.Instance.GraphicsDevice, Game.Instance.Content);
				texture2D = Game.Instance.Content.Load<Texture2D>("UI/HUD/hud");
			}
			Point screenSize = RenderManager.Instance.ScreenSize;
			int x = screenSize.X;
			int num3 = x / 2;
			int num4 = (int)(0.8f * (float)x);
			int num5 = num4 / 2;
			this.mHudBarPosition.X = (float)(num3 - num5);
			this.mHudBarPosition.Y = 32f;
			VertexPositionTexture[] array = new VertexPositionTexture[24];
			float x2 = 512f;
			float y = 64f;
			float num6 = 256f / (float)texture2D.Height;
			float num7 = 64f / (float)texture2D.Height;
			float num8 = 512f / (float)texture2D.Width;
			array[0].Position.X = 0f;
			array[0].Position.Y = y;
			array[0].TextureCoordinate.X = 0f;
			array[0].TextureCoordinate.Y = num6 + num7;
			array[1].Position.X = 0f;
			array[1].Position.Y = 0f;
			array[1].TextureCoordinate.X = 0f;
			array[1].TextureCoordinate.Y = num6;
			array[2].Position.X = x2;
			array[2].Position.Y = 0f;
			array[2].TextureCoordinate.X = num8;
			array[2].TextureCoordinate.Y = num6;
			array[3].Position.X = x2;
			array[3].Position.Y = 0f;
			array[3].TextureCoordinate.X = num8;
			array[3].TextureCoordinate.Y = num6;
			array[4].Position.X = x2;
			array[4].Position.Y = y;
			array[4].TextureCoordinate.X = num8;
			array[4].TextureCoordinate.Y = num6 + num7;
			array[5].Position.X = 0f;
			array[5].Position.Y = y;
			array[5].TextureCoordinate.X = 0f;
			array[5].TextureCoordinate.Y = num6 + num7;
			num6 = 320f / (float)texture2D.Height;
			float num9 = 128f / (float)texture2D.Width;
			num7 = 32f / (float)texture2D.Height;
			num8 = 96f / (float)texture2D.Width;
			array[6].Position.X = 0f;
			array[6].Position.Y = 32f;
			array[6].TextureCoordinate.X = num9;
			array[6].TextureCoordinate.Y = num6 + num7;
			array[7].Position.X = 0f;
			array[7].Position.Y = 0f;
			array[7].TextureCoordinate.X = num9;
			array[7].TextureCoordinate.Y = num6;
			array[8].Position.X = 96f;
			array[8].Position.Y = 0f;
			array[8].TextureCoordinate.X = num9 + num8;
			array[8].TextureCoordinate.Y = num6;
			array[9].Position.X = 96f;
			array[9].Position.Y = 0f;
			array[9].TextureCoordinate.X = num9 + num8;
			array[9].TextureCoordinate.Y = num6;
			array[10].Position.X = 96f;
			array[10].Position.Y = 32f;
			array[10].TextureCoordinate.X = num9 + num8;
			array[10].TextureCoordinate.Y = num6 + num7;
			array[11].Position.X = 0f;
			array[11].Position.Y = 32f;
			array[11].TextureCoordinate.X = num9;
			array[11].TextureCoordinate.Y = num6 + num7;
			array[12].Position.X = 0f;
			array[12].Position.Y = 32f;
			array[12].TextureCoordinate.X = 0f;
			array[12].TextureCoordinate.Y = num6 + num7;
			array[13].Position.X = 0f;
			array[13].Position.Y = 0f;
			array[13].TextureCoordinate.X = 0f;
			array[13].TextureCoordinate.Y = num6;
			array[14].Position.X = 112f;
			array[14].Position.Y = 0f;
			array[14].TextureCoordinate.X = num8;
			array[14].TextureCoordinate.Y = num6;
			array[15].Position.X = 112f;
			array[15].Position.Y = 0f;
			array[15].TextureCoordinate.X = num8;
			array[15].TextureCoordinate.Y = num6;
			array[16].Position.X = 112f;
			array[16].Position.Y = 32f;
			array[16].TextureCoordinate.X = num8;
			array[16].TextureCoordinate.Y = num6 + num7;
			array[17].Position.X = 0f;
			array[17].Position.Y = 32f;
			array[17].TextureCoordinate.X = 0f;
			array[17].TextureCoordinate.Y = num6 + num7;
			num9 = 256f / (float)texture2D.Width;
			num7 = 32f / (float)texture2D.Height;
			num8 = 208f / (float)texture2D.Width;
			array[18].Position.X = 0f;
			array[18].Position.Y = 32f;
			array[18].TextureCoordinate.X = num9;
			array[18].TextureCoordinate.Y = num6 + num7;
			array[19].Position.X = 0f;
			array[19].Position.Y = 0f;
			array[19].TextureCoordinate.X = num9;
			array[19].TextureCoordinate.Y = num6;
			array[20].Position.X = 208f;
			array[20].Position.Y = 0f;
			array[20].TextureCoordinate.X = num9 + num8;
			array[20].TextureCoordinate.Y = num6;
			array[21].Position.X = 208f;
			array[21].Position.Y = 0f;
			array[21].TextureCoordinate.X = num9 + num8;
			array[21].TextureCoordinate.Y = num6;
			array[22].Position.X = 208f;
			array[22].Position.Y = 32f;
			array[22].TextureCoordinate.X = num9 + num8;
			array[22].TextureCoordinate.Y = num6 + num7;
			array[23].Position.X = 0f;
			array[23].Position.Y = 32f;
			array[23].TextureCoordinate.X = num9;
			array[23].TextureCoordinate.Y = num6 + num7;
			VertexPositionTexture[] array2 = new VertexPositionTexture[33];
			float num10 = 32f;
			Vector2 vector;
			vector.X = 32f / (float)texture2D.Width;
			vector.Y = 384f / (float)texture2D.Height;
			array2[0].Position.X = 0f;
			array2[0].Position.Y = 0f;
			array2[0].TextureCoordinate.X = vector.X;
			array2[0].TextureCoordinate.Y = vector.Y;
			pieEffect.Radius = 20f;
			pieEffect.MaxAngle = 6.2831855f;
			pieEffect.SetTechnique(PieEffect.Technique.Technique1);
			pieEffect.Texture = texture2D;
			pieEffect.SetScreenSize(screenSize.X, screenSize.Y);
			for (int num11 = 1; num11 < 33; num11++)
			{
				float num12 = (float)(num11 - 1) / 31f;
				float num13 = (float)Math.Cos((double)((num12 - 0.25f) * 6.2831855f));
				float num14 = (float)Math.Sin((double)((num12 - 0.25f) * 6.2831855f));
				array2[num11].Position.X = 1f;
				array2[num11].Position.Y = num12;
				array2[num11].TextureCoordinate.X = vector.X + -num13 * num10 / (float)texture2D.Width;
				array2[num11].TextureCoordinate.Y = vector.Y + num14 * num10 / (float)texture2D.Height;
			}
			VertexDeclaration iVertexDeclaration;
			VertexBuffer vertexBuffer;
			VertexDeclaration iPieVertexDeclaration;
			VertexBuffer vertexBuffer2;
			lock (Game.Instance.GraphicsDevice)
			{
				iVertexDeclaration = new VertexDeclaration(Game.Instance.GraphicsDevice, VertexPositionTexture.VertexElements);
				vertexBuffer = new VertexBuffer(Game.Instance.GraphicsDevice, array.Length * VertexPositionTexture.SizeInBytes, BufferUsage.WriteOnly);
				vertexBuffer.SetData<VertexPositionTexture>(array);
				iPieVertexDeclaration = new VertexDeclaration(Game.Instance.GraphicsDevice, VertexPositionTexture.VertexElements);
				vertexBuffer2 = new VertexBuffer(Game.Instance.GraphicsDevice, array2.Length * VertexPositionTexture.SizeInBytes, BufferUsage.WriteOnly);
				vertexBuffer2.SetData<VertexPositionTexture>(array2);
			}
			guibasicEffect.TextureEnabled = true;
			guibasicEffect.Texture = texture2D;
			guibasicEffect.Color = new Vector4(1f);
			guibasicEffect.SetScreenSize(screenSize.X, screenSize.Y);
			this.mRenderData = new SurvivalRuleset.RenderData[3];
			for (int num15 = 0; num15 < 3; num15++)
			{
				this.mRenderData[num15] = new SurvivalRuleset.RenderData(guibasicEffect, vertexBuffer, iVertexDeclaration, VertexPositionTexture.SizeInBytes, texture2D, pieEffect, vertexBuffer2, iPieVertexDeclaration);
				this.mRenderData[num15].Texts = this.mScrollTexts;
			}
		}

		// Token: 0x0600051A RID: 1306 RVA: 0x0001D6A0 File Offset: 0x0001B8A0
		internal void ReadWave(XmlNode iNode)
		{
			Wave wave = new Wave(this.mGameScene);
			wave.Read(iNode);
			this.mWaves.Add(wave);
		}

		// Token: 0x0600051B RID: 1307 RVA: 0x0001D6CC File Offset: 0x0001B8CC
		public void Initialize()
		{
			for (int i = 0; i < this.mWaves.Count; i++)
			{
				this.mWaves[i].Initialize(this);
			}
			this.mHealthBar = new BossHealthBar(this.mGameScene.PlayState.Scene);
			this.mWaveIndicator = new WaveIndicator();
			this.mCurrentWave = this.mWaves[0];
			this.mCurrentWaveNr = 1;
			this.mTotalWavesNr = this.mWaves.Count + 1;
			this.mWaveIndicator.SetWave(this.mCurrentWaveNr);
			this.mLevelChangeCountDown = SurvivalRuleset.TIMEBETWEENLEVELS;
			this.mInitialized = true;
			this.mLuggageTimer = 0f;
			this.mCurrentTotalScore = 0;
			this.mCurrentTotalScoreFloat = 0f;
			this.mTotalScore = 0;
			this.mWaveTime = 0f;
			this.mTimeMultiplier = 5;
			this.mTotalDamageStartAmount = 0f;
			this.mDamageMultiplier = 1;
			this.mNrOfSpawnedTreats = 0;
			this.mNewTotalMultiplier = true;
			this.mTotalMultiplier = 6;
			this.mPercentage = 0f;
			StatisticsManager.Instance.SurvivalReset();
			this.mLuggageTimer = SurvivalRuleset.TIMEBETWEENLUGGAGE * 0.25f;
			this.mPlayersAliveLastUpdate = Game.Instance.PlayerCount;
			this.mPlayers = Game.Instance.Players;
			if (this.mGameScene.Level.CurrentScene.Indoors)
			{
				for (int j = 0; j < this.mLuggageMagicks.Count; j++)
				{
					if (this.mLuggageMagicks[j] == MagickType.Blizzard | this.mLuggageMagicks[j] == MagickType.MeteorS | this.mLuggageMagicks[j] == MagickType.Napalm | this.mLuggageMagicks[j] == MagickType.Rain | this.mLuggageMagicks[j] == MagickType.SPhoenix | this.mLuggageMagicks[j] == MagickType.ThunderB | this.mLuggageMagicks[j] == MagickType.ThunderS)
					{
						this.mLuggageMagicks.RemoveAt(j--);
					}
				}
			}
		}

		// Token: 0x0600051C RID: 1308 RVA: 0x0001D8D6 File Offset: 0x0001BAD6
		public void DeInitialize()
		{
		}

		// Token: 0x170000D3 RID: 211
		// (get) Token: 0x0600051D RID: 1309 RVA: 0x0001D8D8 File Offset: 0x0001BAD8
		public int ScoreMultiplier
		{
			get
			{
				return this.mTimeMultiplier * this.mDamageMultiplier;
			}
		}

		// Token: 0x0600051E RID: 1310 RVA: 0x0001D8E8 File Offset: 0x0001BAE8
		public void AddScore(int iName, int iScore)
		{
			if (iScore == 0)
			{
				return;
			}
			for (int i = 0; i < this.mScoreQueue.Keys.Count; i++)
			{
				if (this.mScoreQueue.Keys[i].NameID == iName)
				{
					SurvivalRuleset.ScrollScore key = this.mScoreQueue.Keys[i];
					key.MultiKills++;
					float value = this.mScoreQueue.Values[i];
					this.mScoreQueue.RemoveAt(i);
					this.mScoreQueue.Add(key, value);
					return;
				}
			}
			SurvivalRuleset.ScrollScore key2 = default(SurvivalRuleset.ScrollScore);
			key2.Score = iScore;
			key2.NameID = iName;
			key2.MultiKills = 1;
			this.mScoreQueue.Add(key2, 0.5f);
		}

		// Token: 0x0600051F RID: 1311 RVA: 0x0001D9AC File Offset: 0x0001BBAC
		protected bool AddScrollScore(ref SurvivalRuleset.ScrollScore iScore)
		{
			for (int i = 0; i < this.mScrollScores.Length; i++)
			{
				if (this.mScrollScores[i].Dead)
				{
					iScore.Position = (float)(this.mNrOfActiveTexts + 5);
					iScore.TargetPosition = 0f;
					this.mScrollScores[i] = iScore;
					this.mScrollTexts[i].Clear();
					string @string = LanguageManager.Instance.GetString(iScore.NameID);
					this.mScrollTexts[i].Append(@string);
					if (iScore.MultiKills > 1)
					{
						this.mScrollTexts[i].Append(SurvivalRuleset.SCORE_SPACING);
						this.mScrollTexts[i].Append(SurvivalRuleset.SCORE_MULTIPLY);
						this.mScrollTexts[i].Append(iScore.MultiKills);
					}
					this.mScrollTexts[i].Append(SurvivalRuleset.SCORE_SPACING);
					if (iScore.Score > 0)
					{
						this.mScrollTexts[i].Append(SurvivalRuleset.SCORE_POSITIVE);
					}
					this.mScrollTexts[i].Append(iScore.Score);
					this.mNrOfActiveTexts++;
					this.mTotalScore += iScore.Score * iScore.MultiKills;
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000520 RID: 1312 RVA: 0x0001DAF0 File Offset: 0x0001BCF0
		public void Update(float iDeltaTime, DataChannel iDataChan)
		{
			if (!this.mInitialized || iDataChan == DataChannel.None)
			{
				return;
			}
			NetworkState state = NetworkManager.Instance.State;
			if (state == NetworkState.Server)
			{
				if (this.mNetworkTimer <= 0f)
				{
					this.mNetworkTimer = 1f;
					this.NetworkUpdate();
				}
				this.mNetworkTimer -= iDeltaTime;
			}
			if (this.mCurrentWave != null)
			{
				this.mWaveTime += iDeltaTime;
				this.mCurrentWave.Update(iDeltaTime, this);
				TriggerArea triggerArea = this.mGameScene.PlayState.Level.CurrentScene.GetTriggerArea(TriggerArea.ANYID);
				int count = triggerArea.GetCount(this.mItemTemplate.ID);
				int count2 = triggerArea.GetCount(this.mMagickTemplate.ID);
				Factions iFaction = Factions.EVIL | Factions.WILD | Factions.DEMON;
				if (this.mCurrentWave.HasStarted() && this.mCurrentWave.IsDone() && triggerArea.GetFactionCount(iFaction) <= count + count2)
				{
					if (this.mLevelChangeCountDown <= 0f)
					{
						if (this.mCurrentWaveNr + 1 > this.mWaves.Count)
						{
							if (!this.mGameScene.PlayState.IsGameEnded)
							{
								if (NetworkManager.Instance.State == NetworkState.Server)
								{
									GameEndMessage gameEndMessage;
									gameEndMessage.Condition = EndGameCondition.Victory;
									gameEndMessage.Phony = false;
									gameEndMessage.DelayTime = 0f;
									gameEndMessage.Argument = 1;
									NetworkManager.Instance.Interface.SendMessage<GameEndMessage>(ref gameEndMessage);
								}
								this.mGameScene.PlayState.Endgame(EndGameCondition.Victory, true, false, 0f);
							}
						}
						else
						{
							this.mCurrentWave = this.mWaves[this.mCurrentWaveNr++];
							this.mWaveIndicator.SetWave(this.mCurrentWaveNr);
							this.mLevelChangeCountDown = SurvivalRuleset.TIMEBETWEENLEVELS;
							this.mNrOfSpawnedTreats = 0;
							this.mTimeMultiplier = 5;
							this.mWaveTime = 0f;
						}
					}
					else
					{
						this.mLevelChangeCountDown -= iDeltaTime;
					}
				}
			}
			this.mHealthBar.SetNormalizedHealth(this.mCurrentWave.HitPointPercentage());
			this.mHealthBar.Update(iDataChan, iDeltaTime);
			this.mWaveIndicator.Update(iDeltaTime, iDataChan, this.mGameScene.Scene);
			this.mLuggageTimer -= iDeltaTime;
			if (this.mLuggageTimer <= 0f && this.mNrOfSpawnedTreats < 2)
			{
				TriggerArea triggerArea2 = this.mGameScene.PlayState.Level.CurrentScene.GetTriggerArea(TriggerArea.ANYID);
				int count3 = triggerArea2.GetCount(this.mItemTemplate.ID);
				int count4 = triggerArea2.GetCount(this.mMagickTemplate.ID);
				if (count3 == 0 && count4 == 0)
				{
					if (this.mNrOfSpawnedTreats == 0)
					{
						this.SpawnLuggage(false);
					}
					else
					{
						this.SpawnLuggage(true);
					}
					this.mNrOfSpawnedTreats++;
					this.mLuggageTimer += SurvivalRuleset.TIMEBETWEENLUGGAGE;
				}
				else
				{
					this.mLuggageTimer += SurvivalRuleset.TIMEBETWEENLUGGAGE * 0.5f;
				}
			}
			SurvivalRuleset.RenderData renderData = this.mRenderData[(int)iDataChan];
			for (int i = 0; i < this.mScoreQueue.Count; i++)
			{
				float num = this.mScoreQueue.Values[i] - iDeltaTime;
				if (num <= 0f)
				{
					SurvivalRuleset.ScrollScore scrollScore = this.mScoreQueue.Keys[i];
					if (this.AddScrollScore(ref scrollScore))
					{
						this.mScoreQueue.RemoveAt(i);
					}
					else
					{
						num += 0.25f;
					}
				}
				else
				{
					this.mScoreQueue[this.mScoreQueue.Keys[i]] = num;
				}
			}
			for (int j = 0; j < this.mScrollScores.Length; j++)
			{
				if (!this.mScrollScores[j].Dead && this.mScrollScores[j].Update(iDeltaTime))
				{
					this.mNrOfActiveTexts--;
				}
			}
			this.mScrollScores.CopyTo(renderData.mScrollScore, 0);
			float num2 = (float)StatisticsManager.Instance.SurvivalTotalScore;
			this.mCurrentTotalScoreFloat += (num2 - (float)this.mCurrentTotalScore) * iDeltaTime * 5f;
			this.mCurrentTotalScore = (int)this.mCurrentTotalScoreFloat;
			renderData.SetTotalScore(this.mCurrentTotalScore);
			int num3 = Game.Instance.PlayerCount;
			Player[] players = Game.Instance.Players;
			for (int k = 0; k < players.Length; k++)
			{
				if (players[k].Playing && (players[k].Avatar == null || players[k].Avatar.Dead))
				{
					num3--;
				}
			}
			if (num3 < this.mPlayersAliveLastUpdate)
			{
				this.mDamageMultiplier = 1;
				this.mTotalDamageStartAmount = StatisticsManager.Instance.SurvivalTotalDamage;
			}
			this.mPlayersAliveLastUpdate = num3;
			if (this.mTimeMultiplier > 1 && this.mWaveTime > 30f)
			{
				this.mWaveTime -= 30f;
				this.mTimeMultiplier--;
				this.mNewTotalMultiplier = true;
			}
			float num4 = StatisticsManager.Instance.SurvivalTotalDamage - this.mTotalDamageStartAmount;
			if (this.mDamageMultiplier < 5 && num4 > 10000f)
			{
				this.mTotalDamageStartAmount += 10000f;
				num4 -= 10000f;
				this.mDamageMultiplier++;
				this.mNewTotalMultiplier = true;
			}
			if (this.mNewTotalMultiplier)
			{
				this.mTotalMultiplier = this.mDamageMultiplier * this.mTimeMultiplier;
			}
			this.mCurrentDamageAmount += (num4 - this.mCurrentDamageAmount) * iDeltaTime * 5f;
			renderData.SetTotalMultiPlier(this.mTotalMultiplier, 0);
			renderData.SetSurvivalMultiplier(this.mDamageMultiplier, 0);
			renderData.SetTimeMultiplier(this.mTimeMultiplier, 0);
			if (this.mTimeMultiplier > 1)
			{
				renderData.NormalizedTime = 1f - this.mWaveTime / 30f;
			}
			else
			{
				renderData.NormalizedTime = 1f;
			}
			renderData.DamageAmount = Math.Min(this.mCurrentDamageAmount / 10000f, 1f);
			renderData.DamageBarPosition = 0f;
			renderData.BarPosition = this.mHudBarPosition;
			this.mGameScene.Scene.AddRenderableGUIObject(iDataChan, renderData);
		}

		// Token: 0x06000521 RID: 1313 RVA: 0x0001E124 File Offset: 0x0001C324
		public void LocalUpdate(float iDeltaTime, DataChannel iDataChan)
		{
			if (!this.mInitialized || iDataChan == DataChannel.None)
			{
				return;
			}
			this.mHealthBar.SetNormalizedHealth(this.mPercentage);
			this.mHealthBar.Update(iDataChan, iDeltaTime);
			this.mWaveIndicator.Update(iDeltaTime, iDataChan, this.mGameScene.Scene);
			SurvivalRuleset.RenderData renderData = this.mRenderData[(int)iDataChan];
			for (int i = 0; i < this.mScoreQueue.Count; i++)
			{
				float num = this.mScoreQueue.Values[i] - iDeltaTime;
				if (num <= 0f)
				{
					SurvivalRuleset.ScrollScore scrollScore = this.mScoreQueue.Keys[i];
					if (this.AddScrollScore(ref scrollScore))
					{
						this.mScoreQueue.RemoveAt(i);
					}
					else
					{
						num += 0.25f;
						this.mScoreQueue[this.mScoreQueue.Keys[i]] = num;
					}
				}
				else
				{
					this.mScoreQueue[this.mScoreQueue.Keys[i]] = num;
				}
			}
			for (int j = 0; j < this.mScrollScores.Length; j++)
			{
				if (!this.mScrollScores[j].Dead && this.mScrollScores[j].Update(iDeltaTime))
				{
					this.mNrOfActiveTexts--;
				}
			}
			this.mScrollScores.CopyTo(renderData.mScrollScore, 0);
			float num2 = (float)StatisticsManager.Instance.SurvivalTotalScore;
			this.mCurrentTotalScoreFloat += (num2 - (float)this.mCurrentTotalScore) * iDeltaTime * 5f;
			this.mCurrentTotalScore = (int)this.mCurrentTotalScoreFloat;
			renderData.SetTotalScore(this.mCurrentTotalScore);
			float num3 = StatisticsManager.Instance.SurvivalTotalDamage - this.mTotalDamageStartAmount;
			this.mCurrentDamageAmount += (num3 - this.mCurrentDamageAmount) * iDeltaTime * 5f;
			renderData.SetTotalMultiPlier(this.mTotalMultiplier, 0);
			renderData.SetSurvivalMultiplier(this.mDamageMultiplier, 0);
			renderData.SetTimeMultiplier(this.mTimeMultiplier, 0);
			if (this.mTimeMultiplier > 1)
			{
				renderData.NormalizedTime = 1f - this.mWaveTime / 30f;
			}
			else
			{
				renderData.NormalizedTime = 1f;
			}
			if (this.mNewTotalMultiplier)
			{
				this.mTotalMultiplier = this.mDamageMultiplier * this.mTimeMultiplier;
			}
			renderData.DamageAmount = Math.Min(this.mCurrentDamageAmount / 10000f, 1f);
			renderData.DamageBarPosition = 0f;
			renderData.BarPosition = this.mHudBarPosition;
			this.mGameScene.Scene.AddRenderableGUIObject(iDataChan, renderData);
		}

		// Token: 0x06000522 RID: 1314 RVA: 0x0001E3AE File Offset: 0x0001C5AE
		public void AddedCharacter(NonPlayerCharacter iChar, bool iItemEvent)
		{
			if ((iChar.Faction & (Factions.EVIL | Factions.WILD | Factions.DEMON)) != Factions.NONE)
			{
				this.mCurrentWave.TrackCharacter(iChar, iItemEvent);
			}
		}

		// Token: 0x06000523 RID: 1315 RVA: 0x0001E3C8 File Offset: 0x0001C5C8
		private void NetworkUpdate()
		{
			RulesetMessage rulesetMessage = default(RulesetMessage);
			rulesetMessage.Type = Rulesets.Survival;
			rulesetMessage.Float01 = this.mCurrentDamageAmount;
			rulesetMessage.Byte02 = (byte)this.mDamageMultiplier;
			rulesetMessage.Byte01 = (byte)this.mTimeMultiplier;
			rulesetMessage.Float02 = this.mTotalDamageStartAmount;
			rulesetMessage.PackedFloat = this.mCurrentWave.HitPointPercentage();
			rulesetMessage.Byte03 = (byte)this.mWaveIndicator.WaveNum;
			NetworkManager.Instance.Interface.SendMessage<RulesetMessage>(ref rulesetMessage);
		}

		// Token: 0x06000524 RID: 1316 RVA: 0x0001E454 File Offset: 0x0001C654
		void IRuleset.NetworkUpdate(ref RulesetMessage iMsg)
		{
			this.mCurrentDamageAmount = iMsg.Float01;
			if ((int)iMsg.Byte02 != this.mDamageMultiplier)
			{
				this.mNewTotalMultiplier = true;
			}
			this.mDamageMultiplier = (int)iMsg.Byte02;
			if ((int)iMsg.Byte01 != this.mTimeMultiplier)
			{
				this.mNewTotalMultiplier = true;
			}
			this.mTimeMultiplier = (int)iMsg.Byte01;
			this.mPercentage = iMsg.PackedFloat;
			this.mTotalDamageStartAmount = iMsg.Float02;
			if ((int)iMsg.Byte03 != this.mWaveIndicator.WaveNum)
			{
				this.mWaveIndicator.SetWave((int)iMsg.Byte03);
				this.mCurrentWaveNr = (int)iMsg.Byte03;
			}
		}

		// Token: 0x06000525 RID: 1317 RVA: 0x0001E4F8 File Offset: 0x0001C6F8
		protected void SpawnLuggage(bool iSpawnItem)
		{
			Camera camera = this.mGameScene.PlayState.Camera;
			Matrix matrix;
			camera.GetViewProjectionMatrix(Game.Instance.UpdatingDataChannel, out matrix);
			Matrix matrix2;
			Matrix.Invert(ref matrix, out matrix2);
			Vector4 vector = default(Vector4);
			vector.Z = 1f;
			vector.W = 1f;
			Microsoft.Xna.Framework.Ray ray = default(Microsoft.Xna.Framework.Ray);
			ray.Position = camera.Position;
			Microsoft.Xna.Framework.Plane plane = default(Microsoft.Xna.Framework.Plane);
			plane.Normal = Vector3.Up;
			plane.D = -(ray.Position.Y - MagickCamera.CAMERAOFFSET.Y);
			NavMesh navMesh = this.mGameScene.PlayState.Level.CurrentScene.NavMesh;
			for (int i = 0; i < 10; i++)
			{
				vector.X = ((float)SurvivalRuleset.RANDOM.NextDouble() - 0.5f) * 1.9f;
				vector.Y = ((float)SurvivalRuleset.RANDOM.NextDouble() - 0.5f) * 1.9f;
				Vector4 vector2;
				Vector4.Transform(ref vector, ref matrix2, out vector2);
				float num = 1f / vector2.W;
				ray.Direction.X = vector2.X * num;
				ray.Direction.Y = vector2.Y * num;
				ray.Direction.Z = vector2.Z * num;
				Vector3.Subtract(ref ray.Direction, ref ray.Position, out ray.Direction);
				float? num2;
				ray.Intersects(ref plane, out num2);
				Vector3.Multiply(ref ray.Direction, num2.Value, out ray.Direction);
				Vector3.Add(ref ray.Position, ref ray.Direction, out ray.Direction);
				Vector3 origin;
				navMesh.GetNearestPosition(ref ray.Direction, out origin, MovementProperties.Default);
				Vector4 vector3;
				Vector4.Transform(ref origin, ref matrix, out vector3);
				if (Math.Abs(vector3.X / vector3.W) < 1f && Math.Abs(vector3.X / vector3.W) < 1f)
				{
					Segment iSeg = default(Segment);
					iSeg.Origin = origin;
					iSeg.Delta.Y = -4f;
					float num3;
					Vector3 vector4;
					Vector3 vector5;
					if (this.mGameScene.PlayState.Level.CurrentScene.SegmentIntersect(out num3, out vector4, out vector5, iSeg))
					{
						CharacterTemplate characterTemplate;
						ConditionCollection conditionCollection;
						if (iSpawnItem && this.mLuggageItems.Count > 0)
						{
							characterTemplate = this.mItemTemplate;
							conditionCollection = this.mItemConditions;
							int iType = this.mLuggageItems[SurvivalRuleset.RANDOM.Next(this.mLuggageItems.Count)];
							conditionCollection[0].Clear();
							conditionCollection[0].Add(new EventStorage(new SpawnItemEvent(iType)));
							conditionCollection[1].Clear();
							conditionCollection[1].Add(new EventStorage(new SpawnItemEvent(iType)));
						}
						else
						{
							if (this.mLuggageMagicks.Count <= 0)
							{
								return;
							}
							characterTemplate = this.mMagickTemplate;
							conditionCollection = this.mMagickConditions;
							int num4 = 0;
							bool flag;
							MagickType magickType;
							do
							{
								flag = true;
								magickType = this.mLuggageMagicks[(int)(Math.Pow(SurvivalRuleset.RANDOM.NextDouble(), 2.0) * (double)this.mLuggageMagicks.Count)];
								num4++;
								if (magickType == MagickType.Corporealize)
								{
									flag = false;
								}
								if (!Helper.CheckMagickDLC(magickType))
								{
									flag = false;
								}
								ulong num5 = 1UL << (int)magickType;
								for (int j = 0; j < this.mPlayers.Length; j++)
								{
									if (this.mPlayers[j].Playing && (this.mPlayers[j].UnlockedMagicks & num5) != 0UL)
									{
										flag = false;
									}
								}
								if (num4 > 100)
								{
									flag = true;
									magickType = MagickType.Revive;
								}
							}
							while (!flag);
							conditionCollection[0].Clear();
							conditionCollection[0].Add(new EventStorage(new SpawnMagickEvent(magickType)));
							conditionCollection[1].Clear();
							conditionCollection[1].Add(new EventStorage(new SpawnMagickEvent(magickType)));
						}
						conditionCollection[0].Condition.Repeat = false;
						conditionCollection[0].Condition.Count = 1;
						conditionCollection[0].Condition.Activated = false;
						conditionCollection[0].Condition.EventConditionType = EventConditionType.Death;
						conditionCollection[1].Condition.Repeat = false;
						conditionCollection[1].Condition.Count = 1;
						conditionCollection[1].Condition.Activated = false;
						conditionCollection[1].Condition.EventConditionType = EventConditionType.OverKill;
						vector4.Y += characterTemplate.Length * 0.5f + characterTemplate.Radius;
						Matrix orientation;
						Matrix.CreateRotationY((float)SurvivalRuleset.RANDOM.NextDouble() * 6.2831855f, out orientation);
						NonPlayerCharacter instance = NonPlayerCharacter.GetInstance(this.mGameScene.PlayState);
						instance.Initialize(characterTemplate, vector4, 0);
						instance.Body.Orientation = orientation;
						instance.CharacterBody.DesiredDirection = orientation.Forward;
						instance.SpawnAnimation = Animations.spawn;
						instance.ChangeState(RessurectionState.Instance);
						this.mGameScene.PlayState.EntityManager.AddEntity(instance);
						instance.EventConditions = conditionCollection;
						orientation.Translation = vector4;
						VisualEffectReference visualEffectReference;
						EffectManager.Instance.StartEffect("luggage_spawn".GetHashCodeCustom(), ref orientation, out visualEffectReference);
						instance.Faction = Factions.EVIL;
						if (NetworkManager.Instance.State == NetworkState.Server)
						{
							TriggerActionMessage triggerActionMessage = default(TriggerActionMessage);
							triggerActionMessage.ActionType = TriggerActionType.SpawnLuggage;
							triggerActionMessage.Handle = instance.Handle;
							triggerActionMessage.Template = instance.Template.ID;
							triggerActionMessage.Position = instance.Position;
							triggerActionMessage.Direction = instance.Direction;
							triggerActionMessage.Point0 = 170;
							NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref triggerActionMessage);
						}
						return;
					}
				}
			}
		}

		// Token: 0x06000526 RID: 1318 RVA: 0x0001EB11 File Offset: 0x0001CD11
		public int GetAnyArea()
		{
			return this.mAreas[MagickaMath.Random.Next(this.mAreas.Count)].GetHashCodeCustom();
		}

		// Token: 0x170000D4 RID: 212
		// (get) Token: 0x06000527 RID: 1319 RVA: 0x0001EB38 File Offset: 0x0001CD38
		public int WaveIndex
		{
			get
			{
				return this.mCurrentWaveNr;
			}
		}

		// Token: 0x170000D5 RID: 213
		// (get) Token: 0x06000528 RID: 1320 RVA: 0x0001EB40 File Offset: 0x0001CD40
		// (set) Token: 0x06000529 RID: 1321 RVA: 0x0001EB48 File Offset: 0x0001CD48
		public Wave CurrentWave
		{
			get
			{
				return this.mCurrentWave;
			}
			set
			{
				this.mCurrentWave = value;
			}
		}

		// Token: 0x0600052A RID: 1322 RVA: 0x0001EB51 File Offset: 0x0001CD51
		public List<string> GetAreas()
		{
			return this.mAreas;
		}

		// Token: 0x170000D6 RID: 214
		// (get) Token: 0x0600052B RID: 1323 RVA: 0x0001EB59 File Offset: 0x0001CD59
		public Rulesets RulesetType
		{
			get
			{
				return Rulesets.Survival;
			}
		}

		// Token: 0x170000D7 RID: 215
		// (get) Token: 0x0600052C RID: 1324 RVA: 0x0001EB5C File Offset: 0x0001CD5C
		public bool IsVersusRuleset
		{
			get
			{
				return false;
			}
		}

		// Token: 0x040003AA RID: 938
		private const int PIEVERTEXCOUNT = 33;

		// Token: 0x040003AB RID: 939
		private const float SCORE_QUEUE_DELAY = 0.5f;

		// Token: 0x040003AC RID: 940
		private const float SCORE_TIME = 10f;

		// Token: 0x040003AD RID: 941
		private const float NETWORK_UPDATE_FREQ = 1f;

		// Token: 0x040003AE RID: 942
		private const float TIME_MULTIPLIER_DECAY = 30f;

		// Token: 0x040003AF RID: 943
		private const float DAMAGE_MULTIPLIER_CAP = 10000f;

		// Token: 0x040003B0 RID: 944
		private static readonly char[] SCORE_DECIMAL = new char[]
		{
			'.'
		};

		// Token: 0x040003B1 RID: 945
		private static readonly char[] SCORE_SPACING = new char[]
		{
			' '
		};

		// Token: 0x040003B2 RID: 946
		private static readonly char[] SCORE_POSITIVE = new char[]
		{
			'+'
		};

		// Token: 0x040003B3 RID: 947
		private static readonly char[] SCORE_MULTIPLY = new char[]
		{
			'x'
		};

		// Token: 0x040003B4 RID: 948
		private static readonly Random RANDOM = new Random();

		// Token: 0x040003B5 RID: 949
		private static readonly BitmapFont SCROLL_FONT = FontManager.Instance.GetFont(MagickaFont.Maiandra14);

		// Token: 0x040003B6 RID: 950
		private static readonly BitmapFont MULTI_FONT = FontManager.Instance.GetFont(MagickaFont.Maiandra16);

		// Token: 0x040003B7 RID: 951
		private static readonly BitmapFont TOTAL_FONT = FontManager.Instance.GetFont(MagickaFont.MenuDefault);

		// Token: 0x040003B8 RID: 952
		private static readonly Vector2 TIME_EFFECT_OFFSET = new Vector2(188f, 20f);

		// Token: 0x040003B9 RID: 953
		private static readonly Vector2 TIME_TEXT_OFFSET = new Vector2(188f, 20f);

		// Token: 0x040003BA RID: 954
		private static readonly Vector2 TOTAL_SCORE_OFFSET = new Vector2(461f, 16f);

		// Token: 0x040003BB RID: 955
		private static readonly Vector2 TOTAL_SCORE_OVERLAY = new Vector2(256f, 0f);

		// Token: 0x040003BC RID: 956
		private static readonly Vector2 TOTAL_MULTIPLIER_OFFSET = new Vector2(224f, 32f);

		// Token: 0x040003BD RID: 957
		private static readonly Vector2 DAMAGE_BAR_OFFSET = new Vector2(48f, 0f);

		// Token: 0x040003BE RID: 958
		private static readonly Vector2 DAMAGE_TEXT_OFFSET = new Vector2(110f, 2f);

		// Token: 0x040003BF RID: 959
		private SortedList<SurvivalRuleset.ScrollScore, float> mScoreQueue = new SortedList<SurvivalRuleset.ScrollScore, float>(64);

		// Token: 0x040003C0 RID: 960
		private SurvivalRuleset.ScrollScore[] mScrollScores = new SurvivalRuleset.ScrollScore[16];

		// Token: 0x040003C1 RID: 961
		private Text[] mScrollTexts = new Text[16];

		// Token: 0x040003C2 RID: 962
		private int mNrOfActiveTexts;

		// Token: 0x040003C3 RID: 963
		private SurvivalRuleset.RenderData[] mRenderData;

		// Token: 0x040003C4 RID: 964
		private Dictionary<int, string> mNames = new Dictionary<int, string>(64);

		// Token: 0x040003C5 RID: 965
		private float mNetworkTimer;

		// Token: 0x040003C6 RID: 966
		private int mCurrentTotalScore;

		// Token: 0x040003C7 RID: 967
		private float mCurrentTotalScoreFloat;

		// Token: 0x040003C8 RID: 968
		private int mTotalScore;

		// Token: 0x040003C9 RID: 969
		private float mWaveTime;

		// Token: 0x040003CA RID: 970
		private int mTimeMultiplier;

		// Token: 0x040003CB RID: 971
		private float mCurrentDamageAmount;

		// Token: 0x040003CC RID: 972
		private float mTotalDamageStartAmount;

		// Token: 0x040003CD RID: 973
		private int mDamageMultiplier;

		// Token: 0x040003CE RID: 974
		private bool mNewTotalMultiplier;

		// Token: 0x040003CF RID: 975
		private int mTotalMultiplier;

		// Token: 0x040003D0 RID: 976
		private int mPlayersAliveLastUpdate;

		// Token: 0x040003D1 RID: 977
		private Vector2 mHudBarPosition;

		// Token: 0x040003D2 RID: 978
		private int mNrOfSpawnedTreats;

		// Token: 0x040003D3 RID: 979
		private float mLuggageTimer;

		// Token: 0x040003D4 RID: 980
		private static readonly float TIMEBETWEENLUGGAGE = 30f;

		// Token: 0x040003D5 RID: 981
		private CharacterTemplate mMagickTemplate;

		// Token: 0x040003D6 RID: 982
		private CharacterTemplate mItemTemplate;

		// Token: 0x040003D7 RID: 983
		private ConditionCollection mMagickConditions;

		// Token: 0x040003D8 RID: 984
		private ConditionCollection mItemConditions;

		// Token: 0x040003D9 RID: 985
		private List<int> mLuggageItems;

		// Token: 0x040003DA RID: 986
		private List<MagickType> mLuggageMagicks;

		// Token: 0x040003DB RID: 987
		public static float TIMEBETWEENLEVELS = 5f;

		// Token: 0x040003DC RID: 988
		private bool mInitialized;

		// Token: 0x040003DD RID: 989
		private int mTotalWavesNr;

		// Token: 0x040003DE RID: 990
		private int mCurrentWaveNr;

		// Token: 0x040003DF RID: 991
		private float mLevelChangeCountDown;

		// Token: 0x040003E0 RID: 992
		public List<Wave> mWaves;

		// Token: 0x040003E1 RID: 993
		public List<string> mAreas;

		// Token: 0x040003E2 RID: 994
		private Wave mCurrentWave;

		// Token: 0x040003E3 RID: 995
		private GameScene mGameScene;

		// Token: 0x040003E4 RID: 996
		private WaveIndicator mWaveIndicator;

		// Token: 0x040003E5 RID: 997
		private BossHealthBar mHealthBar;

		// Token: 0x040003E6 RID: 998
		private float mPercentage;

		// Token: 0x040003E7 RID: 999
		private Player[] mPlayers;

		// Token: 0x020000B0 RID: 176
		protected class RenderData : IRenderableGUIObject
		{
			// Token: 0x0600052E RID: 1326 RVA: 0x0001EC90 File Offset: 0x0001CE90
			public RenderData(GUIBasicEffect iEffect, VertexBuffer iVertexBuffer, VertexDeclaration iVertexDeclaration, int iVertexStride, Texture2D iTexture, PieEffect iPieEffect, VertexBuffer iPieVertexBuffer, VertexDeclaration iPieVertexDeclaration)
			{
				this.mPieEffect = iPieEffect;
				this.mPieVertexBuffer = iPieVertexBuffer;
				this.mPieVertexDeclaration = iPieVertexDeclaration;
				this.mHudTexture = iTexture;
				this.mDamageMulText = new Text(10, SurvivalRuleset.MULTI_FONT, TextAlign.Right, false);
				this.mDamageMulText.Append(SurvivalRuleset.SCORE_MULTIPLY);
				this.mDamageMulText.Append(5);
				this.mDamageMulText.DrawShadows = true;
				this.mDamageMulText.ShadowAlpha = 1f;
				this.mDamageMulText.ShadowsOffset = new Vector2(1f, 1f);
				this.mTimeMulText = new Text(10, SurvivalRuleset.MULTI_FONT, TextAlign.Center, false);
				this.mTimeMulText.Append(SurvivalRuleset.SCORE_MULTIPLY);
				this.mTimeMulText.Append(5);
				this.mTimeMulText.DrawShadows = true;
				this.mTimeMulText.ShadowAlpha = 1f;
				this.mTimeMulText.ShadowsOffset = new Vector2(1f, 1f);
				this.mTotalScoreText = new Text(200, SurvivalRuleset.TOTAL_FONT, TextAlign.Right, true);
				this.mTotalScoreText.Append(0);
				this.mTotalScoreText.DrawShadows = true;
				this.mTotalScoreText.ShadowAlpha = 1f;
				this.mTotalScoreText.ShadowsOffset = new Vector2(1f, 1f);
				this.mTotalMultiPlierText = new Text(200, SurvivalRuleset.TOTAL_FONT, TextAlign.Center, false);
				this.mTotalMultiPlierText.Append(SurvivalRuleset.SCORE_MULTIPLY);
				this.mTotalMultiPlierText.Append(10);
				this.mTotalMultiPlierText.DrawShadows = true;
				this.mTotalMultiPlierText.ShadowAlpha = 1f;
				this.mTotalMultiPlierText.ShadowsOffset = new Vector2(1f, 1f);
				this.mTimeMulOffset = SurvivalRuleset.MULTI_FONT.MeasureText(this.mDamageMulText.Characters, true).X;
				this.mScrollOffset = (float)SurvivalRuleset.MULTI_FONT.LineHeight;
				this.mGUIBasicEffect = iEffect;
				this.mVertexBuffer = iVertexBuffer;
				this.mVertexDeclaration = iVertexDeclaration;
				this.mVertexStride = iVertexStride;
				this.mHalfLineHeight = (float)SurvivalRuleset.MULTI_FONT.LineHeight * 0.5f;
				this.mTotalLineHeight = (float)SurvivalRuleset.TOTAL_FONT.LineHeight * 0.5f;
			}

			// Token: 0x0600052F RID: 1327 RVA: 0x0001EEDC File Offset: 0x0001D0DC
			public void SetTotalMultiPlier(int iMul, int iDecimalMul)
			{
				if (iMul == this.mTotalMul && iDecimalMul == this.mTotalDecimalMul)
				{
					return;
				}
				this.mTotalDecimalMul = iDecimalMul;
				this.mTotalMul = iMul;
				this.mTotalDirty = true;
			}

			// Token: 0x06000530 RID: 1328 RVA: 0x0001EF06 File Offset: 0x0001D106
			public void SetSurvivalMultiplier(int iMul, int iDecimalMul)
			{
				if (iMul == this.mDamageMul && iDecimalMul == this.mDamageDecimalMul)
				{
					return;
				}
				this.mDamageDecimalMul = iDecimalMul;
				this.mDamageMul = iMul;
				this.mDamageDirty = true;
			}

			// Token: 0x06000531 RID: 1329 RVA: 0x0001EF30 File Offset: 0x0001D130
			public void SetTimeMultiplier(int iMul, int iDecimalMul)
			{
				if (iMul == this.mTimeMul && iDecimalMul == this.mTimeDecimalMul)
				{
					return;
				}
				this.mTimeDecimalMul = iDecimalMul;
				this.mTimeMul = iMul;
				this.mTimeDirty = true;
			}

			// Token: 0x06000532 RID: 1330 RVA: 0x0001EF5A File Offset: 0x0001D15A
			public void SetTotalScore(int iScore)
			{
				if (iScore == this.mTotalScore)
				{
					return;
				}
				this.mTotalScore = iScore;
				this.mTotalScoreDirty = true;
			}

			// Token: 0x06000533 RID: 1331 RVA: 0x0001EF74 File Offset: 0x0001D174
			public void Draw(float iDeltaTime)
			{
				if (this.mTotalDirty)
				{
					this.mTotalMultiPlierText.Clear();
					this.mTotalMultiPlierText.Append(this.mTotalMul);
					this.mTotalMultiPlierText.Append(SurvivalRuleset.SCORE_MULTIPLY);
					this.mTotalDirty = false;
				}
				if (this.mDamageDirty)
				{
					this.mDamageMulText.Clear();
					this.mDamageMulText.Append(this.mDamageMul);
					this.mDamageMulText.Append(SurvivalRuleset.SCORE_MULTIPLY);
					this.mDamageDirty = false;
				}
				if (this.mTimeDirty)
				{
					this.mTimeMulText.Clear();
					this.mTimeMulText.Append(this.mTimeMul);
					this.mTimeMulText.Append(SurvivalRuleset.SCORE_MULTIPLY);
					this.mTimeDirty = false;
				}
				if (this.mTotalScoreDirty)
				{
					this.mTotalScoreText.Clear();
					this.mTotalScoreText.Append(this.mTotalScore);
					this.mTotalScoreDirty = false;
				}
				Point screenSize = RenderManager.Instance.ScreenSize;
				float iScale = (float)screenSize.Y / 720f;
				Vector2 vector = new Vector2((float)Math.Floor((double)((float)screenSize.X * 0.05f + 0.5f)), (float)Math.Floor((double)((float)screenSize.Y * 0.05f + 0.5f)));
				this.mGUIBasicEffect.SetScreenSize(screenSize.X, screenSize.Y);
				this.mGUIBasicEffect.Color = Vector4.One;
				this.mGUIBasicEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertexBuffer, 0, this.mVertexStride);
				this.mGUIBasicEffect.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
				this.mGUIBasicEffect.Texture = this.mHudTexture;
				this.mGUIBasicEffect.TextureEnabled = true;
				Matrix identity = Matrix.Identity;
				identity.M41 = this.BarPosition.X;
				identity.M42 = this.BarPosition.Y;
				this.mGUIBasicEffect.Transform = identity;
				this.mGUIBasicEffect.Begin();
				this.mGUIBasicEffect.CurrentTechnique.Passes[0].Begin();
				this.mGUIBasicEffect.GraphicsDevice.DrawPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, 0, 2);
				identity.M11 = this.DamageAmount;
				identity.M41 = this.BarPosition.X + SurvivalRuleset.DAMAGE_BAR_OFFSET.X;
				identity.M42 = this.BarPosition.Y + SurvivalRuleset.DAMAGE_BAR_OFFSET.Y;
				this.mGUIBasicEffect.Transform = identity;
				this.mGUIBasicEffect.CommitChanges();
				this.mGUIBasicEffect.GraphicsDevice.DrawPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, 6, 2);
				identity.M11 = 1f;
				this.mGUIBasicEffect.Transform = identity;
				this.mGUIBasicEffect.CommitChanges();
				this.mGUIBasicEffect.GraphicsDevice.DrawPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, 12, 2);
				this.mGUIBasicEffect.CurrentTechnique.Passes[0].End();
				this.mGUIBasicEffect.End();
				this.mPieEffect.GraphicsDevice.RenderState.CullMode = CullMode.None;
				this.mPieEffect.GraphicsDevice.Vertices[0].SetSource(this.mPieVertexBuffer, 0, this.mVertexStride);
				this.mPieEffect.GraphicsDevice.VertexDeclaration = this.mPieVertexDeclaration;
				identity.M41 = this.BarPosition.X + SurvivalRuleset.TIME_EFFECT_OFFSET.X;
				identity.M42 = this.BarPosition.Y + SurvivalRuleset.TIME_EFFECT_OFFSET.Y;
				identity.M11 = 0f;
				identity.M12 = -1f;
				identity.M21 = -1f;
				identity.M22 = 0f;
				identity.M33 = -1f;
				this.mPieEffect.Transform = identity;
				this.mPieEffect.MaxAngle = this.NormalizedTime * 6.2831855f;
				this.mPieEffect.Begin();
				this.mPieEffect.CurrentTechnique.Passes[0].Begin();
				this.mPieEffect.GraphicsDevice.DrawPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleFan, 0, 31);
				this.mPieEffect.CurrentTechnique.Passes[0].End();
				this.mPieEffect.End();
				this.mPieEffect.GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;
				this.mGUIBasicEffect.Begin();
				this.mGUIBasicEffect.CurrentTechnique.Passes[0].Begin();
				float iX = this.BarPosition.X + SurvivalRuleset.TOTAL_SCORE_OFFSET.X;
				float iY = this.BarPosition.Y + SurvivalRuleset.TOTAL_SCORE_OFFSET.Y - this.mTotalLineHeight;
				this.mTotalScoreText.Draw(this.mGUIBasicEffect, iX, iY);
				iX = this.BarPosition.X + SurvivalRuleset.DAMAGE_TEXT_OFFSET.X;
				iY = this.BarPosition.Y + SurvivalRuleset.DAMAGE_TEXT_OFFSET.Y;
				this.mDamageMulText.Draw(this.mGUIBasicEffect, iX, iY);
				iX = this.BarPosition.X + SurvivalRuleset.TIME_TEXT_OFFSET.X;
				iY = this.BarPosition.Y + SurvivalRuleset.TIME_TEXT_OFFSET.Y - this.mHalfLineHeight;
				this.mTimeMulText.Draw(this.mGUIBasicEffect, iX, iY);
				iX = this.BarPosition.X + SurvivalRuleset.TOTAL_MULTIPLIER_OFFSET.X;
				iY = this.BarPosition.Y + SurvivalRuleset.TOTAL_MULTIPLIER_OFFSET.Y - this.mTotalLineHeight;
				this.mTotalMultiPlierText.Draw(this.mGUIBasicEffect, iX, iY);
				this.mGUIBasicEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertexBuffer, 0, this.mVertexStride);
				this.mGUIBasicEffect.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
				this.mGUIBasicEffect.Texture = this.mHudTexture;
				this.mGUIBasicEffect.TextureEnabled = true;
				identity = Matrix.Identity;
				identity.M41 = this.BarPosition.X + SurvivalRuleset.TOTAL_SCORE_OVERLAY.X;
				identity.M42 = this.BarPosition.Y + SurvivalRuleset.TOTAL_SCORE_OVERLAY.Y;
				this.mGUIBasicEffect.Color = Vector4.One;
				this.mGUIBasicEffect.Transform = identity;
				this.mGUIBasicEffect.CommitChanges();
				this.mGUIBasicEffect.GraphicsDevice.DrawPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, 18, 2);
				Vector4 one = Vector4.One;
				for (int i = 0; i < this.mScrollScore.Length; i++)
				{
					if (!this.mScrollScore[i].Dead)
					{
						one.W = this.mScrollScore[i].Alpha;
						this.mGUIBasicEffect.Color = one;
						this.Texts[i].Draw(this.mGUIBasicEffect, vector.X, vector.Y + this.mScrollOffset * 2f + this.mScrollScore[i].Position * 16f, iScale);
					}
				}
				this.mGUIBasicEffect.CurrentTechnique.Passes[0].End();
				this.mGUIBasicEffect.End();
			}

			// Token: 0x170000D8 RID: 216
			// (get) Token: 0x06000534 RID: 1332 RVA: 0x0001F6CA File Offset: 0x0001D8CA
			public int ZIndex
			{
				get
				{
					return 205;
				}
			}

			// Token: 0x040003E8 RID: 1000
			private PieEffect mPieEffect;

			// Token: 0x040003E9 RID: 1001
			private VertexBuffer mPieVertexBuffer;

			// Token: 0x040003EA RID: 1002
			private VertexDeclaration mPieVertexDeclaration;

			// Token: 0x040003EB RID: 1003
			private GUIBasicEffect mGUIBasicEffect;

			// Token: 0x040003EC RID: 1004
			private VertexBuffer mVertexBuffer;

			// Token: 0x040003ED RID: 1005
			private VertexDeclaration mVertexDeclaration;

			// Token: 0x040003EE RID: 1006
			private int mVertexStride;

			// Token: 0x040003EF RID: 1007
			private bool mDamageDirty;

			// Token: 0x040003F0 RID: 1008
			private int mDamageMul;

			// Token: 0x040003F1 RID: 1009
			private int mDamageDecimalMul;

			// Token: 0x040003F2 RID: 1010
			private Text mDamageMulText;

			// Token: 0x040003F3 RID: 1011
			public float DamageAmount;

			// Token: 0x040003F4 RID: 1012
			public float DamageBarPosition;

			// Token: 0x040003F5 RID: 1013
			public Vector2 BarPosition;

			// Token: 0x040003F6 RID: 1014
			private bool mTotalDirty;

			// Token: 0x040003F7 RID: 1015
			private int mTotalMul;

			// Token: 0x040003F8 RID: 1016
			private int mTotalDecimalMul;

			// Token: 0x040003F9 RID: 1017
			private Text mTotalMultiPlierText;

			// Token: 0x040003FA RID: 1018
			private bool mTimeDirty;

			// Token: 0x040003FB RID: 1019
			private int mTimeMul;

			// Token: 0x040003FC RID: 1020
			private int mTimeDecimalMul;

			// Token: 0x040003FD RID: 1021
			private Text mTimeMulText;

			// Token: 0x040003FE RID: 1022
			private float mTimeMulOffset;

			// Token: 0x040003FF RID: 1023
			public float NormalizedTime;

			// Token: 0x04000400 RID: 1024
			private bool mTotalScoreDirty;

			// Token: 0x04000401 RID: 1025
			private int mTotalScore;

			// Token: 0x04000402 RID: 1026
			private Text mTotalScoreText;

			// Token: 0x04000403 RID: 1027
			private Texture2D mHudTexture;

			// Token: 0x04000404 RID: 1028
			public SurvivalRuleset.ScrollScore[] mScrollScore = new SurvivalRuleset.ScrollScore[16];

			// Token: 0x04000405 RID: 1029
			public Text[] Texts;

			// Token: 0x04000406 RID: 1030
			private float mScrollOffset;

			// Token: 0x04000407 RID: 1031
			private float mHalfLineHeight;

			// Token: 0x04000408 RID: 1032
			private float mTotalLineHeight;
		}

		// Token: 0x020000B1 RID: 177
		public struct ScrollScore : IComparable<SurvivalRuleset.ScrollScore>
		{
			// Token: 0x170000D9 RID: 217
			// (get) Token: 0x06000535 RID: 1333 RVA: 0x0001F6D1 File Offset: 0x0001D8D1
			public float Alpha
			{
				get
				{
					return this.mAlpha;
				}
			}

			// Token: 0x06000536 RID: 1334 RVA: 0x0001F6D9 File Offset: 0x0001D8D9
			public void Kill()
			{
				this.Position = 0f;
			}

			// Token: 0x170000DA RID: 218
			// (get) Token: 0x06000537 RID: 1335 RVA: 0x0001F6E6 File Offset: 0x0001D8E6
			public bool Dead
			{
				get
				{
					return this.Position <= 0f;
				}
			}

			// Token: 0x06000538 RID: 1336 RVA: 0x0001F6F8 File Offset: 0x0001D8F8
			public bool Update(float iDeltaTime)
			{
				this.TimeAlive += iDeltaTime;
				this.Position -= iDeltaTime * 2f;
				this.mAlpha = MathHelper.Min(this.TimeAlive, 0.25f) * 4f;
				this.mAlpha *= MathHelper.Max(MathHelper.Min(0.25f, this.Position), 0f) * 4f;
				return this.Dead;
			}

			// Token: 0x06000539 RID: 1337 RVA: 0x0001F776 File Offset: 0x0001D976
			public int CompareTo(SurvivalRuleset.ScrollScore other)
			{
				if (this.NameID > other.NameID)
				{
					return 1;
				}
				if (this.NameID < other.NameID)
				{
					return -1;
				}
				return 0;
			}

			// Token: 0x04000409 RID: 1033
			private float TimeAlive;

			// Token: 0x0400040A RID: 1034
			public float Position;

			// Token: 0x0400040B RID: 1035
			public float TargetPosition;

			// Token: 0x0400040C RID: 1036
			public int Score;

			// Token: 0x0400040D RID: 1037
			public int NameID;

			// Token: 0x0400040E RID: 1038
			public int MultiKills;

			// Token: 0x0400040F RID: 1039
			private float mAlpha;
		}
	}
}
