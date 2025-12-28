using System;
using System.Collections.Generic;
using Magicka.Audio;
using Magicka.GameLogic.GameStates;
using Magicka.Graphics;
using Magicka.Network;
using Microsoft.Xna.Framework;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x020005B8 RID: 1464
	public class RandomTeleportOther : SpecialAbility
	{
		// Token: 0x06002BC6 RID: 11206 RVA: 0x00159F1E File Offset: 0x0015811E
		public RandomTeleportOther(Animations iAnimation) : base(iAnimation, "#specab_randomteleportother".GetHashCodeCustom())
		{
		}

		// Token: 0x06002BC7 RID: 11207 RVA: 0x00159F31 File Offset: 0x00158131
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			throw new Exception("RandomTeleport must be called by an entity!");
		}

		// Token: 0x06002BC8 RID: 11208 RVA: 0x00159F40 File Offset: 0x00158140
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			base.Execute(iOwner, iPlayState);
			if (!(iOwner is Character))
			{
				return false;
			}
			Vector3 position = iOwner.Position;
			Vector3 direction = iOwner.Direction;
			List<Entity> entities = iPlayState.EntityManager.GetEntities(position, 14f, true);
			Character character = null;
			float num = float.MaxValue;
			for (int i = 0; i < entities.Count; i++)
			{
				if (entities[i] is Character && entities[i] != iOwner)
				{
					Vector3 position2 = entities[i].Position;
					Vector3 vector;
					Vector3.Subtract(ref position2, ref position, out vector);
					vector.Y = 0f;
					vector.Normalize();
					float num2 = MagickaMath.Angle(ref direction, ref vector);
					if (num2 < num)
					{
						num = num2;
						character = (entities[i] as Character);
					}
				}
			}
			iPlayState.EntityManager.ReturnEntityList(entities);
			if (character == null)
			{
				AudioManager.Instance.PlayCue(Banks.Spells, SpecialAbility.SOUND_MAGICK_FAIL, iOwner.AudioEmitter);
				return false;
			}
			Vector3 position3 = character.Position;
			float num3 = (float)Math.Pow(SpecialAbility.RANDOM.NextDouble(), 0.25);
			float num4 = (float)SpecialAbility.RANDOM.NextDouble() * 6.2831855f;
			float num5 = (float)((double)num3 * Math.Cos((double)num4));
			float num6 = (float)((double)num3 * Math.Sin((double)num4));
			position3.X += 20f * num5;
			position3.Z += 20f * num6;
			Vector3 position4 = character.Position;
			Vector3.Subtract(ref position3, ref position4, out position4);
			position4.Y = 0f;
			position4.Normalize();
			Vector3 right = Vector3.Right;
			Vector3 translation = iOwner.CastSource.Translation;
			VisualEffectReference visualEffectReference;
			EffectManager.Instance.StartEffect(RandomTeleportOther.EFFECT, ref translation, ref right, out visualEffectReference);
			return NetworkManager.Instance.State == NetworkState.Client || Teleport.Instance.DoTeleport(character, position3, position4, Teleport.TeleportType.Regular);
		}

		// Token: 0x04002F82 RID: 12162
		private const float RADIUS = 14f;

		// Token: 0x04002F83 RID: 12163
		public static readonly int EFFECT = "horrible_staff".GetHashCodeCustom();
	}
}
