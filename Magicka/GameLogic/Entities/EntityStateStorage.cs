using System;
using System.Collections.Generic;
using System.IO;
using Magicka.GameLogic.Entities.Items;
using Magicka.GameLogic.GameStates;

namespace Magicka.GameLogic.Entities
{
	// Token: 0x02000438 RID: 1080
	internal class EntityStateStorage
	{
		// Token: 0x06002176 RID: 8566 RVA: 0x000EEF0F File Offset: 0x000ED10F
		public EntityStateStorage(PlayState iPlayState)
		{
			this.mPlayState = iPlayState;
		}

		// Token: 0x06002177 RID: 8567 RVA: 0x000EEF34 File Offset: 0x000ED134
		public void Read(BinaryReader iReader)
		{
			this.mPickableStates.Clear();
			int num = iReader.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				this.mPickableStates.Add(new Pickable.State(iReader));
			}
			this.mPhysicsStates.Clear();
			num = iReader.ReadInt32();
			for (int j = 0; j < num; j++)
			{
				this.mPhysicsStates.Add(new PhysicsEntity.State(iReader));
			}
		}

		// Token: 0x06002178 RID: 8568 RVA: 0x000EEFA0 File Offset: 0x000ED1A0
		public void Write(BinaryWriter iWriter)
		{
			iWriter.Write(this.mPickableStates.Count);
			for (int i = 0; i < this.mPickableStates.Count; i++)
			{
				this.mPickableStates[i].Write(iWriter);
			}
			iWriter.Write(this.mPhysicsStates.Count);
			for (int j = 0; j < this.mPhysicsStates.Count; j++)
			{
				this.mPhysicsStates[j].Write(iWriter);
			}
		}

		// Token: 0x06002179 RID: 8569 RVA: 0x000EF028 File Offset: 0x000ED228
		public void Store(IEnumerable<Entity> iEntity)
		{
			foreach (Entity iEntity2 in iEntity)
			{
				this.Store(iEntity2);
			}
		}

		// Token: 0x0600217A RID: 8570 RVA: 0x000EF070 File Offset: 0x000ED270
		public void Store(Entity iEntity)
		{
			Pickable pickable = iEntity as Pickable;
			PhysicsEntity physicsEntity = iEntity as PhysicsEntity;
			if (pickable != null)
			{
				if (pickable.IsPickable && pickable.Permanent)
				{
					this.mPickableStates.Add(new Pickable.State(pickable));
					return;
				}
			}
			else if (physicsEntity != null && !physicsEntity.Dead)
			{
				this.mPhysicsStates.Add(new PhysicsEntity.State(physicsEntity));
			}
		}

		// Token: 0x0600217B RID: 8571 RVA: 0x000EF0CC File Offset: 0x000ED2CC
		public void Restore(List<Entity> iTarget)
		{
			foreach (Pickable.State state in this.mPickableStates)
			{
				Pickable pickable = state.Restore(this.mPlayState);
				if (pickable is Item)
				{
					(pickable as Item).Detach();
				}
				pickable.Body.DisableBody();
				iTarget.Add(pickable);
			}
			foreach (PhysicsEntity.State state2 in this.mPhysicsStates)
			{
				PhysicsEntity physicsEntity = state2.ApplyTo(this.mPlayState);
				physicsEntity.Body.DisableBody();
				iTarget.Add(physicsEntity);
			}
		}

		// Token: 0x0600217C RID: 8572 RVA: 0x000EF1A8 File Offset: 0x000ED3A8
		public void Clear()
		{
			this.mPickableStates.Clear();
			this.mPhysicsStates.Clear();
		}

		// Token: 0x04002440 RID: 9280
		private List<Pickable.State> mPickableStates = new List<Pickable.State>();

		// Token: 0x04002441 RID: 9281
		private List<PhysicsEntity.State> mPhysicsStates = new List<PhysicsEntity.State>();

		// Token: 0x04002442 RID: 9282
		private PlayState mPlayState;
	}
}
