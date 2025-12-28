using System;
using Magicka.GameLogic.Spells;
using Microsoft.Xna.Framework;
using PolygonHead;

namespace Magicka.GameLogic.Entities.Bosses
{
	// Token: 0x02000036 RID: 54
	public interface IBoss
	{
		// Token: 0x06000227 RID: 551
		BossEnum GetBossType();

		// Token: 0x06000228 RID: 552
		void Initialize(ref Matrix iOrientation);

		// Token: 0x06000229 RID: 553
		void Initialize(ref Matrix iOrientation, int iUniqueID);

		// Token: 0x0600022A RID: 554
		void DeInitialize();

		// Token: 0x0600022B RID: 555
		bool AddImpulseVelocity(Vector3 iDirection, float iElevation, float iMassPower, float iDistance);

		// Token: 0x17000074 RID: 116
		// (get) Token: 0x0600022C RID: 556
		bool Dead { get; }

		// Token: 0x17000075 RID: 117
		// (get) Token: 0x0600022D RID: 557
		float MaxHitPoints { get; }

		// Token: 0x17000076 RID: 118
		// (get) Token: 0x0600022E RID: 558
		float HitPoints { get; }

		// Token: 0x0600022F RID: 559
		void UpdateBoss(DataChannel iDataChannel, float iDeltaTime, bool iFightStarted);

		// Token: 0x06000230 RID: 560
		DamageResult Damage(int iPartIndex, Damage iDamage, Entity iAttacker, ref Vector3 iAttackPosition, Defines.DamageFeatures iFeatures);

		// Token: 0x06000231 RID: 561
		void Damage(int iPartIndex, float iDamage, Elements iElement);

		// Token: 0x06000232 RID: 562
		void SetSlow(int iIndex);

		// Token: 0x06000233 RID: 563
		bool HasStatus(int iIndex, StatusEffects iStatus);

		// Token: 0x06000234 RID: 564
		float StatusMagnitude(int iIndex, StatusEffects iStatus);

		// Token: 0x06000235 RID: 565
		StatusEffect[] GetStatusEffects();

		// Token: 0x06000236 RID: 566
		float ResistanceAgainst(Elements iElement);

		// Token: 0x06000237 RID: 567
		void ScriptMessage(BossMessages iMessage);

		// Token: 0x06000238 RID: 568
		void NetworkUpdate(ref BossUpdateMessage iMsg);

		// Token: 0x06000239 RID: 569
		void NetworkInitialize(ref BossInitializeMessage iMsg);

		// Token: 0x17000077 RID: 119
		// (get) Token: 0x0600023A RID: 570
		bool NetworkInitialized { get; }
	}
}
