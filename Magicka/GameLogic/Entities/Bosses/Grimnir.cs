using System;
using System.Collections.Generic;
using JigLibX.Geometry;
using Magicka.AI;
using Magicka.Audio;
using Magicka.GameLogic.Entities.Abilities.SpecialAbilities;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Graphics;
using Magicka.Levels;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using XNAnimation;
using XNAnimation.Controllers;
using XNAnimation.Effects;

namespace Magicka.GameLogic.Entities.Bosses
{
	// Token: 0x020003B8 RID: 952
	public class Grimnir : IBoss
	{
		// Token: 0x06001D62 RID: 7522 RVA: 0x000D00B4 File Offset: 0x000CE2B4
		public Grimnir(PlayState iPlayState)
		{
			this.mPlayers = Game.Instance.Players;
			for (int i = 0; i < this.mPlayers.Length; i++)
			{
				if (this.mPlayers[i].Playing)
				{
					this.mNrOfPlayers++;
				}
			}
			this.mPlayState = iPlayState;
			this.mRandom = new Random();
			SkinnedModel skinnedModel;
			lock (Game.Instance.GraphicsDevice)
			{
				this.mPlayState.Content.Load<CharacterTemplate>("Data/Characters/druid");
				this.mPlayState.Content.Load<CharacterTemplate>("Data/Characters/goblin_shaman");
				this.mPlayState.Content.Load<CharacterTemplate>("Data/Characters/goblin_warlock");
				this.mPlayState.Content.Load<CharacterTemplate>("Data/Characters/necromancer");
				this.mPlayState.Content.Load<CharacterTemplate>("Data/Characters/warlock");
				this.mPlayState.Content.Load<CharacterTemplate>("Data/Characters/dwarf_mage");
				this.mModel = this.mPlayState.Content.Load<SkinnedModel>("Models/Bosses/grimnir/grimnir_mesh");
				skinnedModel = this.mPlayState.Content.Load<SkinnedModel>("Models/Bosses/grimnir/grimnir_animation");
			}
			this.mActiveMinions = new List<NonPlayerCharacter>(4);
			this.mController = new AnimationController();
			this.mController.Skeleton = skinnedModel.SkeletonBones;
			this.mClips = new AnimationClip[1];
			this.mClips[0] = skinnedModel.AnimationClips["idle"];
			SkinnedModelBasicEffect iEffect = this.mModel.Model.Meshes[0].MeshParts[0].Effect as SkinnedModelBasicEffect;
			SkinnedModelDeferredBasicMaterial mMaterial;
			Helper.SkinnedModelDeferredMaterialFromBasicEffect(iEffect, out mMaterial);
			this.mRenderData = new Grimnir.RenderData[3];
			for (int j = 0; j < 3; j++)
			{
				this.mRenderData[j] = new Grimnir.RenderData();
				this.mRenderData[j].SetMesh(this.mModel.Model.Meshes[0].VertexBuffer, this.mModel.Model.Meshes[0].IndexBuffer, this.mModel.Model.Meshes[0].MeshParts[0], 2);
				this.mRenderData[j].mMaterial = mMaterial;
			}
		}

		// Token: 0x06001D63 RID: 7523 RVA: 0x000D032C File Offset: 0x000CE52C
		protected unsafe void ChangeState(Grimnir.States iState)
		{
			if (NetworkManager.Instance.State != NetworkState.Client)
			{
				if (NetworkManager.Instance.State == NetworkState.Server)
				{
					Grimnir.ChangeStateMessage changeStateMessage;
					changeStateMessage.State = iState;
					BossFight.Instance.SendMessage<Grimnir.ChangeStateMessage>(this, 2, (void*)(&changeStateMessage), true);
				}
				this.mCurrentState.OnExit(this);
				this.mCurrentState = this.GetState(iState);
				this.mCurrentState.OnEnter(this);
			}
		}

		// Token: 0x06001D64 RID: 7524 RVA: 0x000D0390 File Offset: 0x000CE590
		private IBossState<Grimnir> GetState(Grimnir.States iState)
		{
			switch (iState)
			{
			case Grimnir.States.IntroFade:
				return Grimnir.IntroFadeState.Instance;
			case Grimnir.States.OutroFade:
				return Grimnir.OutroFadeState.Instance;
			case Grimnir.States.Action:
				return Grimnir.ActionState.Instance;
			case Grimnir.States.FadeTransition:
				return Grimnir.FadeTransitionState.Instance;
			default:
				return null;
			}
		}

		// Token: 0x06001D65 RID: 7525 RVA: 0x000D03D0 File Offset: 0x000CE5D0
		public void Initialize(ref Matrix iOrientation, int iUniqueID)
		{
			this.Initialize(ref iOrientation);
		}

		// Token: 0x06001D66 RID: 7526 RVA: 0x000D03DC File Offset: 0x000CE5DC
		public void Initialize(ref Matrix iOrientation)
		{
			this.mHitPoints = 6f;
			this.mGrimnirFloatDelta = 0f;
			this.mDead = false;
			this.mGrimnirOrientation = Matrix.CreateRotationY(3.1415927f);
			Grimnir.GRIMNIR_POSITIONS = new Vector3[6];
			Locator locator;
			this.mPlayState.Level.CurrentScene.GetLocator("grimnira".GetHashCodeCustom(), out locator);
			Grimnir.GRIMNIR_POSITIONS[0] = locator.Transform.Translation;
			this.mPlayState.Level.CurrentScene.GetLocator("grimnirb".GetHashCodeCustom(), out locator);
			Grimnir.GRIMNIR_POSITIONS[1] = locator.Transform.Translation;
			this.mPlayState.Level.CurrentScene.GetLocator("grimnirc".GetHashCodeCustom(), out locator);
			Grimnir.GRIMNIR_POSITIONS[2] = locator.Transform.Translation;
			this.mPlayState.Level.CurrentScene.GetLocator("grimnird".GetHashCodeCustom(), out locator);
			Grimnir.GRIMNIR_POSITIONS[3] = locator.Transform.Translation;
			this.mPlayState.Level.CurrentScene.GetLocator("grimnire".GetHashCodeCustom(), out locator);
			Grimnir.GRIMNIR_POSITIONS[4] = locator.Transform.Translation;
			this.mPlayState.Level.CurrentScene.GetLocator("grimnirf".GetHashCodeCustom(), out locator);
			Grimnir.GRIMNIR_POSITIONS[5] = locator.Transform.Translation;
			this.mController.StartClip(this.mClips[0], true);
			this.mCurrentActionState = Grimnir.ActionStates.Mountaindale;
			this.mNextActionState = this.mCurrentActionState;
			this.mCurrentState = Grimnir.IntroFadeState.Instance;
			this.mCurrentState.OnEnter(this);
			this.mActiveMinions.Clear();
		}

