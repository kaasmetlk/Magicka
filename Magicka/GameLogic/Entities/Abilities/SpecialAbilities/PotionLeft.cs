using System;
using Magicka.GameLogic.GameStates;
using Magicka.Gamers;
using Magicka.Network;
using Microsoft.Xna.Framework;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x02000405 RID: 1029
	internal class PotionLeft : Potion
	{
		// Token: 0x06001FB1 RID: 8113 RVA: 0x000DE66F File Offset: 0x000DC86F
		public PotionLeft(Animations iAnimation) : base(Animations.special0)
		{
		}

		// Token: 0x06001FB2 RID: 8114 RVA: 0x000DE67C File Offset: 0x000DC87C
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			throw new Exception("Potion have to be cast by a character!");
		}

		// Token: 0x06001FB3 RID: 8115 RVA: 0x000DE688 File Offset: 0x000DC888
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			base.Execute(iOwner, iPlayState);
			NetworkState state = NetworkManager.Instance.State;
			if ((state != NetworkState.Client && (!(iOwner is Avatar) || !((iOwner as Avatar).Player.Gamer is NetworkGamer))) || (state == NetworkState.Client && iOwner is Avatar && !((iOwner as Avatar).Player.Gamer is NetworkGamer)))
			{
				MissileEntity missileInstance = iOwner.GetMissileInstance();
				Vector3 translation = iOwner.CastSource.Translation;
				if (iOwner is Character)
				{
					translation = (iOwner as Character).GetLeftAttachOrientation().Translation;
				}
				Vector3 direction = iOwner.Direction;
				direction.Y = 1f;
				direction.Normalize();
				Vector3 up = Vector3.Up;
				Vector3 vector;
				Vector3.Cross(ref direction, ref up, out vector);
				vector.Normalize();
				float num = (float)(5.0 + (0.5 + MagickaMath.Random.NextDouble()) * 10.0 / 2.0);
				Vector3 vector2;
				Vector3.Multiply(ref vector, -num, out vector2);
				Potion.SpawnFlask(ref missileInstance, iOwner, ref translation, ref vector2);
				if (NetworkManager.Instance.State != NetworkState.Offline)
				{
					SpawnMissileMessage spawnMissileMessage = default(SpawnMissileMessage);
					spawnMissileMessage.Type = SpawnMissileMessage.MissileType.PotionFlask;
					spawnMissileMessage.Handle = missileInstance.Handle;
					spawnMissileMessage.Item = 0;
					spawnMissileMessage.Owner = iOwner.Handle;
					spawnMissileMessage.Position = translation;
					spawnMissileMessage.Velocity = direction;
					NetworkManager.Instance.Interface.SendMessage<SpawnMissileMessage>(ref spawnMissileMessage);
				}
				return true;
			}
			return false;
		}
	}
}
