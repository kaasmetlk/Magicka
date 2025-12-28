using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Magicka.AI;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.Entities.Bosses;
using Magicka.GameLogic.Entities.CharacterStates;
using Magicka.Network;
using Microsoft.Xna.Framework;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x020001A6 RID: 422
	public class Spawn : Action
	{
		// Token: 0x06000C76 RID: 3190 RVA: 0x0004AA04 File Offset: 0x00048C04
		public Spawn(Trigger iTrigger, GameScene iScene, XmlNode iNode) : base(iTrigger, iScene)
		{
			this.mNode = iNode;
		}

		// Token: 0x06000C77 RID: 3191 RVA: 0x0004AA70 File Offset: 0x00048C70
		public float GetTotalHitPoins()
		{
			float num = 0f;
			for (int i = 0; i < this.mAmount; i++)
			{
				CharacterTemplate cachedTemplate = CharacterTemplate.GetCachedTemplate(this.mTypeID);
				num += cachedTemplate.MaxHitpoints;
			}
			return num;
		}

		// Token: 0x06000C78 RID: 3192 RVA: 0x0004AAAC File Offset: 0x00048CAC
		public override void Initialize()
		{
			base.Initialize();
			base.GameScene.PlayState.Content.Load<CharacterTemplate>("Data/Characters/" + this.mType);
			LevelModel levelModel = base.GameScene.LevelModel;
			List<AIEvent> list = new List<AIEvent>();
			foreach (object obj in this.mNode.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				if (!(xmlNode is XmlComment))
				{
					list.Add(new AIEvent(levelModel, xmlNode));
				}
			}
			this.mEvents = list.ToArray();
		}

		// Token: 0x06000C79 RID: 3193 RVA: 0x0004AB68 File Offset: 0x00048D68
		protected override void Execute()
		{
			if (NetworkManager.Instance.State == NetworkState.Client)
			{
				return;
			}
			CharacterTemplate cachedTemplate = CharacterTemplate.GetCachedTemplate(this.mTypeID);
			for (int i = 0; i < this.mAmount; i++)
			{
				NonPlayerCharacter instance = NonPlayerCharacter.GetInstance(base.GameScene.PlayState);
				if (instance == null)
				{
					return;
				}
				if (instance is GenericBoss)
				{
					int num = 0;
					while (instance == null || instance is GenericBoss)
					{
						num++;
						instance = NonPlayerCharacter.GetInstance(base.GameScene.PlayState);
					}
				}
				Matrix matrix;
				base.GameScene.GetLocator(this.mAreaID, out matrix);
				if (this.mSnapToNavMesh)
				{
					Vector3 translation = matrix.Translation;
					Vector3 translation2;
					base.GameScene.LevelModel.NavMesh.GetNearestPosition(ref translation, out translation2, cachedTemplate.MoveAbilities);
					matrix.Translation = translation2;
				}
				instance.Initialize(cachedTemplate, this.mMeshIdx, matrix.Translation, this.mUniqueID);
				if (this.mForceDraw)
				{
					instance.ForceDraw();
				}
				instance.HitPoints = instance.MaxHitPoints * this.mHealth;
				instance.OnDeathTrigger = this.mOnDeathTriggerID;
				instance.OnDamageTrigger = this.mOnDamageTriggerID;
				Agent ai = instance.AI;
				ai.WanderSpeed = this.mWanderSpeed;
				ai.TargetArea = this.mTargetAreaID;
				ai.SetOrder(this.mOrder, this.mReactTo, this.mReaction, this.mPriorityTargetHash, this.mPriorityAbility, this.mTriggerID, this.mEvents);
				instance.Dialog = this.mDialogID;
				Matrix orientation = matrix;
				orientation.Translation = default(Vector3);
				instance.CharacterBody.Orientation = orientation;
				instance.CharacterBody.DesiredDirection = orientation.Forward;
				instance.RemoveAfterDeath = !this.mNeverRemove;
				instance.ForceCamera = this.mForceCamera;
				instance.ForceNavMesh = this.mForceNavMesh;
				instance.CannotDieWithoutExplicitKill = this.mCannotDieWithoutExplicitKill;
				if (this.mAnimation != Animations.None)
				{
					instance.ForceAnimation(this.mAnimation);
				}
				if (this.mSpawnAnimation != Animations.None && this.mSpawnAnimation != Animations.idle && this.mSpawnAnimation != Animations.idle_agg)
				{
					instance.SpawnAnimation = this.mSpawnAnimation;
					instance.ChangeState(RessurectionState.Instance);
				}
				if (this.mSpecialIdleAnimation != Animations.None)
				{
					instance.SpecialIdleAnimation = this.mSpecialIdleAnimation;
					if (!(instance.CurrentState is RessurectionState))
					{
						instance.ForceAnimation(this.mSpecialIdleAnimation);
					}
				}
				if (base.GameScene.RuleSet != null && base.GameScene.RuleSet is SurvivalRuleset)
				{
					instance.Faction = Factions.EVIL;
					(base.GameScene.RuleSet as SurvivalRuleset).AddedCharacter(instance, false);
				}
				base.GameScene.PlayState.EntityManager.AddEntity(instance);
				Spawn.HandleAndHealth item = new Spawn.HandleAndHealth(instance.Handle, instance.HitPoints);
				this.mSpawnedEntityHP.Add(item);
				if (NetworkManager.Instance.State == NetworkState.Server)
				{
					TriggerActionMessage triggerActionMessage = default(TriggerActionMessage);
					triggerActionMessage.ActionType = TriggerActionType.SpawnNPC;
					triggerActionMessage.Handle = instance.Handle;
					triggerActionMessage.Template = this.mTypeID;
					triggerActionMessage.Id = this.mUniqueID;
					triggerActionMessage.Position = instance.Position;
					triggerActionMessage.Direction = orientation.Forward;
					triggerActionMessage.Bool0 = this.mForceDraw;
					triggerActionMessage.Point0 = this.mDialogID;
					triggerActionMessage.Point1 = (int)this.mAnimation;
					triggerActionMessage.Point2 = (int)this.mSpawnAnimation;
					triggerActionMessage.Point3 = (int)this.mSpecialIdleAnimation;
					triggerActionMessage.Arg = this.mMeshIdx;
					triggerActionMessage.Color = instance.Color;
					NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref triggerActionMessage);
				}
			}
		}

		// Token: 0x06000C7A RID: 3194 RVA: 0x0004AF04 File Offset: 0x00049104
		public override void QuickExecute()
		{
			if (NetworkManager.Instance.State == NetworkState.Client)
			{
				return;
			}
			CharacterTemplate cachedTemplate = CharacterTemplate.GetCachedTemplate(this.mTypeID);
			for (int i = 0; i < this.mSpawnedEntityHP.Count; i++)
			{
				Spawn.HandleAndHealth value = this.mSpawnedEntityHP[i];
				NonPlayerCharacter instance = NonPlayerCharacter.GetInstance(base.GameScene.PlayState);
				Matrix matrix;
				base.GameScene.GetLocator(this.mAreaID, out matrix);
				if (this.mSnapToNavMesh)
				{
					Vector3 translation = matrix.Translation;
					Vector3 translation2;
					base.GameScene.LevelModel.NavMesh.GetNearestPosition(ref translation, out translation2, cachedTemplate.MoveAbilities);
					matrix.Translation = translation2;
				}
				instance.Initialize(cachedTemplate, this.mMeshIdx, matrix.Translation, this.mUniqueID);
				instance.OnDeathTrigger = this.mOnDeathTriggerID;
				instance.OnDamageTrigger = this.mOnDamageTriggerID;
				Agent ai = instance.AI;
				ai.SetOrder(this.mOrder, this.mReactTo, this.mReaction, this.mPriorityTargetHash, this.mPriorityAbility, this.mTriggerID, this.mEvents);
				instance.Dialog = this.mDialogID;
				Matrix orientation = matrix;
				orientation.Translation = default(Vector3);
				instance.CharacterBody.Orientation = orientation;
				instance.CharacterBody.DesiredDirection = orientation.Forward;
				instance.RemoveAfterDeath = !this.mNeverRemove;
				if (this.mAnimation != Animations.None)
				{
					instance.ForceAnimation(this.mAnimation);
				}
				if (this.mSpawnAnimation != Animations.None && this.mSpawnAnimation != Animations.idle && this.mSpawnAnimation != Animations.idle_agg)
				{
					instance.SpawnAnimation = this.mSpawnAnimation;
					instance.ChangeState(RessurectionState.Instance);
				}
				if (this.mSpecialIdleAnimation != Animations.None)
				{
					instance.SpecialIdleAnimation = this.mSpecialIdleAnimation;
				}
				if (base.GameScene.RuleSet is SurvivalRuleset)
				{
					instance.Faction = Factions.EVIL;
					(base.GameScene.RuleSet as SurvivalRuleset).AddedCharacter(instance, false);
				}
				base.GameScene.PlayState.EntityManager.AddEntity(instance);
				instance.HitPoints = value.Health;
				value.Handle = instance.Handle;
				this.mSpawnedEntityHP[i] = value;
				if (NetworkManager.Instance.State == NetworkState.Server)
				{
					TriggerActionMessage triggerActionMessage = default(TriggerActionMessage);
					triggerActionMessage.ActionType = TriggerActionType.SpawnNPC;
					triggerActionMessage.Handle = instance.Handle;
					triggerActionMessage.Template = this.mTypeID;
					triggerActionMessage.Id = this.mUniqueID;
					triggerActionMessage.Position = instance.Position;
					triggerActionMessage.Direction = orientation.Forward;
					triggerActionMessage.Point0 = this.mDialogID;
					triggerActionMessage.Point1 = (int)this.mAnimation;
					triggerActionMessage.Point2 = (int)this.mSpawnAnimation;
					triggerActionMessage.Point3 = (int)this.mSpecialIdleAnimation;
					NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref triggerActionMessage);
				}
			}
		}

		// Token: 0x06000C7B RID: 3195 RVA: 0x0004B1F0 File Offset: 0x000493F0
		public override void Update(float iDeltaTime)
		{
			for (int i = 0; i < this.mSpawnedEntityHP.Count; i++)
			{
				Spawn.HandleAndHealth value = this.mSpawnedEntityHP[i];
				if (value.Handle != 65535)
				{
					Character character = Entity.GetFromHandle((int)value.Handle) as Character;
					value.Health = character.HitPoints;
					if (value.Health <= 0f)
					{
						this.mSpawnedEntityHP.RemoveAt(i);
						i--;
					}
					else
					{
						this.mSpawnedEntityHP[i] = value;
					}
				}
			}
			base.Update(iDeltaTime);
		}

		// Token: 0x170002E5 RID: 741
		// (get) Token: 0x06000C7C RID: 3196 RVA: 0x0004B281 File Offset: 0x00049481
		// (set) Token: 0x06000C7D RID: 3197 RVA: 0x0004B289 File Offset: 0x00049489
		public string Area
		{
			get
			{
				return this.mArea;
			}
			set
			{
				this.mArea = value;
				this.mAreaID = this.mArea.GetHashCodeCustom();
			}
		}

		// Token: 0x170002E6 RID: 742
		// (get) Token: 0x06000C7E RID: 3198 RVA: 0x0004B2A3 File Offset: 0x000494A3
		// (set) Token: 0x06000C7F RID: 3199 RVA: 0x0004B2AB File Offset: 0x000494AB
		public bool SnapToNavMesh
		{
			get
			{
				return this.mSnapToNavMesh;
			}
			set
			{
				this.mSnapToNavMesh = value;
			}
		}

		// Token: 0x170002E7 RID: 743
		// (get) Token: 0x06000C80 RID: 3200 RVA: 0x0004B2B4 File Offset: 0x000494B4
		// (set) Token: 0x06000C81 RID: 3201 RVA: 0x0004B2BC File Offset: 0x000494BC
		public string TargetArea
		{
			get
			{
				return this.mTargetArea;
			}
			set
			{
				this.mTargetArea = value;
				this.mTargetAreaID = this.mTargetArea.GetHashCodeCustom();
				if (this.mTargetAreaID == GiveOrder.ANYID)
				{
					this.mTargetAreaID = 0;
				}
			}
		}

		// Token: 0x170002E8 RID: 744
		// (get) Token: 0x06000C82 RID: 3202 RVA: 0x0004B2EA File Offset: 0x000494EA
		// (set) Token: 0x06000C83 RID: 3203 RVA: 0x0004B2F2 File Offset: 0x000494F2
		public float Speed
		{
			get
			{
				return this.mWanderSpeed;
			}
			set
			{
				this.mWanderSpeed = value;
			}
		}

		// Token: 0x170002E9 RID: 745
		// (get) Token: 0x06000C84 RID: 3204 RVA: 0x0004B2FB File Offset: 0x000494FB
		// (set) Token: 0x06000C85 RID: 3205 RVA: 0x0004B303 File Offset: 0x00049503
		public string Type
		{
			get
			{
				return this.mType;
			}
			set
			{
				this.mType = value;
				this.mTypeID = this.mType.GetHashCodeCustom();
			}
		}

		// Token: 0x170002EA RID: 746
		// (get) Token: 0x06000C86 RID: 3206 RVA: 0x0004B31D File Offset: 0x0004951D
		// (set) Token: 0x06000C87 RID: 3207 RVA: 0x0004B325 File Offset: 0x00049525
		public int Nr
		{
			get
			{
				return this.mAmount;
			}
			set
			{
				this.mAmount = value;
			}
		}

		// Token: 0x170002EB RID: 747
		// (get) Token: 0x06000C88 RID: 3208 RVA: 0x0004B32E File Offset: 0x0004952E
		// (set) Token: 0x06000C89 RID: 3209 RVA: 0x0004B336 File Offset: 0x00049536
		public float Health
		{
			get
			{
				return this.mHealth;
			}
			set
			{
				this.mHealth = value;
				if (this.mHealth > 1f || this.mHealth < 0f)
				{
					throw new Exception("Health must be between 0.0 and 1.0!");
				}
			}
		}

		// Token: 0x170002EC RID: 748
		// (get) Token: 0x06000C8A RID: 3210 RVA: 0x0004B364 File Offset: 0x00049564
		// (set) Token: 0x06000C8B RID: 3211 RVA: 0x0004B36C File Offset: 0x0004956C
		public string ID
		{
			get
			{
				return this.mUniqueName;
			}
			set
			{
				this.mUniqueName = value;
				this.mUniqueID = this.mUniqueName.GetHashCodeCustom();
			}
		}

		// Token: 0x170002ED RID: 749
		// (get) Token: 0x06000C8C RID: 3212 RVA: 0x0004B386 File Offset: 0x00049586
		// (set) Token: 0x06000C8D RID: 3213 RVA: 0x0004B38E File Offset: 0x0004958E
		public string Dialog
		{
			get
			{
				return this.mDialog;
			}
			set
			{
				this.mDialog = value;
				this.mDialogID = this.mDialog.GetHashCodeCustom();
			}
		}

		// Token: 0x170002EE RID: 750
		// (get) Token: 0x06000C8E RID: 3214 RVA: 0x0004B3A8 File Offset: 0x000495A8
		// (set) Token: 0x06000C8F RID: 3215 RVA: 0x0004B3B0 File Offset: 0x000495B0
		public Order Order
		{
			get
			{
				return this.mOrder;
			}
			set
			{
				this.mOrder = value;
			}
		}

		// Token: 0x170002EF RID: 751
		// (get) Token: 0x06000C90 RID: 3216 RVA: 0x0004B3B9 File Offset: 0x000495B9
		// (set) Token: 0x06000C91 RID: 3217 RVA: 0x0004B3C1 File Offset: 0x000495C1
		public ReactTo ReactTo
		{
			get
			{
				return this.mReactTo;
			}
			set
			{
				this.mReactTo = value;
			}
		}

		// Token: 0x170002F0 RID: 752
		// (get) Token: 0x06000C92 RID: 3218 RVA: 0x0004B3CA File Offset: 0x000495CA
		// (set) Token: 0x06000C93 RID: 3219 RVA: 0x0004B3D2 File Offset: 0x000495D2
		public Order Reaction
		{
			get
			{
				return this.mReaction;
			}
			set
			{
				this.mReaction = value;
			}
		}

		// Token: 0x170002F1 RID: 753
		// (get) Token: 0x06000C94 RID: 3220 RVA: 0x0004B3DB File Offset: 0x000495DB
		// (set) Token: 0x06000C95 RID: 3221 RVA: 0x0004B3E3 File Offset: 0x000495E3
		public new string Trigger
		{
			get
			{
				return this.mTrigger;
			}
			set
			{
				this.mTrigger = value;
				this.mTriggerID = this.mTrigger.GetHashCodeCustom();
			}
		}

		// Token: 0x170002F2 RID: 754
		// (get) Token: 0x06000C96 RID: 3222 RVA: 0x0004B3FD File Offset: 0x000495FD
		// (set) Token: 0x06000C97 RID: 3223 RVA: 0x0004B405 File Offset: 0x00049605
		public bool NeverRemove
		{
			get
			{
				return this.mNeverRemove;
			}
			set
			{
				this.mNeverRemove = value;
			}
		}

		// Token: 0x170002F3 RID: 755
		// (get) Token: 0x06000C98 RID: 3224 RVA: 0x0004B40E File Offset: 0x0004960E
		// (set) Token: 0x06000C99 RID: 3225 RVA: 0x0004B416 File Offset: 0x00049616
		public Animations Animation
		{
			get
			{
				return this.mAnimation;
			}
			set
			{
				this.mAnimation = value;
			}
		}

		// Token: 0x170002F4 RID: 756
		// (get) Token: 0x06000C9A RID: 3226 RVA: 0x0004B41F File Offset: 0x0004961F
		// (set) Token: 0x06000C9B RID: 3227 RVA: 0x0004B427 File Offset: 0x00049627
		public Animations IdleAnimation
		{
			get
			{
				return this.mSpecialIdleAnimation;
			}
			set
			{
				this.mSpecialIdleAnimation = value;
			}
		}

		// Token: 0x170002F5 RID: 757
		// (get) Token: 0x06000C9C RID: 3228 RVA: 0x0004B430 File Offset: 0x00049630
		// (set) Token: 0x06000C9D RID: 3229 RVA: 0x0004B438 File Offset: 0x00049638
		public Animations SpawnAnimation
		{
			get
			{
				return this.mSpawnAnimation;
			}
			set
			{
				this.mSpawnAnimation = value;
			}
		}

		// Token: 0x170002F6 RID: 758
		// (get) Token: 0x06000C9E RID: 3230 RVA: 0x0004B441 File Offset: 0x00049641
		// (set) Token: 0x06000C9F RID: 3231 RVA: 0x0004B44E File Offset: 0x0004964E
		protected override object Tag
		{
			get
			{
				return new List<Spawn.HandleAndHealth>(this.mSpawnedEntityHP);
			}
			set
			{
				this.mSpawnedEntityHP = new List<Spawn.HandleAndHealth>(value as List<Spawn.HandleAndHealth>);
			}
		}

		// Token: 0x06000CA0 RID: 3232 RVA: 0x0004B464 File Offset: 0x00049664
		protected override void WriteTag(BinaryWriter iWriter, object mTag)
		{
			List<Spawn.HandleAndHealth> list = mTag as List<Spawn.HandleAndHealth>;
			iWriter.Write(list.Count);
			foreach (Spawn.HandleAndHealth handleAndHealth in list)
			{
				iWriter.Write(handleAndHealth.Health);
			}
		}

		// Token: 0x06000CA1 RID: 3233 RVA: 0x0004B4CC File Offset: 0x000496CC
		protected override object ReadTag(BinaryReader iReader)
		{
			int num = iReader.ReadInt32();
			List<Spawn.HandleAndHealth> list = new List<Spawn.HandleAndHealth>(num);
			for (int i = 0; i < num; i++)
			{
				list.Add(new Spawn.HandleAndHealth(ushort.MaxValue, iReader.ReadSingle()));
			}
			return list;
		}

		// Token: 0x170002F7 RID: 759
		// (set) Token: 0x06000CA2 RID: 3234 RVA: 0x0004B50A File Offset: 0x0004970A
		public string PriorityTarget
		{
			set
			{
				this.mPriorityTarget = value;
				this.mPriorityTargetHash = this.mPriorityTarget.GetHashCodeCustom();
			}
		}

		// Token: 0x170002F8 RID: 760
		// (set) Token: 0x06000CA3 RID: 3235 RVA: 0x0004B524 File Offset: 0x00049724
		public int PriorityAbility
		{
			set
			{
				this.mPriorityAbility = value;
			}
		}

		// Token: 0x170002F9 RID: 761
		// (get) Token: 0x06000CA4 RID: 3236 RVA: 0x0004B52D File Offset: 0x0004972D
		// (set) Token: 0x06000CA5 RID: 3237 RVA: 0x0004B535 File Offset: 0x00049735
		public string OnDeath
		{
			get
			{
				return this.mOnDeathTrigger;
			}
			set
			{
				this.mOnDeathTrigger = value;
				this.mOnDeathTriggerID = value.GetHashCodeCustom();
			}
		}

		// Token: 0x170002FA RID: 762
		// (get) Token: 0x06000CA6 RID: 3238 RVA: 0x0004B54A File Offset: 0x0004974A
		// (set) Token: 0x06000CA7 RID: 3239 RVA: 0x0004B552 File Offset: 0x00049752
		public string OnDamage
		{
			get
			{
				return this.mOnDamageTrigger;
			}
			set
			{
				this.mOnDamageTrigger = value;
				this.mOnDamageTriggerID = value.GetHashCodeCustom();
			}
		}

		// Token: 0x170002FB RID: 763
		// (get) Token: 0x06000CA8 RID: 3240 RVA: 0x0004B567 File Offset: 0x00049767
		// (set) Token: 0x06000CA9 RID: 3241 RVA: 0x0004B56F File Offset: 0x0004976F
		public bool ForceDraw
		{
			get
			{
				return this.mForceDraw;
			}
			set
			{
				this.mForceDraw = value;
			}
		}

		// Token: 0x170002FC RID: 764
		// (get) Token: 0x06000CAA RID: 3242 RVA: 0x0004B578 File Offset: 0x00049778
		// (set) Token: 0x06000CAB RID: 3243 RVA: 0x0004B580 File Offset: 0x00049780
		public int MeshId
		{
			get
			{
				return this.mMeshIdx;
			}
			set
			{
				this.mMeshIdx = value;
			}
		}

		// Token: 0x170002FD RID: 765
		// (get) Token: 0x06000CAC RID: 3244 RVA: 0x0004B589 File Offset: 0x00049789
		// (set) Token: 0x06000CAD RID: 3245 RVA: 0x0004B591 File Offset: 0x00049791
		public bool ForceCamera
		{
			get
			{
				return this.mForceCamera;
			}
			set
			{
				this.mForceCamera = value;
			}
		}

		// Token: 0x170002FE RID: 766
		// (get) Token: 0x06000CAE RID: 3246 RVA: 0x0004B59A File Offset: 0x0004979A
		// (set) Token: 0x06000CAF RID: 3247 RVA: 0x0004B5A2 File Offset: 0x000497A2
		public bool ForceNavMesh
		{
			get
			{
				return this.mForceNavMesh;
			}
			set
			{
				this.mForceNavMesh = value;
			}
		}

		// Token: 0x170002FF RID: 767
		// (get) Token: 0x06000CB0 RID: 3248 RVA: 0x0004B5AB File Offset: 0x000497AB
		// (set) Token: 0x06000CB1 RID: 3249 RVA: 0x0004B5B3 File Offset: 0x000497B3
		public bool CannotDieWithoutExplicitKill
		{
			get
			{
				return this.mCannotDieWithoutExplicitKill;
			}
			set
			{
				this.mCannotDieWithoutExplicitKill = value;
			}
		}

		// Token: 0x04000B72 RID: 2930
		private XmlNode mNode;

		// Token: 0x04000B73 RID: 2931
		private List<Spawn.HandleAndHealth> mSpawnedEntityHP = new List<Spawn.HandleAndHealth>(10);

		// Token: 0x04000B74 RID: 2932
		private bool mSnapToNavMesh;

		// Token: 0x04000B75 RID: 2933
		private string mArea;

		// Token: 0x04000B76 RID: 2934
		private int mAreaID;

		// Token: 0x04000B77 RID: 2935
		private string mType;

		// Token: 0x04000B78 RID: 2936
		private int mTypeID;

		// Token: 0x04000B79 RID: 2937
		private int mAmount = 1;

		// Token: 0x04000B7A RID: 2938
		private string mUniqueName;

		// Token: 0x04000B7B RID: 2939
		private int mUniqueID;

		// Token: 0x04000B7C RID: 2940
		private string mDialog;

		// Token: 0x04000B7D RID: 2941
		private int mDialogID;

		// Token: 0x04000B7E RID: 2942
		private bool mForceDraw;

		// Token: 0x04000B7F RID: 2943
		private float mHealth = 1f;

		// Token: 0x04000B80 RID: 2944
		private Order mOrder = Order.Attack;

		// Token: 0x04000B81 RID: 2945
		private Order mReaction = Order.Attack;

		// Token: 0x04000B82 RID: 2946
		private ReactTo mReactTo = ReactTo.Attack | ReactTo.Proximity;

		// Token: 0x04000B83 RID: 2947
		private new string mTrigger;

		// Token: 0x04000B84 RID: 2948
		private int mTriggerID;

		// Token: 0x04000B85 RID: 2949
		private AIEvent[] mEvents;

		// Token: 0x04000B86 RID: 2950
		private Animations mAnimation;

		// Token: 0x04000B87 RID: 2951
		private Animations mSpawnAnimation;

		// Token: 0x04000B88 RID: 2952
		private Animations mSpecialIdleAnimation;

		// Token: 0x04000B89 RID: 2953
		private bool mNeverRemove;

		// Token: 0x04000B8A RID: 2954
		private string mTargetArea;

		// Token: 0x04000B8B RID: 2955
		private int mTargetAreaID;

		// Token: 0x04000B8C RID: 2956
		private float mWanderSpeed = 1f;

		// Token: 0x04000B8D RID: 2957
		private string mPriorityTarget;

		// Token: 0x04000B8E RID: 2958
		private int mPriorityTargetHash;

		// Token: 0x04000B8F RID: 2959
		private int mPriorityAbility = -1;

		// Token: 0x04000B90 RID: 2960
		private int mMeshIdx = -1;

		// Token: 0x04000B91 RID: 2961
		private string mOnDeathTrigger;

		// Token: 0x04000B92 RID: 2962
		private int mOnDeathTriggerID;

		// Token: 0x04000B93 RID: 2963
		private string mOnDamageTrigger;

		// Token: 0x04000B94 RID: 2964
		private int mOnDamageTriggerID;

		// Token: 0x04000B95 RID: 2965
		private bool mForceCamera;

		// Token: 0x04000B96 RID: 2966
		private bool mForceNavMesh;

		// Token: 0x04000B97 RID: 2967
		private bool mCannotDieWithoutExplicitKill;

		// Token: 0x020001A7 RID: 423
		private struct HandleAndHealth
		{
			// Token: 0x06000CB2 RID: 3250 RVA: 0x0004B5BC File Offset: 0x000497BC
			public HandleAndHealth(ushort iHandle, float iHealth)
			{
				this.Handle = iHandle;
				this.Health = iHealth;
			}

			// Token: 0x04000B98 RID: 2968
			public ushort Handle;

			// Token: 0x04000B99 RID: 2969
			public float Health;
		}
	}
}