		// Token: 0x06001D67 RID: 7527 RVA: 0x000D05D8 File Offset: 0x000CE7D8
		public void DeInitialize()
		{
		}

		// Token: 0x06001D68 RID: 7528 RVA: 0x000D05DC File Offset: 0x000CE7DC
		public void UpdateBoss(DataChannel iDataChannel, float iDeltaTime, bool iFightStarted)
		{
			if (NetworkManager.Instance.State == NetworkState.Server)
			{
				this.mNetworkUpdateTimer -= iDeltaTime;
				if (this.mNetworkUpdateTimer <= 0f)
				{
					this.mNetworkUpdateTimer = 0.033333335f;
					this.NetworkUpdate();
				}
			}
			this.mNormalizedHitPoints = this.mHitPoints * 0.16666667f;
			this.mCurrentState.OnUpdate(iDeltaTime, this);
			for (int i = 0; i < this.mActiveMinions.Count; i++)
			{
				if (this.mActiveMinions[i].Dead)
				{
					this.mActiveMinions.RemoveAt(i--);
				}
			}
			this.mGrimnirFloatDelta += iDeltaTime;
			Vector3 position = this.mPlayState.Camera.Position;
			Vector3 cameraoffset = MagickCamera.CAMERAOFFSET;
			cameraoffset.Y += 28f;
			cameraoffset.Z += 30f;
			cameraoffset.Y += (float)Math.Sin((double)this.mGrimnirFloatDelta) * 0.125f;
			cameraoffset.X += (float)Math.Cos((double)this.mGrimnirFloatDelta) * 0.125f;
			Vector3.Subtract(ref position, ref cameraoffset, out position);
			this.mGrimnirOrientation.Translation = position;
			Matrix matrix = this.mGrimnirOrientation;
			MagickaMath.UniformMatrixScale(ref matrix, 7f);
			this.mRenderData[(int)iDataChannel].mMaterial.Alpha = 0.65f;
			this.mController.Update(iDeltaTime, ref matrix, true);
			this.mController.SkinnedBoneTransforms.CopyTo(this.mRenderData[(int)iDataChannel].mBones, 0);
			this.mPlayState.Scene.AddRenderableAdditiveObject(iDataChannel, this.mRenderData[(int)iDataChannel]);
			this.mNrOfPlayers = 0;
			for (int j = 0; j < this.mPlayers.Length; j++)
			{
				if (this.mPlayers[j].Playing)
				{
					this.mNrOfPlayers++;
				}
			}
		}

		// Token: 0x06001D69 RID: 7529 RVA: 0x000D07CC File Offset: 0x000CE9CC
		public Vector3 StateCameraTarget(Grimnir.ActionStates iState)
		{
			Locator locator;
			this.mPlayState.Level.CurrentScene.GetLocator(Grimnir.SPAWN_HASH[(int)iState, 1], out locator);
			return locator.Transform.Translation;
		}

		// Token: 0x06001D6A RID: 7530 RVA: 0x000D0808 File Offset: 0x000CEA08
		public void Corporealize()
		{
			this.mHitPoints = 0f;
			this.ChangeState(Grimnir.States.OutroFade);
		}

		// Token: 0x06001D6B RID: 7531 RVA: 0x000D081C File Offset: 0x000CEA1C
		public unsafe void TeleportPlayers(int id)
		{
			if (NetworkManager.Instance.State != NetworkState.Client)
			{
				if (NetworkManager.Instance.State == NetworkState.Server)
				{
					Grimnir.TeleportMessage teleportMessage;
					teleportMessage.Venue = id;
					BossFight.Instance.SendMessage<Grimnir.TeleportMessage>(this, 1, (void*)(&teleportMessage), true);
				}
				for (int i = 0; i < this.mPlayers.Length; i++)
				{
					if (this.mPlayers[i].Playing && this.mPlayers[i].Avatar != null && !this.mPlayers[i].Avatar.Dead)
					{
						Locator locator;
						this.mPlayState.Level.CurrentScene.GetLocator(Grimnir.SPAWN_HASH[id, i], out locator);
						Matrix transform = locator.Transform;
						transform.Translation = default(Vector3);
						Vector3 vector = locator.Transform.Translation;
						Segment iSeg = default(Segment);
						iSeg.Origin = vector;
						iSeg.Delta.Y = iSeg.Delta.Y - 7f;
						float num;
						Vector3 vector2;
						Vector3 vector3;
						if (this.mPlayState.Level.CurrentScene.SegmentIntersect(out num, out vector2, out vector3, iSeg))
						{
							vector = vector2;
						}
						vector.Y += this.mPlayers[i].Avatar.Radius + this.mPlayers[i].Avatar.Capsule.Length * 0.5f + 0.1f;
						this.mPlayers[i].Avatar.Body.MoveTo(vector, transform);
					}
				}
			}
		}

