using System;
using Magicka.GameLogic.GameStates;
using Microsoft.Xna.Framework;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x0200045E RID: 1118
	public class Regenerate : SpecialAbility
	{
		// Token: 0x17000830 RID: 2096
		// (get) Token: 0x06002223 RID: 8739 RVA: 0x000F4ACC File Offset: 0x000F2CCC
		public static Regenerate Instance
		{
			get
			{
				if (Regenerate.sSingelton == null)
				{
					lock (Regenerate.sSingeltonLock)
					{
						if (Regenerate.sSingelton == null)
						{
							Regenerate.sSingelton = new Regenerate();
						}
					}
				}
				return Regenerate.sSingelton;
			}
		}

		// Token: 0x06002224 RID: 8740 RVA: 0x000F4B20 File Offset: 0x000F2D20
		private Regenerate() : base(Animations.None, "#specab_drain".GetHashCodeCustom())
		{
		}

		// Token: 0x06002225 RID: 8741 RVA: 0x000F4B3E File Offset: 0x000F2D3E
		public Regenerate(Animations iAnimation) : base(iAnimation, "#specab_drain".GetHashCodeCustom())
		{
		}

		// Token: 0x06002226 RID: 8742 RVA: 0x000F4B5C File Offset: 0x000F2D5C
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			throw new Exception("Regenerate cannot be spawned without an owner!");
		}

		// Token: 0x06002227 RID: 8743 RVA: 0x000F4B68 File Offset: 0x000F2D68
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			base.Execute(iOwner, iPlayState);
			iOwner.Damage((float)(-(float)this.HITPOINTS), Elements.Life);
			return true;
		}

		// Token: 0x0400253D RID: 9533
		private static Regenerate sSingelton;

		// Token: 0x0400253E RID: 9534
		private static volatile object sSingeltonLock = new object();

		// Token: 0x0400253F RID: 9535
		private int HITPOINTS = 13000;
	}
}
