using System;
using System.Xml;
using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Physics;
using Magicka.GameLogic;
using Magicka.GameLogic.GameStates.Menu;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using XNAnimation;
using XNAnimation.Controllers;
using XNAnimation.Effects;

namespace Magicka.Levels.Versus
{
	// Token: 0x02000382 RID: 898
	internal class King : VersusRuleset
	{
		// Token: 0x06001B67 RID: 7015 RVA: 0x000BCE0C File Offset: 0x000BB00C
		public King(GameScene iScene, XmlNode iNode, King.Settings iSettings) : base(iScene, iNode)
		{
			this.mSettings = iSettings;
			SkinnedModel skinnedModel;
			SkinnedModel skinnedModel2;
			Model model;
			lock (Game.Instance.GraphicsDevice)
			{
				skinnedModel = Game.Instance.Content.Load<SkinnedModel>("Models/Effects/earthbarrier_animation");
				skinnedModel2 = Game.Instance.Content.Load<SkinnedModel>("Models/Effects/earthbarrier4_mesh");
				model = Game.Instance.Content.Load<Model>("Models/Effects/torch");
			}
			Matrix orient = Matrix.CreateRotationX(-1.5707964f);
			this.mBody = new Body();
			this.mCollisionSkin = new CollisionSkin(this.mBody);
			this.mCollisionSkin.AddPrimitive(new Capsule(new Vector3(0f, 0.5f, 0f), orient, 0.5f, 2f), 1, new MaterialProperties(0f, 0f, 0f));
			this.mBody.CollisionSkin = this.mCollisionSkin;
			this.mBody.Mass = 50f;
			this.mBody.Immovable = true;
			this.mBody.Tag = this;
			this.mAnimationController = new AnimationController();
			this.mAnimationController.Skeleton = skinnedModel.SkeletonBones;
			this.mAnimationClip = skinnedModel.AnimationClips["emerge4"];
			SkinnedModelBasicEffect iEffect = skinnedModel2.Model.Meshes[0].MeshParts[0].Effect as SkinnedModelBasicEffect;
			SkinnedModelDeferredBasicMaterial mMaterial;
			Helper.SkinnedModelDeferredMaterialFromBasicEffect(iEffect, out mMaterial);
			this.mHillRenderData = new King.HillRenderData[3];
			for (int i = 0; i < 3; i++)
			{
				this.mHillRenderData[i] = new King.HillRenderData();
				this.mHillRenderData[i].mMaterial = mMaterial;
				this.mHillRenderData[i].SetMesh(skinnedModel2.Model.Meshes[0].VertexBuffer, skinnedModel2.Model.Meshes[0].IndexBuffer, skinnedModel2.Model.Meshes[0].MeshParts[0], 0, 3, 4);
			}
			this.mTorchRenderData = new King.TorchRenderData[3];
			for (int j = 0; j < 3; j++)
			{
				this.mTorchRenderData[j] = new King.TorchRenderData();
				this.mTorchRenderData[j].SetMesh(model.Meshes[0], model.Meshes[0].MeshParts[0], 4, 0, 5);
			}
			this.mOrientation = Matrix.Identity;
		}

		// Token: 0x06001B68 RID: 7016 RVA: 0x000BD0CC File Offset: 0x000BB2CC
		public override void OnPlayerDeath(Player iPlayer)
		{
			if ((this.mScene.PlayState.IsGameEnded && this.mRespawnTimers[iPlayer.ID] <= 0f) || this.mRespawnTimers[iPlayer.ID] > 0f)
			{
				return;
			}
			this.mRespawnTimers[iPlayer.ID] = this.mRespawnTime;
		}

		// Token: 0x06001B69 RID: 7017 RVA: 0x000BD127 File Offset: 0x000BB327
		private void ActivateHill(ref Vector3 iPosition)
		{
			this.mAnimationController.StartClip(this.mAnimationClip, false);
		}