		// Token: 0x06001D6C RID: 7532 RVA: 0x000D09A8 File Offset: 0x000CEBA8
		private unsafe void SpawnMinions(Grimnir.ActionStates iState)
		{
			if (NetworkManager.Instance.State != NetworkState.Client)
			{
				this.mActiveMinions.Clear();
				int num = this.mRandom.Next(4);
				for (int i = 0; i < Grimnir.MINION_SPAWN_COUNT[(int)iState]; i++)
				{
					Locator locator;
					this.mPlayState.Level.CurrentScene.GetLocator(Grimnir.MINION_SPAWN_HASH[(int)iState, (num + 4 - i) % 4], out locator);
					CharacterTemplate cachedTemplate = CharacterTemplate.GetCachedTemplate(Grimnir.MINION_HASH[(int)iState]);
					NonPlayerCharacter instance = NonPlayerCharacter.GetInstance(this.mPlayState);
					Vector3 vector = locator.Transform.Translation;
					Segment iSeg = default(Segment);
					iSeg.Origin = vector + Vector3.Up;
					iSeg.Delta.Y = iSeg.Delta.Y - 5f;
					float num2;
					Vector3 vector2;
					Vector3 vector3;
					if (this.mPlayState.Level.CurrentScene.SegmentIntersect(out num2, out vector2, out vector3, iSeg))
					{
						vector = vector2;
					}
					vector.Y += cachedTemplate.Radius + cachedTemplate.Length * 0.5f + 0.1f;
					instance.Initialize(cachedTemplate, vector, 0);
					Vector3 forward = locator.Transform.Forward;
					instance.CharacterBody.DesiredDirection = forward;
					Agent ai = instance.AI;
					ai.SetOrder(Order.Attack, ReactTo.Attack | ReactTo.Proximity, Order.Attack, 0, 0, 0, null);
					this.mPlayState.EntityManager.AddEntity(instance);
					VisualEffectReference visualEffectReference;
					EffectManager.Instance.StartEffect(Grimnir.SPAWN_MINION_EFFECT, ref vector, ref forward, out visualEffectReference);
					AudioManager.Instance.PlayCue(Banks.Spells, Grimnir.SPAWN_MINION_SOUND, instance.AudioEmitter);
					if (NetworkManager.Instance.State == NetworkState.Server)
					{
						Grimnir.SpawnMessage spawnMessage;
						spawnMessage.Handle = instance.Handle;
						spawnMessage.Position = instance.Position;
						spawnMessage.Direction = instance.Direction;
						spawnMessage.TypeID = Grimnir.MINION_HASH[(int)iState];
						BossFight.Instance.SendMessage<Grimnir.SpawnMessage>(this, 3, (void*)(&spawnMessage), true);
					}
					this.mActiveMinions.Add(instance);
				}
			}
		}

		// Token: 0x06001D6D RID: 7533 RVA: 0x000D0BA3 File Offset: 0x000CEDA3
		public bool AddImpulseVelocity(Vector3 iDirection, float iElevation, float iMassPower, float iDistance)
		{
			throw new NotImplementedException();
		}

		// Token: 0x17000746 RID: 1862
		// (get) Token: 0x06001D6E RID: 7534 RVA: 0x000D0BAA File Offset: 0x000CEDAA
		public bool Dead
		{
			get
			{
				return this.mDead;
			}
		}

		// Token: 0x17000747 RID: 1863
		// (get) Token: 0x06001D6F RID: 7535 RVA: 0x000D0BB2 File Offset: 0x000CEDB2
		public float MaxHitPoints
		{
			get
			{
				return 6f;
			}
		}

		// Token: 0x17000748 RID: 1864
		// (get) Token: 0x06001D70 RID: 7536 RVA: 0x000D0BB9 File Offset: 0x000CEDB9
		public float HitPoints
		{
			get
			{
				return this.mHitPoints;
			}
		}

		// Token: 0x06001D71 RID: 7537 RVA: 0x000D0BC1 File Offset: 0x000CEDC1
		public DamageResult Damage(int iPartIndex, Damage iDamage, Entity iAttacker, ref Vector3 iAttackPosition, Defines.DamageFeatures iFeatures)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06001D72 RID: 7538 RVA: 0x000D0BC8 File Offset: 0x000CEDC8
		public void Damage(int iPartIndex, float iDamage, Elements iElement)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06001D73 RID: 7539 RVA: 0x000D0BCF File Offset: 0x000CEDCF
		public void SetSlow(int iIndex)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06001D74 RID: 7540 RVA: 0x000D0BD6 File Offset: 0x000CEDD6
		public void GetRandomPositionOnCollisionSkin(int iIndex, out Vector3 oPosition)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06001D75 RID: 7541 RVA: 0x000D0BDD File Offset: 0x000CEDDD
		public bool HasStatus(int iIndex, StatusEffects iStatus)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06001D76 RID: 7542 RVA: 0x000D0BE4 File Offset: 0x000CEDE4
		public float StatusMagnitude(int iIndex, StatusEffects iStatus)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06001D77 RID: 7543 RVA: 0x000D0BEB File Offset: 0x000CEDEB
		public StatusEffect[] GetStatusEffects()
		{
			throw new NotImplementedException();
		}

		// Token: 0x06001D78 RID: 7544 RVA: 0x000D0BF2 File Offset: 0x000CEDF2
		public void ScriptMessage(BossMessages iMessage)
		{
		}

		// Token: 0x06001D79 RID: 7545 RVA: 0x000D0BF4 File Offset: 0x000CEDF4
		private unsafe void NetworkUpdate()
		{
			NetworkServer networkServer = NetworkManager.Instance.Interface as NetworkServer;
			if (networkServer == null)
			{
				return;
			}
			Grimnir.UpdateMessage updateMessage = default(Grimnir.UpdateMessage);
			updateMessage.HitPoints = this.mHitPoints;
			for (int i = 0; i < networkServer.Connections; i++)
			{
				BossFight.Instance.SendMessage<Grimnir.UpdateMessage>(this, 0, (void*)(&updateMessage), false, i);
			}
		}

