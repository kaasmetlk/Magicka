using System;
using Magicka.GameLogic.GameStates;
using Magicka.Gamers;
using Magicka.Network;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x0200045C RID: 1116
	public class Votal : SpecialAbility, ITargetAbility
	{
		// Token: 0x1700082E RID: 2094
		// (get) Token: 0x06002211 RID: 8721 RVA: 0x000F4494 File Offset: 0x000F2694
		public static Votal Instance
		{
			get
			{
				if (Votal.mSingelton == null)
				{
					lock (Votal.mSingeltonLock)
					{
						if (Votal.mSingelton == null)
						{
							Votal.mSingelton = new Votal();
						}
					}
				}
				return Votal.mSingelton;
			}
		}

		// Token: 0x06002212 RID: 8722 RVA: 0x000F44E8 File Offset: 0x000F26E8
		public Votal(Animations iAnimation) : base(iAnimation, "#magick_votal".GetHashCodeCustom())
		{
		}

		// Token: 0x06002213 RID: 8723 RVA: 0x000F44FB File Offset: 0x000F26FB
		private Votal() : base(Animations.cast_magick_self, "#magick_votal".GetHashCodeCustom())
		{
		}

		// Token: 0x06002214 RID: 8724 RVA: 0x000F4510 File Offset: 0x000F2710
		public bool Execute(ISpellCaster iOwner, Entity iTarget, PlayState iPlayState)
		{
			if (!(iTarget is Character))
			{
				return false;
			}
			NetworkState state = NetworkManager.Instance.State;
			if ((state != NetworkState.Client && (!(iOwner is Avatar) || !((iOwner as Avatar).Player.Gamer is NetworkGamer))) || (state == NetworkState.Client && iOwner is Avatar && !((iOwner as Avatar).Player.Gamer is NetworkGamer)))
			{
				Damage iDamage = default(Damage);
				iDamage.Amount = 50f;
				iDamage.AttackProperty = AttackProperties.Status;
				iDamage.Element = Elements.None;
				iDamage.Magnitude = 1f;
				switch (SpecialAbility.RANDOM.Next(7))
				{
				case 0:
					iDamage.Element = Elements.Poison;
					break;
				case 1:
					iDamage.Element = Elements.Water;
					break;
				case 2:
					iDamage.AttackProperty = AttackProperties.Knockdown;
					break;
				case 3:
				{
					Damage iDamage2 = default(Damage);
					iDamage2.Amount = 1f;
					iDamage2.AttackProperty = AttackProperties.Status;
					iDamage2.Element = Elements.Water;
					iDamage2.Magnitude = 1f;
					(iTarget as Character).Damage(iDamage2, iOwner as Entity, 0.0, iOwner.Position);
					iDamage.Element = Elements.Cold;
					break;
				}
				case 4:
					iDamage.Element = Elements.Cold;
					break;
				case 5:
					iDamage.Amount = 30f;
					iDamage.AttackProperty = AttackProperties.Bleed;
					break;
				case 6:
					iDamage.Element = Elements.Fire;
					break;
				}
				(iTarget as Character).Damage(iDamage, iOwner as Entity, 0.0, iOwner.Position);
			}
			return true;
		}

		// Token: 0x0400252E RID: 9518
		private static Votal mSingelton;

		// Token: 0x0400252F RID: 9519
		private static volatile object mSingeltonLock = new object();
	}
}
