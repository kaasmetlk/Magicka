using System;
using Magicka.GameLogic.GameStates;
using Magicka.Network;
using Microsoft.Xna.Framework;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x020000E2 RID: 226
	public class RandomTeleport : SpecialAbility
	{
		// Token: 0x060006FC RID: 1788 RVA: 0x000293CD File Offset: 0x000275CD
		public RandomTeleport(Animations iAnimation) : base(iAnimation, "#specab_eteleport".GetHashCodeCustom())
		{
		}

		// Token: 0x060006FD RID: 1789 RVA: 0x000293E0 File Offset: 0x000275E0
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			throw new Exception("RandomTeleport must be called by an entity!");
		}

		// Token: 0x060006FE RID: 1790 RVA: 0x000293EC File Offset: 0x000275EC
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			if (NetworkManager.Instance.State == NetworkState.Client)
			{
				return true;
			}
			base.Execute(iOwner, iPlayState);
			Vector3 position = iOwner.Position;
			float num = (float)Math.Pow(SpecialAbility.RANDOM.NextDouble(), 0.25);
			float num2 = (float)SpecialAbility.RANDOM.NextDouble() * 6.2831855f;
			float num3 = (float)((double)num * Math.Cos((double)num2));
			float num4 = (float)((double)num * Math.Sin((double)num2));
			position.X += 20f * num3;
			position.Z += 20f * num4;
			Vector3 position2 = iOwner.Position;
			Vector3.Subtract(ref position, ref position2, out position2);
			position2.Y = 0f;
			position2.Normalize();
			return Teleport.Instance.DoTeleport(iOwner, position, position2, Teleport.TeleportType.Regular);
		}
	}
}
