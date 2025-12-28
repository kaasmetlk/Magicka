using System;
using Magicka.GameLogic.Entities.Buffs;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Microsoft.Xna.Framework;
using PolygonHead;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x02000587 RID: 1415
	public class DeflectionAura : SpecialAbility, IAbilityEffect
	{
		// Token: 0x06002A45 RID: 10821 RVA: 0x0014BFB8 File Offset: 0x0014A1B8
		public DeflectionAura(Animations iAnimation) : base(iAnimation, "#specab_deflect".GetHashCodeCustom())
		{
			this.mSphere = new BoundingSphere(default(Vector3), 5f);
		}

		// Token: 0x06002A46 RID: 10822 RVA: 0x0014BFF0 File Offset: 0x0014A1F0
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			base.Execute(iOwner, iPlayState);
			this.mOwner = iOwner;
			this.mSphere.Radius = 5f;
			this.mSphere.Center = iOwner.Position;
			this.mPlayState = iPlayState;
			AuraDeflect iAura = new AuraDeflect(5f);
			AuraStorage auraStorage = new AuraStorage(iAura, AuraTarget.Self, AuraType.Deflect, DeflectionAura.EFFECT_HASH, 8f, 5f, VisualCategory.Special, Spell.ARCANECOLOR, null, Factions.NONE);
			(iOwner as Character).AddAura(ref auraStorage, false);
			return true;
		}

		// Token: 0x06002A47 RID: 10823 RVA: 0x0014C076 File Offset: 0x0014A276
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			throw new Exception();
		}

		// Token: 0x170009F1 RID: 2545
		// (get) Token: 0x06002A48 RID: 10824 RVA: 0x0014C07D File Offset: 0x0014A27D
		public bool IsDead
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06002A49 RID: 10825 RVA: 0x0014C080 File Offset: 0x0014A280
		public void Update(DataChannel iDataChannel, float iDeltaTime)
		{
		}

		// Token: 0x06002A4A RID: 10826 RVA: 0x0014C082 File Offset: 0x0014A282
		public void OnRemove()
		{
		}

		// Token: 0x04002DA4 RID: 11684
		public static readonly int EFFECT_HASH = "deflection_aura".GetHashCodeCustom();

		// Token: 0x04002DA5 RID: 11685
		public static readonly int SOUND_HASH = "".GetHashCodeCustom();

		// Token: 0x04002DA6 RID: 11686
		private BoundingSphere mSphere;

		// Token: 0x04002DA7 RID: 11687
		private ISpellCaster mOwner;

		// Token: 0x04002DA8 RID: 11688
		private PlayState mPlayState;
	}
}