		// Token: 0x06001B6A RID: 7018 RVA: 0x000BD13C File Offset: 0x000BB33C
		public override void Initialize()
		{
			base.Initialize();
			this.mTimeLimit = (float)this.mSettings.TimeLimit * 60f;
			this.mTimeLimitTimer = this.mTimeLimit;
			this.mTimeLimitTarget = this.mTimeLimit;
			this.mScoreLimit = this.mSettings.ScoreLimit;
			this.mRespawnTime = 5f;
			this.mTeams = this.mSettings.TeamsEnabled;
			if (this.mTeams)
			{
				this.mScoreUIs.Add(new VersusRuleset.Score(true));
				this.mScoreUIs.Add(new VersusRuleset.Score(false));
				int i = 0;
				int num = 0;
				while (i < this.mPlayers.Length)
				{
					if (this.mPlayers[i].Playing)
					{
						if (num % 2 == 0)
						{
							this.mPlayers[i].Team |= Factions.TEAM_RED;
							this.mPlayers[i].Team &= ~Factions.TEAM_BLUE;
						}
						else
						{
							this.mPlayers[i].Team |= Factions.TEAM_BLUE;
							this.mPlayers[i].Team &= ~Factions.TEAM_RED;
						}
						num++;
					}
					i++;
				}
				for (int j = 0; j < this.mPlayers.Length; j++)
				{
					if (this.mPlayers[j].Playing)
					{
						Texture2D portrait = this.mPlayers[j].Gamer.Avatar.Portrait;
						this.mPlayers[j].Avatar.Faction &= ~Factions.FRIENDLY;
						if ((this.mPlayers[j].Team & Factions.TEAM_RED) == Factions.TEAM_RED)
						{
							this.mIDToScoreUILookUp[j] = 0;
							this.mScoreUIs[0].AddPlayer(this.mPlayers[j].GamerTag, this.mPlayers[j].ID, portrait, Defines.PLAYERCOLORS[(int)this.mPlayers[j].Color]);
						}
						else
						{
							this.mIDToScoreUILookUp[j] = 1;
							this.mScoreUIs[1].AddPlayer(this.mPlayers[j].GamerTag, this.mPlayers[j].ID, portrait, Defines.PLAYERCOLORS[(int)this.mPlayers[j].Color]);
						}
					}
				}
			}
			else
			{
				int k = 0;
				int num2 = 0;
				while (k < this.mPlayers.Length)
				{
					if (this.mPlayers[k].Playing)
					{
						this.mPlayers[k].Avatar.Faction &= ~Factions.FRIENDLY;
						Texture2D portrait2 = this.mPlayers[k].Gamer.Avatar.Portrait;
						VersusRuleset.Score score = new VersusRuleset.Score(num2 % 2 == 0);
						score.AddPlayer(this.mPlayers[k].GamerTag, this.mPlayers[k].ID, portrait2, Defines.PLAYERCOLORS[(int)this.mPlayers[k].Color]);
						this.mIDToScoreUILookUp[k] = this.mScoreUIs.Count;
						this.mScoreUIs.Add(score);
						num2++;
					}
					k++;
				}
			}
			for (int l = 0; l < this.mScores.Length; l++)
			{
				this.mScores[l] = 0;
			}
			for (int m = 0; m < this.mRespawnTimers.Length; m++)
			{
				this.mRespawnTimers[m] = 0f;
			}
			this.mBody.MoveTo(this.mBodyOffScreenPosition, Matrix.Identity);
			this.mBody.EnableBody();
		}

		// Token: 0x06001B6B RID: 7019 RVA: 0x000BD4E6 File Offset: 0x000BB6E6
		public override void DeInitialize()
		{
			this.mBody.DisableBody();
		}

