using System;
using Magicka.GameLogic.GameStates;
using Magicka.Network;
using Microsoft.Xna.Framework;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x0200058B RID: 1419
	public class SmokeBomb : SpecialAbility
	{
		// Token: 0x06002A60 RID: 10848 RVA: 0x0014D09E File Offset: 0x0014B29E
		public SmokeBomb(Animations iAnimation) : base(iAnimation, "#item_specab_smoke".GetHashCodeCustom())
		{
		}

		// Token: 0x06002A61 RID: 10849 RVA: 0x0014D0B1 File Offset: 0x0014B2B1
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			throw new Exception("SmokeBomb must be called by an entity!");
		}

		// Token: 0x06002A62 RID: 10850 RVA: 0x0014D0C0 File Offset: 0x0014B2C0
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			if (NetworkManager.Instance.State == NetworkState.Client)
			{
				return true;
			}
			base.Execute(iOwner, iPlayState);
			Vector3 position = iOwner.Position;
			Matrix matrix = Matrix.CreateRotationY(((float)SpecialAbility.RANDOM.NextDouble() - 0.5f) * 0.7853982f);
			Vector3 direction = iOwner.Direction;
			Vector3 vector;
			Vector3.Negate(ref direction, out vector);
			Vector3.Transform(ref vector, ref matrix, out vector);
			Vector3.Negate(ref vector, out direction);
			Vector3.Multiply(ref vector, 10f, out vector);
			Vector3.Add(ref vector, ref position, out position);
			return Teleport.Instance.DoTeleport(iOwner, position, direction, Teleport.TeleportType.SmokeBomb);
		}
	}
}
