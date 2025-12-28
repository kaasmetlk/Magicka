using System;
using JigLibX.Geometry;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Magicka.GameLogic.Entities.Items
{
	// Token: 0x020004A5 RID: 1189
	public struct SpawnItemEvent
	{
		// Token: 0x060023FE RID: 9214 RVA: 0x0010269D File Offset: 0x0010089D
		public SpawnItemEvent(int iType)
		{
			this.Type = iType;
			this.DespawnTime = 0f;
		}

		// Token: 0x060023FF RID: 9215 RVA: 0x001026B1 File Offset: 0x001008B1
		public SpawnItemEvent(int iType, float iDespawnTime)
		{
			this.Type = iType;
			this.DespawnTime = iDespawnTime;
		}

		// Token: 0x06002400 RID: 9216 RVA: 0x001026C4 File Offset: 0x001008C4
		public SpawnItemEvent(ContentReader iInput)
		{
			string text = iInput.ReadString().ToLowerInvariant();
			this.Type = text.GetHashCodeCustom();
			if (this.Type != SpawnItemEvent.RANDOM)
			{
				Item.CacheWeapon(this.Type, iInput.ContentManager.Load<Item>("Data/Items/Wizard/" + text));
			}
			this.DespawnTime = 0f;
		}

		// Token: 0x06002401 RID: 9217 RVA: 0x00102724 File Offset: 0x00100924
		public void Execute(Entity iItem, Entity iTarget)
		{
			if (NetworkManager.Instance.State == NetworkState.Client || this.Type == SpawnItemEvent.RANDOM)
			{
				return;
			}
			Item pickableIntstance = Item.GetPickableIntstance();
			Item.GetCachedWeapon(this.Type, pickableIntstance);
			pickableIntstance.Body.SetActive();
			pickableIntstance.Detach();
			Vector3 position = iItem.Position;
			position.Y += (pickableIntstance.Body.CollisionSkin.GetPrimitiveLocal(0) as Box).SideLengths.Y;
			Matrix orientation = iItem.Body.Orientation;
			pickableIntstance.Body.MoveTo(position, orientation);
			Vector3 vector = new Vector3((float)(MagickaMath.Random.NextDouble() - 0.5) * 2f, (float)MagickaMath.Random.NextDouble() * 7f, (float)(MagickaMath.Random.NextDouble() - 0.5) * 2f);
			pickableIntstance.Body.Velocity = vector;
			if (this.DespawnTime > 0f)
			{
				pickableIntstance.Despawn(this.DespawnTime);
			}
			iItem.PlayState.EntityManager.AddEntity(pickableIntstance);
			pickableIntstance.Body.EnableBody();
			if (NetworkManager.Instance.State == NetworkState.Server)
			{
				TriggerActionMessage triggerActionMessage = default(TriggerActionMessage);
				triggerActionMessage.ActionType = TriggerActionType.SpawnItem;
				triggerActionMessage.Handle = pickableIntstance.Handle;
				triggerActionMessage.Template = this.Type;
				triggerActionMessage.Position = pickableIntstance.Position;
				triggerActionMessage.Direction = vector;
				triggerActionMessage.Bool0 = true;
				triggerActionMessage.Point0 = 0;
				triggerActionMessage.Time = this.DespawnTime;
				Quaternion.CreateFromRotationMatrix(ref orientation, out triggerActionMessage.Orientation);
				NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref triggerActionMessage);
			}
		}

		// Token: 0x04002708 RID: 9992
		private static int RANDOM = "random".GetHashCodeCustom();

		// Token: 0x04002709 RID: 9993
		public int Type;

		// Token: 0x0400270A RID: 9994
		public float DespawnTime;
	}
}