		// Token: 0x06001B6C RID: 7020 RVA: 0x000BD4F4 File Offset: 0x000BB6F4
		public override void Update(float iDeltaTime, DataChannel iDataChannel)
		{
			if (iDataChannel == DataChannel.None)
			{
				return;
			}
			if (this.mTeams)
			{
				for (int i = 0; i < this.mRespawnTimers.Length; i++)
				{
					float num = Math.Max(this.mRespawnTimers[i] - iDeltaTime, 0f);
					if (this.mRespawnTimers[i] > 0f && num <= 0f && this.mPlayers[i].Playing)
					{
						int teamArea = base.GetTeamArea(this.mPlayers[i].Team);
						Matrix matrix;
						base.GetMatrix(teamArea, out matrix);
						base.RevivePlayer(i, teamArea, ref matrix, null);
					}
					this.mRespawnTimers[i] = num;
				}
			}
			else
			{
				for (int j = 0; j < this.mRespawnTimers.Length; j++)
				{
					float num2 = Math.Max(this.mRespawnTimers[j] - iDeltaTime, 0f);
					if (this.mRespawnTimers[j] > 0f && num2 <= 0f && this.mPlayers[j].Playing)
					{
						int teamArea2 = base.GetTeamArea(this.mPlayers[j].Team);
						Matrix matrix2;
						base.GetMatrix(teamArea2, out matrix2);
						base.RevivePlayer(j, teamArea2, ref matrix2, null);
					}
					this.mRespawnTimers[j] = num2;
				}
			}
			for (int k = 0; k < this.mRespawnTimers.Length; k++)
			{
				int num3;
				if (this.mIDToScoreUILookUp.TryGetValue(k, out num3) && num3 != -1)
				{
					this.mScoreUIs[num3].SetTimer(k, (int)this.mRespawnTimers[k]);
				}
			}
			if (this.mTimeLimit > 0f)
			{
				if (!this.mScene.PlayState.IsGameEnded)
				{
					this.mTimeLimitTimer -= iDeltaTime;
					if (this.mTimeLimitTimer <= 0f)
					{
						base.EndGame();
						this.mTimeLimitTimer = this.mTimeLimit;
					}
				}
				if (NetworkManager.Instance.State == NetworkState.Server)
				{
					if (this.mTimeLimitNetworkUpdate > 1f)
					{
						RulesetMessage rulesetMessage = default(RulesetMessage);
						rulesetMessage.Type = this.RulesetType;
						rulesetMessage.Byte01 = 0;
						rulesetMessage.Float01 = this.mTimeLimitTimer;
						NetworkManager.Instance.Interface.SendMessage<RulesetMessage>(ref rulesetMessage);
						this.mTimeLimitNetworkUpdate = 0f;
					}
					this.mTimeLimitNetworkUpdate += iDeltaTime;
				}
			}
			Matrix transformation = this.mOrientation;
			transformation.Translation = default(Vector3);
			this.mAnimationController.Update(iDeltaTime, ref transformation, true);
			if (this.mAnimationController.HasFinished)
			{
				if (this.mAnimationController.PlaybackMode == PlaybackMode.Forward)
				{
					this.mAnimationController.PlaybackMode = PlaybackMode.Backward;
					this.mAnimationController.StartClip(this.mAnimationClip, false);
				}
				else
				{
					this.mAnimationController.PlaybackMode = PlaybackMode.Forward;
					this.mAnimationController.StartClip(this.mAnimationClip, false);
				}
			}
			this.mBody.MoveTo(default(Vector3), this.mOrientation);
			King.HillRenderData hillRenderData = this.mHillRenderData[(int)iDataChannel];
			this.mAnimationController.SkinnedBoneTransforms.CopyTo(hillRenderData.Bones, 0);
			this.mScene.Scene.AddRenderableObject(iDataChannel, hillRenderData);
			King.TorchRenderData torchRenderData = this.mTorchRenderData[(int)iDataChannel];
			torchRenderData.Transformation = transformation;
			this.mScene.Scene.AddRenderableObject(iDataChannel, torchRenderData);
			VersusRuleset.RenderData renderData = this.mRenderData[(int)iDataChannel];
			renderData.DrawTime = (this.mTimeLimit > 0f);
			renderData.TimeLimit = this.mTimeLimit;
			renderData.Time = this.mTimeLimitTimer;
			if (this.mTimeLimitTimer <= 10f)
			{
				renderData.SetTimeText((int)this.mTimeLimitTimer);
			}
			else
			{
				renderData.SetTimeText(0);
			}
			base.Update(iDeltaTime, iDataChannel);
		}

		// Token: 0x06001B6D RID: 7021 RVA: 0x000BD8A8 File Offset: 0x000BBAA8
		public override void LocalUpdate(float iDeltaTime, DataChannel iDataChannel)
		{
			if (iDataChannel == DataChannel.None)
			{
				return;
			}
			for (int i = 0; i < this.mRespawnTimers.Length; i++)
			{
				this.mRespawnTimers[i] = Math.Max(this.mRespawnTimers[i] - iDeltaTime, 0f);
				for (int j = 0; j < this.mScoreUIs.Count; j++)
				{
					this.mScoreUIs[j].SetTimer(i, (int)this.mRespawnTimers[i]);
				}
			}
			VersusRuleset.RenderData renderData = this.mRenderData[(int)iDataChannel];
			this.mTimeLimitTimer += (this.mTimeLimitTarget - this.mTimeLimitTimer) * iDeltaTime;
			renderData.DrawTime = (this.mTimeLimit > 0f);
			renderData.TimeLimit = this.mTimeLimit;
			renderData.Time = this.mTimeLimitTimer;
			if (this.mTimeLimitTimer <= 10f)
			{
				renderData.SetTimeText((int)this.mTimeLimitTimer);
			}
			else
			{
				renderData.SetTimeText(0);
			}
			base.LocalUpdate(iDeltaTime, iDataChannel);
		}

