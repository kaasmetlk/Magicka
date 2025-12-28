using System;
using System.IO;
using Magicka.GameLogic.Entities;
using Microsoft.Xna.Framework;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x0200059A RID: 1434
	public class SpawnPhys : Action
	{
		// Token: 0x06002AC3 RID: 10947 RVA: 0x00151527 File Offset: 0x0014F727
		public SpawnPhys(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x06002AC4 RID: 10948 RVA: 0x00151534 File Offset: 0x0014F734
		public override void Initialize()
		{
			base.Initialize();
			this.mTemplate = base.GameScene.PlayState.Content.Load<PhysicsEntityTemplate>("Data/PhysicsEntities/" + this.mObject);
			if (this.mTemplate.MaxHitpoints > 0)
			{
				this.mEntity = new DamageablePhysicsEntity(base.GameScene.PlayState);
				this.mSpawnedEntityHP = new SpawnPhys.HandleAndHealth((int)this.mEntity.Handle, (float)this.mTemplate.MaxHitpoints);
				return;
			}
			this.mEntity = new PhysicsEntity(base.GameScene.PlayState);
		}

		// Token: 0x06002AC5 RID: 10949 RVA: 0x001515D0 File Offset: 0x0014F7D0
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
			base.GameScene.PlayState.EntityManager.AddEntity(this.mEntity);
		}

		// Token: 0x06002AC6 RID: 10950 RVA: 0x0015164A File Offset: 0x0014F84A
		public override void QuickExecute()
		{
		}

		// Token: 0x06002AC7 RID: 10951 RVA: 0x0015164C File Offset: 0x0014F84C
		public override void Update(float iDeltaTime)
		{
			DamageablePhysicsEntity damageablePhysicsEntity = this.mEntity as DamageablePhysicsEntity;
			if (damageablePhysicsEntity != null)
			{
				this.mSpawnedEntityHP.Health = damageablePhysicsEntity.HitPoints;
			}
			base.Update(iDeltaTime);
		}

		// Token: 0x17000A02 RID: 2562
		// (get) Token: 0x06002AC8 RID: 10952 RVA: 0x00151680 File Offset: 0x0014F880
		// (set) Token: 0x06002AC9 RID: 10953 RVA: 0x0015168D File Offset: 0x0014F88D
		protected override object Tag
		{
			get
			{
				return this.mSpawnedEntityHP;
			}
			set
			{
				this.mSpawnedEntityHP = (SpawnPhys.HandleAndHealth)value;
			}
		}

		// Token: 0x06002ACA RID: 10954 RVA: 0x0015169B File Offset: 0x0014F89B
		protected override void WriteTag(BinaryWriter iWriter, object mTag)
		{
			iWriter.Write(((SpawnPhys.HandleAndHealth)mTag).Health);
		}

		// Token: 0x06002ACB RID: 10955 RVA: 0x001516AE File Offset: 0x0014F8AE
		protected override object ReadTag(BinaryReader iReader)
		{
			return new SpawnPhys.HandleAndHealth(65535, iReader.ReadSingle());
		}

		// Token: 0x17000A03 RID: 2563
		// (get) Token: 0x06002ACC RID: 10956 RVA: 0x001516C5 File Offset: 0x0014F8C5
		// (set) Token: 0x06002ACD RID: 10957 RVA: 0x001516CD File Offset: 0x0014F8CD
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

		// Token: 0x17000A04 RID: 2564
		// (get) Token: 0x06002ACE RID: 10958 RVA: 0x001516E7 File Offset: 0x0014F8E7
		// (set) Token: 0x06002ACF RID: 10959 RVA: 0x001516EF File Offset: 0x0014F8EF
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

		// Token: 0x17000A05 RID: 2565
		// (get) Token: 0x06002AD0 RID: 10960 RVA: 0x001516F8 File Offset: 0x0014F8F8
		// (set) Token: 0x06002AD1 RID: 10961 RVA: 0x00151700 File Offset: 0x0014F900
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

		// Token: 0x17000A06 RID: 2566
		// (get) Token: 0x06002AD2 RID: 10962 RVA: 0x0015171A File Offset: 0x0014F91A
		// (set) Token: 0x06002AD3 RID: 10963 RVA: 0x00151722 File Offset: 0x0014F922
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

		// Token: 0x17000A07 RID: 2567
		// (get) Token: 0x06002AD4 RID: 10964 RVA: 0x0015173C File Offset: 0x0014F93C
		// (set) Token: 0x06002AD5 RID: 10965 RVA: 0x00151744 File Offset: 0x0014F944
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

		// Token: 0x04002E14 RID: 11796
		private SpawnPhys.HandleAndHealth mSpawnedEntityHP;

		// Token: 0x04002E15 RID: 11797
		private string mArea;

		// Token: 0x04002E16 RID: 11798
		private int mAreaId;

		// Token: 0x04002E17 RID: 11799
		private string mObject;

		// Token: 0x04002E18 RID: 11800
		private string mOnDeath;

		// Token: 0x04002E19 RID: 11801
		private int mOnDeathId;

		// Token: 0x04002E1A RID: 11802
		private string mOnDamage;

		// Token: 0x04002E1B RID: 11803
		private int mOnDamageId;

		// Token: 0x04002E1C RID: 11804
		private string mUniqueName;

		// Token: 0x04002E1D RID: 11805
		private int mUniqueID;

		// Token: 0x04002E1E RID: 11806
		private PhysicsEntity mEntity;

		// Token: 0x04002E1F RID: 11807
		private PhysicsEntityTemplate mTemplate;

		// Token: 0x0200059B RID: 1435
		private struct HandleAndHealth
		{
			// Token: 0x06002AD6 RID: 10966 RVA: 0x0015175E File Offset: 0x0014F95E
			public HandleAndHealth(int iHandle, float iHealth)
			{
				this.Handle = iHandle;
				this.Health = iHealth;
			}

			// Token: 0x04002E20 RID: 11808
			public int Handle;

			// Token: 0x04002E21 RID: 11809
			public float Health;
		}
	}
}