		// Token: 0x06001D7A RID: 7546 RVA: 0x000D0C4C File Offset: 0x000CEE4C
		public unsafe void NetworkUpdate(ref BossUpdateMessage iMsg)
		{
			switch (iMsg.Type)
			{
			case 0:
			{
				Grimnir.UpdateMessage updateMessage;
				BossUpdateMessage.ConvertTo(ref iMsg, (void*)(&updateMessage));
				this.mHitPoints = updateMessage.HitPoints;
				return;
			}
			case 1:
			{
				Grimnir.TeleportMessage teleportMessage;
				BossUpdateMessage.ConvertTo(ref iMsg, (void*)(&teleportMessage));
				int venue = teleportMessage.Venue;
				for (int i = 0; i < this.mPlayers.Length; i++)
				{
					if (this.mPlayers[i].Playing && this.mPlayers[i].Avatar != null && !this.mPlayers[i].Avatar.Dead)
					{
						Locator locator;
						this.mPlayState.Level.CurrentScene.GetLocator(Grimnir.SPAWN_HASH[venue, i], out locator);
						Matrix transform = locator.Transform;
						transform.Translation = default(Vector3);
						Vector3 vector = locator.Transform.Translation;
						Segment iSeg = default(Segment);
						iSeg.Origin = vector;
						iSeg.Delta.Y = iSeg.Delta.Y - 7f;
						float num;
						Vector3 vector2;
						Vector3 vector3;
						if (this.mPlayState.Level.CurrentScene.SegmentIntersect(out num, out vector2, out vector3, iSeg))
						{
							vector = vector2;
						}
						vector.Y += this.mPlayers[i].Avatar.Radius + this.mPlayers[i].Avatar.Capsule.Length * 0.5f + 0.1f;
						this.mPlayers[i].Avatar.Body.MoveTo(vector, transform);
					}
				}
				return;
			}
			case 2:
			{
				Grimnir.ChangeStateMessage changeStateMessage;
				BossUpdateMessage.ConvertTo(ref iMsg, (void*)(&changeStateMessage));
				this.mCurrentState.OnExit(this);
				this.mCurrentState = this.GetState(changeStateMessage.State);
				this.mCurrentState.OnEnter(this);
				return;
			}
			case 3:
			{
				Grimnir.SpawnMessage spawnMessage;
				BossUpdateMessage.ConvertTo(ref iMsg, (void*)(&spawnMessage));
				NonPlayerCharacter nonPlayerCharacter = Entity.GetFromHandle((int)spawnMessage.Handle) as NonPlayerCharacter;
				nonPlayerCharacter.Initialize(CharacterTemplate.GetCachedTemplate(spawnMessage.TypeID), spawnMessage.Position, 0);
				nonPlayerCharacter.CharacterBody.DesiredDirection = spawnMessage.Direction;
				this.mPlayState.EntityManager.AddEntity(nonPlayerCharacter);
				Agent ai = nonPlayerCharacter.AI;
				ai.SetOrder(Order.Attack, ReactTo.Attack | ReactTo.Proximity, Order.Attack, 0, 0, 0, null);
				this.mActiveMinions.Add(nonPlayerCharacter);
				VisualEffectReference visualEffectReference;
				EffectManager.Instance.StartEffect(Grimnir.SPAWN_MINION_EFFECT, ref spawnMessage.Position, ref spawnMessage.Direction, out visualEffectReference);
				AudioManager.Instance.PlayCue(Banks.Spells, Grimnir.SPAWN_MINION_SOUND, nonPlayerCharacter.AudioEmitter);
				return;
			}
			case 4:
			{
				Grimnir.CameraMessage cameraMessage;
				BossUpdateMessage.ConvertTo(ref iMsg, (void*)(&cameraMessage));
				this.mPlayState.Camera.SetPosition(ref cameraMessage.Position);
				return;
			}
			default:
				return;
			}
		}

		// Token: 0x06001D7B RID: 7547 RVA: 0x000D0F0C File Offset: 0x000CF10C
		public void NetworkInitialize(ref BossInitializeMessage iMsg)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06001D7C RID: 7548 RVA: 0x000D0F13 File Offset: 0x000CF113
		public BossEnum GetBossType()
		{
			return BossEnum.Grimnir;
		}

		// Token: 0x17000749 RID: 1865
		// (get) Token: 0x06001D7D RID: 7549 RVA: 0x000D0F16 File Offset: 0x000CF116
		public bool NetworkInitialized
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06001D7E RID: 7550 RVA: 0x000D0F19 File Offset: 0x000CF119
		public float ResistanceAgainst(Elements iElement)
		{
			return 1f;
		}

