using System;
using System.IO;
using Magicka.GameLogic.Entities;
using Microsoft.Xna.Framework;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x0200038F RID: 911
	public sealed class SpawnAnimProp : Action
	{
		// Token: 0x170006DC RID: 1756
		// (get) Token: 0x06001BE3 RID: 7139 RVA: 0x000BF362 File Offset: 0x000BD562
		// (set) Token: 0x06001BE4 RID: 7140 RVA: 0x000BF36A File Offset: 0x000BD56A
		public string Area
		{
			get
			{
				return this.mArea;
			}
			set
			{
				this.mArea = value;
				this.mAreaId = this.mArea.GetHashCodeCustom();
			}
		}

		// Token: 0x170006DD RID: 1757
		// (get) Token: 0x06001BE5 RID: 7141 RVA: 0x000BF384 File Offset: 0x000BD584
		// (set) Token: 0x06001BE6 RID: 7142 RVA: 0x000BF38C File Offset: 0x000BD58C
		public string Object
		{
			get
			{
				return this.mObject;
			}
			set
			{
				this.mObject = value;
			}
		}

		// Token: 0x170006DE RID: 1758
		// (get) Token: 0x06001BE7 RID: 7143 RVA: 0x000BF395 File Offset: 0x000BD595
		// (set) Token: 0x06001BE8 RID: 7144 RVA: 0x000BF39D File Offset: 0x000BD59D
		public string OnDeath
		{
			get
			{
				return this.mOnDeath;
			}
			set
			{
				this.mOnDeath = value;
				this.mOnDeathId = this.mOnDeath.GetHashCodeCustom();
			}
		}

		// Token: 0x170006DF RID: 1759
		// (get) Token: 0x06001BE9 RID: 7145 RVA: 0x000BF3B7 File Offset: 0x000BD5B7
		// (set) Token: 0x06001BEA RID: 7146 RVA: 0x000BF3BF File Offset: 0x000BD5BF
		public string OnDamage
		{
			get
			{
				return this.mOnDamage;
			}
			set
			{
				this.mOnDamage = value;
				this.mOnDamageId = this.mOnDamage.GetHashCodeCustom();
			}
		}

		// Token: 0x170006E0 RID: 1760
		// (get) Token: 0x06001BEB RID: 7147 RVA: 0x000BF3D9 File Offset: 0x000BD5D9
		// (set) Token: 0x06001BEC RID: 7148 RVA: 0x000BF3E1 File Offset: 0x000BD5E1
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

		// Token: 0x170006E1 RID: 1761
		// (get) Token: 0x06001BED RID: 7149 RVA: 0x000BF3FB File Offset: 0x000BD5FB
		// (set) Token: 0x06001BEE RID: 7150 RVA: 0x000BF404 File Offset: 0x000BD604
		public string Animation
		{
			get
			{
				return this.mAnimationStr;
			}
			set
			{
				this.mAnimationStr = value;
				if (!string.IsNullOrEmpty(this.mAnimationStr))
				{
					this.mAnimationStr = this.mAnimationStr.Trim().ToLower();
				}
				if (!string.IsNullOrEmpty(this.mAnimationStr))
				{
					try
					{
						this.mAnimation = (Animations)Enum.Parse(typeof(Animations), this.mAnimationStr);
						return;
					}
					catch (ArgumentException)
					{
						this.mAnimation = Animations.None;
						return;
					}
				}
				this.mAnimation = Animations.None;
			}
		}

		// Token: 0x170006E2 RID: 1762
		// (set) Token: 0x06001BEF RID: 7151 RVA: 0x000BF48C File Offset: 0x000BD68C
		public string Damageable
		{
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					this.mIsDamageable = false;
					return;
				}
				string text = value.Trim();
				if (string.IsNullOrEmpty(text))
				{
					this.mIsDamageable = false;
					return;
				}
				text = text.ToLower();
				if (string.Compare(text, "true") == 0)
				{
					this.mIsDamageable = true;
					return;
				}
				if (string.Compare(text, "false") == 0)
				{
					this.mIsDamageable = false;
					return;
				}
				this.mIsDamageable = false;
			}
		}

		// Token: 0x170006E3 RID: 1763
		// (set) Token: 0x06001BF0 RID: 7152 RVA: 0x000BF4FC File Offset: 0x000BD6FC
		public string Gravity
		{
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					this.mIsDamageable = false;
					return;
				}
				string text = value.Trim();
				if (string.IsNullOrEmpty(text))
				{
					this.mIsAffectedByGravity = false;
					return;
				}
				text = text.ToLower();
				if (string.Compare(text, "true") == 0)
				{
					this.mIsAffectedByGravity = true;
					return;
				}
				if (string.Compare(text, "false") == 0)
				{
					this.mIsAffectedByGravity = false;
					return;
				}
				this.mIsAffectedByGravity = false;
			}
		}

		// Token: 0x06001BF1 RID: 7153 RVA: 0x000BF56A File Offset: 0x000BD76A
		public SpawnAnimProp(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x06001BF2 RID: 7154 RVA: 0x000BF588 File Offset: 0x000BD788
		public override void Initialize()
		{
			base.Initialize();
			this.mTemplate = base.GameScene.PlayState.Content.Load<PhysicsEntityTemplate>("Data/AnimatedProps/" + this.mObject);
			if (this.mTemplate.AnimationClips == null || this.mTemplate.AnimationClips.Length == 0 || this.mTemplate.Skeleton == null)
			{
				if (this.mTemplate.MaxHitpoints > 0)
				{
					this.mEntity = new DamageablePhysicsEntity(base.GameScene.PlayState);
				}
				else
				{
					this.mEntity = new PhysicsEntity(base.GameScene.PlayState);
				}
			}
			else
			{
				this.mEntity = new AnimatedPhysicsEntity(base.GameScene.PlayState);
				(this.mEntity as AnimatedPhysicsEntity).IsDamageable = this.mIsDamageable;
				(this.mEntity as AnimatedPhysicsEntity).IsAffectedByGravity = this.mIsAffectedByGravity;
			}
			if (this.mTemplate.MaxHitpoints > 0 && this.mIsDamageable)
			{
				this.mSpawnedEntityHP = new SpawnAnimProp.HandleAndHealth((int)this.mEntity.Handle, (float)this.mTemplate.MaxHitpoints);
			}
		}

		// Token: 0x06001BF3 RID: 7155 RVA: 0x000BF6A8 File Offset: 0x000BD8A8
		protected override void Execute()
		{
			Matrix iStartTransform;
			base.GameScene.GetLocator(this.mAreaId, out iStartTransform);
			this.mEntity.Initialize(this.mTemplate, iStartTransform, this.mUniqueID);
			DamageablePhysicsEntity damageablePhysicsEntity = this.mEntity as DamageablePhysicsEntity;
			if (damageablePhysicsEntity != null)
			{
				damageablePhysicsEntity.OnDeath = this.mOnDeathId;
				damageablePhysicsEntity.OnDamage = this.mOnDamageId;
			}
			if (this.mEntity is AnimatedPhysicsEntity)
			{
				(this.mEntity as AnimatedPhysicsEntity).ForceAnimation(this.mAnimation);
			}
			base.GameScene.PlayState.EntityManager.AddEntity(this.mEntity);
		}

		// Token: 0x06001BF4 RID: 7156 RVA: 0x000BF745 File Offset: 0x000BD945
		public override void QuickExecute()
		{
		}

		// Token: 0x06001BF5 RID: 7157 RVA: 0x000BF748 File Offset: 0x000BD948
		public override void Update(float iDeltaTime)
		{
			AnimatedPhysicsEntity animatedPhysicsEntity = this.mEntity as AnimatedPhysicsEntity;
			if (animatedPhysicsEntity != null)
			{
				this.mSpawnedEntityHP.Health = animatedPhysicsEntity.HitPoints;
			}
			base.Update(iDeltaTime);
		}

		// Token: 0x170006E4 RID: 1764
		// (get) Token: 0x06001BF6 RID: 7158 RVA: 0x000BF77C File Offset: 0x000BD97C
		// (set) Token: 0x06001BF7 RID: 7159 RVA: 0x000BF789 File Offset: 0x000BD989
		protected override object Tag
		{
			get
			{
				return this.mSpawnedEntityHP;
			}
			set
			{
				this.mSpawnedEntityHP = (SpawnAnimProp.HandleAndHealth)value;
			}
		}

		// Token: 0x06001BF8 RID: 7160 RVA: 0x000BF797 File Offset: 0x000BD997
		protected override void WriteTag(BinaryWriter iWriter, object mTag)
		{
			iWriter.Write(((SpawnAnimProp.HandleAndHealth)mTag).Health);
		}

		// Token: 0x06001BF9 RID: 7161 RVA: 0x000BF7AA File Offset: 0x000BD9AA
		protected override object ReadTag(BinaryReader iReader)
		{
			return new SpawnAnimProp.HandleAndHealth(65535, iReader.ReadSingle());
		}

		// Token: 0x04001E30 RID: 7728
		private SpawnAnimProp.HandleAndHealth mSpawnedEntityHP;

		// Token: 0x04001E31 RID: 7729
		private string mArea;

		// Token: 0x04001E32 RID: 7730
		private int mAreaId;

		// Token: 0x04001E33 RID: 7731
		private string mObject;

		// Token: 0x04001E34 RID: 7732
		private string mOnDeath;

		// Token: 0x04001E35 RID: 7733
		private int mOnDeathId;

		// Token: 0x04001E36 RID: 7734
		private string mOnDamage;

		// Token: 0x04001E37 RID: 7735
		private int mOnDamageId;

		// Token: 0x04001E38 RID: 7736
		private string mUniqueName;

		// Token: 0x04001E39 RID: 7737
		private int mUniqueID;

		// Token: 0x04001E3A RID: 7738
		private PhysicsEntity mEntity;

		// Token: 0x04001E3B RID: 7739
		private PhysicsEntityTemplate mTemplate;

		// Token: 0x04001E3C RID: 7740
		private string mAnimationStr = "";

		// Token: 0x04001E3D RID: 7741
		private Animations mAnimation;

		// Token: 0x04001E3E RID: 7742
		private bool mIsDamageable;

		// Token: 0x04001E3F RID: 7743
		private bool mIsAffectedByGravity = true;

		// Token: 0x02000390 RID: 912
		private struct HandleAndHealth
		{
			// Token: 0x06001BFA RID: 7162 RVA: 0x000BF7C1 File Offset: 0x000BD9C1
			public HandleAndHealth(int iHandle, float iHealth)
			{
				this.Handle = iHandle;
				this.Health = iHealth;
			}

			// Token: 0x04001E40 RID: 7744
			public int Handle;

			// Token: 0x04001E41 RID: 7745
			public float Health;
		}
	}
}
