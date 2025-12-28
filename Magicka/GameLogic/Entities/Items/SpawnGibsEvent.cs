using System;
using System.Collections.Generic;
using JigLibX.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Magicka.GameLogic.Entities.Items
{
	// Token: 0x0200049E RID: 1182
	public struct SpawnGibsEvent
	{
		// Token: 0x060023D6 RID: 9174 RVA: 0x00101A5F File Offset: 0x000FFC5F
		public SpawnGibsEvent(ContentReader iInput)
		{
			this.StartIndex = iInput.ReadInt32();
			this.EndIndex = iInput.ReadInt32();
		}

		// Token: 0x060023D7 RID: 9175 RVA: 0x00101A7C File Offset: 0x000FFC7C
		public void Execute(Entity iItem, Entity iTarget)
		{
			Character character = iItem as Character;
			if (character != null)
			{
				Body body = character.Body;
				Vector3 position = character.Position;
				List<GibReference> gibs = character.Gibs;
				for (int i = this.StartIndex; i <= this.EndIndex; i++)
				{
					GibReference gibReference = gibs[i];
					Gib fromCache = Gib.GetFromCache();
					if (fromCache != null)
					{
						Vector3 randomPositionOnCollisionSkin = character.GetRandomPositionOnCollisionSkin();
						Vector3 iVelocity;
						Vector3.Subtract(ref randomPositionOnCollisionSkin, ref position, out iVelocity);
						iVelocity.Normalize();
						iVelocity.X *= (float)SpawnGibsEvent.sRandom.NextDouble() * 3f + body.Velocity.X * 0.1f;
						iVelocity.Y *= (float)SpawnGibsEvent.sRandom.NextDouble() * 9f + body.Velocity.Y * 0.1f;
						iVelocity.Z *= (float)SpawnGibsEvent.sRandom.NextDouble() * 3f + body.Velocity.Z * 0.1f;
						fromCache.Initialize(gibReference.mModel, gibReference.mMass, gibReference.mScale, randomPositionOnCollisionSkin, iVelocity, 10f + (float)SpawnGibsEvent.sRandom.NextDouble() * 10f, character, character.BloodType, Gib.GORE_GIB_TRAIL_EFFECTS[(int)character.BloodType], character.HasStatus(StatusEffects.Frozen));
						Vector3 angImpulse = new Vector3((float)(SpawnGibsEvent.sRandom.NextDouble() - 0.5) * fromCache.Mass * 0.5f, (float)(SpawnGibsEvent.sRandom.NextDouble() - 0.5) * fromCache.Mass * 0.5f, (float)(SpawnGibsEvent.sRandom.NextDouble() - 0.5) * fromCache.Mass * 0.5f);
						fromCache.Body.ApplyBodyAngImpulse(angImpulse);
						character.PlayState.EntityManager.AddEntity(fromCache);
					}
				}
			}
		}

		// Token: 0x040026EB RID: 9963
		private static Random sRandom = new Random();

		// Token: 0x040026EC RID: 9964
		public int StartIndex;

		// Token: 0x040026ED RID: 9965
		public int EndIndex;
	}
}