		// Token: 0x06001D7F RID: 7551 RVA: 0x000D0F38 File Offset: 0x000CF138
		// Note: this type is marked as 'beforefieldinit'.
		static Grimnir()
		{
			int[,] array = new int[6, 4];
			array[0, 0] = "starta0".GetHashCodeCustom();
			array[0, 1] = "starta1".GetHashCodeCustom();
			array[0, 2] = "starta2".GetHashCodeCustom();
			array[0, 3] = "starta3".GetHashCodeCustom();
			array[1, 0] = "startc0".GetHashCodeCustom();
			array[1, 1] = "startc1".GetHashCodeCustom();
			array[1, 2] = "startc2".GetHashCodeCustom();
			array[1, 3] = "startc3".GetHashCodeCustom();
			array[2, 0] = "startf0".GetHashCodeCustom();
			array[2, 1] = "startf1".GetHashCodeCustom();
			array[2, 2] = "startf2".GetHashCodeCustom();
			array[2, 3] = "startf3".GetHashCodeCustom();
			array[3, 0] = "startd0".GetHashCodeCustom();
			array[3, 1] = "startd1".GetHashCodeCustom();
			array[3, 2] = "startd2".GetHashCodeCustom();
			array[3, 3] = "startd3".GetHashCodeCustom();
			array[4, 0] = "starte0".GetHashCodeCustom();
			array[4, 1] = "starte1".GetHashCodeCustom();
			array[4, 2] = "starte2".GetHashCodeCustom();
			array[4, 3] = "starte3".GetHashCodeCustom();
			array[5, 0] = "startb0".GetHashCodeCustom();
			array[5, 1] = "startb1".GetHashCodeCustom();
			array[5, 2] = "startb2".GetHashCodeCustom();
			array[5, 3] = "startb3".GetHashCodeCustom();
			Grimnir.SPAWN_HASH = array;
			int[,] array2 = new int[6, 4];
			array2[0, 0] = "spawna0".GetHashCodeCustom();
			array2[0, 1] = "spawna1".GetHashCodeCustom();
			array2[0, 2] = "spawna2".GetHashCodeCustom();
			array2[0, 3] = "spawna3".GetHashCodeCustom();
			array2[1, 0] = "spawnc0".GetHashCodeCustom();
			array2[1, 1] = "spawnc1".GetHashCodeCustom();
			array2[1, 2] = "spawnc2".GetHashCodeCustom();
			array2[1, 3] = "spawnc3".GetHashCodeCustom();
			array2[2, 0] = "spawnf0".GetHashCodeCustom();
			array2[2, 1] = "spawnf1".GetHashCodeCustom();
			array2[2, 2] = "spawnf2".GetHashCodeCustom();
			array2[2, 3] = "spawnf3".GetHashCodeCustom();
			array2[3, 0] = "spawnd0".GetHashCodeCustom();
			array2[3, 1] = "spawnd1".GetHashCodeCustom();
			array2[3, 2] = "spawnd2".GetHashCodeCustom();
			array2[3, 3] = "spawnd3".GetHashCodeCustom();
			array2[4, 0] = "spawne0".GetHashCodeCustom();
			array2[4, 1] = "spawne1".GetHashCodeCustom();
			array2[4, 2] = "spawne2".GetHashCodeCustom();
			array2[4, 3] = "spawne3".GetHashCodeCustom();
			array2[5, 0] = "spawnb0".GetHashCodeCustom();
			array2[5, 1] = "spawnb1".GetHashCodeCustom();
			array2[5, 2] = "spawnb2".GetHashCodeCustom();
			array2[5, 3] = "spawnb3".GetHashCodeCustom();
			Grimnir.MINION_SPAWN_HASH = array2;
			Grimnir.MINION_SPAWN_COUNT = new int[]
			{
				4,
				2,
				2,
				2,
				2,
				1
			};
			Grimnir.MINION_HASH = new int[]
			{
				"goblin_shaman".GetHashCodeCustom(),
				"druid".GetHashCodeCustom(),
				"warlock".GetHashCodeCustom(),
				"goblin_warlock".GetHashCodeCustom(),
				"necromancer".GetHashCodeCustom(),
				"dwarf_mage".GetHashCodeCustom()
			};
		}

		// Token: 0x04001FE7 RID: 8167
		private const float NETWORK_UPDATE_PERIOD = 0.033333335f;

		// Token: 0x04001FE8 RID: 8168
		private const float DREAM_FADE_TRANSITION_TIME = 1.5f;

		// Token: 0x04001FE9 RID: 8169
		private const float MINION_SPAWN_DELAY = 1f;

		// Token: 0x04001FEA RID: 8170
		private const float DAMAGE_PER_SUCCESSIVE_DREAM = 1f;

		// Token: 0x04001FEB RID: 8171
		private const float FADE_IN_TIME = 0.25f;

		// Token: 0x04001FEC RID: 8172
		private const float FADE_OUT_TIME = 0.25f;

		// Token: 0x04001FED RID: 8173
		private const float FADE_GAP = 0.2f;

		// Token: 0x04001FEE RID: 8174
		private const float MAXHITPOINTS = 6f;

		// Token: 0x04001FEF RID: 8175
		private const float MAXHITPOINTSDIVISOR = 0.16666667f;

		// Token: 0x04001FF0 RID: 8176
		protected float mNetworkUpdateTimer;

		// Token: 0x04001FF1 RID: 8177
		private static readonly int SPAWN_MINION_EFFECT = Teleport.TELEPORT_EFFECT_APPEAR;

		// Token: 0x04001FF2 RID: 8178
		private static readonly int SPAWN_MINION_SOUND = Teleport.TELEPORT_SOUND_ORIGIN;

		// Token: 0x04001FF3 RID: 8179
		private static readonly Vector3 DIALOG_OFFSET = new Vector3(-3f, 3f, 0f);

		// Token: 0x04001FF4 RID: 8180
		private static readonly int DIALOG_DEAD = "grimnirdead".GetHashCodeCustom();

		// Token: 0x04001FF5 RID: 8181
		private static readonly int[] DIALOG_TAUNTS = new int[]
		{
			"grimnirtaunt1".GetHashCodeCustom(),
			"grimnirtaunt2".GetHashCodeCustom(),
			"grimnirtaunt3".GetHashCodeCustom(),
			"grimnirtaunt4".GetHashCodeCustom(),
			"grimnirtaunt5".GetHashCodeCustom(),
			"grimnirtaunt6".GetHashCodeCustom()
		};

		// Token: 0x04001FF6 RID: 8182
		private static readonly int[,] SPAWN_HASH;

		// Token: 0x04001FF7 RID: 8183
		private static readonly int[,] MINION_SPAWN_HASH;

