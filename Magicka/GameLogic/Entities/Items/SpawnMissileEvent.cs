using System;
using Magicka.GameLogic.GameStates;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Magicka.GameLogic.Entities.Items
{
	// Token: 0x020004A7 RID: 1191
	public struct SpawnMissileEvent
	{
		// Token: 0x06002407 RID: 9223 RVA: 0x00102AB0 File Offset: 0x00100CB0
		public SpawnMissileEvent(ContentReader iInput)
		{
			string iString = iInput.ReadString().ToLowerInvariant();
			this.Type = iString.GetHashCodeCustom();
			this.Velocity = iInput.ReadVector3();
			this.Facing = iInput.ReadBoolean();
		}

		// Token: 0x06002408 RID: 9224 RVA: 0x00102AF0 File Offset: 0x00100CF0
		public void Execute(Entity iItem, Entity iTarget)
		{
			if (NetworkManager.Instance.State != NetworkState.Client)
			{
				int type = this.Type;
				Item cachedWeapon = Item.GetCachedWeapon(type);
				MissileEntity missileEntity = null;
				this.SpawnMissile(ref missileEntity, this.Velocity, cachedWeapon, iItem.PlayState, iItem);
				if (missileEntity != null)
				{
					iItem.PlayState.EntityManager.AddEntity(missileEntity);
				}
			}
		}

		// Token: 0x06002409 RID: 9225 RVA: 0x00102B48 File Offset: 0x00100D48
		public void SpawnMissile(ref MissileEntity iMissile, Vector3 iVelocity, Item iItem, PlayState iPlayState, Entity iOwner)
		{
			if (iMissile == null & NetworkManager.Instance.State == NetworkState.Client)
			{
				return;
			}
			if (iMissile == null)
			{
				iMissile = MissileEntity.GetInstance(iPlayState);
			}
			Vector3 vector = iVelocity;
			Vector3 position = iOwner.Position;
			if (iItem.ProjectileModel != null)
			{
				iMissile.Initialize(iOwner, iItem.ProjectileModel.Meshes[0].BoundingSphere.Radius, ref position, ref vector, iItem.ProjectileModel, iItem.RangedConditions, false);
			}
			else
			{
				iMissile.Initialize(iOwner, 0.75f, ref position, ref vector, iItem.ProjectileModel, iItem.RangedConditions, false);
			}
			iMissile.Danger = iItem.Danger;
			iMissile.Homing = iItem.Homing;
			iMissile.FacingVelocity = iItem.Facing;
			if (NetworkManager.Instance.State == NetworkState.Server)
			{
				SpawnMissileMessage spawnMissileMessage = default(SpawnMissileMessage);
				spawnMissileMessage.Type = SpawnMissileMessage.MissileType.Item;
				spawnMissileMessage.Item = iItem.Handle;
				spawnMissileMessage.Owner = iOwner.Handle;
				spawnMissileMessage.Handle = iMissile.Handle;
				spawnMissileMessage.Position = iMissile.Position;
				spawnMissileMessage.Velocity = iMissile.Body.Velocity;
				NetworkManager.Instance.Interface.SendMessage<SpawnMissileMessage>(ref spawnMissileMessage);
			}
		}

		// Token: 0x0400270D RID: 9997
		public bool Facing;

		// Token: 0x0400270E RID: 9998
		public int Type;

		// Token: 0x0400270F RID: 9999
		public Vector3 Velocity;
	}
}