		// Token: 0x06001B6E RID: 7022 RVA: 0x000BD998 File Offset: 0x000BBB98
		public override void NetworkUpdate(ref RulesetMessage iMsg)
		{
			if (iMsg.Byte01 == 0)
			{
				float num = NetworkManager.Instance.Interface.GetLatency(0) * 0.5f;
				this.mTimeLimitTarget = iMsg.Float01 - num;
				return;
			}
			base.NetworkUpdate(ref iMsg);
		}

		// Token: 0x170006B1 RID: 1713
		// (get) Token: 0x06001B6F RID: 7023 RVA: 0x000BD9DA File Offset: 0x000BBBDA
		public override Rulesets RulesetType
		{
			get
			{
				return Rulesets.King;
			}
		}

		// Token: 0x06001B70 RID: 7024 RVA: 0x000BD9DD File Offset: 0x000BBBDD
		public override bool CanRevive(Player iReviver, Player iRevivee)
		{
			return this.mTeams && (iReviver.Team & iRevivee.Team) != Factions.NONE;
		}

		// Token: 0x06001B71 RID: 7025 RVA: 0x000BD9FB File Offset: 0x000BBBFB
		internal override short[] GetScores()
		{
			return null;
		}

		// Token: 0x170006B2 RID: 1714
		// (get) Token: 0x06001B72 RID: 7026 RVA: 0x000BD9FE File Offset: 0x000BBBFE
		internal override bool Teams
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06001B73 RID: 7027 RVA: 0x000BDA01 File Offset: 0x000BBC01
		internal override short[] GetTeamScores()
		{
			return null;
		}

		// Token: 0x04001DDB RID: 7643
		private King.TorchRenderData[] mTorchRenderData;

		// Token: 0x04001DDC RID: 7644
		private King.HillRenderData[] mHillRenderData;

		// Token: 0x04001DDD RID: 7645
		private AnimationController mAnimationController;

		// Token: 0x04001DDE RID: 7646
		private AnimationClip mAnimationClip;

		// Token: 0x04001DDF RID: 7647
		private Body mBody;

		// Token: 0x04001DE0 RID: 7648
		private CollisionSkin mCollisionSkin;

		// Token: 0x04001DE1 RID: 7649
		private byte[] mScores = new byte[4];

		// Token: 0x04001DE2 RID: 7650
		private float[] mRespawnTimers = new float[4];

		// Token: 0x04001DE3 RID: 7651
		private float mTimeLimitTimer;

		// Token: 0x04001DE4 RID: 7652
		private float mTimeLimitNetworkUpdate;

		// Token: 0x04001DE5 RID: 7653
		private float mTimeLimitTarget;

		// Token: 0x04001DE6 RID: 7654
		private float mTimeLimit;

		// Token: 0x04001DE7 RID: 7655
		private int mScoreLimit;

		// Token: 0x04001DE8 RID: 7656
		private float mRespawnTime;

		// Token: 0x04001DE9 RID: 7657
		private bool mTeams;

		// Token: 0x04001DEA RID: 7658
		private King.Settings mSettings;

		// Token: 0x04001DEB RID: 7659
		private Matrix mOrientation;

		// Token: 0x04001DEC RID: 7660
		private Vector3 mBodyOffScreenPosition = new Vector3(0f, -100f, 0f);

		// Token: 0x02000383 RID: 899
		private class HillRenderData : RenderableObject<SkinnedModelDeferredEffect, SkinnedModelDeferredBasicMaterial>
		{
			// Token: 0x06001B74 RID: 7028 RVA: 0x000BDA04 File Offset: 0x000BBC04
			public HillRenderData()
			{
				this.Bones = new Matrix[80];
			}

			// Token: 0x06001B75 RID: 7029 RVA: 0x000BDA1C File Offset: 0x000BBC1C
			public override void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				SkinnedModelDeferredEffect skinnedModelDeferredEffect = iEffect as SkinnedModelDeferredEffect;
				skinnedModelDeferredEffect.Bones = this.Bones;
				base.Draw(iEffect, iViewFrustum);
			}

			// Token: 0x06001B76 RID: 7030 RVA: 0x000BDA44 File Offset: 0x000BBC44
			public override void DrawShadow(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				SkinnedModelDeferredEffect skinnedModelDeferredEffect = iEffect as SkinnedModelDeferredEffect;
				skinnedModelDeferredEffect.Bones = this.Bones;
				base.DrawShadow(iEffect, iViewFrustum);
			}