		// Token: 0x04001FF8 RID: 8184
		private static readonly int[] MINION_SPAWN_COUNT;

		// Token: 0x04001FF9 RID: 8185
		private static Vector3[] GRIMNIR_POSITIONS;

		// Token: 0x04001FFA RID: 8186
		private static readonly int[] MINION_HASH;

		// Token: 0x04001FFB RID: 8187
		private float mDreamCompleteTimer;

		// Token: 0x04001FFC RID: 8188
		private bool mTransitionBegun;

		// Token: 0x04001FFD RID: 8189
		private float mSpawnDelay;

		// Token: 0x04001FFE RID: 8190
		private bool mMinionsSpawned;

		// Token: 0x04001FFF RID: 8191
		private float mNormalizedHitPoints;

		// Token: 0x04002000 RID: 8192
		private float mFadeTimer;

		// Token: 0x04002001 RID: 8193
		private bool mFadeIn;

		// Token: 0x04002002 RID: 8194
		private float mHitPoints = 6f;

		// Token: 0x04002003 RID: 8195
		private bool mDead;

		// Token: 0x04002004 RID: 8196
		private float mGrimnirFloatDelta;

		// Token: 0x04002005 RID: 8197
		private List<NonPlayerCharacter> mActiveMinions;

		// Token: 0x04002006 RID: 8198
		private int mNrOfPlayers;

		// Token: 0x04002007 RID: 8199
		private Player[] mPlayers;

		// Token: 0x04002008 RID: 8200
		private Random mRandom;

		// Token: 0x04002009 RID: 8201
		private Grimnir.RenderData[] mRenderData;

		// Token: 0x0400200A RID: 8202
		private SkinnedModel mModel;

		// Token: 0x0400200B RID: 8203
		private AnimationController mController;

		// Token: 0x0400200C RID: 8204
		private AnimationClip[] mClips;

		// Token: 0x0400200D RID: 8205
		private PlayState mPlayState;

		// Token: 0x0400200E RID: 8206
		private Matrix mGrimnirOrientation;

		// Token: 0x0400200F RID: 8207
		private Grimnir.ActionStates mCurrentActionState;

		// Token: 0x04002010 RID: 8208
		private Grimnir.ActionStates mNextActionState;

		// Token: 0x04002011 RID: 8209
		private IBossState<Grimnir> mCurrentState;

		// Token: 0x020003B9 RID: 953
		protected class RenderData : RenderableAdditiveObject<SkinnedModelDeferredEffect, SkinnedModelDeferredBasicMaterial>
		{
			// Token: 0x06001D80 RID: 7552 RVA: 0x000D13C9 File Offset: 0x000CF5C9
			public RenderData()
			{
				this.mBones = new Matrix[80];
			}

			// Token: 0x06001D81 RID: 7553 RVA: 0x000D13E0 File Offset: 0x000CF5E0
			public override void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				SkinnedModelDeferredEffect skinnedModelDeferredEffect = iEffect as SkinnedModelDeferredEffect;
				skinnedModelDeferredEffect.ProjectionMapEnabled = false;
				skinnedModelDeferredEffect.Bones = this.mBones;
				skinnedModelDeferredEffect.Colorize = Grimnir.RenderData.ColdColor;
				skinnedModelDeferredEffect.OverrideColor = new Vector4(1f, 1f, 1f, this.Flash);
				base.Draw(iEffect, iViewFrustum);
				skinnedModelDeferredEffect.OverrideColor = Vector4.Zero;
				skinnedModelDeferredEffect.Colorize = Vector4.Zero;
			}

			// Token: 0x04002012 RID: 8210
			protected static readonly Vector4 ColdColor = new Vector4(1f, 1.6f, 2f, 1f);

			// Token: 0x04002013 RID: 8211
			public Matrix[] mBones;

			// Token: 0x04002014 RID: 8212
			public float Flash;
		}

		// Token: 0x020003BA RID: 954
		public class IntroFadeState : IBossState<Grimnir>
		{
			// Token: 0x1700074A RID: 1866
			// (get) Token: 0x06001D83 RID: 7555 RVA: 0x000D1470 File Offset: 0x000CF670
			public static Grimnir.IntroFadeState Instance
			{
				get
				{
					if (Grimnir.IntroFadeState.mSingelton == null)
					{
						lock (Grimnir.IntroFadeState.mSingeltonLock)
						{
							if (Grimnir.IntroFadeState.mSingelton == null)
							{
								Grimnir.IntroFadeState.mSingelton = new Grimnir.IntroFadeState();
							}
						}
					}
					return Grimnir.IntroFadeState.mSingelton;
				}
			}

			// Token: 0x06001D84 RID: 7556 RVA: 0x000D14C4 File Offset: 0x000CF6C4
			private IntroFadeState()
			{
			}

			// Token: 0x06001D85 RID: 7557 RVA: 0x000D14CC File Offset: 0x000CF6CC
			public void OnEnter(Grimnir iOwner)
			{
				iOwner.mFadeTimer = 0.25f;
				RenderManager.Instance.EndTransition(Transitions.Fade, Color.White, iOwner.mFadeTimer);
				for (int i = 0; i < iOwner.mPlayers.Length; i++)
				{
					if (iOwner.mPlayers[i].Playing && iOwner.mPlayers[i].Avatar != null)
					{
						Magicka.Animations iAnimation = (Magicka.Animations)(131 + i);
						iOwner.mPlayers[i].Avatar.GoToAnimation(iAnimation, 0f);
					}
				}
			}

			// Token: 0x06001D86 RID: 7558 RVA: 0x000D154C File Offset: 0x000CF74C
			public void OnUpdate(float iDeltaTime, Grimnir iOwner)
			{
				iOwner.mFadeTimer -= iDeltaTime;
				if (iOwner.mFadeTimer - 0.2f <= 0f)
				{
					iOwner.mCurrentActionState = iOwner.mNextActionState;
					iOwner.ChangeState(Grimnir.States.Action);
				}
			}

