using System;
using System.Collections.Generic;
using System.Xml;
using Magicka.AI;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.Entities.Bosses;
using Magicka.GameLogic.Entities.CharacterStates;
using Microsoft.Xna.Framework;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x02000628 RID: 1576
	public class SpawnGenericBoss : Action
	{
		// Token: 0x06002F4A RID: 12106 RVA: 0x00180A1C File Offset: 0x0017EC1C
		public SpawnGenericBoss(Trigger iTrigger, GameScene iScene, XmlNode iNode) : base(iTrigger, iScene)
		{
			this.mNode = iNode;
		}

		// Token: 0x06002F4B RID: 12107 RVA: 0x00180A7C File Offset: 0x0017EC7C
		public override void Initialize()
		{
			base.Initialize();
			this.mBossRef = new GenericBoss(base.GameScene.PlayState, this.mTypeID, this.mUniqueID, this.mMeshIdx);
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

		// Token: 0x06002F4C RID: 12108 RVA: 0x00180B60 File Offset: 0x0017ED60
		protected override void Execute()
		{
			BossFight instance = BossFight.Instance;
			if (!instance.IsSetup)
			{
				BossFight.Instance.Setup(base.GameScene.PlayState, this.mFreezeTime, this.mHealthAppearDelay, this.mHealthBarWidth);
			}
			Matrix matrix;
			base.GameScene.GetLocator(this.mAreaID, out matrix);
			instance.Initialize(this.mBossRef, this.mAreaID, this.mUniqueID);
			if (this.mForceDraw)
			{
				this.mBossRef.ForceDraw();
			}
			this.mBossRef.HitPoints = this.mBossRef.MaxHitPoints * this.mHealth;
			this.mBossRef.OnDeathTrigger = this.mOnDeathTriggerID;
			this.mBossRef.OnDamageTrigger = this.mOnDamageTriggerID;
			Agent ai = this.mBossRef.AI;
			ai.WanderSpeed = this.mWanderSpeed;
			ai.TargetArea = this.mTargetAreaID;
			ai.SetOrder(this.mOrder, this.mReactTo, this.mReaction, this.mPriorityTargetHash, this.mPriorityAbility, this.mTriggerID, this.mEvents);
			this.mBossRef.Dialog = this.mDialogID;
			Matrix orientation = matrix;
			orientation.Translation = default(Vector3);
			this.mBossRef.CharacterBody.Orientation = orientation;
			this.mBossRef.CharacterBody.DesiredDirection = orientation.Forward;
			this.mBossRef.RemoveAfterDeath = !this.mNeverRemove;
			this.mBossRef.ForceCamera = this.mForceCamera;
			this.mBossRef.ForceNavMesh = this.mForceNavMesh;
			this.mBossRef.CannotDieWithoutExplicitKill = this.mCannotDieWithoutExplicitKill;
			if (this.mAnimation != Animations.None)
			{
				this.mBossRef.ForceAnimation(this.mAnimation);
			}
			if (this.mSpawnAnimation != Animations.None && this.mSpawnAnimation != Animations.idle && this.mSpawnAnimation != Animations.idle_agg)
			{
				this.mBossRef.SpawnAnimation = this.mSpawnAnimation;
				this.mBossRef.ChangeState(RessurectionState.Instance);
			}
			if (this.mSpecialIdleAnimation != Animations.None)
			{
				this.mBossRef.SpecialIdleAnimation = this.mSpecialIdleAnimation;
				if (!(this.mBossRef.CurrentState is RessurectionState))
				{
					this.mBossRef.ForceAnimation(this.mSpecialIdleAnimation);
				}
			}
			if (base.GameScene.RuleSet != null && base.GameScene.RuleSet is SurvivalRuleset)
			{
				this.mBossRef.Faction = Factions.EVIL;
				(base.GameScene.RuleSet as SurvivalRuleset).AddedCharacter(this.mBossRef, false);
			}
			if (!this.mDelayed)
			{
				instance.Start();
			}
		}

		// Token: 0x06002F4D RID: 12109 RVA: 0x00180DEC File Offset: 0x0017EFEC
		public override void QuickExecute()
		{
		}

		// Token: 0x17000B27 RID: 2855
		// (get) Token: 0x06002F4E RID: 12110 RVA: 0x00180DEE File Offset: 0x0017EFEE
		// (set) Token: 0x06002F4F RID: 12111 RVA: 0x00180DF6 File Offset: 0x0017EFF6
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

		// Token: 0x17000B28 RID: 2856
		// (get) Token: 0x06002F50 RID: 12112 RVA: 0x00180E10 File Offset: 0x0017F010
		// (set) Token: 0x06002F51 RID: 12113 RVA: 0x00180E18 File Offset: 0x0017F018
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

		// Token: 0x17000B29 RID: 2857
		// (get) Token: 0x06002F52 RID: 12114 RVA: 0x00180E46 File Offset: 0x0017F046
		// (set) Token: 0x06002F53 RID: 12115 RVA: 0x00180E4E File Offset: 0x0017F04E
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

		// Token: 0x17000B2A RID: 2858
		// (get) Token: 0x06002F54 RID: 12116 RVA: 0x00180E57 File Offset: 0x0017F057
		// (set) Token: 0x06002F55 RID: 12117 RVA: 0x00180E5F File Offset: 0x0017F05F
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

		// Token: 0x17000B2B RID: 2859
		// (get) Token: 0x06002F56 RID: 12118 RVA: 0x00180E79 File Offset: 0x0017F079
		// (set) Token: 0x06002F57 RID: 12119 RVA: 0x00180E81 File Offset: 0x0017F081
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

		// Token: 0x17000B2C RID: 2860
		// (get) Token: 0x06002F58 RID: 12120 RVA: 0x00180EAF File Offset: 0x0017F0AF
		// (set) Token: 0x06002F59 RID: 12121 RVA: 0x00180EB7 File Offset: 0x0017F0B7
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

		// Token: 0x17000B2D RID: 2861
		// (get) Token: 0x06002F5A RID: 12122 RVA: 0x00180ED1 File Offset: 0x0017F0D1
		// (set) Token: 0x06002F5B RID: 12123 RVA: 0x00180ED9 File Offset: 0x0017F0D9
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

		// Token: 0x17000B2E RID: 2862
		// (get) Token: 0x06002F5C RID: 12124 RVA: 0x00180EF3 File Offset: 0x0017F0F3
		// (set) Token: 0x06002F5D RID: 12125 RVA: 0x00180EFB File Offset: 0x0017F0FB
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

		// Token: 0x17000B2F RID: 2863
		// (get) Token: 0x06002F5E RID: 12126 RVA: 0x00180F04 File Offset: 0x0017F104
		// (set) Token: 0x06002F5F RID: 12127 RVA: 0x00180F0C File Offset: 0x0017F10C
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

		// Token: 0x17000B30 RID: 2864
		// (get) Token: 0x06002F60 RID: 12128 RVA: 0x00180F15 File Offset: 0x0017F115
		// (set) Token: 0x06002F61 RID: 12129 RVA: 0x00180F1D File Offset: 0x0017F11D
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

		// Token: 0x17000B31 RID: 2865
		// (get) Token: 0x06002F62 RID: 12130 RVA: 0x00180F26 File Offset: 0x0017F126
		// (set) Token: 0x06002F63 RID: 12131 RVA: 0x00180F2E File Offset: 0x0017F12E
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

		// Token: 0x17000B32 RID: 2866
		// (get) Token: 0x06002F64 RID: 12132 RVA: 0x00180F48 File Offset: 0x0017F148
		// (set) Token: 0x06002F65 RID: 12133 RVA: 0x00180F50 File Offset: 0x0017F150
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

		// Token: 0x17000B33 RID: 2867
		// (get) Token: 0x06002F66 RID: 12134 RVA: 0x00180F59 File Offset: 0x0017F159
		// (set) Token: 0x06002F67 RID: 12135 RVA: 0x00180F61 File Offset: 0x0017F161
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

		// Token: 0x17000B34 RID: 2868
		// (get) Token: 0x06002F68 RID: 12136 RVA: 0x00180F6A File Offset: 0x0017F16A
		// (set) Token: 0x06002F69 RID: 12137 RVA: 0x00180F72 File Offset: 0x0017F172
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

		// Token: 0x17000B35 RID: 2869
		// (get) Token: 0x06002F6A RID: 12138 RVA: 0x00180F7B File Offset: 0x0017F17B
		// (set) Token: 0x06002F6B RID: 12139 RVA: 0x00180F83 File Offset: 0x0017F183
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

		// Token: 0x17000B36 RID: 2870
		// (set) Token: 0x06002F6C RID: 12140 RVA: 0x00180F8C File Offset: 0x0017F18C
		public string PriorityTarget
		{
			set
			{
				this.mPriorityTarget = value;
				this.mPriorityTargetHash = this.mPriorityTarget.GetHashCodeCustom();
			}
		}

		// Token: 0x17000B37 RID: 2871
		// (set) Token: 0x06002F6D RID: 12141 RVA: 0x00180FA6 File Offset: 0x0017F1A6
		public int PriorityAbility
		{
			set
			{
				this.mPriorityAbility = value;
			}
		}

		// Token: 0x17000B38 RID: 2872
		// (get) Token: 0x06002F6E RID: 12142 RVA: 0x00180FAF File Offset: 0x0017F1AF
		// (set) Token: 0x06002F6F RID: 12143 RVA: 0x00180FB7 File Offset: 0x0017F1B7
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

		// Token: 0x17000B39 RID: 2873
		// (get) Token: 0x06002F70 RID: 12144 RVA: 0x00180FCC File Offset: 0x0017F1CC
		// (set) Token: 0x06002F71 RID: 12145 RVA: 0x00180FD4 File Offset: 0x0017F1D4
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

		// Token: 0x17000B3A RID: 2874
		// (get) Token: 0x06002F72 RID: 12146 RVA: 0x00180FE9 File Offset: 0x0017F1E9
		// (set) Token: 0x06002F73 RID: 12147 RVA: 0x00180FF1 File Offset: 0x0017F1F1
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

		// Token: 0x17000B3B RID: 2875
		// (get) Token: 0x06002F74 RID: 12148 RVA: 0x00180FFA File Offset: 0x0017F1FA
		// (set) Token: 0x06002F75 RID: 12149 RVA: 0x00181002 File Offset: 0x0017F202
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

		// Token: 0x17000B3C RID: 2876
		// (get) Token: 0x06002F76 RID: 12150 RVA: 0x0018100B File Offset: 0x0017F20B
		// (set) Token: 0x06002F77 RID: 12151 RVA: 0x00181013 File Offset: 0x0017F213
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

		// Token: 0x17000B3D RID: 2877
		// (get) Token: 0x06002F78 RID: 12152 RVA: 0x0018101C File Offset: 0x0017F21C
		// (set) Token: 0x06002F79 RID: 12153 RVA: 0x00181024 File Offset: 0x0017F224
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

		// Token: 0x17000B3E RID: 2878
		// (get) Token: 0x06002F7A RID: 12154 RVA: 0x0018102D File Offset: 0x0017F22D
		// (set) Token: 0x06002F7B RID: 12155 RVA: 0x00181035 File Offset: 0x0017F235
		public bool Delayed
		{
			get
			{
				return this.mDelayed;
			}
			set
			{
				this.mDelayed = value;
			}
		}

		// Token: 0x17000B3F RID: 2879
		// (get) Token: 0x06002F7C RID: 12156 RVA: 0x0018103E File Offset: 0x0017F23E
		// (set) Token: 0x06002F7D RID: 12157 RVA: 0x00181046 File Offset: 0x0017F246
		public float HealthAppearDelay
		{
			get
			{
				return this.mHealthAppearDelay;
			}
			set
			{
				this.mHealthAppearDelay = value;
			}
		}

		// Token: 0x17000B40 RID: 2880
		// (get) Token: 0x06002F7E RID: 12158 RVA: 0x0018104F File Offset: 0x0017F24F
		// (set) Token: 0x06002F7F RID: 12159 RVA: 0x00181057 File Offset: 0x0017F257
		public float FreezeTime
		{
			get
			{
				return this.mFreezeTime;
			}
			set
			{
				this.mFreezeTime = value;
			}
		}

		// Token: 0x17000B41 RID: 2881
		// (get) Token: 0x06002F80 RID: 12160 RVA: 0x00181060 File Offset: 0x0017F260
		// (set) Token: 0x06002F81 RID: 12161 RVA: 0x00181068 File Offset: 0x0017F268
		public float HealthBarWidth
		{
			get
			{
				return this.mHealthBarWidth;
			}
			set
			{
				this.mHealthBarWidth = value;
			}
		}

		// Token: 0x17000B42 RID: 2882
		// (get) Token: 0x06002F82 RID: 12162 RVA: 0x00181071 File Offset: 0x0017F271
		// (set) Token: 0x06002F83 RID: 12163 RVA: 0x00181079 File Offset: 0x0017F279
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

		// Token: 0x04003360 RID: 13152
		private XmlNode mNode;

		// Token: 0x04003361 RID: 13153
		private GenericBoss mBossRef;

		// Token: 0x04003362 RID: 13154
		private string mArea;

		// Token: 0x04003363 RID: 13155
		private int mAreaID;

		// Token: 0x04003364 RID: 13156
		private string mType;

		// Token: 0x04003365 RID: 13157
		private int mTypeID;

		// Token: 0x04003366 RID: 13158
		private string mUniqueName;

		// Token: 0x04003367 RID: 13159
		private int mUniqueID;

		// Token: 0x04003368 RID: 13160
		private string mDialog;

		// Token: 0x04003369 RID: 13161
		private int mDialogID;

		// Token: 0x0400336A RID: 13162
		private bool mForceDraw;

		// Token: 0x0400336B RID: 13163
		private float mHealth = 1f;

		// Token: 0x0400336C RID: 13164
		private Order mOrder = Order.Attack;

		// Token: 0x0400336D RID: 13165
		private Order mReaction = Order.Attack;

		// Token: 0x0400336E RID: 13166
		private ReactTo mReactTo = ReactTo.Attack | ReactTo.Proximity;

		// Token: 0x0400336F RID: 13167
		private new string mTrigger;

		// Token: 0x04003370 RID: 13168
		private int mTriggerID;

		// Token: 0x04003371 RID: 13169
		private AIEvent[] mEvents;

		// Token: 0x04003372 RID: 13170
		private Animations mAnimation;

		// Token: 0x04003373 RID: 13171
		private Animations mSpawnAnimation;

		// Token: 0x04003374 RID: 13172
		private Animations mSpecialIdleAnimation;

		// Token: 0x04003375 RID: 13173
		private bool mNeverRemove;

		// Token: 0x04003376 RID: 13174
		private string mTargetArea;

		// Token: 0x04003377 RID: 13175
		private int mTargetAreaID;

		// Token: 0x04003378 RID: 13176
		private float mWanderSpeed = 1f;

		// Token: 0x04003379 RID: 13177
		private string mPriorityTarget;

		// Token: 0x0400337A RID: 13178
		private int mPriorityTargetHash;

		// Token: 0x0400337B RID: 13179
		private int mPriorityAbility = -1;

		// Token: 0x0400337C RID: 13180
		private int mMeshIdx = -1;

		// Token: 0x0400337D RID: 13181
		private string mOnDeathTrigger;

		// Token: 0x0400337E RID: 13182
		private int mOnDeathTriggerID;

		// Token: 0x0400337F RID: 13183
		private string mOnDamageTrigger;

		// Token: 0x04003380 RID: 13184
		private int mOnDamageTriggerID;

		// Token: 0x04003381 RID: 13185
		private bool mForceCamera;

		// Token: 0x04003382 RID: 13186
		private bool mForceNavMesh;

		// Token: 0x04003383 RID: 13187
		private bool mDelayed;

		// Token: 0x04003384 RID: 13188
		private float mHealthAppearDelay;

		// Token: 0x04003385 RID: 13189
		private float mFreezeTime;

		// Token: 0x04003386 RID: 13190
		private float mHealthBarWidth = 0.8f;

		// Token: 0x04003387 RID: 13191
		private bool mCannotDieWithoutExplicitKill;
	}
}