			// Token: 0x04001DED RID: 7661
			public Matrix[] Bones;
		}

		// Token: 0x02000384 RID: 900
		private class TorchRenderData : RenderableObject<RenderDeferredEffect, RenderDeferredMaterial>
		{
			// Token: 0x06001B77 RID: 7031 RVA: 0x000BDA6C File Offset: 0x000BBC6C
			public override void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				float num = 0f;
				int i = 0;
				while (i < 8)
				{
					Matrix transformation = this.Transformation;
					Vector3 translation = transformation.Translation;
					Matrix matrix;
					Matrix.CreateRotationY(6.2831855f * (num / 8f), out matrix);
					Vector3 forward = matrix.Forward;
					Vector3.Multiply(ref forward, 5.5f, out forward);
					Vector3.Add(ref forward, ref translation, out translation);
					transformation.Translation = translation;
					this.mMaterial.WorldTransform = transformation;
					base.Draw(iEffect, iViewFrustum);
					i++;
					num += 1f;
				}
			}

			// Token: 0x06001B78 RID: 7032 RVA: 0x000BDAF4 File Offset: 0x000BBCF4
			public override void DrawShadow(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				float num = 0f;
				int i = 0;
				while (i < 8)
				{
					Matrix transformation = this.Transformation;
					Vector3 translation = transformation.Translation;
					Matrix matrix;
					Matrix.CreateRotationY(6.2831855f * (num / 8f), out matrix);
					Vector3 forward = matrix.Forward;
					Vector3.Multiply(ref forward, 5.5f, out forward);
					Vector3.Add(ref forward, ref translation, out translation);
					transformation.Translation = translation;
					this.mMaterial.WorldTransform = transformation;
					base.DrawShadow(iEffect, iViewFrustum);
					i++;
					num += 1f;
				}
			}

			// Token: 0x04001DEE RID: 7662
			public Matrix Transformation;
		}

		// Token: 0x02000385 RID: 901
		internal new class Settings : VersusRuleset.Settings
		{
			// Token: 0x06001B7A RID: 7034 RVA: 0x000BDBB4 File Offset: 0x000BBDB4
			public Settings()
			{
				this.mTimeLimit = base.AddOption<int>(this.LOC_TIME_LIMIT, this.LOC_TT_TIME, new int[]
				{
					0,
					5,
					10,
					30,
					50
				}, new int?[]
				{
					new int?(this.LOC_UNLIMITED),
					null,
					null,
					null,
					null
				});
				this.mTimeLimit.SelectedIndex = 0;
				this.mScoreLimit = base.AddOption<int>(this.LOC_SCORE_LIMIT, this.LOC_TT_SCORE, new int[]
				{
					0,
					5,
					10,
					20,
					50
				}, new int?[]
				{
					new int?(this.LOC_UNLIMITED),
					null,
					null,
					null,
					null
				});
				this.mScoreLimit.SelectedIndex = 3;
				this.mTeams = base.AddOption<bool>(this.LOC_TEAMS, this.LOC_TT_TEAMS, new bool[]
				{
					default(bool),
					true
				}, new int?[]
				{
					new int?(this.LOC_NO),
					new int?(this.LOC_YES)
				});
				this.mTeams.SelectedIndex = 0;
			}

			// Token: 0x170006B3 RID: 1715
			// (get) Token: 0x06001B7B RID: 7035 RVA: 0x000BDD90 File Offset: 0x000BBF90
			public int TimeLimit
			{
				get
				{
					return this.mTimeLimit.SelectedValue;
				}
			}

			// Token: 0x170006B4 RID: 1716
			// (get) Token: 0x06001B7C RID: 7036 RVA: 0x000BDD9D File Offset: 0x000BBF9D
			public int ScoreLimit
			{
				get
				{
					return this.mScoreLimit.SelectedValue;
				}
			}

			// Token: 0x170006B5 RID: 1717
			// (get) Token: 0x06001B7D RID: 7037 RVA: 0x000BDDAA File Offset: 0x000BBFAA
			public override bool TeamsEnabled
			{
				get
				{
					return this.mTeams.SelectedValue;
				}
			}

			// Token: 0x04001DEF RID: 7663
			private DropDownBox<int> mTimeLimit;

			// Token: 0x04001DF0 RID: 7664
			private DropDownBox<int> mScoreLimit;

			// Token: 0x04001DF1 RID: 7665
			private DropDownBox<bool> mTeams;
		}
	}
}