			// Token: 0x06001D87 RID: 7559 RVA: 0x000D1582 File Offset: 0x000CF782
			public void OnExit(Grimnir iOwner)
			{
			}

			// Token: 0x04002015 RID: 8213
			private static Grimnir.IntroFadeState mSingelton;

			// Token: 0x04002016 RID: 8214
			private static volatile object mSingeltonLock = new object();
		}

		// Token: 0x020003BB RID: 955
		public class OutroFadeState : IBossState<Grimnir>
		{
			// Token: 0x1700074B RID: 1867
			// (get) Token: 0x06001D89 RID: 7561 RVA: 0x000D1594 File Offset: 0x000CF794
			public static Grimnir.OutroFadeState Instance
			{
				get
				{
					if (Grimnir.OutroFadeState.mSingelton == null)
					{
						lock (Grimnir.OutroFadeState.mSingeltonLock)
						{
							if (Grimnir.OutroFadeState.mSingelton == null)
							{
								Grimnir.OutroFadeState.mSingelton = new Grimnir.OutroFadeState();
							}
						}
					}
					return Grimnir.OutroFadeState.mSingelton;
				}
			}

			// Token: 0x06001D8A RID: 7562 RVA: 0x000D15E8 File Offset: 0x000CF7E8
			private OutroFadeState()
			{
			}

			// Token: 0x06001D8B RID: 7563 RVA: 0x000D15F0 File Offset: 0x000CF7F0
			public void OnEnter(Grimnir iOwner)
			{
			}

			// Token: 0x06001D8C RID: 7564 RVA: 0x000D15F2 File Offset: 0x000CF7F2
			public void OnUpdate(float iDeltaTime, Grimnir iOwner)
			{
			}

			// Token: 0x06001D8D RID: 7565 RVA: 0x000D15F4 File Offset: 0x000CF7F4
			public void OnExit(Grimnir iOwner)
			{
			}

			// Token: 0x04002017 RID: 8215
			private static Grimnir.OutroFadeState mSingelton;

			// Token: 0x04002018 RID: 8216
			private static volatile object mSingeltonLock = new object();
		}

		// Token: 0x020003BC RID: 956
		public class ActionState : IBossState<Grimnir>
		{
			// Token: 0x1700074C RID: 1868
			// (get) Token: 0x06001D8F RID: 7567 RVA: 0x000D1604 File Offset: 0x000CF804
			public static Grimnir.ActionState Instance
			{
				get
				{
					if (Grimnir.ActionState.mSingelton == null)
					{
						lock (Grimnir.ActionState.mSingeltonLock)
						{
							if (Grimnir.ActionState.mSingelton == null)
							{
								Grimnir.ActionState.mSingelton = new Grimnir.ActionState();
							}
						}
					}
					return Grimnir.ActionState.mSingelton;
				}
			}

			// Token: 0x06001D90 RID: 7568 RVA: 0x000D1658 File Offset: 0x000CF858
			private ActionState()
			{
			}

			// Token: 0x06001D91 RID: 7569 RVA: 0x000D1660 File Offset: 0x000CF860
			public void OnEnter(Grimnir iOwner)
			{
				iOwner.mMinionsSpawned = false;
				iOwner.mSpawnDelay = 1f;
			}

			// Token: 0x06001D92 RID: 7570 RVA: 0x000D1674 File Offset: 0x000CF874
			public void OnUpdate(float iDeltaTime, Grimnir iOwner)
			{
				if (NetworkManager.Instance.State != NetworkState.Client)
				{
					iOwner.mSpawnDelay -= iDeltaTime;
					if (!iOwner.mMinionsSpawned && iOwner.mSpawnDelay <= 0f)
					{
						iOwner.SpawnMinions(iOwner.mCurrentActionState);
						iOwner.mMinionsSpawned = true;
						return;
					}
					if (iOwner.mMinionsSpawned && iOwner.mActiveMinions.Count == 0)
					{
						iOwner.mHitPoints -= 1f;
						if (iOwner.mHitPoints <= 0f || iOwner.mCurrentActionState == Grimnir.ActionStates.Ruins)
						{
							iOwner.ChangeState(Grimnir.States.OutroFade);
							return;
						}
						iOwner.ChangeState(Grimnir.States.FadeTransition);
					}
				}
			}

			// Token: 0x06001D93 RID: 7571 RVA: 0x000D1714 File Offset: 0x000CF914
			public void OnExit(Grimnir iOwner)
			{
			}

			// Token: 0x04002019 RID: 8217
			private static Grimnir.ActionState mSingelton;

			// Token: 0x0400201A RID: 8218
			private static volatile object mSingeltonLock = new object();
		}

		// Token: 0x020003BD RID: 957
		public class FadeTransitionState : IBossState<Grimnir>
		{
			// Token: 0x1700074D RID: 1869
			// (get) Token: 0x06001D95 RID: 7573 RVA: 0x000D1724 File Offset: 0x000CF924
			public static Grimnir.FadeTransitionState Instance
			{
				get
				{
					if (Grimnir.FadeTransitionState.mSingelton == null)
					{
						lock (Grimnir.FadeTransitionState.mSingeltonLock)
						{
							if (Grimnir.FadeTransitionState.mSingelton == null)
							{
								Grimnir.FadeTransitionState.mSingelton = new Grimnir.FadeTransitionState();
							}
						}
					}
					return Grimnir.FadeTransitionState.mSingelton;
				}
			}

			// Token: 0x06001D96 RID: 7574 RVA: 0x000D1778 File Offset: 0x000CF978
			private FadeTransitionState()
			{
			}

