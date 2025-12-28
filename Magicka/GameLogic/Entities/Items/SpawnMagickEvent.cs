using System;
using Magicka.GameLogic.Spells;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Magicka.GameLogic.Entities.Items
{
	// Token: 0x020004A6 RID: 1190
	public struct SpawnMagickEvent
	{
		// Token: 0x06002403 RID: 9219 RVA: 0x001028E5 File Offset: 0x00100AE5
		public SpawnMagickEvent(MagickType iType)
		{
			this.Type = iType;
			this.DespawnTime = 0f;
		}

		// Token: 0x06002404 RID: 9220 RVA: 0x001028F9 File Offset: 0x00100AF9
		public SpawnMagickEvent(MagickType iType, float iDespawnTime)
		{
			this.Type = iType;
			this.DespawnTime = iDespawnTime;
		}

		// Token: 0x06002405 RID: 9221 RVA: 0x0010290C File Offset: 0x00100B0C
		public SpawnMagickEvent(ContentReader iInput)
		{
			this.DespawnTime = 0f;
			string text = iInput.ReadString().ToLowerInvariant();
			if (text.Equals("random", StringComparison.InvariantCultureIgnoreCase))
			{
				this.Type = MagickType.Revive;
				return;
			}
			this.Type = (MagickType)Enum.Parse(typeof(MagickType), text, true);
		}

		// Token: 0x06002406 RID: 9222 RVA: 0x00102964 File Offset: 0x00100B64
		public void Execute(Entity iItem, Entity iTarget)
		{
			if (NetworkManager.Instance.State == NetworkState.Client)
			{
				return;
			}
			BookOfMagick instance = BookOfMagick.GetInstance(iItem.PlayState);
			Matrix orientation = iItem.Body.Orientation;
			Vector3 position = iItem.Position;
			position.Y += iItem.Radius;
			Vector3 vector = new Vector3((float)(MagickaMath.Random.NextDouble() - 0.5) * 3f, (float)MagickaMath.Random.NextDouble() * 7f, (float)(MagickaMath.Random.NextDouble() - 0.5) * 3f);
			instance.Initialize(position, orientation, this.Type, false, vector, this.DespawnTime, 0);
			iItem.PlayState.EntityManager.AddEntity(instance);
			if (NetworkManager.Instance.State == NetworkState.Server)
			{
				TriggerActionMessage triggerActionMessage = default(TriggerActionMessage);
				triggerActionMessage.ActionType = TriggerActionType.SpawnMagick;
				triggerActionMessage.Handle = instance.Handle;
				triggerActionMessage.Template = (int)this.Type;
				triggerActionMessage.Position = instance.Position;
				triggerActionMessage.Direction = vector;
				triggerActionMessage.Time = this.DespawnTime;
				triggerActionMessage.Point0 = 0;
				triggerActionMessage.Bool0 = false;
				Quaternion.CreateFromRotationMatrix(ref orientation, out triggerActionMessage.Orientation);
				NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref triggerActionMessage);
			}
		}

		// Token: 0x0400270B RID: 9995
		public MagickType Type;

		// Token: 0x0400270C RID: 9996
		public float DespawnTime;
	}
}
