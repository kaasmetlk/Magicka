using System;
using Magicka.GameLogic.GameStates;
using Magicka.Gamers;
using Magicka.Network;
using Microsoft.Xna.Framework;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x020004DE RID: 1246
	internal class PotionRight : Potion
	{
		// Token: 0x06002505 RID: 9477 RVA: 0x0010B3D9 File Offset: 0x001095D9
		public PotionRight(Animations iAnimation) : base(Animations.special0)
		{
		}

		// Token: 0x06002506 RID: 9478 RVA: 0x0010B3E6 File Offset: 0x001095E6
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			throw new Exception("Potion have to be cast by a character!");
		}

		// Token: 0x06002507 RID: 9479 RVA: 0x0010B3F4 File Offset: 0x001095F4
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
					translation = (iOwner as Character).GetRightAttachOrientation().Translation;
				}
				Vector3 direction = iOwner.Direction;
				direction.Y = 1f;
				direction.Normalize();
				Vector3 up = Vector3.Up;
				Vector3 vector;
				Vector3.Cross(ref direction, ref up, out vector);
				vector.Normalize();
				float scaleFactor = (float)(5.0 + (0.5 + MagickaMath.Random.NextDouble()) * 10.0 / 2.0);
				Vector3.Multiply(ref vector, scaleFactor, out vector);
				Potion.SpawnFlask(ref missileInstance, iOwner, ref translation, ref vector);
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