			// Token: 0x06001D97 RID: 7575 RVA: 0x000D1780 File Offset: 0x000CF980
			public void OnEnter(Grimnir iOwner)
			{
				iOwner.mFadeIn = true;
				iOwner.mFadeTimer = 0.25f;
				iOwner.mTransitionBegun = false;
				iOwner.mDreamCompleteTimer = 1.5f;
				iOwner.mNextActionState++;
			}

			// Token: 0x06001D98 RID: 7576 RVA: 0x000D17B4 File Offset: 0x000CF9B4
			public unsafe void OnUpdate(float iDeltaTime, Grimnir iOwner)
			{
				if (NetworkManager.Instance.State != NetworkState.Client)
				{
					if (!iOwner.mTransitionBegun)
					{
						iOwner.mDreamCompleteTimer -= iDeltaTime;
						if (iOwner.mDreamCompleteTimer <= 0f)
						{
							RenderManager.Instance.BeginTransition(Transitions.Fade, Color.White, iOwner.mFadeTimer);
							iOwner.mTransitionBegun = true;
							return;
						}
					}
					else if (iOwner.mFadeIn)
					{
						iOwner.mFadeTimer -= iDeltaTime;
						if (iOwner.mFadeTimer - 0.2f <= 0f)
						{
							iOwner.mFadeIn = false;
							iOwner.mFadeTimer = 0.25f;
							int mNextActionState = (int)iOwner.mNextActionState;
							iOwner.TeleportPlayers(mNextActionState);
							Vector3 position = iOwner.StateCameraTarget(iOwner.mNextActionState);
							iOwner.mPlayState.Camera.SetPosition(ref position);
							if (NetworkManager.Instance.State == NetworkState.Server)
							{
								Grimnir.CameraMessage cameraMessage;
								cameraMessage.Position = position;
								BossFight.Instance.SendMessage<Grimnir.CameraMessage>(iOwner, 4, (void*)(&cameraMessage), true);
							}
							RenderManager.Instance.EndTransition(Transitions.Fade, Color.White, iOwner.mFadeTimer);
							return;
						}
					}
					else
					{
						iOwner.mFadeTimer -= iDeltaTime;
						if (iOwner.mFadeTimer - 0.2f <= 0f)
						{
							iOwner.mCurrentActionState = iOwner.mNextActionState;
							iOwner.ChangeState(Grimnir.States.Action);
						}
					}
				}
			}

			// Token: 0x06001D99 RID: 7577 RVA: 0x000D18F3 File Offset: 0x000CFAF3
			public void OnExit(Grimnir iOwner)
			{
			}

			// Token: 0x0400201B RID: 8219
			private static Grimnir.FadeTransitionState mSingelton;

			// Token: 0x0400201C RID: 8220
			private static volatile object mSingeltonLock = new object();
		}

		// Token: 0x020003BE RID: 958
		public enum MessageType : ushort
		{
			// Token: 0x0400201E RID: 8222
			Update,
			// Token: 0x0400201F RID: 8223
			Teleport,
			// Token: 0x04002020 RID: 8224
			ChangeState,
			// Token: 0x04002021 RID: 8225
			Spawn,
			// Token: 0x04002022 RID: 8226
			Camera
		}

		// Token: 0x020003BF RID: 959
		internal struct UpdateMessage
		{
			// Token: 0x04002023 RID: 8227
			public const ushort TYPE = 0;

			// Token: 0x04002024 RID: 8228
			public float HitPoints;
		}

		// Token: 0x020003C0 RID: 960
		internal struct CameraMessage
		{
			// Token: 0x04002025 RID: 8229
			public const ushort TYPE = 4;

			// Token: 0x04002026 RID: 8230
			public Vector3 Position;
		}

		// Token: 0x020003C1 RID: 961
		internal struct SpawnMessage
		{
			// Token: 0x04002027 RID: 8231
			public const ushort TYPE = 3;

			// Token: 0x04002028 RID: 8232
			public ushort Handle;

			// Token: 0x04002029 RID: 8233
			public int TypeID;

			// Token: 0x0400202A RID: 8234
			public Vector3 Position;

			// Token: 0x0400202B RID: 8235
			public Vector3 Direction;
		}

		// Token: 0x020003C2 RID: 962
		internal struct TeleportMessage
		{
			// Token: 0x0400202C RID: 8236
			public const ushort TYPE = 1;

			// Token: 0x0400202D RID: 8237
			public int Venue;
		}

		// Token: 0x020003C3 RID: 963
		internal struct ChangeStateMessage
		{
			// Token: 0x0400202E RID: 8238
			public const ushort TYPE = 2;

			// Token: 0x0400202F RID: 8239
			public Grimnir.States State;
		}

		// Token: 0x020003C4 RID: 964
		public enum ActionStates
		{
			// Token: 0x04002031 RID: 8241
			Mountaindale,
			// Token: 0x04002032 RID: 8242
			Highlands,
			// Token: 0x04002033 RID: 8243
			Havindr,
			// Token: 0x04002034 RID: 8244
			Mines,
			// Token: 0x04002035 RID: 8245
			Swamp,
			// Token: 0x04002036 RID: 8246
			Ruins,
			// Token: 0x04002037 RID: 8247
			NrOfStates
		}

		// Token: 0x020003C5 RID: 965
		public enum States
		{
			// Token: 0x04002039 RID: 8249
			IntroFade,
			// Token: 0x0400203A RID: 8250
			OutroFade,
			// Token: 0x0400203B RID: 8251
			Action,
			// Token: 0x0400203C RID: 8252
			FadeTransition
		}

		// Token: 0x020003C6 RID: 966
		private enum Animations
		{
			// Token: 0x0400203E RID: 8254
			Idle,
			// Token: 0x0400203F RID: 8255
			NrOfAnimations
		}
	}
}
