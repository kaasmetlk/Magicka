using System;
using Magicka.GameLogic.GameStates;
using Microsoft.Xna.Framework;
using PolygonHead;

namespace Magicka.GameLogic.Entities.Bosses
{
	// Token: 0x020001CA RID: 458
	public class GenericBoss : NonPlayerCharacter, IBoss
	{
		// Token: 0x06000F74 RID: 3956 RVA: 0x00060841 File Offset: 0x0005EA41
		public GenericBoss(PlayState iPlayState, int iType, int iUniqueID, int iMeshIdx) : base(iPlayState)
		{
			this.mType = iType;
			this.mUniqueID = iUniqueID;
			this.mMeshIdx = iMeshIdx;
		}

		// Token: 0x06000F75 RID: 3957 RVA: 0x00060860 File Offset: 0x0005EA60
		public void Initialize(ref Matrix iOrientation, int iUniqueID)
		{
			this.Initialize(ref iOrientation);
		}

		// Token: 0x06000F76 RID: 3958 RVA: 0x0006086C File Offset: 0x0005EA6C
		public void Initialize(ref Matrix iOrientation)
		{
			base.Initialize(CharacterTemplate.GetCachedTemplate(this.mType), this.mMeshIdx, iOrientation.Translation, this.mUniqueID);
			base.CharacterBody.DesiredDirection = iOrientation.Forward;
			this.mPlayState.EntityManager.AddEntity(this);
		}

		// Token: 0x06000F77 RID: 3959 RVA: 0x000608BE File Offset: 0x0005EABE
		public void DeInitialize()
		{
		}

		// Token: 0x06000F78 RID: 3960 RVA: 0x000608C0 File Offset: 0x0005EAC0
		public void ScriptMessage(BossMessages iMessage)
		{
		}

		// Token: 0x06000F79 RID: 3961 RVA: 0x000608C2 File Offset: 0x0005EAC2
		public void UpdateBoss(DataChannel iDataChannel, float iDeltaTime, bool iFightStarted)
		{
		}

		// Token: 0x06000F7A RID: 3962 RVA: 0x000608C4 File Offset: 0x0005EAC4
		public DamageResult Damage(int iPartIndex, Damage iDamage, Entity iAttacker, ref Vector3 iAttackPosition, Defines.DamageFeatures iFeatures)
		{
			return DamageResult.None;
		}

		// Token: 0x06000F7B RID: 3963 RVA: 0x000608C7 File Offset: 0x0005EAC7
		public void Damage(int iPartIndex, float iDamage, Elements iElement)
		{
		}

		// Token: 0x06000F7C RID: 3964 RVA: 0x000608C9 File Offset: 0x0005EAC9
		public void SetSlow(int iIndex)
		{
		}

		// Token: 0x06000F7D RID: 3965 RVA: 0x000608CB File Offset: 0x0005EACB
		public void GetRandomPositionOnCollisionSkin(int iIndex, out Vector3 oPosition)
		{
			oPosition = default(Vector3);
		}

		// Token: 0x06000F7E RID: 3966 RVA: 0x000608D4 File Offset: 0x0005EAD4
		public bool HasStatus(int iIndex, StatusEffects iStatus)
		{
			return false;
		}

		// Token: 0x06000F7F RID: 3967 RVA: 0x000608D7 File Offset: 0x0005EAD7
		public float StatusMagnitude(int iIndex, StatusEffects iStatus)
		{
			return 0f;
		}

		// Token: 0x06000F80 RID: 3968 RVA: 0x000608DE File Offset: 0x0005EADE
		public void NetworkInitialize(ref BossInitializeMessage iMsg)
		{
		}

		// Token: 0x06000F81 RID: 3969 RVA: 0x000608E0 File Offset: 0x0005EAE0
		public void NetworkUpdate(ref BossUpdateMessage iMsg)
		{
		}

		// Token: 0x06000F82 RID: 3970 RVA: 0x000608E2 File Offset: 0x0005EAE2
		public BossEnum GetBossType()
		{
			return BossEnum.Generic;
		}

		// Token: 0x17000402 RID: 1026
		// (get) Token: 0x06000F83 RID: 3971 RVA: 0x000608E5 File Offset: 0x0005EAE5
		public bool NetworkInitialized
		{
			get
			{
				return true;
			}
		}

		// Token: 0x04000E14 RID: 3604
		private new int mType;

		// Token: 0x04000E15 RID: 3605
		private int mMeshIdx;
	}
}
