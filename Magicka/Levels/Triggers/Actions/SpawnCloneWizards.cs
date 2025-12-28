using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Magicka.AI;
using Magicka.GameLogic;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.Entities.CharacterStates;
using Magicka.Network;
using Microsoft.Xna.Framework;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x0200023F RID: 575
	internal class SpawnCloneWizards : Action
	{
		// Token: 0x060011A7 RID: 4519 RVA: 0x0006C284 File Offset: 0x0006A484
		public SpawnCloneWizards(Trigger iTrigger, GameScene iScene, XmlNode iNode) : base(iTrigger, iScene)
		{
			this.mNode = iNode;
		}

		// Token: 0x060011A8 RID: 4520 RVA: 0x0006C304 File Offset: 0x0006A504
		public float GetTotalHitPoins()
		{
			float num = 0f;
			for (int i = 0; i < this.mAmount; i++)
			{
				num += this.mTemplate.MaxHitpoints;
			}
			return num;
		}

		// Token: 0x060011A9 RID: 4521 RVA: 0x0006C338 File Offset: 0x0006A538
		public override void Initialize()
		{
			base.Initialize();
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

		// Token: 0x060011AA RID: 4522 RVA: 0x0006C3CC File Offset: 0x0006A5CC
		protected override void Execute()
		{
			if (NetworkManager.Instance.State == NetworkState.Client)
			{
				return;
			}
			NonPlayerCharacter[] array = new NonPlayerCharacter[4];
			for (int i = 0; i < Game.Instance.Players.Length; i++)
			{
				Matrix identity = Matrix.Identity;
				Player player = Game.Instance.Players[i];
				if (player.Playing && player.Avatar != null && !player.Avatar.Dead)
				{
					base.GameScene.GetLocator(this.mAreaID[i], out identity);
					array[i] = NonPlayerCharacter.GetInstance(base.GameScene.PlayState);
					string iString = (this.mName + i).ToLowerInvariant();
					this.mUniqueID = iString.GetHashCodeCustom();
					this.mTemplate = player.Avatar.Template;
					this.mTypeID = player.Avatar.Template.ID;
					array[i].Initialize(this.mTemplate, i, identity.Translation, this.mUniqueID);
					array[i].Color = Defines.PLAYERCOLORS[(int)player.Color];
					array[i].HitPoints = player.Avatar.MaxHitPoints;
					array[i].OnDeathTrigger = this.mOnDeathTriggerID;
					array[i].OnDamageTrigger = this.mOnDamageTriggerID;
					Agent ai = array[i].AI;
					ai.WanderSpeed = this.mWanderSpeed;
					ai.TargetArea = this.mTargetAreaID;
					ai.SetOrder(this.mOrder, this.mReactTo, this.mReaction, this.mPriorityTargetHash, this.mPriorityAbility, this.mTriggerID, this.mEvents);
					array[i].Dialog = this.mDialogID;
					Matrix orientation = identity;
					orientation.Translation = default(Vector3);
					array[i].CharacterBody.Orientation = orientation;
					array[i].CharacterBody.DesiredDirection = orientation.Forward;
					array[i].RemoveAfterDeath = !this.mNeverRemove;
					if (this.mSpawnAnimation != Animations.None && this.mSpawnAnimation != Animations.idle && this.mSpawnAnimation != Animations.idle_agg)
					{
						array[i].SpawnAnimation = this.mSpawnAnimation;
						array[i].ChangeState(RessurectionState.Instance);
					}
					if (this.mSpecialIdleAnimation != Animations.None)
					{
						array[i].SpecialIdleAnimation = this.mSpecialIdleAnimation;
						if (!(array[i].CurrentState is RessurectionState))
						{
							array[i].ForceAnimation(this.mSpecialIdleAnimation);
						}
					}
					if (base.GameScene.RuleSet != null && base.GameScene.RuleSet is SurvivalRuleset)
					{
						array[i].Faction = Factions.EVIL;
						(base.GameScene.RuleSet as SurvivalRuleset).AddedCharacter(array[i], false);
					}
					base.GameScene.PlayState.EntityManager.AddEntity(array[i]);
					SpawnCloneWizards.HandleAndHealth item = new SpawnCloneWizards.HandleAndHealth((int)array[i].Handle, array[i].HitPoints);
					this.mSpawnedEntityHP.Add(item);
					if (NetworkManager.Instance.State == NetworkState.Server)
					{
						TriggerActionMessage triggerActionMessage = default(TriggerActionMessage);
						triggerActionMessage.ActionType = TriggerActionType.SpawnNPC;
						triggerActionMessage.Handle = array[i].Handle;
						triggerActionMessage.Template = this.mTypeID;
						triggerActionMessage.Id = this.mUniqueID;
						triggerActionMessage.Position = identity.Translation;
						triggerActionMessage.Direction = orientation.Forward;
						triggerActionMessage.Point0 = this.mDialogID;
						triggerActionMessage.Point1 = 0;
						triggerActionMessage.Point2 = (int)this.mSpawnAnimation;
						triggerActionMessage.Point3 = (int)this.mSpecialIdleAnimation;
						triggerActionMessage.Arg = i;
						triggerActionMessage.Color = array[i].Color;
						NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref triggerActionMessage);
					}
				}
			}
		}

		// Token: 0x060011AB RID: 4523 RVA: 0x0006C76D File Offset: 0x0006A96D
		public override void QuickExecute()
		{
			this.Execute();
		}

		// Token: 0x17000485 RID: 1157
		// (get) Token: 0x060011AC RID: 4524 RVA: 0x0006C775 File Offset: 0x0006A975
		// (set) Token: 0x060011AD RID: 4525 RVA: 0x0006C780 File Offset: 0x0006A980
		public string Area
		{
			get
			{
				return this.mArea[0];
			}
			set
			{
				for (int i = 0; i < 4; i++)
				{
					this.mArea[i] = value + i;
					this.mAreaID[i] = this.mArea[i].GetHashCodeCustom();
				}
			}
		}

		// Token: 0x17000486 RID: 1158
		// (get) Token: 0x060011AE RID: 4526 RVA: 0x0006C7C2 File Offset: 0x0006A9C2
		// (set) Token: 0x060011AF RID: 4527 RVA: 0x0006C7CA File Offset: 0x0006A9CA
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

		// Token: 0x17000487 RID: 1159
		// (get) Token: 0x060011B0 RID: 4528 RVA: 0x0006C7D3 File Offset: 0x0006A9D3
		// (set) Token: 0x060011B1 RID: 4529 RVA: 0x0006C7DB File Offset: 0x0006A9DB
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

		// Token: 0x17000488 RID: 1160
		// (get) Token: 0x060011B2 RID: 4530 RVA: 0x0006C809 File Offset: 0x0006AA09
		// (set) Token: 0x060011B3 RID: 4531 RVA: 0x0006C811 File Offset: 0x0006AA11
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

		// Token: 0x17000489 RID: 1161
		// (get) Token: 0x060011B4 RID: 4532 RVA: 0x0006C81A File Offset: 0x0006AA1A
		// (set) Token: 0x060011B5 RID: 4533 RVA: 0x0006C822 File Offset: 0x0006AA22
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

		// Token: 0x1700048A RID: 1162
		// (get) Token: 0x060011B6 RID: 4534 RVA: 0x0006C82B File Offset: 0x0006AA2B
		// (set) Token: 0x060011B7 RID: 4535 RVA: 0x0006C833 File Offset: 0x0006AA33
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

		// Token: 0x1700048B RID: 1163
		// (get) Token: 0x060011B8 RID: 4536 RVA: 0x0006C861 File Offset: 0x0006AA61
		// (set) Token: 0x060011B9 RID: 4537 RVA: 0x0006C869 File Offset: 0x0006AA69
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

		// Token: 0x1700048C RID: 1164
		// (get) Token: 0x060011BA RID: 4538 RVA: 0x0006C883 File Offset: 0x0006AA83
		// (set) Token: 0x060011BB RID: 4539 RVA: 0x0006C88B File Offset: 0x0006AA8B
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

		// Token: 0x1700048D RID: 1165
		// (get) Token: 0x060011BC RID: 4540 RVA: 0x0006C8A5 File Offset: 0x0006AAA5
		// (set) Token: 0x060011BD RID: 4541 RVA: 0x0006C8AD File Offset: 0x0006AAAD
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

		// Token: 0x1700048E RID: 1166
		// (get) Token: 0x060011BE RID: 4542 RVA: 0x0006C8B6 File Offset: 0x0006AAB6
		// (set) Token: 0x060011BF RID: 4543 RVA: 0x0006C8BE File Offset: 0x0006AABE
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

		// Token: 0x1700048F RID: 1167
		// (get) Token: 0x060011C0 RID: 4544 RVA: 0x0006C8C7 File Offset: 0x0006AAC7
		// (set) Token: 0x060011C1 RID: 4545 RVA: 0x0006C8CF File Offset: 0x0006AACF
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

		// Token: 0x17000490 RID: 1168
		// (get) Token: 0x060011C2 RID: 4546 RVA: 0x0006C8D8 File Offset: 0x0006AAD8
		// (set) Token: 0x060011C3 RID: 4547 RVA: 0x0006C8E0 File Offset: 0x0006AAE0
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

		// Token: 0x17000491 RID: 1169
		// (get) Token: 0x060011C4 RID: 4548 RVA: 0x0006C8FA File Offset: 0x0006AAFA
		// (set) Token: 0x060011C5 RID: 4549 RVA: 0x0006C902 File Offset: 0x0006AB02
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

		// Token: 0x17000492 RID: 1170
		// (get) Token: 0x060011C6 RID: 4550 RVA: 0x0006C90B File Offset: 0x0006AB0B
		// (set) Token: 0x060011C7 RID: 4551 RVA: 0x0006C913 File Offset: 0x0006AB13
		public Animations Animation
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

		// Token: 0x17000493 RID: 1171
		// (get) Token: 0x060011C8 RID: 4552 RVA: 0x0006C91C File Offset: 0x0006AB1C
		// (set) Token: 0x060011C9 RID: 4553 RVA: 0x0006C924 File Offset: 0x0006AB24
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

		// Token: 0x17000494 RID: 1172
		// (get) Token: 0x060011CA RID: 4554 RVA: 0x0006C92D File Offset: 0x0006AB2D
		// (set) Token: 0x060011CB RID: 4555 RVA: 0x0006C93A File Offset: 0x0006AB3A
		protected override object Tag
		{
			get
			{
				return new List<SpawnCloneWizards.HandleAndHealth>(this.mSpawnedEntityHP);
			}
			set
			{
				this.mSpawnedEntityHP = new List<SpawnCloneWizards.HandleAndHealth>(value as List<SpawnCloneWizards.HandleAndHealth>);
			}
		}

		// Token: 0x060011CC RID: 4556 RVA: 0x0006C950 File Offset: 0x0006AB50
		protected override void WriteTag(BinaryWriter iWriter, object mTag)
		{
			List<SpawnCloneWizards.HandleAndHealth> list = mTag as List<SpawnCloneWizards.HandleAndHealth>;
			iWriter.Write(list.Count);
			foreach (SpawnCloneWizards.HandleAndHealth handleAndHealth in list)
			{
				iWriter.Write(handleAndHealth.Health);
			}
		}

		// Token: 0x060011CD RID: 4557 RVA: 0x0006C9B8 File Offset: 0x0006ABB8
		protected override object ReadTag(BinaryReader iReader)
		{
			int num = iReader.ReadInt32();
			List<SpawnCloneWizards.HandleAndHealth> list = new List<SpawnCloneWizards.HandleAndHealth>(num);
			for (int i = 0; i < num; i++)
			{
				list.Add(new SpawnCloneWizards.HandleAndHealth(65535, iReader.ReadSingle()));
			}
			return list;
		}

		// Token: 0x17000495 RID: 1173
		// (set) Token: 0x060011CE RID: 4558 RVA: 0x0006C9F6 File Offset: 0x0006ABF6
		public string PriorityTarget
		{
			set
			{
				this.mPriorityTarget = value;
				this.mPriorityTargetHash = this.mPriorityTarget.GetHashCodeCustom();
			}
		}

		// Token: 0x17000496 RID: 1174
		// (set) Token: 0x060011CF RID: 4559 RVA: 0x0006CA10 File Offset: 0x0006AC10
		public int PriorityAbility
		{
			set
			{
				this.mPriorityAbility = value;
			}
		}

		// Token: 0x17000497 RID: 1175
		// (get) Token: 0x060011D0 RID: 4560 RVA: 0x0006CA19 File Offset: 0x0006AC19
		// (set) Token: 0x060011D1 RID: 4561 RVA: 0x0006CA21 File Offset: 0x0006AC21
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

		// Token: 0x17000498 RID: 1176
		// (get) Token: 0x060011D2 RID: 4562 RVA: 0x0006CA36 File Offset: 0x0006AC36
		// (set) Token: 0x060011D3 RID: 4563 RVA: 0x0006CA3E File Offset: 0x0006AC3E
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

		// Token: 0x04001079 RID: 4217
		private string mName = "CloneWizard";

		// Token: 0x0400107A RID: 4218
		private CharacterTemplate mTemplate;

		// Token: 0x0400107B RID: 4219
		private XmlNode mNode;

		// Token: 0x0400107C RID: 4220
		private List<SpawnCloneWizards.HandleAndHealth> mSpawnedEntityHP = new List<SpawnCloneWizards.HandleAndHealth>(4);

		// Token: 0x0400107D RID: 4221
		private bool mSnapToNavMesh;

		// Token: 0x0400107E RID: 4222
		private string[] mArea = new string[4];

		// Token: 0x0400107F RID: 4223
		private int[] mAreaID = new int[4];

		// Token: 0x04001080 RID: 4224
		private int mTypeID;

		// Token: 0x04001081 RID: 4225
		private int mAmount;

		// Token: 0x04001082 RID: 4226
		private string mUniqueName;

		// Token: 0x04001083 RID: 4227
		private int mUniqueID;

		// Token: 0x04001084 RID: 4228
		private string mDialog;

		// Token: 0x04001085 RID: 4229
		private int mDialogID;

		// Token: 0x04001086 RID: 4230
		private float mHealth = 1f;

		// Token: 0x04001087 RID: 4231
		private Order mOrder = Order.Attack;

		// Token: 0x04001088 RID: 4232
		private Order mReaction = Order.Attack;

		// Token: 0x04001089 RID: 4233
		private ReactTo mReactTo = ReactTo.Attack | ReactTo.Proximity;

		// Token: 0x0400108A RID: 4234
		private new string mTrigger;

		// Token: 0x0400108B RID: 4235
		private int mTriggerID;

		// Token: 0x0400108C RID: 4236
		private AIEvent[] mEvents;

		// Token: 0x0400108D RID: 4237
		private Animations mSpawnAnimation;

		// Token: 0x0400108E RID: 4238
		private Animations mSpecialIdleAnimation;

		// Token: 0x0400108F RID: 4239
		private bool mNeverRemove;

		// Token: 0x04001090 RID: 4240
		private string mTargetArea;

		// Token: 0x04001091 RID: 4241
		private int mTargetAreaID;

		// Token: 0x04001092 RID: 4242
		private float mWanderSpeed = 1f;

		// Token: 0x04001093 RID: 4243
		private string mPriorityTarget;

		// Token: 0x04001094 RID: 4244
		private int mPriorityTargetHash;

		// Token: 0x04001095 RID: 4245
		private int mPriorityAbility = -1;

		// Token: 0x04001096 RID: 4246
		private string mOnDeathTrigger;

		// Token: 0x04001097 RID: 4247
		private int mOnDeathTriggerID;

		// Token: 0x04001098 RID: 4248
		private string mOnDamageTrigger;

		// Token: 0x04001099 RID: 4249
		private int mOnDamageTriggerID;

		// Token: 0x02000240 RID: 576
		private struct HandleAndHealth
		{
			// Token: 0x060011D4 RID: 4564 RVA: 0x0006CA53 File Offset: 0x0006AC53
			public HandleAndHealth(int iHandle, float iHealth)
			{
				this.Handle = iHandle;
				this.Health = iHealth;
			}

			// Token: 0x0400109A RID: 4250
			public int Handle;

			// Token: 0x0400109B RID: 4251
			public float Health;
		}
	}
}
