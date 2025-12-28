using System;
using Magicka.GameLogic.GameStates;
using Microsoft.Xna.Framework;
using PolygonHead;

namespace Magicka.GameLogic.Entities.Bosses
{
	// Token: 0x020001D1 RID: 465
	internal class PropBoss : DamageablePhysicsEntity, IBoss
	{
		// Token: 0x06000FCB RID: 4043 RVA: 0x00062238 File Offset: 0x00060438
		public PropBoss(PlayState iPlayState, string iType, int iUniqueID, int iOnDeathId, int iOnDamageId) : base(iPlayState)
		{
			this.mType = iType;
			this.mUniqueID = iUniqueID;
			this.mOnDeathId = iOnDeathId;
			this.mOnDamageId = iOnDamageId;
			this.mTemplate = this.mPlayState.Content.Load<PhysicsEntityTemplate>("Data/PhysicsEntities/" + this.mType);
		}

		// Token: 0x06000FCC RID: 4044 RVA: 0x00062290 File Offset: 0x00060490
		public void Initialize(ref Matrix iOrientation, int iUniqueID)
		{
			this.Initialize(ref iOrientation);
		}

		// Token: 0x06000FCD RID: 4045 RVA: 0x0006229C File Offset: 0x0006049C
		public void Initialize(ref Matrix iOrientation)
		{
			base.Initialize(this.mTemplate, iOrientation, this.mUniqueID);
			if (this.mOnDeathId != 0)
			{
				base.OnDeath = this.mOnDeathId;
			}
			if (this.mOnDeathId != 0)
			{
				base.OnDamage = this.mOnDamageId;
			}
			this.mPlayState.EntityManager.AddEntity(this);
		}

		// Token: 0x06000FCE RID: 4046 RVA: 0x000622FA File Offset: 0x000604FA
		public void DeInitialize()
		{
		}

		// Token: 0x06000FCF RID: 4047 RVA: 0x000622FC File Offset: 0x000604FC
		public void ScriptMessage(BossMessages iMessage)
		{
		}

		// Token: 0x06000FD0 RID: 4048 RVA: 0x000622FE File Offset: 0x000604FE
		public void UpdateBoss(DataChannel iDataChannel, float iDeltaTime, bool iFightStarted)
		{
		}

		// Token: 0x06000FD1 RID: 4049 RVA: 0x00062300 File Offset: 0x00060500
		public DamageResult Damage(int iPartIndex, Damage iDamage, Entity iAttacker, ref Vector3 iAttackPosition, Defines.DamageFeatures iFeatures)
		{
			return DamageResult.None;
		}

		// Token: 0x06000FD2 RID: 4050 RVA: 0x00062303 File Offset: 0x00060503
		public void Damage(int iPartIndex, float iDamage, Elements iElement)
		{
		}

		// Token: 0x06000FD3 RID: 4051 RVA: 0x00062305 File Offset: 0x00060505
		public void SetSlow(int iIndex)
		{
		}

		// Token: 0x06000FD4 RID: 4052 RVA: 0x00062307 File Offset: 0x00060507
		public void GetRandomPositionOnCollisionSkin(int iIndex, out Vector3 oPosition)
		{
			oPosition = default(Vector3);
		}

		// Token: 0x06000FD5 RID: 4053 RVA: 0x00062310 File Offset: 0x00060510
		public bool HasStatus(int iIndex, StatusEffects iStatus)
		{
			return false;
		}

		// Token: 0x06000FD6 RID: 4054 RVA: 0x00062313 File Offset: 0x00060513
		public float StatusMagnitude(int iIndex, StatusEffects iStatus)
		{
			return 0f;
		}

		// Token: 0x06000FD7 RID: 4055 RVA: 0x0006231A File Offset: 0x0006051A
		public void NetworkUpdate(ref BossUpdateMessage iMsg)
		{
		}

		// Token: 0x06000FD8 RID: 4056 RVA: 0x0006231C File Offset: 0x0006051C
		public void NetworkInitialize(ref BossInitializeMessage iMsg)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000FD9 RID: 4057 RVA: 0x00062323 File Offset: 0x00060523
		public BossEnum GetBossType()
		{
			return BossEnum.Generic;
		}

		// Token: 0x17000415 RID: 1045
		// (get) Token: 0x06000FDA RID: 4058 RVA: 0x00062326 File Offset: 0x00060526
		public bool NetworkInitialized
		{
			get
			{
				return true;
			}
		}

		// Token: 0x04000E45 RID: 3653
		private string mType;

		// Token: 0x04000E46 RID: 3654
		private int mOnDeathId;

		// Token: 0x04000E47 RID: 3655
		private int mOnDamageId;
	}
}
